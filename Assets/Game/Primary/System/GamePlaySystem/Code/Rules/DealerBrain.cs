namespace Game.Primary.GamePlaySystem.Rules
{
    public enum DealerActionType
    {
        UseItem,
        Fire
    }

    public struct DealerDecision
    {
        public DealerActionType type;
        public int slot;
        public int stealSlot;
        public int targetId;

        public static DealerDecision Use(int slot, int stealSlot = -1)
        {
            return new DealerDecision { type = DealerActionType.UseItem, slot = slot, stealSlot = stealSlot, targetId = -1 };
        }

        public static DealerDecision Shoot(int targetId)
        {
            return new DealerDecision { type = DealerActionType.Fire, slot = -1, stealSlot = -1, targetId = targetId };
        }

        public override string ToString()
        {
            return type == DealerActionType.Fire ? $"Fire -> {targetId}" : $"Use slot {slot} (steal {stealSlot})";
        }
    }

    /// <summary>
    /// AI của Mistress (Dealer). Chỉ ra quyết định dựa trên DealerObservation, không đọc thứ tự đạn bị ẩn.
    /// Gọi lặp lại cho tới khi trả về Fire. allowItems = false để buộc bắn (chặn vòng lặp).
    /// </summary>
    public static class DealerBrain
    {
        public static DealerDecision Decide(DealerObservation obs, GameRng rng, bool allowItems = true)
        {
            double pLive = obs.ProbCurrentLive();
            bool knowsLive = pLive >= 0.999;
            bool knowsBlank = pLive <= 0.001;
            bool knows = knowsLive || knowsBlank;
            int self = obs.selfId;
            int opponent = obs.OpponentId;

            if (allowItems)
            {
                int slot;

                //1. Đã chắc thắng bằng một phát bắn: không cần lãng phí item, chỉ tăng sát thương nếu cần
                if (knowsLive && obs.opponentCharges <= 1)
                    return DealerDecision.Shoot(opponent);

                //2. Chưa biết viên hiện tại -> soi
                if (knows == false && (slot = obs.FindUsable(ItemId.Peek)) >= 0)
                    return DealerDecision.Use(slot);

                //3. Hồi máu khi đang thiếu
                if (obs.selfCharges < obs.selfMaxCharges && (slot = obs.FindUsable(ItemId.Heal)) >= 0)
                    return DealerDecision.Use(slot);

                //4. Thuốc may rủi: chỉ khi thiếu nhiều máu và không có nguy cơ tự hạ
                if (obs.selfHealingDisabled == false && obs.selfCharges >= 2 && obs.selfCharges <= obs.selfMaxCharges - 2
                    && (slot = obs.FindUsable(ItemId.GambleHeal)) >= 0)
                    return DealerDecision.Use(slot);

                //5. Cướp item có giá trị
                if ((slot = obs.FindUsable(ItemId.Steal)) >= 0)
                {
                    int target = -1;
                    if (knows == false)
                        target = obs.FindStealable(ItemId.Peek);
                    if (target < 0 && obs.selfCharges < obs.selfMaxCharges && obs.selfHealingDisabled == false)
                        target = obs.FindStealable(ItemId.Heal);
                    if (target < 0 && pLive >= 0.5 && obs.boosted == false)
                        target = obs.FindStealable(ItemId.DoubleDamage);
                    if (target < 0 && pLive >= 0.5)
                        target = obs.FindStealable(ItemId.SkipTurn);

                    if (target >= 0)
                        return DealerDecision.Use(slot, target);
                }

                //6. Chưa biết: dùng xúc xắc để loại trừ
                if (knows == false && obs.shellsRemaining >= 3 && (slot = obs.FindUsable(ItemId.RevealRandom)) >= 0)
                    return DealerDecision.Use(slot);

                //7. Biết là blank: đảo thành live nếu có, không thì tự bắn để giữ lượt
                if (knowsBlank)
                {
                    if ((slot = obs.FindUsable(ItemId.Invert)) >= 0)
                        return DealerDecision.Use(slot);

                    return DealerDecision.Shoot(self);
                }

                //8. Biết là live: còng + roi rồi bắn
                if (knowsLive)
                {
                    if ((slot = obs.FindUsable(ItemId.SkipTurn)) >= 0)
                        return DealerDecision.Use(slot);

                    if (obs.boosted == false && (slot = obs.FindUsable(ItemId.DoubleDamage)) >= 0)
                        return DealerDecision.Use(slot);

                    return DealerDecision.Shoot(opponent);
                }

                //9. Không chắc chắn
                if (pLive < 0.5 && (slot = obs.FindUsable(ItemId.Invert)) >= 0)
                    return DealerDecision.Use(slot);

                if (pLive >= 0.4 && pLive <= 0.6 && obs.shellsRemaining >= 2 && (slot = obs.FindUsable(ItemId.Eject)) >= 0)
                    return DealerDecision.Use(slot);

                if (pLive > 0.5)
                {
                    if ((slot = obs.FindUsable(ItemId.SkipTurn)) >= 0)
                        return DealerDecision.Use(slot);

                    if (pLive >= 0.7 && obs.boosted == false && (slot = obs.FindUsable(ItemId.DoubleDamage)) >= 0)
                        return DealerDecision.Use(slot);
                }
            }

            //Quyết định bắn
            if (knowsLive)
                return DealerDecision.Shoot(opponent);
            if (knowsBlank)
                return DealerDecision.Shoot(self);
            if (pLive > 0.5)
                return DealerDecision.Shoot(opponent);
            if (pLive < 0.5)
                return DealerDecision.Shoot(self);

            return DealerDecision.Shoot(rng.Chance(0.5) ? opponent : self);
        }
    }
}
