using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroX.Extensions;


namespace TimeOnlineSystem
{
    public class TimeOnlineManager : MonoBehaviour
    {
        [SerializeField] private float savePerSeconds = 10f;
        [SerializeField] int secondsUnSaved = 0;

        [SerializeField] private List<IActionPerformedWhenResetDay> listActionPerformedWhenResetDay;

        private static TimeOnlineManager instance;
        public static TimeOnlineManager Instance => instance;

        
        private DateTime dayOfRecord;
        
        
        private void Awake()
        {
            if (instance == null || instance == this)
            {
                instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.Log("Chỉ nên có một TimeOnlineManager ---> xóa");
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        private void Start()
        {
            dayOfRecord = TimeOnlineDI.DayOfRecord;
            if (DateTimeExtension.IsToday(dayOfRecord) == false)
            {
                TimeOnlineDI.ResetTimeOnline();
                dayOfRecord = DateTime.Now.Date;
                ExecuteResetDay();
                PostOnResetDay();
            }
                

            StartCoroutine(IEUpdate());
        }

        /// <summary>
        /// Dành cho những tác vụ khác cần reset day
        /// </summary>
        void ExecuteResetDay()
        {
            Debug.Log("On Reset New Day");
            foreach (var action in listActionPerformedWhenResetDay)
            {
                try
                {
                    action.Execute();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        
        IEnumerator IEUpdate()
        {
            var waitForSecondsRealtime = new WaitForSecondsRealtime(1);
            while (true)
            {
                yield return waitForSecondsRealtime;
                UpdatePerSecond();
            }
        }

        void UpdatePerSecond()
        {
            secondsUnSaved++;
            
            if (DateTimeExtension.IsToday(dayOfRecord) == false)
            {
                TimeOnlineDI.ResetTimeOnline();
                dayOfRecord = DateTime.Now.Date;
                secondsUnSaved = 0;
                
                ExecuteResetDay();
                PostOnResetDay();
                Debug.Log("Reset Time Online");
            }

            if (secondsUnSaved >= savePerSeconds)
            {
                TimeOnlineDI.PlusTimeOnline(secondsUnSaved);
                secondsUnSaved = 0;
                
                #if UNITY_EDITOR
                Debug.Log("Save Time Online");
                #endif
            }
        }
        
        
        public static int GetTotalSecondsOnline()
        {
            int secondsOnline = TimeOnlineDI.TotalSecondsOnline;
            if(instance != null)
                secondsOnline += instance.secondsUnSaved;

            return secondsOnline;
        }
        
        public static int GetTotalMinutesOnline()
        {
            int secondsOnline = GetTotalSecondsOnline();
            return secondsOnline / 60;
        }

        public static int GetSecondsOnlineInDay()
        {
            int secondsOnline = TimeOnlineDI.SecondsOnlineInDay;
            if(instance != null)
                secondsOnline += instance.secondsUnSaved;

            return secondsOnline;
        }
        
        public static int GetMinutesOnlineInDay()
        {
            int secondsOnline = GetSecondsOnlineInDay();
            return secondsOnline / 60;
        }
        
        

        #region Event

        public static void PostOnResetDay()
        {
            TimeOnlineDI.onResetDay.Emit(new ResetDayData());
        }

        #endregion
    }
}