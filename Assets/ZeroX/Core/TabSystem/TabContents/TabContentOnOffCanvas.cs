using UnityEngine;

namespace ZeroX.TabSystem
{
    public class TabContentOnOffCanvas : TabContent
    {
        [SerializeField] private Canvas canvas;

        protected override void Reset()
        {
            base.Reset();
            canvas = GetComponent<Canvas>();
        }

        public override void OnTabViewTransition()
        {
            
        }

        public override void OnTabViewSwitchTab()
        {
            if(canvas.gameObject.activeSelf == false)
                canvas.gameObject.SetActive(true);
            
            canvas.enabled = TabIndex == TabView.CurrentTabIndex;
        }
    }
}