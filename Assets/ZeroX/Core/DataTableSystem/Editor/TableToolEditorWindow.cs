using UnityEditor;
using UnityEngine;


namespace ZeroX.DataTableSystem.Editors
{
    public class TableToolEditorWindow : EditorWindow
    {
        private Vector2 scrollPos = Vector2.zero;

        private TableToolDrawer tableToolDrawer = new TableToolDrawer();


        [MenuItem("Tools/ZeroX/Data Table System/Table Tool")]
        private static void MenuOpen()
        {
            var w = GetWindow<TableToolEditorWindow>("Table Tool");
            w.Show();
        }
        
        
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.Space(15);
            
            tableToolDrawer.DrawChooseTableAsset();
            tableToolDrawer.Draw();
            
            GUILayout.Space(15);
            
            tableToolDrawer.DrawTableAssetEditor();
            
            EditorGUILayout.EndScrollView();
        }
    }
}