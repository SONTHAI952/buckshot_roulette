using UnityEngine;

namespace ZeroX.SafeAreaSystem
{
    [System.Serializable]
    public class BannerAreaConfig_Specific
    {
        [SerializeField] private bool enable = false;
        [SerializeField] private ConfigOption configOption = ConfigOption.UseGlobalConfig;
        [SerializeField] private BannerAreaConfig config = new BannerAreaConfig();



        public bool Enable => enable && SafeAreaGlobalConfig.bannerArea.enable;

        public BannerAreaConfig Config
        {
            get
            {
                if (configOption == ConfigOption.UseGlobalConfig)
                    return SafeAreaGlobalConfig.bannerArea.config;

                return config;
            }
        }
    }
}