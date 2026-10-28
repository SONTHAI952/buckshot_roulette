using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace ZeroX.Extensions
{
    public static class ListExtension
    {
        public static void Shuffle(this IList l)
        {
            int count = l.Count;
            for (int i = 0; i < count; i++)
            {
                object objA = l[i];
                int newIndex = Random.Range(0, count);
                object objB = l[newIndex];
                l[i] = objB;
                l[newIndex] = objA;
            }
        }

        public static void Foreach<T>(this List<T> list, Action<int, T> action)
        {
            int index = 0;
            foreach (T item in list)
            {
                action(index, item);
                index++;
            }
        }

        /// <summary>
        /// Nếu bạn có một list a, b, c, d. StartIndexIncluded là 2, thì bạn sẽ nhận đc chuỗi index 2, 3, 0, 1
        /// </summary>
        public static IEnumerable<int> ForCyclePositive<T>(this List<T> list, int startIndexIncluded)
        {
            for (int i = startIndexIncluded; i < list.Count; i++)
            {
                yield return i;
            }
            
            for (int i = 0; i < startIndexIncluded; i++)
            {
                yield return i;
            }
        }
        
        /// <summary>
        /// Nếu bạn có một list a, b, c, d. StartIndexIncluded là 2, thì bạn sẽ nhận đc chuỗi index 2, 1, 0, 3
        /// </summary>
        public static IEnumerable<int> ForCycleNegative<T>(this List<T> list, int startIndexIncluded)
        {
            for (int i = startIndexIncluded; i >= 0; i--)
            {
                yield return i;
            }
            
            for (int i = list.Count - 1; i > startIndexIncluded; i--)
            {
                yield return i;
            }
        }
        
        /// <summary>
        /// Nếu bạn có một list a, b, c, d. StartIndexIncluded là 2, thì bạn sẽ nhận đc chuỗi c, d, a, b
        /// </summary>
        public static IEnumerable<T> ForeachCyclePositive<T>(this List<T> list, int startIndexIncluded)
        {
            for (int i = startIndexIncluded; i < list.Count; i++)
            {
                yield return list[i];
            }
            
            for (int i = 0; i < startIndexIncluded; i++)
            {
                yield return list[i];
            }
        }
        
        /// <summary>
        /// Nếu bạn có một list a, b, c, d. StartIndexIncluded là 2, thì bạn sẽ nhận đc chuỗi c, d, a, b
        /// </summary>
        public static IEnumerable<T> ForeachCycleNegative<T>(this List<T> list, int startIndexIncluded)
        {
            for (int i = startIndexIncluded; i >= 0; i--)
            {
                yield return list[i];
            }
            
            for (int i = list.Count - 1; i > startIndexIncluded; i--)
            {
                yield return list[i];
            }
        }

        
        /// <summary>
        /// Lấy ngẫu nhiên 1 phần tử, nhưng không thay đổi list
        /// </summary>
        public static T GetRandom<T>(this List<T> list)
        {
            if (list.Count == 0)
                throw new InvalidOperationException("List is empty");
            
            int index = Random.Range(0, list.Count);
            return list[index];
        }
        
        /// <summary>
        /// Lấy ngẫu nhiên 1 phần tử và xóa phần tử đó khỏi list
        /// </summary>
        public static T ReleaseRandom<T>(this List<T> list)
        {
            if (list.Count == 0)
                throw new InvalidOperationException("List is empty");
            
            int index = Random.Range(0, list.Count);
            T value = list[index];
            list.RemoveAt(index);
            return value;
        }
    }
}