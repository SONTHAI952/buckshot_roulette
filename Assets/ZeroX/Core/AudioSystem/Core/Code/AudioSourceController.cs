using System;
using UnityEngine;
using UnityEngine.Audio;

namespace ZeroX.AudioSystem
{
    public class AudioSourceController : MonoBehaviour
    {
        public enum State
        {
            Stop, Playing, Pausing
        }
        
        
        [SerializeField] private AudioSource audioSource;
        public AudioSource AudioSource => audioSource;
        
        
        
        public bool InUse => state != State.Stop;
        [NonSerialized] protected State state = State.Stop;
        [NonSerialized] public float volumeFactor = 1;
        
        
        

        private void Update()
        {
            if (state == State.Playing)
            {
                if (audioSource.loop == false && audioSource.isPlaying == false && audioSource.time == 0)
                {
                    Stop();
                }
            }
        }

        #region Play - Pause - UnPause - Stop

        public void Play()
        {
            //Tránh gọi play khi đang play, sẽ bị play lại từ đầu
            if (state == State.Stop || state == State.Pausing)
            {
                state = State.Playing;
                gameObject.SetActive(true);
                audioSource.Play();
            }
        }
        
        public void Pause()
        {
            if (state == State.Playing)
            {
                state = State.Pausing;
                audioSource.Pause();
            }
        }

        public void UnPause()
        {
            if (state == State.Pausing)
            {
                state = State.Playing;
                audioSource.UnPause();
            }
        }

        /// <summary>
        /// Stop and put in pool
        /// </summary>
        public void Stop()
        {
            state = State.Stop;
            audioSource.Stop();

            AudioSourceControllerPool.PutInPool(this);
        }

        //Bonus
        public void PlayFromStart()
        {
            state = State.Playing;
            audioSource.Stop();
            audioSource.Play();
        }

        #endregion

        #region Audio Source

        public AudioClip AudioClip
        {
            get => audioSource.clip;
            set => audioSource.clip = value;
        }

        public bool Loop
        {
            get => audioSource.loop;
            set => audioSource.loop = value;
        }

        public float AudioClipLength => audioSource.clip.length;

        public AudioMixerGroup OutputAudioMixerGroup
        {
            get => audioSource.outputAudioMixerGroup;
            set => audioSource.outputAudioMixerGroup = value;
        }

        #endregion

        internal static void SetAudioSource(AudioSourceController controller, AudioSource audioSource)
        {
            controller.audioSource = audioSource;
        }

        public float Volume
        {
            get => Mathf.Clamp01(audioSource.volume / volumeFactor);
            set => audioSource.volume = Mathf.Clamp01(value) * volumeFactor;
        }
        
        
        internal static void ResetAudioSource(AudioSourceController controller)
        {
            var audioSource = controller.audioSource;

            audioSource.volume = 1;
            audioSource.loop = false;
            audioSource.mute = false;

            audioSource.bypassEffects = false;
            audioSource.bypassListenerEffects = false;
            audioSource.bypassReverbZones = false;

            audioSource.priority = 128;
            audioSource.pitch = 1;
            audioSource.panStereo = 0;
            audioSource.spatialBlend = 0;
            audioSource.reverbZoneMix = 1;
        }
    }
}