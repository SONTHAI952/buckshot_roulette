using UnityEngine;

namespace ZeroX.Extensions
{
    public static class RectExtension
    {
        /// <summary>
        /// Thu ngắn rect về phía bên phải. Height giữ nguyên
        /// </summary>
        public static Rect ShiftRight(this Rect rect, Rect justDrawRect, float spacing = 3)
        {
            float oldX = rect.x;
            rect.x = justDrawRect.xMax + spacing;
            rect.width -= (rect.x - oldX);

            return rect;
        }
        
        public static Rect ShiftRight(this Rect rect, float distance)
        {
            rect.x += distance;
            rect.width -= distance;

            return rect;
        }
        
        //Thu ngắn rect về phía dưới. Width giữ nguyên
        public static Rect ShiftDown(this Rect rect, Rect justDrawRect, float spacing = 2)
        {
            float oldY = rect.y;
            rect.y = justDrawRect.yMax + spacing;
            rect.height -= (rect.y - oldY);

            return rect;
        }
        
        public static Rect ShiftDown(this Rect rect, float distance)
        {
            rect.y += distance;
            rect.height -= distance;

            return rect;
        }
        
        public static Rect Clone_ChangeWidth(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }
        
        public static Rect Clone_ChangeHeight(this Rect rect, float height)
        {
            rect.height = height;
            return rect;
        }
    }
}