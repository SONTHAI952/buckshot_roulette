using System;
using UnityEngine;

namespace ZeroX.TabSystem
{
    public abstract class TabMotor : MonoBehaviour
    {
        protected TabView OldTabView { get; private set; }
        public TabView TabView { get; private set; }
        
        private bool isInitialized = false;
        
        
        
        #region Init

        /// <summary>
        /// Init things that belong to themselves and are not related to TabView
        /// </summary>
        internal void InitIfNot()
        {
            if(isInitialized)
                return;
            isInitialized = true;
            
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

        internal void SetTabView(TabView tabView)
        {
            if (tabView == null)
                throw new Exception("TabView to setup is null");
            
            OldTabView = TabView;
            TabView = tabView;

            OnTabViewChanged();
        }

        /// <summary>
        /// When this function is called, the OldTabView, TabView, OldTabIndex, TabIndex are already set. Just get information from there
        /// </summary>
        protected virtual void OnTabViewChanged()
        {
        }

        #endregion



        public virtual void OnTabViewTransition()
        {
        }

        public virtual void OnTabViewSwitchTab()
        {
        }

        public virtual void OnTabViewAddTab(TabData tabData, int index)
        {
        }

        public virtual void OnTabViewRemoveTab(TabData tabData, int index)
        {
        }
    }
}