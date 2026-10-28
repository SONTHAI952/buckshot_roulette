using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.ProgressSystem.Example
{
    public class DemoProgressor : MonoBehaviour
    {
        public Progressor progressor;
        public Slider slider;
        public bool setAnimateProgressImmediately;
        
        private void Awake()
        {
            slider.onValueChanged.AddListener(Slider_OnValueChanged);
        }
        

        private void Slider_OnValueChanged(float value)
        {
            if (setAnimateProgressImmediately)
                progressor.SetProgressAndAnimateProgress(value);
            else
                progressor.Progress = value;
        }
    }
}