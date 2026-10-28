using UnityEngine;
using UnityEngine.UI;

namespace ZeroX
{
    public class ZScreen : MonoBehaviour
    {
        [SerializeField] private CanvasScaler canvasScaler;



        //Const
        private static readonly Vector2Int referenceResolutionPortrait = new Vector2Int(1080, 2400);
        private static readonly Vector2Int referenceResolutionLandscape = new Vector2Int(2400, 1080);
        private static readonly Vector2Int referenceResolutionSquare = new Vector2Int(1080, 1080);
        
        
        
        
        //Temp
        private RectTransform content;
        private static Vector2Int referenceResolution = new Vector2Int(-1, -1);
        private static Vector2Int relativeScreenSize = new Vector2Int(-1, -1);
        private static Vector2Int rawScreenSize = new Vector2Int(-1, -1);
        
        
        
        //Static
        private static ZScreen instance;

        private static ZScreen Instance
        {
            get
            {
                if (instance == null)
                {
                    var prefab = Resources.Load<ZScreen>("ZeroX/ZScreenSystem/ZScreen");
                    instance = Instantiate(prefab);
                    instance.name = prefab.name;
                }

                return instance;
            }
        }

        public static Vector2Int ReferenceResolution => referenceResolution;
        
        
        


        
        
        #region Unity Method

        private void Awake()
        {
            if (instance == null || instance == this)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                Init();
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

        private void Update()
        {
            Refresh();
        }

        #endregion



        #region Init

        [RuntimeInitializeOnLoadMethod]
        private static void AutoSpawn()
        {
            var a = Instance;
        }

        public void Refresh()
        {
            if (CalculateReferenceResolution() != referenceResolution ||
                CalculateRawScreenSize() != rawScreenSize ||
                CalculateRelativeScreenSize() != relativeScreenSize)
            {
                Init();
            }
        }
        
        public void Init()
        {
            content = (RectTransform) transform;
            
            //ReferenceResolution
            referenceResolution = CalculateReferenceResolution();
            canvasScaler.referenceResolution = referenceResolution;
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
            
            //Relative - Raw
            relativeScreenSize = CalculateRelativeScreenSize();
            rawScreenSize = CalculateRawScreenSize();

            Debug.LogFormat("ZScreen Init - RelativeScreenSize: {0}   -   RawScreenSize: {1}", relativeScreenSize, rawScreenSize);
        }
        
        private static Vector2Int CalculateReferenceResolution()
        {
#if UNITY_EDITOR
            Vector2Int nativeResolution = GetGameViewResolution();
#else
            Vector2Int nativeResolution = new Vector2Int(Screen.width, Screen.height);
#endif
        
        
            if (nativeResolution.x == nativeResolution.y)
                return referenceResolutionSquare;

            if (nativeResolution.x < nativeResolution.y)
                return referenceResolutionPortrait;

            return referenceResolutionLandscape;
        }
    
        
#if UNITY_EDITOR

        private static System.Reflection.MethodInfo getSizeOfMainGameView;
        private static Vector2Int GetGameViewResolution()
        {
            if (getSizeOfMainGameView == null)
            {
                System.Type gameViewType = System.Type.GetType("UnityEditor.GameView,UnityEditor");
                getSizeOfMainGameView = gameViewType.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            }
           
            
            Vector2 gameViewSize = (Vector2)getSizeOfMainGameView.Invoke(null,null);
            return new Vector2Int((int)gameViewSize.x, (int)gameViewSize.y);
        }
        
#endif

        private Vector2Int CalculateRelativeScreenSize()
        {
            var sizeDelta = content.sizeDelta;
            return new Vector2Int((int)sizeDelta.x, (int)sizeDelta.y);
        }

        private Vector2Int CalculateRawScreenSize()
        {
#if UNITY_EDITOR
            return GetGameViewResolution();
#else
            return new Vector2Int(Screen.width, Screen.height);
#endif
        }
        
        #endregion



        #region Relative

        /// <summary>
        /// Relative Screen Size
        /// </summary>
        public static Vector2Int ScreenSize => relativeScreenSize;
        
        /// <summary>
        /// Relative Screen Width
        /// </summary>
        public static int Width => relativeScreenSize.x;
        
        /// <summary>
        /// Relative Screen Height
        /// </summary>
        public static int Height => relativeScreenSize.y;
        
        public static Vector3 MousePosition
        {
            get
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(Instance.content, Input.mousePosition, null, out var localPoint);
                return localPoint;
            }
        }

        #endregion
        
        
        
        
        #region Raw

        public static Vector2Int RawScreenSize => rawScreenSize;
        public static int RawWidth => rawScreenSize.x;
        public static int RawHeight => rawScreenSize.y;
        
        public static Vector3 RawMousePosition => Input.mousePosition;

        #endregion



        #region Convert
        
        public static float ConvertX_RelativeToRaw(float x)
        {
            float delta = x / relativeScreenSize.x;
            return delta * rawScreenSize.x;
        }

        public static float ConvertY_RelativeToRaw(float y)
        {
            float delta = y / relativeScreenSize.y;
            return delta * rawScreenSize.y;
        }

        
        public static float ConvertX_RawToRelative(float x)
        {
            float delta = x / rawScreenSize.x;
            return delta * relativeScreenSize.x;
        }
        
        public static float ConvertY_RawToRelative(float y)
        {
            float delta = y / rawScreenSize.y;
            return delta * relativeScreenSize.y;
        }

        public static Vector2 Convert_RawToRelative(Vector2 raw)
        {
            float deltaX = raw.x / rawScreenSize.x;
            float deltaY = raw.y / rawScreenSize.y;
            return new Vector2(deltaX * relativeScreenSize.x, deltaY * relativeScreenSize.y);
        }
        
        public static Vector2 Convert_RelativeToRaw(Vector2 relative)
        {
            float deltaX = relative.x / relativeScreenSize.x;
            float deltaY = relative.y / relativeScreenSize.y;
            return new Vector2(deltaX * rawScreenSize.x, deltaY * rawScreenSize.y);
        }

        #endregion
    }
}