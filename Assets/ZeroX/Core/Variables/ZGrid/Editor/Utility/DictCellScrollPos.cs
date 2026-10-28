using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.ZGridEditors
{
    public class DictCellScrollPos
    {
        private Dictionary<int, Dictionary<int, Vector2>> dict = new Dictionary<int, Dictionary<int, Vector2>>();


        public Vector2 Get(int gPosX, int gPosY)
        {
            if (dict.TryGetValue(gPosX, out var dict2) == false)
            {
                dict2 = new Dictionary<int, Vector2>();
                dict.Add(gPosX, dict2);
            }

            if (dict2.TryGetValue(gPosY, out Vector2 scrollPos) == false)
            {
                scrollPos = Vector2.zero;
                dict2.Add(gPosY, scrollPos);
            }

            return scrollPos;
        }

        public void Set(int gPosX, int gPosY, Vector2 scrollPos)
        {
            if (dict.TryGetValue(gPosX, out var dict2) == false)
            {
                dict2 = new Dictionary<int, Vector2>();
                dict.Add(gPosX, dict2);
            }

            if (dict2.TryAdd(gPosY, scrollPos) == false)
            {
                dict2[gPosY] = scrollPos;
            }
        }
    }
}