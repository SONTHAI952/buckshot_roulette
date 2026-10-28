using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ZeroX.RxSystem.Examples
{
    public class TestObserver_T : MonoBehaviour
    {
        [SerializeField] private Object eventOwnerObject;
        
        private Subject<string> onDo = new Subject<string>();


        private Disposable disposable_DoSpecial_RemoveEvent;
        

        [ContextMenu("Emit Event")]
        public void EmitEvent()
        {
            onDo.Emit("Xin chào");
            
            Debug.Log("Emit Event OK");
        }
        
        
        [ContextMenu("Emit Event In Other Thread")]
        public void EmitEvent_InOtherThread()
        {
            Task task = new Task(() =>
            {
                Debug.Log("Is Main Thread: " + ZeroRx.IsMainThread);
                
                Thread.Sleep(500);
                EmitEvent();
            });
            
            task.Start();
        }
        
        

        [ContextMenu("Register Event")]
        public void RegisterEvent()
        {
            onDo.Subscribe(v => Debug.Log("Is Main Thread: " + ZeroRx.IsMainThread));
            onDo.Subscribe(v => Debug.Log("Do 1: " + v));
            onDo.Subscribe(v => Debug.Log("Do 2: " + v));
            disposable_DoSpecial_RemoveEvent = onDo.Subscribe(DoSpecial_RemoveEvent);
            onDo.Subscribe(v => Debug.Log("Do 3: " + v));
            onDo.Subscribe(v => Debug.Log("Do 4: " + v));
            
            onDo.SubscribeUntilDestroy(v => Debug.Log("Do 5 - Until Destroy: " + v), eventOwnerObject);
            
            Observer<string> observer = new Observer<string>();
            observer.action = v => Debug.Log("Do 6 - Pause Emit: " + v);
            observer.ownerObject = eventOwnerObject;
            observer.pauseEmitWhenOwnerDisabled = true;
            onDo.Subscribe(observer);
            
            onDo.Subscribe(v => Debug.Log("Do 7 Advanced - Until Destroy - Pause Emit: " + v) , eventOwnerObject, true, true);
            
            Debug.Log("Register Event OK");
        }

        [ContextMenu("Register Event In Other Thread")]
        public void RegisterEvent_InOtherThread()
        {
            Task task = new Task(() =>
            {
                Debug.Log("Is Main Thread: " + ZeroRx.IsMainThread);
                
                Thread.Sleep(500);
                RegisterEvent();
            });
            
            task.Start();
        }



        private void DoSpecial_AddEvent(string value)
        {
            Debug.Log("DoSpecial_AddEvent Start: " + value);
            onDo.Subscribe(v => Debug.Log("Do Special: " + v));
            Debug.Log("DoSpecial_AddEvent End: " + value);
        }

        private void DoSpecial_RemoveEvent(string value)
        {
            Debug.Log("DoSpecial_RemoveEvent Start: " + value);
            disposable_DoSpecial_RemoveEvent.Dispose();
            Debug.Log("DoSpecial_RemoveEvent End: " + value);
        }
        
        [ContextMenu("Wait Emit Event")]
        private void WaitEmitEvent()
        {
            StartCoroutine(Timeline());
            
            
            
            IEnumerator Timeline()
            {
                Debug.Log("Start Wait Emit Event");

                var waitToken = onDo.WaitEmit();
                yield return waitToken;
                
                Debug.Log("End Wait Emit Event: " + waitToken.Result);
            }
        }
    }
}