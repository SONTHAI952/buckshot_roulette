using System;
using UnityEngine;

namespace ZeroX.EmissionAndFlyEffectSystem
{
    public class EmissionAndFlyEffectData
    {
        public Camera startCamera;
        public Vector3 startWorldPos;
        
        public Camera endCamera;
        public Vector3 endWorldPos;
        
        /// <summary>
        /// Tổng giá trị của cell, không phải số lượng cell
        /// </summary>
        public long totalCellValue;
        public int? maxCell;
        
        public Vector2? cellSize;
        public float? startCellScale;
        public float? endCellScale;
        
        public Sprite icon;
        
        
        public Action<long> onCellFinished = null;
        public Action onFinished = null;

        
        
        
        
        
        private static Camera GetCanvasCamera(GameObject gObj)
        {
            Canvas canvas = gObj.GetComponentInParent<Canvas>().rootCanvas;
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return canvas.worldCamera;
        }
        
        
        public static EmissionAndFlyEffectData Create(RectTransform startPoint, RectTransform endPoint, long totalCellValue, Sprite icon)
        {
            EmissionAndFlyEffectData data = new EmissionAndFlyEffectData();
            
            data.startCamera = GetCanvasCamera(startPoint.gameObject);
            data.startWorldPos = startPoint.position;
            
            data.endCamera = GetCanvasCamera(endPoint.gameObject);
            data.endWorldPos = endPoint.position;
            
            data.totalCellValue = totalCellValue;
            
            data.icon = icon;

            return data;
        }
        
        public static EmissionAndFlyEffectData Create(Camera startCamera, Vector3 startWorldPos, RectTransform endPoint, long totalCellValue, Sprite icon)
        {
            EmissionAndFlyEffectData data = new EmissionAndFlyEffectData();
            
            data.startCamera = startCamera;
            data.startWorldPos = startWorldPos;
            
            data.endCamera = GetCanvasCamera(endPoint.gameObject);
            data.endWorldPos = endPoint.position;
            
            data.totalCellValue = totalCellValue;
            
            data.icon = icon;

            return data;
        }
    }
}