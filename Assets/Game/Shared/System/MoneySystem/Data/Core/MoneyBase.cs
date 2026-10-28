using ZeroX.RxSystem;

namespace DataSystem.Money
{
    public abstract class MoneyBase
    {
        protected readonly Subject<MoneyTransactionData> onTransaction = new Subject<MoneyTransactionData>();

        public MoneyBase()
        {
        }

        public MoneyBase(Subject<MoneyTransactionData> onTransaction)
        {
            this.onTransaction = onTransaction;
        }

        public abstract string Id { get;}
        
        public abstract decimal Amount { get; }
        
        protected abstract void SetAmount(decimal amount, string where, string reason);

        public abstract void SetDefaultAmount(decimal amount);

        public abstract MoneyTransactionData Plus(decimal amount, string where, string reason);

        public abstract MoneyTransactionData Minus(decimal amount, string where, string reason);
        
        public bool IsEnoughAmount(decimal amount)
        {
            return Amount >= amount;
        }
    }
}