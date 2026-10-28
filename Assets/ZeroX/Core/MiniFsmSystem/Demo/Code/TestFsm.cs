using System.Collections.Generic;

namespace ZeroX.MiniFsmSystem.Demo
{
    [System.Serializable]
    public class TestFsm : MiniFsm<TestFsmId>
    {
        public int a = 12;
        public int b = 100;
        public Abc c = new Abc();
        public List<Abc> d = new List<Abc>();
        
        
        [System.Serializable]
        public class Abc
        {
            public string e = "Xin chào";
            public int f = 12;
            public int g = 100;
            public List<string> h = new List<string>();
        }
    }
}