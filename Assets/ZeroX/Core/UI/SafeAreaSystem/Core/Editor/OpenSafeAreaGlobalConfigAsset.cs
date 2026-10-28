using UnityEditor;
using UnityEngine;

namespace ZeroX.SafeAreaSystem.Editors
{
    public static class MenuEnableBannerArea
    {
        [MenuItem("Tools/ZeroX/Safe Area/Global Config")]
        public static void OpenGlobalConfig()
        {
            var instance = SafeAreaGlobalConfigAsset.Instance;
            EditorGUIUtility.PingObject(instance);
            Selection.SetActiveObjectWithContext(instance, instance);
        }
        
        
        [MenuItem("Tools/ZeroX/Safe Area/Switch Banner Area")]
        private static void SwitchBannerArea()
        {
            SafeAreaGlobalConfig.bannerArea.enable = !SafeAreaGlobalConfig.bannerArea.enable;
            Debug.Log("SafeAreaGlobalConfig.enableBannerArea switched to " + SafeAreaGlobalConfig.bannerArea.enable);
        }
        
        [MenuItem("Tools/ZeroX/Safe Area/Switch Min Area")]
        private static void SwitchMinArea()
        {
            SafeAreaGlobalConfig.minArea.enable = !SafeAreaGlobalConfig.minArea.enable;
            Debug.Log("SafeAreaGlobalConfig.enableMinArea switched to " + SafeAreaGlobalConfig.minArea.enable);
        }
    }
}