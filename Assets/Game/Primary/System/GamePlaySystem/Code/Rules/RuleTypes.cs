namespace Game.Primary.GamePlaySystem.Rules
{
    public enum ShellType
    {
        Live = 0,   //HOT
        Blank = 1   //COLD
    }

    public enum GameMode
    {
        Story = 0,
        DoubleOrNothing = 1
    }

    /// <summary>
    /// Id theo chức năng (xem Doc/Theme18.md để biết tên hiển thị theo theme)
    /// </summary>
    public enum ItemId
    {
        None = 0,
        Peek = 1,           //Magnifying Glass -> Lace Mask
        Eject = 2,          //Beer -> Champagne
        Heal = 3,           //Cigarette Pack -> Ice Cube
        DoubleDamage = 4,   //Hand Saw -> Leather Whip
        SkipTurn = 5,       //Handcuffs -> Fluffy Handcuffs
        Steal = 6,          //Adrenaline -> Collar & Leash
        RevealRandom = 7,   //Burner Phone -> Love Dice
        GambleHeal = 8,     //Expired Medicine -> Mystery Pill
        Invert = 9          //Inverter -> Hot Wax Candle
    }

    public enum ItemBlockReason
    {
        None = 0,
        NotYourTurn,
        EmptySlot,
        NoShell,
        StaminaFull,
        HealingDisabled,
        AlreadyBoosted,
        OpponentAlreadyCuffed,
        NothingToSteal,
        NotEnoughShells,
        CannotStealThisItem,
        PlayerDown
    }

    public static class PlayerIds
    {
        public const int Player = 0;
        public const int Dealer = 1;

        public static int Opponent(int playerId) => playerId == Player ? Dealer : Player;
    }

    public static class RuleConst
    {
        public const int InventorySlots = 8;
    }
}
