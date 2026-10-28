using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ZeroX.JsonSystem;
using ZeroX.RxSystem;
using ZeroX.SaveGameSystem.Security;

namespace ZeroX.SaveGameSystem.StorageServices
{
    public class StorageService_FileSplit : StorageService
    {
        //Don't change this!!!
        private const string rootFolderPathFormat = "SaveGame/FileSplit/db.{0}";
        private const string dbFileName = "db.dat";
        private const string dbPassFileName = "dbp.dat";
        private const string nodeFileNameFormat = "n.{0}.dat";
        
        //Cache
        private byte[] dbPassBytes;
        private string rootFolderPath;
        private string filePath_Database;
        private string filePath_DatabasePass;
        

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

        public override void OpenService(Database database)
        {
            this.database = database;
            storageServiceState = StorageServiceState.Opened;
            
            //Các path cần thiết
            rootFolderPath = Path.Combine(SaveGame.persistentDataPath,  string.Format(rootFolderPathFormat, database.DatabaseId));
            filePath_Database = Path.Combine(rootFolderPath, dbFileName);
            filePath_DatabasePass = Path.Combine(rootFolderPath, dbPassFileName);

            //Tạo folder nếu chưa có
            if (Directory.Exists(rootFolderPath) == false)
                Directory.CreateDirectory(rootFolderPath);
            
            
            //Db Pass
            try
            {
                string dbPass = SgPassManager.GetOrCreateDbPassInPlayerPref(database.DatabaseId);
                dbPassBytes = SGAesEncrypt.EncodeStringKeyToBytes(dbPass);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("Get DB Pass Error -> Delete database and re-create");
                Directory.Delete(rootFolderPath, true);
                Directory.CreateDirectory(rootFolderPath);
                
                string dbPass = SgPassManager.GetOrCreateDbPassInPlayerPref(database.DatabaseId);
                dbPassBytes = SGAesEncrypt.EncodeStringKeyToBytes(dbPass);
            }
            
            
            //Thread
            StartThreadUpdate();
            
            //Event
            ZeroRx.onApplicationPause.Subscribe(OnApplicationPause).AddTo(disposables);
            ZeroRx.onApplicationQuit.Subscribe(OnApplicationQuit).AddTo(disposables);
        }

        public override void CloseService()
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

        public override void SaveAllChangesToStorage()
        {
            SaveDatabaseToStorage();
            SaveNodesToStorage();
        }

        public override ZJsonObject LoadDatabaseDataJson()
        {
            lock (lockObj_SaveDatabaseToStorage)
            {
                if (File.Exists(filePath_Database) == false)
                    return null;
                else
                {
                    try
                    {
                        string text = File_ReadAllText(filePath_Database);
                        return ZJson.JsonToNode(text) as ZJsonObject; //as: file rỗng -> null -> caller tự SetupDefault
                    }
                    catch (Exception e)
                    {
                        //File rác không parse được -> coi như không có data, mất nhánh này nhưng game không kẹt boot
                        Debug.LogException(e);
                        Debug.LogError("Read save database error -> create new save");
                        return null;
                    }
                }
            }
        }
        
        public override ZJsonObject LoadNodeDataJson(long nodeId)
        {
            lock (lockObj_SaveNodeToStorage)
            {
                if (SGUtility.IsNodeIdEmpty(nodeId))
                    throw new Exception("Không thể load dataJson cho node không có nodeId");


                string filePath = GenerateFilePath_Node(nodeId);
                if (File.Exists(filePath) == false)
                    return null;
                else
                {
                    try
                    {
                        string text = File_ReadAllText(filePath);
                        return ZJson.JsonToNode(text) as ZJsonObject; //as: file rỗng -> null -> caller tự SetupDefault
                    }
                    catch (Exception e)
                    {
                        //File rác không parse được -> coi như không có data, mất nhánh này nhưng game không kẹt boot
                        Debug.LogException(e);
                        Debug.LogError("Read save node error -> create new save, nodeId: " + nodeId);
                        return null;
                    }
                }
            }
        }

        public override ZJsonObject LoadNodeDataJsonNoLock(long nodeId)
        {
            if (SGUtility.IsNodeIdEmpty(nodeId))
                throw new Exception("Không thể load dataJson cho node không có nodeId");
            
            string filePath = GenerateFilePath_Node(nodeId);
            if (File.Exists(filePath) == false)
                return null;
            else
            {
                try
                {
                    string text = File_ReadAllText(filePath);
                    return ZJson.JsonToNode(text) as ZJsonObject; //as: file rỗng -> null -> caller tự SetupDefault
                }
                catch (Exception e)
                {
                    //File rác không parse được -> coi như không có data, mất nhánh này nhưng game không kẹt boot
                    Debug.LogException(e);
                    Debug.LogError("Read save node error -> create new save, nodeId: " + nodeId);
                    return null;
                }
            }
        }

        public override void SetDatabaseChanged()
        {
            lock (database)
            {
                databaseChanged = true;
                lastTimeSetChanges = currentMillisecond;
            }
        }
        
        public override void SetNodeChanged(SGNode node)
        {
            lock (database)
            {
                if (node.HasNodeId == false)
                    throw new Exception("Không thể set changed cho node không có nodeId");

                hashSetNodeChanged.Add(node);
                lastTimeSetChanges = currentMillisecond;
            }
        }

        public override bool HasRunningProcess()
        {
            return isSavingAllChangesToStorage;
        }

        public override bool HasChangesNeedWriteToStorage()
        {
            lock (database)
            {
                return databaseChanged || hashSetNodeChanged.Count > 0;
            }
        }

        #endregion
        
        string GenerateFilePath_Node(long nodeId)
        {
            return Path.Combine(rootFolderPath, string.Format(nodeFileNameFormat, nodeId));
        }
        
        string File_ReadAllText(string filePath)
        {
#if UNITY_EDITOR
            return File.ReadAllText(filePath);
#else
            var cipherBytes = File.ReadAllBytes(filePath);
            if (cipherBytes == null || cipherBytes.Length == 0)
                return "";
            
            try
            {
                byte[] plainBytes = SGAesEncrypt.DecryptBytes(cipherBytes, dbPassBytes);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return "";
            }
#endif
        }

        void File_WriteAllText(string filePath, string contents)
        {
            //Ghi atomic: ghi ra file tạm rồi rename đè lên file thật.
            //Bị kill giữa chừng thì chỉ hỏng file tạm, file thật vẫn nguyên bản trước đó -> không còn file cụt
            string tempFilePath = filePath + ".tmp";

#if UNITY_EDITOR
            File.WriteAllText(tempFilePath, contents);
#else
            var cipherBytes = SGAesEncrypt.EncryptStringToBytes(contents, dbPassBytes);
            try
            {
                File.WriteAllBytes(tempFilePath, cipherBytes);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }
#endif

            if (File.Exists(filePath))
                File.Replace(tempFilePath, filePath, null); //Replace = rename, nguyên tử ở mức OS
            else
                File.Move(tempFilePath, filePath);
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
                    
                    SaveDatabaseToStorage();
                    SaveNodesToStorage();
                    
                    lastTimeSaveAllChangesToStorage = currentMillisecond;
                    isSavingAllChangesToStorage = false;
                }
                else
                {
                    //ZeroRx.ExecuteInMainThread(() => Debug.Log("Đợi 1 chút"));
                }
            }
        }

        void SaveDatabaseToStorage()
        {
            ZJsonObject jsonObject;
            lock (database)
            {
                if(databaseChanged == false)
                    return;

                try
                {
                    jsonObject = database.ToJsonObject();
                    databaseChanged = false;
                }
                catch (Exception e)
                {
                    ZeroRx.RunAction(() => Debug.LogException(e));
                    return;
                }
            }

            
            
            lock (lockObj_SaveDatabaseToStorage)
            {
                try
                {
                    File_WriteAllText(filePath_Database, jsonObject.ToString());
                }
                catch (Exception e)
                {
                    ZeroRx.RunAction(() => Debug.LogException(e));
                }
            }
        }

        void SaveNodesToStorage()
        {
            Dictionary<long, ZJsonObject> dictJson;
            List<long> listNodeIdDelete = null;
            lock (database)
            {
                if(hashSetNodeChanged.Count == 0)
                    return;
                
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
                    
                        string filePath = GenerateFilePath_Node(kv.Key);
                        File_WriteAllText(filePath, kv.Value.ToString());
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
                            string filePath = GenerateFilePath_Node(nodeId);
                            if(File.Exists(filePath))
                                File.Delete(filePath);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }
        }
        
        #endregion
    }
}