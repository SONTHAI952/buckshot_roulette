using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.TabSystem
{
    public class TabMotorDotSimple : TabMotor
    {
        [SerializeField] private RectTransform selectedDot;
        [SerializeField] private RectTransform normalDotContainer;
        [SerializeField] private GameObject normalDotPrefab;

        private List<RectTransform> listNormalDot = new List<RectTransform>();

        private bool initialized = false;

        private void Start()
        {
            InitIfNot();
        }

        void InitIfNot()
        {
            if(initialized)
                return;

            initialized = true;
            InitListNormalDot();
        }

        void InitListNormalDot()
        {
            foreach (Transform child in normalDotContainer)
            {
                Destroy(child.gameObject);
            }
            
            listNormalDot.Clear();
            
            for (int i = 0; i < TabView.TabCount; i++)
            {
                var normalDot = Instantiate(normalDotPrefab, normalDotContainer);
                listNormalDot.Add((RectTransform)normalDot.transform);
                
                normalDot.SetActive(true);
            }
        }

        private void Update()
        {
            UpdateSelectedDot();
        }

        void UpdateSelectedDot()
        {
            if (TabView.TabCount == 0 || listNormalDot.Count == 0)
            {
                selectedDot.gameObject.SetActive(false);
                return;
            }

            int leftIndex = Mathf.FloorToInt(TabView.CurrentSmoothTabIndex);
            int rightIndex = Mathf.CeilToInt(TabView.CurrentSmoothTabIndex);

            leftIndex = Mathf.Clamp(leftIndex, 0, TabView.TabCount - 1);
            rightIndex = Mathf.Clamp(rightIndex, 0, TabView.TabCount - 1);
            
            
            //Khi listNormalDot chưa match với TabView Count thì ko làm gì cả
            if (leftIndex >= listNormalDot.Count || rightIndex >= listNormalDot.Count)
            {
                selectedDot.gameObject.SetActive(false);
            }



            if (leftIndex == rightIndex)
            {
                selectedDot.position = listNormalDot[leftIndex].position;
                return;
            }

            float delta = (TabView.CurrentSmoothTabIndex - leftIndex) / (rightIndex - leftIndex);
            selectedDot.position = Vector3.Lerp(listNormalDot[leftIndex].position, listNormalDot[rightIndex].position, delta);
        }

        public override void OnTabViewAddTab(TabData tabData, int index)
        {
            if(initialized == false)
                return;
            
            var normalDot = Instantiate(normalDotPrefab, normalDotContainer);
            listNormalDot.Insert(index, (RectTransform)normalDot.transform);

            normalDot.SetActive(true);
        }

        public override void OnTabViewRemoveTab(TabData tabData, int index)
        {
            if(initialized == false)
                return;
            
            Destroy(listNormalDot[index].gameObject);
            listNormalDot.RemoveAt(index);
        }
    }
}