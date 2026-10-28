using System;
using Game.Primary.GamePlaySystem.Rules;

namespace Game.Primary.GamePlaySystem
{
    public enum PlayerActionType
    {
        UseItem,
        Fire,
        ForfeitSteal
    }

    public struct PlayerAction
    {
        public PlayerActionType type;
        public int slot;
        public int stealSlot;
        public int targetId;

        public static PlayerAction Use(int slot, int stealSlot = -1) => new PlayerAction { type = PlayerActionType.UseItem, slot = slot, stealSlot = stealSlot, targetId = -1 };
        public static PlayerAction Fire(int targetId) => new PlayerAction { type = PlayerActionType.Fire, slot = -1, stealSlot = -1, targetId = targetId };
        public static PlayerAction Forfeit(int slot) => new PlayerAction { type = PlayerActionType.ForfeitSteal, slot = slot, stealSlot = -1, targetId = -1 };
    }

    /// <summary>
    /// Kết quả một run, GamePlayCore bắn ra cho CampaignMode hiển thị popup win/lose
    /// </summary>
    public class RunResult
    {
        public GameMode mode;
        public bool won;
        public int cash;
        public int stagesCleared;
        public int setsCleared;
        public RunStats stats;
        public EconomyData economy;
    }

    /// <summary>
    /// Double or Nothing: sau mỗi set 3 stage, hỏi người chơi Cash Out hay Continue
    /// </summary>
    public class DoubleOrNothingOffer
    {
        public int setIndex;
        public int setCash;
        public int atRiskCash;
        public int nextSetMultiplier;

        private Action<bool> resolve;
        public bool IsResolved { get; private set; }

        public DoubleOrNothingOffer(Action<bool> resolve)
        {
            this.resolve = resolve;
        }

        /// <param name="continuePlaying">true = Double (chơi tiếp), false = Cash Out</param>
        public void Resolve(bool continuePlaying)
        {
            if (IsResolved)
                return;

            IsResolved = true;
            resolve?.Invoke(continuePlaying);
            resolve = null;
        }
    }
}
