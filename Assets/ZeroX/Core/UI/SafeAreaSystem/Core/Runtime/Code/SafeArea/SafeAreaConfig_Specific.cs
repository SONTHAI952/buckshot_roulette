using UnityEngine;

namespace ZeroX.SafeAreaSystem
{
    [System.Serializable]
    public class SafeAreaConfig_Specific
    {
        [SerializeField] private ConfigOption configOption = ConfigOption.UseGlobalConfig;
        [SerializeField] private SafeAreaConfig config = new SafeAreaConfig();



        public bool Enable => SafeAreaGlobalConfig.safeArea.enable;

        public SafeAreaConfig Config
        {
            get
            {
                if (configOption == ConfigOption.UseGlobalConfig)
                    return SafeAreaGlobalConfig.safeArea.config;

                return config;
            }
        }
    }
}