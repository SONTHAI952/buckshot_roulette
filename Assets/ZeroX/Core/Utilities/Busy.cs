using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX
{
    public class Busy<TBusyType> where TBusyType : struct, Enum
    {
        private bool allowDuplicated;
        private HashSet<TBusyType> hashSetAllowDuplicated; //Nếu sử dụng cái này thì chỉ những type có trong danh sách mới được allow duplicated
        
        private int busyCount;
        public TBusyType BusyType { get; private set; }
        
        public bool IsBusy => busyCount > 0;

        
        
        public Busy()
        {
        }
        
        public Busy(bool allowDuplicated)
        {
            this.allowDuplicated = allowDuplicated;
        }
        
        public Busy(params TBusyType[] listTypeAllowDuplicated)
        {
            this.allowDuplicated = true;
            
            hashSetAllowDuplicated = new HashSet<TBusyType>();
            for (int i = 0; i < listTypeAllowDuplicated.Length; i++)
            {
                hashSetAllowDuplicated.Add(listTypeAllowDuplicated[i]);
            }
        }

        public void SetBusy(TBusyType busyType)
        {
            if (busyCount == 0)
            {
                busyCount++;
                BusyType = busyType;
                return;
            }
            
            
            if (allowDuplicated) //Nếu chấp nhận duplicate thì chỉ cho phép cùng loại busy
            {
                if (hashSetAllowDuplicated == null || hashSetAllowDuplicated.Contains(busyType))
                {
                    if (busyType.Equals(BusyType))
                    {
                        busyCount++;
                        return;
                    }
                }
            }
            
            Debug.LogErrorFormat("Không thể set busy từ '{0}' -> '{1}'", BusyType, busyType);
        }

        public void UnBusy(TBusyType busyType)
        {
            if (busyCount == 0)
            {
                Debug.LogError("Đang không busy tại sao lại UnBusy");
                return;
            }

            if (busyType.Equals(BusyType) == false)
            {
                Debug.LogErrorFormat("UnBusy sai loại, loại hiện tại là '{0}' - loại muốn là '{1}'", BusyType, busyType);
                return;
            }

            busyCount--;
        }

        /// <summary>
        /// Có bận nhưng ko phải type truyền vào
        /// </summary>
        public bool IsBusyExclude(TBusyType busyType)
        {
            return IsBusy && BusyType.Equals(busyType) == false;
        }

        public bool IsBusyAt(TBusyType busyType)
        {
            return IsBusy && BusyType.Equals(busyType);
        }
        
        /// <summary>
        /// Có bận và type bận là 1 trong các type truyền vào
        /// </summary>
        public bool IsBusyAt(params TBusyType[] busyTypes)
        {
            if (IsBusy == false)
                return false;
            
            for (int i = 0; i < busyTypes.Length; i++)
            {
                if (busyTypes[i].Equals(BusyType) == false)
                    return false;
            }

            return true;
        }
    }
}