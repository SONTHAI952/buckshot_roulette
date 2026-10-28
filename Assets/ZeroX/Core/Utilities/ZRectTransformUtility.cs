using System.Collections.Generic;
using UnityEngine;

namespace ZeroX
{
    public static class ZRectTransformUtility
    {
        private static Camera GetRenderCameraOfCanvas(Canvas canvas)
        {
            if (canvas.rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return canvas.rootCanvas.worldCamera;
        }
        
        private static Camera GetRenderCameraOfRectTransform(RectTransform rectTransform)
        {
            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            
            if (canvas.rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return canvas.rootCanvas.worldCamera;
        }
        
        private static Vector2 WorldToPlane(Plane plane, Vector3 worldPoint)
        {
            Vector3 n = plane.normal;
            Vector3 origin = -n * plane.distance;

            Vector3 tangent, bitangent;
            if (Mathf.Abs(Vector3.Dot(n, Vector3.up)) < 0.99f)
            {
                // Plane đứng hoặc nghiêng: x nằm ngang, y hướng "lên" dọc mặt phẳng
                tangent = Vector3.Cross(Vector3.up, n).normalized;
                bitangent = Vector3.Cross(n, tangent);
            }
            else
            {
                // Plane nằm ngang: x = world X, y = world Z (nhìn từ trên xuống)
                tangent = Vector3.Cross(n, Vector3.forward).normalized;
                bitangent = Vector3.Cross(tangent, n);
            }

            Vector3 local = plane.ClosestPointOnPlane(worldPoint) - origin;
            return new Vector2(Vector3.Dot(local, tangent), Vector3.Dot(local, bitangent));
        }

        

        #region Screen To

        public static Vector2 ScreenToLocalPointInRectangle(Vector2 fromScreenPos, RectTransform toInRect, Camera toCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInRect, fromScreenPos, toCamera, out Vector2 localPoint);
            return localPoint;
        }
        
        public static Vector2 ScreenToLocalPointInRectangle(Vector2 fromScreenPos, RectTransform toInRect)
        {
            Canvas toCanvas = toInRect.GetComponentInParent<Canvas>();
            Camera toCamera = GetRenderCameraOfCanvas(toCanvas);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInRect, fromScreenPos, toCamera, out Vector2 localPoint);
            return localPoint;
        }

        #endregion


        
        #region Screen Rect To World Rect

        public static void ScreenRectToWorldRectOnPlane(RectTransform rectTransform, Camera worldCamera, Vector3 planeNormal, Vector3 planeInPoint, out Vector3 center, out Vector2 size)
        {
            Camera uiCamera = GetRenderCameraOfRectTransform(rectTransform);
            var rect = rectTransform.rect;
            
            Vector3 uiWorldAnchorCenter = rectTransform.position;
            Vector3 uiWorldAnchorLeft = rectTransform.TransformPoint(new Vector2(-rect.width / 2f, -rect.height / 2f)); //Vị trí bottom left
            Vector3 uiWorldAnchorRight = rectTransform.TransformPoint(new Vector2(rect.width / 2f, rect.height / 2f)); //Vị trí bottom right
            
            Vector3 screenAnchorCenter = uiCamera == null ? uiWorldAnchorCenter : uiCamera.WorldToScreenPoint(uiWorldAnchorCenter);
            Vector3 screenAnchorLeft = uiCamera == null ? uiWorldAnchorLeft : uiCamera.WorldToScreenPoint(uiWorldAnchorLeft);
            Vector3 screenAnchorRight = uiCamera == null ? uiWorldAnchorRight : uiCamera.WorldToScreenPoint(uiWorldAnchorRight);
            
            
            //World Center
            Plane plane = new Plane(planeNormal, planeInPoint);
            var ray = worldCamera.ScreenPointToRay(screenAnchorCenter);
            plane.Raycast(ray, out float enter);
            Vector3 worldAnchorCenter = ray.GetPoint(enter);
            
            
            //World Left
            ray = worldCamera.ScreenPointToRay(screenAnchorLeft);
            plane.Raycast(ray, out enter);
            Vector3 worldAnchorLeft = ray.GetPoint(enter);
            Vector2 inPlaneAnchorLeft = WorldToPlane(plane, worldAnchorLeft);
            
            
            //World Right
            ray = worldCamera.ScreenPointToRay(screenAnchorRight);
            plane.Raycast(ray, out enter);
            Vector3 worldAnchorRight = ray.GetPoint(enter);
            Vector2 inPlaneAnchorRight = WorldToPlane(plane, worldAnchorRight);
            
            
            //Center
            center = worldAnchorCenter;
            
            //Size
            size = new Vector2();
            size.x = Mathf.Abs(inPlaneAnchorRight.x - inPlaneAnchorLeft.x);
            size.y = Mathf.Abs(inPlaneAnchorRight.y - inPlaneAnchorLeft.y);
        }
        #endregion
        
        

        #region World To Local
        
        public static Vector2 WorldToLocalPointInRectangle(Vector3 fromWorldPos, Camera fromCamera, RectTransform toInRect, Camera toCamera)
        {
            var screenPos = fromCamera.WorldToScreenPoint(fromWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInRect, screenPos, toCamera, out Vector2 localPoint);
            return localPoint;
        }

        public static Vector2 WorldToLocalPointInRectangle(Vector3 fromWorldPos, Camera fromCamera, RectTransform toInRect)
        {
            Canvas toCanvas = toInRect.GetComponentInParent<Canvas>();
            Camera toCamera = GetRenderCameraOfCanvas(toCanvas);
            
            var screenPos = fromCamera.WorldToScreenPoint(fromWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInRect, screenPos, toCamera, out Vector2 localPoint);
            return localPoint;
        }
        
        public static Vector2 PointToLocalPointInRectangle(Transform fromPoint, RectTransform toInRect)
        {
            Canvas fromCanvas = fromPoint.GetComponentInParent<Canvas>();
            Camera fromCamera = GetRenderCameraOfCanvas(fromCanvas);
            
            Canvas toCanvas = toInRect.GetComponentInParent<Canvas>();
            Camera toCamera = GetRenderCameraOfCanvas(toCanvas);
            
            var screenPos = fromCamera.WorldToScreenPoint(fromPoint.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInRect, screenPos, toCamera, out Vector2 localPoint);
            return localPoint;
        }
        
        #endregion

        

        #region World To World

        public static Vector3 WorldToWorldPointInRectangle(Vector3 fromWorldPos, Camera fromCamera, RectTransform toInRect, Camera toCamera)
        {
            var screenPos = fromCamera.WorldToScreenPoint(fromWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInRect, screenPos, toCamera, out Vector2 localPoint);
            return toInRect.TransformPoint(localPoint);
        }
        
        public static Vector3 WorldToWorldPointInRectangle(Vector3 fromWorldPos, Camera fromCamera, RectTransform toInRect)
        {
            Canvas toCanvas = toInRect.GetComponentInParent<Canvas>();
            Camera toCamera = GetRenderCameraOfCanvas(toCanvas);
            
            var screenPos = fromCamera.WorldToScreenPoint(fromWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInRect, screenPos, toCamera, out Vector2 localPoint);
            return toInRect.TransformPoint(localPoint);
        }
        
        public static Vector3 PointToWorldPointInRectangle(Transform fromPoint, RectTransform toInRect)
        {
            Canvas fromCanvas = fromPoint.GetComponentInParent<Canvas>();
            Camera fromCamera = GetRenderCameraOfCanvas(fromCanvas);
            
            Canvas toCanvas = toInRect.GetComponentInParent<Canvas>();
            Camera toCamera = GetRenderCameraOfCanvas(toCanvas);
            
            var screenPos = fromCamera.WorldToScreenPoint(fromPoint.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInRect, screenPos, toCamera, out Vector2 localPoint);
            return toInRect.TransformPoint(localPoint);
        }

        #endregion



        #region Size

        private static void GetRectWorldConers(RectTransform rectTransform, out Vector3 bottomLeft, out Vector3 bottomRight, out Vector3 topLeft, out Vector3 topRight)
        {
            Rect rect = rectTransform.rect;
            
            //Local
            bottomLeft = new Vector3(rect.xMin, rect.yMin);
            bottomRight = new Vector3(rect.xMax, rect.yMin);
            
            topLeft = new Vector3(rect.xMin, rect.yMax);
            topRight = new Vector3(rect.xMax, rect.yMax);
            
            
            //Local To World
            bottomLeft = rectTransform.TransformPoint(bottomLeft);
            bottomRight = rectTransform.TransformPoint(bottomRight);
            topLeft = rectTransform.TransformPoint(topLeft);
            topRight = rectTransform.TransformPoint(topRight);
        }

        public static Vector2 RectSizeToLocalSizeInRectangle(RectTransform fromRect, RectTransform toInRect)
        {
            Canvas fromCanvas = fromRect.GetComponentInParent<Canvas>();
            Camera fromCamera = GetRenderCameraOfCanvas(fromCanvas);
            
            Canvas toCanvas = toInRect.GetComponentInParent<Canvas>();
            Camera toCamera = GetRenderCameraOfCanvas(toCanvas);

            GetRectWorldConers(fromRect, out Vector3 bottomLeft, out Vector3 bottomRight, out Vector3 topLeft, out Vector3 topRight);
            
            bottomLeft = WorldToLocalPointInRectangle(bottomLeft, fromCamera, toInRect, toCamera);
            topRight = WorldToLocalPointInRectangle(topRight, fromCamera, toInRect, toCamera);

            Vector2 size = new Vector2();
            size.x = topRight.x - bottomLeft.x;
            size.y = topRight.y - bottomLeft.y;
            return size;
        }

        public static Vector2 WorldSizeXZToLocalSizeInRectangle(Vector3 fromWorldPos, Vector3 fromWorldSize, Camera fromCamera, RectTransform toInRect)
        {
            Canvas toCanvas = toInRect.GetComponentInParent<Canvas>();
            Camera toCamera = GetRenderCameraOfCanvas(toCanvas);
            
            
            var listConerWorldOfBox = CreateListConerOfBox(fromWorldPos, fromWorldSize);

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            
            float minY = float.MaxValue;
            float maxY = float.MinValue;


            for (int i = 0; i < listConerWorldOfBox.Count; i++)
            {
                var cornerWorld = listConerWorldOfBox[i];
                var cornerLocal = WorldToLocalPointInRectangle(cornerWorld, fromCamera, toInRect, toCamera);
                
                if(cornerLocal.x < minX)
                    minX = cornerLocal.x;
                
                if(cornerLocal.x > maxX)
                    maxX = cornerLocal.x;
                
                if(cornerLocal.y < minY)
                    minY = cornerLocal.y;
                
                if(cornerLocal.y > maxY)
                    maxY = cornerLocal.y;
            }
            
            Vector2 size = new Vector2();
            size.x = maxX - minX;
            size.y = maxY - minY;
            return size;
        }

        private static List<Vector3> CreateListConerOfBox(Vector3 center, Vector3 size)
        {
            List<Vector3> listCorner = new List<Vector3>();

            Vector3 halfSize = size / 2;
            
            listCorner.Add(new Vector3(center.x - halfSize.x, center.y - halfSize.y, center.z - halfSize.z));
            listCorner.Add(new Vector3(center.x - halfSize.x, center.y - halfSize.y, center.z + halfSize.z));
            
            listCorner.Add(new Vector3(center.x + halfSize.x, center.y - halfSize.y, center.z - halfSize.z));
            listCorner.Add(new Vector3(center.x + halfSize.x, center.y - halfSize.y, center.z + halfSize.z));
            
            listCorner.Add(new Vector3(center.x - halfSize.x, center.y + halfSize.y, center.z - halfSize.z));
            listCorner.Add(new Vector3(center.x - halfSize.x, center.y + halfSize.y, center.z + halfSize.z));
            
            listCorner.Add(new Vector3(center.x + halfSize.x, center.y + halfSize.y, center.z - halfSize.z));
            listCorner.Add(new Vector3(center.x + halfSize.x, center.y + halfSize.y, center.z + halfSize.z));

            return listCorner;
        }

        #endregion
    }
}