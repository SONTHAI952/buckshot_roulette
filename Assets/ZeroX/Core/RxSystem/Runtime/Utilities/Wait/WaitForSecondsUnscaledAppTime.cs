using UnityEngine;

namespace ZeroX.RxSystem
{
    public class WaitForSecondsUnscaledAppTime : CustomYieldInstruction
    {
        public float waitTime;
        
        private float waitUntilTime = -1f;
        
        

        public override bool keepWaiting
        {
            get
            {
                if (waitUntilTime < 0.0)
                    waitUntilTime = ZeroRx.UnscaledAppTime + waitTime;
                
                bool keepWaiting = ZeroRx.UnscaledAppTime < waitUntilTime;
                if (!keepWaiting)
                    this.Reset();
                
                return keepWaiting;
            }
        }

        public WaitForSecondsUnscaledAppTime(float time)
        {
            waitTime = time;
        }

        public override void Reset()
        { 
            this.waitUntilTime = -1f;
        }
    }
}