using UnityEngine;

namespace ZeroX.AudioSystem
{
    public static class AudioAssetGroupExtension
    {
        public static AudioSourceController PlayRandomIfNotNull(this AudioAssetGroup audioAssetGroup)
        {
            if (audioAssetGroup == null)
                return null;

            return audioAssetGroup.PlayRandom();
        }
        
        public static void PlayRandomOnAudioSourceIfNotNull(this AudioAssetGroup audioAssetGroup, AudioSource audioSource)
        {
            if (audioAssetGroup == null)
                return;

            audioAssetGroup.PlayRandomOnAudioSource(audioSource);
        }
    }
}