using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ZeroX.DashboardSystem
{
    public abstract class SystemDashboardDrawer : DashboardDrawer
    {
        public int order;
        public string title = "";
        public Texture2D icon = null;
        
        [Header("")]
        [TextArea(3, 5)]
        public string shortDescription = "";
        
        [Header("")]
        [TextArea(5, 20)]
        public string fullDescription = "";

        [Header("SDK")]
        public List<SdkDefine> listRequiredSdkDefine = new List<SdkDefine>();
        public List<SdkDefine> listOptionalSdkDefine = new List<SdkDefine>();


        //Property
        private Texture2D Icon => icon == null ? ZDBMedia.EmptySystemIcon : icon;
        public bool IsClickOpen { get; private set; }
        public bool IsClickClose { get; private set; }
        
        
        
        
        //Temp
        private Vector2 scrollPosition;
        
        
        
        public override void DrawPreview()
        {
            GUILayout.BeginHorizontal();
            
            //Icon
            ZDBGUILayout.DrawIcon(Icon, 40, 40);
            GUILayout.Space(15);
            
            
            //Title and short description
            GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, ZDBStyle.Label.MiddleLeft_20, GUILayout.ExpandWidth(false));
            
            //Tick
            DrawTickOnPreview();
            GUILayout.EndHorizontal();
            
            string shortDesc = string.IsNullOrEmpty(shortDescription) ? "(No description)" : shortDescription;
            ZDBGUILayout.Label(shortDesc, GUILayout.ExpandWidth(false));
            GUILayout.EndVertical();
            
            
            //Space
            GUILayout.FlexibleSpace();
            
            
            //Button Open
            IsClickOpen = ZDBGUILayout.DrawButtonWithIcon(ZDBMedia.Next, 40, 40);
            
            GUILayout.EndHorizontal();
        }

        private void DrawTickOnPreview()
        {
            if (IsSdkEnabled())
                ZDBGUILayout.DrawIcon(ZDBMedia.Tick, 20, 20, 5);
        }

        
        
        public override void Draw()
        {
            DrawHeader();
            GUILayout.Space(15);
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            //Description
            string fullDes = string.IsNullOrEmpty(fullDescription) ? "(No description)" : fullDescription;
            ZDBGUILayout.SelectableLabelWithBG(fullDes, 5);
            
            //SDK
            DrawSDK();
            
            //Decor
            GUILayout.Space(5);
            ZDBGUILayout.DrawHorizontalLine();
            GUILayout.Space(15);
            
            //Body - mở rộng label width để nhìn cho tiện
            DrawBodyOverrideLabelWidth();
            
            //Padding Bottom
            GUILayout.Space(200);
            GUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            IsClickClose = ZDBGUILayout.DrawButtonWithIcon(ZDBMedia.Back, 30, 30);
            
            GUILayout.Label(title, ZDBStyle.Label.MiddleCenter_Bold_20, GUILayout.ExpandWidth(true));
            
            ZDBGUILayout.DrawIcon(Icon, 30, 30);
            GUILayout.EndHorizontal();
        }

        private void DrawSDK()
        {
            DrawSDK_Required();
            GUILayout.Space(20);
            DrawSDK_Optional();
        }

        private void DrawSDK_Required()
        {
            if(listRequiredSdkDefine.Count <= 0)
                return;
            
            bool isSdkEnabled = AllRequiredSdkEnabled();
            bool isSdkEnableNew = ZDBGUILayout.DrawToggleRequiredSdk("Required SDKs", isSdkEnabled, () => DrawButtonImportSDKRequired(isSdkEnabled));
            DrawSDK_RequiredAll_Note();
            
            
            if(isSdkEnabled == isSdkEnableNew)
                return;
            
            if(isSdkEnableNew)
                EnableAllRequiredSdk();
            else
                DisableAllRequiredSdk();
            
        }

        private void DrawButtonImportSDKRequired(bool isAllRequiredSdkEnabled)
        {
            if(isAllRequiredSdkEnabled)
                return;
            
            var listSdkDefineToUse = listRequiredSdkDefine.Where(d => d.HasType() == false).ToList();
            if(listSdkDefineToUse.Count == 0)
                return;
            
            
            if(ZDBGUILayout.DrawButtonWithIcon(ZDBMedia.Download, 25, 25, 9) == false)
                return;

            
            if (listSdkDefineToUse.Count == 1)
            {
                listSdkDefineToUse[0].OpenDownloadUrlOrImportPackage();
            }
            else
            {
                GenericMenu genericMenu = new GenericMenu();
                foreach (var sdkDefine in listSdkDefineToUse)
                {
                    genericMenu.AddItem(new GUIContent(sdkDefine.DisplayName), false, () =>
                    {
                        sdkDefine.OpenDownloadUrlOrImportPackage();
                    });
                }
            
                genericMenu.ShowAsContext();
            }
        }

        private void DrawSDK_RequiredAll_Note()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            for (int i = 0; i < listRequiredSdkDefine.Count; i++)
            {
                sb.Append(listRequiredSdkDefine[i].DisplayName);
                if(i < listRequiredSdkDefine.Count - 1)
                    sb.Append(", ");
            }
            sb.Append(")");
            ZDBGUILayout.Label(sb.ToString());
        }

        private void DrawSDK_Optional()
        {
            if(listOptionalSdkDefine.Count <= 0)
                return;

            GUILayout.Label("Optional SDKs", ZDBStyle.Label.MiddleLeft_Bold_15);
            foreach (SdkDefine sdkDefine in listOptionalSdkDefine)
            {
                DrawSDK_Optional(sdkDefine);
            }
        }

        private void DrawSDK_Optional(SdkDefine sdkDefine)
        {
            bool isSdkEnabled = ZDBUtility.HasDefine(sdkDefine.Define);
            bool isSdkEnableNew = ZDBGUILayout.DrawToggleOptionalSdk(sdkDefine.DisplayName, isSdkEnabled, () => DrawButtonImportSDKOptional(sdkDefine, isSdkEnabled));
            if(isSdkEnabled == isSdkEnableNew)
                return;
            
            if(isSdkEnableNew)
                EnableOptionalSdk(sdkDefine);
            else
                DisableOptionalSdk(sdkDefine);
        }

        private void DrawButtonImportSDKOptional(SdkDefine sdkDefine, bool isSdkEnabled)
        {
            if(isSdkEnabled)
                return;
            
            if(sdkDefine.HasType())
                return;
            
            if(ZDBGUILayout.DrawButtonWithIcon(ZDBMedia.Download, 19, 19, 8) == false)
                return;
            
            sdkDefine.OpenDownloadUrlOrImportPackage();
        }

        private void DrawBodyOverrideLabelWidth()
        {
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 250;
            DrawBody();
            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
        
        protected abstract void DrawBody();
        


        #region Utility

        private bool AllRequiredSdkEnabled()
        {
            try
            {
                if (listRequiredSdkDefine.Count > 0)
                {
                    var listSdkDefineString = listRequiredSdkDefine.Select(s => s.Define).ToList();
                    if (ZDBUtility.HasAllDefine(listSdkDefineString) == false)
                        return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("Error at: " + this.name);
                return false;
            }
        }

        private bool AnyOptionalSdkEnabled()
        {
            if (listOptionalSdkDefine.Count > 0)
            {
                var listSdkDefineString = listOptionalSdkDefine.Select(s => s.Define).ToList();
                if (ZDBUtility.HasAnyDefine(listSdkDefineString) == false)
                    return false;
            }

            return true;
        }

        protected virtual bool IsSdkEnabled()
        {
            if (AllRequiredSdkEnabled() == false)
            { 
                return false;
            }

            if (AnyOptionalSdkEnabled() == false)
            {
                return false;
            }
            
            return true;
        }

        private void EnableAllRequiredSdk()
        {
            var listSdkNotImported = listRequiredSdkDefine.Where(r => r.HasType() == false).ToList();
            if (listSdkNotImported.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Missing required SDKs:");
                foreach (var sdkDefine in listSdkNotImported)
                {
                    sb.AppendLine(sdkDefine.DisplayName);
                }
                
                EditorUtility.DisplayDialog("Cannot Enable", sb.ToString(), "OK");
                return;
            }
            
            
            var listDefineString = listRequiredSdkDefine.Select(s => s.Define).ToList();
            ZDBUtility.SetDefines(listDefineString);
        }
        
        private void DisableAllRequiredSdk()
        {
            var listDefineString = listRequiredSdkDefine.Select(s => s.Define).ToList();
            ZDBUtility.RemoveDefines(listDefineString);
        }
        
        private void EnableOptionalSdk(SdkDefine sdkDefine)
        {
            if (AllRequiredSdkEnabled() == false)
            {
                EditorUtility.DisplayDialog("Cannot Enable", "You need to enable all required SDK first", "OK");
                return;
            }
            
            if (sdkDefine.HasType() == false)
            {
                EditorUtility.DisplayDialog("Cannot Enable", "Missing SDK: " + sdkDefine.DisplayName, "OK");
                return;
            }
            
            ZDBUtility.SetDefine(sdkDefine.Define);
        }
        
        private void DisableOptionalSdk(SdkDefine sdkDefine)
        {
            ZDBUtility.RemoveDefine(sdkDefine.Define);
        }

        protected bool GetFoldout(string fieldKey)
        {
            return ZDBSave.SystemDrawerFieldFoldout.Get(id, fieldKey);
        }

        protected void SetFoldout(string fieldKey, bool value)
        {
            ZDBSave.SystemDrawerFieldFoldout.Set(id, fieldKey, value);
        }

        public List<string> GetAllDefine()
        {
            List<string> listDefine = new List<string>();
            
            listDefine.AddRange(listRequiredSdkDefine.Select(s => s.Define));
            listDefine.AddRange(listOptionalSdkDefine.Select(s => s.Define));
            
            return listDefine;
        }
        
        public List<string> GetAllDefineAvailable()
        {
            List<string> listDefine = new List<string>();

            foreach (var sdkDefine in listRequiredSdkDefine)
            {
                if(sdkDefine.HasType())
                    listDefine.Add(sdkDefine.Define);
            }
            
            foreach (var sdkDefine in listOptionalSdkDefine)
            {
                if(sdkDefine.HasType())
                    listDefine.Add(sdkDefine.Define);
            }
            
            return listDefine;
        }
        
        #endregion

        
        
        #region Draw Utility

        protected bool DrawFoldout(string foldoutTitle, GUIStyle style = null)
        {
            bool isFoldout = ZDBSave.SystemDrawerFieldFoldout.Get(id, foldoutTitle);
            
            if(style == null)
                isFoldout = EditorGUILayout.Foldout(isFoldout, foldoutTitle, true);
            else
                isFoldout = EditorGUILayout.Foldout(isFoldout, foldoutTitle, true, style);
            
            ZDBSave.SystemDrawerFieldFoldout.Set(id, foldoutTitle, isFoldout);

            return isFoldout;
        }

        #endregion
    }
}