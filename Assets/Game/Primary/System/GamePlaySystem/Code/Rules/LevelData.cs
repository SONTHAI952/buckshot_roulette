using System;
using System.Collections.Generic;

namespace Game.Primary.GamePlaySystem.Rules
{
    /// <summary>
    /// Một "level" = định nghĩa một run (Story hoặc Double or Nothing).
    /// Lưu dạng json ở Resources/Levels/level_{n}.json, parse bằng JsonUtility.
    /// </summary>
    [Serializable]
    public class LevelData
    {
        public string mode = "Story";             //Story | DoubleOrNothing
        public string displayName = "Story";
        public List<StageData> stages = new List<StageData>();
        public EconomyData economy = new EconomyData();

        public GameMode GetMode()
        {
            return Enum.TryParse(mode, true, out GameMode result) ? result : GameMode.Story;
        }
    }

    [Serializable]
    public class StageData
    {
        public string name = "Stage";

        //Charge: cả hai bên nhận cùng giá trị, random trong [minCharges, maxCharges]
        public int minCharges = 2;
        public int maxCharges = 2;

        //Số item mỗi bên nhận trước mỗi load, random trong [minItemsPerLoad, maxItemsPerLoad]
        public int minItemsPerLoad = 0;
        public int maxItemsPerLoad = 0;
        public List<string> itemPool = new List<string>();

        //Các load cố định (theo thứ tự) trước khi chuyển sang load random
        public List<LoadData> fixedLoads = new List<LoadData>();
        public int minShells = 2;
        public int maxShells = 8;

        //Stage cuối: thua là hết run, không có safe word
        public bool fatal = false;

        //Khi charge <= giá trị này thì khoá hồi máu (0 = không bao giờ khoá)
        public int healingDisabledAtOrBelow = 0;

        //Phạt tiền khi dùng safe word ở stage này, phụ thuộc kết quả stage trước
        public int revivalPenaltyIfWonPrevious = 0;
        public int revivalPenaltyIfLostPrevious = 0;
    }

    [Serializable]
    public class LoadData
    {
        public int live;
        public int blank;

        public LoadData()
        {
        }

        public LoadData(int live, int blank)
        {
            this.live = live;
            this.blank = blank;
        }
    }

    [Serializable]
    public class EconomyData
    {
        //Story: tiền tối đa, bị trừ dần
        public int baseCash = 70000;
        public int healItemPenalty = 220;
        public int ejectItemPenalty = 495;

        //Double or Nothing: tiền mỗi set 3 stage, giảm theo thời gian
        public int setBaseCash = 30000;
        public float cashDecayPerSecond = 25f;
        public int setMinCash = 5000;
        public float continueMultiplier = 2f;
    }
}
