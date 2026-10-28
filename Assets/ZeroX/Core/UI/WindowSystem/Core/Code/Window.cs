using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZeroX.UI;

namespace ZeroX.WindowSystem
{
    [DefaultExecutionOrder(30000)]
    public partial class Window : MonoBehaviour
    {
        //Transition
        [SerializeField] private WindowTransitionConfigMode transitionConfigMode = WindowTransitionConfigMode.Global;
        [SerializeField] private GlobalWindowTransitionId globalTransitionId = GlobalWindowTransitionId.Default;
        [SerializeField] private WindowTransitionConfig transitionConfig = WindowTransitionConfig.CreateDefault();
        
        //Ref
        [SerializeField] private Transform content;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Canvas canvas;
        
        
        //Sorting Config
        [SerializeField] private WindowSortingConfig sortingConfig = new WindowSortingConfig();
        
        //Misc
        [SerializeField] protected ActionTypeWhenClosed actionTypeWhenClosed = ActionTypeWhenClosed.Deactive;
        [SerializeField] protected bool useGuiCamera = true;
        [SerializeField] private FirstAction firstAction = new FirstAction();
        [SerializeField] private List<Window> listChildWindow = new List<Window>();
        [SerializeField] private WindowEvents windowEvents = new WindowEvents();
        [NonSerialized] private readonly WindowEventsOnce windowEventsOnce = new WindowEventsOnce();
        
        
        //Temp
        [NonSerialized] private WindowState windowState = WindowState.Null;
        [NonSerialized] private WindowState preWindowState = WindowState.Null;
        [NonSerialized] private OpenOption lastOpenOption = new OpenOption();
        [NonSerialized] private CloseOption lastCloseOption = new CloseOption();
        private bool isTryGetCanvas = false;
        private bool wasOpenEventFired = false;
        private bool wasOpenedEventFired = false;
        
        
        //Wait Token
        private WaitToken wtWindowOpen;
        private WaitToken wtWindowOpened;
        private WaitToken wtWindowClose;
        private WaitToken wtWindowClosed;


        //Dùng để làm key cho WindowAnim, ví dụ key cho DOTween
        public readonly object AnimObjectKey = new object();
        

        
        //Property
        #region Property

        public float OpenDuration
        {
            get
            {
                if (transitionConfigMode == WindowTransitionConfigMode.Global)
                    return GlobalWindowTransitionTable.GetConfig(globalTransitionId).openDuration;
                else
                    return transitionConfig.openDuration;
            }
        }
        
        public float CloseDuration
        {
            get
            {
                if (transitionConfigMode == WindowTransitionConfigMode.Global)
                    return GlobalWindowTransitionTable.GetConfig(globalTransitionId).closeDuration;
                else
                    return transitionConfig.openDuration;
            }
        }
        
        public bool IgnoreTimeScale
        {
            get
            {
                if (transitionConfigMode == WindowTransitionConfigMode.Global)
                    return GlobalWindowTransitionTable.GetConfig(globalTransitionId).ignoreTimeScale;
                else
                    return transitionConfig.ignoreTimeScale;
            }
        }

        public IWindowAnim WindowAnim
        {
            get
            {
                if (transitionConfigMode == WindowTransitionConfigMode.Global)
                    return GlobalWindowTransitionTable.GetConfig(globalTransitionId).Anim;
                else
                    return transitionConfig.Anim;
            }
        }

        //Ref
        public Canvas Canvas => canvas;
        public CanvasGroup CanvasGroup => canvasGroup;
        public Transform Content => content;
        
        //Misc
        public ActionTypeWhenClosed ActionTypeWhenClosed => actionTypeWhenClosed;
        public bool UseGuiCamera => useGuiCamera;
        public FirstAction FirstAction => firstAction;
        public WindowEvents WindowEvents => windowEvents;
        public WindowEventsOnce WindowEventsOnce => windowEventsOnce;

        public WindowState WindowState
        {
            get => windowState;
            protected set => windowState = value;
        }

        public WindowState PreWindowState
        {
            get => preWindowState;
            protected set => preWindowState = value;
        }

        public OpenOption LastOpenOption => lastOpenOption;
        public CloseOption LastCloseOption => lastCloseOption;

        #endregion
        
        
        
        
        

        #region Unity Method

        protected virtual void Awake()
        {
            if (canvas == null && isTryGetCanvas == false)
            {
                isTryGetCanvas = true;
                canvas = GetComponent<Canvas>();
            }
                
            
            if(canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            
            
            Runner_SetGuiCameraForCanvas();
            
            
            if (windowState == WindowState.Null && firstAction.when == FirstAction.WhenType.Awake)
            {
                ExecuteFirstAction();
            }
        }

        protected virtual void OnEnable()
        {
            if (windowState == WindowState.Null && firstAction.when == FirstAction.WhenType.FirstEnable)
            {
                ExecuteFirstAction();
            }
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void Start()
        {
            if (windowState == WindowState.Null && firstAction.when == FirstAction.WhenType.Start)
            {
                ExecuteFirstAction();
            }
        }

        protected virtual void Reset()
        {
            content = transform.Find("Content");
            if(content == null)
                content = transform.Find("content");
            
            canvas = GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();

            if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
                useGuiCamera = true;
        }

        #endregion

        
        
        #region Window State

        public bool IsOpening => windowState == WindowState.Opening;
        public bool IsOpened => windowState == WindowState.Opened;
        public bool IsClosing => windowState == WindowState.Closing;
        public bool IsClosed => windowState == WindowState.Closed || windowState == WindowState.Null;
        public bool IsOpeningOrOpened => windowState == WindowState.Opening || windowState == WindowState.Opened;
        public bool IsClosingOrClosed => windowState == WindowState.Closing || windowState == WindowState.Closed || windowState == WindowState.Null;

        #endregion



        #region Utility

        void ExecuteFirstAction()
        {
            switch (firstAction.actionType)
            {
                case FirstAction.ActionType.Open:
                {
                    //CloseInternal(0f, 0f, false);
                    OpenOption option = new OpenOption(OpenDuration, firstAction.delay, firstAction.fireEvent);
                    OpenWithOption(option);
                    break;
                }
                case FirstAction.ActionType.OpenImmediate:
                {
                    //CloseInternal(0f, 0f, false);
                    OpenOption option = new OpenOption(0f, firstAction.delay, firstAction.fireEvent);
                    OpenWithOption(option);
                    break;
                }
                case FirstAction.ActionType.Close:
                {
                    //OpenInternal(0f, 0f, false);
                    CloseOption option = new CloseOption(CloseDuration, firstAction.delay, firstAction.fireEvent);
                    CloseWithOption(option);
                    break;
                }
                case FirstAction.ActionType.CloseImmediate:
                {
                    //OpenInternal(0f, 0f, false);
                    CloseOption option = new CloseOption(0f, firstAction.delay, firstAction.fireEvent);
                    CloseWithOption(option);
                    break;
                }
                default:
                {
                    Debug.LogError("Not code for FirstAction.ActionType: " + firstAction.actionType);
                    break;
                }
            }
        }
        
        public OpenOption CreateDefaultOpenOption()
        {
            OpenOption option = new OpenOption(OpenDuration, 0, true);
            return option;
        }

        public CloseOption CreateDefaultCloseOption()
        {
            CloseOption option = new CloseOption(CloseDuration, 0, true);
            return option;
        }

        protected virtual GameObject GetWindowObject()
        {
            return gameObject;
        }

        #endregion
        


        #region Run Window Anim

        private void RunWindowAnim_Open(float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            if(WindowAnim != null)
                WindowAnim.Open(this, duration, ignoreTimeScale, parameters, onFinished);
            else
                onFinished.Invoke();
        }

        private void RunWindowAnim_OpenImmediate(WindowParameters parameters)
        {
            if(WindowAnim != null)
                WindowAnim.OpenImmediate(this, parameters);
        }
        
        private void RunWindowAnim_Close(float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            if(WindowAnim != null)
                WindowAnim.Close(this, duration, ignoreTimeScale, parameters, onFinished);
            else
                onFinished.Invoke();
        }

        private void RunWindowAnim_CloseImmediate(WindowParameters parameters)
        {
            if(WindowAnim != null)
                WindowAnim.CloseImmediate(this, parameters);
        }

        #endregion
        
        

        #region Fire Event

        public void FireEvent_OnOpen(WindowEventData eventData)
        {
            wasOpenEventFired = true;
            
            
            //Window Event
            try
            {
                windowEvents.OnOpen.Invoke(eventData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            
            
            //Window Event Once
            try
            {
                windowEventsOnce.OnOpen?.Invoke(eventData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            windowEventsOnce.OnOpen = null;
            
            
            
            //Wait Token
            wtWindowOpen?.SetResult();
            wtWindowOpen = null;
            
            
            
            //Childs
            foreach (var childWindow in listChildWindow)
            {
                if (childWindow == this)
                {
                    Debug.LogError("Don't drag itself into the listChildWindow");
                    continue;
                }
                
                if(childWindow.IsOpened == false)
                    continue;
                
                childWindow.FireEvent_OnOpen(eventData);
            }
        }

        public void FireEvent_OnOpened(WindowEventData eventData)
        {
            wasOpenedEventFired = true;
            
            
            //Window Event
            try
            {
                windowEvents.OnOpened.Invoke(eventData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            
            
            //Window Event Once
            try
            {
                windowEventsOnce.OnOpened?.Invoke(eventData);
               
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            windowEventsOnce.OnOpened = null;
            
            
            
            //Wait Token
            wtWindowOpened?.SetResult();
            wtWindowOpened = null;
            
            
            
            //Childs
            foreach (var childWindow in listChildWindow)
            {
                if (childWindow == this)
                {
                    Debug.LogError("Don't drag itself into the listChildWindow");
                    continue;
                }
                
                if(childWindow.IsOpened == false)
                    continue;
                
                childWindow.FireEvent_OnOpened(eventData);
            }
        }
        
        public void FireEvent_OnClose(WindowEventData eventData)
        {
            //Window Event
            try
            {
                windowEvents.OnClose.Invoke(eventData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            
            
            //Window Event Once
            try
            {
                windowEventsOnce.OnClose?.Invoke(eventData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            windowEventsOnce.OnClose = null;
            
            
            
            //Wait Token
            wtWindowClose?.SetResult();
            wtWindowClose = null;
            
            
            
            //Childs
            foreach (var childWindow in listChildWindow)
            {
                if (childWindow == this)
                {
                    Debug.LogError("Don't drag itself into the listChildWindow");
                    continue;
                }

                if (childWindow.IsOpening)
                {
                    Debug.LogError("Forcing a child window to fire an OnClose event while it is open is unreasonable. And you should not close the parent window while the child window is opening");
                }
                
                if(childWindow.IsOpened == false)
                    continue;
                
                childWindow.FireEvent_OnClose(eventData);
            }
        }
        
        public void FireEvent_OnClosed(WindowEventData eventData)
        {
            //Window Event
            try
            {
                windowEvents.OnClosed.Invoke(eventData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            
            
            //Window Event Once
            try
            {
                windowEventsOnce.OnClosed?.Invoke(eventData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            windowEventsOnce.OnClosed = null;
            
            
            
            //Wait Token
            wtWindowClosed?.SetResult();
            wtWindowClosed = null;
            
            
            
            //Childs
            foreach (var childWindow in listChildWindow)
            {
                if (childWindow == this)
                {
                    Debug.LogError("Don't drag itself into the listChildWindow");
                    continue;
                }
                
                if (childWindow.IsOpening)
                {
                    Debug.LogError("Forcing a child window to fire an OnClosed event while it is open is unreasonable. And you should not close the parent window while the child window is opening");
                }
                
                if(childWindow.IsOpened == false)
                    continue;
                
                childWindow.FireEvent_OnClosed(eventData);
            }
        }
        
        #endregion



        #region Check Miss Event

        public bool IsMissOpenEvent()
        {
            return (windowState == WindowState.Opening || windowState == WindowState.Opened) && wasOpenEventFired;
        }
        
        public bool IsMissOpenedEvent()
        {
            return windowState == WindowState.Opened && wasOpenedEventFired;
        }

        #endregion
        
        
        


        
        #region Handle GUI Camera For Canvas

        private bool TrySetGuiCameraForCanvas()
        {
            if (GUICamera.Instance == null || GUICamera.Instance.Camera == null)
                return false;

            canvas.worldCamera = GUICamera.Instance.Camera;
            return true;
        }
        
        private void Runner_SetGuiCameraForCanvas()
        {
            if(useGuiCamera == false)
                return;
            
            if (TrySetGuiCameraForCanvas() == false)
            {
                StartCoroutine(Timeline());
            }


            IEnumerator Timeline()
            {
                yield return null;
                if(TrySetGuiCameraForCanvas())
                    yield break;
                
                yield return null;
                if(TrySetGuiCameraForCanvas())
                    yield break;
                
                Debug.LogError("Cannot set GUICamera for canvas because GUICamera not exist!", gameObject);
            }
        }

        #endregion



        #region Open - Close Helper

        private void HandleWhenOpened(OpenOption option)
        {
            preWindowState = windowState;
            windowState = WindowState.Opened;
            SetCanvasGroupInteractable(true);
                
            if (option.fireEvent)
                FireEvent_OnOpened(new WindowEventData(this, option.parameters));
        }
        
        private void HandleWhenClosed(GameObject windowObject, CloseOption option)
        {
            SetCanvasGroupInteractable(true);
            preWindowState = windowState;
            windowState = WindowState.Closed;
            UnApplyWindowSorting_WhenClosed();
                    
            ExecuteActionWhenClosed(windowObject);

            if(option.fireEvent)
                FireEvent_OnClosed(new WindowEventData(this, option.parameters));
        }

        //Đảo ngược actionWhenClosed để mở window
        private void RevertActionWhenClosedToOpen(GameObject windowObject)
        {
            switch (actionTypeWhenClosed)
            {
                case ActionTypeWhenClosed.None:
                {
                    break;
                }
                case ActionTypeWhenClosed.Deactive:
                {
                    if (canvas != null && canvas.enabled == false)
                        canvas.enabled = true;
                    
                    windowObject.SetActive(true);
                    break;
                }
                case ActionTypeWhenClosed.TurnOffCanvas:
                {
                    canvas.enabled = true;
                    SetEnableGraphicRaycaster(true);
                    
                    if(windowObject.activeSelf == false)
                        windowObject.SetActive(true);
                    
                    break;
                }
                default:
                {
                    Debug.LogError("Not code for actionWhenClosed: " + actionTypeWhenClosed);
                    break;
                }
            }
        }
        
        /// <summary>
        /// Khi window đã close thì nên làm gì (DeActive/Invisible)
        /// </summary>
        private void ExecuteActionWhenClosed(GameObject windowObject)
        {
            switch (actionTypeWhenClosed)
            {
                case ActionTypeWhenClosed.None:
                {
                    break;
                }
                case ActionTypeWhenClosed.Deactive:
                {
                    windowObject.SetActive(false);
                    break;
                }
                case ActionTypeWhenClosed.TurnOffCanvas:
                {
                    canvas.enabled = false;
                    SetEnableGraphicRaycaster(false);

                    break;
                }
                default:
                {
                    Debug.LogError("Not code for actionWhenClosed: " + actionTypeWhenClosed);
                    break;
                }
            }
        }

        private void SetEnableGraphicRaycaster(bool enable)
        {
            if(canvas == null)
                return;
            
            var graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            if (graphicRaycaster != null)
                graphicRaycaster.enabled = enable;
        }

        private void SetCanvasGroupInteractable(bool interactable)
        {
            if(canvasGroup == null)
                return;
            
            canvasGroup.interactable = interactable;
        }
        
        #endregion
        

        
        #region Open - Close
        
        /// <summary>
        /// Duration == 0 is immediate
        /// </summary>
        public virtual void OpenWithOption(OpenOption option)
        {
            //Delay
            if (Mathf.Approximately(option.delay, 0) == false && option.delay > 0)
            {
                ZeroRx.RunActionDelay(() =>
                {
                    var optionNoDelay = option.Clone();
                    optionNoDelay.delay = 0;
                    OpenWithOption(optionNoDelay);
                }, option.delay, IgnoreTimeScale);
                
                return;
            }
            
            
            
            
            //Setup
            if (windowState == WindowState.Null) //Cần reset về trạng thái closed để bắt đầu mở
                RunWindowAnim_CloseImmediate(option.parameters);
                
            if (windowState == WindowState.Opening || windowState == WindowState.Opened)
                return;

            
            lastOpenOption = option;
            var windowObject = GetWindowObject();
            preWindowState = windowState;
            windowState = WindowState.Opening;
            RevertActionWhenClosedToOpen(windowObject);
            ApplyWindowSorting();



            //Anim - Event
            if (option.IsZeroDuration() == false && windowObject.activeInHierarchy)
            {
                SetCanvasGroupInteractable(false); //Trong lúc đang mở thì chặn tương tác với UI. Tuy nhiên UI vẫn blocking UI raycast
                
                
                bool isEventFired = false; //Để tránh RunWindowAnim_Open gọi OnFinished ngay lập tức, dẫn đến OnOpen đc bắn sau OnOpened
                RunWindowAnim_Open(option.duration, IgnoreTimeScale, option.parameters, () =>
                {
                    if (isEventFired == false && option.fireEvent)
                    {
                        isEventFired = true;
                        FireEvent_OnOpen(new WindowEventData(this, option.parameters));
                    }
                    
                    HandleWhenOpened(option);
                });

                
                
                if (isEventFired == false && option.fireEvent)
                {
                    isEventFired = true;
                    FireEvent_OnOpen(new WindowEventData(this, option.parameters));
                }
            }
            else //Immediate
            {
                RunWindowAnim_OpenImmediate(option.parameters);
                
                if(option.fireEvent)
                    FireEvent_OnOpen(new WindowEventData(this, option.parameters));
                
                HandleWhenOpened(option);
            }
        }

        
        
        /// <summary>
        /// Duration == 0 is immediate
        /// </summary>
        public virtual void CloseWithOption(CloseOption option)
        {
            //Delay
            if (Mathf.Approximately(option.delay, 0) == false && option.delay > 0)
            {
                ZeroRx.RunActionDelay(() => 
                {
                    var optionNoDelay = option.Clone();
                    optionNoDelay.delay = 0;
                    CloseWithOption(optionNoDelay);
                }, option.delay, IgnoreTimeScale);
                
                return;
            }
            
            
            
            
            //Setup
            if (windowState == WindowState.Null) //Cần reset về trạng thái opened để bắt đầu mở
                RunWindowAnim_OpenImmediate(option.parameters);
                
            if (windowState == WindowState.Closing || windowState == WindowState.Closed)
                return;
                
            
            lastCloseOption = option;
            var windowObject = GetWindowObject();
            preWindowState = windowState;
            windowState = WindowState.Closing;
            UnApplyWindowSorting_WhenClosing();
                
            
            
            //Anim - Event
            if (option.IsZeroDuration() == false && windowObject.activeInHierarchy)
            {
                SetCanvasGroupInteractable(false);
                
                
                bool isEventFired = false; //Để tránh RunWindowAnim_Close gọi OnFinished ngay lập tức, dẫn đến OnClose đc bắn sau OnClosed
                RunWindowAnim_Close(option.duration, IgnoreTimeScale, option.parameters, () =>
                {
                    if (isEventFired == false && option.fireEvent)
                    {
                        isEventFired = true;
                        FireEvent_OnClose(new WindowEventData(this, option.parameters));
                    }
                    
                    HandleWhenClosed(windowObject, option);
                });

                
                
                if (isEventFired == false && option.fireEvent)
                {
                    isEventFired = true;
                    FireEvent_OnClose(new WindowEventData(this, option.parameters));
                }
            }
            else
            {
                RunWindowAnim_CloseImmediate(option.parameters);
                
                if(option.fireEvent)
                    FireEvent_OnClose(new WindowEventData(this, option.parameters));
                
                HandleWhenClosed(windowObject, option);
            }
        }

        
        
        public void Open()
        {
            OpenOption option = CreateDefaultOpenOption();
            OpenWithOption(option);
        }

        public void Close()
        {
            CloseOption option = CreateDefaultCloseOption();
            CloseWithOption(option);
        }
        
        #endregion
        
        
        
        #region Sorting
        
        private void ApplyWindowSorting()
        {
            if(sortingConfig.UseWindowSorting == false)
                return;
            
            if (isTryGetCanvas == false)
            {
                isTryGetCanvas = true;
                
                if(canvas == null)
                    canvas = GetComponent<Canvas>();
            }
            
            WindowSortingManager.Apply(sortingConfig.WindowSortingLayerId, canvas, canvasGroup, sortingConfig.VisibleWhenBehind, OpenDuration);
        }
        
        private void UnApplyWindowSorting_WhenClosing()
        {
            if(canvas == null)
                return;
            
            WindowSortingManager.UnApply_WhenClosing(sortingConfig.WindowSortingLayerId, canvas);
        }
        
        private void UnApplyWindowSorting_WhenClosed()
        {
            if(canvas == null)
                return;
            
            WindowSortingManager.UnApply_WhenClosed(sortingConfig.WindowSortingLayerId, canvas);
        }
        
        #endregion
    }
}