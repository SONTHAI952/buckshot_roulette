using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.ToggleSystem
{
    public class ZToggleTargetCanvasGroup : ZToggleTarget
    {
        [SerializeField] private CanvasGroup canvasGroupOff;
        [SerializeField] private CanvasGroup canvasGroupOn;
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
            canvasGroupOff.alpha = Mathf.Lerp(1f, 0f, isOnVisualProgress);
            
            //Image On
            canvasGroupOn.alpha = Mathf.Lerp(0f, 1f, isOnVisualProgress);
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