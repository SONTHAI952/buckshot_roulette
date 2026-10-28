using System.Reflection;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WindowAvatar : Window
    {
        [Header("Window Avatar")]
        [SerializeField] private Window originalWindow;
        
        private bool initialized = false;
        
        

        void InitIfNot()
        {
            if(initialized)
                return;
            initialized = true;
            
            originalWindow.WindowEvents.OnOpen.AddListener(OnOriginalWindowOpen);
            originalWindow.WindowEvents.OnOpened.AddListener(OnOriginalWindowOpened);
            originalWindow.WindowEvents.OnClose.AddListener(OnOriginalWindowClose);
            originalWindow.WindowEvents.OnClosed.AddListener(OnOriginalWindowClosed);
        }

        void OnOriginalWindowOpen(WindowEventData data)
        {
            SetWindowState(data.window.PreWindowState, data.window.WindowState);
            FireEvent_OnOpen(data);
        }
        
        void OnOriginalWindowOpened(WindowEventData data)
        {
            SetWindowState(data.window.PreWindowState, data.window.WindowState);
            FireEvent_OnOpened(data);
        }
        
        void OnOriginalWindowClose(WindowEventData data)
        {
            SetWindowState(data.window.PreWindowState, data.window.WindowState);
            FireEvent_OnClose(data);
        }
        
        void OnOriginalWindowClosed(WindowEventData data)
        {
            SetWindowState(data.window.PreWindowState, data.window.WindowState);
            FireEvent_OnClosed(data);
        }

        void SetWindowState(WindowState preWindowState, WindowState windowState)
        {
            PreWindowState = preWindowState;
            WindowState = windowState;
        }

        protected override GameObject GetWindowObject()
        {
            return originalWindow.gameObject;
        }

        public override void OpenWithOption(OpenOption option)
        {
            InitIfNot();
            originalWindow.OpenWithOption(option);
        }

        public override void CloseWithOption(CloseOption option)
        {
            InitIfNot();
            originalWindow.CloseWithOption(option);
        }
    }
}