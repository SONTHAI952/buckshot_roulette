using UnityEngine;

namespace ZeroX.ToggleSystem
{
    public abstract class ZToggleTarget : MonoBehaviour
    {
        public abstract void UpdateTarget(ZToggle toggle);
        
        public abstract void UpdateTargetImmediate(ZToggle toggle);
        
        protected virtual void Reset()
        {
        }
    }
}