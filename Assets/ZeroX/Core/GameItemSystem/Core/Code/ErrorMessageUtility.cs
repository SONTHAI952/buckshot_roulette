namespace GameItemSystem
{
    public static class ErrorMessageUtility
    {
        public static string AmountTypeIsUndefined()
        {
            return "Amount type is undefined";
        }

        public static string NotSupportAmountType(AmountType amountType)
        {
            return "Not support amountType: " + amountType;
        }
        
        public static string NotSupportRoundType(RoundType roundType)
        {
            return "Not support round type: ";
        }

        public static string PlusAmountOnlyWorkWithSingletonItem()
        {
            return "Plus amount only work with singleton item";
        }
        
        public static string MinusAmountOnlyWorkWithSingletonItem()
        {
            return "Minus amount only work with singleton item";
        }
        
        public static string SetAmountOnlyWorkWithSingletonItem()
        {
            return "Set amount only work with singleton item";
        }
        
        public static string CannotSetAmountSmallerZero()
        {
            return "Cannot set amount smaller zero";
        }
        
        public static string CannotPlusAmountSmallerZero()
        {
            return "Cannot plus amount smaller zero";
        }
        
        public static string CannotMinusAmountSmallerZero()
        {
            return "Cannot minus amount smaller zero";
        }
    }
}