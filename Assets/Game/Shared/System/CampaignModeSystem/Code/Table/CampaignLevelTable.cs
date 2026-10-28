using System.Linq;
using UnityEngine;
using ZeroX.DataTableSystem.SoTableSystem;

namespace Game.Shared.CampaignModeSystem
{
    [CreateAssetMenu(menuName = "Database/Campaign Level/Table")]
    public class CampaignLevelTable : SoTableOneId<CampaignLevelTable, CampaignLevelRow, int>
    {
        [Header("Editor")]
        [SerializeField] private int loopToLevel = 1000;
        [SerializeField] private int loopRandomMinLevel = 3;
        [SerializeField] private int loopRandomMaxLevel = 25;
        
        
        
        
        
        protected override string ShortMainPath => "Campaign Level Table";

        public static int MaxLevel => Rows[Rows.Count - 1].Level;

        protected override int GetRowId(CampaignLevelRow row)
        {
            return row.Level;
        }

        public static LevelDifficult GetLevelDifficult(int level)
        {
            var row = GetRowByIdWithLog(level);
            if (row == null)
                return LevelDifficult.Normal;
            
            return row.Difficult;
        }

        // public static LevelData GetLevelData(int level)
        // {
        //     var row = GetRowByIdWithLog(level);
        //     if (row == null)
        //         return null;
        //
        //     return row.GenerateLevelData();
        // }
        
#if UNITY_EDITOR
        [ContextMenu("Editor - Loop Level")]
        private void Editor_LoopLevel()
        {
            UnityEditor.Undo.RecordObject(this, "Loop Level");
            
            int startLevel = RawRowsI.Max(r => r.Level);
            int endLevel = loopToLevel;
            
            if(startLevel == endLevel)
                return;

            if (startLevel < 3)
            {
                Debug.LogError("Không thể loop level vì ít level quá");
                return;
            }

            int preLinkLevel1 = 0;
            int preLinkLevel2 = 0;
            for (int level = startLevel; level <= endLevel; level++)
            {
                int linkLevel = Editor_RandomLevel(preLinkLevel1, preLinkLevel2);
                preLinkLevel2 = preLinkLevel1;
                preLinkLevel1 = linkLevel;
                
                var row = new CampaignLevelRow();
                CampaignLevelRow.EditorSetRowData(row, level, linkLevel);
                RawRowsI.Add(row);
            }
            
            
            
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private int Editor_RandomLevel(int preLevel1, int preLevel2)
        {
            int level = preLevel1;
            while (level == preLevel1 || level == preLevel2)
            {
                level = Random.Range(loopRandomMinLevel, loopRandomMaxLevel + 1);
            }

            return level;
        }
#endif
    }
}