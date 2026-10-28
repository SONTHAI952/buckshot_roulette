using System;
using DG.Tweening;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WindowAnimPure_FadeAndScaleMirror : WindowAnimPure
    {
        public float minScale = 0.9f;
        public float normalScale = 1f;
        
        
        
        
        
        public override void OpenImmediate(Window window, WindowParameters parameters)
        {
            DOTween.Kill(window.AnimObjectKey);
            
            
            window.Content.localScale = new Vector3(normalScale, normalScale, normalScale);
            window.CanvasGroup.alpha = 1;
        }

        private void ResetBeforeOpen(Window window)
        {
            window.Content.localScale = new Vector3(minScale, minScale, minScale);
            window.CanvasGroup.alpha = 0;
        }
        
        public override void Open(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            DOTween.Kill(window.AnimObjectKey);
            
            
            if(window.PreWindowState == WindowState.Null || window.PreWindowState == WindowState.Closed)
                ResetBeforeOpen(window);
            
            
            //DOTween
            var sequence = DOTween.Sequence(window.AnimObjectKey);
            
            sequence.Insert(0, window.CanvasGroup.DOFade(1f, duration));
            sequence.Insert(0, window.Content.DOScale(normalScale, duration));
            sequence.SetUpdate(ignoreTimeScale);
            sequence.OnComplete(() =>
            {
                onFinished?.Invoke();
            });
        }

        

        
        
        public override void CloseImmediate(Window window, WindowParameters parameters)
        {
            DOTween.Kill(window.AnimObjectKey);
            
            
            window.Content.localScale = new Vector3(minScale, minScale, minScale);
            window.CanvasGroup.alpha = 0;
        }

        private void ResetBeforeClose(Window window)
        {
            window.Content.localScale = new Vector3(normalScale, normalScale, normalScale);
            window.CanvasGroup.alpha = 1;
        }
        
        public override void Close(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            DOTween.Kill(window.AnimObjectKey);
            
            
            if(window.PreWindowState == WindowState.Null)
                ResetBeforeClose(window);
            
            
            //DOTween
            var sequence = DOTween.Sequence(window.AnimObjectKey);
            
            sequence.Insert(0, window.CanvasGroup.DOFade(0f, duration));
            sequence.Insert(0, window.Content.DOScale(minScale, duration));
            sequence.SetUpdate(ignoreTimeScale);
            sequence.OnComplete(() =>
            {
                onFinished?.Invoke();
            });
        }
    }
}