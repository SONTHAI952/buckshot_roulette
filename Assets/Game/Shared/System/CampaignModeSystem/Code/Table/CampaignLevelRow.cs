using System;
using UnityEngine;

namespace Game.Shared.CampaignModeSystem
{
    [System.Serializable]
    public class CampaignLevelRow
    {
        [SerializeField] private int level;
        [SerializeField] private int linkLevelData;
        
        
        //Temp
        [NonSerialized] private LevelDifficult? difficultCached;
        
        
        
        public int Level => level;


        private string GenerateLevelDataPath()
        {
            return "Database/Campaign Level/level_" + linkLevelData;
        }

        public LevelDifficult Difficult => LevelDifficult.Normal;
        
        
        public static void EditorSetRowData(CampaignLevelRow row, int level, int linkLevelData)
        {
            row.level = level;
            row.linkLevelData = linkLevelData;
        }
    }
}