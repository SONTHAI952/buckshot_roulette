using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.RxSystem
{
    public class Disposables : Disposable
    {
        readonly List<Disposable> listDisposable = new List<Disposable>();
        
        
        public void Dispose()
        {
            lock (this)
            {
                foreach (var disposable in listDisposable)
                {
                    disposable.Dispose();
                }
            
                listDisposable.Clear();
            }
        }

        public void Add(Disposable disposable)
        {
            if (disposable == null)
            {
                Debug.LogError("Cannot add disposable object because it is null");
                return;
            }

            if (disposable == this)
            {
                Debug.LogError("Cannot add itself to itself");
                return;
            }
                
            
            lock (this)
            { 
                listDisposable.Add(disposable);
            }
        }
        
        public bool Remove(Disposable disposable)
        {
            lock (this)
            {
                return listDisposable.Remove(disposable);
            }
        }
        
        public bool Contains(Disposable disposable)
        {
            lock (this)
            {
                return listDisposable.Contains(disposable);
            }
        }
        
        public int Count
        {
            get
            {
                lock (this)
                {
                    return listDisposable.Count;
                }
            }
        }
    }
}