namespace ZeroX.RxSystem
{
    [System.Serializable]
    public class LongRx : VariableRx<long>
    {
        public static implicit operator long(LongRx v) => v.Value;
    }
}