using System.Collections.Generic;
using System.Linq;

namespace PriceSystem
{
    [System.Serializable]
    public class PriceAdsDM
    {
        public List<BuyWithAdsInfo> listBuyWithAdsInfo = new List<BuyWithAdsInfo>();

        public int GetNumberOfTimesPurchased(string key)
        {
            var info = listBuyWithAdsInfo.FirstOrDefault(i => i.key == key);
            if (info == null)
                return 0;

            return info.purchased;
        }

        public int GetNumberOfAdsWatched(string key)
        {
            var info = listBuyWithAdsInfo.FirstOrDefault(i => i.key == key);
            if (info == null)
                return 0;

            return info.adsWatched;
        }

        BuyWithAdsInfo GetBuyWithAdsInfoForce(string key)
        {
            var info = listBuyWithAdsInfo.FirstOrDefault(i => i.key == key);
            if (info == null)
            {
                info = new BuyWithAdsInfo();
                info.key = key;
                listBuyWithAdsInfo.Add(info);
            }

            return info;
        }

        public int IncreaseNumberOfAdsWatched(string key, int amount)
        {
            var info = GetBuyWithAdsInfoForce(key);
            info.adsWatched += amount;

            return info.adsWatched;
        }
        
        public int IncreaseNumberOfTimesPurchased(string key, int amount, bool resetNumberOfAdsWatched)
        {
            var info = GetBuyWithAdsInfoForce(key);
            info.purchased += amount;
            if (resetNumberOfAdsWatched)
                info.adsWatched = 0;

            return info.purchased;
        }

        public void SetBuyWithAdsInfo(string key, int numberOfTimePurchased, int numberAdsWatched)
        {
            var info = GetBuyWithAdsInfoForce(key);
            info.purchased = numberOfTimePurchased;
            info.adsWatched = numberAdsWatched;
        }

        public void ResetAllBuyWithAdsInfo()
        {
            listBuyWithAdsInfo.Clear();
        }
    }
}