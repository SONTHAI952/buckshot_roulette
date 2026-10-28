using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ZeroX.DashboardSystem
{
    public class ZeroXDashboardWindowEditor : EditorWindow
    {
        //Const
        private const float Padding = 10;
        
        
        //Temp
        [NonSerialized] private List<GroupDashboardDrawer> listGroupDashboardDrawer;
        private Vector2 scrollPosition;
        [NonSerialized] private GroupDashboardDrawer openingDrawer;
        
        
        
        
        
        [MenuItem("Tools/ZeroX/Dashboard", priority = 0)]
        public static void OpenWindow()
        {
            var w = GetWindow<ZeroXDashboardWindowEditor>();
            w.titleContent = new GUIContent("ZeroX Dashboard");
        }

        
        
        #region Unity Method

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(Padding);
            
            GUILayout.BeginVertical();
            GUILayout.Space(Padding);
            
            Draw();
            
            GUILayout.EndVertical();
            
            GUILayout.Space(Padding);
            GUILayout.EndHorizontal();
        }

        #endregion


        
        #region Draw

        private void Draw()
        {
            CreateListGroupDashboardDrawer();
            CheckAndGetOpeningDrawer();
            
            if (openingDrawer == null)
            {
                DrawSelf();
            }
            else
            {
                DrawOpeningDrawer();
            }
        }

        
        
        #region Draw Self

        private void DrawSelf()
        {
            GUILayout.Label("ZeroX Dashboard", ZDBStyle.Label.MiddleCenter_Bold_20, GUILayout.Height(28));
            
            GUILayout.Space(15);
            
            DrawListDrawerPreview();
        }
        
        private void DrawListDrawerPreview()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            
            
            foreach (var drawer in listGroupDashboardDrawer)
            {
                ZDBGUILayout.DrawHorizontalLine();
                GUILayout.Space(10);
                
                drawer.DrawPreview();
                if (drawer.IsClickOpen)
                {
                    ZDBSave.OpeningDBDrawer.Group = drawer.id;
                }
                
                GUILayout.Space(10);
            }
            
            ZDBGUILayout.DrawHorizontalLine();
            
            
            
            GUILayout.EndScrollView();
        }

        #endregion

        

        private void DrawOpeningDrawer()
        {
            openingDrawer.Draw();
            if (openingDrawer.IsClickClose)
            {
                openingDrawer = null;
                ZDBSave.OpeningDBDrawer.Group = "";
            }
        }

        

        #endregion


        
        #region Utility

        private void CreateListGroupDashboardDrawer()
        {
            if (listGroupDashboardDrawer != null)
                return;

            
            var groupDashboardDrawers = Resources.LoadAll<GroupDashboardDrawer>(ZDBConst.ResourcesPath.RootFolder);
            listGroupDashboardDrawer = groupDashboardDrawers.OrderBy(d => d.order).ToList();
        }

        private void CheckAndGetOpeningDrawer()
        {
            if(openingDrawer != null)
                return;
            
            //Id
            string openingDrawerId = ZDBSave.OpeningDBDrawer.Group;
            if (string.IsNullOrEmpty(openingDrawerId))
                return;
            
            
            //Drawer
            openingDrawer = listGroupDashboardDrawer.Find(d => d.id == openingDrawerId);
            if (openingDrawer == null)
            {
                ZDBSave.OpeningDBDrawer.Group = "";
                return;
            }
        }
        
        #endregion
    }
}