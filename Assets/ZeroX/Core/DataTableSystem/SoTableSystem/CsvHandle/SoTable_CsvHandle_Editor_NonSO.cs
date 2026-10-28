#if UNITY_EDITOR

using System.Collections.Generic;

namespace ZeroX.DataTableSystem.SoTableSystem
{
    public abstract partial class SoTable<TTable, TRow>
    {
        private void EditorNonSO_ImportFromCsv(List<string> listField, List<List<string>> listCsvRow)
        {
            UnityEditor.Undo.RecordObject(this, "ImportFromCsvData");
            
            NonSO_ImportFromCsv(listField, listCsvRow);

            
            if (listRow != RowsI)
            {
                listRow.Clear();
                listRow.AddRange(RowsI);
            }
            
            
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}

#endif