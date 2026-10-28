using System;
using System.Collections;
using System.Collections.Generic;
using Game.Primary.GamePlaySystem.Rules;
using UnityEngine;

namespace Game.Primary.TableSystem
{
    /// <summary>
    /// Mặt bàn: 8 slot item mỗi bên. Chỉ quản lý các ItemView là con của nó.
    /// </summary>
    public class TableView : MonoBehaviour
    {
        [SerializeField] private Transform[] playerSlots = new Transform[RuleConst.InventorySlots];
        [SerializeField] private Transform[] dealerSlots = new Transform[RuleConst.InventorySlots];
        [SerializeField] private Transform playerItemSpawn;
        [SerializeField] private Transform dealerItemSpawn;
        [SerializeField] private Transform playerUseFocus;
        [SerializeField] private Transform dealerUseFocus;
        [SerializeField] private Transform itemRoot;

        private readonly ItemView[] playerItems = new ItemView[RuleConst.InventorySlots];
        private readonly ItemView[] dealerItems = new ItemView[RuleConst.InventorySlots];


        public event Action<ItemView> OnItemClicked;
        public event Action<ItemView, bool> OnItemHover;

        public Transform PlayerUseFocus => playerUseFocus;
        public Transform DealerUseFocus => dealerUseFocus;



        private ItemView[] ItemsOf(int ownerId) => ownerId == PlayerIds.Player ? playerItems : dealerItems;
        private Transform[] SlotsOf(int ownerId) => ownerId == PlayerIds.Player ? playerSlots : dealerSlots;
        public Transform UseFocusOf(int actorId) => actorId == PlayerIds.Player ? playerUseFocus : dealerUseFocus;

        public ItemView Get(int ownerId, int slot)
        {
            var items = ItemsOf(ownerId);
            return slot >= 0 && slot < items.Length ? items[slot] : null;
        }

        public IEnumerable<ItemView> AllItems()
        {
            foreach (var item in playerItems)
                if (item != null) yield return item;
            foreach (var item in dealerItems)
                if (item != null) yield return item;
        }

        public void NotifyItemClicked(ItemView item) => OnItemClicked?.Invoke(item);
        public void NotifyItemHover(ItemView item, bool enter) => OnItemHover?.Invoke(item, enter);



        public ItemView Spawn(int ownerId, int slot, GameObject prefab)
        {
            var items = ItemsOf(ownerId);
            if (items[slot] != null)
                Destroy(items[slot].gameObject);

            var anchor = SlotsOf(ownerId)[slot];
            var go = Instantiate(prefab, anchor.position, anchor.rotation, itemRoot != null ? itemRoot : transform);
            go.SetActive(true);
            var view = go.GetComponent<ItemView>();
            view.Bind(this, ownerId, slot);
            items[slot] = view;
            return view;
        }

        public IEnumerator PlayDeal(int ownerId, List<int> slots, List<ItemId> itemIds, Func<ItemId, GameObject> getPrefab)
        {
            var spawn = ownerId == PlayerIds.Player ? playerItemSpawn : dealerItemSpawn;
            ItemView last = null;
            for (int i = 0; i < slots.Count; i++)
            {
                var prefab = getPrefab(itemIds[i]);
                if (prefab == null)
                    continue;

                last = Spawn(ownerId, slots[i], prefab);
                StartCoroutine(last.PlaySpawn(spawn.position, i * 0.12f));
            }

            if (last != null)
                yield return new WaitForSeconds(slots.Count * 0.12f + 0.5f);
        }

        /// <summary> Lấy item ra khỏi bàn để phát animation dùng </summary>
        public ItemView Take(int ownerId, int slot)
        {
            var items = ItemsOf(ownerId);
            var item = items[slot];
            items[slot] = null;
            return item;
        }

        public void ClearAll()
        {
            for (int i = 0; i < RuleConst.InventorySlots; i++)
            {
                if (playerItems[i] != null)
                    Destroy(playerItems[i].gameObject);
                if (dealerItems[i] != null)
                    Destroy(dealerItems[i].gameObject);

                playerItems[i] = null;
                dealerItems[i] = null;
            }
        }

        public void ClearHighlights()
        {
            foreach (var item in AllItems())
                item.ClearHighlight();
        }

        public void SetStealable(ICollection<int> dealerSlotIndexes)
        {
            for (int i = 0; i < dealerItems.Length; i++)
            {
                if (dealerItems[i] != null)
                    dealerItems[i].SetStealable(dealerSlotIndexes != null && dealerSlotIndexes.Contains(i));
            }
        }
    }
}
