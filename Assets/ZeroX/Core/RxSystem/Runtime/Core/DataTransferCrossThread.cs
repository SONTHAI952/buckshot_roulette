using System.Collections.Generic;
using System.Threading;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ZeroX.RxSystem
{ 
    public class DataTransferCrossThread
    {
        private enum WorkerFlag
        {
            Stopped, Stopping, Running
        }
        
        
        
        //Other Thread
        public static readonly object objLock_OtherThread = new object();
        public static readonly List<Action> listAction_OtherThread = new List<Action>();
        public static readonly List<DelayAction> listDelayAction_OtherThread = new List<DelayAction>();
        
        //Main Thread
        public static readonly object objLock_MainThread = new object();
        public static readonly List<Action> listAction_MainThread = new List<Action>();
        public static readonly List<DelayAction> listDelayAction_MainThread = new List<DelayAction>();


        //Task
        private static Task workerTask;
        private static WorkerFlag workerFlag = WorkerFlag.Stopped;


        public static void StartWork()
        {
            if (workerFlag != WorkerFlag.Stopped)
            {
                Debug.LogError("Cannot StartWork because it is not stopped");
                return;
            }


            workerFlag = WorkerFlag.Running;
            workerTask = new Task(Worker);
            workerTask.Start();
        }

        public static void StopWork()
        {
            if (workerFlag == WorkerFlag.Running)
            {
                workerFlag = WorkerFlag.Stopping;
            }
        }

        private static void Worker()
        {
            while (true)
            {
                Thread.Sleep(16);

                if (workerFlag == WorkerFlag.Stopping)
                {
                    workerFlag = WorkerFlag.Stopped;
                    return;
                }

                CheckAndTransfer();
            }
        }

        private static void CheckAndTransfer()
        {
            //Chuyển dữ liệu từ list other thread sang list temp
            List<Action> listAction_Temp = null;
            List<DelayAction> listDelayAction_Temp = null;

            lock (objLock_OtherThread)
            {
                if (listAction_OtherThread.Count > 0)
                {
                    listAction_Temp = new List<Action>();
                    listAction_Temp.AddRange(listAction_OtherThread);
                    listAction_OtherThread.Clear();
                }

                if (listDelayAction_OtherThread.Count > 0)
                {
                    listDelayAction_Temp = new List<DelayAction>();
                    listDelayAction_Temp.AddRange(listDelayAction_OtherThread);
                    listDelayAction_OtherThread.Clear();
                }
            }
            
            if(listAction_Temp == null && listDelayAction_Temp == null)
                return;



            
            
            //Chuyển dữ liệu từ listTemp sang list main thread
            lock (objLock_MainThread)
            {
                if (listAction_Temp != null)
                {
                    listAction_MainThread.AddRange(listAction_Temp);
                }

                if (listDelayAction_Temp != null)
                {
                    listDelayAction_MainThread.AddRange(listDelayAction_Temp);
                }
            }
        }





        #region Add Action

        public static void AddActionFromOtherThread(Action action)
        {
            lock (objLock_OtherThread)
            {
                listAction_OtherThread.Add(action);
            }
        }
        
        public static void AddDelayActionFromOtherThread(DelayAction delayAction)
        {
            lock (objLock_OtherThread)
            {
                listDelayAction_OtherThread.Add(delayAction);
            }
        }
        
        public static void AddDelayActionFromMainThread(DelayAction delayAction)
        {
            lock (objLock_MainThread)
            {
                listDelayAction_MainThread.Add(delayAction);
            }
        }

        #endregion
    }
}