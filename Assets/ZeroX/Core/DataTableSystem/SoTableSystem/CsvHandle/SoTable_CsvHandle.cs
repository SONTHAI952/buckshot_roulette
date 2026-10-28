using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using ZeroX.CsvTools;
using ZeroX.JsonSystem;


namespace ZeroX.DataTableSystem.SoTableSystem
{
    public abstract partial class SoTable<TTable, TRow>
    {
        #region Utility

        private FieldInfo GetFieldInfoOfRow(Type rowType, string fieldName)
        {
            BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


            if (rowType.IsSubclassOf(typeof(ScriptableObject)) == false)
            {
                while (true)
                {
                    FieldInfo fieldInfo = rowType.GetField(fieldName, bindingFlags);
                    if (fieldInfo != null)
                        return fieldInfo;
                    
                    
                    rowType = rowType.BaseType;
                    if (rowType == null || rowType == typeof(System.Object))
                        return null;
                }
            }
            else
            {
                while (true)
                {
                    FieldInfo fieldInfo = rowType.GetField(fieldName, bindingFlags);
                    if (fieldInfo != null)
                        return fieldInfo;
                    
                    
                    rowType = rowType.BaseType;
                    if (rowType == null || rowType == typeof(ScriptableObject))
                        return null;
                }
            }

            return null;
        }

        
        
        /// <summary>
        /// Tạo ra danh sách kiểu của row để biết và xử lý. Trong csv có thể có field row type để chỉ định kiểu của row. Nếu không có thì sẽ dùng TRow
        /// </summary>
        private List<Type> CreateListRowType(List<string> listField, List<List<string>> listCsvRow, out int indexOfRowTypeField)
        {
            List<Type> listRowType = new List<Type>();
            var defaultRowType = typeof(TRow);
            
            
            
            indexOfRowTypeField = listField.FindIndex(f => SOTableConst.IsFieldNameRowType(f));
            if (indexOfRowTypeField == -1)
            {
                
                for (int i = 0; i < listCsvRow.Count; i++)
                {
                    listRowType.Add(defaultRowType);
                }

                return listRowType;
            }


            
            Assembly assembly = Assembly.GetAssembly(typeof(TRow));
            foreach (var csvRow in listCsvRow)
            {
                var rowTypeS = csvRow[indexOfRowTypeField];
                
                
                //Nếu không có row type thì dùng default row type
                if (string.IsNullOrWhiteSpace(rowTypeS))
                {
                    listRowType.Add(defaultRowType);
                    continue;
                }
                
                
                
                Type rowType = assembly.GetType(rowTypeS);
                listRowType.Add(rowType == null ? defaultRowType : rowType);
            }

            return listRowType;
        }

        #endregion
        

        
        
        
        /// <summary>
        /// Dùng để biến đổi csv data đầu vào thành dạng mong muốn trước khi import
        /// </summary>
        public virtual void TransformCsv(List<string> listFieldNameInput, List<List<string>> listCsvRowInput, out List<string> listFieldNameOutput, out List<List<string>> listCsvRowOutput)
        {
            listFieldNameOutput = listFieldNameInput;
            listCsvRowOutput = listCsvRowInput;
        }
        
        


        
        public void ImportFromCsv(string csvText, ImportFromCsvOption importOption)
        {
            var listCsvRow = CsvUtility.ConvertCsvToListCsvRow(csvText, importOption.fieldSeparator, importOption.quoteCharacter, importOption.trimField);
            
            //Tách
            var listColumnName = listCsvRow[0];
            listCsvRow.RemoveAt(0);
            
            //Lấy listFieldName
            var listFieldName = CsvUtility.ConvertListColumnNameToListFieldName(listColumnName, importOption.upperCaseAfterWordSeparator, importOption.removeWordSeparator);
            
            ImportFromCsv(listFieldName, listCsvRow);
        }
        
        public void ImportFromCsv(string csvText)
        {
            ImportFromCsvOption importOption = new ImportFromCsvOption(); //Không sửa gì tức là sử dụng option mặc định
            ImportFromCsv(csvText, importOption);
        }

        public void ImportFromCsv(List<string> listFieldName, List<List<string>> listCsvRow)
        {
            TransformCsv(listFieldName, listCsvRow, out var listFieldOutput, out var listCsvRowOutput);
            ImportFromCsv_Core(listFieldOutput, listCsvRowOutput);
        }
        
        
        /// <summary>
        /// Hàm này sẽ ko dử dụng TransformCsv
        /// </summary>
        public void ImportFromCsv_Core(List<string> listFieldName, List<List<string>> listCsvRow)
        {
            var rowType = typeof(TRow);
            if(rowType.IsSubclassOf(typeof(ScriptableObject)) == false)
                NonSO_ImportFromCsv(listFieldName, listCsvRow);
            else
            {
                SO_ImportFromCsv(listFieldName, listCsvRow);
            }
        }
        
        
        
        private void FillCsvRowToRow(List<string> listFieldName, List<string> csvRow, TRow row, Type rowType, int rowIndex, int indexOfRowTypeField)
        {
            for (int cellIndex = 0; cellIndex < csvRow.Count; cellIndex++)
            {
                string fieldName = listFieldName[cellIndex];
                    
                //Field mà là CSV_FIELD_ROW_TYPE thì bỏ qua
                if(cellIndex == indexOfRowTypeField)
                    continue;
                    
                string cellValue = csvRow[cellIndex];
                if(string.IsNullOrEmpty(cellValue))
                    continue;
                    
                var fieldInfo = GetFieldInfoOfRow(rowType, fieldName);
                if (fieldInfo == null)
                {
                    RecordError($"Field '{fieldName}' not exist in row type '{rowType.Name}'");
                    continue;
                }

                SetFieldInfoValue(fieldInfo, row, cellValue, rowIndex + 2);
            }
        }

        
        
        
        
        
        
        
        
        




        #region Set Field Value
        
        void SetFieldInfoValue(FieldInfo fieldInfo, object obj, string cellValue, int rowInSheet)
        {
            var fieldType = fieldInfo.FieldType;
            
            //string
            if (fieldType == typeof(string))
            {
                fieldInfo.SetValue(obj, cellValue);
                return;
            }
            
            //char
            if (fieldType == typeof(char))
            {
                fieldInfo.SetValue(obj, cellValue[0]);
                return;
            }

            //enum
            if (fieldType.IsEnum)
            {
                SetFieldInfoEnumValue(fieldInfo, obj, cellValue, rowInSheet);
                return;
            }
            
            //bool
            if (fieldType == typeof(bool))
            {
                if(cellValue.Equals("true", StringComparison.OrdinalIgnoreCase))
                    fieldInfo.SetValue(obj, true);
                else if(cellValue.Equals("false", StringComparison.OrdinalIgnoreCase))
                    fieldInfo.SetValue(obj, false);
                else
                {
                    if (int.TryParse(cellValue, out int number) == false)
                    {
                        Debug.LogErrorFormat("Field {0} có kiểu là bool nhưng data để import lại không phải - rowInSheet: {1}", fieldInfo.Name, rowInSheet);
                    }
                    else if(number != 0)
                        fieldInfo.SetValue(obj, true);
                    else
                        fieldInfo.SetValue(obj, false);
                }

                return;
            }
            

            //Kiểu số nguyên
            //byte
            if (fieldType == typeof(byte))
            {
                if(byte.TryParse(cellValue, out var number) == false)
                    Debug.LogErrorFormat("Field {0} có kiểu là byte nhưng data để import lại không phải - rowInSheet: {1}", fieldInfo.Name, rowInSheet);
                else
                    fieldInfo.SetValue(obj, number);
                
                return;
            }
            
            //sbyte
            if (fieldType == typeof(sbyte))
            {
                if(sbyte.TryParse(cellValue, out var number) == false)
                    Debug.LogErrorFormat("Field {0} có kiểu là sbyte nhưng data để import lại không phải - rowInSheet: {1}", fieldInfo.Name, rowInSheet);
                else
                    fieldInfo.SetValue(obj, number);
                
                return;
            }
            
            //short
            if (fieldType == typeof(short))
            {
                if(short.TryParse(cellValue, out var number) == false)
                    Debug.LogErrorFormat("Field {0} có kiểu là short nhưng data để import lại không phải - rowInSheet: {1}", fieldInfo.Name, rowInSheet);
                else
                    fieldInfo.SetValue(obj, number);
                
                return;
            }
            
            //ushort
            if (fieldType == typeof(ushort))
            {
                if(ushort.TryParse(cellValue, out var number) == false)
                    Debug.LogErrorFormat("Field {0} có kiểu là ushort nhưng data để import lại không phải - rowInSheet: {1}", fieldInfo.Name, rowInSheet);
                else
                    fieldInfo.SetValue(obj, number);
                
                return;
            }
            
            //int
            if (fieldType == typeof(int))
            {
                if(int.TryParse(cellValue, out var number) == false)
                    Debug.LogErrorFormat("Field {0} có kiểu là int nhưng data để import lại không phải - rowInSheet: {1}", fieldInfo.Name, rowInSheet);
                else
                    fieldInfo.SetValue(obj, number);
                
                return;
            }
            
            //uint
            if (fieldType == typeof(uint))
            {
                if(uint.TryParse(cellValue, out var number) == false)
                    Debug.LogErrorFormat("Field {0} có kiểu là uint nhưng data để import lại không phải - rowInSheet: {1}", fieldInfo.Name, rowInSheet);
                else
                    fieldInfo.SetValue(obj, number);
                
                return;
            }
            
            //long
            if (fieldType == typeof(long))
            {
                if(long.TryParse(cellValue, out var number) == false)
                    Debug.LogErrorFormat("Field {0} có kiểu là long nhưng data để import lại không phải - rowInSheet: {1}", fieldInfo.Name, rowInSheet);
                else
                    fieldInfo.SetValue(obj, number);
                
                return;
            }
            
            //ulong
            if (fieldType == typeof(ulong))
            {
                if(ulong.TryParse(cellValue, out var number) == false)
                    Debug.LogErrorFormat("Field {0} có kiểu là ulong nhưng data để import lại không phải - rowInSheet: {1}", fieldInfo.Name, rowInSheet);
                else
                    fieldInfo.SetValue(obj, number);
                
                return;
            }
            
            //Kiểu số thực
            //float
            if (fieldType == typeof(float))
            {
                try
                {
                    var number = float.Parse(cellValue, CultureInfo.InvariantCulture.NumberFormat);
                    fieldInfo.SetValue(obj, number);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Field {0} có kiểu là float nhưng data để import lại không phải - rowInSheet: {1}", fieldInfo.Name, rowInSheet);
                }
                
                return;
            }
            
            //double
            if (fieldType == typeof(double))
            {
                try
                {
                    var number = double.Parse(cellValue, CultureInfo.InvariantCulture.NumberFormat);
                    fieldInfo.SetValue(obj, number);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Field {0} có kiểu là double nhưng data để import lại không phải - rowInSheet: {1}", fieldInfo.Name, rowInSheet);
                }
                
                return;
            }
            
            //decimal
            if (fieldType == typeof(decimal))
            {
                try
                {
                    var number = decimal.Parse(cellValue, CultureInfo.InvariantCulture.NumberFormat);
                    fieldInfo.SetValue(obj, number);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Field {0} có kiểu là decimal nhưng data để import lại không phải - rowInSheet: {1}", fieldInfo.Name, rowInSheet);
                }
                
                return;
            }
            
            //Kiểu Object
            if(string.IsNullOrEmpty(cellValue) == false && (cellValue[0] == '{' || cellValue[0] == '['))
            {
                try
                {
                    object objFromJson = ZJson.JsonToObject(cellValue, fieldType);
                    fieldInfo.SetValue(obj, objFromJson);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError($"Json sai định dạng ở field {fieldInfo.Name}, rowInSheet: {rowInSheet}");
                }
                
                return;
            }
            
            //Kiểu Color
            if (fieldInfo.FieldType == typeof(Color))
            {
                if (string.IsNullOrEmpty(cellValue))
                {
                    fieldInfo.SetValue(obj, Color.white);
                    return;
                }

                string htmlString;
                if (cellValue[0] != '#')
                    htmlString = "#" + cellValue;
                else
                    htmlString = cellValue;
                    
                    
                if (ColorUtility.TryParseHtmlString(htmlString, out Color color))
                {
                    fieldInfo.SetValue(obj, color);
                }
                else
                {
                    Debug.LogError($"Color html string sai định dạng ở field {fieldInfo.Name}, rowInSheet: {rowInSheet}");
                }
                
                return;
            }

#if UNITY_EDITOR
            if (fieldInfo.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                if (TableToolSetting.Import.IncludeUnityObjectField == false)
                {
                    //Không làm gì cả
                    return;
                }

                if (string.IsNullOrEmpty(cellValue))
                {
                    fieldInfo.SetValue(obj, null);
                    return;
                }
                
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(cellValue);
                if (string.IsNullOrEmpty(assetPath))
                {
                    fieldInfo.SetValue(obj, null);
                    return;
                }
                
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, fieldInfo.FieldType);
                fieldInfo.SetValue(obj, asset);
                return;
            }
#endif


            Debug.LogError("Không hỗ trợ import field có kiểu là: " + fieldInfo.FieldType.Name);
        }
        
        void SetFieldInfoEnumValue(FieldInfo fieldInfo, object obj, string cellValue, int rowInSheet)
        {
            var fieldType = fieldInfo.FieldType;
            
            //Thử vỡi chữ trước
            try
            {
                var enumObj = Enum.Parse(fieldType, cellValue, true);
                fieldInfo.SetValue(obj, enumObj);
                return;
            }
            catch (Exception e)
            { }

            //Sau đó thử với số
            try
            {
                int number = int.Parse(cellValue);
                string nameToParse = Enum.GetName(fieldType, number);
                var enumObj = Enum.Parse(fieldType, nameToParse, true);
                fieldInfo.SetValue(obj, enumObj);
                return;
            }
            catch (Exception e)
            { }

            
            //Định dạng lại chữ
            if (string.IsNullOrEmpty(cellValue) == false)
            {
                //Định dạng lại chữ và thử lại với chữ
                StringBuilder sb = new StringBuilder();

                bool needUpperCase = true; //Để upper case ký tự đầu
                for (int i = 0; i < cellValue.Length; i++)
                {
                    char c = cellValue[i];
                    if (c == '_' || c == ' ')
                    {
                        needUpperCase = true;
                        continue;
                    }
                    
                    if (needUpperCase)
                    {
                        sb.Append(c.ToString().ToUpper());
                        needUpperCase = false;
                    }
                    else
                        sb.Append(c);
                }

                cellValue = sb.ToString();
            }
            
            
            //Thử lại với chữ đã đc định dạng
            try
            {
                var enumObj = Enum.Parse(fieldType, cellValue, true);
                fieldInfo.SetValue(obj, enumObj);
                return;
            }
            catch (Exception e)
            { }
            
            
            Debug.LogErrorFormat("Field {0} có kiểu là {1} nhưng data để import lại không phải - rowInSheet: {2}", fieldInfo.Name, fieldType.Name, rowInSheet);
        }
        
        #endregion
        
        
    }
}