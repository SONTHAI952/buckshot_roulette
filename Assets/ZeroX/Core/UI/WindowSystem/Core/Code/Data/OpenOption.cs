using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    [System.Serializable]
    public class OpenOption
    {
        [Min(0)]
        public float duration = 0.35f;
        public float delay;
        public bool fireEvent = true;

        public WindowParameters parameters;

        public OpenOption()
        {
        }
        
        public OpenOption(float duration, float delay, bool fireEvent)
        {
            this.duration = duration;
            this.delay = delay;
            this.fireEvent = fireEvent;
        }

        public bool IsZeroDuration()
        {
            return duration <= 0 || Mathf.Approximately(duration, 0);
        }

        public OpenOption Clone()
        {
            OpenOption option = new OpenOption();
            option.duration = duration;
            option.delay = delay;
            option.fireEvent = fireEvent;
            option.parameters = parameters;
            return option;
        }
    }
}