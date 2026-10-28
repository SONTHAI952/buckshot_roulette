using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZeroX.DataTableSystem.Editors
{
    public partial class TableToolDrawer
    {
        private void DrawTab_Misc()
        {
            GUILayout.BeginVertical();
            
            DrawLogRowType();
            
            DrawRenameAllRow();
            GUILayout.EndVertical();
            
        }

        private void DrawLogRowType()
        {
            if (GUILayout.Button("Copy FullName Declared Row Type", GUILayout.Height(25), GUILayout.ExpandWidth(true)) == false)
                return;

            var rowType = SOTableReflection.GetDeclaredRowType(tableAsset);
            GUIUtility.systemCopyBuffer = rowType.FullName;
            Debug.Log(rowType.FullName);
        }

        
        
        private void DrawRenameAllRow()
        {
            if (SOTableReflection.GetDeclaredRowType(tableAsset).IsSubclassOf(typeof(ScriptableObject)) == false)
                return;
            
            GUILayout.Space(25);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            
            TableToolSetting.Import.SORowNameStyle = (SORowNameStyle) EditorGUILayout.EnumPopup("SO Row Name Style", TableToolSetting.Import.SORowNameStyle);

            if (GUILayout.Button("Rename All Row", GUILayout.Height(25), GUILayout.ExpandWidth(true)) == false)
            {
                GUILayout.EndVertical();
                return;
            }


            var listRow = SOTableReflection.GetListRow(tableAsset);
            
            
            //Đổi tên tất cả row sang temp trước
            for (int i = 0; i < listRow.Count; i++)
            {
                var row = listRow[i];
                if(row == null)
                    continue;

                var rowSO = (ScriptableObject)row;
                
                string assetPath = AssetDatabase.GetAssetPath(rowSO);
                string tempRowName = "zerox_row_temp_" + i;
                AssetDatabase.RenameAsset(assetPath, tempRowName);
            }
            
            
            //Đổi sang tên thật
            for (int i = 0; i < listRow.Count; i++)
            {
                var row = listRow[i];
                if(row == null)
                    continue;

                var rowSO = (ScriptableObject)row;
                
                string assetPath = AssetDatabase.GetAssetPath(rowSO);
                string rowName = SOTableReflection.Invoke_EditorSO_GenerateRowName(tableAsset, row, i);
                AssetDatabase.RenameAsset(assetPath, rowName);
            }
            
            GUILayout.EndVertical();
        }
    }
}