using System;
using System.IO;
using System.Text;
using UnityEngine;
using ZeroX.SaveGameSystem.Security;
using Random = UnityEngine.Random;

namespace ZeroX.SaveGameSystem
{
    internal static class SgPassManager
    {
        private const string characters = "qwertyuiopasdfghjklzxcvbnm1234567890!@#$%^&*()QWERTYUIOPASDFGHJKLZXCVBNM";
        //private const string pF = "uPB{0}f2+rh9{0}nqRg&@{0}_!PB#Se%CJgNQZ";
        private const string p = "uPBf2+rh9nqRg&@_!PB#Se%CJgNQZ";

        
        public delegate string FGetString(string key, string defaultValue);
        public delegate void FSetString(string key, string value);
        public delegate void FDeleteKey(string key);
        public delegate void FSave();



        #region Core

        
        public static void DeleteDbPass(string databaseId, FDeleteKey DeleteKey, FSave Save)
        {
            string plainKey = string.Format("sg.pass_manager.db.{0}", databaseId);
            string cipherKey = SGAesEncrypt.EncryptString(plainKey, p);
            
            DeleteKey(cipherKey);
            Save();
        }
        
        public static string GetOrCreateDbPass(string databaseId, FGetString GetString, FSetString SetString, FSave Save)
        {
            string plainKey = string.Format("sg.pass_manager.db.{0}", databaseId);
            string cipherKey = SGAesEncrypt.EncryptString(plainKey, p);
            
            
            string cipherPass = GetString(cipherKey, "");
            if(string.IsNullOrWhiteSpace(cipherPass) == false)
                return SGAesEncrypt.DecryptString(cipherPass, p);
            
            
            //Không có thì tạo mới
            string plainPass = RandomDbPass();
            cipherPass = SGAesEncrypt.EncryptString(plainPass, p);
            
            SetString(cipherKey, cipherPass);
            Save();
            
            return plainPass;
        }
        
        private static string RandomDbPass()
        {
            System.Random random = new System.Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 32; i++)
            {
                int randomIndex = random.Next(0, characters.Length);
                sb.Append(characters[randomIndex]);
            }

            return sb.ToString();
        }
        
        #endregion
        

        public static void DeleteDbPassInPlayerPref(string databaseId)
        {
            string plainKey = string.Format("sg.pass_manager.db.{0}", databaseId);
            string cipherKey = SGAesEncrypt.EncryptString(plainKey, p);
            PlayerPrefs.DeleteKey(cipherKey);
            PlayerPrefs.Save();
        }
        
        public static string GetOrCreateDbPassInPlayerPref(string databaseId)
        {
            string plainKey = string.Format("sg.pass_manager.db.{0}", databaseId);
            string cipherKey = SGAesEncrypt.EncryptString(plainKey, p);

            string cipherPass;
            if (PlayerPrefs.HasKey(cipherKey))
            {
                cipherPass = PlayerPrefs.GetString(cipherKey);
                if(string.IsNullOrWhiteSpace(cipherPass) == false)
                    return SGAesEncrypt.DecryptString(cipherPass, p);
            }
            
            string plainPass = RandomDbPass();
            cipherPass = SGAesEncrypt.EncryptString(plainPass, p);
            PlayerPrefs.SetString(cipherKey, cipherPass);
            PlayerPrefs.Save();
            return plainPass;
        }

        

        private static string GetOrCreateDbPassInFile(string filePath, string databaseId)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception("File path cannot empty!");
            }
            
            string cipherPass;
            if (File.Exists(filePath))
            {
                cipherPass = File.ReadAllText(filePath);
                if(string.IsNullOrWhiteSpace(cipherPass) == false)
                    return SGAesEncrypt.DecryptString(cipherPass, p);
            }
            
            
            string directory = Path.GetDirectoryName(filePath);
            if(string.IsNullOrEmpty(directory))
                throw new Exception("Directory path cannot empty!");
            
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            string plainPass = RandomDbPass();
            cipherPass = SGAesEncrypt.EncryptString(plainPass, p);
            File.WriteAllText(filePath, cipherPass);
            return plainPass;
        }

        
    }
}