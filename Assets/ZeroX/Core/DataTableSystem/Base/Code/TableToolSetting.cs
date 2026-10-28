#if UNITY_EDITOR
using UnityEditor;
using ZeroX.DataTableSystem.Editors;


namespace ZeroX.DataTableSystem
{
    public static class TableToolSetting
    {
        private const string root_key = "ZeroX.DataTableSystem.Setting.";
        private const string importSetting_key = root_key + "ImportSetting.";
        private const string exportSetting_key = root_key + "ExportSetting.";
        
        
        
        public static class Import
        {
            public static int FieldSeparatorIndexSelected
            {
                get => EditorPrefs.GetInt(importSetting_key + "FieldSeparatorIndexSelected", 0);
                set => EditorPrefs.SetInt(importSetting_key + "FieldSeparatorIndexSelected", value);
            }
            
            public static int QuoteCharacterIndexSelected
            {
                get => EditorPrefs.GetInt(importSetting_key + "QuoteCharacterIndexSelected", 0);
                set => EditorPrefs.SetInt(importSetting_key + "QuoteCharacterIndexSelected", value);
            }
            
            public static bool TrimField
            {
                get => EditorPrefs.GetBool(importSetting_key + "TrimField", true);
                set => EditorPrefs.SetBool(importSetting_key + "TrimField", value);
            }
            
            public static bool IncludeUnityObjectField
            {
                get => EditorPrefs.GetBool(importSetting_key + "IncludeUnityObjectField", true);
                set => EditorPrefs.SetBool(importSetting_key + "IncludeUnityObjectField", value);
            }
            
            public static SORowNameStyle SORowNameStyle
            {
                get => (SORowNameStyle)EditorPrefs.GetInt(importSetting_key + "SORowNameStyle", (int)SORowNameStyle.Id);
                set => EditorPrefs.GetInt(importSetting_key + "SORowNameStyle", (int)value);
            }
        }
        
        public static class Export
        {
            public static bool ConvertHeaderCamelCaseToSnakeCase
            {
                get => EditorPrefs.GetBool(exportSetting_key + "ConvertHeaderCamelCaseToSnakeCase", true);
                set => EditorPrefs.SetBool(exportSetting_key + "ConvertHeaderCamelCaseToSnakeCase", value);
            }
            
            public static bool ConvertEnumCamelCaseToSnakeCase
            {
                get => EditorPrefs.GetBool(exportSetting_key + "ConvertEnumCamelCaseToSnakeCase", true);
                set => EditorPrefs.SetBool(exportSetting_key + "ConvertEnumCamelCaseToSnakeCase", value);
            }
            
            public static bool IncludeRowType
            {
                get => EditorPrefs.GetBool(exportSetting_key + "IncludeRowType", false);
                set => EditorPrefs.SetBool(exportSetting_key + "IncludeRowType", value);
            }
            
            public static bool IncludeUnityObjectField
            {
                get => EditorPrefs.GetBool(exportSetting_key + "IncludeUnityObjectField", true);
                set => EditorPrefs.SetBool(exportSetting_key + "IncludeUnityObjectField", value);
            }
        }
    }
}

#endif