using System.Collections;
using UnityEngine;
using ZeroX.SaveGameSystem;

namespace ZeroX.AudioSystem
{
    public class AutoLoadVolumeSaved : MonoBehaviour
    {
        private AudioManager audioManager;

        private void Awake()
        {
            audioManager = GetComponent<AudioManager>();
        }

        private void Start()
        {
            StartCoroutine(AutoLoadTimeline());
        }

        private IEnumerator AutoLoadTimeline()
        {
            //Nếu chưa login thì tạm thời mute all
            if (SaveGame.IsLoggedInDefault() == false)
            {
                MuteAllVolume();
                yield return new WaitUntil(() => SaveGame.IsLoggedInDefault());
            }
            
            LoadAllVolume();
        }
        
        

        private void LoadAllVolume()
        {
            LoadVolume(VolumeType.Music);
            LoadVolume(VolumeType.SFX);
            LoadVolume(VolumeType.UI);
        }
        
        private void LoadVolume(VolumeType volumeType)
        {
            audioManager.SetAudioMixerVolume(VolumeTypeUtility.ToFloatFieldInAudioMixer(volumeType), AudioSettingDI.GetVolume(volumeType));
        }

        private void MuteAllVolume()
        {
            MuteVolume(VolumeType.Music);
            MuteVolume(VolumeType.SFX);
            MuteVolume(VolumeType.UI);
        }
        
        private void MuteVolume(VolumeType volumeType)
        {
            audioManager.SetAudioMixerVolume(VolumeTypeUtility.ToFloatFieldInAudioMixer(volumeType), 0);
        }
    }
}