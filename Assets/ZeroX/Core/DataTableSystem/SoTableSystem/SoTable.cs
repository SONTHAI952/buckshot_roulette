using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.DataTableSystem.SoTableSystem
{
    [System.Serializable]
    public abstract class SoTable : ScriptableObject
    {
    }
    
    
    
    [System.Serializable]
    public abstract partial class SoTable<TTable, TRow> : SoTable
        where TTable : SoTable<TTable, TRow>, new()
    {
        
        
        //Fields
        [SerializeField] private List<TRow> listRow = new List<TRow>();
        
        
#if UNITY_EDITOR
        //Trên editor thì sử dụng 1 listRow clone để tránh thao tác thêm bớt trong runtime sẽ ảnh hưởng đến listRow gốc
        [System.NonSerialized] private List<TRow> editor_ListRow;
        
        public List<TRow> RowsI
        {
            get
            {
                if (editor_ListRow == null)
                    editor_ListRow = listRow.ToList(); 

                return editor_ListRow;
            }
            
            set => editor_ListRow = value;
        }
#else
        public List<TRow> RowsI
        {
            get => listRow;
            set => listRow = value;
        }
#endif

        
        
#if UNITY_EDITOR
        /// <summary>
        /// Chỉ sử dụng khi cần truy cập đến list row thật của Table trên editor. Vì RowsI/Row khi gọi trên Editor là row đã clone
        /// </summary>
        public List<TRow> RawRowsI
        {
            get => listRow;
            set => listRow = value;
        }
#endif

        
        


        #region Non Public
        
        /// <summary>
        /// Đường dẫn đầy đủ, là đường dẫn tới file trong Resources
        /// </summary>
        protected virtual string FullMainPath => "";

        /// <summary>
        /// Sẽ được cộng thêm Database/ vào phía trước. Vì vậy không cần bao gồm Database/ trong tên
        /// </summary>
        protected virtual string ShortMainPath => "";
        
        protected string GetMainPath()
        {
            if(string.IsNullOrEmpty(FullMainPath) == false)
                return FullMainPath;
            
            
            if(string.IsNullOrEmpty(ShortMainPath) == false)
                return "Database/" + ShortMainPath;
            
            
            //Sử dụng tên script làm tên table
            return "Database/" + typeof(TTable).Name;
        }
        
        protected virtual TTable LoadMain()
        {
            string mainPath = GetMainPath();
            var table = Resources.Load<TTable>(mainPath);
            if(table == null)
                Debug.LogError($"Load main table '{typeof(TTable).Name}' failed at path: " + mainPath);

            return table;
        }
        
        protected virtual bool IsTwoRowSameId(TRow rowA, TRow rowB)
        {
            return true;
        }

        protected virtual List<object> GetListHierarchyId(TRow row)
        {
            return new List<object>();
        }

        #endregion



        
        
        #region Public
        
        public void SetListRowI(List<TRow> newListRow)
        {
            RowsI = newListRow;
        }
        
        protected virtual void OnAfterRowsChanged()
        {
            
        }
        
        public abstract void ClearCacheDictRowI();
        
        #endregion



        

        #region Static
        
        
        //Table Api
        private static TTable main;

        public static TTable Main
        {
            get
            {
                if (main == null)
                {
                    var instance = ScriptableObject.CreateInstance<TTable>();
                    main = instance.LoadMain();
                    
                    
#if UNITY_EDITOR
                    if(UnityEditor.EditorApplication.isPlaying)
                        Destroy(instance);
                    else
                        DestroyImmediate(instance);
#else
                        Destroy(instance);
#endif
                }

                return main;
            }
        }
        
        public static void PreloadMain()
        {
            var m = Main;
        }

        public static void SetMain(TTable newMain)
        {
            main = newMain;
        }

        public static void ClearCacheDictRow()
        {
            Main.ClearCacheDictRowI();
        }
        
        //Row Api
        public static List<TRow> Rows => Main.RowsI;
        
        #if UNITY_EDITOR
        /// <summary>
        /// Chỉ sử dụng khi cần truy cập đến list row thật của Table trên editor. Vì RowsI/Row khi gọi trên Editor là row đã clone
        /// </summary>
        public static List<TRow> RawRows => Main.RawRowsI;
        #endif
        
        public static void SetListRow(List<TRow> newListRow)
        {
            Main.SetListRowI(newListRow);
        }
        
        public static void UpdateAfterListRowChange()
        {
            Main.OnAfterRowsChanged();
        }
        
        #endregion
    }
}