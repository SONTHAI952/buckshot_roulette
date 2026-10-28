using UnityEngine;

namespace ZeroX.PoolSystem
{
    public abstract class PoolComponent : IPoolMaintainable
    {
        public abstract bool IsModelNull();

        public abstract void CheckAndRemoveAllNullObject();

        public abstract void Dispose();

        public abstract void PutIn(Component component, bool deActive);
        public abstract void PutIn(Component component);
        public abstract void PutIn(Component component, float delay, bool ignoreTimeScale = false);
    }
}