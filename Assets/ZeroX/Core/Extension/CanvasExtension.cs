using UnityEngine;

namespace ZeroX.Extensions
{
    public static class CanvasExtension
    {
        public static Camera GetRenderCamera(this Canvas canvas)
        {
            if (canvas.rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return canvas.rootCanvas.worldCamera;
        }
    }
}