using System;
using System.Collections.Generic;
using Game.Primary.GamePlaySystem.Rules;
using UnityEngine;

namespace Game.Primary.GamePlaySystem
{
    [Serializable]
    public class ItemDefinition
    {
        public ItemId id;
        public string displayName;
        [TextArea(2, 4)] public string description;
        public Color color = Color.white;
        public GameObject prefab;   //Prefab có ItemView
    }

    /// <summary>
    /// Toàn bộ thông số trình bày có thể chỉnh: tên/mô tả item theo theme, màu, thời gian animation...
    /// Luật chơi (charge, load, item pool, tiền) nằm trong level json.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Primary/Gameplay Config", fileName = "Gameplay Config")]
    public class GameplayConfig : ScriptableObject
    {
        [Header("Names")]
        public string gameTitle = "TEASE ROULETTE";
        public string playerName = "YOU";
        public string dealerName = "MISTRESS";

        [Header("Shells")]
        public string liveName = "HOT";
        public string blankName = "COLD";
        public Color liveColor = new Color(1f, 0.16f, 0.48f);
        public Color blankColor = new Color(0.45f, 0.75f, 1f);

        [Header("Items")]
        public List<ItemDefinition> items = new List<ItemDefinition>();

        [Header("Timing")]
        public float dealerThinkMin = 0.7f;
        public float dealerThinkMax = 1.3f;
        public float stealTimeLimit = 8f;
        public float shellShowDuration = 2.2f;
        public float bannerDuration = 1.6f;
        public int dealerMaxItemUsesPerTurn = 10;

        [Header("Debug")]
        [Tooltip("0 = random seed mỗi run")]
        public int fixedSeed = 0;
        public bool showShellOrderInDevelopmentBuild = true;


        public ItemDefinition GetItem(ItemId itemId)
        {
            foreach (var item in items)
            {
                if (item.id == itemId)
                    return item;
            }

            return null;
        }

        public string GetItemName(ItemId itemId)
        {
            var item = GetItem(itemId);
            return item != null ? item.displayName : itemId.ToString();
        }

        public string ShellName(ShellType type) => type == ShellType.Live ? liveName : blankName;
        public Color ShellColor(ShellType type) => type == ShellType.Live ? liveColor : blankColor;
        public string NameOf(int playerId) => playerId == PlayerIds.Player ? playerName : dealerName;
    }
}
