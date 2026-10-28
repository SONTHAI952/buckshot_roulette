using System.Collections.Generic;
using UnityEngine;

#if ZEROX_LOCALIZATION_SYSTEM
using ZeroX.LocalizationSystem;
#endif

using ZeroX.WindowSystem;
using TMPro;
using UnityEngine.UI;

namespace ZeroX.ToastSystem
{
    public class ToastSingle : MonoBehaviour
    {
        public class Message
        {
            public string message;
            public float duration;
            public Color textColor;
            public Color backgroundColor;
        }
        
        
        
        
        //Fields
        [SerializeField] Window windowMessage;
        [SerializeField] float defaultDuration = 2;
        [SerializeField] Color defaultTextColor = Color.black;
        [SerializeField] Color defaultBGColor = Color.white;

        [Header("Component")]
        [SerializeField] TextMeshProUGUI textMessage;
        [SerializeField] Image background;
        
        
        //Temp
        bool isInitialized = false;
        List<Message> listMessage = new List<Message>();
        Message messageShowing = null;
        
        
        //Static
        static ToastSingle instance;

        
        //Properties
        static ToastSingle Instance
        {
            get
            {
                if (instance == null)
                {
                    ToastSingle prefab;
                    var setting = ToastSetting.Instance;
                    if (setting == null || setting.toastSingle == null)
                        prefab = Resources.Load<ToastSingle>(ToastConst.TOAST_DEFAULT_RESOURCES_FOLDER_PATH + "Toast Single");
                    else
                        prefab = setting.toastSingle;
                    
                    instance = Instantiate(prefab);
                    instance.InitIfNot();
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
            }
            else
            {
                Debug.Log("<color=red>Chỉ được có 1 Toast Single Instance tồn tại ---> Xóa</color>");
                Destroy(gameObject);
            }
            
            InitIfNot();
        }

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        void InitIfNot()
        {
            if(isInitialized)
                return;
            
            isInitialized = true;
            windowMessage.WindowEvents.OnClosed.AddListener((view) => ViewMessage_OnClosed());
        }

        Message CreateMessage_Core(bool showBG = true)
        {
            Message message = new Message();
            message.duration = Instance.defaultDuration;
            message.textColor = Instance.defaultTextColor;
            if (showBG)
                message.backgroundColor = Instance.defaultBGColor;
            else
                message.backgroundColor = new Color(0, 0, 0, 0);
            return message;
        }

        void ViewMessage_OnClosed()
        {
            messageShowing = null;
            if (listMessage.Count > 0)
            {
                Show_Core(listMessage[0]);
                listMessage.RemoveAt(0);
            }
        }

        void Show_Core(Message message)
        {
            InitIfNot();

            messageShowing = message;
            textMessage.text = message.message;
            textMessage.color = message.textColor;
            background.color = message.backgroundColor;
            windowMessage.Open();
            windowMessage.CloseDelay(message.duration);
        }

        void AddMessageToQueue(Message message)
        {
            listMessage.Add(message);
        }


        //Public Method
        public static Message CreateMessage(bool showBG = true)
        {
            return Instance.CreateMessage_Core(showBG);
        }
        public static void Show(Message message)
        {
            if(Instance.messageShowing == null)
                Instance.Show_Core(message);
            else
                Instance.AddMessageToQueue(message);
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