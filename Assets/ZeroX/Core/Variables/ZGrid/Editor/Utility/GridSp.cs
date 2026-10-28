using System;
using UnityEditor;
using UnityEngine;
using ZeroX.Editors;

namespace ZeroX.ZGridEditors
{
    public class GridSp
    {
        private SerializedProperty gridSp;
        private SerializedProperty listRowSp;
        
        
        //Property
        public int Height => listRowSp.arraySize;

        public int Width
        {
            get
            {
                if (listRowSp.arraySize == 0)
                    return 0;
                
                var firstRowSp = listRowSp.GetArrayElementAtIndex(0);
                var listCellSp = firstRowSp.FindPropertyRelative(RowReflection.FieldName.listCell);
                return listCellSp.arraySize;
            }
        }

        
        public string DisplayName => gridSp.displayName;

        public readonly Type GridType;
        public readonly Type CellType;
        
        
        
        
        
        public GridSp(SerializedProperty gridSp)
        {
            this.gridSp = gridSp;
            this.listRowSp = gridSp.FindPropertyRelative(GridReflection.FieldName.listRow);
            
            GridType = gridSp.GetFieldType();
            CellType = GridType.GenericTypeArguments[0];
        }
        

        
        #region Utility

        public SerializedProperty GetListCellSp(int gPosY)
        {
            var rowSp = listRowSp.GetArrayElementAtIndex(gPosY);
            return rowSp.FindPropertyRelative(RowReflection.FieldName.listCell);
        }

        private void SetRowSize(int gPosY, int size)
        {
            var listCellSp = GetListCellSp(gPosY);
            listCellSp.arraySize = size;
        }

        #endregion
        
        
        
        #region Row - Height
        
        public void AddRow(int numberAdd = 1)
        {
            int width = Width;
            
            for (int i = 0; i < numberAdd; i++)
            {
                int index = listRowSp.arraySize;
                listRowSp.InsertArrayElementAtIndex(index);
                SetRowSize(index, width);
            }
        }

        public void InsertRow(int gPosY)
        {
            listRowSp.InsertArrayElementAtIndex(gPosY);
            SetRowSize(gPosY, Width);
        }

        public void RemoveRow(int gPosY)
        {
            listRowSp.DeleteArrayElementAtIndex(gPosY);
        }

        public void RemoveLastRow(int numberRemove = 1)
        {
            for (int i = 0; i < numberRemove; i++)
            {
                listRowSp.DeleteArrayElementAtIndex(listRowSp.arraySize - 1);
            }
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
        
        public void MoveRowToAfterIndex(int fromGPosY, int toGPosY)
        {
            if(fromGPosY == toGPosY)
                return;

            if (fromGPosY > toGPosY)
            {
                listRowSp.MoveArrayElement(fromGPosY, toGPosY);
            }
            else
            {
                listRowSp.MoveArrayElement(fromGPosY, toGPosY - 1);
            }
        }
        
        #endregion
        
        
        
        #region Column - Width
        
        /// <summary>
        /// Tăng width. Nếu height == 0 thì height sẽ tự động được tăng lên 1 để có cell
        /// </summary>
        public void AddColumn(int numberAdd = 1)
        {
            //Cần có ít nhất một row để có cell
            if(Height == 0)
                AddRow();

            
            int height = Height;
            for (int y = 0; y < height; y++)
            {
                var listCellSp = GetListCellSp(y);
                for (int i = 0; i < numberAdd; i++)
                {
                    listCellSp.InsertArrayElementAtIndex(listCellSp.arraySize);
                }
            }
        }

        //Nếu không outOfIndex và height == 0 thì height sẽ tự động được tăng lên 1 để có cell
        public void InsertColumn(int gPosX)
        {
            if (gPosX < 0 || gPosX > Width)
                throw new IndexOutOfRangeException();
            
            
            if(Height == 0)
                AddRow();
            
            
            int height = Height;
            for (int y = 0; y < height; y++)
            {
                var listCellSp = GetListCellSp(y);
                listCellSp.InsertArrayElementAtIndex(gPosX);
            }
        }

        public void RemoveColumn(int gPosX)
        {
            int height = Height;
            for (int y = 0; y < height; y++)
            {
                var listCellSp = GetListCellSp(y);
                listCellSp.DeleteArrayElementAtIndex(gPosX);
            }
        }

        public void RemoveLastColumn(int numberRemove = 1)
        {
            int height = Height;
            for (int y = 0; y < height; y++)
            {
                var listCellSp = GetListCellSp(y);
                for (int i = 0; i < numberRemove; i++)
                {
                    listCellSp.DeleteArrayElementAtIndex(listCellSp.arraySize - 1);
                }
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
        
        public void MoveColumnToAfterIndex(int fromGPosX, int toGPosX)
        {
            if(fromGPosX == toGPosX)
                return;
            
            int height = Height;

            if (fromGPosX > toGPosX)
            {
                for (int y = 0; y < height; y++)
                {
                    var listCellSp = GetListCellSp(y);
                    listCellSp.MoveArrayElement(fromGPosX, toGPosX);
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    var listCellSp = GetListCellSp(y);
                    listCellSp.MoveArrayElement(fromGPosX, toGPosX - 1);
                }
            }
        }
        
        #endregion
    }
}