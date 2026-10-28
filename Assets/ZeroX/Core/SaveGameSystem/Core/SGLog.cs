using System;
using UnityEngine;

namespace ZeroX.SaveGameSystem
{
    public static class SGLog
    {
        public static void Log(string message)
        {
            if(ZeroRx.IsMainThread)
                Debug.Log(message);
            else
                ZeroRx.RunAction(() => Debug.Log(message));
        }
        
        public static void LogError(string message)
        {
            if(ZeroRx.IsMainThread)
                Debug.LogError(message);
            else
                ZeroRx.RunAction(() => Debug.LogError(message));
        }
        
        public static void LogException(Exception exception)
        {
            if(ZeroRx.IsMainThread)
                Debug.LogException(exception);
            else
                ZeroRx.RunAction(() => Debug.LogException(exception));
        }
    }
}