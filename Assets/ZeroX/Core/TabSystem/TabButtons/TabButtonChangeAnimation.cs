using UnityEngine;

namespace ZeroX.TabSystem
{
    public class TabButtonChangeAnimation : TabButton
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string normalStateName = "normal";
        [SerializeField] private string selectedStateName = "selected";
        [SerializeField] private float transitionDuration = 0.15f;


        protected override void Reset()
        {
            base.Reset();
            animator = GetComponent<Animator>();
        }

        public override void OnTabViewSwitchTab()
        {
            string nextStateName = TabView.CurrentTabIndex == TabIndex ? selectedStateName : normalStateName;
            animator.CrossFadeInFixedTime(nextStateName, transitionDuration);
        }
    }
}