using System;

namespace ZeroX
{
    [System.Serializable]
    public class ZDateRange
    {
        public string start = "05-11";
        public string end = "06-01";
        
        //Temp
        [NonSerialized] private bool isCached;
        [NonSerialized] private int startDay;
        [NonSerialized] private int startMonth;
        [NonSerialized] private int endDay;
        [NonSerialized] private int endMonth;
        [NonSerialized] private int endYearOffset;


        public ZDateRange()
        {
        }

        /// <summary>
        /// Format dạng: 05-11
        /// </summary>
        public ZDateRange(string start, string end)
        {
            this.start = start;
            this.end = end;
        }
        
        


        private void Cache()
        {
            isCached = true;
            
            string[] startSplit = start.Split('-');
            string[] endSplit = end.Split('-');
            
            startDay = int.Parse(startSplit[0]);
            startMonth = int.Parse(startSplit[1]);
            
            endDay = int.Parse(endSplit[0]);
            endMonth = int.Parse(endSplit[1]);
            
            bool endInNextYear = endMonth < startMonth || (endMonth == startMonth && endDay < startDay);
            endYearOffset = endInNextYear ? 1 : 0;
        }

        private void CalculateStartDateAndEndDate(DateTime currentTime, out DateTime startDate, out DateTime endDate)
        {
            if(isCached == false)
                Cache();
            
            
            
            int nowYear = currentTime.Year;
            if (nowYear < 10)
                nowYear = 10;
            
            startDate = new DateTime(nowYear, startMonth, startDay, 0, 0, 0);
            endDate = new DateTime(nowYear + endYearOffset, endMonth, endDay, 23, 59, 59);
            
            
            if(currentTime >= startDate && currentTime <= endDate) //Nếu đã ok rồi thì thôi
                return;
            
            
            //Chưa ok thì cần dịch now year lùi lại. Ví dụ startDate là 05-12-2025, endDate là 06-01-2026. Giờ đang là 2026 mà lại dịch tiếp 1 lên thì ko đúng
            //Như vậy có nghĩa là cứ không nằm trong range thì coi range là khoảng thời gian đã qua
            nowYear -= 1;
            startDate = new DateTime(nowYear, startMonth, startDay, 0, 0, 0);
            endDate = new DateTime(nowYear + endYearOffset, endMonth, endDay, 0, 0, 0);
        }

        public DateTime StartDate
        {
            get
            {
                CalculateStartDateAndEndDate(DateTime.Now, out DateTime startDate, out DateTime endDate);
                return startDate;
            }
        }
        
        public DateTime EndDate
        {
            get
            {
                CalculateStartDateAndEndDate(DateTime.Now, out DateTime startDate, out DateTime endDate);
                return endDate;
            }
        }

        public bool IsInRange(DateTime dateTime)
        {
            CalculateStartDateAndEndDate(dateTime, out DateTime startDate, out DateTime endDate);
            return dateTime >= startDate && dateTime <= endDate;
        }

        public bool IsNowInRange()
        {
            DateTime now = DateTime.Now;
            CalculateStartDateAndEndDate(now, out DateTime startDate, out DateTime endDate);
            return now >= startDate && now <= endDate;
        }
    }
}