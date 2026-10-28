using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public static class WindowSortingManager
    {
        private class ElementData
        {
            public Canvas canvas;
            public CanvasGroup canvasGroup;
            public bool visibleWhenBehind;
            public float openDuration;

            public ElementData()
            {
            }

            public ElementData(Canvas canvas, CanvasGroup canvasGroup, bool visibleWhenBehind, float openDuration)
            {
                this.canvas = canvas;
                this.canvasGroup = canvasGroup;
                this.visibleWhenBehind = visibleWhenBehind;
                this.openDuration = openDuration;
            }
        }
        
        private class ListElementData
        {
            public List<ElementData> listSorting = new List<ElementData>();
            public List<ElementData> listInvisibleBack = new List<ElementData>();
            
            public void AddOrBringToTop(ElementData elementData)
            {
                listSorting.Remove(elementData);
                listSorting.Add(elementData);

                listInvisibleBack.Remove(elementData);
                listInvisibleBack.Add(elementData);
            }
        }


        private static readonly Dictionary<WindowSortingLayerId, ListElementData> dictSorting = new();
        
        
        //Temp
        private static Sequence tweenHandleInvisibleWhenBehind;



        #region Utility

        private static ListElementData ForceGetListElementData(WindowSortingLayerId layerId)
        {
            if (dictSorting.TryGetValue(layerId, out var listElementData) == false)
            {
                listElementData = new ListElementData();
                dictSorting.Add(layerId, listElementData);
            }

            return listElementData;
        }

        
        private static void MaintainListElementData(List<ElementData> listElementData, ElementData elementDataWantAdd)
        {
            for (int i = 0; i < listElementData.Count; i++)
            {
                var  elementData = listElementData[i];
                if (elementData.canvas == null)
                {
                    listElementData.RemoveAt(i);
                    i--;
                    continue;
                }
                
                if (elementData.canvas == elementDataWantAdd.canvas)
                {
                    listElementData.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }
        
        private static void MaintainListElementData(List<ElementData> listElementData)
        {
            for (int i = 0; i < listElementData.Count; i++)
            {
                var  elementData = listElementData[i];
                if (elementData.canvas == null)
                {
                    listElementData.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }
        
        /// <summary>
        /// Xóa các element có canvas null và Nếu trong list đã có một element có canvas giống với elementDataWantAdd thì xóa đi
        /// </summary>
        private static void MaintainListElementData(ListElementData listElementData, ElementData elementDataWantAdd)
        {
            MaintainListElementData(listElementData.listSorting, elementDataWantAdd);
            MaintainListElementData(listElementData.listInvisibleBack, elementDataWantAdd);
        }

        
        private static void MaintainListElementData(ListElementData listElementData)
        {
            MaintainListElementData(listElementData.listSorting);
            MaintainListElementData(listElementData.listInvisibleBack);
        }
        
        

        private static bool ValidateElementData(ElementData elementData)
        {
            if (elementData.canvas == null)
            {
                Debug.LogError("Cannot handle sorting UI because canvas is null");
                return false;
            }
            
            if (elementData.canvasGroup == null)
            {
                Debug.LogError("Cannot handle sorting UI because canvasGroup is null");
                return false;
            }

            return true;
        }
        
        private static bool Is_A_Behind_B(Canvas canvasA, Canvas canvasB)
        {
            if (canvasA.renderMode != canvasB.renderMode)
            {
                return canvasA.renderMode == RenderMode.ScreenSpaceCamera &&
                       canvasB.renderMode == RenderMode.ScreenSpaceOverlay;
            }

            if (canvasA.sortingLayerID == canvasB.sortingLayerID)
            {
                return canvasA.sortingOrder <= canvasB.sortingOrder;
            }
            
            //2 thằng khác sortingLayer
            int indexA = SortingLayer.GetLayerValueFromID(canvasA.sortingLayerID);
            int indexB = SortingLayer.GetLayerValueFromID(canvasB.sortingLayerID);
            return indexA <= indexB;
        }

        #endregion

        

        #region Apply

        public static void Apply(WindowSortingLayerId layerId, Canvas canvas, CanvasGroup canvasGroup, bool visibleWhenBehind, float openDuration)
        {
            //Validate
            if (canvas == null)
            {
                Debug.LogError("Cannot handle sorting UI because canvas is null");
                return;
            }
            
            if (canvasGroup == null)
            {
                Debug.LogError("Cannot handle sorting UI because canvasGroup is null");
                return;
            }

           
            
            //Kill tween cũ
            if (tweenHandleInvisibleWhenBehind != null)
            {
                tweenHandleInvisibleWhenBehind.Kill();
                tweenHandleInvisibleWhenBehind = null;
            }
            
            
            
            //Setup
            ElementData elementData = new ElementData(canvas, canvasGroup, visibleWhenBehind, openDuration);
            var listElementData = ForceGetListElementData(layerId);
            MaintainListElementData(listElementData, elementData);
            
            //Thêm element vào
            listElementData.AddOrBringToTop(elementData);

            //Apply
            ApplySortingOrder(layerId, listElementData.listSorting);
            ApplyInvisibleWhenBehind(layerId, listElementData.listInvisibleBack, elementData);
        }
        
        private static void ApplySortingOrder(WindowSortingLayerId layerId, List<ElementData> listElementData)
        {
            var sortingOrderStart = WindowSortingTable.Instance.GetStartOrderInLayer(layerId);
            for (int i = 0; i < listElementData.Count; i++)
            {
                listElementData[i].canvas.sortingOrder = sortingOrderStart;
                sortingOrderStart++;
            }
        }

        /// <summary>
        /// Ẩn những thằng ko phải top. Hàm này chỉ dùng elementDataTop để so sánh và ko tác động gì đến elementDataTop cả
        /// </summary>
        private static void ApplyInvisibleWhenBehind(WindowSortingLayerId layerId, List<ElementData> listElementData, ElementData elementDataTop)
        {
            tweenHandleInvisibleWhenBehind = DOTween.Sequence();
            
            for (int i = 0; i < listElementData.Count; i++)
            {
                var otherElementData = listElementData[i];
                if(otherElementData == elementDataTop)
                    continue;

                if(otherElementData.visibleWhenBehind)
                    continue;
                
                if (Is_A_Behind_B(otherElementData.canvas, elementDataTop.canvas))
                {
                    float duration = otherElementData.openDuration + 0.1f; //Để chắc chắn tween của việc ẩn này kết thúc sau tween của WindowAnim, vì 2 thằng có thể cùng dùng một CanvasGroup
                    var tween = otherElementData.canvasGroup.DOFade(0, duration);
                    tweenHandleInvisibleWhenBehind.Insert(0, tween);
                }
            }
        }
        
        #endregion
        

        
        #region UnApply

        /// <summary>
        /// Khi closing thì xử lý listVisibleWhenBack
        /// </summary>
        public static void UnApply_WhenClosing(WindowSortingLayerId layerId, Canvas canvas)
        {
            if (canvas == null)
            {
                Debug.LogError("Cannot handle sorting UI because canvas is null");
                return;
            }
            
            
            //Kill tween cũ
            if (tweenHandleInvisibleWhenBehind != null)
            {
                tweenHandleInvisibleWhenBehind.Kill();
                tweenHandleInvisibleWhenBehind = null;
            }
            
            //Setup
            var listElementData = ForceGetListElementData(layerId).listInvisibleBack;
            MaintainListElementData(listElementData);
            if (listElementData.Count == 0)
                return;
            
            
            //Remove element khỏi list
            bool isTop = listElementData[listElementData.Count - 1].canvas == canvas;
            listElementData.RemoveAll(d => d.canvas == canvas);

            
            //Nếu remove element không phải top thì không cần xử lý tiếp vì elementTop vẫn giữ nguyên
            if (isTop == false)
                return;

            
            //Xử lý sau khi xóa top cũ
            Handle_InvisibleWhenBehind_AfterUnApplyElementTop(listElementData);
        }
        
        /// <summary>
        /// Khi closed thì xử lý listSorting
        /// </summary>
        public static void UnApply_WhenClosed(WindowSortingLayerId layerId, Canvas canvas)
        {
            if (canvas == null)
            {
                Debug.LogError("Cannot handle sorting UI because canvas is null");
                return;
            }
            
            //Setup
            var listElementData = ForceGetListElementData(layerId).listSorting;
            MaintainListElementData(listElementData);
            if (listElementData.Count == 0)
                return;
            
            //Remove element khỏi list
            listElementData.RemoveAll(d => d.canvas == canvas);
        }

        private static void Handle_InvisibleWhenBehind_AfterUnApplyElementTop(List<ElementData> listElementData)
        {
            //Không còn gì trong dach sách thì thôi
            if (listElementData.Count == 0)
                return;
            
            
            
            //Lấy top mới
            var elementDataTop = listElementData[listElementData.Count - 1];
            
            
            //Ẩn những thằng không phải top
            tweenHandleInvisibleWhenBehind = DOTween.Sequence();
            for (int i = 0; i < listElementData.Count; i++)
            {
                var otherElementData = listElementData[i];
                if (otherElementData == elementDataTop)
                    continue;

                if(otherElementData.visibleWhenBehind)
                    continue;
                
                if (Is_A_Behind_B(otherElementData.canvas, elementDataTop.canvas))
                {
                    float duration = otherElementData.openDuration + 0.1f; //Để chắc chắn tween của việc ẩn này kết thúc sau tween của WindowAnim, vì 2 thằng có thể cùng dùng một CanvasGroup
                    var tween = otherElementData.canvasGroup.DOFade(0, duration);
                    tweenHandleInvisibleWhenBehind.Insert(0, tween);
                }
            }
            
            
            //Hiện top mới lên
            float duration2 = elementDataTop.openDuration + 0.1f; //Để chắc chắn tween của việc ẩn này kết thúc sau tween của WindowAnim, vì 2 thằng có thể cùng dùng một CanvasGroup
            var tween2 = elementDataTop.canvasGroup.DOFade(1, duration2);
            tweenHandleInvisibleWhenBehind.Insert(0, tween2);
        }
        
        #endregion
    }
}