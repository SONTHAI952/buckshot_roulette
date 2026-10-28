#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace ZeroX.RxSystem.Editors
{
    public static class ZeroEditorRx
    {
        private static bool isInitialized = false;
        private static object lockObject = new object();
        private static List<Action> listAction;

        private static Thread mainThread;
        
        

        private static void InitIfNot()
        {
            lock (lockObject)
            {
                if (isInitialized)
                    return;
            
                isInitialized = true;
                lockObject = new object();
                listAction = new List<Action>();
                EditorApplication.update += OnEditorApplicationUpdate;
            }
        }


        private static void OnEditorApplicationUpdate()
        {
            UpdateInvokeListActionInMainThread();
        }

        private static void UpdateInvokeListActionInMainThread()
        {
            List<Action> listActionNeedExecute = null;
            lock (lockObject)
            {
                if(listAction.Count == 0)
                    return;
                
                listActionNeedExecute = listAction.ToList();
                listAction.Clear();
            }


            for (int i = 0; i < listActionNeedExecute.Count; i++)
            {
                try
                {
                    listActionNeedExecute[i].Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            
            //Debug.Log("UpdateInvokeListActionInMainThread: " + listActionNeedExecute.Count);
        }


        public static void RunAction(Action action)
        {
            InitIfNot();

            lock (lockObject)
            {
                listAction.Add(action);
            }
        }
    }
}

#endif