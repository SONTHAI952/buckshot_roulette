using Game.Primary.GamePlaySystem.Rules;
using UnityEngine;

namespace Game.Primary.CampaignModeSystem
{
    /// <summary>
    /// Đọc Resources/Levels/level_{n}.json -> LevelData
    /// </summary>
    public static class LevelLoader
    {
        public const int StoryLevel = 1;
        public const int DoubleOrNothingLevel = 2;

        private const string PathFormat = "Levels/level_{0}";


        public static LevelData Load(int level)
        {
            var textAsset = Resources.Load<TextAsset>(string.Format(PathFormat, level));
            if (textAsset == null)
            {
                Debug.LogError($"LevelLoader: không tìm thấy level {level} ({string.Format(PathFormat, level)})");
                return null;
            }

            var data = JsonUtility.FromJson<LevelData>(textAsset.text);
            if (data == null || data.stages == null || data.stages.Count == 0)
            {
                Debug.LogError($"LevelLoader: level {level} không có stage nào");
                return null;
            }

            return data;
        }

        public static bool Exists(int level)
        {
            return Resources.Load<TextAsset>(string.Format(PathFormat, level)) != null;
        }
    }
}
