using DG.Tweening;
using UnityEngine;

namespace ZeroX.ToggleSystem
{
    public class ZToggleTargetTwoPoint : ZToggleTarget
    {
        public Transform targetMove;
        public Transform pointOff;
        public Transform pointOn;
        public float transitionDuration = 0.25f;

        private Tween currentTween;
        
        public override void UpdateTarget(ZToggle toggle)
        {
            KillCurrentTween();

            Transform destPoint = null;
            destPoint = toggle.IsOn ? pointOn : pointOff;
            
            
            float totalDistance = Vector3.Distance(pointOff.localPosition, pointOn.localPosition);
            float distance = Vector3.Distance(targetMove.localPosition, destPoint.localPosition);
            float duration = (distance / totalDistance) * transitionDuration;
            currentTween = targetMove.DOLocalMove(destPoint.localPosition, duration).SetEase(Ease.Linear).SetUpdate(true);
        }

        public override void UpdateTargetImmediate(ZToggle toggle)
        {
            KillCurrentTween();
            if (toggle.IsOn)
            {
                targetMove.localPosition = pointOn.localPosition;
            }
            else
            {
                targetMove.localPosition = pointOff.localPosition;
            }
        }
        
        void KillCurrentTween()
        {
            if (currentTween != null)
            {
                currentTween.Kill();
                currentTween = null;
            }
        }
    }
}