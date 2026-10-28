using UnityEngine;
using UnityEngine.Audio;

namespace ZeroX.AudioSystem
{
    public partial class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioDatabase audioDatabase;
        [SerializeField] private AudioMixer audioMixer;

        public AudioDatabase AudioDatabase => audioDatabase;
        private static AudioManager Instance;
        
        private static float minDb = -30;
        private static float maxDb = 0;
        private static float zeroDb = -80;


        public const float AvoidResonanceTime = 0.032f;
        
        
        private void Awake()
        {
            if (Instance == null || Instance == this)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void Update()
        {
            AudioSourceControllerPool.Update();
        }

        #region Volumne
        
        public void SetAudioMixerVolume(string volumeName, float volumeValue)
        {
            volumeValue = Mathf.Clamp01(volumeValue);
            if (volumeValue > 0)
            {
                float dB = Mathf.Lerp(minDb, maxDb, volumeValue);
                audioMixer.SetFloat(volumeName, dB);
            }
            else
            {
                audioMixer.SetFloat(volumeName, zeroDb);
            }
        }
        
        /// <summary>
        /// Trả về dạng 0 -> 1
        /// </summary>
        public float GetAudioMixerVolume(string volumeName)
        {
            if (audioMixer.GetFloat(volumeName, out float dB) == false)
            {
                Debug.LogError("Cannot getFloat audio mixer group with name: " + volumeName);
                return 1f;
            }
            
            if (dB <= minDb)
                return 0;
            
            dB = Mathf.Clamp(dB, minDb, maxDb);
            float volume = Mathf.Lerp(0, 1, (dB - minDb) / (maxDb - minDb));
            return volume;
        }
        
        

        public static void SetVolume(string volumeName, float volume)
        {
            Instance.SetAudioMixerVolume(volumeName, volume);
        }

        public static float GetVolume(string volumeName)
        {
            return Instance.GetAudioMixerVolume(volumeName);
        }

        #endregion

        public static AudioGroup GetAudioGroup(long audioGroupId)
        {
            return Instance.audioDatabase.GetAudioGroup(audioGroupId);
        }
        
        public static AudioMixerGroup GetAudioMixerGroup(long audioGroupId)
        {
            return Instance.audioDatabase.GetAudioMixerGroup(audioGroupId);
        }
        
        public static AudioGroup GetAudioGroup(AudioAsset audioAsset)
        {
            return Instance.audioDatabase.GetAudioGroup(audioAsset.AudioGroupId);
        }
        
        public static AudioMixerGroup GetAudioMixerGroup(AudioAsset audioAsset)
        {
            return Instance.audioDatabase.GetAudioMixerGroup(audioAsset.AudioGroupId);
        }

        static bool ValidateAudioAsset(AudioAsset audioAsset)
        {
            if (audioAsset == null)
            {
                Debug.LogError("Cannot play because audio asset is empty!");
                return false;
            }
            
            if (audioAsset.AudioClip == null)
            {
                Debug.LogError("Cannot play because audio clip is empty!");
                return false;
            }

            return true;
        }
        
        public static AudioSourceController Play(AudioAsset audioAsset)
        {
            if (ValidateAudioAsset(audioAsset) == false)
                return null;

            audioAsset.lastTimePlay = Time.unscaledTime;
                
            AudioSourceController audioSourceController = AudioSourceControllerPool.GetFromPool();
            audioSourceController.name = "[ASC] " + audioAsset.AudioClip.name;
            audioSourceController.AudioClip = audioAsset.AudioClip;
            audioSourceController.OutputAudioMixerGroup = Instance.audioDatabase.GetAudioMixerGroup(audioAsset.AudioGroupId);
            audioSourceController.volumeFactor = audioAsset.VolumeFactor;
            audioSourceController.Volume = 1;
            audioSourceController.Play();
            return audioSourceController;
        }

        public static void PlayOnAudioSource(AudioAsset audioAsset, AudioSource audioSource)
        {
            if (ValidateAudioAsset(audioAsset) == false)
                return;
            
            audioAsset.lastTimePlay = Time.unscaledTime;
            
            audioSource.Stop();
            audioSource.clip = audioAsset.AudioClip;
            audioSource.outputAudioMixerGroup = Instance.audioDatabase.GetAudioMixerGroup(audioAsset.AudioGroupId);
            audioSource.Play();
        }
        
        public static AudioSourceController PlayAvoidResonance(AudioAsset audioAsset, float time = AvoidResonanceTime)
        {
            if (audioAsset.HasRiskOfResonance(time))
                return null;
            
            return Play(audioAsset);
        }
        
        public static void PlayOnAudioSourceAvoidResonance(AudioAsset audioAsset, AudioSource audioSource, float time = AvoidResonanceTime)
        {
            if (audioAsset.HasRiskOfResonance(time))
                return;
            
            PlayOnAudioSource(audioAsset, audioSource);
        }
    }
}