using UnityEditor;
using UnityEngine;

namespace ZeroX.WindowSystem.Editors
{
    public static class WindowSortingMenu
    {
        [MenuItem(WindowConst.MenuItem.WindowSortingTable)]
        static void OpenTable()
        {
            var instance = WindowSortingTable.GetInstanceWithoutLog();
            if (instance == null)
            {
                instance = AskAndCreate();
                if(instance == null)
                    return;
            }
            
            
            
            EditorGUIUtility.PingObject(instance);
            Selection.activeObject = instance;
        }

        private static WindowSortingTable AskAndCreate()
        {
            bool yes = EditorUtility.DisplayDialog("Table instance not exist!", "Would you like to create one?", "Create One", "No");
            if (yes == false)
                return null;

            WindowSortingTable instance = ScriptableObject.CreateInstance<WindowSortingTable>();
            AssetDatabase.CreateAsset(instance, WindowConst.AbsolutePathImplement.WindowSortingTable);
            AssetDatabase.SaveAssets();
            
            return instance;
        }
    }
}