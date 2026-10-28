using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX
{
    public static class FrameRateOptimizer
    {
        private static List<int> listRefreshRate = new List<int>()
        {
            60, 75, 90, 120, 140, 144, 160, 165, 180, 200, 240
        };

        
        
        public static Coroutine Optimize()
        {
            return ZeroRx.RunCoroutine(Timeline());
            
            
            
            IEnumerator Timeline()
            {
                yield return null;
                yield return null;
                yield return null;
                yield return null;
                yield return null;

                
                //Một số máy nếu không có bước này sẽ trả về refresh rate sai so với refresh rate thực tế
                Application.targetFrameRate = 1200;
                yield return null;
                yield return null;
    
                
                //Optimize
                double refreshRate = GetRefreshRate();
                int targetFrameRate = CalculateAppropriateFrameRate(refreshRate);
                Application.targetFrameRate = targetFrameRate;
            
                Debug.Log($"Refresh Rate: {refreshRate} - TargetFrameRate: {targetFrameRate}");
            }
        }
        
        private static int CalculateAppropriateFrameRate(double refreshRate)
        {
            foreach (var refreshRateToCheck in listRefreshRate)
            {
                if (refreshRate <= refreshRateToCheck + 3) //3 là sai số chấp nhận được
                    return refreshRateToCheck;
            }

            return 60;
        }
        
        private static double GetRefreshRate()
        {
#if UNITY_2022_2_OR_NEWER
            //Unity 2022.2 or Newer
            return Screen.currentResolution.refreshRateRatio.value;
#else
            //Unity 2022.1 or Lower
            return Screen.currentResolution.refreshRate;
#endif
        }
    }
}