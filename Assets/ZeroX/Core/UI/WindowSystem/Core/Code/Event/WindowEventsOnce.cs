using System;

namespace ZeroX.WindowSystem
{
    public class WindowEventsOnce
    {
        public Action<WindowEventData> OnOpen, OnOpened, OnClose, OnClosed;
    }
}