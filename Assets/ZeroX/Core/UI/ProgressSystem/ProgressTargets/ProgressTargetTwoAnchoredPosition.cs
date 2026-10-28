using UnityEngine;

namespace ZeroX.ProgressSystem
{
    [AddComponentMenu(ComponentMenuUtility.progressTargets + "Progress Target Two Anchored Position")]
    public class ProgressTargetTwoAnchoredPosition : ProgressTarget
    {
        public TargetProgress targetProgress = TargetProgress.AnimateProgress;
        public RectTransform targetMove;
        public Vector3 anchoredPositionA;
        public Vector3 anchoredPositionB;
        
        
        public override void UpdateTarget(Progressor progressor)
        {
            float progress = ProgressUtility.GetTargetProgressValue(progressor, targetProgress);
            targetMove.anchoredPosition3D = Vector3.Lerp(anchoredPositionA, anchoredPositionB, progress);
        }
    }
}