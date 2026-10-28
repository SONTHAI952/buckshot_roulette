using GameItemSystem;
using UnityEngine;
using ZeroX.RxSystem;

namespace DataSystem.Money
{
    public class MoneyCoin : MoneyNormal
    {
        public MoneyCoin()
        {
        }

        public MoneyCoin(Subject<MoneyTransactionData> onTransaction) : base(onTransaction)
        {
        }


        public override string Id => GameItemIdDefine.coin;

        public override MoneyType MoneyType => MoneyType.Coin;
        
        public void NotifyTransactionData(MoneyTransactionData transactionInfo, bool notifyWhenSuccess = false)
        {
#if ZEROX_LOCALIZATION_SYSTEM
            string thanhToanThanhCong = ZeroX.LocalizationSystem.LManager.GetText("payment success");
            string banKhongDuCoin = ZeroX.LocalizationSystem.LManager.GetText("not enough coin");
            string daCoLoiXayRaThuLaiSau = ZeroX.LocalizationSystem.LManager.GetText("an error occurred, please try again later");
            string giaoDichBiHuy = ZeroX.LocalizationSystem.LManager.GetText("transaction cancelled");
#else
            string thanhToanThanhCong = "Payment success";
            string banKhongDuCoin = "You don't have enough coins";
            string daCoLoiXayRaThuLaiSau = "An error occurred, please try again later";
            string giaoDichBiHuy = "The transaction is canceled";
#endif
            
            
            
            
#if ZEROX_TOAST_SYSTEM
            if (transactionInfo.result == MoneyTransactionResult.NotEnough)
                ZeroX.ToastSystem.ToastMultiple.Show(banKhongDuCoin);
            else if (transactionInfo.result == MoneyTransactionResult.Error)
                ZeroX.ToastSystem.ToastMultiple.Show(daCoLoiXayRaThuLaiSau);
            else if (transactionInfo.result == MoneyTransactionResult.Cancel)
                ZeroX.ToastSystem.ToastMultiple.Show(giaoDichBiHuy);
            else if(transactionInfo.result == MoneyTransactionResult.Success)
            {
                if(notifyWhenSuccess)
                    ZeroX.ToastSystem.ToastMultiple.Show(thanhToanThanhCong);
            }
            else
            {
                Debug.LogError("Chưa code loại MoneyChangeResult này: " + transactionInfo.result);
            }
#else
            Debug.LogError("Chức năng Notify không hoạt động vì không có ToastSystem -> hãy import");
#endif
        }

    }
}