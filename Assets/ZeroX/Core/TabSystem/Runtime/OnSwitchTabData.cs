namespace ZeroX.TabSystem
{
    public class OnSwitchTabData
    {
        public int prevTabIndex;
        public int currentTabIndex;
        
        public string prevTabId;
        public string currentTabId;
        
        public TabData prevTabData;
        public TabData currentTabData;


        public OnSwitchTabData()
        {
        }
        
        public OnSwitchTabData(TabView tabView, int prevTabIndex, int currentTabIndex)
        {
            this.prevTabIndex = prevTabIndex;
            this.currentTabIndex = currentTabIndex;
            
            prevTabData = prevTabIndex < 0 ? null : tabView.GetTabData(prevTabIndex);
            currentTabData = tabView.GetTabData(currentTabIndex);

            prevTabId = prevTabData == null ? null : prevTabData.Id;
            currentTabId = currentTabData.Id;
        }
    }
}