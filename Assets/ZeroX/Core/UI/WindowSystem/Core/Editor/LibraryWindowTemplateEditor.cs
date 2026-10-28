/*using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.WindowSystem.Editors
{
    public static class LibraryWindowTemplateEditor
    {
        static Object CreateLibraryWindowTemplate(string rootResourceFolder, bool focus)
        {
            string folderPath = Path.Combine("Assets/Resources", rootResourceFolder);
            string filePath = Path.Combine(folderPath, "LibraryWindowTemplate.asset");
            if (Directory.Exists(folderPath) == false)
                Directory.CreateDirectory(folderPath);

            var asset = ScriptableObject.CreateInstance<WindowTemplateTable>();
                
            AssetDatabase.CreateAsset(asset, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (focus)
            {
                EditorUtility.FocusProjectWindow();
                var obj = AssetDatabase.LoadAssetAtPath<WindowTemplateTable>(filePath);
                if (obj != null)
                {
                    EditorGUIUtility.PingObject(obj);
                }
            }

            return asset;
        }
        
        [MenuItem("Tools/ZeroX/Window System/Library Window Template")]
        static void OpenSetting()
        {
            string rootResourceFolder = "ZeroX";
            var list = Resources.LoadAll<WindowTemplateTable>(rootResourceFolder);
            if (list.Length == 0)
            {
                bool confirm = EditorUtility.DisplayDialog("Cannot find Library Window Template", "Cannot find LibraryWindowTemplate!\nCreate One?", "Yes", "No");
                if (confirm)
                {
                    var newSetting = CreateLibraryWindowTemplate(rootResourceFolder, true);
                    EditorUtility.FocusProjectWindow();
                    EditorGUIUtility.PingObject(newSetting);
                    Selection.SetActiveObjectWithContext(newSetting, null);
                    return;
                }
                else
                    return;
            }
        
            if (list.Length > 1)
            {
                Debug.LogError("Found 2 setting files, there should be only one setting");
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(list[1]);
                return;
            }
        
            var setting = list[0];
            Selection.SetActiveObjectWithContext(setting, null);
        }
        
        
        static void DisplayDialogCannotFindTemplate(string templateName)
        {
            string title = string.Format("Cannot find {0}!", templateName);
            string message = string.Format("Cannot find {0}\nMake sure you put the ZeroX folder in the assets directory", templateName);
            EditorUtility.DisplayDialog(title, message, "Ok");
        }

        static void AdjustRectTransform(GameObject obj)
        {
            RectTransform rectTrans = obj.GetComponent<RectTransform>();
            if(rectTrans == null)
                return;
            
            rectTrans.anchorMin = new Vector2(0, 0);
            rectTrans.anchorMax = new Vector2(1, 1);
            rectTrans.offsetMin = new Vector2(0, 0);
            rectTrans.offsetMax = new Vector2(0, 0);
            
            rectTrans.pivot = new Vector2(0.5f, 0.5f);
            rectTrans.anchoredPosition3D = new Vector3(0, 0, 0);
            rectTrans.localScale = new Vector3(1,1,1);
        }
        
        static void AdjustRectTransform_NotAnchor(GameObject obj)
        {
            RectTransform rectTrans = obj.GetComponent<RectTransform>();
            if(rectTrans == null)
                return;
            
            rectTrans.pivot = new Vector2(0.5f, 0.5f);
            rectTrans.anchoredPosition3D = new Vector3(0, 0, 0);
            rectTrans.localScale = new Vector3(1,1,1);
        }

        static GameObject CreateTemplate(string filePath, string name)
        {
            var template = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
            if (template == null)
            {
                DisplayDialogCannotFindTemplate("Window Template");
                return null;
            }
            
            var obj = GameObject.Instantiate(template);
            obj.name = name;
            Undo.RegisterCreatedObjectUndo(obj, "Create Window Template");

            var arr = Selection.gameObjects;
            if (arr != null && arr.Length > 0)
                obj.transform.parent = arr[0].transform;

            obj.transform.SetSiblingIndex(Int32.MaxValue);

            return obj;
        }

        static GameObject CreateTemplate(GameObject template, string name)
        {
            if (template == null)
            {
                DisplayDialogCannotFindTemplate("Window Template");
                return null;
            }
            
            var obj = GameObject.Instantiate(template);
            obj.name = name;
            Undo.RegisterCreatedObjectUndo(obj, "Create Window Template");

            var arr = Selection.gameObjects;
            if (arr != null && arr.Length > 0)
                obj.transform.parent = arr[0].transform;

            obj.transform.SetSiblingIndex(Int32.MaxValue);

            return obj;
        }

        #region Window Animator

        [MenuItem("GameObject/ZeroX/Window Template/Window Animator/Canvas - Overlay - Portrait", false, 10)]
        static void CreateWindowAnimator_Canvas_Overlay_Portrait()
        {
            string name = "Window Animator With Canvas - Portrait";
            string filePath = WindowSystemEditorConst.TEMPLATE_FOLDER_PATH + "Window Animator - Canvas - Overlay - Portrait.prefab";
            
            GameObject obj;
            var library = WindowTemplateTable.Instance;
            if (library == null || library.windowAnimator_Canvas_Overlay_Portrait == null)
                obj = CreateTemplate(filePath, name);
            else
                obj = CreateTemplate(library.windowAnimator_Canvas_Overlay_Portrait, name);
            
            AdjustRectTransform(obj);
        }
        
        [MenuItem("GameObject/ZeroX/Window Template/Window Animator/Canvas - Overlay - Landscape", false, 10)]
        static void CreateWindowAnimator_Canvas_Overlay_Landscape()
        {
            string name = "Window Animator With Canvas - Landscape";
            string filePath = WindowSystemEditorConst.TEMPLATE_FOLDER_PATH + "Window Animator - Canvas - Overlay - Landscape.prefab";
            
            GameObject obj;
            var library = WindowTemplateTable.Instance;
            if (library == null || library.windowAnimator_Canvas_Overlay_Landscape == null)
                obj = CreateTemplate(filePath, name);
            else
                obj = CreateTemplate(library.windowAnimator_Canvas_Overlay_Landscape, name);
            
            AdjustRectTransform(obj);
        }
        
        [MenuItem("GameObject/ZeroX/Window Template/Window Animator/Overlay", false, 10)]
        static void CreateWindowAnimator_Overlay()
        {
            string name = "Window Animator - Overlay";
            string filePath = WindowSystemEditorConst.TEMPLATE_FOLDER_PATH + "Window Animator - Overlay.prefab";
            
            GameObject obj;
            var library = WindowTemplateTable.Instance;
            if (library == null || library.windowAnimator_Overlay == null)
                obj = CreateTemplate(filePath, name);
            else
                obj = CreateTemplate(library.windowAnimator_Overlay, name);
            
            AdjustRectTransform(obj);
        }
        
        [MenuItem("GameObject/ZeroX/Window Template/Window Animator/Non Overlay", false, 10)]
        static void CreateWindowAnimator_NonOverlay()
        {
            string name = "Window Animator - Non Overlay";
            string filePath = WindowSystemEditorConst.TEMPLATE_FOLDER_PATH + "Window Animator - Non Overlay.prefab";
            
            GameObject obj;
            var library = WindowTemplateTable.Instance;
            if (library == null || library.windowAnimator_NonOverlay == null)
                obj = CreateTemplate(filePath, name);
            else
                obj = CreateTemplate(library.windowAnimator_NonOverlay, name);
            
            AdjustRectTransform_NotAnchor(obj);
        }
        
        #endregion

        #region Window DOTween

        [MenuItem("GameObject/ZeroX/Window Template/Window DOTween/Canvas - Overlay - Portrait", false, 10)]
        static void CreateWindowDOTween_Canvas_Overlay_Portrait()
        {
            string name = "Window DOTween With Canvas - Portrait";
            string filePath = WindowSystemEditorConst.TEMPLATE_FOLDER_PATH + "Window DOTween - Canvas - Overlay - Portrait.prefab";
            
            GameObject obj;
            var library = WindowTemplateTable.Instance;
            if (library == null || library.windowDOTween_Canvas_Overlay_Portrait == null)
                obj = CreateTemplate(filePath, name);
            else
                obj = CreateTemplate(library.windowDOTween_Canvas_Overlay_Portrait, name);
            
            AdjustRectTransform(obj);
        }
        
        [MenuItem("GameObject/ZeroX/Window Template/Window DOTween/Canvas - Overlay - Landscape", false, 10)]
        static void CreateWindowDOTween_Canvas_Overlay_Landscape()
        {
            string name = "Window DOTween With Canvas - Landscape";
            string filePath = WindowSystemEditorConst.TEMPLATE_FOLDER_PATH + "Window DOTween - Canvas - Overlay - Landscape.prefab";
            
            GameObject obj;
            var library = WindowTemplateTable.Instance;
            if (library == null || library.windowDOTween_Canvas_Overlay_Landscape == null)
                obj = CreateTemplate(filePath, name);
            else
                obj = CreateTemplate(library.windowDOTween_Canvas_Overlay_Landscape, name);
            
            AdjustRectTransform(obj);
        }
        
        [MenuItem("GameObject/ZeroX/Window Template/Window DOTween/Overlay", false, 10)]
        static void CreateWindowDOTween_Overlay()
        {
            string name = "Window DOTween - Overlay";
            string filePath = WindowSystemEditorConst.TEMPLATE_FOLDER_PATH + "Window DOTween - Overlay.prefab";
            
            GameObject obj;
            var library = WindowTemplateTable.Instance;
            if (library == null || library.windowDOTween_Overlay == null)
                obj = CreateTemplate(filePath, name);
            else
                obj = CreateTemplate(library.windowDOTween_Overlay, name);
            
            AdjustRectTransform(obj);
        }
        
        [MenuItem("GameObject/ZeroX/Window Template/Window DOTween/Non Overlay", false, 10)]
        static void CreateWindowDOTween_NonOverlay()
        {
            string name = "Window DOTween - Non Overlay";
            string filePath = WindowSystemEditorConst.TEMPLATE_FOLDER_PATH + "Window DOTween - Non Overlay.prefab";
            
            GameObject obj;
            var library = WindowTemplateTable.Instance;
            if (library == null || library.windowDOTween_NonOverlay == null)
                obj = CreateTemplate(filePath, name);
            else
                obj = CreateTemplate(library.windowDOTween_NonOverlay, name);
            
            AdjustRectTransform_NotAnchor(obj);
        }

        #endregion
    }
}*/