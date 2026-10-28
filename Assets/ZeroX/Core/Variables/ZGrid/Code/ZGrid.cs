using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX
{
    [System.Serializable]
    public class ZGrid<TCell> : ZGridBase
    {
        [System.Serializable]
        public class Row
        {
            public List<TCell> listCell = new List<TCell>();


            public TCell this[int gPosX]
            {
                get =>listCell[gPosX];
                set => listCell[gPosX] = value;
            }
            

            
            public Row()
            {
            }

            public Row(int width)
            {
                for (int x = 0; x < width; x++)
                {
                    listCell.Add(default);
                }
            }
        }



        
        //SerializeField
        [SerializeField] private List<Row> listRow = new List<Row>();
        
        
        //Property
        public int Width => listRow.Count == 0 ? 0 : listRow[0].listCell.Count;
        public int Height => listRow.Count;
        public TCell this[int gPosX, int gPosY]
        {
            get => listRow[gPosY].listCell[gPosX];
            set => listRow[gPosY].listCell[gPosX] = value;
        }


        //Constructor
        public ZGrid()
        {
        }
        
        public ZGrid(int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                Row row = new Row(width);
                listRow.Add(row);
            }
        }
        
        
        #region Row Width

        public void AddRow(int numberAdd = 1)
        {
            int width = Width;

            for (int i = 0; i < numberAdd; i++)
            {
                Row row = new Row(width);
                listRow.Add(row);
            }
        }

        public void InsertRow(int gPosY)
        {
            Row row = new Row(Width);
            listRow.Insert(gPosY, row);
        }

        public void RemoveRow(int gPosY)
        {
            listRow.RemoveAt(gPosY);
        }

        public void RemoveLastRow(int numberRemove = 1)
        {
            listRow.RemoveRange(listRow.Count - numberRemove, numberRemove);
        }
        
        public void SetHeight(int newHeight)
        {
            int height = Height;
            
            if(newHeight == height)
                return;

            if (newHeight > height)
            {
                AddRow(newHeight - height);
            }
            else
            {
                RemoveLastRow(height - newHeight);
            }
        }

        public void SwapRow(int gPosYA, int gPosYB)
        {
            var rowA = listRow[gPosYA];
            listRow[gPosYA] = listRow[gPosYB];
            listRow[gPosYB] = rowA;
        }

        /// <summary>
        /// Logic tương tự SerializedProperty.MoveArrayElement
        /// </summary>
        public void MoveRow(int fromGPosY, int toGPosY)
        {
            var row = listRow[fromGPosY];
            listRow.RemoveAt(fromGPosY);
            listRow.Insert(toGPosY, row);
        }

        #endregion
        
        
        
        #region Column - Width
        
        /// <summary>
        /// Tăng width. Nếu height == 0 thì height sẽ tự động được tăng lên 1 để có cell
        /// </summary>
        public void AddColumn(int numberAdd = 1)
        {
            //Cần có ít nhất một row để có cell
            if(listRow.Count == 0)
                AddRow();
            
            
            for (int y = 0; y < listRow.Count; y++)
            {
                var row = listRow[y];
                for (int i = 0; i < numberAdd; i++)
                {
                    row.listCell.Add(default);
                }
            }
        }

        //Nếu không outOfIndex và height == 0 thì height sẽ tự động được tăng lên 1 để có cell
        public void InsertColumn(int gPosX)
        {
            if (gPosX < 0 || gPosX > Width)
                throw new IndexOutOfRangeException();
            
            
            if(listRow.Count == 0)
                AddRow();
            
            
            for (int y = 0; y < listRow.Count; y++)
            {
                var row = listRow[y];
                row.listCell.Insert(gPosX, default);
            }
        }

        public void RemoveColumn(int gPosX)
        {
            for (int y = 0; y < listRow.Count; y++)
            {
                var row = listRow[y];
                row.listCell.RemoveAt(gPosX);
            }
        }

        public void RemoveLastColumn(int numberRemove = 1)
        {
            for (int y = 0; y < listRow.Count; y++)
            {
                var row = listRow[y];
                row.listCell.RemoveRange(row.listCell.Count - numberRemove, numberRemove);
            }
        }
        
        public void SetWidth(int newWidth)
        {
            int width = Width;
            
            if(newWidth == width)
                return;
            
            if (newWidth > width)
            {
                AddColumn(newWidth - width);
            }
            else
            {
                RemoveLastColumn(width - newWidth);
            }
        }

        public void SwapColumn(int gPosXA, int gPosXB)
        {
            for (int y = 0; y < listRow.Count; y++)
            {
                var row = listRow[y];
                var cellA = row.listCell[gPosXA];
                row.listCell[gPosXA] = row.listCell[gPosXB];
                row.listCell[gPosXB] = cellA;
            }
        }

        public void MoveColumn(int fromGPosX, int toGPosX)
        {
            int height = Height;

            for (int y = 0; y < height; y++)
            {
                var row = listRow[y];
                
                var cellA = row.listCell[fromGPosX];
                row.listCell.RemoveAt(fromGPosX);
                row.listCell.Insert(toGPosX, cellA);
            }
        }
        #endregion



        public void SetSize(int width, int height)
        {
            SetWidth(width);
            SetHeight(height);
        }
    }
}