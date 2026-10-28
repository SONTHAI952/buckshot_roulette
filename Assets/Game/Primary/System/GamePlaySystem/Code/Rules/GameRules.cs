using System;
using System.Collections.Generic;

namespace Game.Primary.GamePlaySystem.Rules
{
    /// <summary>
    /// Toàn bộ luật chơi trong một stage. Logic thuần, không phụ thuộc view/animation.
    /// Mỗi hành động cập nhật GameState ngay lập tức và trả về danh sách RuleEvent để view phát lại.
    /// </summary>
    public class GameRules
    {
        public GameState State { get; }

        private GameRng Rng => State.rng;
        private StageRules Stage => State.stage;
        private ShotgunState Shotgun => State.shotgun;


        public GameRules(GameState state)
        {
            State = state;
        }



        #region Stage - Load

        public List<RuleEvent> StartStage(StageRules stage)
        {
            var events = new List<RuleEvent>();

            State.stage = stage;
            State.stageIndex++;
            State.stageOver = false;
            State.stageWinnerId = -1;
            State.currentPlayerId = PlayerIds.Player;

            foreach (var player in State.players)
            {
                player.maxCharges = stage.startCharges;
                player.charges = stage.startCharges;
                player.skipNextTurn = false;
                player.cuffImmune = false;
                player.healingDisabled = false;
                for (int i = 0; i < player.slots.Length; i++)
                    player.slots[i] = ItemId.None;
            }

            Shotgun.shells.Clear();
            Shotgun.boosted = false;
            Shotgun.loadIndex = -1;

            events.Add(new StageStartedEvent
            {
                stageIndex = State.stageIndex,
                stageName = stage.name,
                charges = stage.startCharges,
                fatal = stage.fatal
            });

            return events;
        }

        public List<RuleEvent> StartLoad()
        {
            var events = new List<RuleEvent>();
            if (State.stageOver)
                return events;

            Shotgun.loadIndex++;
            Shotgun.shells.Clear();

            //Reset hiệu ứng tạm thời gắn với load
            if (Stage.resetEffectsOnNewLoad)
            {
                bool hadEffect = Shotgun.boosted;
                Shotgun.boosted = false;
                foreach (var player in State.players)
                {
                    hadEffect |= player.skipNextTurn;
                    player.skipNextTurn = false;
                    player.cuffImmune = false;
                }

                if (hadEffect)
                    events.Add(new EffectsResetEvent());
            }

            //Tạo load: thành phần cố định trước, sau đó mới shuffle
            var composition = GetLoadComposition(Shotgun.loadIndex);
            for (int i = 0; i < composition.live; i++)
                Shotgun.shells.Add(CreateShell(ShellType.Live));
            for (int i = 0; i < composition.blank; i++)
                Shotgun.shells.Add(CreateShell(ShellType.Blank));
            Rng.Shuffle(Shotgun.shells);

            Shotgun.initialLive = composition.live;
            Shotgun.initialBlank = composition.blank;

            if (Stage.playerStartsEachLoad)
                State.currentPlayerId = PlayerIds.Player;

            events.Add(new LoadStartedEvent
            {
                loadIndex = Shotgun.loadIndex,
                live = composition.live,
                blank = composition.blank,
                firstPlayerId = State.currentPlayerId
            });

            //Chia item: hai bên nhận cùng số lượng, rút riêng
            if (Stage.HasItems)
            {
                int count = Rng.RangeInclusive(Stage.minItemsPerLoad, Stage.maxItemsPerLoad);
                if (count > 0)
                {
                    events.Add(DealItems(State.Player, count));
                    events.Add(DealItems(State.Dealer, count));
                }
            }

            return events;
        }

        public LoadData GetLoadComposition(int loadIndex)
        {
            if (loadIndex < Stage.fixedLoads.Count)
                return Stage.fixedLoads[loadIndex];

            //Load random: luôn có ít nhất 1 live và 1 blank
            int total = Rng.RangeInclusive(Stage.minShells, Stage.maxShells);
            int live;
            if (total <= 3)
                live = Rng.RangeInclusive(1, total - 1);
            else
                live = total / 2 + Rng.RangeInclusive(-1, 1);

            live = Math.Max(1, Math.Min(total - 1, live));
            return new LoadData(live, total - live);
        }

        private Shell CreateShell(ShellType type)
        {
            return new Shell
            {
                id = State.nextShellId++,
                type = type,
                originalType = type
            };
        }

        private ItemsDealtEvent DealItems(PlayerState player, int count)
        {
            var e = new ItemsDealtEvent { playerId = player.id };
            for (int i = 0; i < count; i++)
            {
                var item = Rng.Pick(Stage.itemPool);
                int slot = player.FirstFreeSlot();
                if (slot < 0)
                {
                    e.discarded++;
                    continue;
                }

                player.slots[slot] = item;
                e.slots.Add(slot);
                e.items.Add(item);
            }

            return e;
        }

        #endregion



        #region Items

        public ItemBlockReason CanUseItem(int actorId, int slot)
        {
            if (State.stageOver)
                return ItemBlockReason.PlayerDown;

            if (actorId != State.currentPlayerId)
                return ItemBlockReason.NotYourTurn;

            var actor = State.Get(actorId);
            if (actor.IsDown)
                return ItemBlockReason.PlayerDown;

            if (slot < 0 || slot >= actor.slots.Length || actor.slots[slot] == ItemId.None)
                return ItemBlockReason.EmptySlot;

            return CanApplyItem(actorId, actor.slots[slot], false);
        }

        /// <summary>
        /// Item có tạo ra hiệu ứng hợp lệ hay không. isStolen = item được lấy qua Steal
        /// </summary>
        public ItemBlockReason CanApplyItem(int actorId, ItemId item, bool isStolen)
        {
            var actor = State.Get(actorId);
            var opponent = State.OpponentOf(actorId);

            switch (item)
            {
                case ItemId.Peek:
                case ItemId.Eject:
                case ItemId.Invert:
                    return Shotgun.IsEmpty ? ItemBlockReason.NoShell : ItemBlockReason.None;

                case ItemId.Heal:
                    if (actor.healingDisabled)
                        return ItemBlockReason.HealingDisabled;
                    return actor.charges >= actor.maxCharges ? ItemBlockReason.StaminaFull : ItemBlockReason.None;

                case ItemId.DoubleDamage:
                    if (Shotgun.IsEmpty)
                        return ItemBlockReason.NoShell;
                    return Shotgun.boosted ? ItemBlockReason.AlreadyBoosted : ItemBlockReason.None;

                case ItemId.SkipTurn:
                    return opponent.skipNextTurn || opponent.cuffImmune
                        ? ItemBlockReason.OpponentAlreadyCuffed
                        : ItemBlockReason.None;

                case ItemId.Steal:
                    if (isStolen)
                        return ItemBlockReason.CannotStealThisItem;
                    return GetStealableSlots(actorId).Count > 0 ? ItemBlockReason.None : ItemBlockReason.NothingToSteal;

                case ItemId.RevealRandom:
                    return Shotgun.Remaining < 2 ? ItemBlockReason.NotEnoughShells : ItemBlockReason.None;

                case ItemId.GambleHeal:
                    return ItemBlockReason.None;

                default:
                    return ItemBlockReason.EmptySlot;
            }
        }

        /// <summary> Các slot của đối thủ mà actor có thể cướp bằng Steal </summary>
        public List<int> GetStealableSlots(int actorId)
        {
            var result = new List<int>();
            var opponent = State.OpponentOf(actorId);
            for (int i = 0; i < opponent.slots.Length; i++)
            {
                var item = opponent.slots[i];
                if (item == ItemId.None || item == ItemId.Steal)
                    continue;

                if (CanApplyItem(actorId, item, true) == ItemBlockReason.None)
                    result.Add(i);
            }

            return result;
        }

        /// <summary>
        /// Dùng item ở slot. Với Steal, stealSlot là slot của đối thủ muốn cướp.
        /// </summary>
        public bool TryUseItem(int actorId, int slot, int stealSlot, out List<RuleEvent> events, out ItemBlockReason reason)
        {
            events = new List<RuleEvent>();
            reason = CanUseItem(actorId, slot);
            if (reason != ItemBlockReason.None)
                return false;

            var actor = State.Get(actorId);
            var opponent = State.OpponentOf(actorId);
            var item = actor.slots[slot];

            if (item == ItemId.Steal)
            {
                if (GetStealableSlots(actorId).Contains(stealSlot) == false)
                {
                    reason = ItemBlockReason.CannotStealThisItem;
                    return false;
                }

                ConsumeItem(actor, slot, actorId, false, events);

                //Item bị cướp được dùng ngay như một lần dùng item, không kích hoạt Steal đệ quy
                var stolen = opponent.slots[stealSlot];
                ConsumeItem(opponent, stealSlot, actorId, true, events);
                ApplyItemEffect(actorId, stolen, events);
                return true;
            }

            ConsumeItem(actor, slot, actorId, false, events);
            ApplyItemEffect(actorId, item, events);
            return true;
        }

        /// <summary> Hết thời gian chọn item khi Steal: Steal bị tiêu hao, không có hiệu ứng </summary>
        public bool ForfeitSteal(int actorId, int slot, out List<RuleEvent> events)
        {
            events = new List<RuleEvent>();
            if (CanUseItem(actorId, slot) != ItemBlockReason.None || State.Get(actorId).slots[slot] != ItemId.Steal)
                return false;

            ConsumeItem(State.Get(actorId), slot, actorId, false, events);
            events.Add(new StealForfeitedEvent { actorId = actorId });
            return true;
        }

        private void ConsumeItem(PlayerState owner, int slot, int actorId, bool viaSteal, List<RuleEvent> events)
        {
            var item = owner.slots[slot];
            owner.slots[slot] = ItemId.None;

            events.Add(new ItemUsedEvent
            {
                actorId = actorId,
                item = item,
                slot = slot,
                ownerId = owner.id,
                viaSteal = viaSteal
            });

            if (actorId == PlayerIds.Player)
            {
                State.stats.itemsUsed++;
                if (item == ItemId.Heal)
                    State.stats.healsUsed++;
                else if (item == ItemId.Eject)
                    State.stats.ejectItemsUsed++;
            }
        }

        private void ApplyItemEffect(int actorId, ItemId item, List<RuleEvent> events)
        {
            var opponent = State.OpponentOf(actorId);

            switch (item)
            {
                case ItemId.Peek:
                {
                    var current = Shotgun.Current;
                    current.knownBy.Add(actorId);
                    events.Add(new ShellRevealedEvent { toPlayerId = actorId, offset = 0, type = current.type });
                    break;
                }

                case ItemId.Eject:
                {
                    var shell = Shotgun.shells[0];
                    Shotgun.shells.RemoveAt(0);
                    if (actorId == PlayerIds.Player)
                        State.stats.shellsEjected++;

                    events.Add(new ShellEjectedEvent { actorId = actorId, type = shell.type });
                    if (Shotgun.IsEmpty)
                        events.Add(new LoadEmptyEvent());
                    break;
                }

                case ItemId.Heal:
                    ApplyHeal(actorId, 1, events);
                    break;

                case ItemId.DoubleDamage:
                    Shotgun.boosted = true;
                    events.Add(new BoostedEvent { actorId = actorId });
                    break;

                case ItemId.SkipTurn:
                    opponent.skipNextTurn = true;
                    events.Add(new CuffedEvent { targetId = opponent.id });
                    break;

                case ItemId.RevealRandom:
                {
                    int offset = Rng.RangeInclusive(1, Shotgun.Remaining - 1);
                    var shell = Shotgun.shells[offset];
                    shell.knownBy.Add(actorId);
                    events.Add(new ShellRevealedEvent { toPlayerId = actorId, offset = offset, type = shell.type });
                    break;
                }

                case ItemId.GambleHeal:
                {
                    bool success = Rng.Chance(0.5);
                    events.Add(new GambleResultEvent { actorId = actorId, success = success });
                    if (success)
                        ApplyHeal(actorId, 2, events);
                    else
                        ApplyDamage(actorId, 1, events);
                    break;
                }

                case ItemId.Invert:
                {
                    var current = Shotgun.Current;
                    current.type = current.type == ShellType.Live ? ShellType.Blank : ShellType.Live;
                    current.inverted = !current.inverted;
                    events.Add(new ShellInvertedEvent { actorId = actorId });

                    //Ai đã biết viên này thì cập nhật luôn loại mới
                    foreach (var knower in current.knownBy)
                        events.Add(new ShellRevealedEvent { toPlayerId = knower, offset = 0, type = current.type });
                    break;
                }
            }
        }

        #endregion



        #region Fire

        public bool CanFire(int actorId, int targetId)
        {
            if (State.stageOver || actorId != State.currentPlayerId)
                return false;

            if (targetId != PlayerIds.Player && targetId != PlayerIds.Dealer)
                return false;

            if (State.Get(actorId).IsDown || State.Get(targetId).IsDown)
                return false;

            return Shotgun.IsEmpty == false;
        }

        public List<RuleEvent> Fire(int actorId, int targetId)
        {
            var events = new List<RuleEvent>();
            if (CanFire(actorId, targetId) == false)
                return events;

            var shell = Shotgun.shells[0];
            Shotgun.shells.RemoveAt(0);

            bool boosted = Shotgun.boosted;
            Shotgun.boosted = false;
            int damage = shell.IsLive ? (boosted ? 2 : 1) : 0;

            if (actorId == PlayerIds.Player)
                State.stats.shotsFired++;

            events.Add(new ShotFiredEvent
            {
                shooterId = actorId,
                targetId = targetId,
                type = shell.type,
                damage = damage,
                boosted = boosted
            });

            if (damage > 0)
                ApplyDamage(targetId, damage, events);

            if (State.stageOver)
                return events;

            //Chỉ tự bắn trúng blank mới giữ lượt
            bool turnEnds = shell.IsLive || targetId != actorId;

            if (Shotgun.IsEmpty)
            {
                if (turnEnds && Stage.playerStartsEachLoad == false)
                    EndTurn(actorId, events);

                events.Add(new LoadEmptyEvent());
            }
            else if (turnEnds)
            {
                EndTurn(actorId, events);
            }

            return events;
        }

        private void EndTurn(int actorId, List<RuleEvent> events)
        {
            var next = State.OpponentOf(actorId);
            if (next.skipNextTurn)
            {
                //Còng chỉ khoá đúng một lượt, sau đó được miễn còng tới khi đi được một lượt
                next.skipNextTurn = false;
                next.cuffImmune = true;
                events.Add(new TurnSkippedEvent { skippedPlayerId = next.id });
                return;
            }

            State.currentPlayerId = next.id;
            next.cuffImmune = false;
            events.Add(new TurnChangedEvent { playerId = next.id });
        }

        #endregion



        #region Charges

        private void ApplyHeal(int playerId, int amount, List<RuleEvent> events)
        {
            var player = State.Get(playerId);
            if (player.healingDisabled)
            {
                events.Add(new HealBlockedEvent { playerId = playerId });
                return;
            }

            int newValue = Math.Min(player.maxCharges, player.charges + amount);
            int delta = newValue - player.charges;
            player.charges = newValue;
            if (delta > 0)
                events.Add(new ChargesChangedEvent { playerId = playerId, delta = delta, newValue = newValue });
        }

        private void ApplyDamage(int playerId, int amount, List<RuleEvent> events)
        {
            var player = State.Get(playerId);
            int newValue = Math.Max(0, player.charges - amount);
            int delta = newValue - player.charges;
            player.charges = newValue;
            events.Add(new ChargesChangedEvent { playerId = playerId, delta = delta, newValue = newValue });

            if (player.IsDown)
            {
                events.Add(new PlayerDownEvent { playerId = playerId });
                EndStage(PlayerIds.Opponent(playerId), events);
                return;
            }

            int threshold = Stage.healingDisabledAtOrBelow;
            if (threshold > 0 && player.healingDisabled == false && player.charges <= threshold)
            {
                player.healingDisabled = true;
                events.Add(new HealingDisabledEvent { playerId = playerId });
            }
        }

        private void EndStage(int winnerId, List<RuleEvent> events)
        {
            State.stageOver = true;
            State.stageWinnerId = winnerId;

            if (winnerId == PlayerIds.Player)
                State.stats.stagesWon++;
            else
                State.stats.stagesLost++;

            events.Add(new StageEndedEvent { winnerId = winnerId });
        }

        /// <summary>
        /// Người chơi thua stage không fatal: dùng safe word để chơi tiếp. Trả về số tiền bị phạt.
        /// </summary>
        public int UseSafeWord(bool wonPreviousStage)
        {
            int penalty = wonPreviousStage ? Stage.revivalPenaltyIfWonPrevious : Stage.revivalPenaltyIfLostPrevious;
            State.stats.revivalsUsed++;
            State.stats.revivalPenalty += penalty;
            return penalty;
        }

        #endregion



        #region Observation

        /// <summary>
        /// Thông tin mà một bên được phép biết. Không chứa thứ tự đạn bị ẩn.
        /// </summary>
        public DealerObservation ToObservation(int playerId)
        {
            var self = State.Get(playerId);
            var opponent = State.OpponentOf(playerId);

            var obs = new DealerObservation
            {
                selfId = playerId,
                selfCharges = self.charges,
                selfMaxCharges = self.maxCharges,
                selfHealingDisabled = self.healingDisabled,
                opponentCharges = opponent.charges,
                opponentMaxCharges = opponent.maxCharges,
                opponentCuffed = opponent.skipNextTurn,
                opponentCuffImmune = opponent.cuffImmune,
                shellsRemaining = Shotgun.Remaining,
                publicLiveRemaining = Shotgun.PublicLiveRemaining,
                publicBlankRemaining = Shotgun.PublicBlankRemaining,
                currentInverted = Shotgun.Current != null && Shotgun.Current.inverted,
                boosted = Shotgun.boosted,
                fatalStage = Stage != null && Stage.fatal
            };

            for (int i = 0; i < RuleConst.InventorySlots; i++)
            {
                obs.selfSlots[i] = self.slots[i];
                obs.opponentSlots[i] = opponent.slots[i];
                obs.usableSlots[i] = self.slots[i] != ItemId.None && CanUseItem(playerId, i) == ItemBlockReason.None;
            }

            obs.stealableOpponentSlots.AddRange(GetStealableSlots(playerId));

            for (int offset = 0; offset < Shotgun.shells.Count; offset++)
            {
                var shell = Shotgun.shells[offset];
                if (shell.knownBy.Contains(playerId))
                    obs.knownShells[offset] = shell.type;
            }

            return obs;
        }

        #endregion
    }
}
