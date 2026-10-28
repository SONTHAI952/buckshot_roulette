using System.Collections.Generic;

namespace Game.Primary.GamePlaySystem.Rules
{
    /// <summary>
    /// Snapshot thông tin hợp lệ của một bên (toDealerObservation trong Gameplay.md).
    /// Chỉ có số lượng công khai + những viên chính bên đó đã soi được.
    /// </summary>
    public class DealerObservation
    {
        public int selfId;
        public int selfCharges;
        public int selfMaxCharges;
        public bool selfHealingDisabled;

        public int opponentCharges;
        public int opponentMaxCharges;
        public bool opponentCuffed;
        public bool opponentCuffImmune;

        public readonly ItemId[] selfSlots = new ItemId[RuleConst.InventorySlots];
        public readonly ItemId[] opponentSlots = new ItemId[RuleConst.InventorySlots];
        public readonly bool[] usableSlots = new bool[RuleConst.InventorySlots];
        public readonly List<int> stealableOpponentSlots = new List<int>();

        public int shellsRemaining;
        public int publicLiveRemaining;     //Đếm theo loại lúc bày ra bàn
        public int publicBlankRemaining;
        public bool currentInverted;
        public bool boosted;
        public bool fatalStage;

        //offset -> loại, chỉ những viên mà bên này đã biết
        public readonly Dictionary<int, ShellType> knownShells = new Dictionary<int, ShellType>();

        public int OpponentId => PlayerIds.Opponent(selfId);

        public bool KnowsCurrent => knownShells.ContainsKey(0);

        /// <summary>
        /// Xác suất viên hiện tại là Live dựa trên thông tin công khai + những gì đã biết.
        /// Viên phía sau không thể bị Inverter tác động nên loại thật = loại ban đầu.
        /// </summary>
        public double ProbCurrentLive()
        {
            if (shellsRemaining <= 0)
                return 0;

            if (knownShells.TryGetValue(0, out var currentType))
                return currentType == ShellType.Live ? 1 : 0;

            int live = publicLiveRemaining;
            int unknown = shellsRemaining;
            foreach (var pair in knownShells)
            {
                if (pair.Key <= 0)
                    continue;

                unknown--;
                if (pair.Value == ShellType.Live)
                    live--;
            }

            if (unknown <= 0)
                return 0;

            double probOriginalLive = (double)System.Math.Max(0, live) / unknown;
            if (probOriginalLive > 1)
                probOriginalLive = 1;

            return currentInverted ? 1 - probOriginalLive : probOriginalLive;
        }

        public int FindUsable(ItemId itemId)
        {
            for (int i = 0; i < selfSlots.Length; i++)
            {
                if (selfSlots[i] == itemId && usableSlots[i])
                    return i;
            }

            return -1;
        }

        public int FindStealable(ItemId itemId)
        {
            foreach (var slot in stealableOpponentSlots)
            {
                if (opponentSlots[slot] == itemId)
                    return slot;
            }

            return -1;
        }
    }
}
