using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ZeroX.DashboardSystem
{
    [CreateAssetMenu(menuName = ZDBConst.SOMenuCreate.Root + "Group Dashboard Drawer")]
    public class GroupDashboardDrawer : DashboardDrawer
    {
        public int order = 0;
        public string title = "";
        public string folderName = "";
        public Texture2D icon = null;
        
        
        [TextArea(3, 5)]
        public string shortDescription = "";
        
        
        [TextArea(5, 10)]
        public string fullDescription = "";


        //Property
        public bool IsClickOpen { get; private set; }
        public bool IsClickClose { get; private set; }
        
        
        
        //Temp
        [NonSerialized] private List<SystemDashboardDrawer> listSystemDashboardDrawer;
        private Vector2 scrollPosition;
        [NonSerialized] private SystemDashboardDrawer openingDrawer;
        
        
        
        public override void DrawPreview()
        {
            GUILayout.BeginHorizontal();
            
            //Icon
            ZDBGUILayout.DrawIcon(icon, 40, 40);
            GUILayout.Space(15);
            
            
            //Title and short description
            GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
            GUILayout.Label(title, ZDBStyle.Label.MiddleLeft_20, GUILayout.ExpandWidth(false));
            
            string shortDesc = string.IsNullOrEmpty(shortDescription) ? "(No description)" : shortDescription;
            ZDBGUILayout.Label(shortDesc, GUILayout.ExpandWidth(false));
            GUILayout.EndVertical();
            
            
            //Space
            GUILayout.FlexibleSpace();
            
            
            //Button Open
            IsClickOpen = ZDBGUILayout.DrawButtonWithIcon(ZDBMedia.Next, 40, 40);
            
            GUILayout.EndHorizontal();
        }

        public override void Draw()
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
            DrawHeader();
            GUILayout.Space(15);
            DrawEnableDisableAllSdkAvailable();
            GUILayout.Space(15);
            DrawListSystemDrawerPreview();
        }
        
        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            IsClickClose = ZDBGUILayout.DrawButtonWithIcon(ZDBMedia.Back, 30, 30);
            
            
            
            GUILayout.Label(title, ZDBStyle.Label.MiddleCenter_Bold_20, GUILayout.ExpandWidth(true));
            
            ZDBGUILayout.DrawIcon(icon, 30, 30);
            GUILayout.EndHorizontal();
        }
        
        private void DrawEnableDisableAllSdkAvailable()
        {
            GUILayout.BeginHorizontal();

            if (ZDBGUILayout.Button("Enable All SDK Available"))
            {
                EnableAllSdkAvailable();
            }
            
            if (ZDBGUILayout.Button("Disable All SDK"))
            {
                DisableAllSdkAvailable();
            }
            
            GUILayout.EndHorizontal();
        }
        
        private void DrawListSystemDrawerPreview()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            
            
            foreach (var drawer in listSystemDashboardDrawer)
            {
                ZDBGUILayout.DrawHorizontalLine();
                GUILayout.Space(10);
                
                drawer.DrawPreview();
                if (drawer.IsClickOpen)
                {
                    ZDBSave.OpeningDBDrawer.System = drawer.id;
                    GUI.FocusControl(null);
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
                ZDBSave.OpeningDBDrawer.System = "";
            }
        }

        
        
        

        #region Utility

        public List<SystemDashboardDrawer> GetAllSystemDashboardDrawerFromResources()
        {
            var groupDashboardDrawers = Resources.LoadAll<SystemDashboardDrawer>(Path.Combine(ZDBConst.ResourcesPath.RootFolder, folderName));
            return groupDashboardDrawers.OrderBy(d => d.order).ToList();
        }

        private void CreateListGroupDashboardDrawer()
        {
            if (listSystemDashboardDrawer != null)
                return;

            listSystemDashboardDrawer = GetAllSystemDashboardDrawerFromResources();
        }
        
        
        private void CheckAndGetOpeningDrawer()
        {
            if(openingDrawer != null)
                return;
            
            //Id
            string openingDrawerId = ZDBSave.OpeningDBDrawer.System;
            if (string.IsNullOrEmpty(openingDrawerId))
                return;
            
            
            //Drawer
            openingDrawer = listSystemDashboardDrawer.Find(d => d.id == openingDrawerId);
            if (openingDrawer == null)
            {
                ZDBSave.OpeningDBDrawer.System = "";
                return;
            }
        }


        public List<string> GetAllDefineAvailable()
        {
            List<string> listDefine = new List<string>();
            var listSystemDrawer = GetAllSystemDashboardDrawerFromResources();
            foreach (var drawer in listSystemDrawer)
            {
                listDefine.AddRange(drawer.GetAllDefineAvailable());
            }

            return listDefine;
        }
        
        public List<string> GetAllDefine()
        {
            List<string> listDefine = new List<string>();
            var listSystemDrawer = GetAllSystemDashboardDrawerFromResources();
            foreach (var drawer in listSystemDrawer)
            {
                listDefine.AddRange(drawer.GetAllDefineAvailable());
            }

            return listDefine;
        }

        private void EnableAllSdkAvailable()
        {
            var listDefine = GetAllDefineAvailable();
            ZDBUtility.SetDefines(listDefine);
        }

        private void DisableAllSdkAvailable()
        {
            var listDefine = GetAllDefine();
            ZDBUtility.RemoveDefines(listDefine);
        }

        #endregion
    }
}