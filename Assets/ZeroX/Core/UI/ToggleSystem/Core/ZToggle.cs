using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ZeroX.ToggleSystem
{
    public class ZToggle : MonoBehaviour
    {
        [SerializeField] private bool isOn;
        [SerializeField] private Button button;
        [SerializeField] private List<ZToggleTarget> listToggleTarget = new List<ZToggleTarget>();

        [SerializeField] private ZToggleUnityEvent_Bool onValueChanged;

        public ZToggleUnityEvent_Bool OnValueChanged => onValueChanged;

        
        public bool IsOn
        {
            get => isOn;
            set => SetIsOn(value);
        }

        
        #if UNITY_EDITOR
        private bool editor_lastValueSetByMethod;
        #endif


        private UnityAction button_OnClick;

        #region Unity Method

        private void Awake()
        {
            if (button != null)
            {
                button_OnClick = Button_OnClick;
                button.onClick.AddListener(button_OnClick);
            }
        }

        private void OnEnable()
        {
            Internal_SetIsOn(isOn, false, true);
        }

        private void OnDestroy()
        {
            if (button != null && button_OnClick != null)
            {
                button.onClick.RemoveListener(button_OnClick);
            }
        }

        private void Reset()
        {
            button = GetComponent<Button>();
        }
        
        
        
#if UNITY_EDITOR
        private void Update()
        {
            if(editor_lastValueSetByMethod != isOn)
                Internal_SetIsOn(isOn, true, false);
        }
#endif

        #endregion
        
        
        
        public void UpdateAllTarget()
        {
            foreach (var toggleTarget in listToggleTarget)
            {
                try
                {
                    toggleTarget.UpdateTarget(this);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        
        public void UpdateAllTargetImmediate()
        {
            foreach (var toggleTarget in listToggleTarget)
            {
                try
                {
                    toggleTarget.UpdateTargetImmediate(this);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        
        
        
        protected void Internal_SetIsOn(bool value, bool fireEvent, bool updateAllTargetImmediate)
        {
            isOn = value;
            
#if UNITY_EDITOR
            editor_lastValueSetByMethod = value;
#endif
            
            if(updateAllTargetImmediate)
                UpdateAllTargetImmediate();
            else
                UpdateAllTarget();
            
            if(fireEvent)
                OnValueChanged?.Invoke(value);
        }
        
        public void SetIsOn(bool newValue)
        {
            if(isOn == newValue)
                return;
            
            Internal_SetIsOn(newValue, true, false);
        }

        public void SetIsOn(bool newValue, bool fireEvent)
        {
            if(isOn == newValue)
                return;
            
            Internal_SetIsOn(newValue, fireEvent, false);
        }

        public void SetIsOn(bool newValue, bool fireEvent, bool updateAllTargetImmediate)
        {
            if(isOn == newValue)
                return;
            
            Internal_SetIsOn(newValue, fireEvent, updateAllTargetImmediate);
        }

        /// <summary>
        /// No fire event
        /// </summary>
        public void SetIsOnSilent(bool newValue)
        {
            if(isOn == newValue)
                return;
            
            Internal_SetIsOn(newValue, false, false);
        }
        
        

        

        void Button_OnClick()
        {
            Internal_SetIsOn(!isOn, true, false);
        }
    }
}