using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZeroX.UI
{
    [AddComponentMenu("Layout/Layout Element Min Size", 200)]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class LayoutElementMinSize : UIBehaviour, ILayoutSelfController
    {
        public bool useMinWidth = false;
        public float minWidth = 500;
        
        public bool useMinHeight = false;
        public float minHeight = 100;
        
        [System.NonSerialized] private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        // field is never assigned warning
        #pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;
        #pragma warning restore 649

        protected LayoutElementMinSize()
        {}

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        /// <summary>
        /// Calculate and apply the horizontal component of the size to the RectTransform
        /// </summary>
        public virtual void SetLayoutHorizontal()
        {
            if(useMinWidth == false)
                return;
            
            m_Tracker.Clear();
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
            
            if (rectTransform.sizeDelta.x < minWidth)
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minWidth);
        }

        /// <summary>
        /// Calculate and apply the vertical component of the size to the RectTransform
        /// </summary>
        public virtual void SetLayoutVertical()
        {
            if(useMinHeight == false)
                return;
            
            m_Tracker.Clear();
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
            
            if (rectTransform.sizeDelta.y < minHeight)
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, minHeight);
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

    #if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

    #endif
    }
}