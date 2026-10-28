using System;
using DG.Tweening;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WindowAnimPure_Fade : WindowAnimPure
    {
        public override void OpenImmediate(Window window, WindowParameters parameters)
        {
            DOTween.Kill(window.AnimObjectKey);
            
            
            window.CanvasGroup.alpha = 1;
        }
        
        private void ResetBeforeOpen(Window window)
        {
            window.Content.localScale = Vector3.one;
            window.CanvasGroup.alpha = 0;
        }
        
        public override void Open(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            DOTween.Kill(window.AnimObjectKey);
            
            
            if(window.PreWindowState == WindowState.Null || window.PreWindowState == WindowState.Closed)
                ResetBeforeOpen(window);
            
            
            window.CanvasGroup.DOFade(1f, duration)
                .SetTarget(window.AnimObjectKey)
                .SetUpdate(ignoreTimeScale)
                .OnComplete(() =>
                {
                    onFinished?.Invoke();
                });
        }
        
        
        
        
        
        public override void CloseImmediate(Window window, WindowParameters parameters)
        {
            DOTween.Kill(window.AnimObjectKey);

            
            window.CanvasGroup.alpha = 0;
        }
        
        private void ResetBeforeClose(Window window)
        {
            window.Content.localScale = Vector3.one;
            window.CanvasGroup.alpha = 1;
        }
        
        public override void Close(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            DOTween.Kill(window.AnimObjectKey);
  
            
            if(window.PreWindowState == WindowState.Null)
                ResetBeforeClose(window);
            
            
            window.CanvasGroup.DOFade(0f, duration)
                .SetTarget(window.AnimObjectKey)
                .SetUpdate(ignoreTimeScale)
                .OnComplete(() =>
                {
                    onFinished?.Invoke();
                });
        }
    }
}