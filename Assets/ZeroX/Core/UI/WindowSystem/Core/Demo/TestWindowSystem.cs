using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.WindowSystem.Demo
{
    public class TestWindowSystem : MonoBehaviour
    {
        [SerializeField] private Button buttonTestPopup;
        
        [SerializeField] private Button buttonOpenPopupA;
        [SerializeField] private Button buttonClosePopupA;
        
        [SerializeField] private Button buttonOpenPopupB;
        [SerializeField] private Button buttonClosePopupB;
        
        [SerializeField] private Button buttonOpenPopupC;
        [SerializeField] private Button buttonClosePopupC;
        
        [SerializeField] private Window popupA;
        [SerializeField] private Window popupB;
        [SerializeField] private Window popupC;
        
       

        private void Awake()
        {
            buttonTestPopup.onClick.AddListener(ButtonTestPopup_OnClick);
            
            buttonOpenPopupA.onClick.AddListener(popupA.Open);
            buttonClosePopupA.onClick.AddListener(popupA.Close);
            
            buttonOpenPopupB.onClick.AddListener(popupB.Open);
            buttonClosePopupB.onClick.AddListener(popupB.Close);
            
            buttonOpenPopupC.onClick.AddListener(popupC.Open);
            buttonClosePopupC.onClick.AddListener(popupC.Close);
        }

        private void Start()
        {
            
        }

        private void ButtonTestPopup_OnClick()
        {
            //PopupSetting.Instance.Open();
            //PopupRewardsReceivedManager.Instance.Show("coin", 100, false);
            //PopupColorPicker.Instance.Open();
            //PopupSelectLanguage.Instance.Open();
            //PopupGDPRManager.Instance.Open().OnFinish(wt => Debug.Log("GDPR: " + GdprDI.GdprStatus));
            //PopupLoadingRateManager.Instance.Open();
            //PopupRateManager.Instance.Open();
            //ToastMultiple.Show("Hello World!");
            
            popupA.Open();
            popupB.Open();
        }
    }
}