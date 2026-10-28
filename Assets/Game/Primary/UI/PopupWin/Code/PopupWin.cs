using Game.Primary.CampaignModeSystem;
using Game.Primary.GamePlaySystem;
using Game.Primary.GamePlaySystem.Rules;
using TMPro;
using UnityEngine;
using ZeroX.ButtonSystem;
using ZeroX.WindowSystem;

namespace Game.Primary.UI
{
    public class PopupWin : WindowController<PopupWin>
    {
        [SerializeField] private TMP_Text textTitle;
        [SerializeField] private TMP_Text textCash;
        [SerializeField] private TMP_Text textDetail;
        [SerializeField] private ZButton buttonPlayAgain;
        [SerializeField] private ZButton buttonMenu;

        private RunResult result;


        protected override void WhenAwake()
        {
            buttonPlayAgain.onClick.AddListener(() =>
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

            var stats = result.stats;
            textTitle.text = result.mode == GameMode.Story ? "SHE SURRENDERS" : "CASHED OUT";
            textCash.text = $"${result.cash:N0}";

            if (result.mode == GameMode.Story)
            {
                var eco = result.economy;
                textDetail.text =
                    $"Base tips   ${eco.baseCash:N0}\n" +
                    $"Safe words used   {stats.revivalsUsed}   -${stats.revivalPenalty:N0}\n" +
                    $"Ice cubes   {stats.healsUsed}   -${stats.healsUsed * eco.healItemPenalty:N0}\n" +
                    $"Champagne   {stats.ejectItemsUsed}   -${stats.ejectItemsUsed * eco.ejectItemPenalty:N0}\n" +
                    $"Zaps fired   {stats.shotsFired}";
            }
            else
            {
                textDetail.text =
                    $"Sets cleared   {result.setsCleared}\n" +
                    $"Rounds won   {stats.stagesWon}\n" +
                    $"Zaps fired   {stats.shotsFired}\n" +
                    $"Time   {Mathf.FloorToInt(stats.elapsedSeconds / 60)}:{Mathf.FloorToInt(stats.elapsedSeconds % 60):00}";
            }
        }
    }
}
