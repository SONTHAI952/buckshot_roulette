using UnityEngine;

namespace ZeroX.ProgressSystem
{
    [AddComponentMenu(ComponentMenuUtility.progressTargets + "Progress Target Animator")]
    public class ProgressTargetAnimator : ProgressTarget
    {
        public TargetProgress targetProgress = TargetProgress.AnimateProgress;
        public Animator animator;
        public string parameterName = "progress";
        
        public override void UpdateTarget(Progressor progressor)
        {
            float progress = ProgressUtility.GetTargetProgressValue(progressor, targetProgress);
            
#if UNITY_EDITOR
            if(UnityEditor.EditorApplication.isPlaying == false)
                return;
#endif
            animator.SetFloat(parameterName, progress);
        }

        protected override void Reset()
        {
            base.Reset();
            animator = GetComponent<Animator>();
        }
    }
}