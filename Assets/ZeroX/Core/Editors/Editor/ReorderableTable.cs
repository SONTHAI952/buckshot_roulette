using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ZeroX.Editors
{
    public partial class ReorderableTable
    {
        public delegate void CellCallbackDelegate(Rect rect, int rowIndex, int columnIndex, bool isActive, bool isFocused);
        public delegate void ColumnHeaderCallbackDelegate(Rect rect, int columnIndex);
        public delegate int TotalColumnCallbackDelegate();
        public delegate float RowHeightCallbackDelegate();
    }
    
    public partial class ReorderableTable
    {
        public float columnSpace = 10;
        public float firstColumnWidthDefault = 100;
        public float columnWidthDefault = 200f;

        //Cần thiết
        public float tablePositionY = 0; //Vị trí Y bắt đầu của table. Cần thiết để tính đc vị trí resize
        public float scrollViewWidth = 250;
        
        public ReorderableList reorderableList { get; private set; }
        public List<float> listColumnWidth = new List<float>();
        List<Rect> listColumnHeaderSpaceRect = new List<Rect>();
        
        Vector2 m_scrollPos = Vector2.zero;
        private int mouseDownOnResizeColumnIndex = 0;


        public ColumnHeaderCallbackDelegate drawColumnHeaderCallback;
        public CellCallbackDelegate drawCellCallBack;
        public TotalColumnCallbackDelegate totalColumnCallback;

        public ReorderableTable(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
        {
            reorderableList = new ReorderableList(serializedObject, elements, draggable, displayHeader, displayAddButton, displayRemoveButton);
            reorderableList.drawHeaderCallback += DrawHeaderCallback;
            reorderableList.drawElementCallback += DrawElementCallback;
        }

        public ReorderableTable(IList elements, Type elementType, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
        {
            reorderableList = new ReorderableList(elements, elementType, draggable, displayHeader, displayAddButton, displayRemoveButton);
            reorderableList.drawHeaderCallback += DrawHeaderCallback;
            reorderableList.drawElementCallback += DrawElementCallback;
        }

        int GetTotalColumn()
        {
            if (totalColumnCallback == null)
                return 0;
            else
                return totalColumnCallback.Invoke();
        }

        private void DrawHeaderCallback(Rect rect)
        {
            listColumnHeaderSpaceRect.Clear();
            
            int totalColumn = GetTotalColumn();
            if(totalColumn == 0)
                return;

            Rect columnRect = new Rect();
            columnRect.x = rect.x + 12;
            columnRect.y = rect.y;
            columnRect.width = GetColumnWidth(0);
            columnRect.height = rect.height;
            drawColumnHeaderCallback?.Invoke(columnRect, 0);


            //Lưu lại rect space
            Rect columnHeaderSpaceRect = new Rect(columnRect.xMax - m_scrollPos.x, tablePositionY, columnSpace, columnRect.height);
            listColumnHeaderSpaceRect.Add(columnHeaderSpaceRect);
            
            for (int columnIndex = 1; columnIndex < totalColumn; columnIndex++)
            {
                columnRect.x = columnRect.xMax + columnSpace;
                columnRect.width = GetColumnWidth(columnIndex);
                drawColumnHeaderCallback?.Invoke(columnRect, columnIndex);
                
                //Lưu lại rect space
                columnHeaderSpaceRect = new Rect(columnRect.xMax - m_scrollPos.x, tablePositionY, columnSpace, columnRect.height);
                listColumnHeaderSpaceRect.Add(columnHeaderSpaceRect);
            }
        }
        
        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            int totalColumn = GetTotalColumn();
            if(totalColumn == 0)
                return;

            Rect columnRect = new Rect();
            columnRect.x = rect.x;
            columnRect.y = rect.y;
            columnRect.width = GetColumnWidth(0);
            columnRect.height = rect.height;
            drawCellCallBack?.Invoke(columnRect, index, 0, isactive, isfocused);
            
            for (int columnIndex = 1; columnIndex < totalColumn; columnIndex++)
            {
                columnRect.x = columnRect.xMax + columnSpace;
                columnRect.width = GetColumnWidth(columnIndex);
                drawCellCallBack?.Invoke(columnRect, index, columnIndex, isactive, isfocused);
            }
        }

        #region Column Width

        public float GetColumnWidth(int columnIndex)
        {
            if (columnIndex >= listColumnWidth.Count)
            {
                if (columnIndex == 0)
                    return firstColumnWidthDefault;
                else
                {
                    return columnWidthDefault;
                }
            }
            else
            {
                return listColumnWidth[columnIndex];
            }
        }
        
        public void SetColumnWidth(int columnIndex, float width)
        {
            if (width < 50)
                width = 50;
            
            if (columnIndex >= listColumnWidth.Count)
            {
                int them = columnIndex + 1 - listColumnWidth.Count;
                for (int i = 0; i < them; i++)
                {
                    if(listColumnWidth.Count == 0)
                        listColumnWidth.Add(firstColumnWidthDefault);
                    else
                    {
                        listColumnWidth.Add(columnWidthDefault);
                    }
                }
            }

            listColumnWidth[columnIndex] = width;
        }
        
        public float GetTotalColumnWidth()
        {
            int totalColumn = GetTotalColumn();
            float width = 12;
            for (int i = 0; i < totalColumn; i++)
            {
                width += GetColumnWidth(i) + columnSpace;
            }
            width += 25;
            return width;
        }

        #endregion

        public void DoLayoutList()
        {
            m_scrollPos = GUILayout.BeginScrollView(m_scrollPos, GUILayout.Width(scrollViewWidth), GUILayout.ExpandHeight(false));
            reorderableList.DoLayoutList();
            float totalWidth = GetTotalColumnWidth();
            GUILayout.Label("", GUILayout.Width(totalWidth), GUILayout.Height(0));
            GUILayout.EndScrollView();
            
            HandleEvent();
        }

        public void DoList(Rect rect)
        {
            reorderableList.DoList(rect);
            HandleEvent();
        }

        void HandleEvent()
        {
            foreach (var rect in listColumnHeaderSpaceRect)
            {
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeLeftRight);
            }
            
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown:
                {
                    for (int i = 0; i < listColumnHeaderSpaceRect.Count; i++)
                    {
                        if (listColumnHeaderSpaceRect[i].Contains(e.mousePosition))
                            mouseDownOnResizeColumnIndex = i;
                    }
                    break;
                }
                case EventType.MouseUp:
                {
                    mouseDownOnResizeColumnIndex = -1;
                    break;
                }
                case EventType.MouseDrag:
                {
                    if(mouseDownOnResizeColumnIndex < 0)
                        break;

                    float width = GetColumnWidth(mouseDownOnResizeColumnIndex);
                    width += e.delta.x;
                    SetColumnWidth(mouseDownOnResizeColumnIndex, width);
                    break;
                }
            }
        }
    }
}