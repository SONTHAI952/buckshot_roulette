using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.RxSystem
{
    public class Subject<T> : SubjectBase
    {
        //Temp
        private readonly List<ObserverBundle<T>> listObserverBundle = new List<ObserverBundle<T>>()
        {
            new ObserverBundle<T>()
        };
        
        
        
        
        
        public Subject()
        { }
        
        public Subject(bool onlyEmitOnMainThread)
        {
            this.onlyEmitOnMainThread = onlyEmitOnMainThread;
        }

        
        
        private void InternalEmit(T value)
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
                        observer.action?.Invoke(value);
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

        public void Emit(T value)
        {
            if (onlyEmitOnMainThread)
            {
                if (ZeroRx.IsMainThread)
                {
                    InternalEmit(value);
                }
                else
                {
                    ZeroRx.RunAction(() => InternalEmit(value));
                }
            }
            else
            {
                InternalEmit(value);
            }
        }
        
        public Disposable Subscribe(Observer<T> observer)
        {
            lock (this)
            {
                var bundle = listObserverBundle[listObserverBundle.Count - 1];
                if (bundle.isEmitting)
                {
                    //Nếu đang emitting thì tạo ra 1 bundle mới (ko emitting) từ bundle cũ
                    bundle = ObserverBundle<T>.Create(bundle);
                    listObserverBundle.Add(bundle);
                }
                
                
                bundle.listObserver.Add(observer);
                return new DisposableSingle(this, observer);
            }
        }
        
        
        public override void UnSubscribe(ObserverBase observerBase)
        {
            if (observerBase is Observer<T> observer)
            {
                lock (this)
                {
                    var bundle = listObserverBundle[listObserverBundle.Count - 1];
                    if (bundle.isEmitting)
                    {
                        //Nếu đang emitting thì tạo ra 1 bundle mới (ko emitting) từ bundle cũ
                        bundle = ObserverBundle<T>.Create(bundle);
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
                    bundle = ObserverBundle<T>.Create(bundle);
                    listObserverBundle.Add(bundle);
                }
                
                bundle.listObserver.Clear();
            }
        }
        
        
        
        #region Subscribe Bonus

        public Disposable Subscribe(Action<T> action)
        {
            Observer<T> observer = new Observer<T>();
            observer.action = action;
            
            return Subscribe(observer);
        }

        public Disposable Subscribe(Action<T> action, Object ownerObject, bool disposeWhenOwnerDestroyed, bool pauseEmitWhenOwnerDisabled = false, bool isOnce = false)
        {
            Observer<T> observer = new Observer<T>();
            observer.action = action;
            observer.ownerObject = ownerObject;
            observer.disposeWhenOwnerDestroyed = disposeWhenOwnerDestroyed;
            observer.pauseEmitWhenOwnerDisabled = pauseEmitWhenOwnerDisabled;
            observer.isOnce = isOnce;

            return Subscribe(observer);
        }

        public Disposable SubscribeUntilDestroy(Action<T> action, Object ownerObject)
        {
            Observer<T> observer = new Observer<T>();
            observer.action = action;
            observer.ownerObject = ownerObject;
            observer.disposeWhenOwnerDestroyed = true;
            
            return Subscribe(observer);
        }

        public Disposable SubscribeOnce(Action<T> action)
        {
            Observer<T> observer = new Observer<T>();
            observer.action = action;
            observer.isOnce = true;
            
            return Subscribe(observer);
        }
        
        public Disposable SubscribeOnceUntilDestroy(Action<T> action, Object ownerObject)
        {
            Observer<T> observer = new Observer<T>();
            observer.action = action;
            observer.ownerObject = ownerObject;
            observer.disposeWhenOwnerDestroyed = true;
            observer.isOnce = true;
            
            return Subscribe(observer);
        }

        #endregion



        #region Wait Emit

        public WaitToken<T> WaitEmit()
        {
            WaitToken<T> waitToken = new WaitToken<T>();
            
            
            
            Observer<T> observer = new Observer<T>();
            observer.action = waitToken.SetResult;
            observer.isOnce = true;
            Subscribe(observer);
            
            
            
            return waitToken;
        }
        
        public WaitToken<T> WaitEmitUntilDestroy(Object ownerObject)
        {
            WaitToken<T> waitToken = new WaitToken<T>();
            
            
            
            Observer<T> observer = new Observer<T>();
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