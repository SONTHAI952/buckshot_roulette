using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZeroX.UI
{
    [AddComponentMenu("Layout/Layout Element Max Size", 199)]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class LayoutElementMaxSize : UIBehaviour, ILayoutSelfController
    {
        public bool useMaxWidth = false;
        public float maxWidth = 500;
        
        public bool useMaxHeight = false;
        public float maxHeight = 100;
        
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

        protected LayoutElementMaxSize()
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
            if(useMaxWidth == false)
                return;
            
            m_Tracker.Clear();
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
            
            if (rectTransform.sizeDelta.x > maxWidth)
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
        }

        /// <summary>
        /// Calculate and apply the vertical component of the size to the RectTransform
        /// </summary>
        public virtual void SetLayoutVertical()
        {
            if(useMaxHeight == false)
                return;
            
            m_Tracker.Clear();
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
            
            if (rectTransform.sizeDelta.y > maxHeight)
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight);
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