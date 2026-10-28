using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace ZeroX.UI
{
    public class TimerDisplayer : MonoBehaviour
    {
        public enum UnitId
        {
            MilliSecond, Second, Minute, Hour, Day
        }
        
        public TMP_Text text;
        
        public bool useRealTime = true;
        public UnitId minUnit = UnitId.Second;
        public UnitId maxUnit = UnitId.Day;
        
        [Min(1)]
        public int minNumberUnitDisplaySameTime = 2;
        public int maxNumberUnitDisplaySameTime = 3;
        

        [SerializeField] private string dayFormat = "{0:00}d";
        [SerializeField] private string hourFormat = "{0:00}h";
        [SerializeField] private string minutesFormat = "{0:00}m";
        [SerializeField] private string secondFormat = "{0:00}s";
        [SerializeField] private string millisecondFormat = "{0:000}ms";
        [SerializeField] private string separator = ":";
        
        
        
        //Const
        private static readonly int[] listTimeUnitMaxValue = new[]
        {
            1000, //MilliSecond
            60, //Second
            60, //Minutes
            24, //Hour
            int.MaxValue
        };
        
        
        //Event
        public Action onFinished;
        
        
        //Properties
        public bool IsRunning => crLooper != null;

        //Temp
        private TimeSpan remainingTimeSpan;
        private Coroutine crLooper;
        private int[] listTimeUnitValue = new int[5];
        

        
        #region Unity Method

        private void OnDisable()
        {
            crLooper = null;
        }

        #endregion


        
        
        #region Set Time

        public void SetTime(TimeSpan timeSpan)
        {
            remainingTimeSpan = timeSpan;
        }

        public void SetSeconds(double value)
        {
            remainingTimeSpan = TimeSpan.FromSeconds(value);
        }
        
        public void SetMinutes(double value)
        {
            remainingTimeSpan = TimeSpan.FromMinutes(value);
        }
       
        #endregion
        
        

        public void Run()
        {
            if (crLooper != null)
            {
                StopCoroutine(crLooper);
                crLooper = null;
            }
            
            if (useRealTime)
            {
                crLooper = StartCoroutine(LooperRealtime());
            }
            else
            {
                crLooper = StartCoroutine(LooperTime());
            }
        }

        public void StopRun()
        {
            if (crLooper != null)
            {
                StopCoroutine(crLooper);
                crLooper = null;
            }
        }


        IEnumerator LooperRealtime()
        {
            WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(1);
            
            while (remainingTimeSpan.TotalMilliseconds > 0)
            {
                UpdateText();
                yield return waitForSecondsRealtime;
                remainingTimeSpan = remainingTimeSpan.Subtract(TimeSpan.FromSeconds(1));
            }
            
            
            remainingTimeSpan = TimeSpan.Zero;
            UpdateText();
            onFinished?.Invoke();
        }
        
        IEnumerator LooperTime()
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(1);
            
            while (remainingTimeSpan.TotalMilliseconds > 0)
            {
                UpdateText();
                yield return waitForSeconds;
                remainingTimeSpan = remainingTimeSpan.Subtract(TimeSpan.FromSeconds(1));
            }
            
            
            remainingTimeSpan = TimeSpan.Zero;
            UpdateText();
            onFinished?.Invoke();
        }

        private void UpdateText()
        {
            UpdateListTimeUnitValue();
            CalculateRangeUnitIndexToDisplay(remainingTimeSpan, out int startUnitIndex, out int endUnitIndex);
            RoundUpFromUnitValue(endUnitIndex);
            
            StringBuilder sb = new StringBuilder();
            
            for (int i = startUnitIndex; i >= endUnitIndex; i--)
            {
                if (i == startUnitIndex)
                {
                    int totalValue = CalculateUnitTotalValue(i);
                    sb.Append(FormatUnit(i, totalValue));
                }
                else
                {
                    int value = listTimeUnitValue[i];
                    sb.Append(FormatUnit(i, value));
                }
                
                if(i > endUnitIndex)
                    sb.Append(separator);
            }
            
            text.text = sb.ToString();
        }

        
        

        #region Utility

        private void UpdateListTimeUnitValue()
        {
            listTimeUnitValue[(int)UnitId.MilliSecond] = remainingTimeSpan.Milliseconds;
            listTimeUnitValue[(int)UnitId.Second] = remainingTimeSpan.Seconds;
            listTimeUnitValue[(int)UnitId.Minute] = remainingTimeSpan.Minutes;
            listTimeUnitValue[(int)UnitId.Hour] = remainingTimeSpan.Hours;
            listTimeUnitValue[(int)UnitId.Day] = remainingTimeSpan.Days;
        }

        /// <summary>
        /// Nếu còn 0h 59m 03s 500ms và truyền vào unityIndex là 1 (tức seconds) thì sẽ làm tròn thành 1h 0m 0s 0ms.
        /// Khi bạn chọn chỉ hiển thị H:M, nếu thời gian còn lại là 0h 0m 3s, bạn không thể hiển thị 0h 0m được. Vì vậy cần làm tròn lên thành 1m
        /// </summary>
        private void RoundUpFromUnitValue(int unitIndex)
        {
            long sumBefore = 0;
            for (int i = unitIndex - 1; i >= 0; i--)
            {
                sumBefore += listTimeUnitValue[i];
            }
            
            if(sumBefore <= 0)
                return;


            listTimeUnitValue[unitIndex]++;
            for (int i = unitIndex; i < listTimeUnitValue.Length - 1; i++)
            {
                if (listTimeUnitValue[i] >= listTimeUnitMaxValue[i])
                {
                    listTimeUnitValue[i] = 0;
                    listTimeUnitValue[i + 1]++;
                }
            }
        }

        private string FormatUnit(int unitIndex, int value)
        {
            if (unitIndex == (int)UnitId.MilliSecond)
                return string.Format(millisecondFormat, value);
            
            if (unitIndex == (int)UnitId.Second)
                return string.Format(secondFormat, value);
            
            if (unitIndex == (int)UnitId.Minute)
                return string.Format(minutesFormat, value);
            
            if (unitIndex == (int)UnitId.Hour)
                return string.Format(hourFormat, value);
            
            if (unitIndex == (int)UnitId.Day)
                return string.Format(dayFormat, value);
            
            Debug.LogError("Chưa code cho unit: " + unitIndex);
            return value.ToString();
        }

        /// <summary>
        /// ví dụ unitIndex truyền vào là hour, thì sẽ lấy số ngày nhân 24 + số giờ hiện tại
        /// </summary>
        private int CalculateUnitTotalValue(int unitIndex)
        {
            int prevUnitValue = listTimeUnitValue[listTimeUnitValue.Length - 1];
            for (int i = listTimeUnitValue.Length - 2; i >= unitIndex; i--)
            {
                prevUnitValue = listTimeUnitValue[i] + prevUnitValue * listTimeUnitMaxValue[i];
            }
            
            return prevUnitValue;
        }

        private double GetUnitTotalValue(TimeSpan timeSpan, int index)
        {
            if (index == 0)
                return timeSpan.TotalMilliseconds;
            
            if (index == 1)
                return timeSpan.TotalSeconds;
            
            if (index == 2)
                return timeSpan.TotalMinutes;
            
            if (index == 3)
                return timeSpan.TotalHours;
            
            if (index == 4)
                return timeSpan.TotalDays;

            throw new IndexOutOfRangeException();
        }
        
        private int GetUnitValue(TimeSpan timeSpan, int index)
        {
            if (index == 0)
                return timeSpan.Milliseconds;
            
            if (index == 1)
                return timeSpan.Seconds;
            
            if (index == 2)
                return timeSpan.Minutes;
            
            if (index == 3)
                return timeSpan.Hours;
            
            if (index == 4)
                return timeSpan.Days;

            throw new IndexOutOfRangeException();
        }

        private int GetMaxUnitIndexGreaterZero(TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0)
                return 4;
            
            if (timeSpan.Hours > 0)
                return 3;
            
            if (timeSpan.Minutes > 0)
                return 2;
            
            if (timeSpan.Seconds > 0)
                return 1;
            
            return 0;
        }

        private void CalculateRangeUnitIndexToDisplay(TimeSpan timeSpan, out int startUnitIndex, out int endUnitIndex)
        {
            int minUnit = (int)this.minUnit;
            int maxUnit = (int)this.maxUnit;
            int unitIndex = GetMaxUnitIndexGreaterZero(timeSpan);
            
            
            //Áp dụng config maxUnit - Claim unitIndex trước khi tính bất kỳ số nào khác
            if(unitIndex > maxUnit)
                unitIndex = maxUnit;
            
            
            //Áp dụng config maxNumberDisplay - Để tính ra endUnitIndexx
            startUnitIndex = unitIndex;
            endUnitIndex = startUnitIndex - (maxNumberUnitDisplaySameTime - 1);
            
            
            //Nếu EndUnitIndex lớn hơn minUnit thì cả StartUnitIndex và EndUnitIndex đều thỏa mãn tất cả điều kiện
            if(endUnitIndex >= minUnit)
                return;
            
            
            //Áp dụng config minUnit - để claim lại endUnitIndex
            endUnitIndex = minUnit;
            
            
            //Range bị ngắn lại rồi - Nếu numberDisplay vẫn lớn hơn minNumberDisplay thì vẫn thỏa mãn
            if(startUnitIndex - endUnitIndex + 1 >= minNumberUnitDisplaySameTime)
                return;


            //Áp dụng config minNumberDisplay - Tăng range lên tới mức minNumberDisplay
            startUnitIndex = endUnitIndex + (minNumberUnitDisplaySameTime - 1);
            
            //Áp dụng maxUnit một lần nữa
            if(startUnitIndex > maxUnit)
                startUnitIndex = maxUnit;
        }

        #endregion
        
    }
}