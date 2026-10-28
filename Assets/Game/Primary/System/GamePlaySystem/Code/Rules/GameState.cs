using System.Collections.Generic;

namespace Game.Primary.GamePlaySystem.Rules
{
    public class Shell
    {
        public int id;
        public ShellType type;

        //Loại lúc bày ra bàn đầu load. Inverter chỉ đổi type, không đổi originalType
        public ShellType originalType;
        public bool inverted;

        //Những player đã biết loại viên này (Peek, RevealRandom)
        public readonly HashSet<int> knownBy = new HashSet<int>();

        public bool IsLive => type == ShellType.Live;
    }

    public class PlayerState
    {
        public int id;
        public bool isDealer;
        public int charges;
        public int maxCharges;
        public readonly ItemId[] slots = new ItemId[RuleConst.InventorySlots];

        public bool skipNextTurn;      //Đang bị còng
        public bool cuffImmune;        //Vừa thoát còng, chưa được đi lượt nào -> chưa thể bị còng lại
        public bool healingDisabled;

        public bool IsDown => charges <= 0;

        public int ItemCount
        {
            get
            {
                int count = 0;
                foreach (var item in slots)
                {
                    if (item != ItemId.None)
                        count++;
                }

                return count;
            }
        }

        public int FreeSlotCount => RuleConst.InventorySlots - ItemCount;

        public int FindItem(ItemId itemId)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == itemId)
                    return i;
            }

            return -1;
        }

        public bool HasItem(ItemId itemId) => FindItem(itemId) >= 0;

        public int FirstFreeSlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == ItemId.None)
                    return i;
            }

            return -1;
        }
    }

    public class ShotgunState
    {
        //shells[0] là viên đang nằm trong nòng
        public readonly List<Shell> shells = new List<Shell>();
        public bool boosted;   //Leather Whip (Hand Saw)
        public int loadIndex = -1;
        public int initialLive;
        public int initialBlank;

        public Shell Current => shells.Count > 0 ? shells[0] : null;
        public int Remaining => shells.Count;
        public bool IsEmpty => shells.Count == 0;

        public int LiveRemaining
        {
            get
            {
                int count = 0;
                foreach (var shell in shells)
                {
                    if (shell.type == ShellType.Live)
                        count++;
                }

                return count;
            }
        }

        public int BlankRemaining => shells.Count - LiveRemaining;

        //Thông tin công khai: đếm theo loại lúc bày ra bàn (ai cũng suy ra được kể cả khi có Inverter)
        public int PublicLiveRemaining
        {
            get
            {
                int count = 0;
                foreach (var shell in shells)
                {
                    if (shell.originalType == ShellType.Live)
                        count++;
                }

                return count;
            }
        }

        public int PublicBlankRemaining => shells.Count - PublicLiveRemaining;
    }

    /// <summary>
    /// Thống kê của người chơi trong cả run, dùng để tính tiền
    /// </summary>
    public class RunStats
    {
        public int shotsFired;
        public int shellsEjected;
        public int healsUsed;
        public int ejectItemsUsed;
        public int itemsUsed;
        public int revivalsUsed;
        public int revivalPenalty;
        public int stagesWon;
        public int stagesLost;
        public float elapsedSeconds;
    }

    public class GameState
    {
        public GameMode mode;
        public GameRng rng;
        public StageRules stage;
        public int stageIndex = -1;

        public readonly PlayerState[] players = new PlayerState[2];
        public readonly ShotgunState shotgun = new ShotgunState();
        public int currentPlayerId = PlayerIds.Player;

        public bool stageOver;
        public int stageWinnerId = -1;

        public RunStats stats = new RunStats();
        public int nextShellId;

        public PlayerState Player => players[PlayerIds.Player];
        public PlayerState Dealer => players[PlayerIds.Dealer];
        public PlayerState Current => players[currentPlayerId];

        public PlayerState Get(int playerId) => players[playerId];
        public PlayerState OpponentOf(int playerId) => players[PlayerIds.Opponent(playerId)];

        public GameState(GameMode mode, int seed)
        {
            this.mode = mode;
            rng = new GameRng(seed);
            players[PlayerIds.Player] = new PlayerState { id = PlayerIds.Player, isDealer = false };
            players[PlayerIds.Dealer] = new PlayerState { id = PlayerIds.Dealer, isDealer = true };
        }
    }
}
