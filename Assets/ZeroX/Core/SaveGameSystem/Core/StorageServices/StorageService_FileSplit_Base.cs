using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ZeroX.JsonSystem;
using ZeroX.RxSystem;
using ZeroX.SaveGameSystem.Security;


namespace ZeroX.SaveGameSystem.StorageServices
{
    public abstract class StorageService_FileSplit_Base : StorageService
    {
        //Temp
        private byte[] dbPassBytes;
        private StorageServiceState storageServiceState = StorageServiceState.Closed;
        private bool databaseChanged = false;
        private Database database;
        readonly HashSet<SGNode> hashSetNodeChanged = new HashSet<SGNode>();
        

        //Task Field
        private bool continueThreadUpdate = true; //flag
        private Task threadUpdate;
        
        public bool IsThreadUpdateFinished => threadUpdate.IsCompleted;
        public bool IsThreadUpdateFaulted => threadUpdate.IsFaulted;
        
        private readonly object lockObj_SaveNodeToStorage = new object();
        private readonly object lockObj_SaveDatabaseToStorage = new object();
        
        
        //Event
        readonly Disposables disposables = new Disposables();

        
        
        //Time Config
        private const ulong updatePerMillisecond = 20;
        private const ulong maxMillisecondIgnoreSave = 100;
        private ulong currentMillisecond = 0;
        private ulong lastTimeSetChanges = 0;
        private ulong lastTimeSaveAllChangesToStorage = 0;
        
        //Process
        private bool isSavingAllChangesToStorage = false;
        
        
        
        
        #region Implement

        public sealed override void OpenService(Database database)
        {
            this.database = database;
            storageServiceState = StorageServiceState.Opened;
            
            //Addition Init
            InitWhenOpenService(database.DatabaseId);
            
            //Db Pass
            InitDbPass();
            
            //Thread
            StartThreadUpdate();
            
            //Event
            ZeroRx.onApplicationPause.Subscribe(OnApplicationPause).AddTo(disposables);
            ZeroRx.onApplicationQuit.Subscribe(OnApplicationQuit).AddTo(disposables);
        }

        

        public sealed override void CloseService()
        {
            lock (database)
            {
                if(storageServiceState == StorageServiceState.Closed)
                    return;
                
                storageServiceState = StorageServiceState.Closed;
            
                disposables.Dispose();
                StopThreadUpdate();
                SaveAllChangesToStorage();
            
                // while (IsTaskUpdateToDiskFinished == false)
                // {
                //     Thread.Sleep(6);
                //     //Đợi Thread khác hoàn thành việc lưu hết changed rồi mới cho đi tiếp
                // }
            
                if(IsThreadUpdateFaulted)
                    Debug.LogException(threadUpdate.Exception);
            }
        }

        public sealed override void SaveAllChangesToStorage()
        {
            SaveDatabaseToStorage();
            SaveNodesToStorage();
        }

        public sealed override ZJsonObject LoadDatabaseDataJson()
        {
            lock (lockObj_SaveDatabaseToStorage)
            {
                if (IsDatabaseExistsInStorage(database.DatabaseId) == false)
                    return null;
                else
                {
                    try
                    {
                        string text = ReadDatabasePlainTextFromStorage(database.DatabaseId);
                        return ZJson.JsonToNode(text) as ZJsonObject; //as: data rỗng -> null -> caller tự SetupDefault
                    }
                    catch (Exception e)
                    {
                        //Data rác không parse được -> coi như không có data, mất nhánh này nhưng game không kẹt boot
                        Debug.LogException(e);
                        Debug.LogError("Read save database error -> create new save");
                        return null;
                    }
                }
            }
        }

        public sealed override ZJsonObject LoadNodeDataJson(long nodeId)
        {
            lock (lockObj_SaveNodeToStorage)
            {
                if (SGUtility.IsNodeIdEmpty(nodeId))
                    throw new Exception("Không thể load dataJson cho node không có nodeId");


                
                if (IsNodeExistsInStorage(nodeId) == false)
                    return null;
                else
                {
                    try
                    {
                        string text = ReadNodePlainTextFromStorage(nodeId);
                        return ZJson.JsonToNode(text) as ZJsonObject; //as: data rỗng -> null -> caller tự SetupDefault
                    }
                    catch (Exception e)
                    {
                        //Data rác không parse được -> coi như không có data, mất nhánh này nhưng game không kẹt boot
                        Debug.LogException(e);
                        Debug.LogError("Read save node error -> create new save, nodeId: " + nodeId);
                        return null;
                    }
                }
            }
        }

        public sealed override ZJsonObject LoadNodeDataJsonNoLock(long nodeId)
        {
            if (SGUtility.IsNodeIdEmpty(nodeId))
                throw new Exception("Không thể load dataJson cho node không có nodeId");
            
            if (IsNodeExistsInStorage(nodeId) == false)
                return null;
            else
            {
                try
                {
                    string text = ReadNodePlainTextFromStorage(nodeId);
                    return ZJson.JsonToNode(text) as ZJsonObject; //as: data rỗng -> null -> caller tự SetupDefault
                }
                catch (Exception e)
                {
                    //Data rác không parse được -> coi như không có data, mất nhánh này nhưng game không kẹt boot
                    Debug.LogException(e);
                    Debug.LogError("Read save node error -> create new save, nodeId: " + nodeId);
                    return null;
                }
            }
        }

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


        #region Abstract
        
        protected abstract void InitWhenOpenService(string databaseId);

        protected abstract byte[] GetOrCreateDatabasePass(string databaseId);
        
        protected abstract void DeleteDatabasePass(string databaseId);

        /// <summary>
        /// File, PlayerPrefs...
        /// </summary>
        protected abstract void DeleteDatabaseFromStorage(string databaseId);
        
        protected abstract bool IsDatabaseExistsInStorage(string databaseId);
        
        protected abstract string ReadDatabaseCipherTextFromStorage(string databaseId);
        
        protected abstract void WriteDatabaseCipherTextToStorage(string databaseId, string cipherText);
        
        protected abstract bool IsNodeExistsInStorage(long nodeId);
        
        protected abstract string ReadNodeCipherTextFromStorage(long nodeId);
        
        protected abstract void WriteNodeCipherTextToStorage(long nodeId, string cipherText);
        
        protected abstract void DeleteNodeFromStorage(long nodeId);

        /// <summary>
        /// Ví dụ là PlayerPref thì nên set tất cả thay đổi rồi mới gọi PlayerPref.Save()
        /// </summary>
        protected abstract void CommitSaveToStorage();

        #endregion
        
        private void InitDbPass()
        {
            try
            {
                dbPassBytes = GetOrCreateDatabasePass(database.DatabaseId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("Get DB Pass Error -> Delete database and re-create");
                
                DeleteDatabasePass(database.DatabaseId);
                DeleteDatabaseFromStorage(database.DatabaseId);
            }
            
            //Thử lại lần 2
            dbPassBytes = GetOrCreateDatabasePass(database.DatabaseId);
        }

        private string ReadDatabasePlainTextFromStorage(string databaseId)
        {
            var cipherText = ReadDatabaseCipherTextFromStorage(databaseId);
            if (string.IsNullOrEmpty(cipherText))
                return "";
            
            try
            {
                return SGAesEncrypt.DecryptString(cipherText, dbPassBytes);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return "";
            }
        }
        
        private void WriteDatabasePlainTextToStorage(string databaseId, string plainText)
        {
            var cipherText = SGAesEncrypt.EncryptString(plainText, dbPassBytes);
            
            try
            {
                WriteDatabaseCipherTextToStorage(databaseId, cipherText);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        private string ReadNodePlainTextFromStorage(long nodeId)
        {
            var cipherText = ReadNodeCipherTextFromStorage(nodeId);
            if (string.IsNullOrEmpty(cipherText))
                return "";
            
            try
            {
                return SGAesEncrypt.DecryptString(cipherText, dbPassBytes);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return "";
            }
        }
        
        private void WriteNodePlainTextToStorage(long nodeId, string plainText)
        {
            var cipherText = SGAesEncrypt.EncryptString(plainText, dbPassBytes);
            
            try
            {
                WriteNodeCipherTextToStorage(nodeId, cipherText);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveAllChangesToStorage();
            }
        }

        void OnApplicationQuit()
        {
            //SaveAllChangesToStorage();
        }
        
        


        #region Thread Update
        
        public void StartThreadUpdate()
        {
            continueThreadUpdate = true;
            threadUpdate = new Task(ThreadUpdate);
            threadUpdate.ContinueWith((t) =>
            {
                if (t.IsFaulted && storageServiceState != StorageServiceState.Closed)
                {
                    ZeroRx.RunAction(() => Debug.LogException(t.Exception));
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
                    
                    bool hasSaveDatabase = SaveDatabaseToStorage();
                    bool hasSaveNodes = SaveNodesToStorage();
                    
                    if(hasSaveDatabase || hasSaveNodes)
                        CommitSaveToStorage();
                    
                    lastTimeSaveAllChangesToStorage = currentMillisecond;
                    isSavingAllChangesToStorage = false;
                }
                else
                {
                    //ZeroRx.ExecuteInMainThread(() => Debug.Log("Đợi 1 chút"));
                }
            }
        }

        private bool SaveDatabaseToStorage()
        {
            ZJsonObject json;
            lock (database)
            {
                if(databaseChanged == false)
                    return false;

                try
                {
                    json = database.ToJsonObject();
                    databaseChanged = false;
                }
                catch (Exception e)
                {
                    ZeroRx.RunAction(() => Debug.LogException(e));
                    return false;
                }
            }

            
            
            lock (lockObj_SaveDatabaseToStorage)
            {
                try
                {
                    WriteDatabasePlainTextToStorage(database.DatabaseId, json.ToString());
                }
                catch (Exception e)
                {
                    ZeroRx.RunAction(() => Debug.LogException(e));
                }

                return true;
            }
        }

        private bool SaveNodesToStorage()
        {
            Dictionary<long, ZJsonObject> dictJson;
            List<long> listNodeIdDelete = null;
            lock (database)
            {
                if(hashSetNodeChanged.Count == 0)
                    return false;
                
                dictJson = new Dictionary<long, ZJsonObject>();
                foreach (var node in hashSetNodeChanged)
                {
                    try
                    {
                        if(node.IsDeleted == false)
                            dictJson.Add(node.NodeId, node.ToJsonObject());
                        else
                        {
                            if(listNodeIdDelete == null)
                                listNodeIdDelete = new List<long>();
                            
                            listNodeIdDelete.Add(node.NodeId);
                        }
                    }
                    catch (Exception e)
                    {
                        ZeroRx.RunAction(() => Debug.LogException(e));
                    }
                }
                
                hashSetNodeChanged.Clear();
            }

            
            
            lock (lockObj_SaveNodeToStorage)
            {
                foreach (var kv in dictJson)
                {
                    try
                    {
                        // if(kv.Value == null)
                        //     continue;
                        
                        WriteNodePlainTextToStorage(kv.Key, kv.Value.ToString());
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }

                if (listNodeIdDelete != null)
                {
                    foreach (var nodeId in listNodeIdDelete)
                    {
                        try
                        {
                            DeleteNodeFromStorage(nodeId);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }

                return true;
            }
        }
        
        #endregion
    }
}