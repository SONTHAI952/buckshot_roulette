using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroX.DataTableSystem.SoTableSystem
{
    public abstract class SoTableFourId<TTable, TRow, TId1, TId2, TId3, TId4> : SoTable<TTable, TRow>
        where TTable : SoTableFourId<TTable, TRow, TId1, TId2, TId3, TId4>, new()
    {
        
        
        
        //Fields
        protected Dictionary<TId1, Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>>> dictRow;



        
        
        #region Non Public
        
        protected abstract void GetRowId(TRow row, out TId1 id1, out TId2 id2, out TId3 id3, out TId4 id4);
        
        private void GenerateDictRowFromListRow()
        {
            if (dictRow == null)
                dictRow = new Dictionary<TId1, Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>>>();
            else
                dictRow.Clear();
            
            
            
            foreach (var row in RowsI)
            {
                GetRowId(row, out TId1 id1, out TId2 id2, out TId3 id3, out TId4 id4);
                
                if (dictRow.TryGetValue(id1, out var dict2) == false) //Nếu trong dict chưa có id1
                {
                    dict2 = new Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>>();
                    dictRow.Add(id1, dict2);
                }

                if (dict2.TryGetValue(id2, out var dict3) == false)
                {
                    dict3 = new Dictionary<TId3, Dictionary<TId4, TRow>>();
                    dict2.Add(id2, dict3);
                }
                
                if (dict3.TryGetValue(id3, out var dict4) == false)
                {
                    dict4 = new Dictionary<TId4, TRow>();
                    dict3.Add(id3, dict4);
                }
                
                dict4.Add(id4, row);
            }
        }
        
        protected override bool IsTwoRowSameId(TRow rowA, TRow rowB)
        {
            GetRowId(rowA, out var rowA_Id1, out var rowA_Id2, out var rowA_Id3, out var rowA_Id4);
            GetRowId(rowB, out var rowB_Id1, out var rowB_Id2, out var rowB_Id3, out var rowB_Id4);

            return rowA_Id1.Equals(rowB_Id1) &&
                   rowA_Id2.Equals(rowB_Id2) &&
                   rowA_Id3.Equals(rowB_Id3) &&
                   rowA_Id4.Equals(rowB_Id4);
        }
        
        protected override List<object> GetListHierarchyId(TRow row)
        {
            GetRowId(row, out var id1, out var id2, out var id3, out var id4);
            
            List<object> list = new List<object>();
            list.Add(id1);
            list.Add(id2);
            list.Add(id3);
            list.Add(id4);

            return list;
        }
        
        #endregion




        
        #region Public
        
        public Dictionary<TId1, Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>>> DictRowI
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
        
        public Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>> GetDictRowByIdI(TId1 id1)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            return dict2;
        }
        
        public Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>> GetDictRowByIdWithLogI(TId1 id1)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }

            return dict2;
        }
        
        public Dictionary<TId3, Dictionary<TId4, TRow>> GetDictRowByIdI(TId1 id1, TId2 id2)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            if (dict2.TryGetValue(id2, out var dict3) == false)
                return null;

            return dict3;
        }
        
        public Dictionary<TId3, Dictionary<TId4, TRow>> GetDictRowByIdWithLogI(TId1 id1, TId2 id2)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }
            
            if (dict2.TryGetValue(id2, out var dict3) == false)
            {
                Debug.LogError("Id2 not exist: " + id2);
                return null;
            }

            return dict3;
        }
        
        public Dictionary<TId4, TRow> GetDictRowByIdI(TId1 id1, TId2 id2, TId3 id3)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            if (dict2.TryGetValue(id2, out var dict3) == false)
                return null;

            if (dict3.TryGetValue(id3, out var dict4) == false)
                return null;

            return dict4;
        }
        
        public Dictionary<TId4, TRow> GetDictRowByIdWithLogI(TId1 id1, TId2 id2, TId3 id3)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }
            
            if (dict2.TryGetValue(id2, out var dict3) == false)
            {
                Debug.LogError("Id2 not exist: " + id2);
                return null;
            }
            
            if (dict3.TryGetValue(id3, out var dict4) == false)
            {
                Debug.LogError("Id3 not exist: " + id2);
                return null;
            }

            return dict4;
        }
        
        public List<TRow> GetListRowByIdI(TId1 id1)
        {
            var dict2 = GetDictRowByIdI(id1);
            if(dict2 == null)
                return new List<TRow>();

            List<TRow> listRow = new List<TRow>();
            foreach (var dict3 in dict2.Values)
            {
                foreach (var dict4 in dict3.Values)
                {
                    listRow.AddRange(dict4.Values.ToList());
                }
            }
            
            return listRow;
        }
        
        public List<TRow> GetListRowByIdI(TId1 id1, TId2 id2)
        {
            var dict3 = GetDictRowByIdI(id1, id2);
            if(dict3 == null)
                return new List<TRow>();

            List<TRow> listRow = new List<TRow>();
            foreach (var dict4 in dict3.Values)
            {
                listRow.AddRange(dict4.Values.ToList());
            }
            
            return listRow;
        }
        
        public List<TRow> GetListRowByIdI(TId1 id1, TId2 id2, TId3 id3)
        {
            var dict4 = GetDictRowByIdI(id1, id2, id3);
            if(dict4 == null)
                return new List<TRow>();

            List<TRow> listRow = dict4.Values.ToList();
            return listRow;
        }
        
        #endregion




        
        #region Static
        
        public Dictionary<TId1, Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>>> DictRow => Main.DictRowI;
        
        
        
        //Get
        public static Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>> GetDictRowById(TId1 id1)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            return dict2;
        }
        
        public static Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>> GetDictRowByIdWithLog(TId1 id1)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }

            return dict2;
        }
        
        public static Dictionary<TId3, Dictionary<TId4, TRow>> GetDictRowById(TId1 id1, TId2 id2)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            if (dict2.TryGetValue(id2, out var dict3) == false)
                return null;

            return dict3;
        }
        
        public static Dictionary<TId3, Dictionary<TId4, TRow>> GetDictRowByIdWithLog(TId1 id1, TId2 id2)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }
            
            if (dict2.TryGetValue(id2, out var dict3) == false)
            {
                Debug.LogError("Id2 not exist: " + id2);
                return null;
            }

            return dict3;
        }
        
        public static Dictionary<TId4, TRow> GetDictRowById(TId1 id1, TId2 id2, TId3 id3)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            if (dict2.TryGetValue(id2, out var dict3) == false)
                return null;

            if (dict3.TryGetValue(id3, out var dict4) == false)
                return null;

            return dict4;
        }
        
        public static Dictionary<TId4, TRow> GetDictRowByIdWithLog(TId1 id1, TId2 id2, TId3 id3)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }
            
            if (dict2.TryGetValue(id2, out var dict3) == false)
            {
                Debug.LogError("Id2 not exist: " + id2);
                return null;
            }
            
            if (dict3.TryGetValue(id3, out var dict4) == false)
            {
                Debug.LogError("Id3 not exist: " + id2);
                return null;
            }

            return dict4;
        }
        
        public static List<TRow> GetListRowById(TId1 id1)
        {
            var dict2 = Main.GetDictRowByIdI(id1);
            if(dict2 == null)
                return new List<TRow>();

            List<TRow> listRow = new List<TRow>();
            foreach (var dict3 in dict2.Values)
            {
                foreach (var dict4 in dict3.Values)
                {
                    listRow.AddRange(dict4.Values.ToList());
                }
            }
            
            return listRow;
        }
        
        public static List<TRow> GetListRowById(TId1 id1, TId2 id2)
        {
            var dict3 = Main.GetDictRowByIdI(id1, id2);
            if(dict3 == null)
                return new List<TRow>();

            List<TRow> listRow = new List<TRow>();
            foreach (var dict4 in dict3.Values)
            {
                listRow.AddRange(dict4.Values.ToList());
            }
            
            return listRow;
        }
        
        public static List<TRow> GetListRowById(TId1 id1, TId2 id2, TId3 id3)
        {
            var dict4 = Main.GetDictRowByIdI(id1, id2, id3);
            if(dict4 == null)
                return new List<TRow>();

            List<TRow> listRow = dict4.Values.ToList();
            return listRow;
        }
        
        #endregion
    }
}