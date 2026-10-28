using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using ZeroX.CsvTools;
using ZeroX.DataTableSystem.SoTableSystem;
using Object = System.Object;

namespace ZeroX.DataTableSystem.Editors
{
    public partial class TableToolDrawer
    {
        //Tab
        private static int tabIndexSelected = 0;
        private string[] tabOptions = new[] {"Import", "Export", "Misc"};
        
        //Shared
        private SoTable tableAsset;
        private Editor tableAssetEditor;
        private bool foldoutTableAssetEditor = false;
        
        
        

        
        

        public void Draw()
        {
            if (tableAsset == null)
                return;
            
            GUILayout.Space(5);
            DrawTabHeader();
            GUILayout.Space(15);
            
            if(tabIndexSelected == 0)
                DrawTab_Import();
            else if(tabIndexSelected == 1)
                DrawTab_Export();
            else if(tabIndexSelected == 2)
                DrawTab_Misc();
            else
            {
                Debug.LogError("Tab not found");
            }
        }

        private void DrawTabHeader()
        {
            tabIndexSelected = GUILayout.Toolbar(tabIndexSelected, tabOptions, GUILayout.Height(20));
        }
        

        public void DrawChooseTableAsset()
        {
            var newTableAsset = (SoTable)EditorGUILayout.ObjectField("Table Asset", tableAsset, typeof(SoTable), false);
            SetTableAsset(newTableAsset);
        }

        public void DrawTableAssetEditor()
        {
            if(tableAsset == null)
                return;

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;
            foldoutTableAssetEditor = EditorGUILayout.Foldout(foldoutTableAssetEditor, "Table Asset Inspector", true, foldoutStyle);

            if (foldoutTableAssetEditor)
            {
                if (tableAssetEditor == null)
                {
                    tableAssetEditor = Editor.CreateEditor(tableAsset);
                }
                tableAssetEditor.OnInspectorGUI();
            }
        }
        
        
        public void SetTableAsset(SoTable newTableAsset)
        {
            if(tableAsset == newTableAsset)
                return;
            
            tableAsset = newTableAsset;
            tableAssetEditor = tableAsset == null ? null : Editor.CreateEditor(tableAsset);
            
            Import_OnTableAssetChanged();
            Export_OnTableAssetChanged();
        }
    }
}