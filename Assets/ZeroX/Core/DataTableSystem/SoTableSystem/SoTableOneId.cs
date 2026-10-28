using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.DataTableSystem.SoTableSystem
{
    public abstract class SoTableOneId<TTable, TRow, TId> : SoTable<TTable, TRow>
        where TTable : SoTableOneId<TTable, TRow, TId>, new()
    {
        
        
        
        //Fields
        private Dictionary<TId, TRow> dictRow;
        
        
        
        
        
        #region Non Public
        
        protected abstract TId GetRowId(TRow row);
        
        private void GenerateDictRowFromListRow()
        {
            if (dictRow == null)
                dictRow = new Dictionary<TId, TRow>();
            else
                dictRow.Clear();

            
            
            foreach (var row in RowsI)
            {
                dictRow.Add(GetRowId(row), row);
            }
        }

        protected override bool IsTwoRowSameId(TRow rowA, TRow rowB)
        {
            return GetRowId(rowA).Equals(GetRowId(rowB));
        }

        protected override List<object> GetListHierarchyId(TRow row)
        {
            List<object> list = new List<object>();
            list.Add(GetRowId(row));

            return list;
        }
        
        #endregion


        
        
        
        #region Public
        
        public Dictionary<TId, TRow> DictRowI
        {
            get
            {
                if(dictRow == null)
                    GenerateDictRowFromListRow();

                return dictRow;
            }
        }
        
        protected override void OnAfterRowsChanged()
        {
            GenerateDictRowFromListRow();
        }

        public sealed override void ClearCacheDictRowI()
        {
            dictRow = null;
        }

        #endregion


        
        
        
        #region Public - Get
        
        public TRow GetRowByIdI(TId id)
        {
            DictRowI.TryGetValue(id, out TRow row);
            return row;
        }
        
        public TRow GetRowByIdWithLogI(TId id)
        {
            if (DictRowI.TryGetValue(id, out TRow row))
                return row;
            else
            {
                Debug.LogError("Cannot find row with id: " + id);
                return default;
            }
        }
        
        #endregion



        
        
        #region Static
        
        public static Dictionary<TId, TRow> DictRow => Main.DictRowI;
        
        public static TRow GetRowById(TId id)
        {
            Main.DictRowI.TryGetValue(id, out TRow row);
            return row;
        }
        
        public static TRow GetRowByIdWithLog(TId id)
        {
            if (Main.DictRowI.TryGetValue(id, out TRow row))
                return row;
            else
            {
                Debug.LogError("Cannot find row with id: " + id);
                return default;
            }
        }
        
        #endregion
    }
}