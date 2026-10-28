using UnityEngine;

namespace ZeroX.SafeAreaSystem
{
    [CreateAssetMenu(menuName = "ZeroX/UI/Safe Area Global Config")]
    public class SafeAreaGlobalConfigAsset : ScriptableObject
    {
        [Header("")]
        public SafeAreaConfig_Global safeArea = new SafeAreaConfig_Global();

        [Header("")]
        public BannerAreaConfig_Global bannerArea = new BannerAreaConfig_Global();

        [Header("")]
        public MinAreaConfig_Global minArea = new MinAreaConfig_Global();

        
        
        
        
        //Static
        private static SafeAreaGlobalConfigAsset instance;

        public static SafeAreaGlobalConfigAsset Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<SafeAreaGlobalConfigAsset>("ZeroX/SafeAreaSystem/Safe Area Global Config");
                    if (instance == null)
                    {
                        Debug.LogError("Safe Area Global Config Asset not exist at path: ZeroX/SafeAreaSystem/Safe Area Global Config");
                    }
                }
                
                return instance;
            }
        }
    }
}