using UnityEngine;
using ZeroX.WindowSystem;

namespace ZeroX.TabSystem
{
    public class TabContentUseWindow : TabContent
    {
        [SerializeField] private Window window;


        protected override void Reset()
        {
            base.Reset();

            window = GetComponent<Window>();
        }

        public override void OnTabViewSwitchTab()
        {
            if (TabIndex == TabView.CurrentTabIndex)
            {
                window.Open();
            }
            else
            {
                window.Close();
            }
        }
    }
}