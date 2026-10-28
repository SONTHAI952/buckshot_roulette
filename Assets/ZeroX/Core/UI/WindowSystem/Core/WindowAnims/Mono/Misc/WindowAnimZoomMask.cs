using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using ZeroX.WindowSystem;

namespace ZeroXWindowSystem
{
    public class WindowAnimZoomMask : WindowAnimMono
    {
        [SerializeField] private RectTransform scaleTarget;
        [SerializeField] private float minScale = 0;
        [SerializeField] private float maxScale = 20;
        
        [Header("Point")]
        [SerializeField] private RectTransform topPoint;
        [SerializeField] private RectTransform bottomPoint;
        [SerializeField] private RectTransform leftPoint;
        [SerializeField] private RectTransform rightPoint;
        
        [Header("Border")]
        [SerializeField] private RectTransform topBorder;
        [SerializeField] private RectTransform bottomBorder;
        [SerializeField] private RectTransform leftBorder;
        [SerializeField] private RectTransform rightBorder;


        private float CurrentScale => scaleTarget.localScale.x;
        private Tween currentTween;
        
        
        

        
        public override void Open(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            currentTween?.Kill();
            
            currentTween = scaleTarget.DOScale(minScale, duration)
                .SetEase(Ease.OutQuint)
                .SetUpdate(ignoreTimeScale)
                .OnUpdate(() => UpdateBorders())
                .OnComplete(() =>
                {
                    currentTween = null;
                    onFinished?.Invoke();
                });
        }

        public override void OpenImmediate(Window window, WindowParameters parameters)
        {
            scaleTarget.localScale = Vector3.one * minScale;
        }

        public override void Close(Window window, float duration, bool ignoreTimeScale, WindowParameters parameters, Action onFinished)
        {
            currentTween?.Kill();
            
            currentTween = scaleTarget.DOScale(maxScale, duration)
                .SetEase(Ease.InQuint)
                .SetUpdate(ignoreTimeScale)
                .OnUpdate(() => UpdateBorders())
                .OnComplete(() =>
                {
                    currentTween = null;
                    onFinished?.Invoke();
                });
        }

        public override void CloseImmediate(Window window, WindowParameters parameters)
        {
            scaleTarget.localScale = Vector3.one * maxScale;
        }


        void UpdateBorders()
        {
            UpdateVerticalBorder(topBorder, topPoint);
            UpdateVerticalBorder(bottomBorder, bottomPoint);
            
            UpdateHorizontalBorder(leftBorder, leftPoint);
            UpdateHorizontalBorder(rightBorder, rightPoint);
        }
        
        
        void UpdateVerticalBorder(RectTransform border, RectTransform point)
        {
            //Vector2 sizeDelta = border.sizeDelta;
            //sizeDelta.x = point.rect.width * CurrentScale;
            //border.sizeDelta = sizeDelta;

            border.position = point.position;
        }
        
        void UpdateHorizontalBorder(RectTransform border, RectTransform point)
        {
            Vector2 sizeDelta = border.sizeDelta;
            sizeDelta.y = point.rect.height * CurrentScale;
            border.sizeDelta = sizeDelta;
            
            border.position = point.position;
        }
    }
}