using System;

namespace GameItemSystem
{
    public static class GameItemAmountFormat
    {
        public static bool IsPrefix_X(string id)
        {
            return id != GameItemIdDefine.coin;
        }
        
        public static string Format(long amount, bool amountIsTime, bool prefix_X)
        {
            if (amountIsTime == false)
            {
                return prefix_X ? string.Format("x{0}", amount) : amount.ToString();
            }
            else
            {
                TimeSpan timeSpan = TimeSpan.FromMinutes(amount);
                int totalHours = (int)timeSpan.TotalHours;
                int minutes = timeSpan.Minutes;

                if (totalHours <= 0)
                {
                    return string.Format("{0:0}m", minutes);
                }
                
                if (minutes > 0)
                {
                    return string.Format("{0:0}h:{1:00}m", totalHours, minutes);
                }
                else
                {
                    return string.Format("{0:0}h", totalHours);
                }
            }
        }
        
        public static string Format(string id, long amount)
        {
            bool isTime = GameItemTable.GetAmountIsTime(id);
            bool prefix_X = IsPrefix_X(id);

            return Format(amount, isTime, prefix_X);
        }

        public static string Format(GameItemData gameItemData)
        {
            bool isTime = GameItemTable.GetAmountIsTime(gameItemData.id);
            bool prefix_X = IsPrefix_X(gameItemData.id);
            
            return Format(gameItemData.Amount, isTime, prefix_X);
        }
    }
}