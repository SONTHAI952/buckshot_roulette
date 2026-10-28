#if ZEROX_SDK_TIKTOK
using System;
using UnityEngine;
using ZeroX.SaveGameSystem.Security;
using ZeroX.TiktokSystem;

namespace ZeroX.SaveGameSystem.StorageServices
{
    public class StorageService_Tiktok : StorageService_FileSplit_Base_NonTask
    {
        private const string DatabaseKeyFormat = "sg.db.{0}.db";
        private const string NodeKeyFormatOriginal = "sg.db.{0}.nodes.{1}";
        
        //Temp
        private string databaseKey;
        private string nodeKeyFormat;
        
        
        protected override void InitWhenOpenService(string databaseId)
        {
            databaseKey = string.Format(DatabaseKeyFormat, databaseId);
            nodeKeyFormat = string.Format(NodeKeyFormatOriginal, databaseId, "{0}");
        }

        

        #region Database Pass

        protected override byte[] GetOrCreateDatabasePass(string databaseId)
        {
            string dbPass = SgPassManager.GetOrCreateDbPass(databaseId, TiktokManager.Instance.Save.GetString, TiktokManager.Instance.Save.SetString, TiktokManager.Instance.Save.Save);
            return SGAesEncrypt.EncodeStringKeyToBytes(dbPass);
        }

        protected override void DeleteDatabasePass(string databaseId)
        {
            SgPassManager.DeleteDbPass(databaseId, TiktokManager.Instance.Save.DeleteKey, TiktokManager.Instance.Save.Save);
        }

        #endregion
        
        

        #region Database
        
        protected override void DeleteDatabaseFromStorage(string databaseId)
        {
            TiktokManager.Instance.Save.DeleteKey(databaseKey);
        }

        protected override bool IsDatabaseExistsInStorage(string databaseId)
        {
            string cipherText = ReadDatabaseCipherTextFromStorage(databaseId);
            return string.IsNullOrEmpty(cipherText) == false;
        }

        protected override string ReadDatabaseCipherTextFromStorage(string databaseId)
        {
            return TiktokManager.Instance.Save.GetString(databaseKey, "");
        }

        protected override void WriteDatabaseCipherTextToStorage(string databaseId, string cipherText)
        {
            TiktokManager.Instance.Save.SetString(databaseKey, cipherText);
        }

        #endregion



        #region Node

        protected override bool IsNodeExistsInStorage(long nodeId)
        {
            string cipherText = ReadNodeCipherTextFromStorage(nodeId);
            return string.IsNullOrEmpty(cipherText) == false;
        }

        protected override string ReadNodeCipherTextFromStorage(long nodeId)
        {
            string nodeKey = string.Format(nodeKeyFormat, nodeId);
            return TiktokManager.Instance.Save.GetString(nodeKey, "");
        }

        protected override void WriteNodeCipherTextToStorage(long nodeId, string cipherText)
        {
            string nodeKey = string.Format(nodeKeyFormat, nodeId);
            TiktokManager.Instance.Save.SetString(nodeKey, cipherText);
        }

        protected override void DeleteNodeFromStorage(long nodeId)
        {
            string nodeKey = string.Format(nodeKeyFormat, nodeId);
            TiktokManager.Instance.Save.DeleteKey(nodeKey);
        }

        #endregion


        protected override void CommitSaveToStorage()
        {
            TiktokManager.Instance.Save.Save();
        }
    }
}
#endif