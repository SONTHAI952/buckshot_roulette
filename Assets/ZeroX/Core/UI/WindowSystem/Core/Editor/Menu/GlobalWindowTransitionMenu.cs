using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.WindowSystem.Editors
{
    public static class GlobalWindowTransitionMenu
    {
        [MenuItem(WindowConst.MenuItem.GlobalWindowTransitionTable)]
        static void OpenTable()
        {
            var instance = GlobalWindowTransitionTable.GetInstanceWithoutLog();
            if (instance == null)
            {
                instance = AskAndCreate();
                if(instance == null)
                    return;
            }
            
            
            
            EditorGUIUtility.PingObject(instance);
            Selection.activeObject = instance;
        }

        private static GlobalWindowTransitionTable AskAndCreate()
        {
            bool yes = EditorUtility.DisplayDialog("Table instance not exist!", "Would you like to create one?", "Create One", "No");
            if (yes == false)
                return null;

            GlobalWindowTransitionTable instance = ScriptableObject.CreateInstance<GlobalWindowTransitionTable>();
            CreateListRowDefault(instance);
            AssetDatabase.CreateAsset(instance, WindowConst.AbsolutePathImplement.GlobalWindowTransitionTable);
            AssetDatabase.SaveAssets();
            
            return instance;
        }

        private static void CreateListRowDefault(GlobalWindowTransitionTable table)
        {
            var listIdNames = Enum.GetNames(typeof(GlobalWindowTransitionId));

            foreach (var idNames in listIdNames)
            {
                GlobalWindowTransitionId id = Enum.Parse<GlobalWindowTransitionId>(idNames);
                GlobalWindowTransitionRow row = new GlobalWindowTransitionRow();
                row.id = id;
                row.config = WindowTransitionConfig.CreateDefault();
                table.RawRowsI.Add(row);
            }
        }
    }
}