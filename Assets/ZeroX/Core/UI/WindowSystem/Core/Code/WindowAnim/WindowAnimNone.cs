using System;

namespace ZeroX.WindowSystem
{
    public class WindowAnimNone : IWindowAnim
    {
        public void OpenImmediate(Window window, WindowParameters parameters)
        {
        }

        public void Open(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            onFinished?.Invoke();
        }

        public void CloseImmediate(Window window, WindowParameters parameters)
        {
            
        }

        public void Close(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            onFinished?.Invoke();
        }
    }
}