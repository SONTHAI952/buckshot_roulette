using System;
using UnityEngine;
using ZeroX.WindowSystem;

namespace ZeroX.BackSystem
{
    public class BackableWindow : BackableMonoBehaviour
    {
        [SerializeField] private Window window;


        private void Reset()
        {
            window = GetComponentInParent<Window>();
        }

        protected override void ExecuteBack()
        {
            window.Close();
        }
    }
}