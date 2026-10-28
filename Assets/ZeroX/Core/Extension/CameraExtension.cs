using UnityEngine;

namespace ZeroX.Extensions
{
    public static class CameraExtension
    {
        public static Vector2 GetScreenSizeInWorldCoords(Camera gameCamera, float distance = 10f)
        {
            float width = 0f;
            float height = 0f;

            if (gameCamera.orthographic)
            {
                if (gameCamera.orthographicSize <= .001f)
                    return Vector2.zero;

                var p1 = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, gameCamera.nearClipPlane));
                var p2 = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, gameCamera.nearClipPlane));
                var p3 = gameCamera.ViewportToWorldPoint(new Vector3(1, 1, gameCamera.nearClipPlane));

                width = (p2 - p1).magnitude;
                height = (p3 - p2).magnitude;
            }
            else
            {
                height = 2.0f * Mathf.Abs(distance) * Mathf.Tan(gameCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                width = height * gameCamera.aspect;
            }

            return new Vector2(width, height);
        }

        public static float GetWidthCameraInWorldCoords(this Camera camera, float height)
        {
            return height * camera.aspect;
        }

        public static float GetHeightCameraInWorldCoords(this Camera camera, float width)
        {
            return width / camera.aspect;
        }

        /// <summary>
        /// Tính theo tỉ lệ camera, height là chiều cao camera của một distance nào đó
        /// </summary>
        public static Vector2 GetLeftLimitPoint(this Camera camera, float height)
        {
            float width = height * camera.aspect;
            Vector2 pos = camera.transform.position;
            pos.x -= width / 2;
            return pos;
        }

        /// <summary>
        /// Tính theo tỉ lệ camera, height là chiều cao camera của một distance nào đó
        /// </summary>
        public static Vector2 GetRightLimitPoint(this Camera camera, float height)
        {
            float width = height * camera.aspect;
            Vector2 pos = camera.transform.position;
            pos.x += width / 2;
            return pos;
        }

        public static float CalculateDeptOfPos(this Camera camera, Vector3 pos)
        {
            Vector3 projectPos = Vector3.ProjectOnPlane(pos, camera.transform.forward);
            float distance = Vector3.Distance(projectPos, pos);
            if (Vector3.Angle(camera.transform.forward, pos - projectPos) < 90)
                return distance;
            else
                return -distance;
        }

        /// <summary>
        /// Hàm này sử dụng plane và inPoint để tìm depth thay cho hàm tính thông thường. 
        /// </summary>
        public static Vector3 ScreenToWorldPoint(this Camera camera, Vector3 screenPoint, Vector3 planeNormal,
            Vector3 inPoint)
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

        // public static float CalculateDistanceFromScreenPointToPlane(this Camera camera, Vector3 screenPoint, Vector3 planeNormal, Vector3 inPoint)
        // {
        //     Plane plane = new Plane(Vector3.up, inPoint);
        //
        //     Ray ray = camera.ScreenPointToRay(screenPoint);
        //     if (plane.Raycast(ray, out float distance))
        //     {
        //         return distance;
        //     }
        //         
        //     Debug.LogError("Not raycast to ray");
        //     return float.MaxValue;
        // }

        public static bool IsPointInCameraView(this Camera camera, Vector3 worldPoint)
        {
            var screenPoint = camera.WorldToScreenPoint(worldPoint);

            float minX = 0;
            float maxX = Screen.width;
            float maxY = Screen.height;
            float minY = 0;

            if (screenPoint.x < minX && Mathf.Approximately(screenPoint.x, minX) == false)
                return false;

            if (screenPoint.x > maxX && Mathf.Approximately(screenPoint.x, maxX) == false)
                return false;

            if (screenPoint.y < minY && Mathf.Approximately(screenPoint.y, minY) == false)
                return false;

            if (screenPoint.y > maxY && Mathf.Approximately(screenPoint.y, maxY) == false)
                return false;

            return true;
        }
    }
}