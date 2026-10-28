using GameItemSystem;
using UnityEngine;
using ZeroX.RxSystem;

namespace DataSystem.Money
{
    public class MoneyGem : MoneyNormal
    {
        public MoneyGem()
        {
        }

        public MoneyGem(Subject<MoneyTransactionData> onTransaction) : base(onTransaction)
        {
        }


        public override string Id => GameItemIdDefine.gem;
        
        public override MoneyType MoneyType => MoneyType.Gem;
        
        public void NotifyTransactionData(MoneyTransactionData transactionInfo, bool notifyWhenSuccess = false)
        {
#if ZEROX_LOCALIZATION_SYSTEM
            string thanhToanThanhCong = ZeroX.LocalizationSystem.LManager.GetText("payment success");
            string banKhongDuGem = ZeroX.LocalizationSystem.LManager.GetText("not enough gem");
            string daCoLoiXayRaThuLaiSau = ZeroX.LocalizationSystem.LManager.GetText("an error occurred, please try again later");
            string giaoDichBiHuy = ZeroX.LocalizationSystem.LManager.GetText("transaction cancelled");
#else
            string thanhToanThanhCong = "Payment success";
            string banKhongDuGem = "You don't have enough gems";
            string daCoLoiXayRaThuLaiSau = "An error occurred, please try again later";
            string giaoDichBiHuy = "The transaction is canceled";
#endif
            
            
            
#if ZEROX_TOAST_SYSTEM
            if (transactionInfo.result == MoneyTransactionResult.NotEnough)
                ZeroX.ToastSystem.ToastMultiple.Show(banKhongDuGem);
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