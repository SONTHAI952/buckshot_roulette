using System;
using DG.Tweening;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WindowAnimPure_PingPong : WindowAnimPure
    {
        public float minScale = 0.2f;
        public float normalScale = 1f;
        
        private AnimationCurve openCurve = new AnimationCurve(
            new Keyframe(0, 0f),
            new Keyframe(0.5714f, 1.144847f),
            new Keyframe(0.8f, 0.95f),
            new Keyframe(1f, 1f));
        
        private AnimationCurve closeCurve = new AnimationCurve(
            new Keyframe(0, 0f),
            new Keyframe(0.2f, 0.05f),
            new Keyframe(0.4286f, -0.144847f),
            new Keyframe(1f, 1f));
        
        
        
        
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


            if (window.PreWindowState == WindowState.Null || window.PreWindowState == WindowState.Closed)
                ResetBeforeOpen(window);
            
            
            //DOTween
            var sequence = DOTween.Sequence(window.AnimObjectKey);
            
            sequence.Insert(0, window.Content.DOScale(normalScale, duration).SetEase(openCurve));
            sequence.Insert(0, window.CanvasGroup.DOFade(1f, duration).SetEase(Ease.InOutSine));
            
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
            
            sequence.Insert(0, window.Content.DOScale(minScale, duration).SetEase(closeCurve));
            sequence.Insert(0, window.CanvasGroup.DOFade(0f, duration).SetEase(Ease.InOutSine));
                
            sequence.SetUpdate(ignoreTimeScale);
            sequence.OnComplete(() =>
            {
                onFinished?.Invoke();
            });
        }
    }
}