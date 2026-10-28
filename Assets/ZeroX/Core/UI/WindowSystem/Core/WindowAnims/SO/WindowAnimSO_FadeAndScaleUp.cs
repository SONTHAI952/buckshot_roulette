using System;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    [CreateAssetMenu(menuName = WindowConst.MenuCreateAsset.Root + "/Window Anim SO/Fade And Scale Up")]
    public class WindowAnimSO_FadeAndScaleUp : WindowAnimSO
    {
        public float minScale = 0.9f;
        public float normalScale = 1f;
        public float maxScale = 1.1f;
        
        private WindowAnimPure_FadeAndScaleUp windowAnimPure = new WindowAnimPure_FadeAndScaleUp();
        
        
        
        public override void OpenImmediate(Window window, WindowParameters parameters)
        {
            windowAnimPure.minScale = minScale;
            windowAnimPure.normalScale = normalScale;
            windowAnimPure.maxScale = maxScale;
            windowAnimPure.OpenImmediate(window, parameters);
        }
        
        public override void Open(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            windowAnimPure.minScale = minScale;
            windowAnimPure.normalScale = normalScale;
            windowAnimPure.maxScale = maxScale;
            windowAnimPure.Open(window, duration, ignoreTimeScale, parameters, onFinished);
        }

        
        

        
        public override void CloseImmediate(Window window, WindowParameters parameters)
        {
            windowAnimPure.minScale = minScale;
            windowAnimPure.normalScale = normalScale;
            windowAnimPure.maxScale = maxScale;
            windowAnimPure.CloseImmediate(window, parameters);
        }
        
        public override void Close(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            windowAnimPure.minScale = minScale;
            windowAnimPure.normalScale = normalScale;
            windowAnimPure.maxScale = maxScale;
            windowAnimPure.Close(window, duration, ignoreTimeScale, parameters, onFinished);
        }
    }
}