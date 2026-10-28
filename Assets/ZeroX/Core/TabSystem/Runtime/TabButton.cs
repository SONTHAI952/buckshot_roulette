using System;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.TabSystem
{
    public abstract class TabButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        protected TabView OldTabView { get; private set; }
        public TabView TabView { get; private set; }

        protected int OldTabIndex { get; private set; } = -1;
        public int TabIndex { get; private set; } = -1;
        
        private bool isInitialized = false;
        public Button Button => button;
        
        

        #region Unity Method

        protected virtual void Reset()
        {
            button = GetComponent<Button>();
        }

        #endregion


        #region Init

        /// <summary>
        /// Init things that belong to themselves and are not related to TabView
        /// </summary>
        internal void InitIfNot()
        {
            if(isInitialized)
                return;
            isInitialized = true;
            
            button.onClick.AddListener(InternalButton_OnClick);
            ExecuteInit();
        }

        /// <summary>
        /// Init things that belong to themselves and are not related to TabView
        /// </summary>
        protected virtual void ExecuteInit()
        {
        }

        #endregion



        #region SetTabView

        internal void SetTabView(TabView tabView, int tabIndex)
        {
            if (tabView == null)
                throw new Exception("TabView to setup is null");
            
            OldTabView = TabView;
            TabView = tabView;

            OldTabIndex = TabIndex;
            TabIndex = tabIndex;
            
            OnTabViewChanged();
        }

        /// <summary>
        /// When this function is called, the OldTabView, TabView, OldTabIndex, TabIndex are already set. Just get information from there
        /// </summary>
        protected virtual void OnTabViewChanged()
        {
        }

        #endregion



        #region SetTabIndex

        internal void SetTabIndex(int tabIndex)
        {
            OldTabIndex = TabIndex;
            TabIndex = tabIndex;
            
            OnTabIndexChanged();
        }

        /// <summary>
        /// When this function is called, the OldTabIndex, TabIndex are already set. Just get information from there
        /// </summary>
        protected virtual void OnTabIndexChanged()
        {
        }

        #endregion

        




        void InternalButton_OnClick()
        {
            try
            {
                TabView.OpenTab(TabIndex);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public virtual void OnTabViewTransition()
        {
        }

        public virtual void OnTabViewSwitchTab()
        {
        }
    }
}