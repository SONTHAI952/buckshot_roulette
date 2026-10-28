using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
using ZeroX.DataTableSystem.Demo;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace ZeroX.JsonSystem.Demo
{
    public class DemoJsonSystem : MonoBehaviour
    {
        [TextArea(5, 10)] public string input = "";
        [TextArea(5, 10)] public string output = "";
        public DataTest dataTestInput = new DataTest();
        public DataTest dataTestOutput = new DataTest();
        public InfoTest infoTest = new InfoTest();

        [Header("List")]
        public List<DataTest> listDataTestInput = new List<DataTest>();
        public List<DataTest> listDataTestOutput = new List<DataTest>();
        
        
        [Header("Built-in")]
        public DataTestBI dataTestBIInput = new DataTestBI();
        public DataTestBI dataTestBIOutput = new DataTestBI();
        public AnimationCurve animationCurveInput = new AnimationCurve();
        public AnimationCurve animationCurveOutput = new AnimationCurve();
        
        [Header("Struct")]
        public InfoTest infoTestInput = new InfoTest();
        public InfoTest infoTestOutput = new InfoTest();
        
        [Header("Misc")]
        public TestDataA testDataAInput = new TestDataA(); 
        public TestDataA testDataAOutput = new TestDataA(); 
        
        
        private void Start()
        {
            Vector3 a;
            a.x = 2;
        }
        

        [ContextMenu("Test Performance String To Node")]
        private void TestPerformance_StringToNode()
        {
            int loop = 10000;
            string jsonString = JsonUtility.ToJson(dataTestInput);
            
            //Simple Json
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < loop; i++)
            {
                JSON.Parse(jsonString);
            }
            stopwatch.Stop();
            Debug.Log("Simple Json: " + stopwatch.ElapsedMilliseconds);
            
            
            //ZJson
            stopwatch.Restart();
            for (int i = 0; i < loop; i++)
            {
                
                ZJson_StringToNode.Parse(jsonString);
            }
            stopwatch.Stop();
            Debug.Log("ZJson: " + stopwatch.ElapsedMilliseconds);
            
            
            //JsonUtility
            // stopwatch.Restart();
            // for (int i = 0; i < loop; i++)
            // {
            //     JsonUtility.FromJson<DataTest>(input);
            // }
            // stopwatch.Stop();
            // Debug.Log("JsonUtility: " + stopwatch.ElapsedMilliseconds);
            
            
            //For bình thường
            // stopwatch.Restart();
            // for (int i = 0; i < loop; i++)
            // {
            //     FakeJson(input);
            // }
            // stopwatch.Stop();
            // Debug.Log("For bình thường: " + stopwatch.ElapsedMilliseconds);
        }


        [ContextMenu("Test Performance Node To String")]
        private void TestPerformance_NodeToString()
        {
            int loop = 10000;
            
            string jsonString = JsonUtility.ToJson(dataTestInput);
            JSONNode simpleJsonNode = JSON.Parse(jsonString);
            ZJsonNode zJsonNode = ZJson_StringToNode.Parse(jsonString);
            
            
            //Simple Json
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < loop; i++)
            {
                var a = simpleJsonNode.ToString();
            }
            stopwatch.Stop();
            Debug.Log("Simple Json: " + stopwatch.ElapsedMilliseconds);
            
            
            //ZJson
            stopwatch.Restart();
            for (int i = 0; i < loop; i++)
            {
                var a = zJsonNode.ToJson();
            }
            stopwatch.Stop();
            Debug.Log("ZJson: " + stopwatch.ElapsedMilliseconds);
        }

        [ContextMenu("Test Performance String To Object")]
        private void TestPerformance_StringToObject()
        {
            string jsonString = JsonUtility.ToJson(dataTestInput);
            int loop = 10000;
            
            //JsonUtility
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < loop; i++)
            {
                var a = JsonUtility.FromJson<DataTest>(jsonString);
            }
            stopwatch.Stop();
            Debug.Log("Json Utility: " + stopwatch.ElapsedMilliseconds);
            
            
            //ZJSon
            stopwatch.Restart();
            for (int i = 0; i < loop; i++)
            {
                var a = ZJson.JsonToObject<DataTest>(jsonString);
            }
            stopwatch.Stop();
            Debug.Log("ZJson: " + stopwatch.ElapsedMilliseconds);
        }
        
        [ContextMenu("Test Performance Node To Object")]
        private void TestPerformance_NodeToObject()
        {
            string jsonString = JsonUtility.ToJson(dataTestInput);
            ZJsonObject jsonNode = (ZJsonObject)ZJson.JsonToNode(jsonString);
            int loop = 10000;
            
            //JsonUtility
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < loop; i++)
            {
                var a = JsonUtility.FromJson<DataTest>(jsonString);
            }
            stopwatch.Stop();
            Debug.Log("Json Utility (StringToObject): " + stopwatch.ElapsedMilliseconds);
            
            
            //ZJSon
            stopwatch.Restart();
            for (int i = 0; i < loop; i++)
            {
                var a = jsonNode.ToObject<DataTest>();
            }
            stopwatch.Stop();
            Debug.Log("ZJson: " + stopwatch.ElapsedMilliseconds);
        }
        
        [ContextMenu("Test Performance Object To String")]
        private void TestPerformance_ObjectToString()
        {
            var data = new Abc();
            int loop = 10000;
            
            //JsonUtility
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < loop; i++)
            {
                var a = JsonUtility.ToJson(data);
            }
            stopwatch.Stop();
            Debug.Log("Json Utility (ObjectToString): " + stopwatch.ElapsedMilliseconds);
            
            //ZJSon
            stopwatch.Restart();
            for (int i = 0; i < loop; i++)
            {
                var a = ZJson.ObjectToJson(data);
            }
            stopwatch.Stop();
            Debug.Log("ZJson: " + stopwatch.ElapsedMilliseconds);
        }


        [ContextMenu("Test Work Normally")]
        private void TestWorkNormally()
        {
            input = ZJson.ObjectToJson(dataTestInput);
            ZJsonNode node = ZJson.JsonToNode(input);
            
            output = node.ToJson();
            dataTestOutput = node.ToObject<DataTest>();
            
            
            dataTestOutput = (DataTest)ZJson_StringToObject.ParseStringOverwriteObject(input, typeof(DataTest), null);
        }

        [ContextMenu("Test Multi Thread")]
        private void TestMultiThread()
        {
            Debug.Log("Start Parse Multithread");
            int completed = 0;
            string json = ZJson.ObjectToJson(dataTestInput);
            for (int i = 0; i < 10000; i++)
            {
                Task task = new Task(() =>
                {
                    ZJson.JsonToObject<DataTest>(json);
                    completed++;
                });
                
                task.Start();
            }


            StartCoroutine(Timeline());
            IEnumerator Timeline()
            {
                yield return new WaitUntil(() => completed >= 1000);
                Debug.Log("Parse Multithread Completed");
            }
        }
        
        [ContextMenu("Test Parse")]
        private void TestParse()
        {
            TestDataA dataA = new TestDataA();
            var node = ZJson.ObjectToNode(dataA).AsObject;
            //node["testDataB"] = null;
            node.Remove("testDataB");

            node.OverwriteObject(dataA);
            
            Debug.Log(dataA.testDataB == null);
        }

        [System.Serializable]
        private class Abc
        {
            public string name = "Lượng";
            public int age;
            //public Vector3 pos = new Vector3(1, 2, 3);
            public Info[] arrayInfo = new Info[]{
                         new Info(),
                         new Info(),
                         new Info(),
                     };
            public TestEnum TestEnum = TestEnum.Ngay;
            
            public List<Info> listInfo = new List<Info>()
            {
                new Info(),
                new Info(),
                new Info(),
            };
            
            public Dictionary<string, object> dict = new Dictionary<string, object>()
            {
                {"k1", "haha\nhihi"},
                {"k2", 1.2m},
                {"k3", new Info()},
                {"k4", "end"},
            };
            // public decimal height = 16.9m;
        }
        
        //[System.Serializable]
        private class Info
        {
            public string interest = "Games";
            public int exp = 999;

            public Info()
            {
            }

            public Info(string interest, int exp)
            {
                this.interest = interest;
                this.exp = exp;
            }
        }

        [System.Serializable]
        private class Info2 : Info
        {
            public string hate = "Wash plate";
        }
 

        private void FakeJson(string json)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < json.Length; i++)
            {
                sb.Append(json[i]);
            }
        }
    }

    [System.Serializable]
    public class DataTest
    {
        public string name;
        public int haha;
        public bool flag;
        public double numberDouble = 5;
        public decimal NumberDecimal = 10;
        public TestEnum testEnum = TestEnum.XinChao;
        public Vector3 pos;

        public InfoTest info = new InfoTest();
        //public List<InfoTest> listInfoTest = new List<InfoTest>();
        //public InfoTest[] arrayInfoTest = new  InfoTest[2];
        //
        // public Dictionary<string, InfoTest> infoTest = new Dictionary<string, InfoTest>()
        // {
        //     {"el_1", new InfoTest(){name = "Info 1", number = 1}},
        //     {"el_2", new InfoTest(){name = "Info 2", number = 1}},
        //     {"el_3", new InfoTest(){name = "Info 3", number = 1}},
        // };

        //public List<Data2> listData2 = new List<Data2>();
    }

    interface ITestSet
    {
        public void ChangeName(string newName);

        public string GetName();
    }
    
    [System.Serializable]
    public struct InfoTest
    {
        public string name;
        public int number;
        private int kaka;

        public Vector3 pos;
        public Quaternion rot;
    }

    public enum TestEnum
    {
        XinChao,
        CacBan,
        Mot,
        Ngay,
        Dep,
        Troi
    }
    
    [System.Serializable]
    public class Data2
    {
        public string name;
        public int number;
        public List<Data3> listData3 = new List<Data3>();
    }
    
    public class Data3
    {
        public string hm;
        public int age;
    }

    [System.Serializable]
    public class DataTestBI
    {
        // Vector
        public Vector2 vector2;
        public Vector3 vector3;
        public Vector4 vector4;
        public Vector2Int vector2Int;
        public Vector3Int vector3Int;

        // Rotation
        public Quaternion quaternion;

        // Color
        public Color color;
        public Color32 color32;

        // Math / Geometry
        public Rect rect;
        public RectInt rectInt;
        public Bounds bounds;
        public BoundsInt boundsInt;

        // Other structs
        public LayerMask layerMask;
        public AnimationCurve animationCurve;
        public Gradient gradient;
        public Matrix4x4 matrix4x4;
        public Hash128 hash128;
    }


    [System.Serializable]
    public class TestDataA
    {
        public string name;
        public Vector3Int pos;
        public TestDataB testDataB = new TestDataB();
    }

    [System.Serializable]
    public class TestDataB
    {
        public string aha;
        public int bhe;
    }
}