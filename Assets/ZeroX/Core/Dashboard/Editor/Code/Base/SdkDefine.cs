using System;
using UnityEditor;
using UnityEngine;

namespace ZeroX.DashboardSystem
{
    [CreateAssetMenu(menuName = ZDBConst.SOMenuCreate.Root + "SDK Define")]
    public class SdkDefine : ScriptableObject
    {
        [SerializeField] private string displayName;
        
        [TextArea(3, 5)]
        [SerializeField] private string sdkTypeCheck; //Type dùng để kiểm tra xem sdk thật sự được import chưa

        [SerializeField] private string downloadUrl = "";
        [SerializeField] private string importPackageName = "";
        
        
        public string Define => this.name;

        public string DisplayName => displayName;

        public string SdkTypeCheck => sdkTypeCheck;


        public string DownloadUrl => downloadUrl;

        public string ImportPackageName => importPackageName;

        
        

        public bool HasType()
        {
            return Type.GetType(sdkTypeCheck) != null;
        }
        
        public void OpenDownloadUrlOrImportPackage()
        {
            if (string.IsNullOrEmpty(importPackageName) == false)
            {
                UnityEditor.PackageManager.Client.Add(importPackageName);
                return;
            }

            if (string.IsNullOrEmpty(downloadUrl) == false)
            {
                Application.OpenURL(downloadUrl);
                return;
            }

            EditorUtility.DisplayDialog("No import data", "No data to download or import", "Ok");
        }
    }
}