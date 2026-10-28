using UnityEditor;
using UnityEngine;


namespace ZeroX.ZGridEditors
{
    public static class GridEditorPref
    {
        public static class Key
        {
            public const string Root = "ZeroX.Variables.Grid.";
            
            public const string CellSizeX_Root = Root + "CellSize.X.";
            public const string CellSizeY_Root = Root + "CellSize.Y.";
            
            public const string CellLabelWidth_Root = Root + "CellLabelWidth.";
            public const string EnableCellLabel_Root = Root + "EnableCellLabel.";

            public const string GridPivot_Root = Root + "GridPivot.";
        }
        
        

        public static Vector2 GetCellSize(GridSp gridSp)
        {
            string cellTypeName = gridSp.CellType.FullName;
            string keyX = Key.CellSizeX_Root + cellTypeName;
            string keyY = Key.CellSizeY_Root + cellTypeName;
            
            Vector2 cellSize = Vector2.zero;
            cellSize.x = EditorPrefs.GetFloat(keyX, 150);
            cellSize.y = EditorPrefs.GetFloat(keyY, 150);

            return cellSize;
        }

        public static void SetCellSize(GridSp gridSp, Vector2 cellSize)
        {
            string cellTypeName = gridSp.CellType.FullName;
            string keyX = Key.CellSizeX_Root + cellTypeName;
            string keyY = Key.CellSizeY_Root + cellTypeName;
            
            EditorPrefs.SetFloat(keyX, cellSize.x);
            EditorPrefs.SetFloat(keyY, cellSize.y);
        }
        
        

        public static float GetCellLabelWidth(GridSp gridSp)
        {
            string cellTypeName = gridSp.CellType.FullName;
            string key = Key.CellLabelWidth_Root + cellTypeName;
            
            return EditorPrefs.GetFloat(key, 80);
        }

        public static void SetCellLabelWidth(GridSp gridSp, float labelWidth)
        {
            string cellTypeName = gridSp.CellType.FullName;
            string key = Key.CellLabelWidth_Root + cellTypeName;
            EditorPrefs.SetFloat(key, labelWidth);
        }

        public static bool GetEnableCellLabel(GridSp gridSp)
        {
            string cellTypeName = gridSp.CellType.FullName;
            string key = Key.EnableCellLabel_Root + cellTypeName;
            
            return EditorPrefs.GetBool(key, true);
        }
        
        public static void SetEnableCellLabel(GridSp gridSp, bool enable)
        {
            string cellTypeName = gridSp.CellType.FullName;
            string key = Key.EnableCellLabel_Root + cellTypeName;
            
            EditorPrefs.SetBool(key, enable);
        }
        
        public static GridPivotType GetGridPivotType(GridSp gridSp)
        {
            string gridTypeName = gridSp.CellType.FullName;
            string key = Key.GridPivot_Root + gridTypeName;
            
            return (GridPivotType)EditorPrefs.GetInt(key, (int)GridPivotType.TopLeft);
        }
        
        public static void SetGridPivotType(GridSp gridSp, GridPivotType pivotType)
        {
            string gridTypeName = gridSp.CellType.FullName;
            string key = Key.GridPivot_Root + gridTypeName;
            
            EditorPrefs.SetInt(key, (int)pivotType);
        }
    }
}