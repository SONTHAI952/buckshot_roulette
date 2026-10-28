using System;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.SafeAreaSystem.Demo
{
    public class DemoSafeArea : MonoBehaviour
    {
        [SerializeField] private Button buttonTurnBanner;
        [SerializeField] private GameObject fakeBanner;


        private void Awake()
        {
            buttonTurnBanner.onClick.AddListener(ButtonTurnBanner_OnClick);
        }

        private void Start()
        {
            SafeAreaGlobalConfig.bannerArea.enable = fakeBanner.activeSelf;
        }

        private void ButtonTurnBanner_OnClick()
        {
            fakeBanner.SetActive(!fakeBanner.activeSelf);
            SafeAreaGlobalConfig.bannerArea.enable = fakeBanner.activeSelf;
        }
    }
}