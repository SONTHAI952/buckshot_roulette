using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using ZeroX.AudioSystem;

namespace ZeroX.UI
{
    public class ButtonClickEffect_Scale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public float normalScale = 1f;
        public float pointerDownScale = 0.95f;
        public float scaleDuration = 0.15f;
        public AudioAsset clickSfx;

        private Tween currentTween;
        public void OnPointerDown(PointerEventData eventData)
        {
            currentTween?.Kill();
            currentTween = transform.DOScale(pointerDownScale, scaleDuration).SetEase(Ease.Linear).SetUpdate(true);
            
            if(clickSfx != null)
                clickSfx.Play();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            currentTween?.Kill();
            currentTween = transform.DOScale(normalScale, scaleDuration).SetEase(Ease.Linear).SetUpdate(true);
        }
    }
}