using UnityEngine;

namespace ZeroX.Extensions
{
    public static class AnimationCurveExtension
    {
        public static AnimationCurve OutSine(float timeStart, float valueStart, float timeEnd, float valueEnd)
        {
            if (timeStart == timeEnd)
            {
                return new AnimationCurve(new Keyframe[1]
                {
                    new Keyframe(timeStart, valueStart)
                });
            }
            else
            {
                return new AnimationCurve(new Keyframe[2]
                {
                    new Keyframe(timeStart, valueStart, 0, 2f),
                    new Keyframe(timeEnd, valueEnd, 0, 0)
                });
            }
        }
        
        public static AnimationCurve InSine(float timeStart, float valueStart, float timeEnd, float valueEnd)
        {
            if (timeStart == timeEnd)
            {
                return new AnimationCurve(new Keyframe[1]
                {
                    new Keyframe(timeStart, valueStart)
                });
            }
            else
            {
                return new AnimationCurve(new Keyframe[2]
                {
                    new Keyframe(timeStart, valueStart, 0, 0),
                    new Keyframe(timeEnd, valueEnd, 2, 0)
                });
            }
        }
    }
}