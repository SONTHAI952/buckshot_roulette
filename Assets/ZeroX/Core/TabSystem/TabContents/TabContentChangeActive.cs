namespace ZeroX.TabSystem
{
    public class TabContentChangeActive : TabContent
    {
        public override void OnTabViewSwitchTab()
        {
            Content.SetActive(TabIndex == TabView.CurrentTabIndex);
        }
    }
}