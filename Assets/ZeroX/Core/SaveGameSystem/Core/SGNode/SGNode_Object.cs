using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ZeroX.JsonSystem;
using ZeroX.SaveGameSystem.Core;

namespace ZeroX.SaveGameSystem
{
    //Cần check lại FromJson/ToJson khi thêm các thuộc tính mới cho node
    public partial class SGNode : IEnumerable<SGNode>
    {
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
        
        internal void Initialize_FromJsonNode(ZJsonObject jsonObject)
        {
            lock (Database)
            {
                if (IsObjectNodeInitialized)
                {
                    Debug.LogError("Đã init rồi, đừng init nữa");
                    return;
                }
                
                
                if (jsonObject == null || jsonObject.ChildCount == 0)
                {
                    SetupDefault();
                    IsObjectNodeInitialized = true;
                    return;
                }
                

                
                //Import các thuộc tính của node
                ChildKeySeq = jsonObject.HasKey(KeyDefine.childKeySeq) ? jsonObject[KeyDefine.childKeySeq].AsLong : SGConstant.EMPTY_CHILD_KEY_SEQ;
                
                
                //Import Child
                ZJsonObject jListChild = jsonObject.HasKey(KeyDefine.listChild) ? jsonObject[KeyDefine.listChild].AsObject : new ZJsonObject();
                dictChildNode = new Dictionary<string, SGNode>();
                listChildKey = new List<string>();
                
                foreach (var kv in jListChild.KeyValues)
                {
                    string key = kv.Key;
                    var jChild = kv.Value.AsObject;
                    
                    NodeType nodeType = jChild.HasKey(KeyDefine.nodeType) ? (NodeType)jChild[KeyDefine.nodeType].AsInt : NodeType.Undefined;
                    long nodeId = jChild.HasKey(KeyDefine.nodeId) ? jChild[KeyDefine.nodeId].AsLong : SGConstant.EMPTY_NODE_ID;

                    SGNode childNode = new SGNode(key, Database, Root, this, nodeType, nodeId);
                    
                    if (jChild.HasKey(KeyDefine.value))
                        SGNode.ImportJChildValueIntoNode(jChild[KeyDefine.value].AsScalar, childNode, nodeType);


                    dictChildNode.Add(key, childNode);
                    listChildKey.Add(key);
                }
                
                
                IsObjectNodeInitialized = true;
            }
        }
        
        public ZJsonObject ToJsonObject()
        {
            lock (Database)
            {
                //Initialized chỉ dành cho nodeObject, vì node primitive sẽ đc import value bởi parent của nó rồi. Ns cách khác, primitive thuộc về node parent
                if (IsObjectNodeInitialized == false)
                {
                    if (HasNodeId)
                    {
                        Debug.LogWarning("Chưa initialize, to json sẽ lấy từ disk");
                        Debug.Log("NodeId: " + NodeId);
                        Debug.Log("Key: " + Key);
                        return Database.StorageService.LoadNodeDataJson(NodeId); // :))))
                    }
                    else
                    {
                        //Vì vậy, trong quá trình ToJsonNode, nếu đây là một node primitive thì sẽ phải custom
                        ZJsonObject jNodePrimitive = new ZJsonObject();
                        jNodePrimitive[KeyDefine.nodeType] = (int) NodeType;
                        
                        if (NodeType != NodeType.Undefined && IsPrimitive)
                            SGNode.ImportNodeIntoJChild(this, jNodePrimitive);

                        return jNodePrimitive;
                    }
                }


                ZJsonObject jNode = new ZJsonObject();
                //Các thuộc tính của node
                jNode.Add(KeyDefine.childKeySeq, ChildKeySeq);

                //Child
                ZJsonObject jListChild = new ZJsonObject();
                foreach (var key in listChildKey)
                {
                    SGNode childNode = dictChildNode[key];

                    ZJsonObject jChild = new ZJsonObject();
                    jChild[KeyDefine.nodeType] = (int) childNode.NodeType;
                    
                    if (childNode.HasNodeId)
                        jChild[KeyDefine.nodeId] = childNode.NodeId;
                    
                    if (childNode.NodeType != NodeType.Undefined && childNode.IsPrimitive)
                        SGNode.ImportNodeIntoJChild(childNode, jChild);

                    jListChild.Add(key, jChild);
                }

                jNode.Add(KeyDefine.listChild, jListChild);
                return jNode;
            }
        }

        public string ToJson()
        { 
            return ToJsonObject().ToString();
        }

        public bool HasChild(string childKey)
        {
            if (string.IsNullOrEmpty(childKey))
            {
                return false;
            }
            
            lock (Database)
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
                
                lock (Database)
                {
                    InitializeIfNot();
                    
                    if (NodeType == NodeType.Object)
                    {
                        if (dictChildNode.TryGetValue(childKey, out var childNode))
                            return childNode;
                
                        childNode = new SGNode(childKey, Database, Root, this, NodeType.Undefined);
                        dictChildNode.Add(childKey, childNode);
                        listChildKey.Add(childKey);

                        Database.StorageService.SetNodeChanged(this);
                
                        return childNode;
                    }
                    else //Nếu đang là primitive
                    {
                        if (HasNodeId) //Nếu đã từng là node -> đổi type và gọi lại là đc
                        {
                            NodeType = NodeType.Object;
                            if(Parent == null)
                                Database.StorageService.SetDatabaseChanged();
                            else
                                Database.StorageService.SetNodeChanged(Parent);
                            
                            return this[childKey];
                        }
                        else
                        {
                            //Chuyển đổi node này sang object
                            NodeType = NodeType.Object;
                            NodeId = Database.NodeIdSeq + 1;
                    
                            var childNode = new SGNode(childKey, Database, Root, this, NodeType.Undefined);
                            dictChildNode.Add(childKey, childNode);
                            listChildKey.Add(childKey);

                            //Save
                            Database.Internal_IncreaseNodeIdSeq(Database);
                            if(Parent == null)
                                Database.StorageService.SetDatabaseChanged();
                            else
                                Database.StorageService.SetNodeChanged(Parent);
                            
                            //Comment nếu ko cần set node này changed ngay vì nó vừa đc chuyển thành object, và có 1 child, và child đó chưa động gì vào cả
                            Database.StorageService.SetNodeChanged(this); 

                            return childNode;
                        }
                    }
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

        /// <summary>
        /// Get child by index
        /// </summary>
        public SGNode GetChildAt(int childIndex)
        {
            lock (Database)
            {
                InitializeIfNot();
                string key = listChildKey[childIndex];
                return this[key];
            }
        }

        /// <summary>
        /// If child not exist -> return false instead of creating a new node
        /// </summary>
        public bool TryGetChild(string childKey, out SGNode childNode)
        {
            if (string.IsNullOrEmpty(childKey))
            {
                Debug.LogError("Child key cannot empty");
                childNode = null;
                return false;
            }

            lock (Database)
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

            lock (Database)
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
                Debug.LogError("Child keys cannot empty");
                childNode = null;
                return false;
            }
            
            lock (Database)
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
            
            lock (Database)
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
        
        /// <summary>
        /// Xóa chính nó và các child. Hàm này sẽ xóa các child theo đệ quy. Nó không gọi lên parent để xóa nó khỏi list đâu, nên mới gọi là internal
        /// </summary>
        internal void Internal_Delete()
        {
            lock (Database)
            {
                if(IsDeleted)
                    return;
                
                InitializeIfNot();
                
                if (listChildKey.Count == 0)
                {
                    IsDeleted = true;
                    if(HasNodeId)
                        Database.StorageService.SetNodeChanged(this);
                    return;
                }

                foreach (var childNode in dictChildNode.Values)
                {
                    childNode.Internal_Delete();
                }
                
                listChildKey.Clear();
                dictChildNode.Clear();
                IsDeleted = true;

                //Save
                Database.StorageService.SetNodeChanged(this);
            }
        }
        
        public void DeleteAllChild()
        {
            lock (Database)
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
                Database.StorageService.SetNodeChanged(this);
            }
        }
        
        public void DeleteChild(string childKey)
        {
            if (string.IsNullOrEmpty(childKey))
            {
                Debug.LogError("Child key cannot empty");
                return;
            }
            
            lock (Database)
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
                
                Database.StorageService.SetNodeChanged(this);
            }
        }

        public void DeleteChildAt(int childIndex)
        {
            lock (Database)
            {
                if (childIndex < 0 || childIndex >= ChildCount)
                    throw new IndexOutOfRangeException();
                
                string key = listChildKey[childIndex];
                DeleteChild(key);
            }
        }

        public int DeleteAllChild(Predicate<SGNode> match)
        {
            lock (Database)
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
                Database.StorageService.SetNodeChanged(this);
                
                return listChildKeyRemove.Count;
            }
        }

        public void DeleteLastChilds(int number)
        {
            if(number <= 0)
                return;

            lock (Database)
            {
                InitializeIfNot();
                
                int end = listChildKey.Count - number - 1;
                if (end < -1)
                    end = -1;
                
                for (int i = listChildKey.Count - 1; i > end; i--)
                {
                    string childKey = listChildKey[i];
                    dictChildNode[childKey].Internal_Delete();
                    
                    dictChildNode.Remove(childKey);
                    listChildKey.RemoveAt(i);
                }
                
                Database.StorageService.SetNodeChanged(this);
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
            lock (Database)
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
            lock (Database)
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
            lock (Database)
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
                    var jNode = Database.StorageService.LoadNodeDataJsonNoLock(childNode.NodeId);
                    childNode.Initialize_FromJsonNode(jNode);
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
                    var jNode = Database.StorageService.LoadNodeDataJsonNoLock(childNode.NodeId);
                    childNode.Initialize_FromJsonNode(jNode);
                }
            });
        }
    }
}