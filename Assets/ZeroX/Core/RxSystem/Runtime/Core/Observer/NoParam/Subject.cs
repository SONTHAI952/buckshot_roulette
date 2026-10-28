using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.RxSystem
{
    public class Subject : SubjectBase
    {
        //Temp
        private readonly List<ObserverBundle> listObserverBundle = new List<ObserverBundle>()
        {
            new ObserverBundle()
        };
        
        
        
        
        
        public Subject()
        { }
        
        public Subject(bool onlyEmitOnMainThread)
        {
            this.onlyEmitOnMainThread = onlyEmitOnMainThread;
        }

        
        
        private void InternalEmit()
        {
            lock (this)
            {
                int bundleIndex = listObserverBundle.Count - 1; //Luôn duyệt trên bundle cuối cùng
                var bundle = listObserverBundle[bundleIndex];
                bool isFirstEmitter = bundleIndex == 0 && bundle.isEmitting == false;
                
                bundle.isEmitting = true;
                int count = bundle.listObserver.Count;
                
                
                
                
                
                
                //Invoke
                for (int i = 0; i < count; i++)
                {
                    var observer = bundle.listObserver[i];
                    
                    
                    
                    //Kiểm tra xem có dispose khi owner bị destroy không
                    if (observer.disposeWhenOwnerDestroyed && observer.ownerObject == null)
                    {
                        UnSubscribe(observer);
                        continue;
                    }
            
            
                    //Kiểm tra xem có tạm dừng emit không
                    if (observer.pauseEmitWhenOwnerDisabled)
                    {
                        if (observer.ownerObject == null) //Dừng khi owner disable, nhưng owner bị destroy thì coi như disable vĩnh viễn và ko còn emit được nữa
                        {
                            UnSubscribe(observer);
                            continue;
                        }
                        else
                        {
                            if (observer.ownerObject is Behaviour behaviour)
                            {
                                if(behaviour.isActiveAndEnabled == false)
                                    continue;
                            }
                            else if (observer.ownerObject is GameObject gObj)
                            {
                                if(gObj.activeInHierarchy == false)
                                    continue;
                            }
                        }
                    }
            
            
                    //Nếu chỉ emit một lần thì xóa khỏi list trước khi invoke
                    if (observer.isOnce)
                    {
                        UnSubscribe(observer);
                    }
            
            
                    //Invoke
                    try
                    {
                        observer.action?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                
                
                
                
                
                
                
                
                //Sau khi xong, nếu không phải firstEmitter thì thôi
                if(isFirstEmitter == false)
                    return;

                
                //Nếu không có thay đổi về listObserver thì thôi
                int numberBundle = listObserverBundle.Count;
                if(numberBundle <= 1)
                    return;
                
                
                //Tiến hành cập nhật các thay đổi trong khi emit về root
                bundle.isEmitting = false;
                bundle.listObserver = listObserverBundle[numberBundle - 1].listObserver;
                listObserverBundle.RemoveRange(1, numberBundle - 1);
            }
        }

        public void Emit()
        {
            if (onlyEmitOnMainThread)
            {
                if (ZeroRx.IsMainThread)
                {
                    InternalEmit();
                }
                else
                {
                    ZeroRx.RunAction(InternalEmit);
                }
            }
            else
            {
                InternalEmit();
            }
        }
        
        public Disposable Subscribe(Observer observer)
        {
            lock (this)
            {
                var bundle = listObserverBundle[listObserverBundle.Count - 1];
                if (bundle.isEmitting)
                {
                    //Nếu đang emitting thì tạo ra 1 bundle mới (ko emitting) từ bundle cũ
                    bundle = ObserverBundle.Create(bundle);
                    listObserverBundle.Add(bundle);
                }
                
                
                bundle.listObserver.Add(observer);
                return new DisposableSingle(this, observer);
            }
        }
        
        
        public override void UnSubscribe(ObserverBase observerBase)
        {
            if (observerBase is Observer observer)
            {
                lock (this)
                {
                    var bundle = listObserverBundle[listObserverBundle.Count - 1];
                    if (bundle.isEmitting)
                    {
                        //Nếu đang emitting thì tạo ra 1 bundle mới (ko emitting) từ bundle cũ
                        bundle = ObserverBundle.Create(bundle);
                        listObserverBundle.Add(bundle);
                    }
                    
                    bundle.listObserver.Remove(observer);
                }
            }
            else
            {
                string observerBaseType = observerBase == null ? "null" : observerBase.GetType().Name;
                Debug.LogError("Cannot unsubscribe because observerBase is not Observer type. It is " + observerBaseType);
            }
        }
        
        public void UnSubscribeAll()
        {
            lock (this)
            {
                var bundle = listObserverBundle[listObserverBundle.Count - 1];
                if (bundle.isEmitting)
                {
                    //Nếu đang emitting thì tạo ra 1 bundle mới (ko emitting) từ bundle cũ
                    bundle = ObserverBundle.Create(bundle);
                    listObserverBundle.Add(bundle);
                }
                
                bundle.listObserver.Clear();
            }
        }
        
        
        
        #region Subscribe Bonus

        public Disposable Subscribe(Action action)
        {
            Observer observer = new Observer();
            observer.action = action;
            
            return Subscribe(observer);
        }

        public Disposable Subscribe(Action action, Object ownerObject, bool disposeWhenOwnerDestroyed, bool pauseEmitWhenOwnerDisabled = false, bool isOnce = false)
        {
            Observer observer = new Observer();
            observer.action = action;
            observer.ownerObject = ownerObject;
            observer.disposeWhenOwnerDestroyed = disposeWhenOwnerDestroyed;
            observer.pauseEmitWhenOwnerDisabled = pauseEmitWhenOwnerDisabled;
            observer.isOnce = isOnce;

            return Subscribe(observer);
        }

        public Disposable SubscribeUntilDestroy(Action action, Object ownerObject)
        {
            Observer observer = new Observer();
            observer.action = action;
            observer.ownerObject = ownerObject;
            observer.disposeWhenOwnerDestroyed = true;
            
            return Subscribe(observer);
        }

        public Disposable SubscribeOnce(Action action)
        {
            Observer observer = new Observer();
            observer.action = action;
            observer.isOnce = true;
            
            return Subscribe(observer);
        }
        
        public Disposable SubscribeOnceUntilDestroy(Action action, Object ownerObject)
        {
            Observer observer = new Observer();
            observer.action = action;
            observer.ownerObject = ownerObject;
            observer.disposeWhenOwnerDestroyed = true;
            observer.isOnce = true;
            
            return Subscribe(observer);
        }

        #endregion



        #region Wait Emit

        public WaitToken WaitEmit()
        {
            WaitToken waitToken = new WaitToken();
            
            
            
            Observer observer = new Observer();
            observer.action = waitToken.SetResult;
            observer.isOnce = true;
            Subscribe(observer);
            
            
            
            return waitToken;
        }
        
        public WaitToken WaitEmitUntilDestroy(Object ownerObject)
        {
            WaitToken waitToken = new WaitToken();
            
            
            
            Observer observer = new Observer();
            observer.action = waitToken.SetResult;
            observer.ownerObject = ownerObject;
            observer.disposeWhenOwnerDestroyed = true;
            observer.isOnce = true;
            Subscribe(observer);
            
            
            
            return waitToken;
        }

        #endregion
    }
}