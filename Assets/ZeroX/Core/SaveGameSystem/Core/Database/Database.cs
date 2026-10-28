using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZeroX.JsonSystem;
using ZeroX.SaveGameSystem.Core;
using Debug = UnityEngine.Debug;

namespace ZeroX.SaveGameSystem
{
    //Cần check lại FromJson/ToJson khi thêm các thuộc tính mới cho database
    public partial class Database : IEnumerable<SGNode>
    {
#if UNITY_EDITOR
        public string DatabaseId { get; private set; }
        protected bool IsInitialized { get; private set; }
        public bool IsDatabaseClosed { get; private set; }
        public StorageService StorageService { get; private set; }
#else
        [NonSerialized] public string DatabaseId;
        [NonSerialized] protected bool IsInitialized;
        [NonSerialized] public bool IsDatabaseClosed;
        [NonSerialized] public StorageService StorageService;
#endif
        
        
        
        
        public int ChildCount
        {
            get
            {
                InitializeIfNot();
                return dictChildNode.Count;
            }
        }
        

        private long nodeIdSeq;
        public long NodeIdSeq
        {
            get
            {
                InitializeIfNot();
                return nodeIdSeq;
            }
            private set => nodeIdSeq = value;
        }

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

        //Child
        private List<string> listChildKey;
        private Dictionary<string, SGNode> dictChildNode;

        // protected Dictionary<string, SGNode> DictChildNode
        // {
        //     get
        //     {
        //         InitializeIfNot();
        //         return dictChildNode;
        //     }
        // }

        void InitializeIfNot()
        {
            lock (this)
            {
                if(IsInitialized)
                    return;
            
                ZJsonObject jsonObject = StorageService.LoadDatabaseDataJson();
                FromJsonNodeInternal(jsonObject, false);
                
                IsInitialized = true;
            }
        }

        void SetupDefault()
        {
            NodeIdSeq = SGConstant.START_NODE_ID_SEQ;
            ChildKeySeq = SGConstant.EMPTY_CHILD_KEY_SEQ;
            dictChildNode = new Dictionary<string, SGNode>();
            listChildKey = new List<string>();
        }
        
        private void FromJsonNodeInternal(ZJsonObject jDb, bool setChanged)
        {
            lock (this)
            {
                if (IsInitialized)
                {
                    Debug.LogError("Do thời gian có hạn nên tính năng này hiện tại chỉ ở mức dùng để init");
                    return;
                }

                
                if (jDb == null || jDb.ChildCount == 0)
                {
                    SetupDefault();
                    IsInitialized = true;
                    if(setChanged)
                        StorageService.SetDatabaseChanged();
                    return;
                }


                
                //Import các thuộc tính của database
                NodeIdSeq = jDb.HasKey(KeyDefine.nodeIdSequence) ? jDb[KeyDefine.nodeIdSequence].AsLong : SGConstant.START_NODE_ID_SEQ;
                ChildKeySeq = jDb.HasKey(KeyDefine.childKeySeq) ? jDb[KeyDefine.childKeySeq].AsLong : SGConstant.EMPTY_CHILD_KEY_SEQ;


                //Import Child
                ZJsonObject jListChild = jDb.HasKey(KeyDefine.listChild) ? jDb[KeyDefine.listChild].AsObject : new ZJsonObject();
                dictChildNode = new Dictionary<string, SGNode>();
                listChildKey = new List<string>();
                
                foreach (var kv in jListChild.KeyValues)
                {
                    string key = kv.Key;
                    var jChild = kv.Value.AsObject;
                    
                    NodeType nodeType = jChild.HasKey(KeyDefine.nodeType) ? (NodeType) jChild[KeyDefine.nodeType].AsInt : NodeType.Undefined;
                    long nodeId = jChild.HasKey(KeyDefine.nodeId) ? jChild[KeyDefine.nodeId].AsLong : 0;

                    SGNode childNode = new SGNode(key, this, null, nodeType, nodeId);
                    
                    if (jChild.HasKey(KeyDefine.value))
                        SGNode.ImportJChildValueIntoNode(jChild[KeyDefine.value].AsScalar, childNode, nodeType);


                    dictChildNode.Add(key, childNode);
                    listChildKey.Add(key);
                }
                
                
                IsInitialized = true;
                
                if(setChanged)
                    StorageService.SetDatabaseChanged();
            }
        }

        public ZJsonObject ToJsonObject()
        {
            lock (this)
            {
                if (IsInitialized == false)
                {
                    Debug.LogError("Chưa initialized, to json sẽ lấy từ disk");
                    return StorageService.LoadDatabaseDataJson();
                }

                ZJsonObject jDb = new ZJsonObject();
                //Các thuộc tính của database
                jDb.Add(KeyDefine.nodeIdSequence, NodeIdSeq);
                jDb.Add(KeyDefine.childKeySeq, ChildKeySeq);
                
                
                //Child
                ZJsonObject jListChild = new ZJsonObject();
                foreach (var key in listChildKey)
                {
                    SGNode childNode = dictChildNode[key];
                    
                    ZJsonObject jChild = new ZJsonObject();
                    jChild[KeyDefine.nodeType] = (int)childNode.NodeType;
                    
                    if (childNode.HasNodeId)
                        jChild[KeyDefine.nodeId] = childNode.NodeId;
                    
                    if (childNode.NodeType != NodeType.Undefined && childNode.IsPrimitive)
                        SGNode.ImportNodeIntoJChild(childNode, jChild);

                    jListChild.Add(key, jChild);
                }
                
                jDb.Add(KeyDefine.listChild, jListChild);
                return jDb;
            }
        }

        public string ToJson()
        {
            return ToJsonObject().ToString();
        }

        #region Constructor

        public Database(string databaseId, StorageService storageService)
        {
            this.DatabaseId = databaseId;
            this.StorageService = storageService;
        }

        #endregion

        public void OpenDatabase()
        {
            lock (this)
            {
                StorageService.OpenService(this);
                InitializeIfNot();
            }
        }

        public void CloseDatabase()
        {
            lock (this)
            {
                if(IsDatabaseClosed)
                    return;
            
                IsDatabaseClosed = true;
                StorageService.CloseService();
            }
        }
        
        public bool IsStorageServiceHasRunningProcess()
        {
            return StorageService.HasRunningProcess();
        }

        public bool HasChangesNeedWriteToStorage()
        {
            return StorageService.HasChangesNeedWriteToStorage();
        }

        public void SaveAllChangesImmediately()
        {
            if (IsDatabaseClosed)
            {
                Debug.LogError("Cannot save because database closed");
                return;
            }
            
            StorageService.SaveAllChangesToStorage();
        }

        public static void Internal_IncreaseNodeIdSeq(Database database)
        {
            lock (database)
            {
                database.NodeIdSeq++;
                database.StorageService.SetDatabaseChanged();
            }
        }
        
        public bool HasChild(string childKey)
        {
            if (string.IsNullOrEmpty(childKey))
            {
                return false;
            }
            
            lock (this)
            {
                InitializeIfNot();
                return dictChildNode.ContainsKey(childKey);
            }
        }

        #region Get Child

        /// <summary>
        /// If node with childKey not exist -> create new node and return
        /// </summary>
        public SGNode this[string childKey]
        {
            get
            {
                if (string.IsNullOrEmpty(childKey))
                {
                    Debug.LogError("Child key cannot empty");
                    return null;
                }
                
                lock (this)
                {
                    InitializeIfNot();
                    
                    if (dictChildNode.TryGetValue(childKey, out var childNode))
                        return childNode;
                
                    childNode = new SGNode(childKey, this, null, NodeType.Undefined);
                    dictChildNode.Add(childKey, childNode);
                    listChildKey.Add(childKey);

                    StorageService.SetDatabaseChanged();
                
                    return childNode;
                }
            }
        }
        
        /// <summary>
        /// childKey will convert to string, not index
        /// </summary>
        public SGNode this[object childKey]
        {
            get
            {
                if (childKey == null)
                {
                    Debug.LogError("Child key cannot empty");
                    return null;
                }

                return this[childKey.ToString()];
            }
        }

        public SGNode GetChildAt(int childIndex)
        {
            lock (this)
            {
                //Gọi đến ChildCount thì đã InitializeIfNot rồi
                if (childIndex < 0 || childIndex >= ChildCount)
                    throw new IndexOutOfRangeException();

                string key = listChildKey[childIndex];
                return this[key];
            }
        }
        
        /// <summary>
        /// If child not exist -> return null instead of creating a new node
        /// </summary>
        public bool TryGetChild(string childKey, out SGNode childNode)
        {
            if (string.IsNullOrEmpty(childKey))
            {
                Debug.LogError("Child key cannot empty");
                childNode = null;
                return false;
            }

            lock (this)
            {
                InitializeIfNot();
                
                if (dictChildNode.ContainsKey(childKey))
                {
                    childNode = this[childKey];
                    return true;
                }
                else
                {
                    childNode = null;
                    return false;
                }
            }
        }
        
        /// <summary>
        /// If child not exist -> return false instead of creating a new node
        /// </summary>
        public bool TryGetChild(object childKey, out SGNode childNode)
        {
            if(childKey == null)
            {
                Debug.LogError("Child key cannot empty");
                childNode = null;
                return false;
            }
            
            string childKeyString = childKey.ToString();
            if (string.IsNullOrEmpty(childKeyString))
            {
                Debug.LogError("Child key cannot empty");
                childNode = null;
                return false;
            }

            lock (this)
            {
                InitializeIfNot();
                
                if (dictChildNode.ContainsKey(childKeyString))
                {
                    childNode = this[childKeyString];
                    return true;
                }
                else
                {
                    childNode = null;
                    return false;
                }
            }
        }
        
        
        /// <summary>
        /// Get recursively
        /// <para>Example childKeys is [inventory, sword_1, damage] will return node inventory/sword_1/damage if exists</para>
        /// </summary>
        public bool TryGetChild(out SGNode childNode, params string[] childKeys)
        {
            if (childKeys == null || childKeys.Length == 0)
            {
                childNode = null;
                return false;
            }

            lock (this)
            {
                InitializeIfNot();
                
                if (string.IsNullOrEmpty(childKeys[0]))
                {
                    Debug.LogError("Child key cannot empty");
                    childNode = null;
                    return false;
                }

                SGNode childNodeTemp;
                if (dictChildNode.ContainsKey(childKeys[0]) == false)
                {
                    childNode = null;
                    return false;
                }
                else
                {
                    childNodeTemp = this[childKeys[0]];
                }

                for (int i = 1; i < childKeys.Length; i++)
                {
                    string childKey = childKeys[i];
                    if (string.IsNullOrEmpty(childKey))
                    {
                        Debug.LogError("Child key cannot empty");
                        childNode = null;
                        return false;
                    }

                    if (childNodeTemp.HasChild(childKey) == false)
                    {
                        childNode = null;
                        return false;
                    }

                    childNodeTemp = childNodeTemp[childKey];
                }

                childNode = childNodeTemp;
                return true;
            }
        }
        
        /// <summary>
        /// Get recursively
        /// <para>Example childKeys is [inventory, sword_1, damage] will return node inventory/sword_1/damage if exists</para>
        /// </summary>
        public bool TryGetChild(out SGNode childNode, params object[] childKeys)
        {
            if (childKeys == null || childKeys.Length == 0)
            {
                Debug.LogError("Child keys cannot empty");
                childNode = null;
                return false;
            }
            
            lock (this)
            {
                InitializeIfNot();
                
                if (childKeys[0] == null)
                {
                    Debug.LogError("Child key cannot empty");
                    childNode = null;
                    return false;
                }

                string childKeyString = childKeys[0].ToString();
                if (string.IsNullOrEmpty(childKeyString))
                {
                    Debug.LogError("Child key cannot empty");
                    childNode = null;
                    return false;
                }

                SGNode childNodeTemp;
                if (dictChildNode.ContainsKey(childKeyString) == false)
                {
                    childNode = null;
                    return false;
                }
                else
                {
                    childNodeTemp = this[childKeyString];
                }

                for (int i = 1; i < childKeys.Length; i++)
                {
                    if (childKeys[i] == null)
                    {
                        Debug.LogError("Child key cannot empty");
                    }
                    
                    childKeyString = childKeys[i].ToString();
                    if (string.IsNullOrEmpty(childKeyString))
                    {
                        Debug.LogError("Child key cannot empty");
                        childNode = null;
                        return false;
                    }

                    if (childNodeTemp.HasChild(childKeyString) == false)
                    {
                        childNode = null;
                        return false;
                    }

                    childNodeTemp = childNodeTemp[childKeyString];
                }

                childNode = childNodeTemp;
                return true;
            }
        }

        #endregion
        
        #region Delete Child

        public void DeleteAllChild()
        {
            lock (this)
            {
                InitializeIfNot();
                
                if(listChildKey.Count == 0)
                    return;
                
                foreach (var childNode in dictChildNode.Values)
                {
                    childNode.Internal_Delete();
                }
                
                listChildKey.Clear();
                dictChildNode.Clear();

                //Save
                StorageService.SetDatabaseChanged();
            }
        }
        
        public void DeleteChild(string childKey)
        {
            if (string.IsNullOrEmpty(childKey))
            {
                Debug.LogError("Child key cannot empty");
                return;
            }
            
            lock (this)
            {
                InitializeIfNot();

                if (dictChildNode.TryGetValue(childKey, out var childNode) == false)
                {
                    Debug.Log("Has no child with this key to delete: " + childKey);
                    return;
                }
                
                childNode.Internal_Delete();

                dictChildNode.Remove(childKey);
                listChildKey.Remove(childKey);
                
                StorageService.SetDatabaseChanged();
            }
        }
        
        public void DeleteChildAt(int childIndex)
        {
            lock (this)
            {
                if (childIndex < 0 || childIndex >= ChildCount)
                    throw new IndexOutOfRangeException();
                
                string key = listChildKey[childIndex];
                DeleteChild(key);
            }
        }
        
        public int DeleteAllChild(Predicate<SGNode> match)
        {
            lock (this)
            {
                InitializeIfNot();
                
                if (ChildCount == 0)
                    return 0;
                
                HashSet<string> listChildKeyRemove = new HashSet<string>();
                foreach (var kv in dictChildNode)
                {
                    if(match(kv.Value))
                        listChildKeyRemove.Add(kv.Key);
                }

                foreach (var childKey in listChildKeyRemove)
                {
                    var childNode = dictChildNode[childKey];
                    childNode.Internal_Delete();
                    
                    dictChildNode.Remove(childKey);
                }

                listChildKey.RemoveAll(k => listChildKeyRemove.Contains(k));

                //Save
                StorageService.SetDatabaseChanged();
                
                return listChildKeyRemove.Count;
            }
        }

        #endregion

        
        #region Add Child

        public SGNode AddChild(string key)
        {
            return this[key];
        }
        
        /// <summary>
        /// Auto generate key number
        /// </summary>
        public SGNode AddChild()
        {
            lock (this)
            {
                ChildKeySeq++;
                while (dictChildNode.ContainsKey(ChildKeySeq.ToString()))
                {
                    ChildKeySeq++;
                }
            
                return this[ChildKeySeq.ToString()];
            }
        }
        
        #endregion
        
        
        public IEnumerator<SGNode> GetEnumerator()
        {
            lock (this)
            {
                InitializeIfNot();
                
                foreach (var childNode in dictChildNode.Values)
                {
                    yield return childNode;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<SGNode> GetEnumerableExactOrder()
        {
            lock (this)
            {
                InitializeIfNot();
                
                foreach (var childKey in listChildKey)
                {
                    yield return dictChildNode[childKey];
                }
            }
        }
        
        public void InitializeChildRange(int fromIndex, int toIndex)
        {
            InitializeIfNot();
            
            if(fromIndex < 0)
                throw new IndexOutOfRangeException();
            
            if(fromIndex > toIndex)
                throw new Exception("FromIndex cannot larger toIndex");
            
            if(toIndex >= listChildKey.Count)
                throw new IndexOutOfRangeException();
            
            Parallel.For(fromIndex, toIndex, index =>
            {
                string key = listChildKey[index];
                var childNode = dictChildNode[key];
                if (childNode.HasNodeId && childNode.IsObjectNodeInitialized == false)
                {
                    var jsonObject = StorageService.LoadNodeDataJsonNoLock(childNode.NodeId);
                    childNode.Initialize_FromJsonNode(jsonObject);
                }
            });
        }
        
        public void InitializeAllChild()
        {
            InitializeIfNot();

            Parallel.ForEach(dictChildNode.Values, (childNode, loopState, index) =>
            {
                if (childNode.HasNodeId && childNode.IsObjectNodeInitialized == false)
                {
                    var jNode = StorageService.LoadNodeDataJsonNoLock(childNode.NodeId);
                    childNode.Initialize_FromJsonNode(jNode);
                }
            });
        }
        
        
        #region Transaction

        /// <summary>
        /// Begins a new transaction. Call <see cref="Commit" /> to end the transaction.
        /// </summary>
        public void BeginTransaction()
        {
            //Chưa làm :)))
        }

        /// <summary>
        /// Commits the transaction that was begun by <see cref="BeginTransaction" />.
        /// </summary>
        public void Commit()
        {
            //Chưa làm :)))
        }

        #endregion
    }
}