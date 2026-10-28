using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.Extensions
{
    public static class RandomExtension
    {
        public static string RandomFromBangTiLe(Dictionary<string, float> bangTiLe)
        {
            float luckyNumber = UnityEngine.Random.Range(0, 100f);
            float sum = 0;
            foreach (var kv in bangTiLe)
            {
                sum += kv.Value;
                if (sum > luckyNumber)
                    return kv.Key;
            }

            return null;
        }

        public static string RandomFromBangTiLe(Dictionary<string, Dictionary<string, float>> bangTiLe)
        {
            float luckyNumber = UnityEngine.Random.Range(0, 100f);
            float sum = 0;
            foreach (var kv in bangTiLe)
            {
                sum += kv.Value["ti_le"];
                if (sum > luckyNumber)
                    return kv.Key;
            }

            return null;
        }

        public static int RandomFromBangTiLe(Dictionary<int, float> bangTiLe)
        {
            float luckyNumber = UnityEngine.Random.Range(0, 100f);
            float sum = 0;
            foreach (var kv in bangTiLe)
            {
                sum += kv.Value;
                if (sum > luckyNumber)
                    return kv.Key;
            }

            return -1;
        }

        public static T RandomFromBangTiLe<T>(Dictionary<T, float> bangTiLe, T defaultValue)
        {
            float luckyNumber = UnityEngine.Random.Range(0, 100f);
            float sum = 0;
            foreach (var kv in bangTiLe)
            {
                sum += kv.Value;
                if (sum > luckyNumber)
                    return kv.Key;
            }

            return defaultValue;
        }
    }
}