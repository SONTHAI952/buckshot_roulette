using System.Collections.Generic;
using System.Linq;

namespace ZeroX.RxSystem
{
    public class ObserverBundle<T>
    {
        public List<Observer<T>> listObserver = new List<Observer<T>>();
        public bool isEmitting = false;


        public static ObserverBundle<T> Create(ObserverBundle<T> oldBundle)
        {
            ObserverBundle<T> bundle = new ObserverBundle<T>();
            bundle.listObserver = oldBundle.listObserver.ToList();
            bundle.isEmitting = false;

            return bundle;
        }
    }
}