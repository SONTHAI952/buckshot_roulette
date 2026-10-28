using SimpleJSON;

namespace PriceSystem
{
    public class PriceData_GameItem : PriceData
    {
        public string gameItemId;
        public long amount;

        //{"priceType":"game_item","gameItemId":"coin","amount":"50"}
        public static PriceData_GameItem ParseFromJsonNode(JSONNode jsonNode)
        {
            PriceData_GameItem priceData = new PriceData_GameItem();
            priceData.priceType = PriceType.GameItem;
            priceData.gameItemId = jsonNode["gameItemId"].Value;
            priceData.amount = jsonNode["amount"].AsLong;
            return priceData;
        }
    }
}