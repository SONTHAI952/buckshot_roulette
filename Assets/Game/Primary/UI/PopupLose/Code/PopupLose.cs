using Game.Primary.CampaignModeSystem;
using Game.Primary.GamePlaySystem;
using Game.Primary.GamePlaySystem.Rules;
using TMPro;
using UnityEngine;
using ZeroX.ButtonSystem;
using ZeroX.WindowSystem;

namespace Game.Primary.UI
{
    public class PopupLose : WindowController<PopupLose>
    {
        [SerializeField] private TMP_Text textTitle;
        [SerializeField] private TMP_Text textDetail;
        [SerializeField] private ZButton buttonRetry;
        [SerializeField] private ZButton buttonMenu;

        private RunResult result;


        protected override void WhenAwake()
        {
            buttonRetry.onClick.AddListener(() =>
            {
                Close();
                CampaignMode.Instance.Replay();
            });

            buttonMenu.onClick.AddListener(() =>
            {
                Close();
                CampaignMode.Instance.QuitToHome();
            });
        }

        public void Open(RunResult result)
        {
            this.result = result;
            Open();
        }

        protected override void Window_OnOpen()
        {
            if (result == null)
                return;

            textTitle.text = "YOU BROKE";
            if (result.mode == GameMode.Story)
            {
                textDetail.text =
                    "No safe words left.\nShe wins this time.\n\n" +
                    $"Rounds won   {result.stats.stagesWon}\n" +
                    $"Zaps fired   {result.stats.shotsFired}";
            }
            else
            {
                textDetail.text =
                    "Double or nothing... nothing.\nAll tips at risk are gone.\n\n" +
                    $"Sets cleared   {result.setsCleared}\n" +
                    $"Rounds won   {result.stats.stagesWon}";
            }
        }
    }
}
