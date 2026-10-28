using System;
using System.Collections.Generic;
using ZeroX.JsonSystem;
using ZeroX.SaveGameSystem.Core;

namespace ZeroX.SaveGameSystem
{
    public partial class SGNode
    {
        //Fill by parent
#if UNITY_EDITOR
        public string Key { get;}
        public Database Database { get;}
        public SGNode Root { get; }
        public SGNode Parent { get;}
        public NodeType NodeType { get; private set; }
        public long NodeId { get; private set; } = SGConstant.EMPTY_NODE_ID;
#else
        [NonSerialized] public string Key;
        [NonSerialized] public Database Database;
        [NonSerialized] public SGNode Root;
        [NonSerialized] public SGNode Parent;
        [NonSerialized] public NodeType NodeType;
        [NonSerialized] public long NodeId = SGConstant.EMPTY_NODE_ID;
#endif
        
        
        
        //No Fill By Parent
#if UNITY_EDITOR
        public bool IsObjectNodeInitialized { get; private set; }
        public bool IsDeleted { get; private set; }
#else
        [NonSerialized] public bool IsObjectNodeInitialized;
        [NonSerialized] public bool IsDeleted;
#endif
        
        public bool IsRoot => Root == this;
        public bool HasNodeId => NodeId > SGConstant.EMPTY_NODE_ID;
        
        /// <summary>
        /// Đây là đang nói về một node, nó bao gồm cả kiểu dữ liệu string, decimal... Nó không giống như primitive của DotNet
        /// </summary>
        public bool IsPrimitive => NodeType != NodeType.Object;
        
        public bool IsNumber => NodeType == NodeType.Int ||
                                NodeType == NodeType.Float ||
                                NodeType == NodeType.Long ||
                                NodeType == NodeType.Decimal ||
                                NodeType == NodeType.Double ||
                                NodeType == NodeType.BigInteger;
        
        private long childKeySeq;
        public long ChildKeySeq
        {
            get
            {
                InitializeIfNot();
                return childKeySeq;
            }
            set => childKeySeq = value;
        }

        public int ChildCount
        {
            get
            {
                InitializeIfNot();
                return dictChildNode.Count;
            }
        }
        
        
        
        
        
        #region Constructor

        /// <summary>
        /// Only call by database
        /// </summary>
        internal SGNode(string key, Database database, SGNode parent, NodeType nodeType, long nodeId)
        {
            this.Key = key;
            this.Database = database;
            this.Root = this;
            this.Parent = parent;
            this.NodeType = nodeType;
            this.NodeId = nodeId;
        }
        
        internal SGNode(string key, Database database, SGNode parent, NodeType nodeType)
        {
            this.Key = key;
            this.Database = database;
            this.Root = this;
            this.Parent = parent;
            this.NodeType = nodeType;
        }

        internal SGNode(string key, Database database, SGNode root, SGNode parent, NodeType nodeType, long nodeId)
        {
            this.Key = key;
            this.Database = database;
            this.Root = root;
            this.Parent = parent;
            this.NodeType = nodeType;
            this.NodeId = nodeId;
        }
        
        internal SGNode(string key, Database database, SGNode root, SGNode parent, NodeType nodeType)
        {
            this.Key = key;
            this.Database = database;
            this.Root = root;
            this.Parent = parent;
            this.NodeType = nodeType;
        }
        #endregion
        
        

        private void InitializeIfNot()
        {
            lock (Database)
            {
                if (IsObjectNodeInitialized)
                    return;

                if (HasNodeId == false) //Không có id thì lấy gì mà load
                {
                    SetupDefault();
                    IsObjectNodeInitialized = true;
                    return;
                }
            
                ZJsonObject jsonObject = Database.StorageService.LoadNodeDataJson(NodeId);
                Initialize_FromJsonNode(jsonObject);
            
                IsObjectNodeInitialized = true;
            }
        }

        private void SetupDefault()
        {
            ChildKeySeq = SGConstant.EMPTY_CHILD_KEY_SEQ;
            dictChildNode = new Dictionary<string, SGNode>();
            listChildKey = new List<string>();
            //dictChildKeyToIndex = new Dictionary<string, int>();
        }

        public void Delete()
        {
            if(Parent == null)
                Database.DeleteChild(Key);
            else
            {
                Parent.DeleteChild(Key);
            }
        }
        

        #region Transaction

        /// <summary>
        /// Begins a new transaction. Call <see cref="Commit" /> to end the transaction.
        /// </summary>
        public void BeginTransaction()
        {
            //Chưa làm :)))
            //Trong khi transaction sẽ chưa SetChanges cho đến khi commit
        }

        /// <summary>
        /// Commits the transaction that was begun by <see cref="BeginTransaction" />.
        /// </summary>
        public void Commit()
        {
            //Chưa làm :)))
            //Khi commit sẽ check xem có changes không để set Changes
            //Node không nên gọi thẳng đến Storage Service để set changes nữa mà nên thông qua 1 hàm
        }

        #endregion


        public void SetUndefined()
        {
            lock (Database)
            {
                NodeType = NodeType.Undefined;
                
                if(Parent == null)
                    Database.StorageService.SetDatabaseChanged();
                else
                    Database.StorageService.SetNodeChanged(Parent);
            }
        }
    }
}