using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ZeroX.UI.Editors
{
    [CustomEditor(typeof(SetGUICameraForCanvas))]
    public class SetGUICameraForCanvasEditor : Editor
    {
        [NonSerialized] private bool isInitialized;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            InitIfNot();
        }

        private void InitIfNot()
        {
            if (isInitialized)
                return;

            isInitialized = true;

            AutoSetGuiCameraForCanvas();
        }
        
        private void AutoSetGuiCameraForCanvas()
        {
            if(EditorApplication.isPlaying)
                return;
            
            foreach (var t in targets)
            {
                SetGUICameraForCanvas comp = (SetGUICameraForCanvas)t;
                AutoSetGuiCameraForCanvas(comp);
            }
        }
        
        private void AutoSetGuiCameraForCanvas(SetGUICameraForCanvas comp)
        {
            //Nếu được mở trong prefab edit mode thì đừng set gui camera
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if(prefabStage != null && prefabStage.IsPartOfPrefabContents(comp.gameObject))
                return;
            
            //Nếu là prefab asset thì đừng set gui camera
            if(PrefabUtility.IsPartOfPrefabAsset(comp.gameObject))
                return;
            
            var canvas = comp.GetComponent<Canvas>();
            if (canvas == null)
                return;
            
            if(canvas.renderMode == RenderMode.WorldSpace)
                return;
            
            var guiCamera = FindObjectOfType<GUICamera>();
            if (guiCamera == null)
                return;
           
            canvas.worldCamera = guiCamera.GetComponentInChildren<Camera>();
            EditorUtility.SetDirty(canvas);
        }
    }
}