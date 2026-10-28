using System;
using DataSystem.Money;
using UnityEngine;
using UnityEngine.UI;

namespace MoneyDisplayerSystem.Demo
{
    public class TestMoneyDisplayer : MonoBehaviour
    {
        [SerializeField] private Button buttonPlusCoin;
        [SerializeField] private MoneyDisplayer coinDisplayer;
        
        [Header("Use World Object")]
        [SerializeField] private Button buttonPlusCoinFromObjectInWorld;
        [SerializeField] private Camera cameraOfObjectInWorld;
        [SerializeField] private Transform emissionPointOnWorld;

        private void Awake()
        {
            buttonPlusCoin.onClick.AddListener(ButtonPlusCoin_OnClick);
            buttonPlusCoinFromObjectInWorld.onClick.AddListener(ButtonPlusCoinFromObjectInWorld_OnClick);
        }

        void ButtonPlusCoin_OnClick()
        {
            MoneyDI.Coin.Plus(50, "test", "test");
            coinDisplayer.ShowPlusEffect(buttonPlusCoin.transform);
        }

        void ButtonPlusCoinFromObjectInWorld_OnClick()
        {
            MoneyDI.Coin.Plus(50, "test", "test");
            coinDisplayer.ShowPlusEffect(cameraOfObjectInWorld, emissionPointOnWorld.position);
        }
    }
}