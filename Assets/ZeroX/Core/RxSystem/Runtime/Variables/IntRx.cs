namespace ZeroX.RxSystem
{
    [System.Serializable]
    public class IntRx : VariableRx<int>
    {
        public static implicit operator int(IntRx v) => v.Value;
    }
}