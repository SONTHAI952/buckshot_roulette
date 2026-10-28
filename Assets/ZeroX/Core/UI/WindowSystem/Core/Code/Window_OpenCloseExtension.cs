using UnityEngine;

namespace ZeroX.WindowSystem
{
    public static class Window_OpenCloseExtension
    {
        public static void Reverse(this Window window)
        {
            if(window.WindowState == WindowState.Null || window.WindowState == WindowState.Closed || window.WindowState == WindowState.Closing)
                window.Open();
            else if(window.WindowState == WindowState.Opened || window.WindowState == WindowState.Opening)
                window.Close();
            else
                Debug.LogError("Not code for window.WindowState: " + window.WindowState);
        }
        
        public static void Open(this Window window, float duration, float delay, bool fireEvent)
        {
            OpenOption option = new OpenOption(duration, delay, fireEvent);
            window.OpenWithOption(option);
        }
        
        public static void Close(this Window window, float duration, float delay, bool fireEvent)
        {
            CloseOption option = new CloseOption(duration, delay, fireEvent);
            window.CloseWithOption(option);
        }
        
        
        public static void OpenWithParameter(this Window window, string key, object value)
        {
            OpenOption option = window.CreateDefaultOpenOption();
            option.parameters = new WindowParameters();
            option.parameters.Add(key, value);
            window.OpenWithOption(option);
        }
        
        public static void CloseWithParameter(this Window window, string key, object value)
        {
            CloseOption option = window.CreateDefaultCloseOption();
            option.parameters = new WindowParameters();
            option.parameters.Add(key, value);
            window.CloseWithOption(option);
        }
        
        
        public static void OpenWithParameters(this Window window, WindowParameters parameters)
        {
            OpenOption option = window.CreateDefaultOpenOption();
            option.parameters = parameters;
            window.OpenWithOption(option);
        }
        
        public static void CloseWithParameters(this Window window, WindowParameters parameters)
        {
            CloseOption option = window.CreateDefaultCloseOption();
            option.parameters = parameters;
            window.CloseWithOption(option);
        }
        
        
        public static void OpenSilent(this Window window)
        {
            OpenOption option = new OpenOption(window.OpenDuration, 0, false);
            window.OpenWithOption(option);
        }

        public static void CloseSilent(this Window window)
        {
            CloseOption option = new CloseOption(window.CloseDuration, 0, false);
            window.CloseWithOption(option);
        }
        
        
        public static void OpenImmediate(this Window window)
        {
            OpenOption option = new OpenOption(0, 0, true);
            window.OpenWithOption(option);
        }
        
        public static void CloseImmediate(this Window window)
        {
            CloseOption option = new CloseOption(0, 0, true);
            window.CloseWithOption(option);
        }
        
        
        public static void OpenImmediateSilent(this Window window)
        {
            OpenOption option = new OpenOption(0, 0, false);
            window.OpenWithOption(option);
        }
        
        public static void CloseImmediateSilent(this Window window)
        {
            CloseOption option = new CloseOption(0, 0, false);
            window.CloseWithOption(option);
        }
        
        
        public static void OpenDelay(this Window window, float delay)
        {
            OpenOption option = new OpenOption(window.OpenDuration, delay, true);
            window.OpenWithOption(option);
        }
        
        public static void CloseDelay(this Window window, float delay)
        {
            CloseOption option = new CloseOption(window.CloseDuration, delay, true);
            window.CloseWithOption(option);
        }
        
        
        public static void OpenDelaySilent(this Window window, float delay)
        {
            OpenOption option = new OpenOption(window.OpenDuration, delay, false);
            window.OpenWithOption(option);
        }
        
        public static void CloseDelaySilent(this Window window, float delay)
        {
            CloseOption option = new CloseOption(window.CloseDuration, delay, false);
            window.CloseWithOption(option);
        }
        
        
        public static void OpenImmediateDelay(this Window window, float delay)
        {
            OpenOption option = new OpenOption(0, delay, true);
            window.OpenWithOption(option);
        }
        
        public static void CloseImmediateDelay(this Window window, float delay)
        {
            CloseOption option = new CloseOption(0, delay, true);
            window.CloseWithOption(option);
        }
        
        
        public static void OpenImmediateDelaySilent(this Window window, float delay)
        {
            OpenOption option = new OpenOption(0, delay, false);
            window.OpenWithOption(option);
        }
        
        public static void CloseImmediateDelaySilent(this Window window, float delay)
        {
            CloseOption option = new CloseOption(0, delay, false);
            window.CloseWithOption(option);
        }
        
        
        public static void OpenInDuration(this Window window, float duration)
        {
            OpenOption option = new OpenOption(duration, 0f, true);
            window.OpenWithOption(option);
        }
        
        public static void CloseInDuration(this Window window, float duration)
        {
            CloseOption option = new CloseOption(duration, 0f, true);
            window.CloseWithOption(option);
        }
        
        
        public static void OpenInDurationSilent(this Window window, float duration)
        {
            OpenOption option = new OpenOption(duration, 0f, false);
            window.OpenWithOption(option);
        }
        
        public static void CloseInDurationSilent(this Window window, float duration)
        {
            CloseOption option = new CloseOption(duration, 0f, false);
            window.CloseWithOption(option);
        }
    }
}