using Newtonsoft.Json.Linq;
using SimpleJSON;

namespace PriceSystem
{
    public class PriceData_Ads : PriceData
    {
        public int amount;
        
        //{"priceType":"ads","amount":"1"}
        public static PriceData_Ads ParseFromJsonNode(JSONNode jsonNode)
        {
            PriceData_Ads priceData = new PriceData_Ads();
            priceData.priceType = PriceType.Ads;
            if (jsonNode.HasKey("amount") == false)
                priceData.amount = 1;
            else
                priceData.amount = jsonNode["amount"].AsInt;
            return priceData;
        }
    }
}