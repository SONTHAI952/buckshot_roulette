using UnityEngine;

namespace ZeroX.AudioSystem
{
    public static class AudioAssetExtension
    {
        public static AudioSourceController PlayIfNotNull(this AudioAsset audioAsset)
        {
            if (audioAsset == null)
                return null;
            
            return AudioManager.Play(audioAsset);
        }
        
        public static void PlayOnAudioSourceIfNotNull(this AudioAsset audioAsset, AudioSource audioSource)
        {
            if (audioAsset == null)
                return;
            
            AudioManager.PlayOnAudioSource(audioAsset, audioSource);
        }
    }
}