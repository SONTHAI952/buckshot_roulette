using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WaitUntilWindowClosingOrClosed : CustomYieldInstruction
    {
        private Window window;

        public override bool keepWaiting => window.IsClosingOrClosed == false;
        
        

        public WaitUntilWindowClosingOrClosed(Window window)
        {
            if (window == null)
            {
                Debug.LogError("Window to wait is null!");
            }
            
            this.window = window;
        }
    }
}