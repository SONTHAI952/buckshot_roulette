using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.DataTableSystem.SoTableSystem
{
    public abstract partial class SoTable<TTable, TRow>
    {
        /// <summary>
        /// Tùy vào môi trường là editor và rowSO có phải là asset trong project không để xử lý
        /// </summary>
        private void SO_ClearRowsI()
        {
            if(RowsI.Count == 0)
                return;
            
            
#if !UNITY_EDITOR //Nếu ko phải trên editor thì xóa thoải mái
            foreach (var row in RowsI)
            {
                if(row == null)
                    continue;
                
                Destroy((ScriptableObject)(object)row);
            }
            RowsI.Clear();
            
#else
            bool editorApplicationIsPlaying = UnityEditor.EditorApplication.isPlaying;
            foreach (var row in RowsI)
            {
                if(row == null)
                    continue;

                
                string rowPath = UnityEditor.AssetDatabase.GetAssetPath((ScriptableObject) (object) row);
                if (string.IsNullOrEmpty(rowPath) == false) //Nếu đây là những rowSo tồn tại trong thư mục project thì không được xóa
                    continue;
                
                
                if(editorApplicationIsPlaying)
                    Destroy((ScriptableObject) (object) row);
                else
                    DestroyImmediate((ScriptableObject) (object) row);
            }
            
            RowsI.Clear();
#endif
        }
        
        private void SO_ImportFromCsv(List<string> listFieldName, List<List<string>> listCsvRow)
        {
            var listRowType = CreateListRowType(listFieldName, listCsvRow, out int indexOfRowTypeField);
            var newListRow = new List<TRow>();
            
            
            for (int rowIndex = 0; rowIndex < listCsvRow.Count; rowIndex++)
            {
                var csvRow = listCsvRow[rowIndex];
                var rowType = listRowType[rowIndex];

                try
                {
                    var row = (TRow)(object)ScriptableObject.CreateInstance(rowType);
                    newListRow.Add(row);
                    
                    FillCsvRowToRow(listFieldName, csvRow, row, rowType, rowIndex, indexOfRowTypeField);
                }
                catch (InvalidCastException e)
                {
                    throw new InvalidCastException(string.Format("Cannot cast type '{0}' to type '{1}'", rowType.Name, typeof(TRow).Name));
                }
            }
            
            
            //Đối với row dạng SO thì không thể cứ thế clear list được, còn phải destroy đi các rowSO.
            SO_ClearRowsI();
            
            //Giờ mới add để tránh quá trình import lỗi giữa chừng
            RowsI.AddRange(newListRow);
            OnAfterRowsChanged();
        }
    }
}