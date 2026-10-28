using UnityEngine;

namespace ZeroX.UI
{
    public class PopupOverlayAll : MonoBehaviour
    {
        //Temp
        private static int showQueueCount = 0;
        
        
        
        //Static
        private static PopupOverlayAll instance;

        private static PopupOverlayAll Instance
        {
            get
            {
                if (instance == null)
                {
                    var prefab = Resources.Load<PopupOverlayAll>("ZeroX/Popup Overlay All");
                    instance = Instantiate(prefab);
                }

                return instance;
            }
        }
        
        

        
        
        private void Awake()
        {
            if (instance == null || instance == this)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        public static void Show()
        {
            showQueueCount++;
            Instance.gameObject.SetActive(true);
        }

        /// <summary>
        /// Only really hidden when the queue is empty
        /// </summary>
        public static void Hide()
        {
            showQueueCount--;
            if (showQueueCount < 0)
                showQueueCount = 0;
            
            if (showQueueCount == 0)
            {
                Instance.gameObject.SetActive(false);
            }
        }

        public static void HideDelay(float delay)
        {
            ZeroRx.RunActionDelayUnscaledTime(Hide, delay);
        }
    }
}