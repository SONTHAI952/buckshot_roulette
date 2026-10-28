using UnityEngine;
#if ZEROX_LOCALIZATION_SYSTEM
using ZeroX.LocalizationSystem;
#endif
using ZeroX.WindowSystem;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace ZeroX.ToastSystem
{
    public class ToastFlyUp : MonoBehaviour
    {
        public class Message
        {
            public string message;
            public float duration;
            public Color textColor;
            public Color backgroundColor;
            public RectTransform target;
        }
        
        
        
        
        //Fields
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform container;
        [SerializeField] Window messageModel;
        [SerializeField] float defaultDuration = 2f;
        [SerializeField] float flyDistance = 50f;
        [SerializeField] Color defaultTextColor = Color.black;
        [SerializeField] Color defaultBGColor = Color.white;

        [Header("Component")]
        [SerializeField] TextMeshProUGUI textMessage;
        [SerializeField] Image background;
        
        
        
        //Temp
        bool isInitialized = false;
        
        
        
        //Static
        private static ToastFlyUp instance;
        
        
        
        //Properties
        public static ToastFlyUp Instance
        {
            get
            {
                if (instance == null)
                {
                    ToastFlyUp prefab;
                    var setting = ToastSetting.Instance;
                    if (setting == null || setting.toastFlyUp == null)
                        prefab = Resources.Load<ToastFlyUp>(ToastConst.TOAST_DEFAULT_RESOURCES_FOLDER_PATH + "Toast Fly Up");
                    else
                        prefab = setting.toastFlyUp;
                    
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
                Debug.Log("<color=red>Chỉ được có 1 Toast Fly Up Instance tồn tại ---> Xóa</color>");
                Destroy(gameObject);
            }
            
            InitIfNot();
        }

        void InitIfNot()
        {
            if (isInitialized)
                return;
            
            isInitialized = true;

            RectTransform rect = messageModel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
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



        Vector2 GetScreenPointOfTarget(Canvas targetCanvas, RectTransform target)
        {
            if (targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return target.position;

            
            if (targetCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                if (targetCanvas.worldCamera == null)
                    return target.position;
                
                return targetCanvas.worldCamera.WorldToScreenPoint(target.position);
            }
            
            
            if(targetCanvas.renderMode == RenderMode.WorldSpace)
            {
                return Camera.main.WorldToScreenPoint(target.position);
            }
            
            
            Debug.LogError("Not code for render mode: " + targetCanvas.renderMode);
            return Vector2.zero;
        }

        Camera GetCameraOfCanvas(Canvas canvas)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                return canvas.worldCamera;
            }

            
            if (canvas.renderMode == RenderMode.WorldSpace)
                return Camera.main;
            
            
            Debug.LogError("Not code for render mode: " + canvas.renderMode);
            return null;
        }
        
        
        Vector3 CalculateMessageLocalPos(RectTransform target)
        {
            if (target == null)
            {
                Debug.LogError("Target is null");
                return Vector3.zero;
            }
            
            
            Canvas targetCanvas = target.GetComponentInParent<Canvas>();
            if (targetCanvas == null)
            {
                Debug.LogError("Canvas of target is null");
                return Vector3.zero;
            }
            
            
            //Tính localPoint
            Vector2 screenPoint = GetScreenPointOfTarget(targetCanvas, target);
            Camera thisCanvasCamera = GetCameraOfCanvas(canvas);

            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(container, screenPoint, thisCanvasCamera, out var localPoint);
            return localPoint;
        }

        void Show_Core(Message message)
        {
            canvas.gameObject.SetActive(true);
            textMessage.text = message.message;
            textMessage.color = message.textColor;
            background.color = message.backgroundColor;

            Window window = Instantiate(messageModel, container);
            RectTransform rectView = (RectTransform)window.transform;
            rectView.localPosition = CalculateMessageLocalPos(message.target);
            
            
            window.WindowEvents.OnClosed.AddListener((p) =>
            {
                Destroy(window.gameObject);
                if(container.childCount == 1)
                    canvas.gameObject.SetActive(false);
            });
            window.Open();
            window.CloseDelay(message.duration);
            rectView.DOAnchorPosY(rectView.anchoredPosition.y + flyDistance, message.duration + window.CloseDuration).SetEase(Ease.Linear).SetUpdate(true);
        }

        
        //Public Method
        public static Message CreateMessage(bool showBG = true)
        {
            return Instance.CreateMessage_Core(showBG);
        }

        public static Message CreateMessage(RectTransform target, bool showBG = true)
        {
            Message message = CreateMessage(showBG);
            message.target = target;
            return message;
        }

        public static void Show(Message message)
        {
            Instance.Show_Core(message);
        }
        
        public static void Show(string message, RectTransform target)
        {
            Message m = CreateMessage();
            m.message = message;
            m.target = target;
            Show(m);
        }

#if ZEROX_LOCALIZATION_SYSTEM
        public static void Show_L(RectTransform target, string keyMessage)
        {
            Message m = CreateMessage();
            m.target = target;
            m.message = LManager.GetText(keyMessage);
            Show(m);
        }
        
        public static void Show_L(RectTransform target, string keyMessage, params object[] parameters)
        {
            Message m = CreateMessage();
            m.target = target;
            m.message = LManager.GetText(keyMessage, parameters);
            Show(m);
        }
        
        public static void Show_L(RectTransform target, LCategory lCategory, string keyMessage)
        {
            Message m = CreateMessage();
            m.target = target;
            m.message = LManager.GetText(lCategory, keyMessage);
            Show(m);
        }
        
        public static void Show_L(RectTransform target, LCategory lCategory, string keyMessage, params object[] parameters)
        {
            Message m = CreateMessage();
            m.target = target;
            m.message = LManager.GetText(lCategory, keyMessage, parameters);
            Show(m);
        }
#endif
    }
}