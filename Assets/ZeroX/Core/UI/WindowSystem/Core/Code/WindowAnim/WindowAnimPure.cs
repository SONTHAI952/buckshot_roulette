using System;

namespace ZeroX.WindowSystem
{
    public abstract class WindowAnimPure : IWindowAnim
    {
        public abstract void OpenImmediate(Window window, WindowParameters parameters);
        public abstract void Open(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished);
        
        
        public abstract void CloseImmediate(Window window, WindowParameters parameters);
        public abstract void Close(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished);
    }
}