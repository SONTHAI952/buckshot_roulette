using System;

namespace ZeroX.WindowSystem
{
    public class WindowAnimMono_Fade : WindowAnimMono
    {
        private WindowAnimPure_Fade windowAnimPure = new WindowAnimPure_Fade();
        
        
        
        
        
        public override void OpenImmediate(Window window, WindowParameters parameters)
        {
            windowAnimPure.OpenImmediate(window, parameters);
        }
        
        public override void Open(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            windowAnimPure.Open(window, duration, ignoreTimeScale, parameters, onFinished);
        }

        

        

        public override void CloseImmediate(Window window, WindowParameters parameters)
        {
            windowAnimPure.CloseImmediate(window, parameters);
        }
        
        public override void Close(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            windowAnimPure.Close(window, duration, ignoreTimeScale, parameters, onFinished);
        }
    }
}