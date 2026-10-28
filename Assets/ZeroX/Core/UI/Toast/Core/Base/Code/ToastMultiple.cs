using System.Collections.Generic;
using UnityEngine;

#if ZEROX_LOCALIZATION_SYSTEM
using ZeroX.LocalizationSystem;
#endif

using ZeroX.WindowSystem;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

namespace ZeroX.ToastSystem
{
    public class ToastMultiple : MonoBehaviour
    {
        public class Message
        {
            public string message;
            public float duration;
            public Color textColor;
            public Color backgroundColor;
        }
        
        
        
        
        //Fields
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform m_container;
        [SerializeField] Window messageModel;
        [SerializeField] float defaultDuration = 2;
        [SerializeField] Color defaultTextColor = Color.black;
        [SerializeField] Color defaultBGColor = Color.white;
        
        [Header("Component")]
        [SerializeField] TextMeshProUGUI textMessage;
        [SerializeField] Image background;

        
        
        //Temp
        List<RectTransform> listViewMessageShowing = new List<RectTransform>();

        
        
        //Static
        private static ToastMultiple instance;
        static ToastMultiple Instance
        {
            get
            {
                if (instance == null)
                {
                    ToastMultiple prefab;
                    var setting = ToastSetting.Instance;
                    if (setting == null || setting.toastMultiple == null)
                        prefab = Resources.Load<ToastMultiple>(ToastConst.TOAST_DEFAULT_RESOURCES_FOLDER_PATH + "Toast Multiple");
                    else
                        prefab = setting.toastMultiple;

                    instance = Instantiate(prefab);
                }

                return instance;
            }
        }

        
        
        
        void Awake()
        {
            if (instance == null || instance == this)
            {
                instance = this;
                
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                messageModel.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("<color=red>Chỉ được có 1 Toast Multiple Instance tồn tại ---> Xóa</color>");
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        Message CreateMessage_Core(bool showBG = true)
        {
            Message message = new Message();
            message.duration = defaultDuration;
            message.textColor = defaultTextColor;
            if (showBG)
                message.backgroundColor = defaultBGColor;
            else
                message.backgroundColor = new Color(0, 0, 0, 0);
            return message;
        }
        
        void Show_Core(Message message)
        {
            canvas.gameObject.SetActive(true);
            textMessage.text = message.message;
            textMessage.color = message.textColor;
            background.color = message.backgroundColor;

            Window window = Instantiate(messageModel, m_container);
            RectTransform rectView = window.GetComponent<RectTransform>();
            rectView.anchoredPosition = messageModel.GetComponent<RectTransform>().anchoredPosition;
            window.WindowEvents.OnClosed.AddListener((p) =>
            {
                listViewMessageShowing.Remove(rectView);
                Destroy(window.gameObject);
                if (listViewMessageShowing.Count == 0)
                    canvas.gameObject.SetActive(false);
            });
            window.Open();
            window.CloseDelay(message.duration);

            if (listViewMessageShowing.Count > 0)
            {
                float posY = messageModel.GetComponent<RectTransform>().anchoredPosition.y + listViewMessageShowing.Sum(p => (p.sizeDelta.y + 20));
                foreach (var viewAnother in listViewMessageShowing)
                {
                    RectTransform rect = viewAnother.GetComponent<RectTransform>();
                    rect.DOAnchorPosY(posY, 0.3f).SetUpdate(true);
                    posY -= rect.sizeDelta.y + 20f;
                }
            }

            listViewMessageShowing.Add(rectView);
        }

        //Public Method

        public static Message CreateMessage(bool showBG = true)
        {
            return Instance.CreateMessage_Core(showBG);
        }

        public static void Show(Message message)
        {
            Instance.Show_Core(message);
        }
        public static void Show(string message)
        {
            Message m = CreateMessage();
            m.message = message;
            Show(m);
        }
        
#if ZEROX_LOCALIZATION_SYSTEM
        public static void Show_L(string keyMessage)
        {
            Message m = CreateMessage();
            m.message = LManager.GetText(keyMessage);
            Show(m);
        }
        
        public static void Show_L(string keyMessage, params object[] parameters)
        {
            Message m = CreateMessage();
            m.message = LManager.GetText(keyMessage, parameters);
            Show(m);
        }
        
        public static void Show_L(LCategory lCategory, string keyMessage)
        {
            Message m = CreateMessage();
            m.message = LManager.GetText(lCategory, keyMessage);
            Show(m);
        }
        
        public static void Show_L(LCategory lCategory, string keyMessage, params object[] parameters)
        {
            Message m = CreateMessage();
            m.message = LManager.GetText(lCategory, keyMessage, parameters);
            Show(m);
        }
#endif
    }
}