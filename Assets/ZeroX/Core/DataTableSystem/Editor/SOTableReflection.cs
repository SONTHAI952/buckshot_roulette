using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZeroX.DataTableSystem.SoTableSystem;

namespace ZeroX.DataTableSystem.Editors
{
    public static class SOTableReflection
    {
        private const BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const string soTableNameSpace = "ZeroX.DataTableSystem.SoTableSystem";
        private const string soTableName = "SoTable`2";




        
        #region Invoke Method

        public static void Invoke_Editor_ImportFromCsv(SoTable soTable, List<string> listField, List<List<string>> listCsvRow)
        {
            var soTableGenericType = GetSOTableGenericType(soTable);
            var methodInfo = soTableGenericType.GetMethod("Editor_ImportFromCsv", bindingFlags);
            methodInfo.Invoke(soTable, new object[] {listField, listCsvRow});
        }

        public static string Invoke_EditorSO_GenerateRowName(SoTable soTable, object row, int rowIndex)
        {
            var soTableGenericType = GetSOTableGenericType(soTable);
            var methodInfo = soTableGenericType.GetMethod("EditorSO_GenerateRowName", bindingFlags);
            return (string)methodInfo.Invoke(soTable, new object[] {row, rowIndex});
        }

        #endregion
        
        
        
        
        
        public static Type GetSOTableGenericType(SoTable soTable)
        {
            var genericTypeDefinition = typeof(SoTable<,>);
            Type resultType = soTable.GetType();

            while (true)
            {
                if(resultType.IsGenericType && resultType.GetGenericTypeDefinition() == genericTypeDefinition)
                    break;
                
                resultType = resultType.BaseType;
                if (resultType == null)
                {
                    Debug.LogError("Không tìm thấy class SOTable generic từ class của Table: " + soTable.GetType().Name);
                    return null;
                }
            }

            return resultType;
        }
        
        public static Type GetDeclaredRowType(SoTable soTable)
        {
            var soTableGenericType = GetSOTableGenericType(soTable);
            return soTableGenericType.GenericTypeArguments[1];
        }

        
        
        public static bool IsFieldExportable(FieldInfo fieldInfo)
        {
            //IsSerializable sẽ không chính xác đối với các kiểu như Vector3
            // if (fieldInfo.FieldType.IsSerializable == false) //Nếu type không thể Serializable thì false
            // {
            //     Debug.Log("Filed ko Serializable: " + fieldInfo.Name);
            //     return false;
            // }
            
            if(fieldInfo.GetCustomAttributes().Any(a => a is NonSerializedAttribute)) //Nếu field có đánh dấu NonSerialized thì false
                return false;
            
            if (fieldInfo.IsPublic)
                return true;
            
            if (fieldInfo.GetCustomAttributes().Any(a => a is SerializeField) == false) //Nếu private và không có SerializeField thì bỏ qua 
                return false;

            return true;
        }

        public static IList GetListRow(SoTable soTable)
        {
            var soTableGenericType = GetSOTableGenericType(soTable);
            
            var listRowFI = soTableGenericType.GetField("listRow", BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return (IList)listRowFI.GetValue(soTable);
        }




        
        #region Get List Field
        
        /// <summary>
        /// Hàm này không chỉ tìm fieldInfo của rowType truyền vào mà còn cả các fieldInfo của các baseRowType
        /// </summary>
        private static List<FieldInfo> GetListFieldInfoOfRow(Type rowType)
        {
            //Tạo danh sách rowType từ rowType truyền vào cho đến hết baseType
            List<Type> listRowType = new List<Type>();
            listRowType.Add(rowType);

            while (true)
            {
                rowType = rowType.BaseType;
                if (rowType == null)
                    break;
                
                if (rowType == typeof(ScriptableObject) || rowType == typeof(System.Object))
                    break;
                
                listRowType.Add(rowType);
            }

            listRowType.Reverse(); //Đảo ngược lại để các field sắp xếp từ baseRowType thấp nhất đến cao nhất
            
            
            
            //Bắt đầu tìm các fieldInfo
            List<FieldInfo> listFieldInfo = new List<FieldInfo>();
            foreach (var rt in listRowType)
            {
                var fieldInfos = rt.GetFields(BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                fieldInfos = fieldInfos.OrderBy(f => f.MetadataToken).ToArray();
                
                //Filter and add
                foreach (var fieldInfo in fieldInfos)
                {
                    if(IsFieldExportable(fieldInfo))
                        listFieldInfo.Add(fieldInfo);
                }
            }
            
            
            return listFieldInfo;
        }

        /// <summary>
        /// Hàm này sẽ lấy declaredRowType(là type khai báo khi tạo class Table) và cả các rowType kế thừa trong listRow
        /// </summary>
        private static List<Type> GetListRowTypeOfSoTable(SoTable soTable)
        {
            //Đầu tiên cần tạo ra danh sách rowType có trong soTable
            var declaredRowType = GetDeclaredRowType(soTable);
            List<Type> listRowType = new List<Type>();
            HashSet<Type> hashSetRowType = new HashSet<Type>();
            
            //Declared row type
            listRowType.Add(declaredRowType);
            hashSetRowType.Add(declaredRowType);
            
            
            //Các row type của các row trong listRow. Vì có thể có row kế thừa
            var listRow = GetListRow(soTable);
            foreach (object row in listRow)
            {
                if (row == null)
                    continue;

                var rowType = row.GetType();
                if (hashSetRowType.Add(rowType))
                    listRowType.Add(rowType);
            }

            return listRowType;
        }

        /// <summary>
        /// Bao gồm cả các field của các row kế thừa trong table
        /// </summary>
        public static List<FieldInfo> GetListFieldInfoOfRow(SoTable soTable)
        {
            List<FieldInfo> listFieldInfo = new List<FieldInfo>();
            HashSet<FieldInfo> hashSetFieldInfo = new HashSet<FieldInfo>();
            
            var listRowType = GetListRowTypeOfSoTable(soTable);
            foreach (var rowType in listRowType)
            {
                var fis = GetListFieldInfoOfRow(rowType);
                foreach (var fieldInfo in fis)
                {
                    if(hashSetFieldInfo.Add(fieldInfo))
                        listFieldInfo.Add(fieldInfo);
                }
            }

            return listFieldInfo;
        }
        
        #endregion
    }
}