using Game.Primary.CampaignModeSystem;
using Game.Primary.RunSystem;
using TMPro;
using UnityEngine;
using ZeroX.ButtonSystem;
using ZeroX.WindowSystem;

namespace Game.Primary.UI
{
    /// <summary>
    /// Xác nhận 18+ ở lần mở game đầu tiên
    /// </summary>
    public class PopupAgeGate : WindowController<PopupAgeGate>
    {
        [SerializeField] private TMP_Text textTitle;
        [SerializeField] private TMP_Text textBody;
        [SerializeField] private ZButton buttonConfirm;
        [SerializeField] private ZButton buttonLeave;


        protected override void WhenAwake()
        {
            buttonConfirm.onClick.AddListener(ButtonConfirm_OnClick);
            buttonLeave.onClick.AddListener(ButtonLeave_OnClick);
        }

        private void ButtonConfirm_OnClick()
        {
            RunDI.AgeConfirmed = true;
            Close();
            if (CampaignMode.Initialized)
                CampaignMode.Instance.OpenHome();
        }

        private void ButtonLeave_OnClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
