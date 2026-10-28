using System.Collections;
using UnityEngine;

namespace PriceSystem
{
    public class PriceSystemManager : MonoBehaviour
    {
        public static PriceSystemManager Instance { get; private set; }




        private void Awake()
        {
            if (Instance == null || Instance == this)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                OnAwake();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if(Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            StartCoroutine(Loop());
        }

        private void OnAwake()
        {
            PriceAdsDI.ResetForNewDay_If_EnoughCondition();
        }
        
        
        
        
        

        IEnumerator Loop()
        {
            var waitFor1Seconds = new WaitForSecondsRealtime(1);

            while (true)
            {
                PriceAdsDI.ResetForNewDay_If_EnoughCondition();
                
                yield return waitFor1Seconds;
            }
        }
    }
}