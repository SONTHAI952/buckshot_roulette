using GameItemSystem;

namespace DataSystem.Money
{
    public class MoneyTransactionData
    {
        public MoneyType moneyType = MoneyType.Null;
        public MoneyTransactionType transactionType = MoneyTransactionType.Null;
        public MoneyTransactionResult result = MoneyTransactionResult.Null;
        public decimal amount = 0;

        
        //Các parameter
        public InventoryParameters parameters;

        
        public string Where => parameters?.where;
        public string Reason => parameters?.reason;
    }
}