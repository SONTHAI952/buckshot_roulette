using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.DataTableSystem.SoTableSystem
{
    public abstract partial class SoTable<TTable, TRow>
    {
        private void NonSO_ImportFromCsv(List<string> listFieldName, List<List<string>> listCsvRow)
        {
            var listRowType = CreateListRowType(listFieldName, listCsvRow, out int indexOfRowTypeField);
            var newListRow = new List<TRow>();
            
            
            for (int rowIndex = 0; rowIndex < listCsvRow.Count; rowIndex++)
            {
                var csvRow = listCsvRow[rowIndex];
                var rowType = listRowType[rowIndex];

                try
                {
                    var row = (TRow)Activator.CreateInstance(rowType);
                    newListRow.Add(row);
                
                    FillCsvRowToRow(listFieldName, csvRow, row, rowType, rowIndex, indexOfRowTypeField);
                }
                catch (InvalidCastException e)
                {
                    throw new InvalidCastException(string.Format("Cannot cast type '{0}' to type '{1}'", rowType.Name, typeof(TRow).Name));
                }
            }
            
            
            RowsI.Clear();
            RowsI.AddRange(newListRow);
            OnAfterRowsChanged();
        }
    }
}