using UnityEngine;

namespace ZeroX.ProgressSystem
{
    [AddComponentMenu(ComponentMenuUtility.progressTargets + "Progress Target Two Point")]
    public class ProgressTargetTwoPoint : ProgressTarget
    {
        public TargetProgress targetProgress = TargetProgress.AnimateProgress;
        public Transform targetMove;
        public Transform pointA;
        public Transform pointB;
        
        
        public override void UpdateTarget(Progressor progressor)
        {
            float progress = ProgressUtility.GetTargetProgressValue(progressor, targetProgress);
            targetMove.position = Vector3.Lerp(pointA.position, pointB.position, progress);
        }
    }
}