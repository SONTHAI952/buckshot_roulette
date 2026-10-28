using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.ToggleSystem
{
    /// <summary>
    /// Chỉ cho phép một toggle trong group được bật tại một thời điểm
    /// </summary>
    public class ZToggleGroup : MonoBehaviour
    {
        [SerializeField] private bool allowAllOff = false;
        [SerializeField] private List<ZToggle> listToggle = new List<ZToggle>();

        
        //Temp
        private bool isHandlingGroup = false;
        

        private void Awake()
        {
            foreach (var toggle in listToggle)
            {
                toggle.OnValueChanged.AddListener(v => Toggle_OnValueChanged(toggle));
            }
        }

        private void Toggle_OnValueChanged(ZToggle toggle)
        {
            if(isHandlingGroup)
                return;
            
            
            
            isHandlingGroup = true;
            
            if (toggle.IsOn)
            {
                foreach (var otherToggle in listToggle)
                {
                    if(otherToggle == toggle)
                        continue;
                    
                    otherToggle.SetIsOn(false);
                }
            }
            else
            {
                if(allowAllOff == false)
                    toggle.SetIsOn(true);
            }
            
            isHandlingGroup = false;
        }
    }
}