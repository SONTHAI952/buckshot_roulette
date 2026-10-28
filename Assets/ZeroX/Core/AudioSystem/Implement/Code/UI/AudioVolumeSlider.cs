using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.AudioSystem
{
    public class AudioVolumeSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private List<VolumeType> listVolumeType;

        private void Awake()
        {
            slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnEnable()
        {
            if(listVolumeType.Count == 0)
                return;

            float volume = AudioManager.GetVolume(listVolumeType[0]);
            slider.normalizedValue = volume;
        }

        void OnValueChanged(float value)
        {
            float volume = slider.normalizedValue;
            foreach (var volumeType in listVolumeType)
            {
                AudioManager.SetVolumeAndSave(volumeType, volume);
            }
        }
    }
}