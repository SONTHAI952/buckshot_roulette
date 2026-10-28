using System;
using UnityEngine;

namespace ZeroX.RxSystem.Examples
{
    public class TestAction : MonoBehaviour
    {
        private Action onDo;

        private Action actDo3;
        private Action actDo4;

        private int reInvokeCount = 0;
        

        [ContextMenu("Emit Event")]
        public void EmitEvent()
        {
            onDo?.Invoke();
        }

        [ContextMenu("Register Event")]
        public void RegisterEvent()
        {
            onDo += () =>
            {
                Debug.Log("Do 1 - Remove do3");
                onDo -= actDo3;
            };
            
            onDo += () =>
            {
                Debug.Log("Do 2 - Remove do4");
                onDo -= actDo4;
            };

            
            actDo3 = () => Debug.Log("Do 3");
            actDo4 = () => Debug.Log("Do 4");
            onDo += actDo3;
            onDo += actDo4;


            onDo += () =>
            {
                Debug.Log("Do 5 - ReInvoke");
                reInvokeCount++;
                if(reInvokeCount > 1)
                    return;
                
                onDo.Invoke();
            };

            onDo += () => Debug.Log("Do 6");
        }
    }
}