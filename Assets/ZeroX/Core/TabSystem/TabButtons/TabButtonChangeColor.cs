using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.TabSystem
{
    public class TabButtonChangeColor : TabButton
    {
        [SerializeField] private Image imageBackground;
        [SerializeField] Color normalColor = Color.white;
        [SerializeField] Color selectedColor = Color.green;
        
        protected override void Reset()
        {
            base.Reset();
            imageBackground = GetComponent<Image>();
        }
        
        public override void OnTabViewSwitchTab()
        {
            imageBackground.color = TabView.CurrentTabIndex == TabIndex ? selectedColor : normalColor;
        }
    }
}