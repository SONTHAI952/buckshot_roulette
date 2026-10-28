using Newtonsoft.Json.Linq;
using SimpleJSON;

namespace PriceSystem
{
    [System.Serializable]
    public class PriceData_Free : PriceData
    {
        //{"priceType":"free"}
        public static PriceData_Free ParseFromJsonNode(JSONNode jsonNode)
        {
            PriceData_Free priceData = new PriceData_Free();
            priceData.priceType = PriceType.Free;
            return priceData;
        }
    }
}