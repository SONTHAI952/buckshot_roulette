/*using System;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public abstract class WindowControllerBase<T> : MonoBehaviour where T : WindowControllerBase<T>, new()
    {
        [SerializeField] private Window window;

        public Window Window => window;

        protected virtual string WindowPath => typeof(T).Name;
        
        

        protected void Awake()
        {
            window.WindowEvents.OnOpen.AddListener(Window_OnOpen);
            window.WindowEvents.OnOpened.AddListener(Window_OnOpened);
            window.WindowEvents.OnClose.AddListener(Window_OnClose);
            window.WindowEvents.OnClosed.AddListener(Window_OnClosed);
            
            OnAwake();
        }

        protected void Start()
        {
            OnStart();
        }

        protected virtual void Reset()
        {
            window = GetComponent<Window>();
        }
        
        protected virtual void OnAwake()
        {
            
        }
        
        protected virtual void OnStart()
        {
            
        }

        protected virtual void Window_OnOpen(WindowEventData eventData)
        {
            
        }
        
        protected virtual void Window_OnOpened(WindowEventData eventData)
        {
            
        }
        
        protected virtual void Window_OnClose(WindowEventData eventData)
        {
            
        }
        
        protected virtual void Window_OnClosed(WindowEventData eventData)
        {
            
        }



        public static string GetWindowPath()
        {
            var temp = new T();
            string windowPath = temp.WindowPath;
            Destroy(temp);

            return windowPath;
        }

        public static T GetWindow()
        {
            // string windowPath = GetWindowPath();
            //
            // var w = WindowManager.GetWindow(windowPath);
            // var m = w.GetComponent<T>();
            // return m;
            return null;
        }

        public static T Instance => GetWindow();

        public static T GetRegisteredWindow()
        {
            return null;
            // string windowPath = GetWindowPath();
            // var w = WindowManager.GetRegisteredWindowWithLog(windowPath);
            // if (w == null)
            //     return null;
            //
            // var m = w.GetComponent<T>();
            // return m;
        }
        
        public void SimpleOpen()
        {
            window.Open();
        }
        
        public void SimpleOpenImmediate()
        {
            window.OpenImmediate();
        }
        
        public static T SimpleStaticOpen()
        {
            var m = GetWindow();
            m.SimpleOpen();
            return m;
        }

        public static T SimpleStaticOpenImmediate()
        {
            var m = GetWindow();
            m.SimpleOpenImmediate();
            return m;
        }

        public void SimpleClose()
        {
            window.Close();
        }
        
        public void SimpleCloseImmediate()
        {
            window.CloseImmediate();
        }
        
        public static void SimpleStaticClose()
        {
            var temp = new T();
            string windowPath = temp.WindowPath;
            Destroy(temp);
            
            //WindowManager.CloseWindow(windowPath);
        }
        
        public static void SimpleStaticCloseImmediate()
        {
            var temp = new T();
            string windowPath = temp.WindowPath;
            Destroy(temp);
            
            //WindowManager.CloseWindowImmediate(windowPath);
        }
    }
}*/