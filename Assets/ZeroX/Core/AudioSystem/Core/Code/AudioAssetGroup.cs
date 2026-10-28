using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.AudioSystem
{
    [CreateAssetMenu(menuName = "ZeroX/Audio System/Audio Asset Group")]
    public class AudioAssetGroup : ScriptableObject
    {
        [SerializeField] private List<AudioAsset> listAudioAsset = new List<AudioAsset>();



        public List<AudioAsset> ListAudioAsset => listAudioAsset;
        public int Count => listAudioAsset.Count;
        

        public AudioAsset GetRandom()
        {
            if (listAudioAsset.Count == 0)
            {
                Debug.LogError("List audio asset is empty");
                return null;
            }

            int random = Random.Range(0, listAudioAsset.Count);
            return listAudioAsset[random];
        }
        
        public AudioSourceController PlayRandom()
        {
            var audioAsset = GetRandom();
            if (audioAsset == null)
                return null;
            
            return audioAsset.Play();
        }
        
        public AudioSourceController PlayRandomAvoidResonance(float time = AudioManager.AvoidResonanceTime)
        {
            var audioAsset = GetRandom();
            if (audioAsset == null)
                return null;
            
            return audioAsset.PlayAvoidResonance(time);
        }
        
        public void PlayRandomOnAudioSource(AudioSource audioSource)
        {
            var audioAsset = GetRandom();
            if (audioAsset == null)
                return;
            
            audioAsset.PlayOnAudioSource(audioSource);
        }
        
        public void PlayRandomOnAudioSourceAvoidResonance(AudioSource audioSource, float time = AudioManager.AvoidResonanceTime)
        {
            var audioAsset = GetRandom();
            if (audioAsset == null)
                return;
            
            audioAsset.PlayOnAudioSourceAvoidResonance(audioSource, time);
        }

        public void Play(int index)
        {
            listAudioAsset[index].Play();
        }
        
        public void PlayAvoidResonance(int index, float time = AudioManager.AvoidResonanceTime)
        {
            listAudioAsset[index].PlayAvoidResonance(time);
        }
    }
}