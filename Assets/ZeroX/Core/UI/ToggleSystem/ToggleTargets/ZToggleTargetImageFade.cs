using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.ToggleSystem
{
    public class ZToggleTargetImageFade : ZToggleTarget
    {
        [SerializeField] private Image imageOff;
        [SerializeField] private Image imageOn;
        [SerializeField] private float transitionDuration = 0.25f;
        
        private float isOnVisualProgress;
        private Tween currentTween;

        private void OnDestroy()
        {
            KillCurrentTween();
        }


        public override void UpdateTarget(ZToggle toggle)
        {
            KillCurrentTween();
            DoIsOnVisualProgress(toggle.IsOn ? 1 : 0);
        }

        public override void UpdateTargetImmediate(ZToggle toggle)
        {
            KillCurrentTween();
            SetIsOnVisualProgress(toggle.IsOn ? 1 : 0);
        }

        void SetIsOnVisualProgress(float isOnVisualProgress)
        {
            //Debug.Log(isOnVisualProgress);
            this.isOnVisualProgress = isOnVisualProgress;

            //Image Off
            Color imageOffColorA = imageOff.color;
            imageOffColorA.a = 1;

            Color imageOffColorB = imageOff.color;
            imageOffColorB.a = 0;
            
            imageOff.color = Color.Lerp(imageOffColorA, imageOffColorB, isOnVisualProgress);



            //Image On
            Color imageOnColorA = imageOn.color;
            imageOnColorA.a = 0;
            
            Color imageOnColorB = imageOn.color;
            imageOnColorB.a = 1;
            
            imageOn.color = Color.Lerp(imageOnColorA, imageOnColorB, isOnVisualProgress);
        }
        
        
        void KillCurrentTween()
        {
            if (currentTween != null)
            {
                currentTween.Kill();
                currentTween = null;
            }
        }
        
        void DoIsOnVisualProgress(float target)
        {
            float duration = transitionDuration * Mathf.Abs(target - isOnVisualProgress);
            currentTween = DOVirtual.Float(isOnVisualProgress, target, duration, v =>
            {
                SetIsOnVisualProgress(v);
            }).SetUpdate(true).OnComplete(() => currentTween = null);
        }
    }
}