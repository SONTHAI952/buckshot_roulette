using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroX.Extensions
{
    public static class UnityActionExtension
    {
        public static void TryInvoke<T>(this UnityAction<T> action, T arg0)
        {
            var list = action.GetInvocationList();
            foreach (var a in list)
            {
                try
                {
                    a.DynamicInvoke(arg0);
                }
                catch (Exception e)
                {
                    if (e is TargetInvocationException)
                    {
                        TargetInvocationException e2 = (TargetInvocationException) e;
                        if(e2.InnerException != null)
                            Debug.LogError(e2.InnerException);
                        else
                            Debug.LogError(e);
                    }
                    else
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }
    }
}