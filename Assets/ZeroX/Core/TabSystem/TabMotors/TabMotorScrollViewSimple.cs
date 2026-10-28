using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.TabSystem
{
    public class TabMotorScrollViewSimple : TabMotor
    {
        [SerializeField] private ScrollRect scrollRect;
        
        
        public override void OnTabViewTransition()
        {
            if (scrollRect.horizontal && scrollRect.vertical)
            {
                Debug.LogError("ScrollRect enable both horizontal and vertical, please enable only one");
                return;
            }


            if (scrollRect.horizontal)
            {
                float delta;
                if (TabView.TabCount < 2)
                    delta = TabView.CurrentSmoothTabIndex;
                else
                    delta = TabView.CurrentSmoothTabIndex / (TabView.TabCount - 1);
                
                scrollRect.horizontalNormalizedPosition = delta;
            }
        }
    }
}