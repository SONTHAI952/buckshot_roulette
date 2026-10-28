using Game.Primary.GamePlaySystem.Rules;

namespace Game.Primary.GamePlaySystem
{
    /// <summary>
    /// Câu chữ hiển thị trong gameplay, gom một chỗ để dễ đổi/dịch.
    /// </summary>
    public static class GameTexts
    {
        public const string YourTurn = "YOUR TURN";
        public const string DealerTurn = "HER TURN";
        public const string PickWandHint = "Tap the Wand to zap, or tap an item";
        public const string ZapHer = "ZAP HER";
        public const string ZapMyself = "ZAP MYSELF";
        public const string Cancel = "CANCEL";
        public const string Use = "USE";
        public const string StealHint = "Pick one of her items";
        public const string SafeWord = "SAFE WORD!";
        public const string SafeWordSub = "You tapped out... the game goes on";
        public const string NoSafeWords = "NO SAFE WORDS";
        public const string NoSafeWordsSub = "Lose this round and it's over";
        public const string RoundWon = "SHE BROKE FIRST";
        public const string RoundLost = "YOU BROKE FIRST";
        public const string Reloading = "Reloading the Wand...";
        public const string HealingLocked = "Too worked up to cool down";
        public const string Flipped = "FLIPPED";

        public static string Stage(int index, int count) => $"ROUND {index + 1}/{count}";

        public static string BlockReason(ItemBlockReason reason)
        {
            switch (reason)
            {
                case ItemBlockReason.NotYourTurn: return "Wait for your turn";
                case ItemBlockReason.NoShell: return "The Wand is empty";
                case ItemBlockReason.StaminaFull: return "Your stamina is already full";
                case ItemBlockReason.HealingDisabled: return HealingLocked;
                case ItemBlockReason.AlreadyBoosted: return "The whip is already cracked";
                case ItemBlockReason.OpponentAlreadyCuffed: return "She can't be cuffed again yet";
                case ItemBlockReason.NothingToSteal: return "Nothing worth taking";
                case ItemBlockReason.NotEnoughShells: return "Not enough charges left";
                case ItemBlockReason.CannotStealThisItem: return "You can't take that";
                case ItemBlockReason.PlayerDown: return "Round is over";
                default: return "Can't use that now";
            }
        }

        public static string Ordinal(int n)
        {
            if (n % 100 >= 11 && n % 100 <= 13)
                return n + "th";

            switch (n % 10)
            {
                case 1: return n + "st";
                case 2: return n + "nd";
                case 3: return n + "rd";
                default: return n + "th";
            }
        }
    }
}
