using System;
using ZeroX.RxSystem;
using ZeroX.SaveGameSystem;

namespace TimeOnlineSystem
{
    public static class TimeOnlineDI
    {
        private const string timeOnline_key = "timeOnline";
            private const string totalTimeOnline_key = "totalTimeOnline";
            
            private const string timeOnlineInDay_key = "timeOnlineInDay";
            private const string dayOfRecord_key = "dayOfRecord";

        
        //Event
        public static readonly Subject<TimeOnlineChangedData> onTimeOnlineChanged = new Subject<TimeOnlineChangedData>();
        public static readonly Subject<ResetDayData> onResetDay = new Subject<ResetDayData>();
        
        
        private static SGNode TimeOnlineNode => SaveGame.Default[timeOnline_key];

        
        
        
        //Tổng số giây online
        public static int TotalSecondsOnline
        {
            get => TimeOnlineNode[totalTimeOnline_key].IntValue;
            private set => TimeOnlineNode[totalTimeOnline_key].IntValue = value;
        }
        
        public static int SecondsOnlineInDay
        {
            get => TimeOnlineNode[timeOnlineInDay_key].IntValue;
            private set => TimeOnlineNode[timeOnlineInDay_key].IntValue = value;
        }
        
        public static DateTime DayOfRecord
        {
            get => TimeOnlineNode[dayOfRecord_key].DateValue;
            private set => TimeOnlineNode[dayOfRecord_key].DateValue = value;
        }

        public static void PlusTimeOnline(int seconds)
        {
            TotalSecondsOnline += seconds;
            SecondsOnlineInDay += seconds;

            var changedData = new TimeOnlineChangedData();
            changedData.oldTime = SecondsOnlineInDay - seconds;
            changedData.currentTime = SecondsOnlineInDay;
            onTimeOnlineChanged.Emit(changedData);
        }
        
        public static void ResetTimeOnline()
        {
            SecondsOnlineInDay = 0;
            DayOfRecord = DateTime.Now;
        }
    }
}