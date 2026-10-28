using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroX.TabSystem
{
    public enum TabViewTransitionType
    {
        Duration, Speed
    }

    public class TabView : MonoBehaviour
    {
        [SerializeField] private List<TabData> listTabData = new List<TabData>();
        [SerializeField] private List<TabMotor> listTabMotor = new List<TabMotor>();
        [SerializeField] private TabInteractive tabInteractive;

        [Header("Transition")] [SerializeField]
        protected bool ignoreTimeScale = true;

        [SerializeField] protected TabViewTransitionType transitionType = TabViewTransitionType.Speed;
        [SerializeField] protected AnimationCurve transitionEase = AnimationCurve.Linear(0, 0, 1 ,1);
        [Min(0)] [SerializeField] protected float transitionDuration = 0.25f;
        [SerializeField] protected float transitionSpeed = 4f;
        [SerializeField] private float snapSpeed = 4f;

        [Header("Misc")] [SerializeField] protected FirstAction firstAction = new FirstAction();



        private bool IsInitialized { get; set; }


        //Tab Index
        public int CurrentTabIndex { get; protected set; } //Sẽ đc thay đổi bởi hàm OpenTab nên ko cần hàm set riêng
        public int PrevTabIndex { get; protected set; }
        private bool everChangedTabIndex = false;

        public float
            CurrentSmoothTabIndex { get; protected set; } //Có thể bị thay đổi bởi nhiều chỗ nên có hàm set riêng

        public float PreSmoothTabIndex { get; set; }
        private bool everChangedSmoothTabIndex = false;


        //Misc
        public int TabCount => listTabData.Count;
        public bool IsBlockInteractive { get; protected set; }

        public TabInteractive TabInteractive
        {
            get => tabInteractive;
            set => tabInteractive = value;
        }


        //Transition
        public bool IgnoreTimeScale
        {
            get => ignoreTimeScale;
            set => ignoreTimeScale = value;
        }

        public TabViewTransitionType TransitionType
        {
            get => transitionType;
            set => transitionType = value;
        }

        public float TransitionDuration
        {
            get => transitionDuration;
            set => transitionDuration = value;
        }

        public float TransitionSpeed
        {
            get => transitionSpeed;
            set => transitionSpeed = value;
        }

        public float SnapSpeed
        {
            get => snapSpeed;
            set => snapSpeed = value;
        }


        //Events
        public event Action onTransition;
        public event Action<OnSwitchTabData> onSwitchTab;

        /// <summary>
        /// TabData and tabIndex
        /// </summary>
        public event Action<TabData, int> onAddTab;

        /// <summary>
        /// TabData and tabIndex
        /// </summary>
        public event Action<TabData, int> onRemoveTab;


        //Temp
        private bool isFirstEnable = true;
        private Coroutine tweenSmoothTabIndex;





        #region Unity Method

        protected virtual void Awake()
        {
            InitIfNot();
            SetTabViewForAllModule();
            
            if(firstAction.when == FirstAction.WhenType.Awake)
                CheckAndExecuteFirstAction();
        }

        protected virtual void OnEnable()
        {
            if (isFirstEnable)
            {
                isFirstEnable = false;
                
                if (firstAction.when == FirstAction.WhenType.FirstEnable)
                    CheckAndExecuteFirstAction();
            }
            else
            {
                SetCurrentSmoothTabIndex(CurrentTabIndex, true);
            }
        }

        protected void OnDisable()
        {
            IsBlockInteractive = false;
        }

        protected virtual void Start()
        {
            if(firstAction.when == FirstAction.WhenType.Start)
                CheckAndExecuteFirstAction();
        }

        #endregion


        #region Init

        void InitIfNot()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            //List TabData
            for (int i = 0; i < listTabData.Count; i++)
            {
                var tabData = listTabData[i];
                InitTabData(tabData, i);
            }

            //TabInteractive
            if (tabInteractive != null)
                InitTabInteractive(tabInteractive);

            //Tab Motors
            foreach (var tabMotor in listTabMotor)
            {
                InitTabMotor(tabMotor);
            }
        }

        void InitTabData(TabData tabData, int tabIndex)
        {
            //Button
            if (tabData.TabButton != null)
            {
                tabData.TabButton.InitIfNot();
            }

            //Content
            tabData.TabContent.InitIfNot();
        }

        void InitTabInteractive(TabInteractive tabInteractive)
        {
            tabInteractive.InitIfNot();
        }

        void InitTabMotor(TabMotor tabMotor)
        {
            tabMotor.InitIfNot();
        }

        #endregion

        #region SetTabViewForAllModule

        void SetTabViewForAllModule()
        {
            //ListTabData
            for (int i = 0; i < listTabData.Count; i++)
            {
                var tabData = listTabData[i];
                SetTabViewForTabData(tabData, i);
            }

            //Interactive
            if(tabInteractive != null)
                SetTabViewForTabInteractive(tabInteractive);


            //Motors
            foreach (var tabMotor in listTabMotor)
            {
                if(tabMotor != null)
                    SetTabViewForTabMotor(tabMotor);
            }
        }

        void SetTabViewForTabData(TabData tabData, int tabIndex)
        {
            if (tabData.TabButton != null)
            {
                tabData.TabButton.SetTabView(this, tabIndex);
            }

            tabData.TabContent.SetTabView(this, tabIndex);
        }

        void SetTabViewForTabInteractive(TabInteractive tabInteractive)
        {
            tabInteractive.SetTabView(this);
        }
        
        void SetTabViewForTabMotor(TabMotor tabMotor)
        {
            tabMotor.SetTabView(this);
        }

        #endregion

        #region First Action

        void CheckAndExecuteFirstAction()
        {
            if(firstAction.isPerformed)
                return;

            firstAction.isPerformed = true;
            
            if(string.IsNullOrWhiteSpace(firstAction.openTab))
                return;
            
            if(firstAction.actionType == FirstAction.ActionType.Open)
                OpenTab(firstAction.openTab);
            else if(firstAction.actionType == FirstAction.ActionType.OpenImmediate)
                OpenTabImmediate(firstAction.openTab);
            else
            {
                Debug.LogError("Not code for firstAction.actionType: " + firstAction.actionType);
            }
        }

        #endregion


        #region OpenTab By Index

        private void InternalOpenTabImmediate(int tabIndex)
        {
            InitIfNot();
            
            if (listTabData.Count == 0)
            {
                Debug.LogError("List tab empty");
                return;
            }
            
            if (tabIndex < 0 || tabIndex >= listTabData.Count)
                throw new IndexOutOfRangeException("Tab index out of range");

            firstAction.isPerformed = true;
            
            //TabIndex
            bool tabIndexChanged = SetCurrentTabIndex(tabIndex, true, false);
            
            //SmoothTabIndex
            bool smoothTabIndexChanged = SetCurrentSmoothTabIndex(tabIndex, false);
            
            //Notify
            if(tabIndexChanged || smoothTabIndexChanged)
                NotifyTabViewTransition();
        }

        private void InternalOpenTab(int tabIndex, bool blockInteractive, float duration)
        {
            if ((tabIndex == CurrentTabIndex && Mathf.Approximately(tabIndex, CurrentSmoothTabIndex)) ||
                Mathf.Approximately(duration, 0) ||
                gameObject.activeInHierarchy == false)
            {
                InternalOpenTabImmediate(tabIndex);
                return;
            }
            
            InitIfNot();
            
            if (listTabData.Count == 0)
            {
                Debug.LogError("List tab empty");
                return;
            }
            
            if (tabIndex < 0 || tabIndex >= listTabData.Count)
                throw new IndexOutOfRangeException("Tab index out of range");

            firstAction.isPerformed = true;
            
            //TabIndex
            SetCurrentTabIndex(tabIndex, true, false);
            
            //SmoothTabIndex
            if (everChangedSmoothTabIndex)
            {
                if(Mathf.Approximately(CurrentSmoothTabIndex, CurrentTabIndex))
                    return;
            }
            
            DoSmoothTabIndex(CurrentTabIndex, blockInteractive, duration);
        }

        
        
        public void OpenTabImmediate(int tabIndex)
        {
            InternalOpenTabImmediate(tabIndex);
        }
        
        public void OpenTab(int tabIndex)
        {
            float duration = CalculateTransitionDuration(tabIndex);
            InternalOpenTab(tabIndex, false, duration);
        }
        
        /// <summary>
        /// OpenTab with blockInteractive
        /// </summary>
        public void OpenTabForce(int tabIndex)
        {
            float duration = CalculateTransitionDuration(tabIndex);
            InternalOpenTab(tabIndex, true, duration);
        }

        public void OpenTab(int tabIndex, float duration)
        {
            InternalOpenTab(tabIndex, false, duration);
        }
        
        /// <summary>
        /// OpenTab with blockInteractive
        /// </summary>
        public void OpenTabForce(int tabIndex, float duration)
        {
            InternalOpenTab(tabIndex, true, duration);
        }
        
        #endregion
        
        

        #region OpenTab By Path

        string[] GetListIdFromTabPath(string tabPath)
        {
            tabPath = tabPath.Replace('\\', '/');
            tabPath = tabPath.Trim('/');
            return tabPath.Split('/');
        }
        
        private TabData InternalOpenTabById(string tabId, bool immediate, bool blockInteractive)
        {
            int tabIndex = listTabData.FindIndex(d => d.Id == tabId);
            if (tabIndex < 0)
            {
                Debug.LogError("Has not tab with id: " + tabId);
                return null;
            }

            if (immediate)
            {
                InternalOpenTabImmediate(tabIndex);
            }
            else
            {
                float duration = CalculateTransitionDuration(tabIndex);
                InternalOpenTab(tabIndex, blockInteractive, duration);
            }
            
            return listTabData[tabIndex];
        }

        private void InternalOpenTabByPath(string tabPath, bool immediate, bool blockInteractive)
        {
            if (string.IsNullOrEmpty(tabPath))
            {
                Debug.LogError("TabPath cannot empty");
                return;
            }
            
            var listTabId = GetListIdFromTabPath(tabPath);

            TabView currentTabView = this;
            for (int i = 0; i < listTabId.Length; i++)
            {
                string tabId = listTabId[i];
                var tabData = currentTabView.InternalOpenTabById(tabId, immediate, blockInteractive);
                if(tabData == null)
                    return;

                if (i < listTabId.Length - 1)
                {
                    var nextTabView = tabData.ChildTabView;
                    if (nextTabView == null)
                    {
                        Debug.LogErrorFormat("Tab '{0}' has no childTabView to open tab '{1}'", tabId, listTabId[i + 1]);
                        return;
                    }

                    currentTabView = nextTabView;
                }
            }
        }

        public void OpenTabImmediate(string tabPath)
        {
            InternalOpenTabByPath(tabPath, true, false);
        }
        
        public void OpenTab(string tabPath)
        {
            InternalOpenTabByPath(tabPath, false, false);
        }
        
        public void OpenTabForce(string tabPath)
        {
            InternalOpenTabByPath(tabPath, false, true);
        }

        #endregion



        #region Get Tab By Index

        public TabData GetTabData(int tabIndex)
        {
            if (listTabData.Count == 0)
            {
                Debug.LogError("List tab empty");
                return null;
            }
            
            if (tabIndex < 0 || tabIndex >= listTabData.Count)
                throw new IndexOutOfRangeException("Tab index out of range");
            
            return listTabData[tabIndex];
        }

        public TabContent GetTabContent(int tabIndex)
        {
            var tabData = GetTabData(tabIndex);
            if (tabData == null)
                return null;

            return tabData.TabContent;
        }
        
        public TabButton GetTabButton(int tabIndex)
        {
            var tabData = GetTabData(tabIndex);
            if (tabData == null)
                return null;

            return tabData.TabButton;
        }

        #endregion
        
        

        #region Get Tab By Path

        private TabData InternalGetTabDataById(string tabId)
        {
            return listTabData.FirstOrDefault(d => d.Id == tabId);
        }

        public TabData GetTabData(string tabPath)
        {
            if (string.IsNullOrEmpty(tabPath))
            {
                Debug.LogError("TabPath cannot empty");
                return null;
            }
            
            
            
            var listTabId = GetListIdFromTabPath(tabPath);
            
            TabView currentTabView = this;
            TabData resultTabData = null;
            for (int i = 0; i < listTabId.Length; i++)
            {
                string tabId = listTabId[i];
                resultTabData = currentTabView.InternalGetTabDataById(tabId);
                if(resultTabData == null)
                    return null;

                
                if (i < listTabId.Length - 1)
                {
                    var nextTabView = resultTabData.ChildTabView;
                    if (nextTabView == null)
                    {
                        Debug.LogErrorFormat("Tab '{0}' has no childTabView to open tab '{1}'", tabId, listTabId[i + 1]);
                        return null;
                    }

                    currentTabView = nextTabView;
                }
            }

            return resultTabData;
        }

        public TabContent GetTabContent(string tabPath)
        {
            var tabData = GetTabData(tabPath);
            if (tabData == null)
                return null;

            return tabData.TabContent;
        }

        public TabButton GetTabButton(string tabPath)
        {
            var tabData = GetTabData(tabPath);
            if (tabData == null)
                return null;

            return tabData.TabButton;
        }

        #endregion


        #region Transition

        float CalculateTransitionDuration(float endSmoothTabIndex)
        {
            if (transitionType == TabViewTransitionType.Duration)
            {
                return transitionDuration;
            }
            else if (transitionType == TabViewTransitionType.Speed)
            {
                float distance = Mathf.Abs(CurrentSmoothTabIndex - endSmoothTabIndex);
                return distance / transitionSpeed;
            }
            else
            {
                Debug.LogError("Not code for transition type: " + transitionType);
                return transitionDuration;
            }
        }
        
        void DoSmoothTabIndex(float endSmoothTabIndex, bool blockInteractive, float duration)
        {
            if(tweenSmoothTabIndex != null)
                StopCoroutine(tweenSmoothTabIndex);

            tweenSmoothTabIndex = StartCoroutine(Timeline());
            
            

            IEnumerator Timeline()
            {
                IsBlockInteractive = blockInteractive;
                
                //Nếu làm như thế này thì càng nhiều tab thời gian transition càng lâu
                //float distance = Mathf.Abs(endIndex - SmoothTabIndex);
                //float duration = distance * transitionDuration;
                
                float fromSmoothTabIndex = CurrentSmoothTabIndex;
                
                float time = 0;
                while (time < duration)
                {
                    float delta = transitionEase.Evaluate(time / duration);
                    float newSmoothTabIndex = Mathf.Lerp(fromSmoothTabIndex, endSmoothTabIndex, delta);
                    SetCurrentSmoothTabIndex(newSmoothTabIndex, true);

                    yield return null;
                    
                    if (ignoreTimeScale)
                        time += Time.unscaledDeltaTime;
                    else
                        time += Time.deltaTime;
                }

                //End
                SetCurrentSmoothTabIndex(endSmoothTabIndex, true);
                
                tweenSmoothTabIndex = null;
                IsBlockInteractive = false;
            }
        }

        internal void StopCurrentTweenSmoothTabIndex()
        {
            if(tweenSmoothTabIndex != null)
                StopCoroutine(tweenSmoothTabIndex);

            tweenSmoothTabIndex = null;
            IsBlockInteractive = false;
        }

        #endregion
        


        #region SnapTab

        internal void InternalSnap(bool blockInteractive)
        {
            int nextTabIndex = Mathf.RoundToInt(CurrentSmoothTabIndex);
            nextTabIndex = Mathf.Clamp(nextTabIndex, 0, TabCount);
            
            float distance = Mathf.Abs(nextTabIndex - CurrentSmoothTabIndex);
            float duration = distance / snapSpeed;

            InternalOpenTab(nextTabIndex, false, duration);
            SetCurrentTabIndex(nextTabIndex, true, false);
        }
        
        public void SnapTab()
        {
            InternalSnap(false);
        }

        public void SnapTabForce()
        {
            InternalSnap(true);
        }

        #endregion
        
        
        
        
        
        /// <summary>
        /// Trả về true nếu có sự thay đổi hoặc là lần đầu set, trả về false nếu không
        /// </summary>
        internal bool SetCurrentTabIndex(int tabIndex, bool notifySwitchTab, bool notifyAllUpdateAccordingTabView)
        {
            if (tabIndex < 0 || tabIndex >= listTabData.Count)
                throw new IndexOutOfRangeException("Tab index out of range: " + tabIndex);

            bool oldEverChangedTabIndex = everChangedTabIndex;
            if (everChangedTabIndex)
            {
                if(CurrentTabIndex == tabIndex)
                    return false;
            }
            else
                everChangedTabIndex = true;
            
            PrevTabIndex = CurrentTabIndex;
            CurrentTabIndex = tabIndex;

            if (notifySwitchTab)
            {
                NotifySwitchTab(oldEverChangedTabIndex ? PrevTabIndex : -1, CurrentTabIndex);
            }
            
            if (notifyAllUpdateAccordingTabView)
            {
                NotifyTabViewTransition();
            }
            
            return true;
        }

        /// <summary>
        /// Trả về true nếu có sự thay đổi hoặc là lần đầu set, trả về false nếu không
        /// </summary>
        internal bool SetCurrentSmoothTabIndex(float smoothTabIndex, bool notifyAllUpdateAccordingTabView)
        {
            if (everChangedSmoothTabIndex)
            {
                if(Mathf.Approximately(CurrentSmoothTabIndex, smoothTabIndex))
                    return false;
            }
            else
                everChangedSmoothTabIndex = true;
            
            PreSmoothTabIndex = CurrentSmoothTabIndex;
            CurrentSmoothTabIndex = smoothTabIndex;
            
            if(notifyAllUpdateAccordingTabView)
                NotifyTabViewTransition();

            return true;
        }
        
        internal void NotifyTabViewTransition()
        {
            //Update listTabData
            for (int i = 0; i < listTabData.Count; i++)
            {
                var tabData = listTabData[i];
                
                //Button
                if (tabData.TabButton != null)
                {
                    try
                    {
                        tabData.TabButton.OnTabViewTransition();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                
                
                //Content
                try
                {
                    tabData.TabContent.OnTabViewTransition();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }


            //Update listTabMotor
            foreach (var motor in listTabMotor)
            {
                try
                {
                    motor.OnTabViewTransition();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            
            
            //Event
            try
            {
                onTransition?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        

        internal void NotifySwitchTab(int prevTabIndex, int currentTabIndex)
        {
            //Update listTabData
            for (int i = 0; i < listTabData.Count; i++)
            {
                var tabData = listTabData[i];
                
                //Button
                if (tabData.TabButton != null)
                {
                    try
                    {
                        tabData.TabButton.OnTabViewSwitchTab();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                
                
                //Content
                try
                {
                    tabData.TabContent.OnTabViewSwitchTab();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            
            
            //Update listTabMotor
            foreach (var motor in listTabMotor)
            {
                try
                {
                    motor.OnTabViewSwitchTab();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            
            
            //Event
            OnSwitchTabData data = new OnSwitchTabData(this, prevTabIndex, currentTabIndex);
            
            try
            {
                onSwitchTab?.Invoke(data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }



        #region Add Tab

        public void AddTab(string id, GameObject button, GameObject content, TabView childTab)
        {
            int tabIndex = listTabData.Count;
            TabData data = new TabData(id, button, content, childTab);
            listTabData.Add(data);
            InitTabData(data, tabIndex);
            SetTabViewForTabData(data, tabIndex);
            
            NotifyAddTab(data, tabIndex);
        }
        
        public void AddTab(string id, GameObject button, GameObject content)
        {
            AddTab(id, button, content, null);
        }
        
        public void AddTab(string id, GameObject content)
        {
            AddTab(id, null, content, null);
        }
        
        internal void NotifyAddTab(TabData tabData, int tabIndex)
        {
            //Motors
            foreach (var tabMotor in listTabMotor)
            {
                try
                {
                    tabMotor.OnTabViewAddTab(tabData, tabIndex);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            
            //Events
            try
            {
                onAddTab?.Invoke(tabData, tabIndex);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Remove Tab

        public void RemoveTab(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= listTabData.Count)
                throw new IndexOutOfRangeException("TabIndex out of range");

            var tabData = listTabData[tabIndex];
            listTabData.RemoveAt(tabIndex);

            //Set lại index
            for (int i = 0; i < listTabData.Count; i++)
            {
                var data = listTabData[0];
                if (data.TabButton != null)
                {
                    data.TabButton.SetTabIndex(i);
                }
                
                data.TabContent.SetTabIndex(i);
            }
            
            
            NotifyRemoveTab(tabData, tabIndex);
        }
        
        public void RemoveAllTab()
        {
            while (listTabData.Count > 0)
            {
                RemoveTab(listTabData.Count - 1);
            }
        }

        internal void NotifyRemoveTab(TabData tabData, int tabIndex)
        {
            //Motors
            foreach (var tabMotor in listTabMotor)
            {
                try
                {
                    tabMotor.OnTabViewRemoveTab(tabData, tabIndex);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            
            //Events
            try
            {
                onRemoveTab?.Invoke(tabData, tabIndex);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        #endregion

        #region Add Motor

        public void AddTabMotor(TabMotor tabMotor)
        {
            listTabMotor.Add(tabMotor);
            InitTabMotor(tabMotor);
            SetTabViewForTabMotor(tabMotor);
        }

        public void RemoveTabMotor(int index)
        {
            listTabMotor.RemoveAt(index);
        }

        public void RemoveTabMotor(TabMotor tabMotor)
        {
            listTabMotor.Remove(tabMotor);
        }

        #endregion


        public void SetTabInteractive(TabInteractive tabInteractive)
        {
            this.tabInteractive = tabInteractive;

            if (tabInteractive != null)
            {
                InitTabInteractive(tabInteractive);
                SetTabViewForTabInteractive(tabInteractive);
            }
        }
        
        
        
        
        #if UNITY_EDITOR

        [ContextMenu("Auto Fill List Tab Data")]
        public void Editor_AutoFillListTabData()
        {
            UnityEditor.Undo.RecordObject(this, "Auto Fill List Tab Data");
            listTabData.Clear();

            //Buttons
            Dictionary<string, GameObject> dictButton = new Dictionary<string, GameObject>();
            var allTabButton = GetComponentsInChildren<TabButton>(true);
            foreach (var tabButton in allTabButton)
            {
                if(tabButton.GetComponentInParent<TabView>() != this)
                    continue;
                
                dictButton.Add(tabButton.gameObject.name.ToLower(), tabButton.gameObject);
            }
            
            //Contents
            Dictionary<string, GameObject> dictContent = new Dictionary<string, GameObject>();
            var allTabContent = GetComponentsInChildren<TabContent>(true);
            foreach (var tabContent in allTabContent)
            {
                if(tabContent.GetComponentInParent<TabView>() != this)
                    continue;
                
                dictContent.Add(tabContent.gameObject.name.ToLower(), tabContent.gameObject);
            }
            
            //Add to list
            foreach (var kv in dictContent)
            {
                string id = kv.Key;
                var content = kv.Value;
                dictButton.TryGetValue(id, out var button);
                TabView childTab = content.GetComponentInChildren<TabView>();

                TabData data = new TabData(id, button, content, childTab);
                listTabData.Add(data);
            }
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        #endif
    }
}