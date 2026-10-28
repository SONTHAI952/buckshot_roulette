using System;
using System.Collections.Generic;
using GameItemSystem;
using UnityEngine;
using ZeroX.RxSystem;

namespace DataSystem.Money
{
    public static class MoneyDI
    {
        public static readonly Subject<MoneyTransactionData> onTransaction = new Subject<MoneyTransactionData>();

        public static MoneyCoin Coin = new MoneyCoin(onTransaction);
        public static MoneyGem Gem = new MoneyGem(onTransaction);
        
        public static bool IsMoneyId(string id)
        {
            return GameItemTable.GetType(id) == GameItemType.money;
        }

        #region Plus Money Generic

        public static MoneyTransactionData PlusMoney(string gameItemID, decimal amount, string where, string reason)
        {
            if (gameItemID == Coin.Id)
                return Coin.Plus(amount, where, reason);
            else if (gameItemID == Gem.Id)
                return Gem.Plus(amount, where, reason);
            else
                throw new Exception("Not support this money: " + gameItemID);
        }
        
        public static MoneyTransactionData MinusMoney(string gameItemID, decimal amount, string where, string reason)
        {
            if (gameItemID == Coin.Id)
                return Coin.Minus(amount, where, reason);
            else if (gameItemID == Gem.Id)
                return Gem.Minus(amount, where, reason);
            else
                throw new Exception("Not support this money: " + gameItemID);
        }

        #endregion

        #region Notify

        public static void NotifyMoneyTransactionData(MoneyTransactionData transactionInfo, bool notifyWhenSuccess = false)
        {
            switch (transactionInfo.moneyType)
            {
                case MoneyType.Coin:
                {
                    Coin.NotifyTransactionData(transactionInfo, notifyWhenSuccess);
                    return;
                }
                case MoneyType.Gem:
                {
                    Gem.NotifyTransactionData(transactionInfo, notifyWhenSuccess);
                    return;
                }
                default:
                {
                    Debug.LogError("Chưa code loại MoneyType này: " + transactionInfo.moneyType);
                    return;
                }
            }
        }

        #endregion

        public static decimal GetAmount(MoneyType moneyType)
        {
            if (moneyType == MoneyType.Coin)
                return Coin.Amount;
            
            if (moneyType == MoneyType.Gem)
                return Gem.Amount;
            
            Debug.LogError("Not code for money type: " + moneyType);
            return 0;
        }
    }
}