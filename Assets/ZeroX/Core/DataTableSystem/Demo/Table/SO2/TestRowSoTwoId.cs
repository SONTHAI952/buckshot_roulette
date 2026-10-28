using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.DataTableSystem.Demo
{
    [System.Serializable]
    public class TestRowSoTwoId : ScriptableObject
    {
        public TestEnemyType type;
        public string id;
        
        
        public int powerLevel;
        public TestRange range;
        public List<string> listReward;
        
        [TextArea]
        public string des1;
        
        [TextArea]
        public string des2;
    }
}