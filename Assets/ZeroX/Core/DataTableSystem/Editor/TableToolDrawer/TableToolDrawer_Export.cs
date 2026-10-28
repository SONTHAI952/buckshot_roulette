using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using ZeroX.CsvTools;
using ZeroX.DataTableSystem.SoTableSystem;
using ZeroX.JsonSystem;

namespace ZeroX.DataTableSystem.Editors
{
    public partial class TableToolDrawer
    {
        private void DrawTab_Export()
        {
            DrawExportSetting();
            GUILayout.Space(15);

            
            
            
            
            //Export To Clipboard
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Export To Clipboard", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("With Header", GUILayout.Height(35)))
            {
                ExportToClipboard_WithHeader();
                LogExportSuccess();
            }
            
            if (GUILayout.Button("No Header", GUILayout.Height(35)))
            {
                ExportToClipboard_NoHeader();
                LogExportSuccess();
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            
            
            
            
            GUILayout.Space(30);
            
            
            
            
            
            //Export Only Row Type To Clipboard
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Export Only Row Type To Clipboard");
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("With Header", GUILayout.Height(20)))
            {
                ExportToClipboard_OnlyRowType_WithHeader();
                LogExportSuccess();
            }
            
            if (GUILayout.Button("No Header", GUILayout.Height(20)))
            {
                ExportToClipboard_OnlyRowType_NoHeader();
                LogExportSuccess();
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawExportSetting()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Export Settings", EditorStyles.boldLabel);
            
            
            TableToolSetting.Export.ConvertHeaderCamelCaseToSnakeCase = EditorGUILayout.Toggle("Header: camelCase to snake_case", TableToolSetting.Export.ConvertHeaderCamelCaseToSnakeCase);
            TableToolSetting.Export.ConvertEnumCamelCaseToSnakeCase = EditorGUILayout.Toggle("Enum: camelCase to snake_case", TableToolSetting.Export.ConvertEnumCamelCaseToSnakeCase);
            DrawOption_IncludeRowType();
            TableToolSetting.Export.IncludeUnityObjectField = EditorGUILayout.Toggle("Include Unity Object Field", TableToolSetting.Export.IncludeUnityObjectField);
            
            GUILayout.EndVertical();
        }
        
        private void DrawOption_IncludeRowType()
        {
            GUILayout.BeginHorizontal();
            TableToolSetting.Export.IncludeRowType = EditorGUILayout.Toggle("Include Row Type", TableToolSetting.Export.IncludeRowType);
            bool isClickRecommendOption = GUILayout.Button("R", GUILayout.Width(20));
            GUILayout.EndHorizontal();
            
            if (isClickRecommendOption)
            {
                TableToolSetting.Export.IncludeRowType = CalculateRecommendOption_IncludeRowType();
            }
        }
        

        private bool CalculateRecommendOption_IncludeRowType()
        {
            Type declaredRowType = SOTableReflection.GetDeclaredRowType(tableAsset);
            if (declaredRowType.IsAbstract || declaredRowType.IsInterface)
                return true;


            var listRow = SOTableReflection.GetListRow(tableAsset);
            HashSet<Type> hashSetRowType = new HashSet<Type>();
            foreach (var row in listRow)
            {
                if(row == null)
                    continue;

                hashSetRowType.Add(row.GetType());
                if (hashSetRowType.Count > 1)
                    return true;
            }

            return false;
        }

        private List<string> CreateListHeader()
        {
            var fieldInfos = SOTableReflection.GetListFieldInfoOfRow(tableAsset);

            List<string> list = new List<string>();
            foreach (var fieldInfo in fieldInfos)
            {
                if(TableToolSetting.Export.ConvertHeaderCamelCaseToSnakeCase)
                    list.Add(Convert_CamelCase_To_SnakeCase(fieldInfo.Name));
                else
                    list.Add(fieldInfo.Name);
            }

            return list;
        }

        

        private List<List<string>> ConvertTableRows_To_ListRowString()
        {
            //Setting
            bool includeUnityObjectField = TableToolSetting.Export.IncludeUnityObjectField;
            
            
            var listRow = SOTableReflection.GetListRow(tableAsset);
            if (listRow == null)
            {
                return new List<List<string>>();
            }


            var listFieldInfos = SOTableReflection.GetListFieldInfoOfRow(tableAsset); //Danh sách các fieldInfo của row
            List<List<string>> listRowResult = new List<List<string>>();
            foreach (var rowObj in listRow)
            {
                var rowResult = new List<string>();
                listRowResult.Add(rowResult);
                
                foreach (var fieldInfo in listFieldInfos)
                {
                    string s = ExportFieldToString(fieldInfo, rowObj, includeUnityObjectField);
                    rowResult.Add(s);
                }
            }

            return listRowResult;
        }

        private string ExportFieldToString(FieldInfo fieldInfo, object rowObj, bool includeUnityObjectField)
        {
            if (fieldInfo.DeclaringType.IsInstanceOfType(rowObj) == false)
                return "";
            
            var value = fieldInfo.GetValue(rowObj);
            
            //Nếu value null thì thêm chuỗi rỗng
            if (value == null)
                return "";
            
            //Nếu là UnityObject
            if (value is UnityEngine.Object valueUnityObject)
            {
                if (includeUnityObjectField == false)
                {
                    return "";
                }
                
                string assetPath = AssetDatabase.GetAssetPath(valueUnityObject);
                if (string.IsNullOrEmpty(assetPath))
                {
                    return "";
                }
                
                return AssetDatabase.AssetPathToGUID(assetPath);
            }
            
            //Kiểm tra xem có thể convert sang json không
            string json = "";

            try
            {
                json = ZJson.ObjectToJson(value);
            }
            catch (Exception e)
            {
                json = "";
            }
            
            
            if (string.IsNullOrEmpty(json) == false && (json[0] == '{' || json[0] == '['))
                return json;

            if (fieldInfo.FieldType.IsEnum)
            {
                if(TableToolSetting.Export.ConvertEnumCamelCaseToSnakeCase)
                    return Convert_CamelCase_To_SnakeCase(value.ToString());
                else
                    return value.ToString();
            }
            
            
            return string.Format(CultureInfo.InvariantCulture, "{0}", value);
        }

        private List<string> ConvertTableRows_To_ListRowType()
        {
            var listRow = SOTableReflection.GetListRow(tableAsset);
            if (listRow == null || listRow.Count == 0)
            {
                return new List<string>();
            }
            
            List<string> listRowType = new List<string>();
            foreach (object rowObj in listRow)
            {
                var rowType = rowObj.GetType();
                listRowType.Add(rowType.FullName);
            }
            
            return listRowType;
        }
        

        private void ExportToClipboard_WithHeader()
        {
            List<List<string>> listRow = ConvertTableRows_To_ListRowString();
            List<string> listHeader = CreateListHeader();
            
            
            if (TableToolSetting.Export.IncludeRowType)
            {
                //Thêm row type
                listHeader.Add(SOTableConst.FIELD_NAME_ROW_TYPE);
                var listRowType = ConvertTableRows_To_ListRowType();
                
                for (int rowIndex = 0; rowIndex < listRow.Count; rowIndex++)
                {
                    var row = listRow[rowIndex];
                    row.Add(listRowType[rowIndex]);
                }
            }
            
            
            //Thêm row header
            listRow.Insert(0, listHeader);
            
            CsvUtility.ExportToClipboard(listRow);
        }

        private void ExportToClipboard_NoHeader()
        {
            List<List<string>> listRow = ConvertTableRows_To_ListRowString();
            
            
            if (TableToolSetting.Export.IncludeRowType)
            {
                //Thêm row type
                var listRowType = ConvertTableRows_To_ListRowType();
                
                for (int i = 0; i < listRow.Count; i++)
                {
                    var row = listRow[i];
                    row.Add(listRowType[i]);
                }
            }
            
            
            CsvUtility.ExportToClipboard(listRow);
        }

        private void ExportToClipboard_OnlyRowType_WithHeader()
        {
            var listRowType = ConvertTableRows_To_ListRowType();
            listRowType.Insert(0, SOTableConst.FIELD_NAME_ROW_TYPE);
            
            CsvUtility.ExportOneColumnToClipboard(listRowType);
        }
        
        private void ExportToClipboard_OnlyRowType_NoHeader()
        {
            var listRowType = ConvertTableRows_To_ListRowType();
            CsvUtility.ExportOneColumnToClipboard(listRowType);
        }
        
        

        private string Convert_CamelCase_To_SnakeCase(string s)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                
                
                //Nếu char hiện tại là lower và char tiếp theo là upper thì thêm _ sau khi đã add char hiện tại
                //Nếu char hiện tại là upper, char trước đó cũng là upper và char tiếp theo là lower thì thêm _ trước khi add char hiện tại
                //listReward -> list_reward
                //testFIELDNay -> test_field_nay


                if (i >= s.Length - 1) //Ký tự cuối thì add thôi
                {
                    sb.Append(char.ToLower(c));
                    continue;
                }
                
                if (char.IsLower(c) && char.IsUpper(s[i + 1]))
                {
                    sb.Append(char.ToLower(c));
                    sb.Append('_');
                    continue;
                }

                if (char.IsUpper(c) && char.IsLower(s[i + 1]) && i > 0 && char.IsUpper(s[i - 1]))
                {
                    sb.Append('_');
                    sb.Append(char.ToLower(c));
                    continue;
                }

                sb.Append(char.ToLower(c));
            }

            return sb.ToString();
        }

        private void Export_OnTableAssetChanged()
        {
            TableToolSetting.Export.IncludeRowType = CalculateRecommendOption_IncludeRowType();
        }

        private void LogExportSuccess()
        {
            Debug.Log("Export success");
        }
    }
}