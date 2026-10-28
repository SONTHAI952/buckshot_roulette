using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ZeroX.WindowSystem.Editors
{
    public static class WindowTemplateMenu
    {
        [MenuItem(WindowConst.MenuItem.WindowTemplateTable)]
        static void OpenTable()
        {
            var instance = WindowTemplateTable.GetInstanceWithoutLog();
            if (instance == null)
            {
                instance = AskAndCreate();
                if(instance == null)
                    return;
            }
            
            
            
            //EditorGUIUtility.PingObject(instance);
            Selection.activeObject = instance;
        }

        private static WindowTemplateTable AskAndCreate()
        {
            bool yes = EditorUtility.DisplayDialog("Table instance not exist!", "Would you like to create one?", "Create One", "No");
            if (yes == false)
                return null;

            WindowTemplateTable instance = ScriptableObject.CreateInstance<WindowTemplateTable>();
            AssetDatabase.CreateAsset(instance, WindowConst.AbsolutePathImplement.WindowTemplateTable);
            AssetDatabase.SaveAssets();
            
            return instance;
        }


        #region Utility

        private static GameObject GetDefaultPrefabInFolder(string fileName)
        {
            string filePath = Path.Combine(WindowConst.AbsolutePathImplement.DefaultWindowTemplateFolder, fileName + ".prefab");
            return AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
        }

        private static GameObject GetPrefabInTable(string fieldName)
        {
            var windowTemplateTable = WindowTemplateTable.GetInstanceWithoutLog();
            if (windowTemplateTable == null)
                return null;
            
            Type windowTemplateTableType = typeof(WindowTemplateTable);
            var fieldInfo = windowTemplateTableType.GetField(fieldName, BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                Debug.LogError("Window Template Table has not field: " + fieldName);
                return null;
            }
            
            return fieldInfo.GetValue(windowTemplateTable) as GameObject;
        }
        
        private static void SpawnTemplate(GameObject prefab)
        {
            var template = (GameObject)PrefabUtility.InstantiatePrefab(prefab, Selection.activeTransform);
            PrefabUtility.UnpackPrefabInstance(template, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            Undo.RegisterCreatedObjectUndo(template, "WindowSystem Create " + template.name);
            
            Selection.activeGameObject = template;
        }

        private static void SpawnTemplate(string prefabFieldNameInTable, string defaultPrefabName)
        {
            GameObject prefab = GetPrefabInTable(prefabFieldNameInTable);
            if (prefab != null)
            {
                SpawnTemplate(prefab);
                return;
            }

            prefab = GetDefaultPrefabInFolder(defaultPrefabName);
            if (prefab != null)
            {
                SpawnTemplate(prefab);
                return;
            }

            EditorUtility.DisplayDialog("No template prefab found!", "No template prefab found!", "Ok");
        }
        
        #endregion
        
        

        #region Create

        [MenuItem(WindowConst.MenuCreateTemplate.CanvasOverlay_Portrait, priority = 0)]
        public static void Create_CanvasOverlay_Portrait()
        {
            SpawnTemplate("canvasOverlayPortrait", "Window - Canvas - Overlay - Portrait");
        }
        
        [MenuItem(WindowConst.MenuCreateTemplate.CanvasOverlay_Landscape, priority = 1)]
        public static void Create_CanvasOverlay_Landscape()
        {
            SpawnTemplate("canvasOverlayLandscape", "Window - Canvas - Overlay - Landscape");
        }
        
        [MenuItem(WindowConst.MenuCreateTemplate.Overlay, priority = 2)]
        public static void Create_Overlay()
        {
            SpawnTemplate("overlay", "Window - Overlay");
        }
        
        [MenuItem(WindowConst.MenuCreateTemplate.NonOverlay, priority = 3)]
        public static void Create_NonOverlay()
        {
            SpawnTemplate("nonOverlay", "Window - Non Overlay");
        }

        #endregion
    }
}