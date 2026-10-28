using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ZeroX.WindowSystem
{
    public abstract class WindowController<T> : MonoBehaviour where T : WindowController<T>, new()
    {
        [SerializeField] private Window window;
        [SerializeField] protected Button buttonCloseOutside;
        [SerializeField] protected Button buttonClose;
        
        
        //Temp
        private static T instance;
        private bool isRegisteredEvent = false; //Nếu đã từng là singleton thì sẽ được đăng ký event và WhenAwake đã được gọi, vì vậy có nghĩa các hàm event và lifecycle khác cũng nên được gọi cho đầy đủ
        private WindowEventData lastWindowEventData;
        private Button lastButtonCloseClicked;
        
        
        //Property
        public Window Window => window;
        
        public Button ButtonCloseOutside => buttonCloseOutside;
        public Button ButtonClose => buttonClose;
        
        protected WindowEventData LastWindowEventData => lastWindowEventData;
        protected Button LastButtonCloseClicked => lastButtonCloseClicked;

        protected virtual string ShortWindowPath => "";
        protected virtual string FullWindowPath => "";


        
        

        #region Unity Method

        private void Awake()
        {
            CheckAndInitSingleton();

            //Nếu là singleton
            if (instance == this)
            {
                RegisterEvents();
                WhenAwake();
                InvokeEventIfRegisterSlow();
            }
        }
        
        private void OnEnable()
        {
            CheckAndInitSingleton();
            
            if (isRegisteredEvent)
            {
                WhenEnable();
            }
        }
        
        private void Start()
        {
            if (isRegisteredEvent)
            {
                WhenStart();
            }
        }
        
        private void OnDisable()
        {
            if (isRegisteredEvent)
            {
                WhenDisable();
            }
        }
        
        private void OnDestroy()
        {
            //Nếu một window trước đó là singleton, tức nó đã init một số thứ, thì khi Destroy nó nên được OnDestroy để hoàn thiện lifeCycle
            //Vì trong khi nó bị disable thì có thể bị đứa khác lấy mất instance nên phải làm vậy
            if (isRegisteredEvent)
            {
                WhenDestroy();
                UnRegisterEvents();
            }

            if (instance == this)
            {
                instance = null;
            }
        }

        protected virtual void Reset()
        {
            var buttons = GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                string nameLower = button.name.ToLower();
                
                if (buttonClose == null && nameLower == "button close")
                    buttonClose = button;
                
                if(buttonCloseOutside == null && nameLower == "overlay")
                    buttonCloseOutside = button;
            }
        }

        #endregion

        

        #region Singleton - Auto Create
        
        private void CheckAndInitSingleton()
        {
            //Cho phép instance được đăng ký lại nếu instance cũ bị disable và không nằm trong DontDestroyOnLoad
            if (instance == null ||
                instance == this ||
                (instance.isActiveAndEnabled == false &&
                 instance.gameObject.scene.name != "DontDestroyOnLoad"))
            {
                if (instance != null && instance != this)
                {
                    Debug.Log($"Switch WindowController Instance: {instance.name} -> {this.name}");
                }
                
                instance = (T)this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected string GetCurrentWindowPath()
        {
            if(string.IsNullOrEmpty(FullWindowPath) == false)
                return FullWindowPath;
            
            
            if(string.IsNullOrEmpty(ShortWindowPath) == false)
                return "Window/" + ShortWindowPath;


            return "Window/" + typeof(T).Name;
        }

        private static void AutoCreateInstance()
        {
#pragma warning disable
            T temp = new T();
            string prefabPath = temp.GetCurrentWindowPath();
            Destroy(temp);
#pragma warning restore
            
            
            var prefab = Resources.Load<T>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"Load window prefab failed at path: " + prefabPath);
                return;
            }
            
            
            instance = Instantiate(prefab);
        }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    AutoCreateInstance();
                }

                return instance;
            }
        }
        
        public static bool HasInstance => instance != null;

        #endregion


        
        #region Controller Lifecycle Method

        protected virtual void WhenAwake()
        {
            
        }
        
        protected virtual void WhenEnable()
        {
            
        }
        
        protected virtual void  WhenStart()
        {
            
        }
        
        protected virtual void WhenDisable()
        {
            
        }
        
        protected virtual void WhenDestroy()
        {
            
        }

        #endregion
        
        

        #region Internal - Window Events
        
        private void RegisterEvents()
        {
            isRegisteredEvent = true;
            
            
            //Window Events
            window.WindowEvents.OnOpen.AddListener(Internal_Window_OnOpen);
            window.WindowEvents.OnOpened.AddListener(Internal_Window_OnOpened);
            window.WindowEvents.OnClose.AddListener(Internal_Window_OnClose);
            window.WindowEvents.OnClosed.AddListener(Internal_Window_OnClosed);
            
            
            //Button Events
            if(buttonClose != null)
                buttonClose.onClick.AddListener(Internal_ButtonClose_OnClick);
            
            if(buttonCloseOutside != null)
                buttonCloseOutside.onClick.AddListener(Internal_ButtonCloseOutside_OnClick);
        }
        
        private void UnRegisterEvents()
        {
            isRegisteredEvent = false;

            
            //Window Events
            if (window != null)
            {
                window.WindowEvents.OnOpen.RemoveListener(Internal_Window_OnOpen);
                window.WindowEvents.OnOpened.RemoveListener(Internal_Window_OnOpened);
                window.WindowEvents.OnClose.RemoveListener(Internal_Window_OnClose);
                window.WindowEvents.OnClosed.RemoveListener(Internal_Window_OnClosed);
            }


            //Button Events
            if (buttonClose != null)
                buttonClose.onClick.RemoveListener(Internal_ButtonClose_OnClick);
            
            if (buttonCloseOutside != null)
                buttonCloseOutside.onClick.RemoveListener(Internal_ButtonCloseOutside_OnClick);
        }

        private void InvokeEventIfRegisterSlow()
        {
            if (window.IsMissOpenEvent())
            {
                Internal_Window_OnOpen(new WindowEventData(window, window.LastOpenOption.parameters));
                Debug.Log("Invoke slow: OnOpen");
            }

            if (window.IsMissOpenedEvent())
            {
                Internal_Window_OnOpened(new WindowEventData(window, window.LastOpenOption.parameters));
                Debug.Log("Invoke slow: OnOpened");
            }
        }

        private void Internal_Window_OnOpen(WindowEventData eventData)
        {
            lastWindowEventData = eventData;
            Window_OnOpen();
        }
        
        private void Internal_Window_OnOpened(WindowEventData eventData)
        {
            lastWindowEventData = eventData;
            Window_OnOpened();
        }
        
        private void Internal_Window_OnClose(WindowEventData eventData)
        {
            lastWindowEventData = eventData;
            Window_OnClose();
        }
        
        private void Internal_Window_OnClosed(WindowEventData eventData)
        {
            lastWindowEventData = eventData;
            Window_OnClosed();
        }

        #endregion
        
        
        
        #region Window Events
        
        protected virtual void Window_OnOpen()
        {
            
        }
        
        protected virtual void Window_OnOpened()
        {
            
        }
        
        protected virtual void Window_OnClose()
        {
            
        }
        
        protected virtual void Window_OnClosed()
        {
            
        }

        #endregion


        
        #region Button Events

        private void Internal_ButtonClose_OnClick()
        {
            lastButtonCloseClicked = buttonClose;
            ButtonClose_OnClick();
        }
        
        private void Internal_ButtonCloseOutside_OnClick()
        {
            lastButtonCloseClicked = buttonCloseOutside;
            ButtonClose_OnClick();
        }

        protected virtual void ButtonClose_OnClick()
        {
            Close();
        }

        #endregion



        #region Public API

        public virtual void Open()
        {
            Window.Open();
        }

        public void Close()
        {
            Window.Close();
        }

        #endregion
    }
}