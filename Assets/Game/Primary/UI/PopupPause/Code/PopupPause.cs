using Game.Primary.CampaignModeSystem;
using UnityEngine;
using ZeroX.ButtonSystem;
using ZeroX.WindowSystem;

namespace Game.Primary.UI
{
    public class PopupPause : WindowController<PopupPause>
    {
        [SerializeField] private ZButton buttonResume;
        [SerializeField] private ZButton buttonSetting;
        [SerializeField] private ZButton buttonQuit;

        private bool quitting;


        protected override void WhenAwake()
        {
            buttonResume.onClick.AddListener(Close);
            buttonSetting.onClick.AddListener(() => global::UI.PopupSetting.Instance.Open());
            buttonQuit.onClick.AddListener(() =>
            {
                quitting = true;
                Close();
                CampaignMode.Instance.QuitToHome();
            });
        }

        protected override void Window_OnOpen()
        {
            quitting = false;
        }

        protected override void Window_OnClose()
        {
            if (quitting == false && CampaignMode.Initialized)
                CampaignMode.Instance.ResumeGame();
        }
    }
}
