using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.PoolSystem
{
    public class Pool : IPoolMaintainable
    {
        public object id { get; }
        GameObject model;

        private Transform parentWhenInstantiate = null;
        
        Dictionary<GameObject, LinkedListNode<GameObject>> listAll = new Dictionary<GameObject, LinkedListNode<GameObject>>();
        LinkedList<GameObject> listNotUsing = new LinkedList<GameObject>();
        LinkedList<GameObject> listUsing = new LinkedList<GameObject>();
        private bool isDisposed = false;
        
        public bool defaultActiveWhenGetOut = true;
        public bool defaultDeactiveWhenPutIn = true;

#if UNITY_EDITOR
        private const bool hideWhenNotUsing = false;
        void ProgressHideFlag(Object obj, bool active)
        {
            if (hideWhenNotUsing == false)
                return;

            obj.hideFlags = active ? HideFlags.None : HideFlags.HideInHierarchy;
            UnityEditor.EditorApplication.DirtyHierarchyWindowSorting();
        }
        #endif

        public int CountAll => listUsing.Count + listNotUsing.Count;
        public int CountUsing => listUsing.Count;
        public int CountNotUsing => listNotUsing.Count;

        public GameObject Model => model;
        
        private Pool(object id, GameObject model)
        {
            this.id = id;
            this.model = model;
        }
        
        internal static Pool CreateInstance(object id, GameObject model)
        {
            if(model == null)
                throw new Exception("Model of pool cannot null");
            
            return new Pool(id, model);
        }

        public GameObject GetOut(bool active)
        {
            if (isDisposed)
            {
                Debug.LogError("Cannot get out because pool has been disposed");
                return null;
            }

            if (listNotUsing.Count == 0)
            {
                GameObject obj = GameObject.Instantiate(model, parentWhenInstantiate);
                listAll.Add(obj, listUsing.AddLast(obj));
                obj.SetActive(active);
                return obj;
            }
            else
            {
                LinkedListNode<GameObject> cacheNode = listNotUsing.First;
                listNotUsing.RemoveFirst();       //Xóa trong linkedList

                if (cacheNode.Value == null)  //nếu gameobject đã bị destroy
                {
                    listAll.Remove(cacheNode.Value);  //xóa trong listAll
                    return GetOut(active);
                }
                else
                {
                    listUsing.AddLast(cacheNode);
                    cacheNode.Value.SetActive(active);
                    
                    #if UNITY_EDITOR
                    ProgressHideFlag(cacheNode.Value, true);
                    #endif
                    
                    GameObject obj = cacheNode.Value;
                    PoolManager.RemoveActionDelay(obj);
                    return obj;
                }
            }
        }

        public GameObject GetOut()
        {
            return GetOut(defaultActiveWhenGetOut);
        }
        
        public void PutIn(GameObject obj, bool deactive)
        {
            if (isDisposed)
            {
                Debug.LogError("Cannot put in because pool has been disposed -> destroy obj");
                if(obj != null)
                    Object.Destroy(obj);
                return;
            }
            
            if(obj == null)
                return;
            
            LinkedListNode<GameObject> cacheNode;
            if (listAll.TryGetValue(obj, out cacheNode) == false)  //nếu ko tồn tại trong listAll
            {
                Debug.LogError("Cannot put in because object not in list of pool -> destroy object");
                if(obj != null)
                    Object.Destroy(obj);
                return;
            }
            
            if (cacheNode.List != listUsing)    //nếu ko phải trong list using
            {
                return;
            }
            
            if (cacheNode.Value == null)   //nếu trong list using nhưng value null -> tồn tại trong object pool nhưng đã bị destroy
            {
                listAll.Remove(obj);
                listUsing.Remove(cacheNode);
                return;
            }
            
            listUsing.Remove(cacheNode);
            listNotUsing.AddLast(cacheNode);

            obj.SetActive(!deactive);
            PoolManager.RemoveActionDelay(obj);
            
            #if UNITY_EDITOR
            ProgressHideFlag(obj, false);
            #endif
        }

        public void PutIn(GameObject obj)
        {
            PutIn(obj, defaultDeactiveWhenPutIn);
        }

        public void PutIn(GameObject obj, float delay, bool ignoreTimeScale = false)
        {
            if (isDisposed)
            {
                Debug.LogError("Cannot put in because pool has been disposed -> destroy obj");
                if(obj != null)
                    Object.Destroy(obj);
                return;
            }
            
            ActionDelayData action = new ActionDelayData();
            action.delay = delay;
            action.timeStart = ignoreTimeScale ? Time.unscaledTime : Time.time;
            action.action = () => PutIn(obj, defaultDeactiveWhenPutIn);
            PoolManager.AddActionDelay(obj, action, ignoreTimeScale);
        }
        
        
        //GetOut mở rộng
        public GameObject GetOut(Transform parent)
        {
            var obj = GetOut();
            if (obj == null)
                return null;
            
            obj.transform.SetParent(parent);
            return obj;
        }
        
        public GameObject GetOut(Vector3 position)
        {
            var obj = GetOut();
            if (obj == null)
                return null;
            
            obj.transform.position = position;
            return obj;
        }
        
        public GameObject GetOut(Vector3 position, Quaternion rotation)
        {
            var obj = GetOut();
            if (obj == null)
                return null;
            
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }
        
        public GameObject GetOut(Vector3 position, Quaternion rotation, Transform parent)
        {
            var obj = GetOut();
            if (obj == null)
                return null;
            
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(parent);
            return obj;
        }

        /// <summary>
        /// Tạo trước 1 số gameobject
        /// </summary>
        public void Prewarm(int number)
        {
            if (isDisposed)
            {
                Debug.LogError("Cannot prewarm because pool has been disposed");
                return;
            }
            
            number = number - listAll.Count;
            if(number <= 0)
                return;

            for (int i = 0; i < number; i++)
            {
                GameObject obj = GameObject.Instantiate(model, parentWhenInstantiate);
                obj.SetActive(false);
                listAll.Add(obj, listNotUsing.AddLast(obj));
            }
        }

        /// <summary>
        /// Parent khi được instantiate, không phải khi getout
        /// </summary>
        public void SetParentWhenInstantiate(Transform parent)
        {
            parentWhenInstantiate = parent;
        }

        public bool IsModelNull()
        {
            return model == null;
        }

        public void CheckAndRemoveAllNullObject()
        {
            int removeCount = 0;
            
            //Check list using
            if (listUsing.Count > 0)
            {
                removeCount = 0;
                var nextNode = listUsing.First;
                while (nextNode != null)
                {
                    var oldNode = nextNode;
                    nextNode = oldNode.Next;
                    
                    if (oldNode.Value == null)
                    {
                        listAll.Remove(oldNode.Value);
                        listUsing.Remove(oldNode);
                        removeCount++;
                    }
                }
                
                if(removeCount > 0)
                    Debug.Log("Pool(listUsing) remove all null object count: " + removeCount, model);
            }
            
            //Check list not using
            if (listNotUsing.Count > 0)
            {
                removeCount = 0;
                var nextNode = listNotUsing.First;
                while (nextNode != null)
                {
                    var oldNode = nextNode;
                    nextNode = oldNode.Next;
                    
                    if (oldNode.Value == null)
                    {
                        listAll.Remove(oldNode.Value);
                        listNotUsing.Remove(oldNode);
                        removeCount++;
                    }
                }
                
                if(removeCount > 0)
                    Debug.Log("Pool(listNotUsing) remove all null object count: " + removeCount, model);
            }
        }
        
        public void RemoveAll(bool alsoDestroyGameObject)
        {
            if (alsoDestroyGameObject)
            {
                foreach (var kv in listAll)
                {
                    if(kv.Value != null)
                        Object.Destroy(kv.Key);
                }
            }
            
            listAll.Clear();
            listUsing.Clear();
            listNotUsing.Clear();
        }
        
        public void DeactiveAllNotUsing()
        {
            foreach (var gObj in listNotUsing)
            {
                gObj.SetActive(false);
            }
        }

        public void Dispose()
        {
            isDisposed = true;
            RemoveAll(true);
        }
    }
}