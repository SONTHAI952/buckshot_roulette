using System;
using UnityEngine;

namespace ZeroX.WindowSystem.Demo
{
    public class LogWindowName : MonoBehaviour
    {
        private Window window;

        private void Awake()
        {
            window = GetComponent<Window>();
            window.WindowEvents.OnOpen.AddListener(Window_OnOpen);
            window.WindowEvents.OnClose.AddListener(Window_OnClose);
        }

        private void Window_OnOpen(WindowEventData eventData)
        {
            Debug.Log("Window_OnOpen: " + this.name);
        }
        
        private void Window_OnClose(WindowEventData eventData)
        {
            Debug.Log("Window_OnClose: " + this.name);
        }
    }
}