using System;
using UnityEditor;
using UnityEngine;
using ZeroX.WindowSystem;

namespace ZeroX.DashboardSystem
{
    [CreateAssetMenu(menuName = ZDBConst.SOMenuCreate.Core + "Window")]
    public class WindowDashboardDrawer : SystemDashboardDrawer
    {
        //Temp
        [NonSerialized] private Editor windowTemplateTableEditor;
        [NonSerialized] private Editor globalWindowTransitionTableEditor;
        [NonSerialized] private Editor windowSortingTableEditor;
        
        
        
        protected override void DrawBody()
        {
            DrawWindowTemplateTable();
            ZDBGUILayout.HorizontalSpacingWithLine(30);
            DrawGlobalWindowTransitionTable();
            ZDBGUILayout.HorizontalSpacingWithLine(30);
            DrawWindowSortingTable();
        }

        private void DrawWindowTemplateTable()
        {
            bool foldout = DrawFoldout("Templates");
            if(foldout == false)
                return;
            
            if (windowTemplateTableEditor == null)
            {
                var instance = WindowTemplateTable.Instance;
                if (instance == null)
                {
                    EditorGUILayout.HelpBox("Window Template Table instance not exist", MessageType.Error);
                    return;
                }
                
                windowTemplateTableEditor = Editor.CreateEditor(instance);
            }
            
            
            windowTemplateTableEditor.OnInspectorGUI();
        }
        
        private void DrawGlobalWindowTransitionTable()
        {
            bool foldout = DrawFoldout("Global Window Transition");
            if(foldout == false)
                return;
            
            if (globalWindowTransitionTableEditor == null)
            {
                var instance = GlobalWindowTransitionTable.Instance;
                if (instance == null)
                {
                    EditorGUILayout.HelpBox("Global Window Transition Table instance not exist", MessageType.Error);
                    return;
                }
                
                globalWindowTransitionTableEditor = Editor.CreateEditor(instance);
            }
            
            
            globalWindowTransitionTableEditor.OnInspectorGUI();
        }
        
        private void DrawWindowSortingTable()
        {
            bool foldout = DrawFoldout("Window Sorting");
            if(foldout == false)
                return;
            
            if (windowSortingTableEditor == null)
            {
                var instance = WindowSortingTable.Instance;
                if (instance == null)
                {
                    EditorGUILayout.HelpBox("Window Sorting Table instance not exist", MessageType.Error);
                    return;
                }
                
                windowSortingTableEditor = Editor.CreateEditor(instance);
            }
            
            
            windowSortingTableEditor.OnInspectorGUI();
        }
    }
}