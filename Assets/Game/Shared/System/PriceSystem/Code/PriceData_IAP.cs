using Newtonsoft.Json.Linq;
using SimpleJSON;

namespace PriceSystem
{
    public class PriceData_IAP : PriceData
    {
        public string productId;

        //{"priceType":"iap","productId":"product.sample"}
        public static PriceData_IAP ParseFromJsonNode(JSONNode jsonNode)
        {
            PriceData_IAP priceData = new PriceData_IAP();
            priceData.priceType = PriceType.IAP;
            priceData.productId = jsonNode["productId"].Value;
            return priceData;
        }
    }
}