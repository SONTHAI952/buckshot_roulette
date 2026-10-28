using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZeroX.TabSystem
{
    public class TabInteractiveScrollViewSimple : TabInteractive
    {
        [SerializeField] private ScrollRect scrollRect;
        private bool touchDownValid = false;

        private void Update()
        {
            if (scrollRect.horizontal && scrollRect.vertical)
            {
                Debug.LogError("ScrollRect enable both horizontal and vertical, please enable only one");
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (IsTouchOnSelfScrollRect())
                    touchDownValid = true;
                else
                    touchDownValid = false;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (touchDownValid)
                {
                    touchDownValid = false;
                    CheckSnapOrMoveToNext();
                }
            }

            if (touchDownValid)
            {
                if (scrollRect.horizontal)
                {
                    int tabIndex = GetTabIndex(scrollRect.horizontalNormalizedPosition);
                    float smoothTabIndex = GetSmoothTabIndex(scrollRect.horizontalNormalizedPosition);
                    
                    SetCurrentTabIndex(tabIndex, true, false);
                    SetCurrentSmoothTabIndex(smoothTabIndex, false);
                    NotifyTabViewTransition();
                }
                else
                {
                    int tabIndex = GetTabIndex(scrollRect.verticalNormalizedPosition);
                    float smoothTabIndex = GetSmoothTabIndex(scrollRect.verticalNormalizedPosition);
                
                    SetCurrentTabIndex(tabIndex, true, false);
                    SetCurrentSmoothTabIndex(smoothTabIndex, false);
                    NotifyTabViewTransition();
                }
            }
        }

        void CheckSnapOrMoveToNext()
        {
            if (scrollRect.horizontal)
            {
                float velocity = scrollRect.velocity.x;
                
                if (Mathf.Abs(velocity) < 850)
                {
                    TabView.SnapTab();
                    return;
                }

                if (velocity < 0) //TabIndex tăng
                {
                    int nextIndex = Mathf.FloorToInt(TabView.CurrentSmoothTabIndex) + 1;
                    nextIndex = Mathf.Clamp(nextIndex, 0, TabView.TabCount - 1);
                    OpenTabKeepCurrentVelocity(nextIndex);
                }
                else
                {
                    int nextIndex = Mathf.CeilToInt(TabView.CurrentSmoothTabIndex) - 1;
                    nextIndex = Mathf.Clamp(nextIndex, 0, TabView.TabCount - 1);
                    OpenTabKeepCurrentVelocity(nextIndex);
                }
            }
        }

        int GetTabIndex(float scrollRectNormalizedPos)
        {
            float smoothTabIndex = Mathf.Lerp(0, TabView.TabCount - 1, scrollRectNormalizedPos);
            int tabIndex = Mathf.RoundToInt(smoothTabIndex);
            if (tabIndex < 0)
                tabIndex = 0;

            if (tabIndex >= TabView.TabCount)
                tabIndex = TabView.TabCount - 1;

            return tabIndex;
        }
        
        float GetSmoothTabIndex(float scrollRectNormalizedPos)
        {
            return scrollRectNormalizedPos * (TabView.TabCount - 1);
            //return Mathf.Lerp(0, TabView.TabCount - 1, scrollRectNormalizedPos);
        }

        bool IsTouchOnSelfScrollRect()
        {
            if (EventSystem.current == null)
                return false;
        
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = Input.mousePosition;
            
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            if (results.Count == 0)
                return false;

            return IsInside(scrollRect.transform, results[0].gameObject.transform);
        }

        bool IsInside(Transform container, Transform childWantCheck)
        {
            while (true)
            {
                if (childWantCheck == container)
                    return true;

                if (childWantCheck == null)
                    return false;

                childWantCheck = childWantCheck.parent;
            }
        }

        void OpenTabKeepCurrentVelocity(int index)
        {
            float speed = Mathf.Abs(TabView.CurrentSmoothTabIndex - TabView.PreSmoothTabIndex) / Time.deltaTime;
            if (speed < TabView.SnapSpeed)
                speed = TabView.SnapSpeed;
            float distance = Mathf.Abs(index - TabView.CurrentSmoothTabIndex);
            float duration = distance / speed;
            TabView.OpenTab(index, duration);
        }
    }
}