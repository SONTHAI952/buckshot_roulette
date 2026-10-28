using UnityEngine;
using UnityEngine.Audio;

namespace ZeroX.Extensions
{
    public static class AudioMixerExtension
    {
        public static void SetVolume(this AudioMixer audioMixer, string name, float volume)
        {
            if (volume > 0)
            {
                volume = Mathf.Lerp(-30, 0f, volume);
                audioMixer.SetFloat(name, volume);
            }
            else
                audioMixer.SetFloat(name, -80);
        }

        public static float GetVolume(this AudioMixer audioMixer, string name)
        {
            float db;
            audioMixer.GetFloat(name, out db);

            if (db <= -30)
                return 0f;

            return (db + 30) / (30);
        }
    }
}