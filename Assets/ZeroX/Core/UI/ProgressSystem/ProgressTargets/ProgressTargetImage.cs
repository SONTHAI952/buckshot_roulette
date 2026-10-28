using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.ProgressSystem
{
    [AddComponentMenu(ComponentMenuUtility.progressTargets + "Progress Target Image")]
    public class ProgressTargetImage : ProgressTarget
    {
        public TargetProgress targetProgress = TargetProgress.AnimateProgress;
        public Image image;
        
        public override void UpdateTarget(Progressor progressor)
        {
            image.fillAmount = ProgressUtility.GetTargetProgressValue(progressor, targetProgress);
        }

        protected override void Reset()
        {
            base.Reset();
            image = GetComponent<Image>();
        }
    }
}