namespace ZeroX.RxSystem
{
    [System.Serializable]
    public class ByteRx : VariableRx<byte>
    {
        public static implicit operator byte(ByteRx v) => v.Value;
    }
}