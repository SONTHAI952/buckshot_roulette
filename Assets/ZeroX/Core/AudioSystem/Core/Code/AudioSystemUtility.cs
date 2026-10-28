using UnityEngine;

namespace ZeroX.AudioSystem
{
    public static class AudioSystemUtility
    {
        public static void ResetAudioSource(AudioSource audioSource)
        {
            audioSource.Stop();
            
            audioSource.outputAudioMixerGroup = null;
            audioSource.mute = false;
            audioSource.bypassEffects = false;
            audioSource.bypassListenerEffects = false;
            audioSource.bypassReverbZones = false;
            audioSource.playOnAwake = true;
            audioSource.loop = false;

            audioSource.priority = 128;
            audioSource.volume = 1;
            audioSource.pitch = 1;
            audioSource.panStereo = 0;
            audioSource.spatialBlend = 0;
            audioSource.reverbZoneMix = 1;

            audioSource.dopplerLevel = 1;
            audioSource.spread = 0;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.minDistance = 1;
            audioSource.maxDistance = 500;
        }
    }
}