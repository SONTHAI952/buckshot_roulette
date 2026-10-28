using UnityEngine;
using ZeroX.UI;

namespace ZeroX.ProgressSystem
{
    [AddComponentMenu(ComponentMenuUtility.progressTargets + "Progress Target Sliced Filled Image")]
    public class ProgressTargetSlicedFilledImage : ProgressTarget
    {
        public TargetProgress targetProgress = TargetProgress.AnimateProgress;
        public SlicedFilledImage image;
        
        public override void UpdateTarget(Progressor progressor)
        {
            image.fillAmount = ProgressUtility.GetTargetProgressValue(progressor, targetProgress);
        }
        
        protected override void Reset()
        {
            base.Reset();
            image = GetComponent<SlicedFilledImage>();
        }
    }
}