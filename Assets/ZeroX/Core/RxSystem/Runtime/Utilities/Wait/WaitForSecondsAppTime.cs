using UnityEngine;

namespace ZeroX.RxSystem
{
    public class WaitForSecondsAppTime : CustomYieldInstruction
    {
        public float waitTime;
        
        private float waitUntilTime = -1f;
        
        

        public override bool keepWaiting
        {
            get
            {
                if (waitUntilTime < 0.0)
                    waitUntilTime = ZeroRx.AppTime + waitTime;
                
                bool keepWaiting = ZeroRx.AppTime < waitUntilTime;
                if (!keepWaiting)
                    this.Reset();
                
                return keepWaiting;
            }
        }

        public WaitForSecondsAppTime(float time)
        {
            waitTime = time;
        }

        public override void Reset()
        { 
            this.waitUntilTime = -1f;
        }
    }
}