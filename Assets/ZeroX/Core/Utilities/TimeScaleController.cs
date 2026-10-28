using System;
using System.Collections;
using UnityEngine;

namespace ZeroX
{
    public class TimeScaleController : MonoBehaviour
    {
        public float timeScaleOriginal = 1f;
        public float fixedDeltaTimeOriginal = 0.02f;
        public bool resetOnDestroy = true;

        private static TimeScaleController instance;

        public static TimeScaleController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("Time Scale Controller").AddComponent<TimeScaleController>();
                }

                return instance;
            }
        }

        private float timeScaleBeforePause = 1f;
        private float fixedDeltaTimeBeforePause = 0.02f;
        public bool timePausing { get; private set; }

        private Coroutine coroutineChangeTimeSmooth;

        private void OnDestroy()
        {
            if (resetOnDestroy)
            {
                Time.timeScale = timeScaleOriginal;
                Time.fixedDeltaTime = fixedDeltaTimeOriginal;
            }
        }

        public void PauseTime()
        {
            if (timePausing)
            {
                Debug.Log("<color=red>Đang pause time rồi</color>");
                return;
            }

            timePausing = true;
            timeScaleBeforePause = Time.timeScale;
            fixedDeltaTimeBeforePause = Time.fixedDeltaTime;
            Time.timeScale = 0f;
        }

        public void CancelPauseTime()
        {
            if (timePausing == false)
            {
                Debug.Log("<color=red>Không đang pause time</color>");
                return;
            }

            timePausing = false;
            Time.timeScale = timeScaleBeforePause;
            Time.fixedDeltaTime = fixedDeltaTimeBeforePause;
        }

        public void ChangeTime(float timeScale)
        {
            if (timePausing)
            {
                timeScaleBeforePause = timeScale;
                fixedDeltaTimeBeforePause = fixedDeltaTimeOriginal * timeScale;
            }
            else
            {
                Time.timeScale = timeScale;
                Time.fixedDeltaTime = fixedDeltaTimeOriginal * timeScale;
            }
        }

        public void ChangeTimeSmooth(float timeScale, float duration)
        {
            if (coroutineChangeTimeSmooth != null)
            {
                StopCoroutine(coroutineChangeTimeSmooth);
                coroutineChangeTimeSmooth = null;
            }

            if (timePausing)
            {
                coroutineChangeTimeSmooth =
                    StartCoroutine(ChangeTimeSmoothIE(timeScaleBeforePause, timeScale, duration));
            }
            else
            {
                coroutineChangeTimeSmooth = StartCoroutine(ChangeTimeSmoothIE(Time.timeScale, timeScale, duration));
            }
        }

        public void CancelChangeTime()
        {
            if (coroutineChangeTimeSmooth != null)
            {
                StopCoroutine(coroutineChangeTimeSmooth);
                coroutineChangeTimeSmooth = null;
            }

            //nếu đang pause thì phải sửa time before pause, vì khi cancel pause thì sẽ set lại bằng cái này
            if (timePausing)
            {
                timeScaleBeforePause = timeScaleOriginal;
                fixedDeltaTimeBeforePause = fixedDeltaTimeOriginal;
            }
            else
            {
                Time.timeScale = timeScaleOriginal;
                Time.fixedDeltaTime = fixedDeltaTimeOriginal;
            }
        }

        IEnumerator ChangeTimeSmoothIE(float from, float to, float duration)
        {
            for (float time = 0; time < duration; time += Time.unscaledDeltaTime)
            {
                if (timePausing)
                {
                    while (timePausing)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }

                Time.timeScale = Mathf.Lerp(from, to, time / duration);
                Time.fixedDeltaTime = fixedDeltaTimeOriginal * Time.timeScale;
                yield return new WaitForEndOfFrame();
            }

            Time.timeScale = to;
            Time.fixedDeltaTime = fixedDeltaTimeOriginal * to;
        }
    }
}