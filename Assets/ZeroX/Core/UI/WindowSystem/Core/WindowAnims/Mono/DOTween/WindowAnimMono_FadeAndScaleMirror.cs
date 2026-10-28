using System;


namespace ZeroX.WindowSystem
{
    public class WindowAnimMono_FadeAndScaleMirror : WindowAnimMono
    {
        public float minScale = 0.9f;
        public float normalScale = 1f;
        
        private WindowAnimPure_FadeAndScaleMirror windowAnimPure = new WindowAnimPure_FadeAndScaleMirror();
        
        
        
        public override void OpenImmediate(Window window, WindowParameters parameters)
        {
            windowAnimPure.minScale = minScale;
            windowAnimPure.normalScale = normalScale;
            windowAnimPure.OpenImmediate(window, parameters);
        }
        
        public override void Open(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            windowAnimPure.minScale = minScale;
            windowAnimPure.normalScale = normalScale;
            windowAnimPure.Open(window, duration, ignoreTimeScale, parameters, onFinished);
        }

        
        

        
        public override void CloseImmediate(Window window, WindowParameters parameters)
        {
            windowAnimPure.minScale = minScale;
            windowAnimPure.normalScale = normalScale;
            windowAnimPure.CloseImmediate(window, parameters);
        }
        
        public override void Close(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            windowAnimPure.minScale = minScale;
            windowAnimPure.normalScale = normalScale;
            windowAnimPure.Close(window, duration, ignoreTimeScale, parameters, onFinished);
        }
    }
}