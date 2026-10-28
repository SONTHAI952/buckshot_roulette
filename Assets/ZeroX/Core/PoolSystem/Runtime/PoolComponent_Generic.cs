using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZeroX.PoolSystem
{
    public class Pool<TComp> : PoolComponent where TComp : Component
    {
        public object id { get; }
        TComp model;
        
        private Transform parentWhenInstantiate = null;
        
        Dictionary<TComp, LinkedListNode<TComp>> listAll = new Dictionary<TComp, LinkedListNode<TComp>>();
        LinkedList<TComp> listNotUsing = new LinkedList<TComp>();
        LinkedList<TComp> listUsing = new LinkedList<TComp>();
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

        public TComp Model => model;
        
        private Pool(object id, TComp model)
        {
            this.id = id;
            this.model = model;
        }
        
        internal static Pool<TComp> CreateInstance(object id, TComp model)
        {
            if(model == null)
                throw new Exception("Model of pool cannot null");
            
            return new Pool<TComp>(id, model);
        }
        
        public TComp GetOut(bool active)
        {
            if (isDisposed)
            {
                Debug.LogError("Cannot get out because pool has been disposed");
                return null;
            }
            
            if (listNotUsing.Count == 0)
            {
                TComp obj = GameObject.Instantiate(model, parentWhenInstantiate);
                listAll.Add(obj, listUsing.AddLast(obj));
                obj.gameObject.SetActive(active);
                return obj;
            }
            else
            {
                LinkedListNode<TComp> cacheNode = listNotUsing.First;
                listNotUsing.RemoveFirst();       //Xóa trong linkedList

                if (cacheNode.Value == null || cacheNode.Value.gameObject == null)  //nếu Component hoặc gameobject đã bị destroy
                {
                    listAll.Remove(cacheNode.Value);  //xóa trong listAll
                    return GetOut(active);
                }
                else
                {
                    listUsing.AddLast(cacheNode);
                    cacheNode.Value.gameObject.SetActive(active);
                    
                    #if UNITY_EDITOR
                    ProgressHideFlag(cacheNode.Value, true);
                    #endif

                    TComp obj = cacheNode.Value;
                    PoolManager.RemoveActionDelay(obj);
                    return obj;
                }
            }
        }

        public TComp GetOut()
        {
            return GetOut(defaultActiveWhenGetOut);
        }

        public override void PutIn(Component component, bool deActive)
        {
            PutIn((TComp)component, deActive);
        }

        public override void PutIn(Component component)
        {
            PutIn((TComp)component, defaultDeactiveWhenPutIn);
        }

        public override void PutIn(Component comp, float delay, bool ignoreTimeScale = false)
        {
            PutIn((TComp)comp, delay, ignoreTimeScale);
        }
        
        public void PutIn(TComp comp, bool deactive)
        {
            if (isDisposed)
            {
                Debug.LogError("Cannot put in because pool has been disposed -> destroy obj");
                if(comp != null && comp.gameObject != null)
                    Object.Destroy(comp.gameObject);
                return;
            }
            
            if(comp == null)
                return;
            
            LinkedListNode<TComp> cacheNode;
            if (listAll.TryGetValue(comp, out cacheNode) == false)  //nếu ko tồn tại trong listAll
            {
                Debug.LogError("Cannot put in because object not in list of pool -> destroy object");
                if(comp != null && comp.gameObject != null)
                    Object.Destroy(comp.gameObject);
                return;
            }
            
            if (cacheNode.List != listUsing)    //nếu ko phải trong list using
            {
                return;
            }
            
            if (cacheNode.Value == null || cacheNode.Value.gameObject == null)   //nếu trong list using nhưng value null -> tồn tại trong object pool nhưng đã bị destroy
            {
                listAll.Remove(comp);
                listUsing.Remove(cacheNode);
                return;
            }
            
            listUsing.Remove(cacheNode);
            listNotUsing.AddLast(cacheNode);

            comp.gameObject.SetActive(!deactive);
            PoolManager.RemoveActionDelay(comp);
            
            #if UNITY_EDITOR
            ProgressHideFlag(comp, false);
            #endif
        }

        public void PutIn(TComp comp)
        {
            PutIn(comp, defaultDeactiveWhenPutIn);
        }
        
        public void PutIn(TComp comp, float delay, bool ignoreTimeScale = false)
        {
            if (isDisposed)
            {
                Debug.LogError("Cannot put in because pool has been disposed -> destroy obj");
                if(comp != null && comp.gameObject != null)
                    Object.Destroy(comp.gameObject);
                return;
            }
            
            ActionDelayData action = new ActionDelayData();
            action.delay = delay;
            action.timeStart = ignoreTimeScale ? Time.unscaledTime : Time.time;
            action.action = () => PutIn(comp, defaultDeactiveWhenPutIn);
            PoolManager.AddActionDelay(comp, action, ignoreTimeScale);
        }
        
        
        //GetOut mở rộng
        public TComp GetOut(Transform parent)
        {
            var obj = GetOut();
            if (obj == null)
                return null;
            
            obj.transform.SetParent(parent);
            return obj;
        }
        
        public TComp GetOut(Vector3 position)
        {
            var obj = GetOut();
            if (obj == null)
                return null;
            
            obj.transform.position = position;
            return obj;
        }
        
        public TComp GetOut(Vector3 position, Quaternion rotation)
        {
            var obj = GetOut();
            if (obj == null)
                return null;
            
            var transform = obj.transform;
            transform.position = position;
            transform.rotation = rotation;
            return obj;
        }
        
        public TComp GetOut(Vector3 position, Quaternion rotation, Transform parent)
        {
            var obj = GetOut();
            if (obj == null)
                return null;
            
            var transform = obj.transform;
            transform.position = position;
            transform.rotation = rotation;
            transform.SetParent(parent);
            return obj;
        }
        
        /// <summary>
        /// Tạo trước 1 số TComp
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
                TComp obj = GameObject.Instantiate(model, parentWhenInstantiate);
                obj.gameObject.SetActive(false);
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

        public override bool IsModelNull()
        {
            return model == null;
        }

        public override void CheckAndRemoveAllNullObject()
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
                    if(kv.Key != null && kv.Key.gameObject != null)
                        GameObject.Destroy(kv.Key.gameObject);
                }
            }
            
            listAll.Clear();
            listUsing.Clear();
            listNotUsing.Clear();
        }

        public void DeactiveAllNotUsing()
        {
            foreach (var comp in listNotUsing)
            {
                comp.gameObject.SetActive(false);
            }
        }

        public override void Dispose()
        {
            isDisposed = true;
            RemoveAll(true);
        }
    }
}