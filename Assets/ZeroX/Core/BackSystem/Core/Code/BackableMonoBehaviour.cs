using System;
using UnityEngine;

namespace ZeroX.BackSystem
{
    public abstract class BackableMonoBehaviour : MonoBehaviour, IBackable
    {
        private void Start()
        {
            
        }

        public void Back()
        {
            if(enabled == false)
                return;
            
            ExecuteBack();
        }

        protected abstract void ExecuteBack();
    }
}