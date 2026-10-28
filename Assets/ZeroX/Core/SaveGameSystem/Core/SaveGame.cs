using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZeroX.SaveGameSystem.Core;

namespace ZeroX.SaveGameSystem
{
    public static class SaveGame
    {
        private static string persistentDataPathInternal;
        private static string deviceUniqueIdentifierInternal;
        
        public static string persistentDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(persistentDataPathInternal))
                {
#if UNITY_EDITOR
                    persistentDataPathInternal = Path.Combine(Application.persistentDataPath, "Editor");
#else
                    persistentDataPathInternal = Application.persistentDataPath;
#endif
                }

                return persistentDataPathInternal;
            }
        }
        
        public static string deviceUniqueIdentifier
        {
            get
            {
                if (string.IsNullOrEmpty(deviceUniqueIdentifierInternal))
                {
                    deviceUniqueIdentifierInternal = SystemInfo.deviceUniqueIdentifier;
                }

                return deviceUniqueIdentifierInternal;
            }
        }
        
        
        

        static Dictionary<string, Database> dictDatabase = new Dictionary<string, Database>();
        
        private static Database defaultDatabase;
        private static Database sharedDatabase;
        
        
        
        [RuntimeInitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            string a = persistentDataPath;
            string b = deviceUniqueIdentifier;
        }
        
        static Database OpenDatabase(string databaseId)
        {
            StorageService storageService = StorageServiceProvider.Generate();
            Database database = new Database(databaseId, storageService);
            database.OpenDatabase();
            
            return database;
        }
        
        public static Database Login(string databaseId)
        {
            databaseId = databaseId.ToLower();
            if (dictDatabase.TryGetValue(databaseId, out var database))
            {
                if (database.IsDatabaseClosed == false)
                {
                    Debug.Log("<color=green>Đã login trước đó rồi</color>");
                    return database;
                }
                else
                {
                    dictDatabase.Remove(databaseId);
                    if (defaultDatabase != null)
                    {
                        if (databaseId == defaultDatabase.DatabaseId)
                            defaultDatabase = null;
                    }
                    
                    return Login(databaseId);
                }
            }

            database = OpenDatabase(databaseId);
            dictDatabase.Add(databaseId, database);
            Debug.LogFormat("<color=green>Login thành công: {0}</color>", databaseId);
            return database;
        }

        /// <summary>
        /// Login default database. Use database id is "0"
        /// </summary>
        public static Database Login()
        {
            defaultDatabase = Login(SGConstant.DEFAULT_DATABASE_ID);
            return defaultDatabase;
        }

        public static void Logout(string databaseId)
        {
            if (dictDatabase.TryGetValue(databaseId, out var database) == false)
            {
                Debug.Log("Database chưa login nên không cần logout: " + databaseId);
                return;
            }
            
            database.CloseDatabase();
            dictDatabase.Remove(databaseId);
            if (defaultDatabase != null)
            {
                if (databaseId == defaultDatabase.DatabaseId)
                    defaultDatabase = null;
            }
            
            Debug.LogFormat("<color=green>Logout thành công: {0}</color>", databaseId);
        }

        public static void Logout()
        {
            Logout(SGConstant.DEFAULT_DATABASE_ID);
        }

        public static bool IsLoggedIn(string databaseId)
        {
            return dictDatabase.ContainsKey(databaseId);
        }
        
        public static bool IsLoggedInDefault()
        {
            return defaultDatabase != null;
        }

        public static Database GetDatabaseLoggedIn(string databaseId)
        {
            if (dictDatabase.TryGetValue(databaseId, out var database))
                return database;
            else
                return null;
        }

        public static void SetDefaultDatabase(string databaseId)
        {
            if (dictDatabase.TryGetValue(databaseId, out var database))
            {
                defaultDatabase = database;
            }
            else
            {
                Debug.LogError("Chưa login database với id này nên không thể set làm default: " + database);
            }
        }

        public static Database Default
        {
            get
            {
                if (defaultDatabase == null)
                {
                    Login();
                }

                return defaultDatabase;
            }
        }

        public static Database Shared
        {
            get
            {
                if (sharedDatabase == null)
                {
                    sharedDatabase = OpenDatabase(SGConstant.SHARED_DATABASE_ID);
                }

                return sharedDatabase;
            }
        }
    }
}