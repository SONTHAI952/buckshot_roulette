using DG.Tweening;
using UnityEngine;

namespace ZeroX.ButtonSystem
{
    public static class ButtonClickVfxRunner_Scale
    {
        private const float normalScale = 1f;
        private const float pointerDownScale = 0.95f;
        private const float scaleDuration = 0.15f;
        
        
        
        
        public static void OnPointerDown(Transform transform, ref Tween currentTween)
        {
            currentTween?.Kill();
            currentTween = transform.DOScale(pointerDownScale, scaleDuration).SetEase(Ease.Linear).SetUpdate(true);
        }

        public static void OnPointerUp(Transform transform, ref Tween currentTween)
        {
            currentTween?.Kill(); 
            currentTween = transform.DOScale(normalScale, scaleDuration).SetEase(Ease.Linear).SetUpdate(true);
        }
    }
}