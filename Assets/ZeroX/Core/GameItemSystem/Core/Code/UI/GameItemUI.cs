using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameItemSystem.UI
{
    public class GameItemUI : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private Image imageIcon;
        [SerializeField] private TMP_Text textAmount;
        [SerializeField] private string amountFormat = "{0}";

        public string Id => gameItemData == null ? id : gameItemData.id;
        [NonSerialized] public GameItemData gameItemData;


        public Sprite Icon
        {
            get => imageIcon.sprite;
            set => imageIcon.sprite = value;
        }
        
        public virtual void UpdateUI()
        {
            id = gameItemData.id;
            imageIcon.sprite = gameItemData.Icon;
            textAmount.text = string.Format(amountFormat, gameItemData.Amount);
        }
    }
}