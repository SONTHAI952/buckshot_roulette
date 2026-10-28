using System;
using System.Collections.Generic;

namespace Game.Primary.GamePlaySystem.Rules
{
    /// <summary>
    /// StageData đã được resolve thành con số cụ thể (charge, item pool dạng enum).
    /// </summary>
    public class StageRules
    {
        public string name;
        public int startCharges;
        public int minItemsPerLoad;
        public int maxItemsPerLoad;
        public List<ItemId> itemPool = new List<ItemId>();
        public List<LoadData> fixedLoads = new List<LoadData>();
        public int minShells;
        public int maxShells;
        public bool fatal;
        public int healingDisabledAtOrBelow;
        public int revivalPenaltyIfWonPrevious;
        public int revivalPenaltyIfLostPrevious;

        //Giả định chưa xác minh với bản gốc, để config được
        public bool playerStartsEachLoad = true;
        public bool resetEffectsOnNewLoad = true;

        public bool HasItems => maxItemsPerLoad > 0 && itemPool.Count > 0;

        public static StageRules FromData(StageData data, GameRng rng)
        {
            var rules = new StageRules
            {
                name = data.name,
                startCharges = Math.Max(1, rng.RangeInclusive(data.minCharges, Math.Max(data.minCharges, data.maxCharges))),
                minItemsPerLoad = Math.Max(0, data.minItemsPerLoad),
                maxItemsPerLoad = Math.Max(data.minItemsPerLoad, data.maxItemsPerLoad),
                minShells = Math.Max(2, data.minShells),
                maxShells = Math.Max(Math.Max(2, data.minShells), data.maxShells),
                fatal = data.fatal,
                healingDisabledAtOrBelow = data.healingDisabledAtOrBelow,
                revivalPenaltyIfWonPrevious = data.revivalPenaltyIfWonPrevious,
                revivalPenaltyIfLostPrevious = data.revivalPenaltyIfLostPrevious
            };

            foreach (var itemName in data.itemPool)
            {
                if (Enum.TryParse(itemName, true, out ItemId itemId) && itemId != ItemId.None)
                    rules.itemPool.Add(itemId);
            }

            foreach (var load in data.fixedLoads)
            {
                if (load.live + load.blank > 0)
                    rules.fixedLoads.Add(new LoadData(load.live, load.blank));
            }

            return rules;
        }
    }
}
