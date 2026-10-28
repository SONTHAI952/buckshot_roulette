using System;
using UnityEngine;
using ZeroX.ToggleSystem;

namespace ZeroX.HapticSystem
{
    public class HapticSettingToggle : MonoBehaviour
    {
        [SerializeField] private ZToggle toggle;


        private void Awake()
        {
            toggle.OnValueChanged.AddListener(Toggle_OnValueChanged);
        }

        private void OnEnable()
        {
            toggle.SetIsOn(HapticManager.EnableHaptic, false, true);
        }


        void Toggle_OnValueChanged(bool value)
        {
            HapticManager.EnableHaptic = value;
        }
    }
}