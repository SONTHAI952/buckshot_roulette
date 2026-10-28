using GameItemSystem;
using ZeroX.RxSystem;

namespace DataSystem.Money
{
    public abstract class MoneyNormal : MoneyBase
    {
        protected MoneyNormal()
        {
        }

        protected MoneyNormal(Subject<MoneyTransactionData> onTransaction) : base(onTransaction)
        {
        }
        

        public abstract MoneyType MoneyType { get; }
        
        public override decimal Amount => InventoryDI.GetAmountDecimal(Id);

        protected override void SetAmount(decimal amount, string where, string reason)
        {
            InventoryDI.SetAmount(Id, amount, where, reason);
        }

        public override void SetDefaultAmount(decimal amount)
        {
            if(InventoryDI.ContainsId(Id))
                return;
            
            InventoryDI.SetAmount(Id, amount, "system", "default");
        }

        public override MoneyTransactionData Plus(decimal amount, string where, string reason)
        {
            MoneyTransactionData transactionData = new MoneyTransactionData();
            transactionData.moneyType = MoneyType;
            transactionData.transactionType = MoneyTransactionType.Increase;
            transactionData.amount = amount;
            transactionData.parameters = new InventoryParameters(where, reason);

            if (amount == 0)
            {
                transactionData.result = MoneyTransactionResult.Success;
                return transactionData;
            }

            SetAmount(Amount + amount, where, reason);

            transactionData.result = MoneyTransactionResult.Success;
            
            onTransaction.Emit(transactionData);
            return transactionData;
        }

        public override MoneyTransactionData Minus(decimal amount, string where, string reason)
        {
            MoneyTransactionData transactionData = new MoneyTransactionData();
            transactionData.moneyType = MoneyType;
            transactionData.transactionType = MoneyTransactionType.Decrease;
            transactionData.amount = amount;
            transactionData.parameters = new InventoryParameters(where, reason);
            
            if (amount == 0)
            {
                transactionData.result = MoneyTransactionResult.Success;
                return transactionData;
            }
            
            if (amount < 0)
            {
                transactionData.result = MoneyTransactionResult.Error;
                return transactionData;
            }

            if (Amount < amount)
            {
                transactionData.result = MoneyTransactionResult.NotEnough;
                return transactionData;
            }

            SetAmount(Amount - amount, where, reason);
            
            transactionData.result = MoneyTransactionResult.Success;
            
            onTransaction.Emit(transactionData);
            return transactionData;
        }
    }
}