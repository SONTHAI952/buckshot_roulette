using System;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.TabSystem
{
    public class TestTabSystem : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform point;
        [SerializeField] private RectTransform rectTransform;

        [SerializeField] private TabView tabView;
        [SerializeField] private string tabPathWantOpen;


        private void Awake()
        {
            tabView.onSwitchTab += TabViewOnSwitchTab;
        }

        private void TabViewOnSwitchTab(OnSwitchTabData data)
        {
            Debug.LogFormat("PrevIndex: {0}  -  CurrentIndex: {1}", data.prevTabIndex, data.currentTabIndex);
        }

        [ContextMenu("Log Info")]
        public void LogInfo()
        {
            Debug.Log(scrollRect.horizontalNormalizedPosition);
        }
        
        [ContextMenu("ChangeRectLocalPos")]
        public void ChangeRectLocalPos()
        {
            Vector2 oldLocalPos = rectTransform.localPosition;
            Vector2 oldAnchoredPos = rectTransform.anchoredPosition;
            
            rectTransform.anchoredPosition += Vector2.right * 100;
            
            Debug.LogFormat("OldLocalPos: {0} - LocalPos: {1}", oldLocalPos, rectTransform.localPosition);
            Debug.LogFormat("OldAnchoredPos: {0} - AnchoredPos: {1}", oldAnchoredPos, rectTransform.anchoredPosition);
            
        }
        
        [ContextMenu("Convert Local Pos")]
        public void ConvertLocalPos()
        {
            RectTransform child = (RectTransform)point.parent;
            Vector3 finalLocalPos = child.localPosition + child.localRotation * point.localPosition;
            Debug.Log(finalLocalPos);
        }

        [ContextMenu("Snap Pos")]
        public void SnapPos()
        {
            // float pos = scrollRect.CalculateHorizontalNormalizedPos(point);
            // scrollRect.horizontalNormalizedPosition = pos;

            
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
                OpenTab();
        }

        [ContextMenu("Open Tab")]
        public void OpenTab()
        {
            tabView.OpenTab(tabPathWantOpen);
        }

        [ContextMenu("Get Tab")]
        public void GetTab()
        {
            var data = tabView.GetTabData("a/b/c");
            if(data == null)
                Debug.Log("Tab null");
            else
            {
                Debug.Log("Tab: " + data.Id, data.TabContent);
            }
        }
    }
}