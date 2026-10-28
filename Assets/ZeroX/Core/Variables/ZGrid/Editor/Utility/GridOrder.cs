using UnityEditor;
using UnityEngine;

namespace ZeroX.ZGridEditors
{
    public class GridOrder
    {
        private ZGridEditorWindow window;
        private GridSp gridSp;
        
        
        //Const
        private const int GPosEmpty = -1;
        private const float DestLineSize = 3f;

        
        //Temp
        private int selectedGPosX = GPosEmpty;
        private int selectedGPosY = GPosEmpty;
        private Rect selectedBoxRect;
        private float offsetMouse;

        private int destGPosX = GPosEmpty;
        private int destGPosY = GPosEmpty;
        private Rect destLineRect; //Đường thẳng thể hiện index mà sẽ move đến
        
        
        
        //Property
        private bool IsSelecting => selectedGPosX != GPosEmpty || selectedGPosY != GPosEmpty;

        
        
        public void SetContext(ZGridEditorWindow window, GridSp gridSp)
        {
            this.window = window;
            this.gridSp = gridSp;
        }

        

        #region Event Utility

        private bool IsLeftClickDownInLastRect()
        {
            Event ev = Event.current;
            Rect controlRect = GUILayoutUtility.GetLastRect();
            return ev.type == EventType.MouseDown && ev.button == 0 && controlRect.Contains(ev.mousePosition);
        }
        
        private bool IsLeftClickUp()
        {
            Event ev = Event.current;
            return ev.type == EventType.MouseUp && ev.button == 0;
        }

        private bool IsLeftMouseDrag()
        {
            Event ev = Event.current;
            return ev.type == EventType.MouseDrag && ev.button == 0;
        }

        #endregion


        #region Utility
        
        private Vector2 ScreenSpaceToWindowSpace(Vector2 screenPos)
        {
            Vector2 pos = screenPos - window.position.position;
            pos.y -= 19;

            return pos;
        }

        private Rect LocalGUIRectToWindowSpace(Rect localRect)
        {
            Rect screenRect = GUIUtility.GUIToScreenRect(localRect);
            
            Rect result = new Rect();
            result.size = screenRect.size;
            result.position = ScreenSpaceToWindowSpace(screenRect.position);
            
            return result;
        }
        
        #endregion


        
        #region Event

        public void OnAfterDrawColumnControl(int gPosX)
        {
            if(IsLeftClickDownInLastRect() == false)
                return;
            
            
            //SelectedIndex
            selectedGPosX = gPosX;
            selectedGPosY = GPosEmpty;
            
            //SelectedBoxRect
            Rect localSelectedBoxRect = GUILayoutUtility.GetLastRect();
            localSelectedBoxRect.x -= ZGridEditorWindow.ButtonInsertSize / 2f;
            localSelectedBoxRect.width += ZGridEditorWindow.ButtonInsertSize;
            
            selectedBoxRect = LocalGUIRectToWindowSpace(localSelectedBoxRect);
            
            
            selectedBoxRect.height = window.position.height - selectedBoxRect.y;
            
            //Offset
            offsetMouse = localSelectedBoxRect.x - Event.current.mousePosition.x;
            
            
            
            //Event.current.Use();
        }
        
        public void OnAfterDrawRowControl(int gPosY)
        {
            if(IsLeftClickDownInLastRect() == false)
                return;
            
            
            //SelectedIndex
            selectedGPosX = GPosEmpty;
            selectedGPosY = gPosY;
            
            
            //SelectedBoxRect
            selectedBoxRect = GUILayoutUtility.GetLastRect();
            selectedBoxRect.y -= ZGridEditorWindow.ButtonInsertSize / 2f;
            selectedBoxRect.height += ZGridEditorWindow.ButtonInsertSize / 2f;
            
            selectedBoxRect.width = window.position.width - selectedBoxRect.x;
            
            //Offset
            offsetMouse = selectedBoxRect.y - Event.current.mousePosition.y;
            
            
            //Event.current.Use();
        }

        public void OnAfterDrawGridContentScrollView(Vector2 scrollPos, int width, int height, Vector2 cellSize)
        {
            if(IsSelecting == false)
                return;
            
            if((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) == false)
                return;
            
            
            //Mouse
            Vector2 mouseScreenPos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            
            //Scroll
            Rect scrollViewLocalRect = GUILayoutUtility.GetLastRect();
            Rect scrollViewContentLocalRect = scrollViewLocalRect;
            scrollViewContentLocalRect.position -= scrollPos; //ScrollViewContent là thứ trượt trong scrollView
            scrollViewContentLocalRect.position += new Vector2(ZGridEditorWindow.ButtonInsertSize / 2f, ZGridEditorWindow.ButtonInsertSize / 2f); //Vì các cell đc offset một đoạn bằng button insert size đầu tiên
            Vector2 scrollViewContentScreenPos = GUIUtility.GUIToScreenPoint(scrollViewContentLocalRect.position);
            
            //Offset mouse and scroll: 2 điểm cùng đơn vị, lấy scrollViewContentScreenPos làm gốc tọa độ
            Vector2 offset = mouseScreenPos - scrollViewContentScreenPos; //Có thể coi là localPos trong không gian của scrollViewContent
            
            
            
            //Calculate DestGPosX
            destGPosX = Mathf.FloorToInt(offset.x / cellSize.x);
            destGPosX = Mathf.Clamp(destGPosX, 0, width);
            if (destGPosX == selectedGPosX + 1) //Về mặt visual, Move tới sau vị trí của ô đứng trước nó thì vẫn là vị trí đó
                destGPosX = selectedGPosX;
            
            
            
            //Calculate DestGPosY
            GridPivotType gridPivotType = GridEditorPref.GetGridPivotType(gridSp);
            float offsetY_Fix = gridPivotType == GridPivotType.TopLeft ? offset.y : height * cellSize.y - offset.y; //Đảo ngược offset để tính từ dưới lên
            
            destGPosY = Mathf.FloorToInt(offsetY_Fix / cellSize.y);
            destGPosY = Mathf.Clamp(destGPosY, 0, height);
            if (destGPosY == selectedGPosY + 1) //Về mặt visual, Move tới sau vị trí của ô đứng trước nó thì vẫn là vị trí đó
                destGPosY = selectedGPosY;
            
            
            //Vẽ thì vẫn vẽ từ trên xuống nên cần đảo ngược trở lại
            int destGPosYToDraw = gridPivotType == GridPivotType.TopLeft ? destGPosY : height - destGPosY;
            
            
            //Tính toán destLineRect
            float columnScreenPos = scrollViewContentScreenPos.x + destGPosX * cellSize.x;
            float columnWindowPos = ScreenSpaceToWindowSpace(new Vector2(columnScreenPos, 0)).x;

            float rowScreenPos = scrollViewContentScreenPos.y + destGPosYToDraw * cellSize.y;
            float rowWindowPos = ScreenSpaceToWindowSpace(new Vector2(0, rowScreenPos)).y;

            Vector2 scrollViewContentWindowPos = ScreenSpaceToWindowSpace(scrollViewContentScreenPos);

            
            if (selectedGPosX != GPosEmpty)
            {
                destLineRect = new Rect(columnWindowPos - DestLineSize, 0, DestLineSize, 10000);
                destLineRect.x = columnWindowPos - DestLineSize; //Để line ở giữa 2 cell
                destLineRect.y = scrollViewContentWindowPos.y;
                destLineRect.width = DestLineSize;
                destLineRect.height = 10000;
            }
            else if (selectedGPosY != GPosEmpty)
            {
                destLineRect = new Rect(0, rowWindowPos - DestLineSize, 10000, DestLineSize);
                destLineRect.x = scrollViewContentWindowPos.x;
                destLineRect.y = rowWindowPos - DestLineSize;
                destLineRect.width = 10000;
                destLineRect.height = DestLineSize;
            }
        }

        #endregion

        
        
        

        public void Handle()
        {
            if (IsLeftClickUp())
            {
                if(selectedGPosX >= 0)
                    ExecuteOrderColumn();
                else if(selectedGPosY >= 0)
                    ExecuteOrderRow();
                
                return;
            }

            //Nghĩa là chưa click up
            if(selectedGPosX >= 0)
                PreviewOrderColumn();
            else if(selectedGPosY >= 0)
                PreviewOrderRow();
        }


        #region Execute Preview

        private void PreviewOrderColumn()
        {
            Event ev = Event.current;
            
            //Overlay
            selectedBoxRect.x = ev.mousePosition.x + offsetMouse;
            GUI.Box(selectedBoxRect, "");
            
            
            //Header
            Rect headerRect = selectedBoxRect;
            headerRect.width = ZGridEditorWindow.RowColumnControl_ElementSize;
            headerRect.height = ZGridEditorWindow.RowColumnControl_ElementSize;
            headerRect.x += selectedBoxRect.width / 2 - headerRect.width / 2f;
            GUI.Box(headerRect, selectedGPosX.ToString(), GridEditorStyle.BoxLabelSelectedToOrder);
            
            
            //DestLine
            GUI.DrawTexture(destLineRect, GridEditorMedia.DestLine, ScaleMode.StretchToFill);
            
            
            
            window.Repaint();
        }
        
        private void PreviewOrderRow()
        {
            Event ev = Event.current;
            
            //Overlay
            selectedBoxRect.y = ev.mousePosition.y + offsetMouse;
            GUI.Box(selectedBoxRect, "");
            
            //Header
            Rect headerRect = selectedBoxRect;
            headerRect.width = ZGridEditorWindow.RowColumnControl_ElementSize;
            headerRect.height = ZGridEditorWindow.RowColumnControl_ElementSize;
            headerRect.y += selectedBoxRect.height / 2 - headerRect.height / 2f;
            GUI.Box(headerRect, selectedGPosY.ToString(), GridEditorStyle.BoxLabelSelectedToOrder);
            
            //DestLine
            GUI.DrawTexture(destLineRect, GridEditorMedia.DestLine, ScaleMode.StretchToFill);
            
            window.Repaint();
        }

        #endregion


        #region Execute Order

        private void ExecuteOrderColumn()
        {
            int fromGPosX = selectedGPosX;
            int toGPosX = destGPosX;
            window.listEditGridAction.Add(() => gridSp.MoveColumnToAfterIndex(fromGPosX, toGPosX));
            
            
            selectedGPosX = GPosEmpty;
            destGPosX = GPosEmpty;
        }
        
        private void ExecuteOrderRow()
        {
            int fromGPosY = selectedGPosY;
            int toGPosY = destGPosY;
            window.listEditGridAction.Add(() => gridSp.MoveRowToAfterIndex(fromGPosY, toGPosY));
            
            selectedGPosY = GPosEmpty;
            destGPosY = GPosEmpty;
        }

        #endregion
        
    }
}