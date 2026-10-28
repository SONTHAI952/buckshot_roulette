using System;
using TMPro;
using UnityEngine;

namespace GameItemSystem.UI
{
    public class GameItemAmountDisplayer : MonoBehaviour
    {
        public string id;
        [SerializeField] private TMP_Text textAmount;
        [SerializeField] private string textAmountFormat = "{0}";


        private void Awake()
        {
            InventoryDI.OnGameItemChanged.SubscribeUntilDestroy(OnGameItemChanged, this);
        }

        private void Start()
        {
            textAmount.text = string.Format(textAmountFormat, InventoryDI.GetAmount(id));
        }

        private void OnGameItemChanged(OnGameItemChangedData data)
        {
            if(data.id != id)
                return;

            textAmount.text = string.Format(textAmountFormat, InventoryDI.GetAmount(id));
        }
    }
}