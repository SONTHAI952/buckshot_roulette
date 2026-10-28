using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.DataTableSystem.Demo
{
    public abstract class TestRowBase
    {
        public string id;
        public TestEnemyType type;
        
        public int powerLevel;
        public TestRange range;
        public List<string> listReward;
        
        [TextArea]
        public string des1;
        
        [TextArea]
        public string des2;


        private string priField = "ds";
        [NonSerialized] public string pubFieldButNonSer = "caca";

        public TestData testData1 = new TestData();
        private TestData testData2 = new TestData();
    }
    
    [System.Serializable]
    public class TestRange
    {
        public float min;
        public float max;
    }
    
    [System.Serializable]
    public class TestData
    {
        public string name = "Abc";
        public int age = 25;
    }
}