using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ZeroX.Extensions
{
    public static class MonoBehaviourExtension
    {
        public static Coroutine StartTimer(this MonoBehaviour monoBehaviour, float time, UnityAction action,
            bool realTime = false)
        {
            IEnumerator Timer()
            {
                if (time != 0)
                {
                    if (realTime)
                        yield return new WaitForSecondsRealtime(time);
                    else
                        yield return new WaitForSeconds(time);
                }

                action.Invoke();
            }

            return monoBehaviour.StartCoroutine(Timer());
        }
    }
}