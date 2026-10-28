using System;
using System.Collections.Generic;
using UnityEngine;
using ZeroX.ToggleSystem;

namespace ZeroX.AudioSystem.UI
{
    public class AudioVolumeToggle : MonoBehaviour
    {
        [SerializeField] private ZToggle toggle;
        [SerializeField] private List<VolumeType> listVolumeType;

        private void Awake()
        {
            toggle.OnValueChanged.AddListener(Toggle_OnValueChanged);
        }
        
        private void OnEnable()
        {
            if(listVolumeType.Count == 0)
                return;

            float volume = AudioManager.GetVolume(listVolumeType[0]);
            toggle.SetIsOn(volume > 0, false, true);
        }

        void Toggle_OnValueChanged(bool value)
        {
            float volume = toggle.IsOn ? 1 : 0;
            foreach (var volumeType in listVolumeType)
            {
                AudioManager.SetVolumeAndSave(volumeType, volume);
            }
        }
    }
}