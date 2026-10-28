using System;
using ZeroX.JsonSystem;

namespace ZeroX.SaveGameSystem
{
    public abstract class StorageService
    {
        /// <summary>
        /// Call by database
        /// </summary>
        public abstract void OpenService(Database database);

        public abstract void CloseService();

        public abstract ZJsonObject LoadDatabaseDataJson();
        
        public abstract ZJsonObject LoadNodeDataJson(long nodeId);

        public abstract ZJsonObject LoadNodeDataJsonNoLock(long nodeId);

        public abstract void SetDatabaseChanged();
        
        public abstract void SetNodeChanged(SGNode node);
        
        public abstract void SaveAllChangesToStorage();

        public virtual bool HasRunningProcess()
        {
            return false;
        }

        public virtual bool HasChangesNeedWriteToStorage()
        {
            return false;
        }
    }
}