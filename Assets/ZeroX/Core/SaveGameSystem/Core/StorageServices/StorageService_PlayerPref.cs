using System;
using UnityEngine;
using ZeroX.SaveGameSystem.Security;

namespace ZeroX.SaveGameSystem.StorageServices
{
    public class StorageService_PlayerPref : StorageService_FileSplit_Base
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
            string dbPass = SgPassManager.GetOrCreateDbPassInPlayerPref(databaseId);
            return SGAesEncrypt.EncodeStringKeyToBytes(dbPass);
        }

        protected override void DeleteDatabasePass(string databaseId)
        {
            SgPassManager.DeleteDbPassInPlayerPref(databaseId);
        }

        #endregion
        
        

        #region Database
        
        protected override void DeleteDatabaseFromStorage(string databaseId)
        {
            PlayerPrefs.DeleteKey(databaseKey);
        }

        protected override bool IsDatabaseExistsInStorage(string databaseId)
        {
            string cipherText = ReadDatabaseCipherTextFromStorage(databaseId);
            return string.IsNullOrEmpty(cipherText) == false;
        }

        protected override string ReadDatabaseCipherTextFromStorage(string databaseId)
        {
            return PlayerPrefs.GetString(databaseKey, "");
        }

        protected override void WriteDatabaseCipherTextToStorage(string databaseId, string cipherText)
        {
            RunActionInMainThread(() =>
            {
                PlayerPrefs.SetString(databaseKey, cipherText);
            });
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
            return PlayerPrefs.GetString(nodeKey, "");
        }

        protected override void WriteNodeCipherTextToStorage(long nodeId, string cipherText)
        {
            string nodeKey = string.Format(nodeKeyFormat, nodeId);
            
            RunActionInMainThread(() =>
            {
                PlayerPrefs.SetString(nodeKey, cipherText);
            });
        }

        protected override void DeleteNodeFromStorage(long nodeId)
        {
            string nodeKey = string.Format(nodeKeyFormat, nodeId);
            
            RunActionInMainThread(() =>
            {
                PlayerPrefs.DeleteKey(nodeKey);
            });
        }

        #endregion


        protected override void CommitSaveToStorage()
        {
            RunActionInMainThread(() =>
            {
                PlayerPrefs.Save();
            });
        }


        private void RunActionInMainThread(Action action)
        {
#if UNITY_EDITOR
            ZeroX.RxSystem.Editors.ZeroEditorRx.RunAction(action);
#else
            ZeroRx.RunAction(action);
#endif
        }
    }
}