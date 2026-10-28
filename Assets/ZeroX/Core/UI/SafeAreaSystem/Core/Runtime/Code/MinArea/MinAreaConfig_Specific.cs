using UnityEngine;

namespace ZeroX.SafeAreaSystem
{
    [System.Serializable]
    public class MinAreaConfig_Specific
    {
        [SerializeField] private bool enable = false;
        [SerializeField] private ConfigOption configOption = ConfigOption.Custom;
        [SerializeField] private MinAreaConfig config = new MinAreaConfig();
        
        
        
        public bool Enable => enable && SafeAreaGlobalConfig.minArea.enable;

        public MinAreaConfig Config
        {
            get
            {
                if (configOption == ConfigOption.UseGlobalConfig)
                    return SafeAreaGlobalConfig.minArea.config;

                return config;
            }
        }
    }
}