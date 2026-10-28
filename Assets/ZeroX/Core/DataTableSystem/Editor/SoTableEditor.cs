using UnityEditor;
using UnityEngine;
using ZeroX.DataTableSystem.SoTableSystem;

namespace ZeroX.DataTableSystem.Editors
{
    [CustomEditor(typeof(SoTable), true)]
    public class SoTableEditor : Editor
    {
        private TableToolDrawer tableToolDrawer = new TableToolDrawer();
        
        
        
        public override void OnInspectorGUI()
        {
            DrawTool();
            base.OnInspectorGUI();
        }


        private void DrawTool()
        {
            bool foldout = DrawToolFoldout();
            if(foldout == false)
                return;

            
            
            GUILayout.BeginVertical(EditorStyles.helpBox);
            
            tableToolDrawer.SetTableAsset((SoTable) target);
            tableToolDrawer.Draw();
            
            GUILayout.EndVertical();
            
            GUILayout.Space(30);
        }

        private bool DrawToolFoldout()
        {
            string key = "ZeroX.SOTable.foldout.tool";
            bool foldout = EditorPrefs.GetBool(key);
            string buttonText = foldout ? "Close Tool" : "Open Tool";

            if (GUILayout.Button(buttonText, GUILayout.Width(100)))
                foldout = !foldout;
            
            //foldout = EditorGUILayout.Foldout(foldout, "Tool", true);
            EditorPrefs.SetBool(key, foldout);

            return foldout;
        }
    }
}