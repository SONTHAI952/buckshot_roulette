using System.Collections.Generic;
using System.Linq;

namespace ZeroX.RxSystem
{
    public class ObserverBundle
    {
        public List<Observer> listObserver = new List<Observer>();
        public bool isEmitting = false;


        public static ObserverBundle Create(ObserverBundle oldBundle)
        {
            ObserverBundle bundle = new ObserverBundle();
            bundle.listObserver = oldBundle.listObserver.ToList();
            bundle.isEmitting = false;

            return bundle;
        }
    }
}