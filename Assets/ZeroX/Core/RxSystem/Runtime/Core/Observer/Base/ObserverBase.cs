using UnityEngine;

namespace ZeroX.RxSystem
{
    public abstract class ObserverBase
    {
        public Object ownerObject;
        public bool disposeWhenOwnerDestroyed;
        public bool pauseEmitWhenOwnerDisabled;
        
        
        
        public bool isOnce;
    }
}