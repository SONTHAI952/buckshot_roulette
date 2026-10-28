namespace ZeroX.SaveGameSystem
{
    public enum NodeType
    {
        Undefined = 0,
        Object = 1,
        
        //String
        String = 10,
        
        //Number
        Int = 20,
        Float = 21,
        Long = 22,
        Decimal = 23,
        Double = 24,
        
        //Bonus
        Bool = 50,
        
        //Big
        BigInteger = 80,
    }
}