using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using UnityEngine;

namespace PriceSystem
{
    public static class PriceDataParser
    {
        public static PriceData ParseFromJson(string json)
        {
            var jsonNode = JSON.Parse(json);
            return ParseFromJsonNode(jsonNode);
        }

        public static PriceData ParseFromJsonNode(JSONNode jsonNode)
        {
            string priceTypeString = jsonNode["priceType"].Value;
            PriceType priceType = PriceTypeExtension.FromString(priceTypeString);

            switch (priceType)
            {
                case PriceType.GameItem:
                    return PriceData_GameItem.ParseFromJsonNode(jsonNode);
                case PriceType.Ads:
                    return PriceData_Ads.ParseFromJsonNode(jsonNode);
                case PriceType.IAP:
                    return PriceData_IAP.ParseFromJsonNode(jsonNode);
                case PriceType.Free:
                    return PriceData_Free.ParseFromJsonNode(jsonNode);
                default:
                {
                    Debug.LogError("Chưa code parse cho price type này: " + priceType);
                    return null;
                }
            }
        }

        public static List<PriceData> ParseListFromJson(string json)
        {
            var jsonArray = JSON.Parse(json).AsArray;
            
            List<PriceData> listPriceData = new List<PriceData>();

            foreach (var jsonNode in jsonArray.Children)
            {
                var priceData = ParseFromJsonNode(jsonNode);
                listPriceData.Add(priceData);
            }

            return listPriceData;
        }
    }
}