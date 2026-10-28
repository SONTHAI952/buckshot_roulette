using UnityEngine;

namespace ZeroX
{
    public class ZInput : MonoBehaviour
    {
        //Const
        private static readonly Vector2 referenceResolutionPortrait = new Vector2(1080, 2400);
        private static readonly Vector2 referenceResolutionLandscape = new Vector2(2400, 1080);
        private static readonly Vector2 referenceResolutionSquare = new Vector2(1080, 1080);
        private const float referenceFps = 60;

        //Temp - Reference
        private static Vector2 referenceResolution = new Vector2(-1, -1); //Sẽ tự động được tính trong Update

        //Temp
        private static int? currentFingerDownId = null;
        private static int lastFrameUpdateInput = -1;

        //Temp - Touch Info
        private static bool touchDown = false;
        private static bool touchStay = false;
        private static bool touchUp = false;
        private static Vector2 prevTouchPosition = Vector2.zero;
        private static Vector2 touchPosition = Vector2.zero;
        private static Vector2 touchPositionRelative = Vector2.zero;
        private static Vector2 touchMoveDelta = Vector2.zero;
        private static Vector2 touchMoveDeltaRelative = Vector2.zero;





        #region Unity Method

        [RuntimeInitializeOnLoadMethod]
        static void AutoInit()
        {
            var zInputObj = new GameObject("ZInput");
            zInputObj.AddComponent<ZInput>();
            DontDestroyOnLoad(zInputObj);

            referenceResolution = CalculateReferenceResolution();
        }

        private void Update()
        {
            referenceResolution = CalculateReferenceResolution();
            UpdateInputIfNot();
        }

        #endregion



        #region Init

        private static Vector2 CalculateReferenceResolution()
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
                getSizeOfMainGameView = gameViewType.GetMethod("GetSizeOfMainGameView",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            }


            Vector2 gameViewSize = (Vector2)getSizeOfMainGameView.Invoke(null, null);
            return new Vector2Int((int)gameViewSize.x, (int)gameViewSize.y);
        }

#endif



        #endregion




        private static void UpdateInputIfNot()
        {
            if (lastFrameUpdateInput == Time.frameCount)
                return;
            lastFrameUpdateInput = Time.frameCount;

            UpdateTouchDown();
            UpdateTouchPositionAndPrevTouchPosition();
            UpdateTouchPositionRelative();
            UpdateTouchMoveDeltaAndMoveDeltaRelative();
            UpdateTouchUp();
            UpdateTouchStay();
        }

        public static void UpdateInput()
        {
            UpdateInputIfNot();
        }

        static void UpdateTouchDown()
        {
            touchDown = false; //Trước khi update thì reset

            if (Input.GetMouseButtonDown(0) == false)
                return;

            //Tới được đây tức là vừa có mouse hoặc finger down
            if (Input.touchCount == 0) //Device không sử dụng touch
            {
                touchDown = true;
                return;
            }

            if (currentFingerDownId == null) //Chưa có ngón nào nhấn thì mới tính
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    var touch = Input.GetTouch(i);
                    if (touch.phase == TouchPhase.Began)
                    {
                        touchDown = true;
                        currentFingerDownId = touch.fingerId;
                        return;
                    }
                }
            }
            else
            {
                //Kiểm tra xem ngón hiện tại được ghi nhận có còn tồn tại ko, ko còn thì phải update lại
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.GetTouch(i).fingerId == currentFingerDownId.Value)
                        return;
                }

                //Nếu fingerId đc ghi nhận ko còn tồn tại thì cần update lại vì vừa có 1 finger touch down
                currentFingerDownId = null;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    var touch = Input.GetTouch(i);
                    if (touch.phase == TouchPhase.Began)
                    {
                        touchDown = true;
                        currentFingerDownId = touch.fingerId;
                        return;
                    }
                }
            }
        }

        static void UpdateTouchStay()
        {
            touchStay = false;

            if (Input.GetMouseButton(0) == false)
                return;

            //Tới được đây tức là có mouse hoặc finger stay
            if (Input.touchCount == 0) //Device không sử dụng touch
            {
                touchStay = true;
                return;
            }

            touchStay = currentFingerDownId != null;
        }

        static void UpdateTouchUp()
        {
            touchUp = false;

            if (Input.touchCount == 0) //Không sử dụng touch
            {
                touchUp = Input.GetMouseButtonUp(0);
                return;
            }

            if (currentFingerDownId == null)
                return;

            for (int i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);
                if (touch.fingerId == currentFingerDownId.Value)
                {
                    if (touch.phase == TouchPhase.Ended)
                    {
                        currentFingerDownId = null;
                        touchUp = true;
                    }

                    return;
                }
            }

            //Nếu ko tìm thấy touch nào với currentFingerDown, tức là vì lý do nào đó touch đã bị mất thì cũng tính là touchUp
            currentFingerDownId = null;
            touchUp = true;
        }

        static void UpdateTouchPositionAndPrevTouchPosition()
        {
            if (Input.touchCount == 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    prevTouchPosition = Input.mousePosition;
                    touchPosition = Input.mousePosition;
                    return;
                }

                if (Input.GetMouseButton(0))
                {
                    prevTouchPosition = touchPosition;
                    touchPosition = Input.mousePosition;
                    return;
                }
            }

            if (currentFingerDownId == null)
            {
                prevTouchPosition = Vector2.zero;
                touchPosition = Vector2.zero;
                return;
            }

            for (int i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);
                if (touch.fingerId == currentFingerDownId)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        prevTouchPosition = touch.position;
                        touchPosition = touch.position;
                    }
                    else
                    {
                        prevTouchPosition = touchPosition;
                        touchPosition = touch.position;
                    }

                    return;
                }
            }

            prevTouchPosition = Vector2.zero;
            touchPosition = Vector2.zero;
        }

        static void UpdateTouchPositionRelative()
        {
            float screenRatio = referenceResolution.x / referenceResolution.y;
            float height = Screen.width / screenRatio;
            if (height - 1 <= Screen.height) //Có thể dựa trên width, -1 để tránh lỗi số float gần đúng
            {
                float ratio = referenceResolution.x / Screen.width;
                touchPositionRelative.x = touchPosition.x * ratio;
                touchPositionRelative.y = touchPosition.y * ratio;
                return;
            }
            else //Không dựa trên width thì chắc chắn dựa trên height
            {
                float ratio = referenceResolution.y / Screen.height;
                touchPositionRelative.x = touchPosition.x * ratio;
                touchPositionRelative.y = touchPosition.y * ratio;
                return;
            }
        }

        static void UpdateTouchMoveDeltaAndMoveDeltaRelative()
        {
            touchMoveDelta = touchPosition - prevTouchPosition;

            float screenRatio = referenceResolution.x / referenceResolution.y;


            float height = Screen.width / screenRatio;
            if (height - 1 <= Screen.height) //Có thể dựa trên width, -1 để tránh lỗi số float gần đúng
            {
                float ratio = referenceResolution.x / Screen.width;
                touchMoveDeltaRelative.x = touchMoveDelta.x * ratio;
                touchMoveDeltaRelative.y = touchMoveDelta.y * ratio;
            }
            else //Không dựa trên width thì chắc chắn dựa trên height
            {
                float ratio = referenceResolution.y / Screen.height;
                touchMoveDeltaRelative.x = touchMoveDelta.x * ratio;
                touchMoveDeltaRelative.y = touchMoveDelta.y * ratio;
            }

            //AdjustMoveDeltaAndMoveDeltaRelativeByFps(); //Delta là đã dựa theo 2 frame rồi nên không cần dựa vào fps nữa
        }

        /// <summary>
        /// Move delta sẽ nhỏ hơn trên thiết bị có FPS cao, nên cần chia tỉ lệ để có moveDelta chuẩn
        /// </summary>
        static void AdjustMoveDeltaAndMoveDeltaRelativeByFps()
        {
            float currentFps = 1 / Time.deltaTime;
            float ratio = currentFps / referenceFps;

            touchMoveDelta *= ratio;
            touchMoveDeltaRelative *= ratio;
        }


        private static Touch? GetTouchByFingerId(int fingerId)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);
                if (touch.fingerId == fingerId)
                    return touch;
            }

            return null;
        }





        #region Public API

        public static bool IsTouchDown
        {
            get
            {
                UpdateInputIfNot();
                return touchDown;
            }
        }

        public static bool IsTouchStay
        {
            get
            {
                UpdateInputIfNot();
                return touchStay;
            }
        }

        public static bool IsTouchUp
        {
            get
            {
                UpdateInputIfNot();
                return touchUp;
            }
        }

        /// <summary>
        /// Only use when touched
        /// </summary>
        public static Vector2 TouchPosition
        {
            get
            {
                UpdateInputIfNot();
                return touchPosition;
            }
        }

        /// <summary>
        /// Only use when touched
        /// </summary>
        public static Vector2 TouchPositionRelative
        {
            get
            {
                UpdateInputIfNot();
                return touchPositionRelative;
            }
        }

        /// <summary>
        /// No need to multiply by Time.deltaTime because delta is calculated based on the change between 2 frames
        /// <para>Only use when touched</para>
        /// </summary>
        public static Vector2 TouchMoveDelta
        {
            get
            {
                UpdateInputIfNot();
                return touchMoveDelta;
            }
        }

        /// <summary>
        /// No need to multiply by Time.deltaTime because delta is calculated based on the change between 2 frames
        /// <para>Only use when touched</para>
        /// </summary>
        public static Vector2 TouchMoveDeltaRelative
        {
            get
            {
                UpdateInputIfNot();
                return touchMoveDeltaRelative;
            }
        }

        #endregion
    }
}