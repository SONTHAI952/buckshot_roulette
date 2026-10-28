using System;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public abstract class ComponentInsideWindow : MonoBehaviour
    {
        private Window window;

        protected void Awake()
        {
            window = GetComponentInParent<Window>();
            if (window != null)
            {
                window.WindowEvents.OnOpen.AddListener(Window_OnOpen);
                window.WindowEvents.OnOpened.AddListener(Window_OnOpened);
                window.WindowEvents.OnClose.AddListener(Window_OnClose);
                window.WindowEvents.OnClosed.AddListener(Window_OnClosed);
            }
            
            OnAwake();
        }

        protected void Start()
        {
            OnStart();
        }

        private void OnEnable()
        {
            if (window == null)
            {
                Window_OnOpen(new WindowEventData(null, null));
                Window_OnOpened(new WindowEventData(null, null));
            }
        }

        private void OnDisable()
        {
            if (window == null)
            {
                Window_OnClose(new WindowEventData(null, null));
                Window_OnClosed(new WindowEventData(null, null));
            }
        }


        protected virtual void OnAwake()
        {
            
        }
        
        protected virtual void OnStart()
        {
            
        }
        
        protected virtual void Window_OnOpen(WindowEventData eventData)
        {
            
        }
        
        protected virtual void Window_OnOpened(WindowEventData eventData)
        {
            
        }
        
        protected virtual void Window_OnClose(WindowEventData eventData)
        {
            
        }
        
        protected virtual void Window_OnClosed(WindowEventData eventData)
        {
            
        }
    }
}