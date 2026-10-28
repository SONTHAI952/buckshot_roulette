using UnityEngine;
using UnityEngine.Events;

namespace ZeroX.BackSystem
{
    public class BackableTriggerUnityEvent : BackableMonoBehaviour
    {
        [SerializeField] private UnityEvent onBack;
        

        
        protected override void ExecuteBack()
        {
            onBack.Invoke();
        }
    }
}