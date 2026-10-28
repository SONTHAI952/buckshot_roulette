using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZeroX
{
    public static class PointerOverUIChecker
    {
        public static bool IsOverUI(Vector2 pointerPos)
        {
            if (EventSystem.current == null)
                return false;

            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(pointerPos.x, pointerPos.y);

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        /// <summary>
        /// Bỏ qua canvas dạng worldSpace
        /// </summary>
        public static bool IsOverUI_IgnoreWorldSpace(Vector2 pointerPos)
        {
            if (EventSystem.current == null)
                return false;

            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(pointerPos.x, pointerPos.y);

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);


            foreach (RaycastResult result in results)
            {
                Canvas canvas = result.gameObject.GetComponentInParent<Canvas>();
                if (canvas == null)
                    continue;

                if (canvas.renderMode == RenderMode.WorldSpace)
                    continue;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Tự động lấy mouse pos hiện tại
        /// </summary>
        public static bool IsOverUI()
        {
            Vector3 pointerPos;


#if UNITY_EDITOR
            pointerPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#else
        if (Input.touchCount <= 0)
        {
            pointerPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        else
        {
            var touch = Input.GetTouch(0);
            pointerPos = new Vector2(touch.position.x, touch.position.y);
        }
#endif

            return IsOverUI(pointerPos);
        }

        /// <summary>
        /// Bỏ qua canvas dạng worldSpace. Tự động lấy mouse pos hiện tại
        /// </summary>
        public static bool IsOverUI_IgnoreWorldSpace()
        {
            Vector3 pointerPos;


#if UNITY_EDITOR
            pointerPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#else
        if (Input.touchCount <= 0)
        {
            pointerPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        else
        {
            var touch = Input.GetTouch(0);
            pointerPos = new Vector2(touch.position.x, touch.position.y);
        }
#endif

            return IsOverUI_IgnoreWorldSpace(pointerPos);
        }
    }
}