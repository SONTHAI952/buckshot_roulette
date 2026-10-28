using System;
using System.Collections;
using UnityEngine;

namespace ZeroX.UI
{
    [DefaultExecutionOrder(-30000)]
    [RequireComponent(typeof(Canvas))]
    public class SetGUICameraForCanvas : MonoBehaviour
    {
        private static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        
        private void Awake()
        {
            if (CheckAndSet() == false)
            {
                ZeroRx.RunCoroutine(WaitGUICameraInstanceToSet());
            }
        }

        bool CheckAndSet()
        {
            if (GUICamera.Instance == null || GUICamera.Instance.Camera == null)
                return false;

            GetComponent<Canvas>().worldCamera = GUICamera.Instance.Camera;
            return true;
        }

        IEnumerator WaitGUICameraInstanceToSet()
        {
            yield return waitForEndOfFrame;
            if (CheckAndSet())
                yield break;
            
            yield return waitForEndOfFrame;
            if (CheckAndSet())
                yield break;
            
            Debug.LogError("Cannot set GUICamera for canvas because GUICamera not exist!", gameObject);
        }
    }
}