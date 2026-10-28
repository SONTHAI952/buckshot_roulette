using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WaitUntilWindowOpeningOrOpened : CustomYieldInstruction
    {
        private Window window;

        public override bool keepWaiting => window.IsOpeningOrOpened == false;
        
        

        public WaitUntilWindowOpeningOrOpened(Window window)
        {
            if (window == null)
            {
                Debug.LogError("Window to wait is null!");
            }
            
            this.window = window;
        }
    }
}