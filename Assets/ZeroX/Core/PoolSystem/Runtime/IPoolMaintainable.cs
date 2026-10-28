namespace ZeroX.PoolSystem
{
    public interface IPoolMaintainable
    {
        bool IsModelNull();
        void CheckAndRemoveAllNullObject();
        void Dispose();
    }
}