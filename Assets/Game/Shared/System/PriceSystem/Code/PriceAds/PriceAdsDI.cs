using System;
using UnityEngine;
using ZeroX.RxSystem;
using ZeroX.SaveGameSystem;

namespace PriceSystem
{
    public static class PriceAdsDI
    {
        private const string priceSystem_key = "priceSystem";
            private const string lastTimeResetForNewDay_key = "lastTimeResetForNewDay";
            
            private const string priceAds_key = "priceAds";
                private const string listBuyWithAdsData_key = "listBuyWithAdsData";
                    private const string adsWatched_key = "adsWatched";
                    private const string purchased_key = "purchased";
                    private const string lastTimePurchased_key = "lastTimePurchased";


        //Main Nodes
        private static SGNode PriceAdsNode => SaveGame.Default[priceSystem_key][priceAds_key];
        private static SGNode ListBuyWithAdsDataNode => PriceAdsNode[listBuyWithAdsData_key];
        
        
        //Events
        public static Subject onResetForNewDay = new Subject();
        
        
        //Configs
        
        
        
        
        public static DateTime LastTimeResetForNewDay
        {
            get => PriceAdsNode[lastTimeResetForNewDay_key].GetDateTime();
            private set => PriceAdsNode[lastTimeResetForNewDay_key].SetDateTime(value);
        }
        
        
        public static int GetNumberOfAdsWatched(string key)
        {
            if (ListBuyWithAdsDataNode.TryGetChild(key, out var buyWithAdsDataNode) == false)
                return 0;

            return buyWithAdsDataNode[adsWatched_key].IntValue;
        }
        
        public static int GetNumberOfTimesPurchased(string key)
        {
            if (ListBuyWithAdsDataNode.TryGetChild(key, out var buyWithAdsDataNode) == false)
                return 0;

            return buyWithAdsDataNode[purchased_key].IntValue;
        }
        
        public static DateTime GetLastTimePurchased(string key)
        {
            if (ListBuyWithAdsDataNode.TryGetChild(key, out var buyWithAdsDataNode) == false)
                return DateTime.MinValue;

            return buyWithAdsDataNode[lastTimePurchased_key].GetDateTime();
        }
        
        public static int IncreaseNumberOfAdsWatched(string key, int amount)
        {
            var buyWithAdsDataNode = ListBuyWithAdsDataNode[key];
            
            buyWithAdsDataNode[adsWatched_key].IntValue += amount;
            return buyWithAdsDataNode[adsWatched_key].IntValue;
        }
        
        public static int IncreaseNumberOfTimesPurchased(string key, int amount)
        {
            var buyWithAdsDataNode = ListBuyWithAdsDataNode[key];
            
            buyWithAdsDataNode[purchased_key].IntValue += amount;
            buyWithAdsDataNode[lastTimePurchased_key].SetDateTime(DateTime.Now);
            return buyWithAdsDataNode[purchased_key].IntValue;
        }
        
        public static int IncreaseNumberOfTimesPurchased(string key, int amount, bool resetNumberOfAdsWatched)
        {
            var buyWithAdsDataNode = ListBuyWithAdsDataNode[key];
            
            buyWithAdsDataNode[purchased_key].IntValue += amount;
            buyWithAdsDataNode[lastTimePurchased_key].SetDateTime(DateTime.Now);
            
            if (resetNumberOfAdsWatched)
                buyWithAdsDataNode[adsWatched_key].IntValue = 0;
            
            return buyWithAdsDataNode[purchased_key].IntValue;
        }


        #region Reset For New Day
        
        public static bool IsNewDay()
        {
            return (DateTime.Now.Date - LastTimeResetForNewDay.Date).TotalDays > 0.5d; //Vì chỉ check phần Date nên nếu là ngày hôm nay thì sẽ xấp xỉ 0, nên chỉ cần check > 0.5
        }

        public static void ResetForNewDay()
        {
            Debug.Log("Price Ads - Reset For New Day");
            LastTimeResetForNewDay = DateTime.Now;
            
            
            
            ListBuyWithAdsDataNode.DeleteAllChild();
            
            
            //Events
            onResetForNewDay.Emit();
        }
        
        /// <summary>
        /// Trả về true nếu có thực hiện reset
        /// </summary>
        public static bool ResetForNewDay_If_EnoughCondition()
        {
            if (IsNewDay() == false)
                return false;
            
            ResetForNewDay();
            return true;
        }

        #endregion
        
    }
}