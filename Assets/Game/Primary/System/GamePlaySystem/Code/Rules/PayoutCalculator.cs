using System;

namespace Game.Primary.GamePlaySystem.Rules
{
    /// <summary>
    /// Tính tiền thưởng (calculatePayout trong Gameplay.md). UI không tự cộng trừ tiền.
    /// </summary>
    public static class PayoutCalculator
    {
        public static int CalculateStory(EconomyData economy, RunStats stats)
        {
            int cash = economy.baseCash
                       - stats.revivalPenalty
                       - stats.healsUsed * economy.healItemPenalty
                       - stats.ejectItemsUsed * economy.ejectItemPenalty;

            return Math.Max(0, cash);
        }

        /// <summary>
        /// Tiền của một set 3 stage trong Double or Nothing. setIndex bắt đầu từ 0, mỗi lần Continue nhân thêm continueMultiplier.
        /// </summary>
        public static int CalculateDoubleOrNothingSet(EconomyData economy, int healsUsed, int ejectItemsUsed, float elapsedSeconds, int setIndex)
        {
            double cash = economy.setBaseCash
                          - elapsedSeconds * economy.cashDecayPerSecond
                          - healsUsed * economy.healItemPenalty
                          - ejectItemsUsed * economy.ejectItemPenalty;

            cash = Math.Max(economy.setMinCash, cash);
            cash *= Math.Pow(economy.continueMultiplier, Math.Max(0, setIndex));
            return (int)Math.Round(cash);
        }
    }
}
