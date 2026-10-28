using Game.Primary.GamePlaySystem;
using TMPro;
using UnityEngine;
using ZeroX.ButtonSystem;
using ZeroX.WindowSystem;

namespace Game.Primary.UI
{
    /// <summary>
    /// Sau mỗi set 3 round: Cash Out (chốt tiền) hoặc Double (chơi tiếp, thua là mất hết)
    /// </summary>
    public class PopupDoubleOrNothing : WindowController<PopupDoubleOrNothing>
    {
        [SerializeField] private TMP_Text textTitle;
        [SerializeField] private TMP_Text textCash;
        [SerializeField] private TMP_Text textDetail;
        [SerializeField] private ZButton buttonCashOut;
        [SerializeField] private ZButton buttonDouble;

        private DoubleOrNothingOffer offer;


        protected override void WhenAwake()
        {
            buttonCashOut.onClick.AddListener(() => Resolve(false));
            buttonDouble.onClick.AddListener(() => Resolve(true));
        }

        public void Open(DoubleOrNothingOffer offer)
        {
            this.offer = offer;
            Open();
        }

        protected override void Window_OnOpen()
        {
            if (offer == null)
                return;

            textTitle.text = $"SET {offer.setIndex + 1} CLEARED";
            textCash.text = $"${offer.atRiskCash:N0}";
            textDetail.text =
                $"This set paid ${offer.setCash:N0}\n" +
                $"Cash out now, or go again for x{offer.nextSetMultiplier}.\n" +
                "Lose once and you lose it all.";
        }

        private void Resolve(bool continuePlaying)
        {
            var current = offer;
            offer = null;
            Close();
            current?.Resolve(continuePlaying);
        }
    }
}
