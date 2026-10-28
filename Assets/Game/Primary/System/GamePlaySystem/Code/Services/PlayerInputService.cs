using System.Collections.Generic;
using Game.Primary.GamePlaySystem.Rules;
using Game.Primary.TableSystem;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Game.Primary.GamePlaySystem
{
    /// <summary>
    /// Nhận thao tác của người chơi (chạm/click item, Wand, Mistress, nút HUD) và chuyển thành một PlayerAction.
    /// Không tự áp dụng luật: TurnFlowService lấy action và gửi cho GameRules.
    /// </summary>
    public class PlayerInputService : MonoBehaviour
    {
        private enum Mode
        {
            Disabled,
            Choosing,
            Aiming,
            Stealing
        }

        private GamePlayCore core;
        private Mode mode = Mode.Disabled;
        private ItemView selected;
        private int stealItemSlot = -1;
        private float stealTimeLeft;
        private PlayerAction? pending;
        private readonly List<int> stealableSlots = new List<int>();


        public bool IsWaitingAction => mode != Mode.Disabled;



        public void Setup(GamePlayCore core)
        {
            this.core = core;
            core.Table.OnItemClicked += Table_OnItemClicked;
            core.Table.OnItemHover += Table_OnItemHover;
            core.Wand.OnClicked += Wand_OnClicked;
            core.Mistress.OnClicked += Mistress_OnClicked;
            core.Hud.OnUseClicked += Hud_OnUseClicked;
            core.Hud.OnZapHerClicked += () => SubmitFire(PlayerIds.Dealer);
            core.Hud.OnZapSelfClicked += () => SubmitFire(PlayerIds.Player);
            core.Hud.OnCancelAimClicked += CancelAim;
        }

        public void BeginTurn()
        {
            pending = null;
            mode = Mode.Choosing;
            core.Wand.Interactable = true;
            core.Hud.ShowHint(GameTexts.PickWandHint);
        }

        public void EndTurn()
        {
            mode = Mode.Disabled;
            Deselect();
            stealItemSlot = -1;
            stealableSlots.Clear();
            if (core == null)
                return;

            core.Wand.Interactable = false;
            core.Table.SetStealable(null);
            core.Table.ClearHighlights();
            core.Hud.ShowAim(false);
            core.Hud.ShowSteal(false);
            core.Hud.HideItemCard();
        }

        public bool TryConsumeAction(out PlayerAction action)
        {
            if (pending.HasValue)
            {
                action = pending.Value;
                pending = null;
                return true;
            }

            action = default;
            return false;
        }



        private void Update()
        {
            if (core == null || core.IsPaused)
                return;

            if (mode == Mode.Stealing)
            {
                stealTimeLeft -= Time.deltaTime;
                core.Hud.SetStealTimer(stealTimeLeft / Mathf.Max(0.1f, core.Config.stealTimeLimit));
                if (stealTimeLeft <= 0)
                    Submit(PlayerAction.Forfeit(stealItemSlot));
            }

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (mode == Mode.Aiming)
                    CancelAim();
                else if (mode == Mode.Choosing)
                    Deselect();
            }
#endif
        }



        #region Handlers

        private void Table_OnItemClicked(ItemView item)
        {
            if (core == null || core.Rules == null || core.IsPaused)
                return;

            if (mode == Mode.Stealing)
            {
                if (item.OwnerId == PlayerIds.Dealer && stealableSlots.Contains(item.Slot))
                    Submit(PlayerAction.Use(stealItemSlot, item.Slot));
                else
                    GameToast.Show(GameTexts.BlockReason(ItemBlockReason.CannotStealThisItem));
                return;
            }

            if (mode == Mode.Aiming)
                CancelAim();

            if (selected == item && mode == Mode.Choosing && item.OwnerId == PlayerIds.Player)
            {
                TryUse(item);
                return;
            }

            Select(item);
        }

        private void Table_OnItemHover(ItemView item, bool enter)
        {
            if (core == null || core.Rules == null || mode == Mode.Stealing || selected != null)
                return;

            if (enter)
                ShowCard(item, false);
            else
                core.Hud.HideItemCard();
        }

        private void Wand_OnClicked()
        {
            if (mode != Mode.Choosing)
                return;

            Deselect();
            mode = Mode.Aiming;
            core.Hud.ShowAim(true);
            core.Wand.SetHighlight(true);
        }

        private void Mistress_OnClicked()
        {
            if (mode == Mode.Aiming)
                SubmitFire(PlayerIds.Dealer);
        }

        private void Hud_OnUseClicked()
        {
            if (selected != null && mode == Mode.Choosing)
                TryUse(selected);
        }

        #endregion



        #region Logic

        private void Select(ItemView item)
        {
            Deselect();
            selected = item;
            item.SetSelected(true);
            ShowCard(item, true);
        }

        private void ShowCard(ItemView item, bool allowUse)
        {
            var def = core.Config.GetItem(item.ItemId);
            string blocked = null;
            bool canUse = false;

            if (item.OwnerId == PlayerIds.Player && mode == Mode.Choosing)
            {
                var reason = core.Rules.CanUseItem(PlayerIds.Player, item.Slot);
                if (reason != ItemBlockReason.None)
                    blocked = GameTexts.BlockReason(reason);
                canUse = allowUse && reason == ItemBlockReason.None;
            }
            else if (item.OwnerId == PlayerIds.Dealer)
            {
                blocked = core.Config.dealerName + "'s item";
            }

            core.Hud.ShowItemCard(def, blocked, canUse);
        }

        private void Deselect()
        {
            if (selected != null)
                selected.SetSelected(false);

            selected = null;
            if (core != null)
                core.Hud.HideItemCard();
        }

        private void TryUse(ItemView item)
        {
            var reason = core.Rules.CanUseItem(PlayerIds.Player, item.Slot);
            if (reason != ItemBlockReason.None)
            {
                GameToast.Show(GameTexts.BlockReason(reason));
                return;
            }

            if (item.ItemId == ItemId.Steal)
            {
                BeginSteal(item.Slot);
                return;
            }

            Submit(PlayerAction.Use(item.Slot));
        }

        private void BeginSteal(int slot)
        {
            Deselect();
            mode = Mode.Stealing;
            stealItemSlot = slot;
            stealTimeLeft = core.Config.stealTimeLimit;
            stealableSlots.Clear();
            stealableSlots.AddRange(core.Rules.GetStealableSlots(PlayerIds.Player));
            core.Table.SetStealable(stealableSlots);
            core.Wand.Interactable = false;
            core.Hud.ShowSteal(true);
            core.Hud.ShowHint(GameTexts.StealHint);
        }

        private void CancelAim()
        {
            if (mode != Mode.Aiming)
                return;

            mode = Mode.Choosing;
            core.Hud.ShowAim(false);
            core.Wand.SetHighlight(false);
        }

        private void SubmitFire(int targetId)
        {
            if (mode != Mode.Aiming)
                return;

            Submit(PlayerAction.Fire(targetId));
        }

        private void Submit(PlayerAction action)
        {
            if (mode == Mode.Disabled)
                return;

            pending = action;
            EndTurn();
        }

        #endregion
    }
}
