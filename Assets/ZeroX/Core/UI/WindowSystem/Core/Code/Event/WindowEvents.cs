using UnityEngine;
using UnityEngine.Events;

namespace ZeroX.WindowSystem
{
    [System.Serializable]
    public class WindowUnityEvent : UnityEvent<WindowEventData>
    {}
    
    [System.Serializable]
    public class WindowEvents
    {
        [SerializeField] private WindowUnityEvent onOpen = new WindowUnityEvent();
        [SerializeField] private WindowUnityEvent onOpened = new WindowUnityEvent();
        [SerializeField] private WindowUnityEvent onClose = new WindowUnityEvent();
        [SerializeField] private WindowUnityEvent onClosed = new WindowUnityEvent();

        public WindowUnityEvent OnOpen => onOpen;
        public WindowUnityEvent OnOpened => onOpened;
        public WindowUnityEvent OnClose => onClose;
        public WindowUnityEvent OnClosed => onClosed;
    }
}