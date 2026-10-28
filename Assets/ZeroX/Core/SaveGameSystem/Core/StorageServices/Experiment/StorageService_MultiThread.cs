using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ZeroX.JsonSystem;
using ZeroX.RxSystem;

namespace ZeroX.SaveGameSystem.StorageServices
{
    public abstract class StorageService_MultiThread : StorageService
    {
        //Service - Fields
        private StorageServiceState storageServiceState = StorageServiceState.Closed;
        protected bool databaseChanged = false;
        protected Database database;
        protected readonly HashSet<SGNode> hashSetNodeChanged = new HashSet<SGNode>();
        

        
        //Thread - Fields
        private bool continueThreadUpdate = true; //flag
        private Task threadUpdate;
        
        public bool IsThreadUpdateFinished => threadUpdate.IsCompleted;
        public bool IsThreadUpdateFaulted => threadUpdate.IsFaulted;
        
        protected readonly object lockObj_SaveDatabaseToStorage = new object();
        protected readonly object lockObj_SaveNodeToStorage = new object();
        

        //Thread - Time Config
        private const ulong updatePerMillisecond = 20;
        private const ulong maxMillisecondIgnoreSave = 100;
        private ulong currentMillisecond = 0;
        private ulong lastTimeSetChanges = 0;
        private ulong lastTimeSaveAllChangesToStorage = 0;
        
        //Thread - Process
        private bool isSavingAllChangesToStorage = false;
        
        
        
        //Event
        readonly Disposables disposables = new Disposables();
        
        
        
        
        
        
        #region Implement


        #region Open Close

        public sealed override void OpenService(Database database)
        {
            this.database = database;
            storageServiceState = StorageServiceState.Opened;

            
            
            try
            {
                ExecuteOpenService();
            }
            catch (Exception e)
            {
                storageServiceState = StorageServiceState.Closed;
                SGLog.LogException(e);
                
                return;
            }
            
            
            
            //Thread
            StartThreadUpdate();
            
            ZeroRx.RunAction(() =>
            {
                ZeroRx.onApplicationPause.Subscribe(OnApplicationPause).AddTo(disposables);
                ZeroRx.onApplicationQuit.Subscribe(OnApplicationQuit).AddTo(disposables);
            });
        }
        
        protected abstract void ExecuteOpenService();
        
        
        
        public sealed override void CloseService()
        {
            if(storageServiceState == StorageServiceState.Closed)
                return;
            
            storageServiceState = StorageServiceState.Closed;
            
            disposables.Dispose();
            StopThreadUpdate();
            SaveAllChangesToStorage();


            try
            {
                ExecuteCloseService();
            }
            catch (Exception e)
            {
                SGLog.LogException(e);
            }
            
            // while (IsTaskUpdateToDiskFinished == false)
            // {
            //     Thread.Sleep(6);
            //     //Đợi Thread khác hoàn thành việc lưu hết changed rồi mới cho đi tiếp
            // }
            
            if(IsThreadUpdateFaulted)
                SGLog.LogException(threadUpdate.Exception);
        }
        
        protected abstract void ExecuteCloseService();

        #endregion
        
        

        #region Load

        public sealed override ZJsonObject LoadDatabaseDataJson()
        {
            lock (lockObj_SaveDatabaseToStorage)
            {
                return ExecuteLoadDatabaseDataJson();
            }
        }

        /// <summary>
        /// No need lock lockObj_SaveDatabaseToStorage because was locked in base
        /// </summary>
        protected abstract ZJsonObject ExecuteLoadDatabaseDataJson();

        

        
        
        public sealed override ZJsonObject LoadNodeDataJson(long nodeId)
        {
            lock (lockObj_SaveNodeToStorage)
            {
                return ExecuteLoadNodeDataJson(nodeId);
            }
        }

        public sealed override ZJsonObject LoadNodeDataJsonNoLock(long nodeId)
        {
            return ExecuteLoadNodeDataJson(nodeId);
        }

        /// <summary>
        /// No need lock because was locked in base
        /// </summary>
        protected abstract ZJsonObject ExecuteLoadNodeDataJson(long nodeId);

        #endregion
        
        
        
        public sealed override void SetDatabaseChanged()
        {
            lock (database)
            {
                databaseChanged = true;
                lastTimeSetChanges = currentMillisecond;
            }
        }

        public sealed override void SetNodeChanged(SGNode node)
        {
            lock (database)
            {
                if (node.HasNodeId == false)
                    throw new Exception("Không thể set changed cho node không có nodeId");

                hashSetNodeChanged.Add(node);
                lastTimeSetChanges = currentMillisecond;
            }
        }
        
        
        
        public sealed override void SaveAllChangesToStorage()
        {
            SaveDatabaseToStorage();
            SaveNodesToStorage();
        }

        public sealed override bool HasRunningProcess()
        {
            return isSavingAllChangesToStorage;
        }

        public sealed override bool HasChangesNeedWriteToStorage()
        {
            lock (database)
            {
                return databaseChanged || hashSetNodeChanged.Count > 0;
            }
        }

        #endregion

        

        #region Events

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveAllChangesToStorage();
            }
        }

        private void OnApplicationQuit()
        {
            //SaveAllChangesToStorage();
        }

        #endregion



        #region Thread Update

        public void StartThreadUpdate()
        {
            continueThreadUpdate = true;
            threadUpdate = new Task(ThreadUpdate);
            threadUpdate.ContinueWith((t) =>
            {
                if (t.IsFaulted && storageServiceState != StorageServiceState.Closed)
                {
                    SGLog.LogException(t.Exception);
                    ZeroRx.RunAction(() =>
                    {
                        Debug.Log("Thread Update To Disk error -> restart...");
                        StartThreadUpdate();
                        Debug.Log("Thread Update To Disk restart success");
                    });
                }
            });
            threadUpdate.Start();
        }
        
        public void StopThreadUpdate()
        {
            continueThreadUpdate = false;
        }
        
        void ThreadUpdate()
        {
            while (continueThreadUpdate)
            {
                Thread.Sleep((int)updatePerMillisecond);
                currentMillisecond += updatePerMillisecond;
                
                
                
                
                
                //Nếu đang set change liên tục thì nên từ từ rồi save
                //Cơ mà nếu đợi quá lâu rồi thì nên save luôn
                if (currentMillisecond - lastTimeSetChanges > updatePerMillisecond ||
                    currentMillisecond - lastTimeSaveAllChangesToStorage > maxMillisecondIgnoreSave)
                {
                    isSavingAllChangesToStorage = true;


                    try
                    {
                        SaveDatabaseToStorage();
                        SaveNodesToStorage();
                    }
                    catch (Exception e)
                    {
                        SGLog.LogException(e);
                    }
                    
                    
                    lastTimeSaveAllChangesToStorage = currentMillisecond;
                    isSavingAllChangesToStorage = false;
                }
                else
                {
                    //ZeroRx.ExecuteInMainThread(() => Debug.Log("Đợi 1 chút"));
                }


                
                
                
                try
                {
                    Update_InOtherThread_AfterSaveAllChangesToStorage();
                }
                catch (Exception e)
                {
                    SGLog.LogException(e);
                }
            }
        }

        protected virtual void Update_InOtherThread_AfterSaveAllChangesToStorage()
        {
            
        }
        
        #endregion


        protected abstract void SaveDatabaseToStorage();
        
        protected abstract void SaveNodesToStorage();
    }
}