using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZeroX.AudioSystem
{
    [CreateAssetMenu(menuName = "ZeroX/Audio System/Audio Asset")]
    public class AudioAsset : ScriptableObject
    {
        [AudioGroupIdField]
        [SerializeField] private long audioGroupId = -1;
        [SerializeField] private AudioClip audioClip;
        
        [Range(0, 1)]
        [SerializeField] private float volumeFactor = 1;

        
        //Const
        public const string fn_audioGroupId = "audioGroupId";
        public const string fn_audioClip = "audioClip";

        
        //Temp
        [NonSerialized]
        public float lastTimePlay = -999f; //Sử dụng để tránh cộng hưởng âm thanh
        
        
        
        
        public long AudioGroupId => audioGroupId;
        public AudioClip AudioClip => audioClip;

        public void SetAudioClip(AudioClip audioClip)
        {
            this.audioClip = audioClip;
        }

        public float Length => audioClip.length;

        public float VolumeFactor => volumeFactor;




        #region Play

        public AudioSourceController Play()
        {
            return AudioManager.Play(this);
        }
        
        public void PlayOnAudioSource(AudioSource audioSource)
        {
            AudioManager.PlayOnAudioSource(this, audioSource);
        }

        public AudioSourceController PlayAvoidResonance(float time = AudioManager.AvoidResonanceTime)
        {
            if (HasRiskOfResonance(time))
                return null;
            
            return AudioManager.Play(this);
        }
        
        public void PlayOnAudioSourceAvoidResonance(AudioSource audioSource, float time = AudioManager.AvoidResonanceTime)
        {
            if (HasRiskOfResonance(time))
                return;
            
            AudioManager.PlayOnAudioSource(this, audioSource);
        }

        #endregion


        
        #region Volume

        public void SetVolumeForAudioSource(AudioSource audioSource, float volume)
        {
            audioSource.volume = volumeFactor * Mathf.Clamp01(volume);
        }

        public float GetVolumeFromAudioSource(AudioSource audioSource)
        {
            return Mathf.Clamp01(audioSource.volume / volumeFactor);
        }

        #endregion
        
        
        
        public bool HasRiskOfResonance(float time)
        {
            return Time.unscaledTime - lastTimePlay < time;
        }
    }
}