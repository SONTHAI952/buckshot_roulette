using UnityEngine;

namespace ZeroX.WindowSystem.Demo
{
    public class PopupA : WindowController<PopupA>
    {
        protected override void WhenAwake()
        {
            Debug.Log("Window Path: " + GetCurrentWindowPath());
        }


        protected override void Window_OnOpen()
        {
            Debug.Log("Popup OnOpen");
        }

        protected override void Window_OnClose()
        {
        }
    }
}