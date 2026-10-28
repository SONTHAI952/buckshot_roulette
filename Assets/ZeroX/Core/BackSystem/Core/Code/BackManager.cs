using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace ZeroX.BackSystem
{
    public class BackManager : MonoBehaviour
    {
        public static BackManager Instance { get; private set; }

        private Vector2 CenterScreenPoint => new Vector2(Screen.width / 2f, Screen.height / 2f);
        
        
        
        private void Awake()
        {
            if (Instance == null || Instance == this)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape) == false)
                return;

            var backable = FindBackableAtScreenPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
            if (backable != null)
            {
                backable.Back();
            }
        }



        private Transform FindUIAtScreenPoint(Vector2 screenPoint)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = screenPoint;
            
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            if(results.Count == 0)
                return null;

            return results[0].gameObject.transform;
        }

        private IBackable FindBackableAtScreenPoint(Vector2 screenPoint)
        {
            var ui = FindUIAtScreenPoint(screenPoint);
            if (ui == null)
                return null;
            
            var backable = ui.gameObject.GetComponentInParent<IBackable>();
            return backable;
        }
        
        public bool IsButtonClickable(Button button)
        {
            if (button.isActiveAndEnabled == false || button.interactable == false)
                return false;
            
            Canvas canvas = button.GetComponentInParent<Canvas>();
            if (canvas == null)
                return false;

            canvas = canvas.rootCanvas;
            Camera canvasCamera = GetRenderCameraOfCanvas(canvas);

            var screenPoint = RectTransformUtility.WorldToScreenPoint(canvasCamera, button.transform.position);
            var child = FindUIAtScreenPoint(screenPoint);
            if (child == null)
                return false;

            return IsParent(child, button.transform);
        }


        /// <summary>
        /// Dùng để kiểm tra 1 button back riêng biệt. Thường dùng cho button pause của các mode chơi. Vì các mode chơi nếu ko phải game trên UI thì sẽ ko có UI để tìm backable
        /// <para>Gọi hàm này trong Update của các mode chơi</para>
        /// </summary>
        public void CheckAndTriggerBackButton(Button button)
        {
            if(Input.GetKeyDown(KeyCode.Escape) == false)
                return;

            var backable = FindBackableAtScreenPoint(CenterScreenPoint);
            if(backable != null)
                return;
            
            if(IsButtonClickable(button) == false)
                return;
            
            button.onClick.Invoke();
        }
        
        
        #region Utility

        private Camera GetRenderCameraOfCanvas(Canvas canvas)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return canvas.worldCamera;
        }

        private bool IsParent(Transform child, Transform parent)
        {
            Transform tempParent = child;
            while (true)
            {
                if (tempParent == null)
                    return false;
                
                if (tempParent == parent)
                    return true;

                tempParent = tempParent.parent;
            }
        }

        #endregion
    }
}