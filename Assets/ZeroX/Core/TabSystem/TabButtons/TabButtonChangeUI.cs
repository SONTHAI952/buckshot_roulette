using UnityEngine;

namespace ZeroX.TabSystem
{
    public class TabButtonChangeUI : TabButton
    {
        [SerializeField] private GameObject normalUI;
        [SerializeField] private GameObject selectedUI;
        
        public override void OnTabViewSwitchTab()
        {
            bool isSelected = TabView.CurrentTabIndex == TabIndex;
            normalUI.SetActive(!isSelected);
            selectedUI.SetActive(isSelected);
        }
    }
}