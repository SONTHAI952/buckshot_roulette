using Game.Primary.CampaignModeSystem;
using Game.Primary.RunSystem;
using TMPro;
using UnityEngine;
using ZeroX.ButtonSystem;
using ZeroX.WindowSystem;

namespace Game.Primary.UI
{
    /// <summary>
    /// Menu chính: chọn Story / Double or Nothing, mở Setting, xem thành tích
    /// </summary>
    public class PopupHome : WindowController<PopupHome>
    {
        [SerializeField] private TMP_Text textTitle;
        [SerializeField] private TMP_Text textStats;
        [SerializeField] private ZButton buttonStory;
        [SerializeField] private ZButton buttonDoubleOrNothing;
        [SerializeField] private ZButton buttonSetting;


        protected override void WhenAwake()
        {
            buttonStory.onClick.AddListener(() => Play(LevelLoader.StoryLevel));
            buttonDoubleOrNothing.onClick.AddListener(() => Play(LevelLoader.DoubleOrNothingLevel));
            buttonSetting.onClick.AddListener(() => global::UI.PopupSetting.Instance.Open());
        }

        protected override void Window_OnOpen()
        {
            textStats.text =
                $"Story wins: {RunDI.StoryWins}    Best tips: ${RunDI.StoryBestCash:N0}\n" +
                $"Double or Nothing best: ${RunDI.DoubleOrNothingBestCash:N0}  ({RunDI.DoubleOrNothingBestSets} sets)";
        }

        private void Play(int level)
        {
            Close();
            CampaignMode.Instance.PlayLevel(level);
        }
    }
}
