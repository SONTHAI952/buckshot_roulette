using UnityEngine;

namespace ZeroX.SafeAreaSystem
{
    public class ScreenPixelConverter
    {
        private Canvas canvas;
        private RectTransform rectTransform;


        public Vector2 RelativeScreenSize => rectTransform.sizeDelta;
        public Vector2Int RawScreenSize => GetRawScreenSize();
        
        
        public void Init(SafeArea safeArea)
        {
            canvas = GetRootCanvas(safeArea);
            rectTransform = (RectTransform)canvas.transform;
        }

        private Canvas GetRootCanvas(SafeArea safeArea)
        {
            Canvas canvas = safeArea.transform.GetComponentInParent<Canvas>();
            while (canvas.transform.parent != null)
            {
                Canvas parentCanvas = canvas.transform.parent.GetComponentInParent<Canvas>();
                if (parentCanvas == null)
                    break;
                else
                    canvas = parentCanvas;
            }

            return canvas;
        }


        private Vector2Int GetRawScreenSize()
        {
            #if UNITY_EDITOR
            return GetGameViewResolution();
            #else
            return new Vector2Int(Screen.width, Screen.height);
            #endif
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
        
        

        public float ConvertX_RelativeToRaw(float x)
        {
            float delta = x / rectTransform.sizeDelta.x;
            return delta * RawScreenSize.x;
        }
        
        public float ConvertY_RelativeToRaw(float y)
        {
            float delta = y / rectTransform.sizeDelta.y;
            return delta * RawScreenSize.y;
        }
        
        public float ConvertX_RawToRelative(float x)
        {
            float delta = x / RawScreenSize.x;
            return delta * rectTransform.sizeDelta.x;
        }
        
        public float ConvertY_RawToRelative(float y)
        {
            float delta = y / RawScreenSize.y;
            return delta * rectTransform.sizeDelta.y;
        }

        public Rect ConvertRect_RelativeToRaw(Rect rect)
        {
            Vector2 relativeScreenSize = rectTransform.sizeDelta;
            Vector2Int rawScreenSize = GetRawScreenSize();
            
            //X
            float delta = rect.x / relativeScreenSize.x;
            rect.x = delta * rawScreenSize.x;
            
            
            //Y
            delta = rect.y / relativeScreenSize.y;
            rect.y = delta * rawScreenSize.y;
            
            
            //Width
            delta = rect.width / relativeScreenSize.x;
            rect.width = delta * rawScreenSize.x;
            
            
            //Height
            delta = rect.height / relativeScreenSize.y;
            rect.height = delta * rawScreenSize.y;
            
            return rect;
        }
        
        public Rect ConvertRect_RawToRelative(Rect rect)
        {
            Vector2 relativeScreenSize = rectTransform.sizeDelta;
            Vector2Int rawScreenSize = GetRawScreenSize();
            
            //X
            float delta = rect.x / rawScreenSize.x;
            rect.x = delta * relativeScreenSize.x;
            
            //Y
            delta = rect.y / rawScreenSize.y;
            rect.y = delta * relativeScreenSize.y;
            
            //Width
            delta = rect.width / rawScreenSize.y;
            rect.width = delta * relativeScreenSize.y;
            
            //Height
            delta = rect.height / rawScreenSize.y;
            rect.height = delta * relativeScreenSize.y;

            return rect;
        }
    }
}