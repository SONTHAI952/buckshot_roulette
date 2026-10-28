using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.TabSystem
{
    /// <summary>
    /// Tác dụng là tạo hiệu ứng highlight move theo tab button đang chọn
    /// </summary>
    public class TabMotorFollowSelectedTabButton : TabMotor
    {
        [Tooltip("Follower cần đặt trong cùng parent với các tab button")]
        [SerializeField] private RectTransform follower;
        [SerializeField] private bool matchSizeWithTabButton = true;
        [SerializeField] private float followSpeed = 20;


        //Temp
        [NonSerialized] private bool isInitialized = false;


        private void Update()
        {
            Vector3 destLocalPos = CalculateDestLocalPosition();
            follower.localPosition = Vector3.Lerp(follower.localPosition, destLocalPos, followSpeed * Time.unscaledDeltaTime);

            if (matchSizeWithTabButton)
            {
                Vector3 destSizeDelta = CalculateDestSizeDelta();
                follower.sizeDelta = Vector2.Lerp(follower.sizeDelta, destSizeDelta, followSpeed * Time.unscaledDeltaTime);
            }
        }


        IEnumerator DelayInit()
        {
            yield return null;
            yield return null;
            
            follower.localPosition = CalculateDestLocalPosition();
            if(matchSizeWithTabButton)
                follower.sizeDelta = CalculateDestSizeDelta();
        }


        public override void OnTabViewSwitchTab()
        {
            if(isInitialized)
                return;
            
            isInitialized = true;
            StartCoroutine(DelayInit());
        }

        private Vector3 CalculateDestLocalPosition()
        {
            var tabButtonFirst = TabView.GetTabButton(0);
            var tabButtonLast = TabView.GetTabButton(TabView.TabCount - 1);

            float delta = TabView.CurrentSmoothTabIndex / (TabView.TabCount - 1);
            
            return Vector3.Lerp(tabButtonFirst.transform.localPosition, tabButtonLast.transform.localPosition, delta);
        }

        private Vector2 CalculateDestSizeDelta()
        {
            var tabButton = TabView.GetTabButton(TabView.CurrentTabIndex);
            RectTransform targetRect = (RectTransform)tabButton.Button.transform;
            return targetRect.sizeDelta;
        }
    }
}