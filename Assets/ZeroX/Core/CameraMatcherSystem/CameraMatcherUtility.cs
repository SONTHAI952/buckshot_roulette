using UnityEngine;

namespace ZeroX.CameraMatcherSystem
{
    public static class CameraMatcherUtility
    {
        public static Camera GetRenderCameraOfRectTransform(RectTransform rectTransform)
        {
            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            if (canvas.rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return canvas.rootCanvas.worldCamera;
        }
        
        /// <summary>
        /// Hàm này sử dụng plane và inPoint để tìm depth thay cho hàm tính thông thường. 
        /// </summary>
        public static Vector3 ScreenToWorldPoint(Camera camera, Vector3 screenPoint, Vector3 planeNormal, Vector3 inPoint)
        {
            Plane plane = new Plane(planeNormal, inPoint);

            Ray ray = camera.ScreenPointToRay(screenPoint);
            if (plane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }
            
            Debug.LogError("Not raycast to ray");
            return Vector3.zero;
        }
    }
}