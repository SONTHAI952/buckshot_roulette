using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WaitUntilWindowClosed : CustomYieldInstruction
    {
        private Window window;

        public override bool keepWaiting => window.WindowState != WindowState.Closed;
        
        

        public WaitUntilWindowClosed(Window window)
        {
            if (window == null)
            {
                Debug.LogError("Window to wait is null!");
            }
            
            this.window = window;
        }
    }
}