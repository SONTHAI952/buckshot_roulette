using UnityEngine;
using UnityEngine.Audio;

namespace ZeroX.AudioSystem
{
    [System.Serializable]
    public class AudioGroup
    {
        [SerializeField] private long id;
        [SerializeField] private string name;
        [SerializeField] private AudioMixerGroup audioMixerGroup;

        public const string fn_id = "id";
        public const string fn_name = "name";
        public const string fn_audioMixerGroup = "audioMixerGroup";


        public long Id => id;
        public string Name => name;
        public AudioMixerGroup AudioMixerGroup => audioMixerGroup;
    }
}