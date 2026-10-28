using System;
using System.Collections;
using TMPro;
using UnityEngine;


namespace ZeroX
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text textFps;
        [SerializeField] private string textFormat = "{0} FPS";
        [SerializeField] private float period = 1;
        
      
        
        
        //Temp
        private float startTimeOfPeriod;
        private int fpsAccumulated;

        
        
        private void OnEnable()
        {
            ResetCounter();
            textFps.text = string.Format(textFormat, 0);
        }


        private void Reset()
        {
            textFps = GetComponent<TMP_Text>();
        }


        private void Update()
        {
            fpsAccumulated++;


            if (Time.realtimeSinceStartup - startTimeOfPeriod >= period)
            {
                //Calculate FPS
                float timeAccumulated = Time.realtimeSinceStartup - startTimeOfPeriod;
                float fps = fpsAccumulated / (timeAccumulated / period); //timeAccumulated có thể sẽ vượt qua period một chút, nên dòng này fix lại về số FPS chuẩn trong một period
                fps /= period;
                
                ResetCounter();
                
                //Visual
                textFps.text = string.Format(textFormat, Mathf.RoundToInt(fps));
            }
        }

        private void ResetCounter()
        {
            fpsAccumulated = 0;
            startTimeOfPeriod = Time.realtimeSinceStartup;
        }
        
        public static WaitToken<int> CalculateAverageFps(float duration)
        {
            WaitToken<int> waitToken = new WaitToken<int>();
            ZeroRx.RunCoroutine(Timeline());
            return waitToken;
            
            
            IEnumerator Timeline()
            {
                yield return null;
                
                float startTime = Time.realtimeSinceStartup;
                int fpsAcc = 0;
                
                
                while (Time.realtimeSinceStartup - startTime < duration)
                {
                    yield return null;
                    fpsAcc++;
                }
                
                
                float timeAccumulated = Time.realtimeSinceStartup - startTime;
                float fps = fpsAcc / (timeAccumulated / duration); //timeAccumulated có thể sẽ vượt qua period một chút, nên dòng này fix lại về số FPS chuẩn trong một period
                fps /= duration;
                
                waitToken.SetResult(Mathf.RoundToInt(fps));
            }
        }
    }
}
