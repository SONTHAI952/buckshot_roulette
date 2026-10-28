using System;
using UnityEngine;

namespace ZeroX.UI
{
    public abstract class GrayEffect : MonoBehaviour
    {
        [SerializeField] protected bool m_isGray = false;
        [SerializeField] protected bool m_updateOnStart = false;

        protected virtual void Start()
        {
            if (m_updateOnStart)
                SetGray(m_isGray);
        }

        public bool IsGray
        {
            get => m_isGray;
            set => SetGray(value);
        }

        public bool UpdateOnStart
        {
            get => m_updateOnStart;
            set => m_updateOnStart = value;
        }

        public void SetGray(bool isGray)
        {
            m_isGray = isGray;
            ExecuteSetGray(isGray);
        }

        protected abstract void ExecuteSetGray(bool isGray);
    }
}