using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ZeroX.AudioSystem
{
    [CreateAssetMenu(menuName = "ZeroX/Audio System/Audio Database")]
    public class AudioDatabase : ScriptableObject
    {
        [SerializeField] protected long audioGroupIdSeq = -1;
        [SerializeField] protected List<AudioGroup> listAudioGroup = new List<AudioGroup>();

        public const string fn_audioGroupIdSeq = "audioGroupIdSeq";
        public const string fn_listAudioGroup = "listAudioGroup";

        [NonSerialized] private bool initialized = false;
        [NonSerialized] private Dictionary<long, AudioGroup> dictAudioGroup = new Dictionary<long, AudioGroup>();

        void InitIfNot()
        {
            if(initialized)
                return;
            initialized = true;

            foreach (var audioGroup in listAudioGroup)
            {
                dictAudioGroup.Add(audioGroup.Id, audioGroup);
            }
        }
        
        public IEnumerable<AudioGroup> GetIEnumerableAudioGroup()
        {
            foreach (var audioGroup in listAudioGroup)
            {
                yield return audioGroup;
            }
        }

        public AudioGroup GetAudioGroup(long audioGroupId)
        {
            InitIfNot();
            if (dictAudioGroup.TryGetValue(audioGroupId, out var audioGroup))
                return audioGroup;
            
            Debug.LogError("Cannot find audio group with id: " + audioGroupId);
            return null;
        }

        public AudioMixerGroup GetAudioMixerGroup(long audioGroupId)
        {
            InitIfNot();
            if (dictAudioGroup.TryGetValue(audioGroupId, out var audioGroup))
                return audioGroup.AudioMixerGroup;
            
            Debug.LogError("Cannot find audio group with id: " + audioGroupId);
            return null;
        }
    }
}