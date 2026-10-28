using System.Collections.Generic;
using System.Linq;
using Game.Primary.GamePlaySystem.Rules;
using NUnit.Framework;

namespace Game.Primary.Tests
{
    public class GameRulesTests
    {
        private const int P = PlayerIds.Player;
        private const int D = PlayerIds.Dealer;


        #region Helper

        private static StageRules MakeStage(int charges = 4, int threshold = 0, bool fatal = false, params ItemId[] pool)
        {
            return new StageRules
            {
                name = "Test",
                startCharges = charges,
                minItemsPerLoad = 0,
                maxItemsPerLoad = 0,
                itemPool = pool.ToList(),
                minShells = 2,
                maxShells = 8,
                fatal = fatal,
                healingDisabledAtOrBelow = threshold
            };
        }

        /// <summary> Tạo rules với các viên đạn theo đúng thứ tự cho trước </summary>
        private static GameRules MakeRules(StageRules stage, params ShellType[] shells)
        {
            var rules = new GameRules(new GameState(GameMode.Story, 1234));
            rules.StartStage(stage);
            SetShells(rules, shells);
            return rules;
        }

        private static void SetShells(GameRules rules, params ShellType[] shells)
        {
            var sg = rules.State.shotgun;
            sg.shells.Clear();
            foreach (var type in shells)
                sg.shells.Add(new Shell { id = rules.State.nextShellId++, type = type, originalType = type });
        }

        private static void Give(GameRules rules, int playerId, int slot, ItemId item)
        {
            rules.State.Get(playerId).slots[slot] = item;
        }

        private static List<RuleEvent> Use(GameRules rules, int actor, int slot, int stealSlot = -1)
        {
            bool ok = rules.TryUseItem(actor, slot, stealSlot, out var events, out var reason);
            Assert.IsTrue(ok, "Use item failed: " + reason);
            return events;
        }

        private const ShellType L = ShellType.Live;
        private const ShellType B = ShellType.Blank;

        #endregion



        #region Fire

        [Test]
        public void Fire_LiveAtDealer_RemovesOneCharge_EndsTurn()
        {
            var rules = MakeRules(MakeStage(), L, B);
            rules.Fire(P, D);
            Assert.AreEqual(3, rules.State.Dealer.charges);
            Assert.AreEqual(D, rules.State.currentPlayerId);
        }

        [Test]
        public void Fire_LiveAtSelf_RemovesOneCharge_EndsTurn()
        {
            var rules = MakeRules(MakeStage(), L, B);
            rules.Fire(P, P);
            Assert.AreEqual(3, rules.State.Player.charges);
            Assert.AreEqual(D, rules.State.currentPlayerId);
        }

        [Test]
        public void Fire_BlankAtDealer_NoDamage_EndsTurn()
        {
            var rules = MakeRules(MakeStage(), B, L);
            rules.Fire(P, D);
            Assert.AreEqual(4, rules.State.Dealer.charges);
            Assert.AreEqual(D, rules.State.currentPlayerId);
        }

        [Test]
        public void Fire_BlankAtSelf_NoDamage_KeepsTurn()
        {
            var rules = MakeRules(MakeStage(), B, L);
            rules.Fire(P, P);
            Assert.AreEqual(4, rules.State.Player.charges);
            Assert.AreEqual(P, rules.State.currentPlayerId);
        }

        [Test]
        public void Boost_DoublesLiveDamage_ThenResets()
        {
            var rules = MakeRules(MakeStage(), L, L, B);
            Give(rules, P, 0, ItemId.DoubleDamage);
            Use(rules, P, 0);
            rules.Fire(P, D);
            Assert.AreEqual(2, rules.State.Dealer.charges);
            Assert.IsFalse(rules.State.shotgun.boosted);

            rules.Fire(D, P);
            Assert.AreEqual(3, rules.State.Player.charges, "Boost không được giữ cho phát sau");
        }

        [Test]
        public void Boost_OnBlank_NoDamage_AndResets()
        {
            var rules = MakeRules(MakeStage(), B, L);
            Give(rules, P, 0, ItemId.DoubleDamage);
            Use(rules, P, 0);
            rules.Fire(P, D);
            Assert.AreEqual(4, rules.State.Dealer.charges);
            Assert.IsFalse(rules.State.shotgun.boosted);
        }

        [Test]
        public void CannotFire_WhenNotYourTurn_OrEmpty()
        {
            var rules = MakeRules(MakeStage(), L);
            Assert.IsFalse(rules.CanFire(D, P));
            rules.Fire(P, D);
            Assert.IsTrue(rules.State.shotgun.IsEmpty);
            Assert.IsFalse(rules.CanFire(rules.State.currentPlayerId, P));
            Assert.AreEqual(0, rules.Fire(P, D).Count);
        }

        #endregion



        #region Load and information

        [Test]
        public void Counts_AlwaysMatchRemaining()
        {
            var rules = new GameRules(new GameState(GameMode.Story, 42));
            rules.StartStage(MakeStage(charges: 8));
            for (int load = 0; load < 20; load++)
            {
                rules.StartLoad();
                var sg = rules.State.shotgun;
                Assert.AreEqual(sg.Remaining, sg.LiveRemaining + sg.BlankRemaining);
                Assert.GreaterOrEqual(sg.LiveRemaining, 1);
                Assert.GreaterOrEqual(sg.BlankRemaining, 1);
                while (sg.IsEmpty == false && rules.State.stageOver == false)
                {
                    rules.Fire(rules.State.currentPlayerId, rules.State.currentPlayerId);
                    Assert.AreEqual(sg.Remaining, sg.LiveRemaining + sg.BlankRemaining);
                }

                if (rules.State.stageOver)
                    rules.StartStage(MakeStage(charges: 8));
            }
        }

        [Test]
        public void Peek_RevealsCurrent_WithoutConsuming()
        {
            var rules = MakeRules(MakeStage(), L, B);
            Give(rules, P, 3, ItemId.Peek);
            var events = Use(rules, P, 3);
            var reveal = events.OfType<ShellRevealedEvent>().Single();
            Assert.AreEqual(ShellType.Live, reveal.type);
            Assert.AreEqual(0, reveal.offset);
            Assert.AreEqual(2, rules.State.shotgun.Remaining);
            Assert.IsTrue(rules.State.shotgun.Current.knownBy.Contains(P));
            Assert.IsFalse(rules.State.shotgun.Current.knownBy.Contains(D));
        }

        [Test]
        public void Eject_RemovesCurrent_NoDamage_KeepsTurn()
        {
            var rules = MakeRules(MakeStage(), L, B, B);
            Give(rules, P, 0, ItemId.Eject);
            var events = Use(rules, P, 0);
            Assert.AreEqual(ShellType.Live, events.OfType<ShellEjectedEvent>().Single().type);
            Assert.AreEqual(2, rules.State.shotgun.Remaining);
            Assert.AreEqual(4, rules.State.Player.charges);
            Assert.AreEqual(4, rules.State.Dealer.charges);
            Assert.AreEqual(P, rules.State.currentPlayerId);
            Assert.AreEqual(1, rules.State.stats.ejectItemsUsed);
        }

        [Test]
        public void Eject_LastShell_EndsLoad_NoCrash()
        {
            var rules = MakeRules(MakeStage(), B);
            Give(rules, P, 0, ItemId.Eject);
            Give(rules, P, 1, ItemId.Eject);
            var events = Use(rules, P, 0);
            Assert.IsTrue(events.OfType<LoadEmptyEvent>().Any());
            Assert.AreEqual(ItemBlockReason.NoShell, rules.CanUseItem(P, 1));
        }

        [Test]
        public void Invert_FlipsCurrent_UpdatesCounts_KeepsPublicCounts()
        {
            var rules = MakeRules(MakeStage(), B, L);
            Give(rules, P, 0, ItemId.Invert);
            Use(rules, P, 0);
            var sg = rules.State.shotgun;
            Assert.AreEqual(ShellType.Live, sg.Current.type);
            Assert.AreEqual(2, sg.LiveRemaining);
            Assert.AreEqual(0, sg.BlankRemaining);
            Assert.AreEqual(1, sg.PublicLiveRemaining, "Số công khai đếm theo loại ban đầu");
            Assert.IsTrue(sg.Current.inverted);
        }

        [Test]
        public void ShellKnowledge_IsGone_AfterFire()
        {
            var rules = MakeRules(MakeStage(), B, L);
            Give(rules, D, 0, ItemId.Peek);
            rules.Fire(P, D);
            Use(rules, D, 0);
            Assert.IsTrue(rules.ToObservation(D).KnowsCurrent);
            rules.Fire(D, P);
            Assert.IsFalse(rules.ToObservation(D).KnowsCurrent);
        }

        [Test]
        public void RevealRandom_RevealsFutureShell_WithCorrectType()
        {
            var rules = MakeRules(MakeStage(), B, B, L, B);
            Give(rules, P, 0, ItemId.RevealRandom);
            var e = Use(rules, P, 0).OfType<ShellRevealedEvent>().Single();
            Assert.GreaterOrEqual(e.offset, 1);
            Assert.Less(e.offset, 4);
            Assert.AreEqual(rules.State.shotgun.shells[e.offset].type, e.type);
            Assert.AreEqual(4, rules.State.shotgun.Remaining);
        }

        [Test]
        public void RevealRandom_Blocked_WithOneShell()
        {
            var rules = MakeRules(MakeStage(), L);
            Give(rules, P, 0, ItemId.RevealRandom);
            Assert.AreEqual(ItemBlockReason.NotEnoughShells, rules.CanUseItem(P, 0));
        }

        #endregion



        #region Items

        [Test]
        public void Heal_RestoresOne_NotAboveMax_BlockedWhenDisabled()
        {
            var rules = MakeRules(MakeStage(charges: 4, threshold: 2), L, L, L, B);
            Give(rules, P, 0, ItemId.Heal);
            Assert.AreEqual(ItemBlockReason.StaminaFull, rules.CanUseItem(P, 0));

            rules.Fire(P, P);            //4 -> 3
            rules.Fire(D, D);            //Dealer tự bắn live -> lượt về P
            Use(rules, P, 0);
            Assert.AreEqual(4, rules.State.Player.charges);

            Give(rules, P, 1, ItemId.Heal);
            rules.Fire(P, P);            //4 -> 3
            Assert.AreEqual(3, rules.State.Player.charges);
            Assert.IsFalse(rules.State.Player.healingDisabled);

            //Hạ tới ngưỡng -> khoá hồi máu
            rules.State.Player.charges = 3;
            SetShells(rules, L, B);
            rules.State.currentPlayerId = P;
            rules.Fire(P, P);            //3 -> 2
            Assert.IsTrue(rules.State.Player.healingDisabled);
            rules.State.currentPlayerId = P;
            Assert.AreEqual(ItemBlockReason.HealingDisabled, rules.CanUseItem(P, 1));
        }

        [Test]
        public void Cuffs_SkipExactlyOneTurn_NoInfiniteLock()
        {
            var rules = MakeRules(MakeStage(charges: 8), B, B, B, B, B);
            Give(rules, P, 0, ItemId.SkipTurn);
            Give(rules, P, 1, ItemId.SkipTurn);
            Use(rules, P, 0);

            var events = rules.Fire(P, D);
            Assert.IsTrue(events.OfType<TurnSkippedEvent>().Any());
            Assert.AreEqual(P, rules.State.currentPlayerId, "Dealer bị còng -> mất lượt");

            Assert.AreEqual(ItemBlockReason.OpponentAlreadyCuffed, rules.CanUseItem(P, 1), "Không còng liên tiếp");

            rules.Fire(P, D);
            Assert.AreEqual(D, rules.State.currentPlayerId);
        }

        [Test]
        public void Steal_CannotStealSteal_AndEffectRunsOnce()
        {
            var rules = MakeRules(MakeStage(charges: 4), L, B);
            Give(rules, P, 0, ItemId.Steal);
            Give(rules, D, 0, ItemId.Steal);
            Give(rules, D, 1, ItemId.DoubleDamage);

            var stealable = rules.GetStealableSlots(P);
            CollectionAssert.DoesNotContain(stealable, 0);
            CollectionAssert.Contains(stealable, 1);

            Assert.IsFalse(rules.TryUseItem(P, 0, 0, out _, out var reason));
            Assert.AreEqual(ItemBlockReason.CannotStealThisItem, reason);

            var events = Use(rules, P, 0, 1);
            Assert.AreEqual(1, events.OfType<BoostedEvent>().Count());
            Assert.AreEqual(ItemId.None, rules.State.Dealer.slots[1]);
            Assert.AreEqual(ItemId.None, rules.State.Player.slots[0]);
            Assert.IsTrue(rules.State.shotgun.boosted);
        }

        [Test]
        public void Steal_Forfeit_ConsumesItem()
        {
            var rules = MakeRules(MakeStage(), L, B);
            Give(rules, P, 2, ItemId.Steal);
            Give(rules, D, 0, ItemId.Peek);
            Assert.IsTrue(rules.ForfeitSteal(P, 2, out _));
            Assert.AreEqual(ItemId.None, rules.State.Player.slots[2]);
            Assert.AreEqual(ItemId.Peek, rules.State.Dealer.slots[0]);
        }

        [Test]
        public void GambleHeal_BothBranches_AndCanKill()
        {
            bool sawSuccess = false, sawFail = false;
            for (int seed = 0; seed < 40 && (sawSuccess == false || sawFail == false); seed++)
            {
                var rules = new GameRules(new GameState(GameMode.DoubleOrNothing, seed));
                rules.StartStage(MakeStage(charges: 4));
                SetShells(rules, L, B);
                rules.State.Player.charges = 1;
                Give(rules, P, 0, ItemId.GambleHeal);
                var events = Use(rules, P, 0);
                var result = events.OfType<GambleResultEvent>().Single();
                if (result.success)
                {
                    sawSuccess = true;
                    Assert.AreEqual(3, rules.State.Player.charges);
                }
                else
                {
                    sawFail = true;
                    Assert.AreEqual(0, rules.State.Player.charges);
                    Assert.IsTrue(rules.State.stageOver);
                    Assert.AreEqual(D, rules.State.stageWinnerId);
                }
            }

            Assert.IsTrue(sawSuccess && sawFail);
        }

        [Test]
        public void ItemsBlocked_OutsideTurn()
        {
            var rules = MakeRules(MakeStage(), L, B);
            Give(rules, D, 0, ItemId.Peek);
            Assert.AreEqual(ItemBlockReason.NotYourTurn, rules.CanUseItem(D, 0));
        }

        #endregion



        #region Stage config

        private static LevelData LoadStoryLevel()
        {
            return new LevelData
            {
                mode = "Story",
                stages = new List<StageData>
                {
                    new StageData { minCharges = 2, maxCharges = 2, fixedLoads = new List<LoadData> { new LoadData(1, 2), new LoadData(3, 2) } },
                    new StageData { minCharges = 4, maxCharges = 4, minItemsPerLoad = 2, maxItemsPerLoad = 2,
                        itemPool = new List<string> { "Peek", "Eject", "Heal", "DoubleDamage", "SkipTurn" },
                        fixedLoads = new List<LoadData> { new LoadData(1, 1) } },
                    new StageData { minCharges = 4, maxCharges = 4, minItemsPerLoad = 4, maxItemsPerLoad = 4,
                        itemPool = new List<string> { "Peek", "Eject", "Heal", "DoubleDamage", "SkipTurn" },
                        fatal = true, healingDisabledAtOrBelow = 2 }
                }
            };
        }

        [Test]
        public void Story_Stage1_TwoCharges_NoItems_FirstLoadOneLiveTwoBlank()
        {
            var level = LoadStoryLevel();
            var rules = new GameRules(new GameState(GameMode.Story, 7));
            rules.StartStage(StageRules.FromData(level.stages[0], rules.State.rng));
            var events = rules.StartLoad();
            Assert.AreEqual(2, rules.State.Player.charges);
            Assert.AreEqual(2, rules.State.Dealer.charges);
            Assert.AreEqual(0, rules.State.Player.ItemCount);
            var load = events.OfType<LoadStartedEvent>().Single();
            Assert.AreEqual(1, load.live);
            Assert.AreEqual(2, load.blank);
            Assert.AreEqual(P, load.firstPlayerId);
        }

        [Test]
        public void Story_Stage2_FourCharges_TwoItemsEach()
        {
            var level = LoadStoryLevel();
            var rules = new GameRules(new GameState(GameMode.Story, 7));
            rules.StartStage(StageRules.FromData(level.stages[1], rules.State.rng));
            var events = rules.StartLoad();
            Assert.AreEqual(4, rules.State.Player.charges);
            Assert.AreEqual(2, rules.State.Player.ItemCount);
            Assert.AreEqual(2, rules.State.Dealer.ItemCount);
            var load = events.OfType<LoadStartedEvent>().Single();
            Assert.AreEqual(1, load.live);
            Assert.AreEqual(1, load.blank);
        }

        [Test]
        public void Story_Stage3_FourItems_EveryLoadHasLiveAndBlank_SlotsCapped()
        {
            var level = LoadStoryLevel();
            var rules = new GameRules(new GameState(GameMode.Story, 99));
            rules.StartStage(StageRules.FromData(level.stages[2], rules.State.rng));
            for (int i = 0; i < 6; i++)
            {
                rules.State.shotgun.shells.Clear();
                var load = rules.StartLoad().OfType<LoadStartedEvent>().Single();
                Assert.GreaterOrEqual(load.live, 1);
                Assert.GreaterOrEqual(load.blank, 1);
                Assert.LessOrEqual(rules.State.Player.ItemCount, RuleConst.InventorySlots);
            }

            Assert.AreEqual(RuleConst.InventorySlots, rules.State.Player.ItemCount);
            Assert.IsTrue(rules.State.stage.fatal);
        }

        [Test]
        public void SafeWord_Penalty_DependsOnPreviousStage()
        {
            var stage = MakeStage();
            stage.revivalPenaltyIfWonPrevious = 4000;
            stage.revivalPenaltyIfLostPrevious = 2000;
            var rules = MakeRules(stage, L);
            Assert.AreEqual(4000, rules.UseSafeWord(true));
            Assert.AreEqual(2000, rules.UseSafeWord(false));
            Assert.AreEqual(6000, rules.State.stats.revivalPenalty);
        }

        [Test]
        public void Payout_Story_SubtractsPenalties()
        {
            var eco = new EconomyData { baseCash = 70000, healItemPenalty = 220, ejectItemPenalty = 495 };
            var stats = new RunStats { revivalPenalty = 4000, healsUsed = 2, ejectItemsUsed = 1 };
            Assert.AreEqual(70000 - 4000 - 440 - 495, PayoutCalculator.CalculateStory(eco, stats));
        }

        [Test]
        public void SameSeed_SameActions_SameState()
        {
            string Run()
            {
                var level = LoadStoryLevel();
                var rules = new GameRules(new GameState(GameMode.Story, 555));
                rules.StartStage(StageRules.FromData(level.stages[2], rules.State.rng));
                rules.StartLoad();
                var log = new System.Text.StringBuilder();
                for (int i = 0; i < 30 && rules.State.stageOver == false; i++)
                {
                    if (rules.State.shotgun.IsEmpty)
                        rules.StartLoad();

                    var obs = rules.ToObservation(rules.State.currentPlayerId);
                    var d = DealerBrain.Decide(obs, rules.State.rng);
                    if (d.type == DealerActionType.UseItem)
                        rules.TryUseItem(rules.State.currentPlayerId, d.slot, d.stealSlot, out _, out _);
                    else
                        rules.Fire(rules.State.currentPlayerId, d.targetId);

                    log.Append(d).Append('|').Append(rules.State.Player.charges).Append(rules.State.Dealer.charges).Append(';');
                }

                return log.ToString();
            }

            Assert.AreEqual(Run(), Run());
        }

        #endregion



        #region AI

        [Test]
        public void Observation_DoesNotLeakShellOrder()
        {
            var rules = MakeRules(MakeStage(), L, B, B, L);
            var obs = rules.ToObservation(D);
            Assert.AreEqual(0, obs.knownShells.Count);
            Assert.AreEqual(0.5, obs.ProbCurrentLive(), 1e-9);
        }

        [Test]
        public void Observation_AfterInvert_ProbabilityFlips()
        {
            var rules = MakeRules(MakeStage(), B, B, L);
            Give(rules, P, 0, ItemId.Invert);
            Use(rules, P, 0);
            var obs = rules.ToObservation(D);
            Assert.AreEqual(2.0 / 3.0, obs.ProbCurrentLive(), 1e-9);
        }

        [Test]
        public void Dealer_KnownBlank_ShootsSelf_KnownLive_ShootsPlayer()
        {
            var rules = MakeRules(MakeStage(), B, L);
            rules.State.currentPlayerId = D;
            rules.State.shotgun.Current.knownBy.Add(D);
            var d = DealerBrain.Decide(rules.ToObservation(D), rules.State.rng);
            Assert.AreEqual(DealerActionType.Fire, d.type);
            Assert.AreEqual(D, d.targetId);

            SetShells(rules, L, B);
            rules.State.shotgun.Current.knownBy.Add(D);
            d = DealerBrain.Decide(rules.ToObservation(D), rules.State.rng);
            Assert.AreEqual(P, d.targetId);
        }

        [Test]
        public void Dealer_NeverUsesBlockedItems_AndAlwaysEndsWithFire()
        {
            for (int seed = 0; seed < 50; seed++)
            {
                var rules = new GameRules(new GameState(GameMode.DoubleOrNothing, seed));
                var stage = MakeStage(4, 2, true, ItemId.Peek, ItemId.Eject, ItemId.Heal, ItemId.DoubleDamage,
                    ItemId.SkipTurn, ItemId.Steal, ItemId.RevealRandom, ItemId.GambleHeal, ItemId.Invert);
                stage.minItemsPerLoad = 4;
                stage.maxItemsPerLoad = 4;
                rules.StartStage(stage);
                rules.StartLoad();
                rules.State.currentPlayerId = D;

                int guard = 0;
                while (rules.State.stageOver == false && rules.State.currentPlayerId == D && rules.State.shotgun.IsEmpty == false && guard++ < 20)
                {
                    var d = DealerBrain.Decide(rules.ToObservation(D), rules.State.rng, guard < 15);
                    if (d.type == DealerActionType.UseItem)
                    {
                        Assert.IsTrue(rules.TryUseItem(D, d.slot, d.stealSlot, out _, out var reason), $"seed {seed}: {reason}");
                    }
                    else
                    {
                        Assert.IsTrue(rules.CanFire(D, d.targetId));
                        rules.Fire(D, d.targetId);
                        break;
                    }
                }

                Assert.Less(guard, 20, "AI bị lặp");
            }
        }

        #endregion
    }
}
