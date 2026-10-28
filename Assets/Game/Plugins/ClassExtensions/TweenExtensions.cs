
using DG.Tweening;

public static class TweenExtensions
{
    public static void CleanKill(this Tween tween)
    {
        if (tween != null && tween.IsActive()) tween.Kill();
    }
}
