using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace ZeroX.ProgressSystem
{
    public static class ProgressorExtension
    {
        public static TweenerCore<float, float, FloatOptions> DOValue(this Progressor target, float endValue, float duration, bool snapping = false)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.Value, x => target.Value = x, endValue, duration);
            t.SetOptions(snapping).SetTarget(target);
            return t;
        }
        
        public static TweenerCore<float, float, FloatOptions> DOProgress(this Progressor target, float endValue, float duration, bool snapping = false)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.Progress, x => target.Progress = x, endValue, duration);
            t.SetOptions(snapping).SetTarget(target);
            return t;
        }
    }
}