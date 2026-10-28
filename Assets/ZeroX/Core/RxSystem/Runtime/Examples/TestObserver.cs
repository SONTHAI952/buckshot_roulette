using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.RxSystem.Examples
{
    public class TestObserver : MonoBehaviour
    {
        [SerializeField] private Object eventOwnerObject;
        
        private Subject onDo = new Subject();


        private Disposable disposable_DoSpecial_RemoveEvent;

        

        [ContextMenu("Emit Event")]
        public void EmitEvent()
        {
            onDo.Emit();
            
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
            onDo.Subscribe(() => Debug.Log("Is Main Thread: " + ZeroRx.IsMainThread));
            onDo.Subscribe(() => Debug.Log("Do 1"));
            onDo.Subscribe(() => Debug.Log("Do 2"));
            disposable_DoSpecial_RemoveEvent = onDo.Subscribe(DoSpecial_RemoveEvent);
            onDo.Subscribe(() => Debug.Log("Do 3"));
            onDo.Subscribe(() => Debug.Log("Do 4"));
            
            onDo.SubscribeUntilDestroy(() => Debug.Log("Do 5 - Until Destroy"), eventOwnerObject);
            
            Observer observer = new Observer();
            observer.action = () => Debug.Log("Do 6 - Pause Emit");
            observer.ownerObject = eventOwnerObject;
            observer.pauseEmitWhenOwnerDisabled = true;
            onDo.Subscribe(observer);
            
            onDo.Subscribe(() => Debug.Log("Do 7 Advanced - Until Destroy - Pause Emit") , eventOwnerObject, true, true);
            
            onDo.Subscribe(() => Debug.Log("Do 8"));
            onDo.Subscribe(() => Debug.Log("Do 9"));
            
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
        
        private int reInvokeCount = 0;
        
        [ContextMenu("Register Event - ReInvoke")]
        public void RegisterEvent_ReInvoke()
        {
            Disposable disposable3 = null;
            Disposable disposable4 = null;
            
            onDo.Subscribe(() =>
            {
                Debug.Log("Do 1");
                
                if(reInvokeCount == 0)
                    disposable3.Dispose();
            });
            
            onDo.Subscribe(() =>
            {
                Debug.Log("Do 2");
                
                if(reInvokeCount == 1)
                    disposable4.Dispose();
            });
            
            
            disposable3 = onDo.Subscribe(() => Debug.Log("Do 3"));
            disposable4 = onDo.Subscribe(() => Debug.Log("Do 4"));

            onDo.Subscribe(() =>
            {
                reInvokeCount++;
                if (reInvokeCount > 2)
                    return;
                
                Debug.Log("Do 5 - ReInvoke: " + reInvokeCount);
                onDo.Emit();
            });
            
            onDo.Subscribe(() => Debug.Log("Do 6")); 
            onDo.Subscribe(() => Debug.Log("Do 7"));
            
            Debug.Log("Register Event OK");
        }

        private void DoSpecial_AddEvent()
        {
            Debug.Log("DoSpecial_AddEvent Start");
            onDo.Subscribe(() => Debug.Log("Do Special"));
            Debug.Log("DoSpecial_AddEvent End");
        }
        
        private void DoSpecial_RemoveEvent()
        {
            Debug.Log("DoSpecial_RemoveEvent Start");
            disposable_DoSpecial_RemoveEvent.Dispose();
            Debug.Log("DoSpecial_RemoveEvent End");
        }

        [ContextMenu("Wait Emit Event")]
        private void WaitEmitEvent()
        {
            StartCoroutine(Timeline());
            
            
            
            IEnumerator Timeline()
            {
                Debug.Log("Start Wait Emit Event");

                yield return onDo.WaitEmit();
                
                Debug.Log("End Wait Emit Event");
            }
        }


        
        [ContextMenu("Test Dead Lock")]
        private void TestDeadLock()
        {
            int startTask2 = 0;
            Subject subject = new Subject();
            subject.onlyEmitOnMainThread = false;

            
            
            Task task1 = new Task(() =>
            {
                ZeroRx.RunAction(() => Debug.Log("Task 1 Start"));
                Thread.Sleep(1000);
                subject.Emit();
            });
            task1.ContinueWith(t =>
            {
                ZeroRx.RunAction(() => Debug.Log("Task 1 completed"));
            });
            

            Task task2 = new Task(() =>
            {
                ZeroRx.RunAction(() => Debug.Log("Task 2 Start"));
                subject.Emit();
            });
            task2.ContinueWith(t =>
            {
                ZeroRx.RunAction(() => Debug.Log("Task 2 completed"));
            });
            
            
            
            
            subject.Subscribe(() =>
            {
                
                startTask2++;
                if (startTask2 == 1)
                {
                    ZeroRx.RunAction(() => Debug.Log("Start Task 2 By Emit - Start"));
                    task2.Start();
                    ZeroRx.RunAction(() => Debug.Log("Start Task 2 By Emit - End"));
                }
            });

            subject.Subscribe(() =>
            {
                Thread.Sleep(3000);
            });
            
            
            Debug.Log("Start Test");
            task1.Start();
        }
    }
}