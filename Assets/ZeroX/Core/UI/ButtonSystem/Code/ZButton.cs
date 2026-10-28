using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZeroX.ButtonSystem
{
    public class ZButton : Button
    {
        [SerializeField] private ButtonClickVfxType clickVfxType = ButtonClickVfxType.Scale;
        [SerializeField] private ButtonClickSfxType clickSfxType = ButtonClickSfxType.Normal;

        

        //Temp
        private Tween clickEffectTween;

        
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            transition = Transition.None;
            
            if (name.ToLower() == "overlay")
                clickVfxType = ButtonClickVfxType.None;
        }
#endif


        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            
            
            PlayVfx();
            PlaySfx();
        }


        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            
            
            //VFX
            ButtonClickVfxRunner_Scale.OnPointerUp(transform, ref clickEffectTween);
        }


        private void PlayVfx()
        {
            switch (clickVfxType)
            {
                case ButtonClickVfxType.None:
                {
                    break;
                }
                case ButtonClickVfxType.Scale:
                {
                    ButtonClickVfxRunner_Scale.OnPointerDown(transform, ref clickEffectTween);
                    break;
                }
                default:
                {
                    Debug.LogError("Not code for ButtonClickVfxType: " + clickVfxType);
                    break;
                }
            }
        }

        private void PlaySfx()
        {
            if (clickSfxType != ButtonClickSfxType.None)
            {
                var clickSfxAsset = ButtonSystemSetting.Instance.GetClickSfx(clickSfxType);
                if (clickSfxAsset != null)
                    clickSfxAsset.Play();
            }
        }
    }
}