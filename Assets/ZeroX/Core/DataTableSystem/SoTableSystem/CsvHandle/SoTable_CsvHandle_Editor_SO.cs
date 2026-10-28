#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using ZeroX.DataTableSystem.Editors;

namespace ZeroX.DataTableSystem.SoTableSystem
{
    public abstract partial class SoTable<TTable, TRow>
    {
        private string EditorSO_GenerateRowFolderPath()
        {
            string tableFilePath = AssetDatabase.GetAssetPath(this);
            string tableFileName = Path.GetFileNameWithoutExtension(tableFilePath);
            string tableFolderPath = Path.GetDirectoryName(tableFilePath);

            string rowFolderParentPath = Path.Combine(tableFolderPath, tableFileName);
            string rowFolderPath = Path.Combine(rowFolderParentPath, "Rows");
            
            if (AssetDatabase.IsValidFolder(rowFolderParentPath) == false)
                AssetDatabase.CreateFolder(tableFolderPath, tableFileName);
            
            if (AssetDatabase.IsValidFolder(rowFolderPath) == false)
                AssetDatabase.CreateFolder(rowFolderParentPath, "Rows");

            return rowFolderPath;
        }

        /// <summary>
        /// Không bao gồm extension .asset
        /// </summary>
        private string EditorSO_GenerateRowName(TRow row, int rowIndex, SORowNameStyle nameStyle)
        {
            if (nameStyle == SORowNameStyle.Index)
            {
                return "row_" + rowIndex;
            }

            if (nameStyle == SORowNameStyle.Id)
            {
                var listHierarchyId = GetListHierarchyId(row);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < listHierarchyId.Count; i++)
                {
                    sb.Append(listHierarchyId[i].ToString().ToLower());
                    if(i < listHierarchyId.Count - 1)
                        sb.Append("_");
                }
                
                return sb.ToString();
            }
            
            Debug.LogError("Not code for SO row name style: " + nameStyle);
            return "row_" + rowIndex;
        }
        

        private class EditorSO_ImportFromCsv_Context
        {
            private SoTable<TTable, TRow> table;
            private List<TRow> poolRow;
            public readonly List<TRow> newListRow = new List<TRow>();
            public readonly List<int> listRowIndexNeedCreateAsset = new List<int>();
            private Dictionary<Type, TRow> dictTempRow = new Dictionary<Type, TRow>();


            public void Init(SoTable<TTable, TRow> table)
            {
                this.table = table;
                poolRow = CreatePoolRowFromListRow();
            }


            #region Pool
            
            private List<TRow> CreatePoolRowFromListRow()
            {
                poolRow = new List<TRow>();

                for (int i = 0; i < table.listRow.Count; i++)
                {
                    var row = table.listRow[i];
                    
                    if(row == null)
                        continue;

                    if (row.Equals(null))
                    {
                        string path = AssetDatabase.GetAssetPath((ScriptableObject) (object) row);
                        AssetDatabase.DeleteAsset(path);
                        
                        table.listRow.RemoveAt(i);
                        i--;
                        continue;
                    }
                
                    poolRow.Add(row);
                }
            
                return poolRow;
            }
            
            public void DeleteAllRowInPool()
            {
                //Xóa các row trong pool không dùng đến
                foreach (var rowInPool in poolRow)
                {
                    string path = AssetDatabase.GetAssetPath((ScriptableObject) (object) rowInPool);
                    AssetDatabase.DeleteAsset(path);
                }
            }
            

            /// <summary>
            /// Lấy ra và đồng thời xóa trong pool
            /// </summary>
            public bool TryGetOutRowInPool(Type rowType, TRow tempRow, out TRow rowInPool)
            {
                for (int i = 0; i < poolRow.Count; i++)
                {
                    var row = poolRow[i];
                    if (row.GetType() == rowType && table.IsTwoRowSameId(row, tempRow))
                    {
                        poolRow.RemoveAt(i);
                        rowInPool = row;
                        return true;
                    }
                }

                rowInPool = default;
                return false;
            }

            #endregion



            #region Row Index Need Create Asset

            public void AddRowIndexNeedCreateAsset(int index)
            {
                listRowIndexNeedCreateAsset.Add(index);
            }

            /// <summary>
            /// Dùng trong trường họp không thể hoàn thành tác vụ import do có lỗi xảy ra
            /// </summary>
            private void DestroyAllRowNeedCreateAsset()
            {
                foreach (var rowIndex in listRowIndexNeedCreateAsset)
                {
                    var row = newListRow[rowIndex];
                    DestroyImmediate((ScriptableObject)(object)row);
                }
            }

            #endregion



            #region Temp Row

            public TRow ForceGetTempRow(Type rowType)
            {
                if (dictTempRow.TryGetValue(rowType, out TRow tempRow) == false)
                {
                    tempRow = (TRow)(object)ScriptableObject.CreateInstance(rowType);
                    dictTempRow.Add(rowType, tempRow);
                }

                return tempRow;
            }

            public void DestroyAllTempRow()
            {
                foreach (var tempRow in dictTempRow.Values)
                {
                    DestroyImmediate((ScriptableObject)(object)tempRow);
                }
            }
            
            #endregion
            
            
            public void CleanWhenException()
            {
                DestroyAllRowNeedCreateAsset();
                DestroyAllTempRow();
            }
        }

        
            
            
        

        private void EditorSO_ImportFromCsv(List<string> listFieldName, List<List<string>> listCsvRow)
        {
            var listRowType = CreateListRowType(listFieldName, listCsvRow, out int indexOfRowTypeField);
            
            Undo.RecordObject(this, "ImportFromCsvData");
            
            string rowFolderPath = EditorSO_GenerateRowFolderPath();
            EditorSO_ImportFromCsv_Context context = new EditorSO_ImportFromCsv_Context();
            context.Init(this);
           



            //Ưu tiên dùng lại các row trong pool cùng type và id
            for (int rowIndex = 0; rowIndex < listCsvRow.Count; rowIndex++)
            {
                var csvRow = listCsvRow[rowIndex];
                var rowType = listRowType[rowIndex];
                
                
                //Tạo tempRow để so sánh Id. Vì mỗi loại row có một cách lấy id riêng, nên để lấy được id của csv thì chỉ có fill thử vào tempRow rồi lấy id
                TRow tempRow = default;
                try
                {
                    tempRow = context.ForceGetTempRow(rowType);
                    FillCsvRowToRow(listFieldName, csvRow, tempRow, rowType, rowIndex, indexOfRowTypeField);
                }
                catch (InvalidCastException e)
                {
                    context.CleanWhenException();
                    throw new InvalidCastException(string.Format("Cannot cast type '{0}' to type '{1}'", rowType.Name, typeof(TRow).Name));
                }
                
                
                
                //Lấy ra row từ pool
                if (context.TryGetOutRowInPool(rowType, tempRow, out TRow row))
                {
                    var soRow = (ScriptableObject)(object)row;
                    Undo.RecordObject(soRow, "FillCsvRowToRow");
                    
                    FillCsvRowToRow(listFieldName, csvRow, row, rowType, rowIndex, indexOfRowTypeField);
                    context.newListRow.Add(row);
                    
                    EditorUtility.SetDirty(soRow);
                }
                else
                {
                    //Nếu không có trong pool thì tạo mới nhưng chưa create asset vội để tránh replace vào row cũ
                    try
                    {
                        row = (TRow)(object)ScriptableObject.CreateInstance(rowType);
                    }
                    catch (InvalidCastException e)
                    {
                        context.CleanWhenException();
                        throw new InvalidCastException(string.Format("Cannot cast type '{0}' to type '{1}'", rowType.Name, typeof(TRow).Name));
                    }
                    
                    context.AddRowIndexNeedCreateAsset(rowIndex);
                    FillCsvRowToRow(listFieldName, csvRow, row, rowType, rowIndex, indexOfRowTypeField);
                    context.newListRow.Add(row);
                }
            }
            
            
            //Xóa các tempRow và row còn thừa trong pool
            context.DestroyAllTempRow();
            context.DeleteAllRowInPool();
            
            
            
            //Tạo các row asset mới từ đây
            var rowNameStyle = TableToolSetting.Import.SORowNameStyle;
            foreach (var rowIndex in context.listRowIndexNeedCreateAsset)
            {
                var row = context.newListRow[rowIndex];
                
                string rowName = EditorSO_GenerateRowName(row, rowIndex, rowNameStyle);
                string rowFilePath = Path.Combine(rowFolderPath, rowName + ".asset");
                int duplicateIndex = 0;
                while (File.Exists(rowFilePath))
                {
                    duplicateIndex++;
                    rowFilePath = Path.Combine(rowFolderPath, string.Format("{0} ({1}).asset", rowName, duplicateIndex));
                }
                AssetDatabase.CreateAsset((UnityEngine.Object)(object)row, rowFilePath);
            }
            
            
            
            //Commit thay đổi vào row
            listRow.Clear();
            listRow.AddRange(context.newListRow);

            if (editor_ListRow != null)
            {
                editor_ListRow.Clear();
                editor_ListRow.AddRange(context.newListRow);
            }
            
            
            //Save table and refresh
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

#endif