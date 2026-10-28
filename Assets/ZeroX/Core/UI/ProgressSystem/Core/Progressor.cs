using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.ProgressSystem
{
    [AddComponentMenu(ComponentMenuUtility.progressSystem + "Progressor")]
    public class Progressor : MonoBehaviour
    {
        [SerializeField] private float minValue;
        [SerializeField] private float maxValue = 1;
        [SerializeField] private float value;
        [SerializeField] private AnimateValueMode animateValueMode = AnimateValueMode.Instant;
        [SerializeField] private float animateValue;
        [SerializeField] private float animateValueSpeed = 2f;
        [SerializeField] private bool wholeNumbers;
        [SerializeField] private List<ProgressTarget> listProgressTarget = new List<ProgressTarget>();

        //Event
        public ProgressorUnityEvent onValueChanged;
        public ProgressorUnityEvent onProgressChanged;
        public ProgressorUnityEvent onAnimateValueChanged;
        public ProgressorUnityEvent onAnimateProgressChanged;

        private Coroutine crAnimateValueMachine;


        #region Unity Method

        private void Start()
        {
            UpdateProgressTarget();
            NotifyValueAndProgressChanged();
            NotifyAnimateValueAndAnimateProgressChanged();
        }

        protected virtual void OnEnable()
        {
            if (crAnimateValueMachine == null && Mathf.Approximately(animateValue, value) == false)
            {
                StartAnimateValueMachine();
            }
        }

        protected virtual void OnDisable()
        {
            StopAnimateValueMachine();
        }

        #endregion
        

        #region Notify Method
        
        void NotifyValueChanged()
        {
            try
            {
                onValueChanged?.Invoke(GetValue());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        void NotifyProgressChanged()
        {
            try
            {
                onProgressChanged?.Invoke(GetProgress());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void NotifyValueAndProgressChanged()
        {
            try
            {
                float value = GetValue();
                float progress = GetProgress();
                onValueChanged?.Invoke(value);
                onProgressChanged?.Invoke(progress);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void NotifyAnimateValueChanged()
        {
            try
            {
                onAnimateValueChanged?.Invoke(GetAnimateValue());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        void NotifyAnimateProgressChanged()
        {
            try
            {
                onAnimateProgressChanged?.Invoke(GetAnimateProgress());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        void NotifyAnimateValueAndAnimateProgressChanged()
        {
            try
            {
                float animateValue = GetAnimateValue();
                float animateProgress = GetAnimateProgress();
                onAnimateValueChanged?.Invoke(animateValue);
                onAnimateProgressChanged?.Invoke(animateProgress);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion
        

        #region Value
        
        float GetValue()
        {
            return wholeNumbers ? Mathf.Floor(value) : value;
        }
        
        void SetValueInternal(float newValue, bool updateProgressTarget, bool notify)
        {
            //Set Value
            newValue = Mathf.Clamp(newValue, minValue, maxValue);
            if(Mathf.Approximately(newValue, this.value))
                return;
                
            this.value = newValue;

                
            //Set Animate Value
            bool animateValueChanged = false;
            if (animateValueMode == AnimateValueMode.Instant)
            {
                animateValue = this.value;
                animateValueChanged = true;
            }
            else if (animateValueMode == AnimateValueMode.Smooth)
            {
                StartAnimateValueMachine();
            }
            else if (animateValueMode == AnimateValueMode.Custom)
            {
            }
            else
            {
                Debug.LogError("Chưa code cho animate value này: " + animateValueMode);
            }
            
            
            //Update ProgressTarget
            if(updateProgressTarget)
                UpdateProgressTarget();
            
                
            //Event
            if (notify)
            {
                NotifyValueAndProgressChanged();

                if (animateValueChanged)
                    NotifyAnimateValueAndAnimateProgressChanged();
            }
        }

        private void SetValue_Editor(float newValue)
        {
            //Set Value
            newValue = Mathf.Clamp(newValue, minValue, maxValue);
            if(Mathf.Approximately(newValue, this.value))
                return;
                
            this.value = newValue;
            
            //Update ProgressTarget
            UpdateProgressTarget();
            
            //Event
            NotifyValueAndProgressChanged();
        }

        public void SetValueSilent(float newValue)
        {
            SetValueInternal(newValue, true, false);
        }
        
        #endregion

        #region Animate Value
        
        float GetAnimateValue()
        {
            if (animateValueMode == AnimateValueMode.Instant)
            {
                return GetValue();
            }
            else
            {
                if (wholeNumbers)
                    return Mathf.Floor(animateValue);
                else
                    return animateValue;
            }
        }

        void SetAnimateValueInternal(float newAnimateValue, bool updateProgressTarget, bool notify)
        {
            newAnimateValue = Mathf.Clamp(newAnimateValue, minValue, maxValue);
            if(Mathf.Approximately(newAnimateValue, animateValue))
                return;
                
            
            if (animateValueMode == AnimateValueMode.Instant)
            {
                animateValue = newAnimateValue;
            }
            else if (animateValueMode == AnimateValueMode.Smooth)
            {
                animateValue = newAnimateValue;
                StartAnimateValueMachine();
            }
            else if (animateValueMode == AnimateValueMode.Custom)
            {
                animateValue = newAnimateValue;
            }
            else
            {
                animateValue = newAnimateValue;
                Debug.LogError("Chưa code cho animate value này: " + animateValueMode);
            }
            
            
            //Update ProgressTarget
            if(updateProgressTarget)
                UpdateProgressTarget();
            
            
            //Event
            if (notify)
                NotifyAnimateValueAndAnimateProgressChanged();
        }

        void SetAnimateValue_Editor(float newAnimateValue)
        {
            //Ko start animate value machine
            
            newAnimateValue = Mathf.Clamp(newAnimateValue, minValue, maxValue);
            if(Mathf.Approximately(newAnimateValue, animateValue))
                return;
            
            animateValue = newAnimateValue;

            //Update ProgressTarget
            UpdateProgressTarget();
            
            //Event
            NotifyAnimateValueAndAnimateProgressChanged();
        }
        
        public void SetAnimateValueSilent(float newAnimateValue)
        {
            SetAnimateValueInternal(newAnimateValue, true, false);
        }
        
        #endregion

        #region Min Value
        
        float GetMinValue()
        {
            return wholeNumbers ? Mathf.Floor(minValue) : minValue;
        }

        void SetMinValueInternal(float newMinValue, bool updateProgressTarget, bool notify)
        {
            if(Mathf.Approximately(newMinValue, minValue))
                return;
            
            minValue = newMinValue;

            if (minValue <= maxValue)
            {
                bool valueChanged = false;
                bool animateValueChanged = false;
                if (this.value < minValue)
                {
                    valueChanged = true;
                    SetValueInternal(minValue, false, false);
                }
                
                if (animateValue < minValue)
                {
                    animateValueChanged = true;
                    SetAnimateValueInternal(minValue, false, false);
                }
                
                if(updateProgressTarget)
                    UpdateProgressTarget();

                if (notify)
                {
                    if (valueChanged)
                        NotifyValueChanged();
                    NotifyProgressChanged();
                    
                    
                    if (animateValueChanged)
                        NotifyAnimateValueChanged();
                    NotifyAnimateProgressChanged();
                }
            }
            else
            {
                bool valueChanged = false;
                bool animateValueChanged = false;
                if (this.value > minValue)
                {
                    valueChanged = true;
                    SetValueInternal(minValue, false, false);
                }
                
                if (animateValue > minValue)
                {
                    animateValueChanged = true;
                    SetAnimateValueInternal(minValue, false, false);
                }
                
                if(updateProgressTarget)
                    UpdateProgressTarget();

                if (notify)
                {
                    if (valueChanged)
                        NotifyValueChanged();
                    NotifyProgressChanged();
                
                
                    if (animateValueChanged)
                        NotifyAnimateValueChanged();
                    NotifyAnimateProgressChanged();
                }
            }
        }

        public void SetMinValueSilent(float newMinValue)
        {
            SetMinValueInternal(newMinValue, true, false);
        }
        
        #endregion

        #region Max Value
        
        float GetMaxValue()
        {
            return wholeNumbers ? Mathf.Floor(maxValue) : maxValue;
        }

        void SetMaxValueInternal(float newMaxValue, bool updateProgressTarget, bool notify)
        {
            maxValue = newMaxValue;
                
            if (maxValue >= minValue)
            {
                bool valueChanged = false;
                bool animateValueChanged = false;
                if (this.value > maxValue)
                {
                    valueChanged = true;
                    SetValueInternal(maxValue, false, false);
                }
                
                if (animateValue > maxValue)
                {
                    animateValueChanged = true;
                    SetAnimateValueInternal(maxValue, false, false);
                }
                
                if(updateProgressTarget)
                    UpdateProgressTarget();

                if (notify)
                {
                    if (valueChanged)
                        NotifyValueChanged();
                    NotifyProgressChanged();
                
                
                    if (animateValueChanged)
                        NotifyAnimateValueChanged();
                    NotifyAnimateProgressChanged();
                }
            }
            else
            {
                bool valueChanged = false;
                bool animateValueChanged = false;
                if (this.value < maxValue)
                {
                    valueChanged = true;
                    SetValueInternal(maxValue, false, false);
                }
                
                if (animateValue < maxValue)
                {
                    animateValueChanged = true;
                    SetAnimateValueInternal(maxValue, false, false);
                }
                
                if(updateProgressTarget)
                    UpdateProgressTarget();

                if (notify)
                {
                    if (valueChanged)
                        NotifyValueChanged();
                    NotifyProgressChanged();
                
                
                    if (animateValueChanged)
                        NotifyAnimateValueChanged();
                    NotifyAnimateProgressChanged();
                }
            }
        }

        public void SetMaxValueSilent(float newMaxValue)
        {
            SetMaxValueInternal(newMaxValue, true, false);
        }
        
        #endregion

        #region Progress
        
        float GetProgress()
        {
            float min = GetMinValue();
            float max = GetMaxValue();

            float length = max - min;
            if (length == 0)
                return 0;
            
            return (GetValue() - min) / length;
        }

        void SetProgressInternal(float newProgress, bool updateProgressTarget, bool notify)
        {
            float newValue;
            if (maxValue >= minValue)
                newValue = minValue + (maxValue - minValue) * Mathf.Clamp01(newProgress);
            else
                newValue = maxValue - (minValue - maxValue) * Mathf.Clamp01(newProgress);
            SetValueInternal(newValue, updateProgressTarget, notify);
        }

        public void SetProgressSilent(float newProgress)
        {
            SetProgressInternal(newProgress, true, false);
        }
        
        #endregion

        #region Animate Progress
        
        float GetAnimateProgress()
        {
            float min = GetMinValue();
            float max = GetMaxValue();
            
            float length = max - min;
            if (length == 0)
                return 0;
            
            return (GetAnimateValue() - min) / (max - min);
        }

        void SetAnimateProgressInternal(float newAnimateProgress, bool updateProgressTarget, bool notify)
        {
            float newAnimateValue;
            if (maxValue >= minValue)
                newAnimateValue = minValue + (maxValue - minValue) * Mathf.Clamp01(newAnimateProgress);
            else
                newAnimateValue = maxValue - (minValue - maxValue) * Mathf.Clamp01(newAnimateProgress);
            SetAnimateValueInternal(newAnimateValue, updateProgressTarget, notify);
        }
        
        public void SetAnimateProgressSilent(float newAnimateProgress)
        {
            SetAnimateProgressInternal(newAnimateProgress, true, false);
        }
        
        #endregion

        #region Mix
        
        public void SetProgressAndAnimateProgress(float progress)
        {
            SetProgressInternal(progress, false, false);
            SetAnimateProgressInternal(progress, false, false);
            
            UpdateProgressTarget();
            NotifyValueAndProgressChanged();
            NotifyAnimateValueAndAnimateProgressChanged();
        }
        
        public void SetProgressAndAnimateProgressSilent(float progress)
        {
            SetProgressInternal(progress, false, false);
            SetAnimateProgressInternal(progress, false, false);
            
            UpdateProgressTarget();
        }

        public void SetValueAndAnimateValue(float value)
        {
            SetValueInternal(value, false, false);
            SetAnimateValueInternal(value, false, false);
            
            UpdateProgressTarget();
            NotifyValueAndProgressChanged();
            NotifyAnimateValueAndAnimateProgressChanged();
        }
        
        public void SetValueAndAnimateValueSilent(float value)
        {
            SetValueInternal(value, false, false);
            SetAnimateValueInternal(value, false, false);
            
            UpdateProgressTarget();
        }

        #endregion


        #region Property
        
        public float Value
        {
            get => GetValue();
            set => SetValueInternal(value, true, true);
        }

        public float AnimateValue
        {
            get => GetAnimateValue();
            set => SetAnimateValueInternal(value, true, true);
        }
        
        public float MinValue
        {
            get => GetMinValue();
            set => SetMinValueInternal(value, true, true);
        }

        public float MaxValue
        {
            get => GetMaxValue();
            set => SetMaxValueInternal(value, true, true);
        }

        public float Progress
        {
            get => GetProgress();
            set => SetProgressInternal(value, true, true);
        }

        public float AnimateProgress
        {
            get => GetAnimateProgress();
            set => SetAnimateProgressInternal(value, true, true);
        }

        public float InverseProgress
        {
            get => 1 - GetProgress();
            set => SetProgressInternal(1 - Mathf.Clamp01(value), true, true);
        }

        public float InverseAnimateProgress
        {
            get => 1 - GetAnimateProgress();
            set => SetAnimateProgressInternal(1 - Mathf.Clamp01(value), true, true);
        }
        
        public AnimateValueMode AnimateValueMode
        {
            get => animateValueMode;
            set
            {
                if(animateValueMode == value)
                    return;
                
                animateValueMode = value;
                
                if (animateValueMode == AnimateValueMode.Instant)
                    StopAnimateValueMachine();
                else if (animateValueMode == AnimateValueMode.Smooth)
                    StartAnimateValueMachine();
                else if(animateValueMode == AnimateValueMode.Custom)
                    StopAnimateValueMachine();
                else
                {
                    StopAnimateValueMachine();
                    Debug.LogError("Not code for AnimateValueMode: " + animateValueMode);
                }
            }
        }

        public float AnimateValueSpeed
        {
            get => animateValueSpeed;
            set => animateValueSpeed = value;
        }

        public bool WholeNumbers
        {
            get => wholeNumbers;
            set
            {
                if(wholeNumbers == value)
                    return;

                wholeNumbers = value;
                UpdateProgressTarget();
                NotifyValueAndProgressChanged();
                NotifyAnimateValueAndAnimateProgressChanged();
            }
        }

        #endregion


        void UpdateProgressTarget()
        {
            try
            {
                for (int i = 0; i < listProgressTarget.Count; i++)
                {
                    if (listProgressTarget[i] == null)
                    {
                        Debug.LogError("Has progress target null in list", this);
                        continue;
                    }
                    
                    listProgressTarget[i].UpdateTarget(this);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void StopAnimateValueMachine()
        {
            if (crAnimateValueMachine != null)
            {
                StopCoroutine(crAnimateValueMachine);
                crAnimateValueMachine = null;
            }
        }
        
        private void StartAnimateValueMachine()
        {
            if(gameObject.activeInHierarchy == false)
                return;
            
            if(crAnimateValueMachine != null)
                return;
            
            if (Mathf.Approximately(value, animateValue))
                return;
            
            crAnimateValueMachine = StartCoroutine(AnimateValueMachine());
            
            

            IEnumerator AnimateValueMachine()
            {
                while (true)
                {
                    yield return null;
                    
                    if (Mathf.Approximately(value, animateValue))
                        break;
                    
                    animateValue = Mathf.MoveTowards(animateValue, value, Time.deltaTime * animateValueSpeed);
                    UpdateProgressTarget();
                    NotifyAnimateValueAndAnimateProgressChanged();
                }

                crAnimateValueMachine = null;
            }
        }

        public bool IsProgressMax => Mathf.Approximately(value, maxValue);
        
        public bool IsProgressMin => Mathf.Approximately(value, minValue);

        public bool IsAnimateProgressMax => Mathf.Approximately(animateValue, maxValue);

        public bool IsAnimateProgressMin => Mathf.Approximately(animateValue, minValue);

        #region List Progress Target Handle

        public bool AddProgressTarget(ProgressTarget progressTarget)
        {
            if(listProgressTarget.Contains(progressTarget))
                return false;
                    
            listProgressTarget.Add(progressTarget);
            progressTarget.UpdateTarget(this);
            return true;
        }

        public bool RemoveProgressTarget(ProgressTarget progressTarget)
        {
            return listProgressTarget.Remove(progressTarget);
        }

        public int CountProgressTarget()
        {
            return listProgressTarget.Count;
        }

        public void ForeachListProgressTarget(Action<ProgressTarget> action)
        {
            foreach (var progressTarget in listProgressTarget)
            {
                action?.Invoke(progressTarget);
            }
        }

        #endregion
    }
}