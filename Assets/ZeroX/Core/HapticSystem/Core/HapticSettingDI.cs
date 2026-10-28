using ZeroX.SaveGameSystem;

namespace ZeroX.HapticSystem
{
    public static class HapticSettingDI
    {
        private const string hapticSetting_key = "hapticSetting";
        private const string enableHaptic_key = "enableHaptic";
        
        private static SGNode HapticSettingNode => SaveGame.Default["hapticSetting"];


        private static bool? enableHapticCached = null;
        

        public static bool EnableHaptic
        {
            get
            {
                if(enableHapticCached == null)
                    enableHapticCached = SaveGame.Default[enableHaptic_key].GetBool(true);

                return enableHapticCached.Value;
            }
            set
            {
                SaveGame.Default[enableHaptic_key].SetBool(value);
                enableHapticCached = value;
            }
        }
    }
}