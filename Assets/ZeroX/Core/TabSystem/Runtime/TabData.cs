using UnityEngine;

namespace ZeroX.TabSystem
{
    [System.Serializable]
    public class TabData
    {
        [SerializeField] string id;
        [SerializeField] GameObject button;
        [SerializeField] GameObject content;
        [SerializeField] TabView childTabView;

        //Cache
        private TabButton tabButton;
        private bool cachedButton = false;

        private TabContent tabContent;
        private bool cachedContent = false;
        
        
        #region Constructor

        public TabData(string id, GameObject button, GameObject content, TabView childTabView)
        {
            this.id = id;
            this.button = button;
            this.content = content;
            this.childTabView = childTabView;
        }

        public TabData(string id, GameObject button, GameObject content)
        {
            this.id = id;
            this.button = button;
            this.content = content;
        }

        public TabData(string id, GameObject content)
        {
            this.id = id;
            this.content = content;
        }

        #endregion
        
        
        

        public string Id => id;
        
        public TabButton TabButton
        {
            get
            {
                if (cachedButton == false)
                {
                    cachedButton = true;
                    if(button != null)
                        tabButton = button.GetComponent<TabButton>();
                }

                return tabButton;
            }
        }
        
        public TabContent TabContent
        {
            get
            {
                if (cachedContent == false)
                {
                    cachedContent = true;
                    tabContent = content.GetComponent<TabContent>();
                }

                return tabContent;
            }
        }

        public TabView ChildTabView => childTabView;
    }
}