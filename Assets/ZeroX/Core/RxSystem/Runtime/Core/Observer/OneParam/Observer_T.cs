using System;
using UnityEngine;

namespace ZeroX.RxSystem
{
    public class Observer<T> : ObserverBase
    {
        public Action<T> action;
    }
}