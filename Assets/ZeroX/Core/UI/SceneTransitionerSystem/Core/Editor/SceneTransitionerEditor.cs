using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZeroX.SceneTransitionerSystem.Editor
{
    public class SceneTransitionerEditor
    {
        #region Setting
        
        public static SceneTransitionerSetting CreateSetting()
        {
            if (Directory.Exists(SceneTransitionerConst.folderPath_Setting) == false)
                Directory.CreateDirectory(SceneTransitionerConst.folderPath_Setting);
            
            if (File.Exists(SceneTransitionerConst.filePath_Setting))
            {
                Debug.LogError("Setting file has been already exist at path: " + SceneTransitionerConst.filePath_Setting);
                return Resources.Load<SceneTransitionerSetting>(SceneTransitionerConst.filePathInResources_Setting);
            }

            var asset = ScriptableObject.CreateInstance<SceneTransitionerSetting>();
                
            AssetDatabase.CreateAsset(asset, SceneTransitionerConst.filePath_Setting);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //Focus
            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(asset);
            
            return asset;
        }

        [MenuItem("Tools/ZeroX/Scene Transitioner/Setting")]
        private static void OpenSetting()
        {
            var setting = Resources.Load<SceneTransitionerSetting>(SceneTransitionerConst.filePathInResources_Setting);

            if (setting == null)
            {
                bool confirm = EditorUtility.DisplayDialog("Cannot find Setting", "Cannot find SceneTransitioner Setting!\nCreate One?", "Yes", "No");
                if (confirm == false)
                {
                    return;
                }
                else
                    setting = CreateSetting();
            }
            
            Selection.SetActiveObjectWithContext(setting, null);
        }
        
        #endregion


        
        

        #region Template
        
        static void DisplayDialogCannotFindTemplate(string templateName)
        {
            string title = string.Format("Cannot find {0}!", templateName);
            string message = string.Format("Cannot find {0}\nMake sure you put the ZeroX folder in the Assets directory", templateName);
            EditorUtility.DisplayDialog(title, message, "Ok");
        }

        private static SceneTransitioner CreateTemplate(SceneTransitioner template)
        {
            if (template == null)
            {
                Debug.LogError("Cannot create template because template prefab is null");
                return null;
            }
                
            
            //Spawn
            var sceneTransitioner = GameObject.Instantiate(template);
            sceneTransitioner.name = template.name;
            Undo.RegisterCreatedObjectUndo(sceneTransitioner.gameObject, "Create Scene Transitioner Template");

            //Parent
            var arr = Selection.gameObjects;
            if (arr != null && arr.Length > 0)
                sceneTransitioner.transform.parent = arr[0].transform;

            //Sibling Index
            sceneTransitioner.transform.SetSiblingIndex(Int32.MaxValue);

            return sceneTransitioner;
        }

        private static SceneTransitioner GetTemplatePrefab()
        {
            //SceneTransitioner không phải là cái cần tạo nhiều nên không cần tính năng custom template
            //var setting = SceneTransitionerSetting.HasInstance() ? SceneTransitionerSetting.Instance : null;
            
            //if (setting == null || setting.template == null)
                return Resources.Load<SceneTransitioner>(SceneTransitionerConst.filePathInResources_Template);
            //else
                //return setting.template;
        }
        
        [MenuItem("GameObject/ZeroX/Scene Transitioner Template", false, 10)]
        static void CreateTemplate()
        {
            SceneTransitioner prefab = GetTemplatePrefab();
            if (prefab == null)
            {
                DisplayDialogCannotFindTemplate("Scene Transitioner Template");
                return;
            }
            
            
            CreateTemplate(prefab);
        }

        #endregion
    }
}