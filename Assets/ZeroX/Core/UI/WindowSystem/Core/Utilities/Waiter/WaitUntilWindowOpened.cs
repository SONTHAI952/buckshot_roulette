using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WaitUntilWindowOpened : CustomYieldInstruction
    {
        private Window window;

        public override bool keepWaiting => window.WindowState != WindowState.Opened;
        
        

        public WaitUntilWindowOpened(Window window)
        {
            if (window == null)
            {
                Debug.LogError("Window to wait is null!");
            }
            
            this.window = window;
        }
    }
}