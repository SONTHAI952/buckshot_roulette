using System;
using UnityEngine;

namespace Game.Primary.PlatformLayoutSystem
{
    /// <summary>
    /// Giữ hai bộ layout (dọc / ngang) cho một RectTransform và tự áp dụng khi hướng màn hình đổi
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class RectOrientationLayout : MonoBehaviour
    {
        [Serializable]
        public class Layout
        {
            public Vector2 anchorMin;
            public Vector2 anchorMax;
            public Vector2 pivot;
            public Vector2 anchoredPosition;
            public Vector2 sizeDelta;

            public static Layout From(RectTransform rt)
            {
                return new Layout
                {
                    anchorMin = rt.anchorMin,
                    anchorMax = rt.anchorMax,
                    pivot = rt.pivot,
                    anchoredPosition = rt.anchoredPosition,
                    sizeDelta = rt.sizeDelta
                };
            }

            public void ApplyTo(RectTransform rt)
            {
                rt.anchorMin = anchorMin;
                rt.anchorMax = anchorMax;
                rt.pivot = pivot;
                rt.anchoredPosition = anchoredPosition;
                rt.sizeDelta = sizeDelta;
            }
        }

        [SerializeField] private Layout portrait = new Layout();
        [SerializeField] private Layout landscape = new Layout();

        private bool? lastPortrait;


        public void Setup(Layout portraitLayout, Layout landscapeLayout)
        {
            portrait = portraitLayout;
            landscape = landscapeLayout;
            lastPortrait = null;
            Apply();
        }

        private void OnEnable()
        {
            lastPortrait = null;
            Apply();
        }

        private void Update()
        {
            Apply();
        }

        private void Apply()
        {
            bool isPortrait = OrientationUtility.IsPortrait;
            if (lastPortrait == isPortrait)
                return;

            lastPortrait = isPortrait;
            (isPortrait ? portrait : landscape).ApplyTo((RectTransform)transform);
        }
    }
}
