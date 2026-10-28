using System;
using System.Collections;
using System.Collections.Generic;
using Game.Primary.GamePlaySystem.Rules;
using UnityEngine;

namespace Game.Primary.GamePlaySystem
{
    /// <summary>
    /// State machine của một run: stage -> load -> lượt. Gửi hành động cho GameRules rồi phát lại RuleEvent qua các view.
    /// Đầu vào bị khoá trong lúc phát animation (PlayerInputService chỉ mở khi tới lượt người chơi).
    /// </summary>
    public class TurnFlowService : MonoBehaviour
    {
        private GamePlayCore core;
        private int dealerItemUses;


        private GameRules Rules => core.Rules;
        private GameState State => core.State;
        private GameplayConfig Config => core.Config;

        public bool IsRunning { get; private set; }



        public void Setup(GamePlayCore core)
        {
            this.core = core;
        }

        private void Update()
        {
            if (IsRunning && core.IsPaused == false && State != null)
                State.stats.elapsedSeconds += Time.deltaTime;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        /// <summary> Chờ theo thời gian game, đứng yên khi pause </summary>
        private IEnumerator Wait(float seconds)
        {
            float t = 0;
            while (t < seconds)
            {
                if (core.IsPaused == false)
                    t += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator WhilePaused()
        {
            while (core.IsPaused)
                yield return null;
        }



        #region Run

        public IEnumerator RunGame()
        {
            IsRunning = true;
            var level = core.LevelData;
            core.Hud.SetVisible(true);
            core.Hud.SetCash("");

            if (level.GetMode() == GameMode.DoubleOrNothing)
                yield return RunDoubleOrNothing(level);
            else
                yield return RunStory(level);

            IsRunning = false;
        }

        private IEnumerator RunStory(LevelData level)
        {
            bool wonPrevious = true;
            int cleared = 0;
            for (int i = 0; i < level.stages.Count; i++)
            {
                var stage = StageRules.FromData(level.stages[i], State.rng);
                core.Hud.SetStage(GameTexts.Stage(i, level.stages.Count), level.displayName);
                yield return RunStage(stage);

                if (State.stageWinnerId == PlayerIds.Player)
                {
                    wonPrevious = true;
                    cleared++;
                    continue;
                }

                if (stage.fatal)
                {
                    FinishRun(false, 0, cleared, 0);
                    yield break;
                }

                int penalty = Rules.UseSafeWord(wonPrevious);
                wonPrevious = false;
                yield return PresentSafeWord(penalty);
            }

            int cash = PayoutCalculator.CalculateStory(level.economy, State.stats);
            FinishRun(true, cash, cleared, 1);
        }

        private IEnumerator RunDoubleOrNothing(LevelData level)
        {
            int setIndex = 0;
            int atRisk = 0;
            while (true)
            {
                int healsBefore = State.stats.healsUsed;
                int ejectsBefore = State.stats.ejectItemsUsed;
                float timeBefore = State.stats.elapsedSeconds;

                for (int i = 0; i < level.stages.Count; i++)
                {
                    var stage = StageRules.FromData(level.stages[i], State.rng);
                    core.Hud.SetStage(GameTexts.Stage(i, level.stages.Count), $"{level.displayName}  SET {setIndex + 1}");
                    core.Hud.SetCash(atRisk > 0 ? $"AT RISK ${atRisk:N0}" : "");
                    yield return RunStage(stage);

                    if (State.stageWinnerId != PlayerIds.Player)
                    {
                        //Không hồi sinh, mất toàn bộ tiền đang đặt cược
                        FinishRun(false, 0, setIndex * level.stages.Count + i, setIndex);
                        yield break;
                    }
                }

                int setCash = PayoutCalculator.CalculateDoubleOrNothingSet(level.economy,
                    State.stats.healsUsed - healsBefore,
                    State.stats.ejectItemsUsed - ejectsBefore,
                    State.stats.elapsedSeconds - timeBefore,
                    setIndex);
                atRisk += setCash;

                bool decided = false;
                bool continuePlaying = false;
                var offer = new DoubleOrNothingOffer(value =>
                {
                    decided = true;
                    continuePlaying = value;
                })
                {
                    setIndex = setIndex,
                    setCash = setCash,
                    atRiskCash = atRisk,
                    nextSetMultiplier = Mathf.RoundToInt(Mathf.Pow(level.economy.continueMultiplier, setIndex + 1))
                };

                core.Hud.SetCash($"AT RISK ${atRisk:N0}");
                core.RaiseDoubleOrNothingOffer(offer);
                while (decided == false)
                    yield return null;

                if (continuePlaying == false)
                {
                    FinishRun(true, atRisk, (setIndex + 1) * level.stages.Count, setIndex + 1);
                    yield break;
                }

                setIndex++;
                yield return core.Hud.ShowBanner("DOUBLE OR NOTHING", $"Set {setIndex + 1} pays x{offer.nextSetMultiplier}", Config.liveColor, Config.bannerDuration);
            }
        }

        private void FinishRun(bool won, int cash, int stagesCleared, int setsCleared)
        {
            IsRunning = false;
            var result = new RunResult
            {
                mode = core.LevelData.GetMode(),
                won = won,
                cash = cash,
                stagesCleared = stagesCleared,
                setsCleared = setsCleared,
                stats = State.stats,
                economy = core.LevelData.economy
            };

            core.FinishRun(result);
        }

        #endregion



        #region Stage - Turn

        private IEnumerator RunStage(StageRules stage)
        {
            yield return Present(Rules.StartStage(stage));

            while (State.stageOver == false)
            {
                yield return WhilePaused();

                if (State.shotgun.IsEmpty)
                {
                    yield return Present(Rules.StartLoad());
                    continue;
                }

                if (State.currentPlayerId == PlayerIds.Player)
                    yield return PlayerTurnStep();
                else
                    yield return DealerTurnStep();
            }

            yield return Wait(0.6f);
        }

        private IEnumerator PlayerTurnStep()
        {
            core.Hud.SetTurn(true);
            core.PlayerInput.BeginTurn();

            PlayerAction action;
            while (core.PlayerInput.TryConsumeAction(out action) == false)
                yield return null;

            core.PlayerInput.EndTurn();
            core.Hud.ShowHint("");

            switch (action.type)
            {
                case PlayerActionType.UseItem:
                {
                    if (Rules.TryUseItem(PlayerIds.Player, action.slot, action.stealSlot, out var events, out var reason))
                        yield return Present(events);
                    else
                        GameToast.Show(GameTexts.BlockReason(reason));
                    break;
                }

                case PlayerActionType.ForfeitSteal:
                {
                    if (Rules.ForfeitSteal(PlayerIds.Player, action.slot, out var events))
                        yield return Present(events);
                    break;
                }

                case PlayerActionType.Fire:
                    yield return PresentFire(PlayerIds.Player, action.targetId);
                    break;
            }
        }

        private IEnumerator DealerTurnStep()
        {
            core.Hud.SetTurn(false);
            yield return Wait(UnityEngine.Random.Range(Config.dealerThinkMin, Config.dealerThinkMax));

            bool allowItems = dealerItemUses < Config.dealerMaxItemUsesPerTurn;
            var decision = DealerBrain.Decide(Rules.ToObservation(PlayerIds.Dealer), State.rng, allowItems);

            if (decision.type == DealerActionType.UseItem)
            {
                if (Rules.TryUseItem(PlayerIds.Dealer, decision.slot, decision.stealSlot, out var events, out _))
                {
                    dealerItemUses++;
                    yield return Present(events);
                    yield break;
                }

                //Quyết định không hợp lệ -> buộc bắn để không bị kẹt
                dealerItemUses = Config.dealerMaxItemUsesPerTurn;
                decision = DealerBrain.Decide(Rules.ToObservation(PlayerIds.Dealer), State.rng, false);
            }

            yield return PresentFire(PlayerIds.Dealer, decision.targetId);
        }

        private IEnumerator PresentFire(int shooterId, int targetId)
        {
            if (Rules.CanFire(shooterId, targetId) == false)
                yield break;

            if (shooterId == PlayerIds.Dealer)
            {
                core.Mistress.PlayShoot();
                core.Mistress.Say(targetId == PlayerIds.Dealer ? "Watch me..." : "Your turn to squirm.", 1.6f);
            }

            core.Sfx.PlayTension();
            yield return core.Wand.Aim(shooterId, targetId);
            yield return Present(Rules.Fire(shooterId, targetId));

            if (State.stageOver == false)
                yield return core.Wand.Return();
        }

        private IEnumerator PresentSafeWord(int penalty)
        {
            string sub = penalty > 0 ? $"{GameTexts.SafeWordSub}\n-${penalty:N0}" : GameTexts.SafeWordSub;
            yield return core.Mistress.PlayRevive();
            yield return core.Hud.ShowBanner(GameTexts.SafeWord, sub, Config.blankColor, Config.bannerDuration + 0.6f);
        }

        #endregion



        #region Present

        private IEnumerator Present(List<RuleEvent> events)
        {
            foreach (var e in events)
            {
                yield return WhilePaused();
                yield return Safe(PresentEvent(e));
                core.Hud.RefreshShellInfo(State, Config);
                core.Hud.SetDebug(State, Config);
            }
        }

        /// <summary>
        /// Chạy coroutine trình diễn, nếu view lỗi thì log và bỏ qua thay vì làm chết cả flow
        /// (state của GameRules đã được cập nhật trước nên game vẫn đúng luật).
        /// </summary>
        private static IEnumerator Safe(IEnumerator routine)
        {
            while (true)
            {
                object current;
                try
                {
                    if (routine.MoveNext() == false)
                        yield break;
                    current = routine.Current;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    yield break;
                }

                if (current is IEnumerator nested)
                    yield return Safe(nested);
                else
                    yield return current;
            }
        }

        private IEnumerator PresentEvent(RuleEvent e)
        {
            switch (e)
            {
                case StageStartedEvent stageStarted:
                    yield return OnStageStarted(stageStarted);
                    break;

                case LoadStartedEvent loadStarted:
                    dealerItemUses = 0;
                    core.Hud.ShowHint(GameTexts.Reloading);
                    core.Sfx.PlayLoad();
                    yield return core.ShellDisplay.PlayLoad(loadStarted.live, loadStarted.blank, Config.liveColor, Config.blankColor,
                        Config.shellShowDuration, core.Wand.EjectPoint);
                    core.Hud.ShowHint("");
                    break;

                case ItemsDealtEvent dealt:
                    core.Sfx.PlayDeal();
                    yield return core.Table.PlayDeal(dealt.playerId, dealt.slots, dealt.items, id => Config.GetItem(id)?.prefab);
                    if (dealt.discarded > 0 && dealt.playerId == PlayerIds.Player)
                        GameToast.Show($"No room for {dealt.discarded} more item(s)");
                    break;

                case EffectsResetEvent _:
                    core.Wand.SetBoosted(false);
                    core.Mistress.SetCuffed(false);
                    core.Hud.SetCuffed(PlayerIds.Player, false);
                    core.Hud.SetCuffed(PlayerIds.Dealer, false);
                    break;

                case ItemUsedEvent used:
                    yield return OnItemUsed(used);
                    break;

                case ShellRevealedEvent revealed:
                    yield return OnShellRevealed(revealed);
                    break;

                case ShellEjectedEvent ejected:
                {
                    core.Sfx.PlayEject();
                    var land = core.Wand.RestAnchor.position + core.Wand.RestAnchor.right * 0.25f;
                    yield return core.Wand.Eject(ejected.type, Config.ShellColor(ejected.type), land);
                    core.Hud.ShowHintTemporary($"Popped out: {ColoredShell(ejected.type)}");
                    break;
                }

                case ShellInvertedEvent _:
                    yield return core.Wand.Invert(Config.liveColor);
                    core.Hud.ShowHintTemporary(GameTexts.Flipped + "!");
                    break;

                case BoostedEvent _:
                    core.Wand.SetBoosted(true);
                    core.Hud.ShowHintTemporary("Next HOT zap hits twice as hard");
                    yield return Wait(0.3f);
                    break;

                case CuffedEvent cuffed:
                    if (cuffed.targetId == PlayerIds.Dealer)
                    {
                        core.Mistress.SetCuffed(true);
                        core.Mistress.Say("Oh? Bold.", 1.5f);
                    }

                    core.Hud.SetCuffed(cuffed.targetId, true);
                    yield return Wait(0.5f);
                    break;

                case TurnSkippedEvent skipped:
                    GameToast.Show($"{Config.NameOf(skipped.skippedPlayerId)} is cuffed and skips a turn");
                    if (skipped.skippedPlayerId == PlayerIds.Dealer)
                        core.Mistress.SetCuffed(false);
                    core.Hud.SetCuffed(skipped.skippedPlayerId, false);
                    yield return Wait(0.8f);
                    break;

                case ChargesChangedEvent charges:
                    yield return OnChargesChanged(charges);
                    break;

                case HealBlockedEvent _:
                    GameToast.Show(GameTexts.HealingLocked);
                    yield return Wait(0.4f);
                    break;

                case HealingDisabledEvent disabled:
                {
                    var player = State.Get(disabled.playerId);
                    core.StaminaMonitor.SetCharges(disabled.playerId, player.charges, true);
                    core.Hud.SetHearts(disabled.playerId, player.charges, true, false);
                    core.StaminaMonitor.SetStatus(GameTexts.NoSafeWords);
                    GameToast.Show($"{Config.NameOf(disabled.playerId)}: {GameTexts.HealingLocked}");
                    yield return Wait(0.6f);
                    break;
                }

                case GambleResultEvent gamble:
                    GameToast.Show(gamble.success ? "Lucky pill! +2 stamina" : "Bad trip... -1 stamina");
                    yield return Wait(0.5f);
                    break;

                case ShotFiredEvent shot:
                    dealerItemUses = 0;
                    core.Sfx.PlayShot(shot.type);
                    yield return core.Wand.Fire(shot.type, Config.ShellColor(shot.type), shot.boosted);
                    if (shot.type == ShellType.Blank)
                        core.Hud.ShowHintTemporary($"{ColoredShell(ShellType.Blank)}... nothing happens");
                    break;

                case TurnChangedEvent turn:
                    dealerItemUses = 0;
                    core.Hud.SetTurn(turn.playerId == PlayerIds.Player);
                    break;

                case LoadEmptyEvent _:
                    yield return core.Wand.Return();
                    yield return Wait(0.3f);
                    break;

                case PlayerDownEvent down:
                    if (down.playerId == PlayerIds.Dealer)
                        yield return core.Mistress.PlayDown();
                    else
                        core.Hud.Flash(new Color(0.05f, 0, 0.02f, 1f), 1.6f);
                    break;

                case StageEndedEvent ended:
                {
                    bool won = ended.winnerId == PlayerIds.Player;
                    core.Sfx.PlayRoundEnd(won);
                    yield return core.Wand.Return();
                    yield return core.Hud.ShowBanner(won ? GameTexts.RoundWon : GameTexts.RoundLost, "",
                        won ? Config.liveColor : Config.blankColor, Config.bannerDuration);
                    break;
                }

                case StealForfeitedEvent forfeited:
                    GameToast.Show(forfeited.actorId == PlayerIds.Player ? "Too slow... the leash slipped" : "She hesitated");
                    break;
            }
        }

        private IEnumerator OnStageStarted(StageStartedEvent e)
        {
            core.Table.ClearAll();
            core.ShellDisplay.Clear();
            core.Wand.ResetToRest();
            core.Mistress.ResetState();
            core.StaminaMonitor.Setup(e.charges);
            core.StaminaMonitor.SetStatus(e.fatal ? GameTexts.NoSafeWords : "");
            core.Hud.SetupHearts(e.charges, Config.playerName, Config.dealerName);
            core.Hud.SetHearts(PlayerIds.Player, e.charges, false, false);
            core.Hud.SetHearts(PlayerIds.Dealer, e.charges, false, false);
            core.Hud.SetCuffed(PlayerIds.Player, false);
            core.Hud.SetCuffed(PlayerIds.Dealer, false);
            core.Hud.SetTurn(true);

            core.Sfx.PlayStageStart();
            string sub = e.fatal ? GameTexts.NoSafeWordsSub : $"{e.charges} stamina each";
            yield return core.Hud.ShowBanner($"ROUND {e.stageIndex + 1}", sub, e.fatal ? Config.liveColor : Color.white, Config.bannerDuration);
            core.Mistress.Say(e.fatal ? "No more safe words, darling." : "Let's play.", 2f);
        }

        private IEnumerator OnItemUsed(ItemUsedEvent e)
        {
            var view = core.Table.Take(e.ownerId, e.slot);
            string itemName = Config.GetItemName(e.item);
            var focus = core.Table.UseFocusOf(e.actorId).position;

            if (e.actorId == PlayerIds.Dealer)
            {
                core.Mistress.PlayUseItem();
                GameToast.Show(e.viaSteal ? $"She snatched your {itemName}!" : $"{Config.dealerName} uses {itemName}");
            }
            else if (e.viaSteal)
            {
                GameToast.Show($"You took her {itemName}");
            }

            core.Sfx.PlayItem(e.item);

            if (view != null)
            {
                if (e.viaSteal)
                    yield return view.PlayStolen(focus);

                yield return view.PlayUse(focus);
            }
            else
            {
                yield return Wait(0.3f);
            }
        }

        private IEnumerator OnShellRevealed(ShellRevealedEvent e)
        {
            if (e.toPlayerId == PlayerIds.Player)
            {
                string where = e.offset == 0 ? "The current charge" : $"The {GameTexts.Ordinal(e.offset + 1)} charge";
                core.Hud.ShowHintTemporary($"{where} is {ColoredShell(e.type)}", 2.5f);
                yield return Wait(1f);
            }
            else
            {
                core.Mistress.Say(e.offset == 0 ? "Mmm... interesting." : "I see what's coming.", 1.6f);
                yield return Wait(0.8f);
            }
        }

        private IEnumerator OnChargesChanged(ChargesChangedEvent e)
        {
            var player = State.Get(e.playerId);
            core.StaminaMonitor.SetCharges(e.playerId, e.newValue, player.healingDisabled);
            core.Hud.SetHearts(e.playerId, e.newValue, player.healingDisabled, true);

            if (e.delta < 0)
            {
                core.Sfx.PlayHurt(e.playerId);
                if (e.playerId == PlayerIds.Player)
                {
                    core.CameraController.Shake(0.06f * -e.delta);
                    core.Hud.Flash(new Color(Config.liveColor.r, Config.liveColor.g, Config.liveColor.b, 0.55f), 0.8f);
                    core.Haptic.PlayHit(-e.delta);
                    yield return Wait(0.7f);
                }
                else
                {
                    yield return core.Mistress.PlayHit(-e.delta);
                }
            }
            else if (e.delta > 0)
            {
                core.Sfx.PlayHeal();
                if (e.playerId == PlayerIds.Player)
                    core.Hud.Flash(new Color(Config.blankColor.r, Config.blankColor.g, Config.blankColor.b, 0.35f), 0.6f);
                yield return Wait(0.4f);
            }
        }

        private string ColoredShell(ShellType type)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(Config.ShellColor(type))}>{Config.ShellName(type)}</color>";
        }

        #endregion
    }
}
