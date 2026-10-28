using System;
using System.Collections.Generic;

namespace ZeroX.WindowSystem
{
    public interface IWindowAnim
    {
        public void OpenImmediate(Window window, WindowParameters parameters);
        public void Open(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished);
        
        
        public void CloseImmediate(Window window, WindowParameters parameters);
        public void Close(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished);
    }
}