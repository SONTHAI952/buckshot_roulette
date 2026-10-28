using System.IO;
using GameItemSystem;
using UnityEditor;
using UnityEngine;
using ZeroX.FolderShortcut;

namespace ZeroX.DashboardSystem
{
    [CreateAssetMenu(menuName = ZDBConst.SOMenuCreate.Core + "GameItem")]
    public class GameItemDashboardDrawer : SystemDashboardDrawer
    {
        //Temp
        private Editor gameItemTableEditor;
        
        
        protected override void DrawBody()
        {
            GUILayout.BeginHorizontal();
            DrawButtonOpenFolderIcon();
            DrawButtonOpenFolderOverrideIcon();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(25);
            DrawGameItemTable();
            
        }

        private string GetGameItemFolder()
        {
            string tablePath = AssetDatabase.GetAssetPath(GameItemTable.Main);
            return Path.Combine(Path.GetDirectoryName(tablePath), "Game Item");
        }

        private void DrawButtonOpenFolderIcon()
        {
            if(ZDBGUILayout.Button("Folder Icon") == false)
                return;
            
            string folderPath = Path.Combine(GetGameItemFolder(), "Icon");
            FolderShortcutUtility.ShowFolderContents(folderPath);
        }
        
        private void DrawButtonOpenFolderOverrideIcon()
        {
            if(ZDBGUILayout.Button("Folder Override Icon") == false)
                return;

            string folderPath = Path.Combine(GetGameItemFolder(), "Override Icon");
            FolderShortcutUtility.ShowFolderContents(folderPath);
        }
        
        private void DrawGameItemTable()
        {
            bool foldout = DrawFoldout("Table");
            if(foldout == false)
                return;
            
            if (gameItemTableEditor == null)
            {
                var instance = GameItemTable.Main;
                if (instance == null)
                {
                    EditorGUILayout.HelpBox("GameItem Table instance not exist", MessageType.Error);
                    return;
                }
                
                gameItemTableEditor = Editor.CreateEditor(instance);
            }
            
            
            gameItemTableEditor.OnInspectorGUI();
        }
    }
}