using System;
using UnityEngine;

namespace ZeroX.ProgressSystem
{
    public abstract class ProgressTarget : MonoBehaviour
    {
        public abstract void UpdateTarget(Progressor progressor);

        protected virtual void Reset()
        {
        }
    }
}