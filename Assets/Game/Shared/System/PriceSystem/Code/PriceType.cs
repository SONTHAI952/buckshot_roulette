using System.Collections.Generic;
using UnityEngine;

namespace PriceSystem
{
    public enum PriceType
    {
        Null = -1, GameItem, Ads, IAP, Free
    }

    public static class PriceTypeExtension
    {
        static Dictionary<string, PriceType> dictStringToType = new Dictionary<string, PriceType>()
        {
            {"game_item", PriceType.GameItem},
            {"ads", PriceType.Ads},
            {"iap", PriceType.IAP},
            {"free", PriceType.Free},
        };

        public static PriceType FromString(string priceTypeString)
        {
            if (dictStringToType.TryGetValue(priceTypeString, out var priceType))
            {
                return priceType;
            }
            else
            {
                Debug.LogError("Chưa code FromString cho loại price type này: " + priceTypeString);
                return PriceType.Null;
            }
        }
    }
}