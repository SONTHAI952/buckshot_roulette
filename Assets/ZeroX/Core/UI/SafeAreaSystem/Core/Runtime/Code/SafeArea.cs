using UnityEngine;

namespace ZeroX.SafeAreaSystem
{
    public class SafeArea : MonoBehaviour
    {
        public SafeAreaConfig_Specific safeAreaConfig = new SafeAreaConfig_Specific();

        [Header("")]
        public BannerAreaConfig_Specific bannerAreaConfig = new BannerAreaConfig_Specific();
        
        [Header("")]
        public MinAreaConfig_Specific minAreaConfig = new MinAreaConfig_Specific();
        

        //Temp
        private RectTransform panel;
        private ScreenPixelConverter screenPixelConverter = new ScreenPixelConverter();
        private Vector2Int rawScreenSize = new Vector2Int (0, 0);
        private Vector2 relativeScreenSize = new Vector2Int (0, 0);
        private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;
        private Rect lastRelativeSafeArea = new Rect (0, 0, 0, 0);

        

        #region Unity Method

        private void Awake ()
        {
            panel = GetComponent<RectTransform>();

            if (panel == null)
            {
                Debug.LogError ("Cannot apply safe area - no RectTransform found on " + name);
                Destroy (gameObject);
            }
            
            screenPixelConverter.Init(this);
            
            if(enabled)
                Refresh();
        }

        private void Update ()
        {
            Refresh();
        }

        #endregion
        

        
        private void Refresh()
        {
            bool screenSizeChanged = false;
            if (screenPixelConverter.RawScreenSize != rawScreenSize || screenPixelConverter.RelativeScreenSize != relativeScreenSize)
            {
                screenSizeChanged = true;
                rawScreenSize = screenPixelConverter.RawScreenSize;
                relativeScreenSize = screenPixelConverter.RelativeScreenSize;
            }
            
            
            
            Rect relativeSafeArea = CalculateSafeArea();

            if (screenSizeChanged ||
                relativeSafeArea != lastRelativeSafeArea ||
                Screen.orientation != lastOrientation)
            {
                lastOrientation = Screen.orientation;
                ApplySafeArea(relativeSafeArea);
            }
        }

        private void ApplySafeArea(Rect relativeSafeArea)
        {
            lastRelativeSafeArea = relativeSafeArea;

            var rawSafeArea = screenPixelConverter.ConvertRect_RelativeToRaw(relativeSafeArea);


            // Check for invalid screen startup state on some Samsung devices (see below)
            if (rawScreenSize.x > 0 && rawScreenSize.y > 0)
            {
                // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
                Vector2 anchorMin = rawSafeArea.position;
                Vector2 anchorMax = rawSafeArea.position + rawSafeArea.size;
                anchorMin.x /= rawScreenSize.x;
                anchorMin.y /= rawScreenSize.y;
                anchorMax.x /= rawScreenSize.x;
                anchorMax.y /= rawScreenSize.y;

                // Fix for some Samsung devices (e.g. Note 10+, A71, S20) where Refresh gets called twice and the first time returns NaN anchor coordinates
                // See https://forum.unity.com/threads/569236/page-2#post-6199352
                if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
                {
                    panel.anchorMin = anchorMin;
                    panel.anchorMax = anchorMax;
                }
            }
        }
        


        #region Calculate Safe Area

        /// <summary>
        /// Safe area rect được trả về ở đây là relativeSafeArea 
        /// </summary>
        private Rect CalculateSafeArea()
        {
            Rect safeArea = CalculateSafeArea_SafeArea();
            Rect originalSafeArea = safeArea;
            safeArea = CalculateSafeArea_BannerArea(safeArea);
            safeArea = CalculateSafeArea_MinArea(safeArea, originalSafeArea);

            return safeArea;
        }

        private Rect CalculateSafeArea_SafeArea()
        {
            Rect rect;
            if (safeAreaConfig.Enable == false)
            {
                rect = new Rect(0, 0, rawScreenSize.x, rawScreenSize.y);
            }
            else
            {
                rect = Screen.safeArea; //Lúc này là rawSafeAreaRect

                SafeAreaConfig config = safeAreaConfig.Config;
                if (config.bottom == false)
                {
                    rect.height += rect.y;
                    rect.y = 0;
                }

                if (config.top == false)
                {
                    rect.height += (rawScreenSize.y - rect.yMax);
                }

                if (config.left == false)
                {
                    rect.width += rect.x;
                    rect.x = 0;
                }
            
                if(config.right == false)
                {
                    rect.width += (rawScreenSize.x - rect.xMax);
                }
            }

            
            //Biến đổi về relativeSafeArea
            return screenPixelConverter.ConvertRect_RawToRelative(rect);
        }

        private Rect CalculateSafeArea_BannerArea(Rect rect)
        {
            if (bannerAreaConfig.Enable == false)
                return rect;

            BannerAreaConfig config = bannerAreaConfig.Config;


            if (config.left.enable)
            {
                rect.x += config.left.value;
                rect.width -= config.left.value;
            }
            
            if (config.right.enable)
            {
                rect.width -= config.right.value;
            }
            
            if (config.top.enable)
            {
                rect.height -= config.top.value;
            }
            
            if (config.bottom.enable)
            {
                rect.y += config.bottom.value;
                rect.height -= config.bottom.value;
            }

            return rect;
        }

        private Rect CalculateSafeArea_MinArea(Rect rect, Rect originalSafeArea)
        {
            if (minAreaConfig.Enable == false)
                return rect;

            MinAreaConfig config = minAreaConfig.Config;


            if (config.left.enable)
            {
                float minPaddingLeft = config.left.value;
                if (config.left.includeOriginalSafePadding == false)
                    minPaddingLeft += originalSafeArea.x;
                
                
                if (rect.x < minPaddingLeft)
                {
                    rect.width -= minPaddingLeft - rect.x;
                    rect.x = minPaddingLeft;
                }
            }

            if (config.right.enable)
            {
                float minPaddingRight = config.right.value;
                if (config.right.includeOriginalSafePadding == false)
                    minPaddingRight += relativeScreenSize.x - originalSafeArea.xMax;
                
                
                float paddingRight = relativeScreenSize.x - rect.xMax;
                if (paddingRight < minPaddingRight)
                {
                    rect.width -= minPaddingRight - paddingRight;
                }
            }

            if (config.top.enable)
            {
                float minPaddingTop = config.top.value;
                if (config.top.includeOriginalSafePadding == false)
                    minPaddingTop += relativeScreenSize.y - originalSafeArea.yMax;
                
                
                float paddingTop = relativeScreenSize.y - rect.yMax;
                if (paddingTop < minPaddingTop)
                {
                    rect.height -= minPaddingTop - paddingTop;
                }
            }

            if (config.bottom.enable)
            {
                float minPaddingBottom = config.bottom.value;
                if (config.bottom.includeOriginalSafePadding == false)
                    minPaddingBottom += originalSafeArea.y;
                
                
                if (rect.y < minPaddingBottom)
                {
                    rect.height -= minPaddingBottom - rect.y;
                    rect.y = minPaddingBottom;
                }
            }

            return rect;
        }

        #endregion
        
        
    }
}
