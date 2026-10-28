using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.ToastSystem.Editors
{
    public static class ToastSettingEditor
    {
        #region Setting
        
        static Object CreateToastSetting(bool focus)
        {
            string folderPath = Path.Combine(ToastConst.ROOT_FOLDER_PATH, "Implement/Resources/ZeroX/Toast/");
            string filePath = Path.Combine(folderPath, "Toast Setting.asset");
            if (Directory.Exists(folderPath) == false)
                Directory.CreateDirectory(folderPath);

            var asset = ScriptableObject.CreateInstance<ToastSetting>();
            
            AssetDatabase.CreateAsset(asset, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (focus)
            {
                EditorUtility.FocusProjectWindow();
                var obj = AssetDatabase.LoadAssetAtPath<ToastSetting>(filePath);
                if (obj != null)
                {
                    EditorGUIUtility.PingObject(obj);
                }
            }

            return asset;
        }
        
        [MenuItem("Tools/ZeroX/Toast/Setting")]
        static void OpenSetting()
        {
            var toastSetting = Resources.Load<ToastSetting>(ToastConst.SETTING_RESOURCES_FILE_PATH);
            if (toastSetting == null)
            {
                bool confirm = EditorUtility.DisplayDialog("Cannot find Setting", "Cannot find Toast Setting!\nCreate One?", "Yes", "No");
                if (confirm)
                {
                    var newSetting = CreateToastSetting(true);
                    EditorUtility.FocusProjectWindow();
                    EditorGUIUtility.PingObject(newSetting);
                    Selection.SetActiveObjectWithContext(newSetting, null);
                    return;
                }
                else
                    return;
            }
            
            Selection.SetActiveObjectWithContext(toastSetting, null);
        }
        
        #endregion



        #region Template
        
        static void DisplayDialogCannotFindTemplate(string templateName)
        {
            string title = string.Format("Cannot find {0}!", templateName);
            string message = string.Format("Cannot find {0}\nMake sure you put the ZeroX folder in the assets directory", templateName);
            EditorUtility.DisplayDialog(title, message, "Ok");
        }

        static GameObject CreateTemplate(string fileName, string name)
        {
            var template = Resources.Load<GameObject>(ToastConst.TOAST_DEFAULT_RESOURCES_FOLDER_PATH + fileName);
            if (template == null)
            {
                DisplayDialogCannotFindTemplate(fileName);
                return null;
            }
            
            var obj = GameObject.Instantiate(template);
            obj.name = name;
            Undo.RegisterCreatedObjectUndo(obj, "Create Toast Template");

            var arr = Selection.gameObjects;
            if (arr != null && arr.Length > 0)
                obj.transform.parent = arr[0].transform;

            obj.transform.SetSiblingIndex(Int32.MaxValue);

            return obj;
        }

        [MenuItem("GameObject/ZeroX/Toast/Toast Multiple")]
        public static void CreateToastMultipleTemplate()
        {
            CreateTemplate("Toast Multiple", "Toast Multiple");
        }
        
        [MenuItem("GameObject/ZeroX/Toast/Toast Fly Up")]
        public static void CreateToastFlyUpTemplate()
        {
            CreateTemplate("Toast Fly Up", "Toast Fly Up");
        }
        
        [MenuItem("GameObject/ZeroX/Toast/Toast Single")]
        public static void CreateToastSingleTemplate()
        {
            CreateTemplate("Toast Single", "Toast Single");
        }

        #endregion
    }
}