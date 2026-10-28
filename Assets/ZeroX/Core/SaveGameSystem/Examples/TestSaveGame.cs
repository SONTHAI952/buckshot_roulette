using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
using ZeroX.JsonSystem;
using ZeroX.SaveGameSystem.Security;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace ZeroX.SaveGameSystem.Examples
{
    public class TestSaveGame : MonoBehaviour
    {
        [NonSerialized]
        EString es;
        private void Start()
        {
            //SaveGame.Default.TryGetC
            //SaveGame.Default.DeleteChild("daily_reward");
            // var a = SaveGame.Default["lucky_wheel"]["lastTimeSpin"];
            // var b = SaveGame.Default["lucky_wheel"]["spinedCount"];
            // var c = SaveGame.Default["daily_reward"]["lastTimeReceived"];
            // var d = SaveGame.Default["daily_reward"]["receivedToDay"];
            // var e = SaveGame.Default["lucky_wheel"]["spinedCount"]["some_thing"];
            // var f = SaveGame.Default["lucky_wheel"]["spinedCount"]["woa_woa"];
            //var jNode = SGJson.Parse("đâsCaSCÂcS@3@!1");
            //Debug.Log(jNode);
            //var a = SaveGame.Default["lucky_wheel"]["lastTimeSpin"];
            //a.StringValue = "06-11-1997";
            //Debug.Log(a.StringValue);
            //a.DeleteChild("0");
            //SaveGame.Default["lucky_wheel"]["lastTimeSpin"].SetString("06-11-1997");
            //Debug.Log(a.GetString());
            // SaveGame.Default["lucky_wheel"]["spinnedCount"].SetInt(232);
            // SaveGame.Default["lucky_wheel"].SetString("hế lô");
            // var d = SaveGame.Default["lucky_wheel"];
            // var b = SaveGame.Default["lucky_wheel"]["spinnedCount"];
            // var c = b.ChildKeySeq;
            // b.AddChild("hello các bạn");
            //Debug.Log(SaveGame.Default["lucky_wheel"]["spinnedCount"].GetInt());

            // SaveGame.Default["inventory"].AddChild("coin");
            // SaveGame.Default["inventory"].AddChild("gem");
            //SaveGame.Default["lucky_wheel"].DeleteChild("spinnedCount");
            
            
            // SaveGame.Default["lucky_wheel"]["last_time_spin"].SetDateTime(DateTime.Now);
            // SaveGame.Default["lucky_wheel"]["spinned_count"].SetInt(5);
            //
            // var moneyNode = SaveGame.Default["money"];
            // moneyNode["coin"].LongValue = 155000;
            // moneyNode["gem"].LongValue = 1000;
            //EHistory.PlusAmountOfDate(DateTime.Now.AddDays(-1), "hello", 3);
            //UEHistory.PlusAmountOfDate(DateTime.Now, "hello", 5);
            //Debug.Log(UEHistory.GetAmountInDateRange(DateTime.Now.AddDays(-1), DateTime.Now, "hello"));
            //SaveGame.Default.TryGetChild(out var childNode, "inventory", "ship_1", "bullet_level");
            //Debug.Log(childNode.FloatValue);

            // List<string> list = new List<string>(){"a1", "a2", "a3", "a4", "a5"};
            // var listTestNode = SaveGame.Default["list_test"];
            // listTestNode.AddChildValueRange(list);
            // listTestNode.AddChildWithSeq("bottle_1");
            // listTestNode.AddChildWithSeq("bottle_2");
            // listTestNode.AddChildWithSeq("bottle_3");
            // listTestNode.AddChildWithSeq("bottle_3");
            // listTestNode.AddChildWithSeq("bottle_5");
            //int a = listTestNode.DeleteAllChild(n => n.StringValue == "bottle_3");
            //Debug.Log(a);

            // SaveGame.Login();
            // var a = SaveGame.Default["cmn"];
            // a["cc"].IntValue = 2;
            // Debug.Log(a["cc"].IntValue);
            // Aes aesAlg = Aes.Create();
            // Debug.Log(aesAlg.Mode);
            // Debug.Log(aesAlg.Padding);
            //Firebase.Crashlytics.Crashlytics.LogException(new Exception("Nothing"));
            
            //DateTime dateTime = DateTime.Now;
            //Debug.Log(dateTime.);
        }


        IEnumerator WaitTask()
        {
            Task task = new Task(() =>
            {
                Thread.Sleep(1000);
                throw new Exception("nonono");
            });
            task.Start();
            
            yield return new WaitForTaskEnd(task);
            Debug.Log("Task end: " + task.IsFaulted);
        }

        public void Login()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            SaveGame.Login();
            st.Stop();
            Debug.Log("Login time: " + st.ElapsedMilliseconds);
        }

        public void Logout()
        {
            SaveGame.Logout();
        }

        [ContextMenu("Test Encrypt Type")]
        public void TestEncryptTypePerformance()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            EString e = new EString("");
            for (int i = 0; i < 10000; i++)
            {
                e.Value = "hello các bạn nhé, dài chút nào";
            }
            st.Stop();
            Debug.Log("EString: " + st.ElapsedMilliseconds);
            
            st.Restart();
            for (int i = 0; i < 10000; i++)
            {
                SGAesEncrypt.EncryptString("hello các bạn nhé, dài chút nào", "123456789000");
            }
            st.Stop();
            Debug.Log("AES: " + st.ElapsedMilliseconds);
        }

        [ContextMenu("Test Save One File")]
        public void TestSaveGame10kKey_1File()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            for (int i = 0; i < 10000; i++)
            {
                SaveGame.Default["hello"]["key_" + i].StringValue = "hello các bạn nhé";
            }
            st.Stop();
            Debug.Log("Save 10k key trong 1 file: " + st.ElapsedMilliseconds);
        }
        
        [ContextMenu("Test Save Multi File")]
        public void TestSaveGame100kKey_10kFile()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            //SGNode helloNode = SaveGame.Default["hello"];
            for (int i = 0; i < 10000; i++)
            {
                //var nodeA = helloNode["A_" + i];
                for (int j = 0; j < 10; j++)
                {
                    SaveGame.Default["A_" + i]["B_" + j].StringValue = "hello các bạn nhé";
                    //nodeA["B_" + j].StringValue = "hello các bạn nhé";
                }
            }
            st.Stop();
            Debug.Log("Save 100k key trong 10k file: " + st.ElapsedMilliseconds);
        }
        
        [ContextMenu("Test Save Multi File")]
        public void TestSaveGame10kKey_10kFile()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            for (int i = 0; i < 10000; i++)
            {
                SaveGame.Default["A_" + i]["B_1"].StringValue = "hello các bạn nhé";
            }
            st.Stop();
            Debug.Log("Save 10k key trong 10k file: " + st.ElapsedMilliseconds);
        }

        public void TestSet100Level()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            var nodeCampaignLevel = SaveGame.Default["campaign_level"];
            for (int i = 1; i <= 100; i++)
            {
                var nodeLevelData = nodeCampaignLevel[i.ToString()];
                nodeLevelData["isCompleted"].IntValue = 1;
                nodeLevelData["starReached"].IntValue = 3;
                nodeLevelData["data_1"].StringValue = "hello các bạn";
                nodeLevelData["data_2"].StringValue = "hello các bạn";
                nodeLevelData["data_3"].StringValue = "hello các bạn";
                nodeLevelData["data_4"].StringValue = "hello các bạn";
                nodeLevelData["data_5"].StringValue = "hello các bạn";
                nodeLevelData["data_6"].StringValue = "hello các bạn";
            }
            st.Stop();
            Debug.Log("Set 100 level: " + st.ElapsedMilliseconds);
        }

        public void TestGet100Level()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            var nodeCampaignLevel = SaveGame.Default["campaign_level"];
            for (int i = 1; i <= 100; i++)
            {
                var nodeLevelData = nodeCampaignLevel[i.ToString()];
                int starReached = nodeLevelData["starReached"].IntValue;
            }
            st.Stop();
            Debug.Log("Get 100 level: " + st.ElapsedMilliseconds);
        }

        public void TestSet1000Level()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            var nodeCampaignLevel = SaveGame.Default["campaign_level"];
            for (int i = 1; i <= 10000; i++)
            {
                var nodeLevelData = nodeCampaignLevel[i.ToString()];
                nodeLevelData["isCompleted"].IntValue = 1;
                nodeLevelData["starReached"].IntValue = 3;
                nodeLevelData["data_1"].StringValue = "hello các bạn";
                nodeLevelData["data_2"].StringValue = "hello các bạn";
                nodeLevelData["data_3"].StringValue = "hello các bạn";
                nodeLevelData["data_4"].StringValue = "hello các bạn";
                nodeLevelData["data_5"].StringValue = "hello các bạn";
                nodeLevelData["data_6"].StringValue = "hello các bạn";
            }
            st.Stop();
            Debug.Log("Set 10000 level: " + st.ElapsedMilliseconds);
        }
        
        public void TestGet1000Level()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            var nodeCampaignLevel = SaveGame.Default["campaign_level"];
            for (int i = 1; i <= 10000; i++)
            {
                var nodeLevelData = nodeCampaignLevel[i.ToString()];
                int starReached = nodeLevelData["starReached"].IntValue;
            }
            st.Stop();
            Debug.Log("Get 10000 level: " + st.ElapsedMilliseconds);
        }
        
        public void TestInitialize1000Level()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            var nodeCampaignLevel = SaveGame.Default["campaign_level"];
            nodeCampaignLevel.InitializeAllChild();
            // for (int i = 1; i <= 10000; i++)
            // {
            //     var nodeLevelData = nodeCampaignLevel[i.ToString()];
            //     int starReached = nodeLevelData["starReached"].IntValue;
            // }
            st.Stop();
            Debug.Log("Initialize 10000 level: " + st.ElapsedMilliseconds);
        }

        string GenerateSmallString()
        {
            Dictionary<string, Dictionary<string, string>> dictSmall = new Dictionary<string, Dictionary<string, string>>();
            for (int i = 0; i < 100; i++)
            {
                string key1 = "key_" + i;
                Dictionary<string, string> dict2 = new Dictionary<string, string>();
                dictSmall.Add(key1, dict2);
                for (int j = 0; j < 10; j++)
                {
                    string key2 = "key_" + j;
                    dict2.Add(key2, "hello các bạn nhé ahihihihi");
                }
            }
            
            return ZJson.ObjectToJson(dictSmall);
        }
        
        string GenerateBigString()
        {
            Dictionary<string, Dictionary<string, string>> dictBig = new Dictionary<string, Dictionary<string, string>>();
            for (int i = 0; i < 1000; i++)
            {
                string key1 = "key_" + i;
                Dictionary<string, string> dict2 = new Dictionary<string, string>();
                dictBig.Add(key1, dict2);
                for (int j = 0; j < 100; j++)
                {
                    string key2 = "key_" + j;
                    dict2.Add(key2, "hello các bạn nhé ahihihihi");
                }
            }
            
            return ZJson.ObjectToJson(dictBig);
        }
        
        string GenerateHugeString()
        {
            Dictionary<string, Dictionary<string, string>> dictBig = new Dictionary<string, Dictionary<string, string>>();
            for (int i = 0; i < 1000; i++)
            {
                string key1 = "key_" + i;
                Dictionary<string, string> dict2 = new Dictionary<string, string>();
                dictBig.Add(key1, dict2);
                for (int j = 0; j < 1000; j++)
                {
                    string key2 = "key_" + j;
                    dict2.Add(key2, "hello các bạn nhé ahihihihi");
                }
            }
            
            return ZJson.ObjectToJson(dictBig);
        }

        [ContextMenu("Test Write File")]
        public void TestWriteFile()
        {
            string jsonSmall = GenerateSmallString();
            string jsonBig = GenerateBigString();
            string jsonHuge = GenerateHugeString();

            string pathSmall = Path.Combine(SaveGame.persistentDataPath, "smallJson.dat");
            string pathBig = Path.Combine(SaveGame.persistentDataPath, "bigJson.dat");
            string pathHuge = Path.Combine(SaveGame.persistentDataPath, "hugeJson.dat");
            
            
            Stopwatch st = new Stopwatch();
            st.Start();
            File.WriteAllText(pathSmall, jsonSmall);
            st.Stop();
            Debug.Log("Write Small: " + st.ElapsedMilliseconds);
            
            st.Restart();
            File.WriteAllText(pathBig, jsonBig);
            st.Stop();
            Debug.Log("Write Big: " + st.ElapsedMilliseconds);
            
            st.Restart();
            File.WriteAllText(pathHuge, jsonHuge);
            st.Stop();
            Debug.Log("Write Huge: " + st.ElapsedMilliseconds);
        }
        
        [ContextMenu("Test Read File")]
        public void TestReadFile()
        {
            string pathSmall = Path.Combine(SaveGame.persistentDataPath, "smallJson.dat");
            string pathBig = Path.Combine(SaveGame.persistentDataPath, "bigJson.dat");
            string pathHuge = Path.Combine(SaveGame.persistentDataPath, "hugeJson.dat");
            
            
            
            Stopwatch st = new Stopwatch();
            st.Start();
            File.ReadAllText(pathSmall);
            st.Stop();
            Debug.Log("Read Small: " + st.ElapsedMilliseconds);
            
            st.Restart();
            File.ReadAllText(pathBig);
            st.Stop();
            Debug.Log("Read Big: " + st.ElapsedMilliseconds);
            
            st.Restart();
            File.ReadAllText(pathHuge);
            st.Stop();
            Debug.Log("Read Huge: " + st.ElapsedMilliseconds);
        }
        
        
        
        [ContextMenu("Test Write File Encrypt")]
        public void TestWriteFileWithEncrypt()
        {
            string jsonSmall = GenerateSmallString();
            string jsonBig = GenerateBigString();
            string jsonHuge = GenerateHugeString();

            string pathSmall = Path.Combine(SaveGame.persistentDataPath, "smallJson.dat");
            string pathBig = Path.Combine(SaveGame.persistentDataPath, "bigJson.dat");
            string pathHuge = Path.Combine(SaveGame.persistentDataPath, "hugeJson.dat");
            
            
            Stopwatch st = new Stopwatch();
            st.Start();
            File.WriteAllText(pathSmall, SGAesEncrypt.EncryptString(jsonSmall, "12345678"));
            st.Stop();
            Debug.Log("Write Small Encrypt: " + st.ElapsedMilliseconds);
            
            st.Restart();
            File.WriteAllText(pathBig, SGAesEncrypt.EncryptString(jsonBig, "12345678"));
            st.Stop();
            Debug.Log("Write Big Encrypt: " + st.ElapsedMilliseconds);
            
            st.Restart();
            File.WriteAllText(pathHuge, SGAesEncrypt.EncryptString(jsonHuge, "12345678"));
            st.Stop();
            Debug.Log("Write Huge Encrypt: " + st.ElapsedMilliseconds);
        }
        
        [ContextMenu("Test Read File Decrypt")]
        public void TestReadFileWithDecrypt()
        {
            string pathSmall = Path.Combine(SaveGame.persistentDataPath, "smallJson.dat");
            string pathBig = Path.Combine(SaveGame.persistentDataPath, "bigJson.dat");
            string pathHuge = Path.Combine(SaveGame.persistentDataPath, "hugeJson.dat");
            
            
            
            Stopwatch st = new Stopwatch();
            st.Start();
            SGAesEncrypt.DecryptString(File.ReadAllText(pathSmall), "12345678");
            st.Stop();
            Debug.Log("Read Small Decrypt: " + st.ElapsedMilliseconds);
            
            st.Restart();
            SGAesEncrypt.DecryptString(File.ReadAllText(pathBig), "12345678");
            st.Stop();
            Debug.Log("Read Big Decrypt: " + st.ElapsedMilliseconds);
            
            st.Restart();
            SGAesEncrypt.DecryptString(File.ReadAllText(pathHuge), "12345678");
            st.Stop();
            Debug.Log("Read Huge Decrypt: " + st.ElapsedMilliseconds);
        }
        
        [ContextMenu("Test Base 64 Performance")]
        public void TestBase64Performance()
        {
            string jsonSmall = GenerateSmallString();
            string jsonBig = GenerateBigString();
            string jsonHuge = GenerateHugeString();

            
            
            Stopwatch st = new Stopwatch();
            st.Start();
            var s = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonHuge));
            st.Stop();
            Debug.Log("To Base 64: " + st.ElapsedMilliseconds);
            
            // st.Restart();
            // SGAesEncrypt.DecryptString(File.ReadAllText(pathBig), "12345678");
            // st.Stop();
            // Debug.Log("Read Big Decrypt: " + st.ElapsedMilliseconds);
            //
            // st.Restart();
            // SGAesEncrypt.DecryptString(File.ReadAllText(pathHuge), "12345678");
            // st.Stop();
            // Debug.Log("Read Huge Decrypt: " + st.ElapsedMilliseconds);
        }
        
        public void TestReadFileNormal()
        {
            string pathFolder = Path.Combine(SaveGame.persistentDataPath, "SaveGame/FileSplit/db.0");
            
            Stopwatch st = new Stopwatch();
            st.Start();
            for (int i = 101; i <= 10101; i++)
            {
                string filePath = pathFolder + string.Format("/n.{0}.dat", i);
                File.ReadAllText(filePath);
            }

            st.Stop();
            Debug.Log("Read file normal: " + st.ElapsedMilliseconds);
        }

        [ContextMenu("Read read file parallel")]
        public void TestReadFileParallel()
        {
            string pathFolder = Path.Combine(SaveGame.persistentDataPath, "SaveGame/FileSplit/db.0");
            
            //List<string> listText = new List<string>();
            Stopwatch st = new Stopwatch();
            st.Start();

            Parallel.For(101, 10101, i =>
            {
                string filePath = pathFolder + string.Format("/n.{0}.dat", i);
                string s = File.ReadAllText(filePath);
                // lock (listText)
                // {
                //     listText.Add(s);
                // }
            });
            
            st.Stop();
            Debug.Log("Read file parallel: " + st.ElapsedMilliseconds);

            // Debug.Log(listText.Count);
            // Debug.Log(listText[5000]);
        }

        [ContextMenu("Test read binary vs base 64")]
        public void TestReadBinaryVsBase64()
        {
            string smallString = GenerateSmallString();
            string pathBase64 = Path.Combine(SaveGame.persistentDataPath, "base64.dat");
            string pathBinary = Path.Combine(SaveGame.persistentDataPath, "binary.dat");
            
            File.WriteAllText(pathBase64, SGAesEncrypt.EncryptString(smallString, "12345678"));
            File.WriteAllBytes(pathBinary, SGAesEncrypt.EncryptStringToBytes(smallString, "12345678"));
            
            Stopwatch st = new Stopwatch();
            st.Start();
            for (int i = 0; i < 1000; i++)
            {
                SGAesEncrypt.DecryptString(File.ReadAllText(pathBase64), "12345678");
            }
            st.Stop();
            Debug.Log("Base64: " + st.ElapsedMilliseconds);
            
            st.Restart();
            for (int i = 0; i < 1000; i++)
            {
                var cipherBytes = File.ReadAllBytes(pathBinary);
                var plainBytes = SGAesEncrypt.DecryptBytes(cipherBytes, "12345678");
                Encoding.UTF8.GetString(plainBytes);
            }
            st.Stop();
            Debug.Log("Binary: " + st.ElapsedMilliseconds);
        }

        [ContextMenu("Test key dài ngắn")]
        void TestKeyDaiNgan()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            for (int i = 0; i < 1000; i++)
            {
                dict.Add("linh_tinh_" + i, Random.Range(0, 10000));
            }
            
            dict.Add("a123456789123456789123456789", 1000);
           
            for (int i = 1000; i < 2000; i++)
            {
                dict.Add("linh_tinh_" + i, Random.Range(0, 10000));
            }
            
            dict.Add("a", 999);
            
            for (int i = 2000; i < 3000; i++)
            {
                dict.Add("linh_tinh_" + i, Random.Range(0, 10000));
            }
            
            Stopwatch st = new Stopwatch();
            st.Start();
            for (int i = 0; i < 1000000; i++)
            {
                string key = "a";
                //int value = dict["a"];
            }
            st.Stop();
            Debug.Log("Key ngắn: " + st.ElapsedMilliseconds);
            
            
            st.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                string key = "a123456789123456789123456789";
                //int value = dict["a123456789123456789123456789"];
            }
            st.Stop();
            Debug.Log("Key dài: " + st.ElapsedMilliseconds);
        }

        [ContextMenu("Test key dài ngắn 2")]
        public void TestKeyDaiNgan2()
        {
            Dictionary<string, int> dictKeyNgan = new Dictionary<string, int>();
            Dictionary<string, int> dictKeyDai = new Dictionary<string, int>();
            
            //Set Key Ngắn
            Stopwatch st = new Stopwatch();
            st.Start();
            for (int i = 0; i < 10000; i++)
            {
                dictKeyNgan["a" + i] = i;
            }
            st.Stop();
            Debug.Log("Set Key Ngắn: " + st.ElapsedMilliseconds);
            
            //Get Key Ngắn
            st.Restart();
            for (int i = 0; i < 10000; i++)
            {
                int value = dictKeyNgan["a" + i];
            }
            st.Stop();
            Debug.Log("Get Key Ngắn: " + st.ElapsedMilliseconds);
            
            
            //Set Key Dài
            st.Restart();
            for (int i = 0; i < 10000; i++)
            {
                dictKeyDai["ancaskcnjksanjkfjsaknfjsankjcnkasjcnkjsankfas" + i] = i;
            }
            st.Stop();
            Debug.Log("Set Key Dài: " + st.ElapsedMilliseconds);
            
            
            //Get Key Dài
            st.Restart();
            for (int i = 0; i < 10000; i++)
            {
                int value = dictKeyDai["ancaskcnjksanjkfjsaknfjsankjcnkasjcnkjsankfas" + i];
            }
            st.Stop();
            Debug.Log("Get Key Dài: " + st.ElapsedMilliseconds);
        }
        
        
        
        [ContextMenu("Test key dài ngắn 3")]
        public void TestKeyDaiNgan3()
        {
            Dictionary<string, int> dictKeyNgan = new Dictionary<string, int>();
            Dictionary<string, int> dictKeyDai = new Dictionary<string, int>();
            
            List<string> listKeyNgan = new List<string>();
            List<string> listKeyDai = new List<string>();

            //Cache key
            for (int i = 0; i < 10000; i++)
            {
                listKeyNgan.Add("a" + i);
                listKeyDai.Add("ancaskcnjksanjkfjsaknfjsankjcnkasjcnkjsankfas" + i);
            }
            
            
            Stopwatch st = new Stopwatch();
            st.Start();
            for (int i = 0; i < 10000; i++)
            {
                dictKeyNgan[listKeyNgan[i]] = i;
            }
            st.Stop();
            Debug.Log("Set Key Ngắn cached: " + st.ElapsedMilliseconds);
            
            st.Restart();
            for (int i = 0; i < 10000; i++)
            {
                int value = dictKeyNgan[listKeyNgan[i]];
            }
            st.Stop();
            Debug.Log("Get Key Ngắn cached: " + st.ElapsedMilliseconds);
            
            
            
            st.Restart();
            for (int i = 0; i < 10000; i++)
            {
                dictKeyDai[listKeyDai[i]] = i;
            }
            st.Stop();
            Debug.Log("Set Key Dài cached: " + st.ElapsedMilliseconds);
            
            
            st.Restart();
            for (int i = 0; i < 10000; i++)
            {
                int value = dictKeyDai[listKeyDai[i]];
            }
            st.Stop();
            Debug.Log("Get Key Dài cached: " + st.ElapsedMilliseconds);
        }


        [ContextMenu("Test Multi Thread Read Write")]
        public void TestMultiThreadReadWrite()
        {
            Task task1 = new Task(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    if (i % 2 == 0)
                        SaveGame.Default["test_multi_thread"]["i" + i]["value nhe"].StringValue = "task_1 write: " + i;
                    else
                    {
                        string a = SaveGame.Default["test_multi_thread"]["i" + i]["value nhe"].StringValue;
                    }
                }
            });
            
            Task task2 = new Task(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    if (i % 2 == 0)
                        SaveGame.Default["test_multi_thread"]["i" + i]["value nhe"].StringValue = "task_2 write: " + i;
                    else
                    {
                        string a = SaveGame.Default["test_multi_thread"]["i" + i]["value nhe"].StringValue;
                    }
                }
            });

            task1.ContinueWith(t =>
            {
                if(t.IsFaulted)
                    Debug.LogException(t.Exception);
                else
                    Debug.Log("Task 1 completed");
            });
            
            task2.ContinueWith(t =>
            {
                if(t.IsFaulted)
                    Debug.LogException(t.Exception);
                else
                    Debug.Log("Task 2 completed");
            });
            
            task1.Start();
            task2.Start();
        }

        [ContextMenu("Test Big Value")]
        public void TestBigValue()
        {
            var b1 = SaveGame.Default["bigValue"].BigIntValue;
            var b2 = b1 + 1;
            SaveGame.Default["bigValue"].BigIntValue +=2;
            Debug.Log("b1: " + b1);
            Debug.Log("b2: " + b2);
            Debug.Log("b3: " + SaveGame.Default["bigValue"].BigIntValue);
        }
        
        [ContextMenu("Test String Value")]
        public void TestStringValue()
        {
            for (int i = 0; i < 10; i++)
            {
                SaveGame.Default["testNhe"]["order_" + i].StringValue = "Item_" + i;
            }

            for (int i = 0; i < 10; i++)
            {
                Debug.Log(SaveGame.Default["testNhe"]["order_" + i].StringValue);
            }
        }
        
        [ContextMenu("Test Double Value")]
        public void TestDoubleValue()
        {
            SaveGame.Default["testDouble"]["haha"].DoubleValue = Double.MaxValue - 100;
            SaveGame.Default["testFloat"]["haha"].FloatValue = 100000.1234f;
            SaveGame.Default["testDecimal"]["haha"].DecimalValue = decimal.MaxValue;
            
            Debug.Log("Double Value: " + SaveGame.Default["testDouble"]["haha"].DoubleValue);
            Debug.Log("Float Value: " + SaveGame.Default["testFloat"]["haha"].FloatValue);
            Debug.Log("Decimal Value: " + SaveGame.Default["testDecimal"]["haha"].DecimalValue);
            Debug.Log(SaveGame.persistentDataPath);
            
            //decimal d = decimal.Parse("1.2E+4");
            //Debug.Log(d);
            
            float f = float.Parse("1.e+4", CultureInfo.InvariantCulture);
            Debug.Log(f);
        }
        
        [ContextMenu("Test Convert Value")]
        public void TestConvertValue()
        {
            // var node = SaveGame.Default["testConvertValue"]["haha"];
            // node.StringValue = "99.5";
            //
            // Debug.Log(node.IntValue);
            // Debug.Log(node.FloatValue);
            // Debug.Log(node.LongValue);
            // Debug.Log(node.DoubleValue);
            // Debug.Log(node.DecimalValue);
            // Debug.Log(node.BoolValue);
            // Debug.Log(node.BigIntValue);


            SaveGame.Default["testConvertValue"]["objectToString"].IntValue = 2;
            Debug.Log(SaveGame.Default["testConvertValue"]["objectToString"].StringValue);
        }
        
        [ContextMenu("Test Default Value")]
        public void TestDefaultValue()
        {
            var node = SaveGame.Default["testDefaultValue"]["haha"];
            node.TimeValue = DateTime.Now.AddYears(-100);
            Debug.Log(node.DateTimeValue);
            Debug.Log(node.DateValue);
            Debug.Log(node.TimeValue);
        }

        private void GetValue(DateTime defaultValue = default)
        {
            
        }
        
        // private void GetValue()
        // {
        //     GetValue(DateTime.MinValue);
        // }

        private BigInteger CreateBigIntSuperLarge()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 5000; i++)
            {
                sb.Append("9");
            }

            return BigInteger.Parse(sb.ToString());
        }
        
        [ContextMenu("Test Cast Type")]
        private void TestCastType()
        {
            var node = SaveGame.Default["testCast"];
            
            node["int"].IntValue = int.MaxValue;
            node["long"].LongValue = long.MaxValue;
            node["float"].FloatValue = float.MinValue;
            node["double"].DoubleValue = double.MaxValue;
            node["decimal"].DecimalValue = decimal.MaxValue;
            node["bigInt"].BigIntValue = CreateBigIntSuperLarge();
            
            
            //Int
            Debug.Log(node["int"].IntValue);
            Debug.Log(node["int"].LongValue);
            Debug.Log(node["int"].FloatValue);
            Debug.Log(node["int"].DoubleValue);
            Debug.Log(node["int"].DecimalValue);
            Debug.Log(node["int"].BigIntValue);
            
            //Long
            Debug.Log(node["long"].IntValue);
            Debug.Log(node["long"].LongValue);
            Debug.Log(node["long"].FloatValue);
            Debug.Log(node["long"].DoubleValue);
            Debug.Log(node["long"].DecimalValue);
            Debug.Log(node["long"].BigIntValue);
            
            //Float
            Debug.Log(node["float"].IntValue);
            Debug.Log(node["float"].LongValue);
            Debug.Log(node["float"].FloatValue);
            Debug.Log(node["float"].DoubleValue);
            Debug.Log(node["float"].DecimalValue);
            Debug.Log(node["float"].BigIntValue);
            
            //Double
            Debug.Log(node["double"].IntValue);
            Debug.Log(node["double"].LongValue);
            Debug.Log(node["double"].FloatValue);
            Debug.Log(node["double"].DoubleValue);
            Debug.Log(node["double"].DecimalValue);
            Debug.Log(node["double"].BigIntValue);
            
            
            //Decimal
            Debug.Log(node["decimal"].IntValue);
            Debug.Log(node["decimal"].LongValue);
            Debug.Log(node["decimal"].FloatValue);
            Debug.Log(node["decimal"].DoubleValue);
            Debug.Log(node["decimal"].DecimalValue);
            Debug.Log(node["decimal"].BigIntValue);
            
            //BigInt
            Debug.Log(node["bigInt"].IntValue);
            Debug.Log(node["bigInt"].LongValue);
            Debug.Log(node["bigInt"].FloatValue);
            Debug.Log(node["bigInt"].DoubleValue);
            Debug.Log(node["bigInt"].DecimalValue);
            Debug.Log(node["bigInt"].BigIntValue);
        }

        
        [ContextMenu("Test Try Catch Cast Performance")]
        private void TestTryCatchCastPerformance()
        {
            float a = float.MaxValue;
            Stopwatch st = new Stopwatch();
            st.Start();
            for (int i = 0; i < 100000000; i++)
            {
                // try
                // {
                     int b = (int)a;
                // }
                // catch (Exception e)
                // {
                //     Debug.LogException(e);
                // }
            }
            st.Stop();
            
            Debug.Log(st.ElapsedMilliseconds);
        }

        [ContextMenu("Test Lock")]
        private void TestLock()
        {
            object A = new object();
            object B = A;

            Task task = new Task(() =>
            {
                lock (A)
                {
                    Thread.Sleep(5000);
                }
            });
            task.Start();


            StartCoroutine(Timeline());
            IEnumerator Timeline()
            {
                yield return new WaitForSecondsRealtime(1);
                lock (B)
                {
                    Debug.Log("Rất bth");
                }
            }
        }

        [ContextMenu("Test To Json")]
        private void TestToJson()
        {
            var a = SaveGame.Default["person"]["age"];
            string json = SaveGame.Default["person"].ToJson();
            Debug.Log(json);
        }

        [ContextMenu("Test Set Object Performance")]
        private void TestSetObjectPerformance()
        {
            //TestSetObjectPerformance_JsonUtility();
            //TestSetObjectPerformance_Literal();
            //TestSetObjectPerformance_Dictionary();
            //TestSetObjectPerformance_Reflection();
        }


        private void TestSetObjectPerformance_JsonUtility()
        {
            SGNode sgNode = SaveGame.Default["test_performance_json_utility"];
            sgNode.DeleteAllChild();
            FakeData data = new FakeData();
            
            Stopwatch st = new Stopwatch();
            st.Start();

            for (int i = 0; i < 100000; i++)
            {
                string json = JsonUtility.ToJson(data);
                var jsonNode = JSON.Parse(json);
                SetJsonNodeToSGNode(jsonNode, sgNode);
            }
            
            st.Stop();
            Debug.Log("Json Utility: " + st.ElapsedMilliseconds);
        }

        private void TestSetObjectPerformance_Literal()
        {
            SGNode sgNode = SaveGame.Default["test_performance_literal"];
            sgNode.DeleteAllChild();
            
            Stopwatch st = new Stopwatch();
            st.Start();

            for (int i = 0; i < 100000; i++)
            {
                var child1_Node = sgNode["child1"];
                child1_Node["childName1"].SetString("This is child name 1");
                child1_Node["childName2"].SetString("This is child name 2");
                child1_Node["childName3"].SetString("This is child name 3");
                
                var child2_Node = sgNode["child2"];
                child2_Node["childName1"].SetString("This is child name 1");
                child2_Node["childName2"].SetString("This is child name 2");
                child2_Node["childName3"].SetString("This is child name 3");
                
                var child3_Node = sgNode["child3"];
                child3_Node["childName1"].SetString("This is child name 1");
                child3_Node["childName2"].SetString("This is child name 2");
                child3_Node["childName3"].SetString("This is child name 3");
            }
            
            st.Stop();
            Debug.Log("Literal: " + st.ElapsedMilliseconds);
        }
        
        private void TestSetObjectPerformance_Dictionary()
        {
            Dictionary<string, Dictionary<string, EString>> dict = new Dictionary<string, Dictionary<string, EString>>();
            dict.Add("child1", new Dictionary<string, EString>());
            dict.Add("child2", new Dictionary<string, EString>());
            dict.Add("child3", new Dictionary<string, EString>());
            
            Stopwatch st = new Stopwatch();
            st.Start();

            for (int i = 0; i < 100000; i++)
            {
                var child1_Node = dict["child1"];
                child1_Node["childName1"] = new EString("This is child name 1");
                child1_Node["childName2"] = new EString("This is child name 2");
                child1_Node["childName3"] = new EString("This is child name 2");
                
                var child2_Node = dict["child2"];
                child2_Node["childName1"] = new EString("This is child name 1");
                child2_Node["childName2"] = new EString("This is child name 2");
                child2_Node["childName3"] = new EString("This is child name 3");
         
                
                var child3_Node = dict["child3"];
                child3_Node["childName1"] = new EString("This is child name 1");
                child3_Node["childName2"] = new EString("This is child name 2");
                child3_Node["childName3"] = new EString("This is child name 3");
            }
            
            st.Stop();
            Debug.Log("Dictionary: " + st.ElapsedMilliseconds);
        }

        private void SetJsonNodeToSGNode(JSONNode jsonNode, SGNode sgNode)
        {
            if (jsonNode.IsObject)
            {
                foreach (var childNode in jsonNode)
                {
                    SetJsonNodeToSGNode(childNode, sgNode[childNode.Key]);
                }
                
                return;
            }

            if (jsonNode.IsArray)
            {
                foreach (var childNode in jsonNode.Value)
                {
                    SetJsonNodeToSGNode(childNode, sgNode.AddChild());
                }
                return;
            }
            
            //sgNode.SetString(jsonNode.Value);
        }

        private void TestSetObjectPerformance_Reflection()
        {
            SGNode sgNode = SaveGame.Default["test_performance_reflection"];
            sgNode.DeleteAllChild();
            FakeData data = new FakeData();

            
            Stopwatch st = new Stopwatch();
            st.Start();
            
            for (int i = 0; i < 100000; i++)
            {
                SetObjectToSGNode(data, sgNode);
            }
            
            st.Stop();
            Debug.Log("Reflection: " + st.ElapsedMilliseconds);
        }

        
        Type stringType = typeof(string);
        private void SetObjectToSGNode(object obj, SGNode sgNode)
        {
            var objType = obj.GetType();
            if (objType.IsPrimitive || objType == stringType)
            {
                sgNode.SetString((string)obj);
                return;
            }

            
            if (objType.IsArray)
            {
                //Chưa code, tạm thời đang test sẽ ko có array
                return;
            }
            
            
            
            var fields = objType.GetFields();
            foreach (var fieldInfo in fields)
            {
                SetObjectToSGNode(fieldInfo.GetValue(obj), sgNode[fieldInfo.Name]);
            }
        }


        [ContextMenu("Test Set Vector")]
        public void TestSetVector()
        {
            SGNode rootNode = SaveGame.Default["test_vector"];
            
            // rootNode["v2"].SetVector2(new Vector2(1.5f, 2.6f));
            // rootNode["v2Int"].SetVector2Int(new Vector2Int(3, 4));
            //
            // rootNode["v3"].SetVector3(new Vector3(1.5f, 2.6f, 3.6f));
            // rootNode["v3Int"].SetVector3Int(new Vector3Int(4, 5, 6));
            
            
            
            Debug.Log("v2: " + rootNode["v2"].GetVector2());
            Debug.Log("v2Int: " + rootNode["v2Int"].GetVector2Int());
            
            Debug.Log("v2 -> v2Int: " + rootNode["v2"].GetVector2Int());
            Debug.Log("v2Int -> v2: " + rootNode["v2Int"].GetVector2());
            
            
            
            Debug.Log("v3: " + rootNode["v3"].GetVector3());
            Debug.Log("v3Int: " + rootNode["v3Int"].GetVector3Int());
            
            Debug.Log("v3 -> v3Int: " + rootNode["v3"].GetVector3Int());
            Debug.Log("v3Int -> v3: " + rootNode["v3Int"].GetVector3());
            
            
            
            Debug.Log("v2 -> v3: " + rootNode["v2"].GetVector3());
            Debug.Log("v2 -> v3Int: " + rootNode["v2"].GetVector3Int());
            Debug.Log("v2Int -> v3: " + rootNode["v2Int"].GetVector3());
            Debug.Log("v2Int -> v3Int: " + rootNode["v2Int"].GetVector3Int());
            
            Debug.Log("v3 -> v2: " + rootNode["v3"].GetVector2());
            Debug.Log("v3 -> v2Int: " + rootNode["v3"].GetVector2Int());
            Debug.Log("v3Int -> v2: " + rootNode["v3Int"].GetVector2());
            Debug.Log("v3Int -> v2Int: " + rootNode["v3Int"].GetVector2Int());
        }

        [ContextMenu("Test Set Object")]
        public void TestSetObject()
        {
            SGNode root = SaveGame.Default["test_set_object"];

            // decimal[] array = new decimal[] { 2, 5, 7 };
            // root["array"].SetObject(array);
            //
            // List<BigInteger> list = new List<BigInteger> { 3, 6, 8 };
            // root["list"].SetObject(list);
            //
            // Dictionary<string, bool> dict = new Dictionary<string, bool>() { { "ha1", true }, { "ha2", false } };
            // root["dict"].SetObject(dict);
            //
            // root["str"].SetObject("hahaha");
            root["obj"].SetObject(new FakeData());
            
            
            //root["test_xoa_last_child"].SetObject(new List<object>(){2, 3, 5, null, "haha"});

            // Dictionary<string, int> dict2 = new Dictionary<string, int>()
            // {
            //     {"say1", 2},
            //     {"say2", 4},
            //     {"say3", 6},
            // };
            // root["test_set_dict"].SetObject(dict2);
        }

        [ContextMenu("Test Get Object")]
        private void TestGetObject()
        {
            SGNode root = SaveGame.Default["test_get_object"];
            List<List<FakeData>> listFakeData = new()
            {
                new List<FakeData>() { new FakeData(), new FakeData() },
                new List<FakeData>() { new FakeData(), new FakeData() },
                new List<FakeData>() { new FakeData(), new FakeData() },
            };

            var childFakeData = listFakeData[0][1].listNested[0][0];
            childFakeData.childName1 = "Sơn tùng";
            
            root["mass_obj"].SetObject(listFakeData);
            
            var listFakeDataNew = root["mass_obj"].GetObject<List<List<FakeData>>>();
            Debug.Log(listFakeDataNew[0][1].listNested[0][0].childName1);
        }
    }
}


[System.Serializable]
public class FakeData
{
    public string name = "Lượng";
    public long age = 28;
    public ChildFakeData child1 = new ChildFakeData();
    public ChildFakeData child2 = new ChildFakeData();
    public ChildFakeData child3 = new ChildFakeData();
    
    public Dictionary<string, ChildFakeData> hello = new Dictionary<string, ChildFakeData>()
    {
        {"ha1", new ChildFakeData("oh", "my", "baby")},
        {"ha2", new ChildFakeData("shit", "man", "woa")}
    };
    
    [SerializeField] private List<string> listPrivate1 = new List<string>(){"kaka", "hihi"};
    private List<string> listPrivate2 = new List<string>(){"kaka", "hihi"};
    
    public List<string> listPublic1 = new List<string>(){"woa", "moa"}; 
    [NonSerialized] public List<string> listPublic2 = new List<string>(){"woa", "moa"};

    public List<List<ChildFakeData>> listNested = new List<List<ChildFakeData>>()
    {
        new List<ChildFakeData>(){new ChildFakeData(), new ChildFakeData()},
        new List<ChildFakeData>(){new ChildFakeData(), new ChildFakeData()},
        new List<ChildFakeData>(){new ChildFakeData(), new ChildFakeData()},
        new List<ChildFakeData>(){new ChildFakeData(), new ChildFakeData()},
        new List<ChildFakeData>(){new ChildFakeData(), new ChildFakeData()},
    };
}

[System.Serializable]
public class ChildFakeData
{
    public string childName1 = "This is child name 1";
    public string childName2 = "This is child name 2";
    public string childName3 = "This is child name 3";


    public ChildFakeData()
    {
        
    }
    
    public ChildFakeData(string childName1, string childName2, string childName3)
    {
        this.childName1 = childName1;
        this.childName2 = childName2;
        this.childName3 = childName3;
    }
}

[System.Serializable]
public class ClA
{
    public ClB[] mang = new[] { new ClB(), new ClB()};
}

[System.Serializable]
public class ClB
{
    public string name = "Lượng";
}