using System;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WindowAnimBridge : WindowAnimMono
    {
        public MonoBehaviour windowAnim;

        private bool CheckBehaviourExecuteAnim()
        {
            if (windowAnim == null)
            {
                Debug.LogError("behaviourExecuteAnim null", gameObject);
                return false;
            }
            
            if (windowAnim is IWindowAnim == false)
            {
                Debug.LogError("behaviourExecuteAnim not implement from IWindowAnim", gameObject);
                return false;
            }

            return true;
        }
        
        public override void Open(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            if (CheckBehaviourExecuteAnim() == false)
            {
                window.gameObject.SetActive(true);
                return;
            }

            IWindowAnim iWindowAnim = (IWindowAnim) windowAnim;
            iWindowAnim.Open(window, duration, ignoreTimeScale, parameters, onFinished);
        }

        public override void OpenImmediate(Window window, WindowParameters parameters)
        {
            if (CheckBehaviourExecuteAnim() == false)
            {
                window.gameObject.SetActive(true);
                return;
            }

            IWindowAnim iWindowAnim = (IWindowAnim) windowAnim;
            iWindowAnim.OpenImmediate(window, parameters);
        }

        public override void Close(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            if (CheckBehaviourExecuteAnim() == false)
            {
                window.gameObject.SetActive(false);
                return;
            }

            IWindowAnim iWindowAnim = (IWindowAnim) windowAnim;
            iWindowAnim.Close(window, duration, ignoreTimeScale, parameters, onFinished);
        }

        public override void CloseImmediate(Window window, WindowParameters parameters)
        {
            if (CheckBehaviourExecuteAnim() == false)
            {
                window.gameObject.SetActive(false);
                return;
            }

            IWindowAnim iWindowAnim = (IWindowAnim) windowAnim;
            iWindowAnim.CloseImmediate(window, parameters);
        }
    }
}