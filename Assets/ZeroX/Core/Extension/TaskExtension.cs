using System;
using UnityEngine;
using System.Threading.Tasks;
    
namespace ZeroX.Extensions
{
    public static class TaskExtension
    {
        public static WaitToken CreateWaitToken(this Task task)
        {
            WaitToken waitToken = new WaitToken();
            task.ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    waitToken.SetResult();
                    return;
                }


                if (t.IsFaulted)
                {
                    waitToken.SetFault(t.Exception);
                    return;
                }

                if (t.IsCanceled)
                {
                    waitToken.Cancel();
                    return;
                }
                
                Debug.LogError("Not code for task case");
                waitToken.SetFault(new Exception("Not code for task case"));
            });

            return waitToken;
        }

        public static WaitToken<T> CreateWaitToken<T>(this Task<T> task)
        {
            WaitToken<T> waitToken = new WaitToken<T>();
            task.ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    waitToken.SetResult(t.Result);
                    return;
                }


                if (t.IsFaulted)
                {
                    waitToken.SetFault(t.Exception);
                    return;
                }

                if (t.IsCanceled)
                {
                    waitToken.Cancel();
                    return;
                }
                
                Debug.LogError("Not code for task case");
                waitToken.SetFault(new Exception("Not code for task case"));
            });

            return waitToken;
        }
    }
}