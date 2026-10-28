using UnityEngine;

namespace ZeroX
{
    public class ZListDrawerAttribute : PropertyAttribute
    {
        public float labelWidth = 25;
        
        public ZListDrawerAttribute()
        {
        }

        public ZListDrawerAttribute(float labelWidth)
        {
            this.labelWidth = labelWidth;
        }
    }
}