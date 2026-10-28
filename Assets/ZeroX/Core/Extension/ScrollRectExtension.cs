using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.Extensions
{
    public static class ScrollRectExtension
    {
        public static Vector3 Transform_LocalPointInAnySpaceInsideContent_To_LocalPointInContentSpace(Transform content,
            Transform spaceOfLocalPos, Vector3 localPos)
        {
            if (content == spaceOfLocalPos)
                return localPos;

            while (true)
            {
                Vector3 spaceOfLocalPos_LocalScale = spaceOfLocalPos.localScale;
                localPos.x *= spaceOfLocalPos_LocalScale.x;
                localPos.y *= spaceOfLocalPos_LocalScale.y;
                localPos.z *= spaceOfLocalPos_LocalScale.z;

                localPos = spaceOfLocalPos.localPosition +
                           spaceOfLocalPos.localRotation *
                           localPos; //LocalPos giờ là localPos của parentOfLocalPos.parent
                spaceOfLocalPos = spaceOfLocalPos.parent;

                if (spaceOfLocalPos == null)
                    throw new Exception("ParentOfLocalPos is not inside content");

                if (spaceOfLocalPos == content)
                    break;
            }

            return localPos;
        }

        public static Vector3 Transform_TargetInAnySpaceInsideContent_To_LocalPointInContentSpace(Transform content,
            Transform target)
        {
            Transform spaceOfLocalPos = target.parent;
            Vector3 localPos = target.localPosition;



            if (content == spaceOfLocalPos)
                return localPos;

            while (true)
            {
                Vector3 spaceOfLocalPos_LocalScale = spaceOfLocalPos.localScale;
                localPos.x *= spaceOfLocalPos_LocalScale.x;
                localPos.y *= spaceOfLocalPos_LocalScale.y;
                localPos.z *= spaceOfLocalPos_LocalScale.z;

                localPos = spaceOfLocalPos.localPosition +
                           spaceOfLocalPos.localRotation *
                           localPos; //LocalPos giờ là localPos của parentOfLocalPos.parent
                spaceOfLocalPos = spaceOfLocalPos.parent;

                if (spaceOfLocalPos == null)
                    throw new Exception("Target is not inside content");

                if (spaceOfLocalPos == content)
                    break;
            }

            return localPos;
        }



        #region CalculateHorizontalNormalizedPos

        /// <summary>
        /// customPivot có giá trị từ 0 -> 1 tương ứng từ trái qua phải
        /// </summary>
        public static float CalculateHorizontalNormalizedPos(this ScrollRect scrollRect, Vector3 localPosInContentSpace,
            float customPivot)
        {
            RectTransform viewPort = scrollRect.viewport;
            Rect viewPortRect = viewPort.rect;

            RectTransform content = scrollRect.content;
            Rect contentRect = content.rect;

            if (contentRect.width * content.localScale.x <= viewPortRect.width)
                return 0;

            float posLeftX = -content.pivot.x * contentRect.width * content.localScale.x;
            float posRightX = (1 - content.pivot.x) * contentRect.width * content.localScale.x;

            //Fix để điểm cần tính pos sẽ nằm ở giữa viewPort
            float posLeftXFix = posLeftX + viewPortRect.width * customPivot;
            float posRightXFix = posRightX - viewPortRect.width * (1 - customPivot);

            float targetLocalPosX = localPosInContentSpace.x * content.localScale.x;

            return (targetLocalPosX - posLeftXFix) / (posRightXFix - posLeftXFix);
        }

        public static float CalculateHorizontalNormalizedPos(this ScrollRect scrollRect, Vector3 localPosInContentSpace)
        {
            return CalculateHorizontalNormalizedPos(scrollRect, localPosInContentSpace, 0.5f);
        }

        /// <summary>
        /// customPivot có giá trị từ 0 -> 1 tương ứng từ trái qua phải
        /// </summary>
        public static float CalculateHorizontalNormalizedPos(this ScrollRect scrollRect, Transform target,
            float customPivot)
        {
            Vector3 localPosInContentSpace =
                Transform_TargetInAnySpaceInsideContent_To_LocalPointInContentSpace(scrollRect.content, target);
            return CalculateHorizontalNormalizedPos(scrollRect, localPosInContentSpace, customPivot);
        }

        public static float CalculateHorizontalNormalizedPos(this ScrollRect scrollRect, Transform target)
        {
            Vector3 localPosInContentSpace =
                Transform_TargetInAnySpaceInsideContent_To_LocalPointInContentSpace(scrollRect.content, target);
            return CalculateHorizontalNormalizedPos(scrollRect, localPosInContentSpace, 0.5f);
        }

        public static float CalculateHorizontalNormalizedPos(this ScrollRect scrollRect, Transform target,
            Transform customPivot)
        {
            var viewLocalPos = scrollRect.viewport.InverseTransformPoint(customPivot.position);

            RectTransform viewPort = scrollRect.viewport;
            Rect viewPortRect = viewPort.rect;
            float posLeftX = -viewPort.pivot.x * viewPortRect.width;

            float anchorWithViewPort = (viewLocalPos.x - posLeftX) / (viewPortRect.width / viewPort.localScale.x);

            return CalculateHorizontalNormalizedPos(scrollRect, target, anchorWithViewPort);
        }

        #endregion





        #region CalculateVerticalNormalizedPos

        /// <summary>
        /// customPivot có giá trị từ 0 -> 1 tương ứng từ dưới lên trên
        /// </summary>
        public static float CalculateVerticalNormalizedPos(this ScrollRect scrollRect, Vector3 localPosInContentSpace,
            float customPivot)
        {
            RectTransform viewPort = scrollRect.viewport;
            Rect viewPortRect = viewPort.rect;

            RectTransform content = scrollRect.content;
            Rect contentRect = content.rect;

            if (contentRect.height * content.localScale.y <= viewPortRect.height)
                return 0;

            float posBottomY = -content.pivot.y * contentRect.height * content.localScale.y;
            float posTopY = (1 - content.pivot.y) * contentRect.height * content.localScale.y;

            //Fix để điểm cần tính pos sẽ nằm ở giữa viewPort
            float posBottomYFix = posBottomY + viewPortRect.height * customPivot;
            float posTopYFix = posTopY - viewPortRect.height * (1 - customPivot);

            float targetLocalPosY = localPosInContentSpace.y * content.localScale.y;

            return (targetLocalPosY - posBottomYFix) / (posTopYFix - posBottomYFix);
        }

        public static float CalculateVerticalNormalizedPos(this ScrollRect scrollRect, Vector3 localPosInContentSpace)
        {
            return CalculateVerticalNormalizedPos(scrollRect, localPosInContentSpace, 0.5f);
        }

        /// <summary>
        /// customPivot có giá trị từ 0 -> 1 tương ứng từ dưới lên trên
        /// </summary>
        public static float CalculateVerticalNormalizedPos(this ScrollRect scrollRect, Transform target,
            float customPivot)
        {
            Vector3 localPosInContentSpace =
                Transform_TargetInAnySpaceInsideContent_To_LocalPointInContentSpace(scrollRect.content, target);
            return CalculateVerticalNormalizedPos(scrollRect, localPosInContentSpace, customPivot);
        }

        public static float CalculateVerticalNormalizedPos(this ScrollRect scrollRect, Transform target)
        {
            Vector3 localPosInContentSpace =
                Transform_TargetInAnySpaceInsideContent_To_LocalPointInContentSpace(scrollRect.content, target);
            return CalculateVerticalNormalizedPos(scrollRect, localPosInContentSpace, 0.5f);
        }

        public static float CalculateVerticalNormalizedPos(this ScrollRect scrollRect, Transform target,
            Transform customPivot)
        {
            var viewLocalPos = scrollRect.viewport.InverseTransformPoint(customPivot.position);

            RectTransform viewPort = scrollRect.viewport;
            Rect viewPortRect = viewPort.rect;
            float posBottomY = -viewPort.pivot.y * viewPortRect.height * viewPort.localScale.y;

            float anchorWithViewPort = (viewLocalPos.y - posBottomY) / (viewPortRect.height / viewPort.localScale.x);

            return CalculateVerticalNormalizedPos(scrollRect, target, anchorWithViewPort);
        }

        #endregion





        #region ScrollHorizontalTo

        public static void ScrollHorizontalTo(this ScrollRect scrollRect, Vector3 localPosInContentSpace,
            float anchorWithViewPort)
        {
            float horizontalNormalizedPosition =
                CalculateHorizontalNormalizedPos(scrollRect, localPosInContentSpace, anchorWithViewPort);
            horizontalNormalizedPosition = Mathf.Clamp01(horizontalNormalizedPosition);

            scrollRect.horizontalNormalizedPosition = horizontalNormalizedPosition;
        }

        public static void ScrollHorizontalTo(this ScrollRect scrollRect, Vector3 localPosInContentSpace)
        {
            float horizontalNormalizedPosition =
                CalculateHorizontalNormalizedPos(scrollRect, localPosInContentSpace, 0.5f);
            horizontalNormalizedPosition = Mathf.Clamp01(horizontalNormalizedPosition);

            scrollRect.horizontalNormalizedPosition = horizontalNormalizedPosition;
        }





        public static void ScrollHorizontalTo(this ScrollRect scrollRect, Transform target, float anchorWithViewPort)
        {
            Vector3 localPosInContentSpace =
                Transform_TargetInAnySpaceInsideContent_To_LocalPointInContentSpace(scrollRect.content, target);

            float horizontalNormalizedPosition =
                CalculateHorizontalNormalizedPos(scrollRect, localPosInContentSpace, anchorWithViewPort);
            horizontalNormalizedPosition = Mathf.Clamp01(horizontalNormalizedPosition);

            scrollRect.horizontalNormalizedPosition = horizontalNormalizedPosition;
        }

        public static void ScrollHorizontalTo(this ScrollRect scrollRect, Transform target)
        {
            Vector3 localPosInContentSpace =
                Transform_TargetInAnySpaceInsideContent_To_LocalPointInContentSpace(scrollRect.content, target);

            float horizontalNormalizedPosition =
                CalculateHorizontalNormalizedPos(scrollRect, localPosInContentSpace, 0.5f);
            horizontalNormalizedPosition = Mathf.Clamp01(horizontalNormalizedPosition);

            scrollRect.horizontalNormalizedPosition = horizontalNormalizedPosition;
        }

        #endregion





        #region ScrollVerticalTo

        public static void ScrollVerticalTo(this ScrollRect scrollRect, Vector3 localPosInContentSpace,
            float anchorWithViewPort)
        {
            float verticalNormalizedPosition =
                CalculateVerticalNormalizedPos(scrollRect, localPosInContentSpace, anchorWithViewPort);
            verticalNormalizedPosition = Mathf.Clamp01(verticalNormalizedPosition);

            scrollRect.verticalNormalizedPosition = verticalNormalizedPosition;
        }

        public static void ScrollVerticalTo(this ScrollRect scrollRect, Vector3 localPosInContentSpace)
        {
            float verticalNormalizedPosition = CalculateVerticalNormalizedPos(scrollRect, localPosInContentSpace, 0.5f);
            verticalNormalizedPosition = Mathf.Clamp01(verticalNormalizedPosition);

            scrollRect.verticalNormalizedPosition = verticalNormalizedPosition;
        }





        public static void ScrollVerticalTo(this ScrollRect scrollRect, Transform target, float anchorWithViewPort)
        {
            Vector3 localPosInContentSpace =
                Transform_TargetInAnySpaceInsideContent_To_LocalPointInContentSpace(scrollRect.content, target);

            float verticalNormalizedPosition =
                CalculateVerticalNormalizedPos(scrollRect, localPosInContentSpace, anchorWithViewPort);
            verticalNormalizedPosition = Mathf.Clamp01(verticalNormalizedPosition);

            scrollRect.verticalNormalizedPosition = verticalNormalizedPosition;
        }

        public static void ScrollVerticalTo(this ScrollRect scrollRect, Transform target)
        {
            Vector3 localPosInContentSpace =
                Transform_TargetInAnySpaceInsideContent_To_LocalPointInContentSpace(scrollRect.content, target);

            float verticalNormalizedPosition = CalculateVerticalNormalizedPos(scrollRect, localPosInContentSpace, 0.5f);
            verticalNormalizedPosition = Mathf.Clamp01(verticalNormalizedPosition);

            scrollRect.verticalNormalizedPosition = verticalNormalizedPosition;
        }

        #endregion





        #region DoScrollHorizontalTo

        public static Tweener DoScrollHorizontalTo(this ScrollRect scrollRect, Vector3 localPosInContentSpace,
            float anchorWithViewPort, float duration, bool scrollOutRange = false)
        {
            float horizontalNormalizedPosition =
                CalculateHorizontalNormalizedPos(scrollRect, localPosInContentSpace, anchorWithViewPort);

            if (scrollOutRange == false)
                horizontalNormalizedPosition = Mathf.Clamp(horizontalNormalizedPosition, 0, 1);

            return DOTween.To(() => scrollRect.horizontalNormalizedPosition,
                (value) => scrollRect.horizontalNormalizedPosition = value, horizontalNormalizedPosition, duration);
        }

        public static Tweener DoScrollHorizontalTo(this ScrollRect scrollRect, Vector3 localPosInContentSpace,
            float duration, bool scrollOutRange = false)
        {
            return DoScrollHorizontalTo(scrollRect, localPosInContentSpace, 0.5f, duration, scrollOutRange);
        }


        public static Tweener DoScrollHorizontalTo(this ScrollRect scrollRect, Transform target,
            float anchorWithViewPort, float duration, bool scrollOutRange = false)
        {
            Vector3 localPosInContentSpace =
                Transform_TargetInAnySpaceInsideContent_To_LocalPointInContentSpace(scrollRect.content, target);
            float horizontalNormalizedPosition =
                CalculateHorizontalNormalizedPos(scrollRect, localPosInContentSpace, anchorWithViewPort);

            if (scrollOutRange == false)
                horizontalNormalizedPosition = Mathf.Clamp(horizontalNormalizedPosition, 0, 1);

            return DOTween.To(() => scrollRect.horizontalNormalizedPosition,
                (value) => scrollRect.horizontalNormalizedPosition = value, horizontalNormalizedPosition, duration);
        }

        public static Tweener DoScrollHorizontalTo(this ScrollRect scrollRect, Transform target, float duration,
            bool scrollOutRange = false)
        {
            return DoScrollHorizontalTo(scrollRect, target, 0.5f, duration, scrollOutRange);
        }

        #endregion





        #region DoScrollVerticalTo

        public static Tweener DoScrollVerticalTo(this ScrollRect scrollRect, Vector3 localPosInContentSpace,
            float anchorWithViewPort, float duration, bool scrollOutRange = false)
        {
            float verticalNormalizedPosition =
                CalculateVerticalNormalizedPos(scrollRect, localPosInContentSpace, anchorWithViewPort);

            if (scrollOutRange == false)
                verticalNormalizedPosition = Mathf.Clamp(verticalNormalizedPosition, 0, 1);

            return DOTween.To(() => scrollRect.verticalNormalizedPosition,
                (value) => scrollRect.verticalNormalizedPosition = value, verticalNormalizedPosition, duration);
        }

        public static Tweener DoScrollVerticalTo(this ScrollRect scrollRect, Vector3 localPosInContentSpace,
            float duration, bool scrollOutRange = false)
        {
            return DoScrollVerticalTo(scrollRect, localPosInContentSpace, 0.5f, duration, scrollOutRange);
        }


        public static Tweener DoScrollVerticalTo(this ScrollRect scrollRect, Transform target, float anchorWithViewPort,
            float duration, bool scrollOutRange = false)
        {
            Vector3 localPosInContentSpace =
                Transform_TargetInAnySpaceInsideContent_To_LocalPointInContentSpace(scrollRect.content, target);
            float verticalNormalizedPosition =
                CalculateVerticalNormalizedPos(scrollRect, localPosInContentSpace, anchorWithViewPort);

            if (scrollOutRange == false)
                verticalNormalizedPosition = Mathf.Clamp(verticalNormalizedPosition, 0, 1);

            return DOTween.To(() => scrollRect.verticalNormalizedPosition,
                (value) => scrollRect.verticalNormalizedPosition = value, verticalNormalizedPosition, duration);
        }

        public static Tweener DoScrollVerticalTo(this ScrollRect scrollRect, Transform target, float duration,
            bool scrollOutRange = false)
        {
            return DoScrollVerticalTo(scrollRect, target, 0.5f, duration, scrollOutRange);
        }

        #endregion
    }
}