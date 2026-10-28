using System.Collections.Generic;

namespace Game.Primary.GamePlaySystem.Rules
{
    /// <summary>
    /// Kết quả của một hành động trong luật chơi. View đọc lần lượt để phát animation/sfx.
    /// Animation không bao giờ là nguồn sự thật: state đã được cập nhật trước khi event được trả về.
    /// </summary>
    public abstract class RuleEvent
    {
    }

    public class StageStartedEvent : RuleEvent
    {
        public int stageIndex;
        public string stageName;
        public int charges;
        public bool fatal;
    }

    public class LoadStartedEvent : RuleEvent
    {
        public int loadIndex;
        public int live;
        public int blank;
        public int firstPlayerId;
    }

    public class ItemsDealtEvent : RuleEvent
    {
        public int playerId;
        public readonly List<int> slots = new List<int>();
        public readonly List<ItemId> items = new List<ItemId>();
        public int discarded;   //Không đủ slot
    }

    public class EffectsResetEvent : RuleEvent
    {
    }

    public class ItemUsedEvent : RuleEvent
    {
        public int actorId;
        public ItemId item;
        public int slot;
        public int ownerId;     //Chủ slot (khác actor khi bị Steal)
        public bool viaSteal;
    }

    public class ShellRevealedEvent : RuleEvent
    {
        public int toPlayerId;
        public int offset;      //0 = viên hiện tại, 1 = viên kế tiếp...
        public ShellType type;
    }

    public class ShellEjectedEvent : RuleEvent
    {
        public int actorId;
        public ShellType type;
    }

    public class ShellInvertedEvent : RuleEvent
    {
        public int actorId;
    }

    public class BoostedEvent : RuleEvent
    {
        public int actorId;
    }

    public class CuffedEvent : RuleEvent
    {
        public int targetId;
    }

    public class TurnSkippedEvent : RuleEvent
    {
        public int skippedPlayerId;
    }

    public class ChargesChangedEvent : RuleEvent
    {
        public int playerId;
        public int delta;
        public int newValue;
    }

    public class HealBlockedEvent : RuleEvent
    {
        public int playerId;
    }

    public class HealingDisabledEvent : RuleEvent
    {
        public int playerId;
    }

    public class GambleResultEvent : RuleEvent
    {
        public int actorId;
        public bool success;
    }

    public class ShotFiredEvent : RuleEvent
    {
        public int shooterId;
        public int targetId;
        public ShellType type;
        public int damage;
        public bool boosted;
    }

    public class TurnChangedEvent : RuleEvent
    {
        public int playerId;
    }

    public class LoadEmptyEvent : RuleEvent
    {
    }

    public class PlayerDownEvent : RuleEvent
    {
        public int playerId;
    }

    public class StageEndedEvent : RuleEvent
    {
        public int winnerId;
    }

    public class StealForfeitedEvent : RuleEvent
    {
        public int actorId;
    }
}
