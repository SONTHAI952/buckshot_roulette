using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.ProgressSystem
{
    public class ProgressTargetChangeAnchor : ProgressTarget
    {
        public TargetProgress targetProgress = TargetProgress.AnimateProgress;
        public Direction direction = Direction.LeftToRight;
        public RectTransform fillRectTransform;
        
        public enum Direction
        {
            LeftToRight, RightToLeft, BottomToTop, TopToBottom
        }
        
        public override void UpdateTarget(Progressor progressor)
        {
            float progress = ProgressUtility.GetTargetProgressValue(progressor, targetProgress);

            CalculateAnchoredMinMax(progress, out var anchoredMin, out var anchoredMax);
            fillRectTransform.anchorMin = anchoredMin;
            fillRectTransform.anchorMax = anchoredMax;
            fillRectTransform.sizeDelta = Vector2.zero;
            fillRectTransform.anchoredPosition = Vector2.zero;
        }

        void CalculateAnchoredMinMax(float progress, out Vector2 anchoredMin, out Vector2 anchoredMax)
        {
            if (direction == Direction.LeftToRight)
            {
                anchoredMin = Vector2.zero;
                anchoredMax = new Vector2(progress, 1);
            }
            else if (direction == Direction.RightToLeft)
            {
                anchoredMin = new Vector2(1 - progress, 0);
                anchoredMax = Vector2.one;
            }
            else if (direction == Direction.BottomToTop)
            {
                anchoredMin = new Vector2(0, 0);
                anchoredMax = new Vector2(1, progress);
            }
            else if (direction == Direction.TopToBottom)
            { 
                anchoredMin = new Vector2(0, 1 - progress);
                anchoredMax = Vector2.one;
            }
            else
            { 
                Debug.LogError("Not code for direction: " + direction);
                anchoredMin = Vector2.zero;
                anchoredMax = Vector2.zero;
            }
        }
    }
}