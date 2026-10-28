using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.DataTableSystem.Demo
{
    [System.Serializable]
    public abstract class TestRowSoBase : ScriptableObject
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
    }
}