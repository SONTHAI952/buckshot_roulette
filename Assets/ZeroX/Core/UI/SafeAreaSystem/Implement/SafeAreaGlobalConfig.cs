using UnityEngine;

namespace ZeroX.SafeAreaSystem
{
    public static class SafeAreaGlobalConfig
    {
        public static SafeAreaConfig_Global safeArea = new SafeAreaConfig_Global();
        public static BannerAreaConfig_Global bannerArea = new BannerAreaConfig_Global();
        public static MinAreaConfig_Global minArea = new MinAreaConfig_Global();




        
        [RuntimeInitializeOnLoadMethod]
        private static void AutoInitialize()
        {
            var configAsset = SafeAreaGlobalConfigAsset.Instance;
            
            safeArea = configAsset.safeArea.Clone();
            bannerArea = configAsset.bannerArea.Clone();
            minArea = configAsset.minArea.Clone();
        }
    }
}