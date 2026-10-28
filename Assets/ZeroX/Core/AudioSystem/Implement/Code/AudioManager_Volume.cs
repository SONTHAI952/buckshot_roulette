using UnityEngine;

namespace ZeroX.AudioSystem
{
    public partial class AudioManager
    {
        public static void SetVolume(VolumeType volumeType, float volume)
        {
            string audioMixerFloatField = VolumeTypeUtility.ToFloatFieldInAudioMixer(volumeType);
            SetVolume(audioMixerFloatField, volume);
        }

        public static float GetVolume(VolumeType volumeType)
        {
            string audioMixerFloatField = VolumeTypeUtility.ToFloatFieldInAudioMixer(volumeType);
            return GetVolume(audioMixerFloatField);
        }

        public static void SetVolumeAndSave(VolumeType volumeType, float volume)
        {
            string audioMixerFloatField = VolumeTypeUtility.ToFloatFieldInAudioMixer(volumeType);
            SetVolume(audioMixerFloatField, volume);
            
            AudioSettingDI.SetVolume(volumeType, volume);
        }
    }
}