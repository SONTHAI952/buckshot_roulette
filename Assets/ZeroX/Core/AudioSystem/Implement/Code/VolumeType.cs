using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.AudioSystem
{
    public enum VolumeType
    {
        Music, SFX, UI
    }

    public static class VolumeTypeUtility
    {
        private static Dictionary<VolumeType, string> dictFloatFieldInAudioMixerGroup = new Dictionary<VolumeType, string>()
        {
            {VolumeType.Music, "Music"},
            {VolumeType.SFX, "SFX"},
            {VolumeType.UI, "UI"},
        };
        
        private static Dictionary<VolumeType, string> dictKeyInSaveGame = new Dictionary<VolumeType, string>()
        {
            {VolumeType.Music, "musicVolume"},
            {VolumeType.SFX, "sfxVolume"},
            {VolumeType.UI, "uiVolume"},
        };
        
        public static string ToFloatFieldInAudioMixer(VolumeType volumeType)
        {
            if (dictFloatFieldInAudioMixerGroup.TryGetValue(volumeType, out string name))
            {
                return name;
            }

            throw new Exception("Chưa thêm VolumeType vào dict: " + volumeType);
        }

        public static string ToKeyInSaveGame(VolumeType volumeType)
        {
            if (dictKeyInSaveGame.TryGetValue(volumeType, out string name))
            {
                return name;
            }

            throw new Exception("Chưa thêm VolumeType vào dict: " + volumeType);
        }
    }
}