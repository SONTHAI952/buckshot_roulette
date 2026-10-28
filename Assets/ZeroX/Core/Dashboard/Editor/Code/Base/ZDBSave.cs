using UnityEditor;

namespace ZeroX.DashboardSystem
{
    public class ZDBSave
    {
        private const string Root_Save_Key = "ZeroX.DashboardSystem.";
        private const string OpeningDBDrawer_PreKey = Root_Save_Key + "OpeningDBDrawerId.";
        private const string SystemDrawerFieldFoldout_PreKey = Root_Save_Key + "SystemDrawerFieldFoldout.";
        
        
        
        public static class OpeningDBDrawer
        {
            public static string Group
            {
                get => EditorPrefs.GetString(OpeningDBDrawer_PreKey + "Main", "");
                set => EditorPrefs.SetString(OpeningDBDrawer_PreKey + "Main", value);
            }
            
            public static string System
            {
                get => EditorPrefs.GetString(OpeningDBDrawer_PreKey + "System", "");
                set => EditorPrefs.SetString(OpeningDBDrawer_PreKey + "System", value);
            }
        }
        
        
        public static class SystemDrawerFieldFoldout
        {
            public static bool Get(string systemDrawerId, string foldoutKey)
            {
                string key = $"{SystemDrawerFieldFoldout_PreKey}{systemDrawerId}.{foldoutKey}";
                return EditorPrefs.GetBool(key, false);
            }
            
            public static void Set(string systemDrawerId, string foldoutKey, bool value)
            {
                string key = $"{SystemDrawerFieldFoldout_PreKey}{systemDrawerId}.{foldoutKey}";
                EditorPrefs.SetBool(key, value);
            }
        }
    }
}