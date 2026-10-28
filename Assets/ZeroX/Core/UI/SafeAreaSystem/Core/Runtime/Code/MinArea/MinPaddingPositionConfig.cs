using UnityEngine;

namespace ZeroX.SafeAreaSystem
{
    [System.Serializable]
    public class MinPaddingPositionConfig
    {
        public bool enable;
        public float value;
        
        [Tooltip("Does this min value include the padding of the original safe area?")]
        public bool includeOriginalSafePadding = true;


        public MinPaddingPositionConfig()
        {
        }

        public MinPaddingPositionConfig(bool enable, float value)
        {
            this.enable = enable;
            this.value = value;
        }


        public MinPaddingPositionConfig Clone()
        {
            MinPaddingPositionConfig clone = new MinPaddingPositionConfig();
            clone.enable = enable;
            clone.value = value;

            return clone;
        }
    }
}