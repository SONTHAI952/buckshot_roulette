using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.TabSystem
{
    public class TabButtonChangeSprite : TabButton
    {
        [SerializeField] private Image imageBackground;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite selectedSprite;


        protected override void Reset()
        {
            base.Reset();
            imageBackground = GetComponent<Image>();
        }

        public override void OnTabViewSwitchTab()
        {
            imageBackground.sprite = TabView.CurrentTabIndex == TabIndex ? selectedSprite : normalSprite;
        }
    }
}