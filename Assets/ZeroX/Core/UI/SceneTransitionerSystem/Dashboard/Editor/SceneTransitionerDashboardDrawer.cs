using System;
using UnityEditor;
using UnityEngine;
using ZeroX.SceneTransitionerSystem;
using ZeroX.SceneTransitionerSystem.Editor;

namespace ZeroX.DashboardSystem
{
    [CreateAssetMenu(menuName = ZDBConst.SOMenuCreate.Core + "Scene Transitioner")]
    public class SceneTransitionerDashboardDrawer : SystemDashboardDrawer
    {
        //Temp
        [NonSerialized] private Editor sceneTransitionerSettingEditor;
        
        protected override void DrawBody()
        {
            DrawWindowTemplateTable();
        }
        
        
        private void DrawWindowTemplateTable()
        {
            bool foldout = DrawFoldout("Settings");
            if(foldout == false)
                return;


            if (SceneTransitionerSetting.HasInstance() == false)
            {
                if (ZDBGUILayout.Button("Create Setting"))
                {
                    SceneTransitionerEditor.CreateSetting();
                    sceneTransitionerSettingEditor = null;
                }
                
                return;
            }
            
            
            if (sceneTransitionerSettingEditor == null)
            {
                sceneTransitionerSettingEditor = Editor.CreateEditor(SceneTransitionerSetting.Instance);
            }
            
            
            sceneTransitionerSettingEditor.OnInspectorGUI();
        }
    }
}