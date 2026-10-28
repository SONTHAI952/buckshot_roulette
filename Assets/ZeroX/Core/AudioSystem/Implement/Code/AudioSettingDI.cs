using UnityEngine;
using ZeroX.SaveGameSystem;

namespace ZeroX.AudioSystem
{
    public static class AudioSettingDI
    {
        private static SGNode AudioSettingNode => SaveGame.Default["audioSetting"];
        
        /// <summary>
        /// 0 -> 1
        /// </summary>
        public static void SetVolume(VolumeType volumeType, float volume)
        {
            string key = VolumeTypeUtility.ToKeyInSaveGame(volumeType);
            AudioSettingNode[key].IntValue = (int)Mathf.Clamp(volume * 100, 0, 100);
        }

        /// <summary>
        /// 0 -> 1
        /// </summary>
        public static float GetVolume(VolumeType volumeType)
        {
            string key = VolumeTypeUtility.ToKeyInSaveGame(volumeType);
            return AudioSettingNode[key].GetInt(100) / 100f;
        }
    }
}