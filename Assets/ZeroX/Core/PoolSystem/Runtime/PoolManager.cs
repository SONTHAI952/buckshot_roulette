using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ZeroX.PoolSystem
{
    public class PoolManager : MonoBehaviour
    {
        static Dictionary<object, object> dictPool = new Dictionary<object, object>();

        static Dictionary<object, ActionDelayData> dictActionDelayTimeScale = new Dictionary<object, ActionDelayData>();
        static Dictionary<object, ActionDelayData> dictActionDelayRealtime = new Dictionary<object, ActionDelayData>();
        
        static Dictionary<object, ActionDelayData> dictActionDelayTimeScaleNeedInvoked = new  Dictionary<object, ActionDelayData>();
        static Dictionary<object, ActionDelayData> dictActionDelayRealtimeNeedInvoked = new  Dictionary<object, ActionDelayData>();
        

        private static PoolManager instance;
        private UnityAction<Scene, LoadSceneMode> onSceneLoadedAction;
        

        
        

        #region Unity Method
        
        [RuntimeInitializeOnLoadMethod]
        private static void AutoInit()
        {
            Debug.Log("Pool Manager -> Auto Init");
            instance = new GameObject("Pool Manager").AddComponent<PoolManager>();
        }

        private void Awake()
        {
            if (instance == null || instance == this)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                
                onSceneLoadedAction = (a, b) => OnSceneLoaded();
                SceneManager.sceneLoaded += onSceneLoadedAction;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                SceneManager.sceneLoaded -= onSceneLoadedAction;
            }
        }

        private void Update()
        {
            if (dictActionDelayTimeScale.Count > 0)
            {
                foreach (var kv in dictActionDelayTimeScale)
                {
                    var data = kv.Value;
                    if (Time.time - data.timeStart >= data.delay)
                    {
                        dictActionDelayTimeScaleNeedInvoked.Add(kv.Key, kv.Value);
                    }
                }

                //Invoke
                if (dictActionDelayTimeScaleNeedInvoked.Count > 0)
                {
                    foreach (var kv in dictActionDelayTimeScaleNeedInvoked)
                    {
                        kv.Value.action.Invoke();
                        dictActionDelayTimeScale.Remove(kv.Key);
                    }
                    
                    dictActionDelayTimeScaleNeedInvoked.Clear();
                }
            }
            
            //Realtime
            if (dictActionDelayRealtime.Count > 0)
            {
                foreach (var kv in dictActionDelayRealtime)
                {
                    var data = kv.Value;
                    if (Time.unscaledTime - data.timeStart >= data.delay)
                    {
                        dictActionDelayRealtimeNeedInvoked.Add(kv.Key, kv.Value);
                    }
                }

                //Invoke
                if (dictActionDelayRealtimeNeedInvoked.Count > 0)
                {
                    foreach (var kv in dictActionDelayRealtimeNeedInvoked)
                    {
                        kv.Value.action.Invoke();
                        dictActionDelayRealtime.Remove(kv.Key);
                    }
                    
                    dictActionDelayRealtimeNeedInvoked.Clear();
                }
            }
        }
        
        #endregion
        
        
        
        void OnSceneLoaded()
        {
            List<object> listRemove = new List<object>();
            foreach (var kv in dictPool)
            {
                if (kv.Value is IPoolMaintainable maintainable)
                {
                    if (maintainable.IsModelNull())
                    {
                        listRemove.Add(kv.Key);
                        continue;
                    }
                    
                    maintainable.CheckAndRemoveAllNullObject();
                }
            }

            if (listRemove.Count > 0)
            {
                foreach (var keyPool in listRemove)
                {
                    dictPool.Remove(keyPool);
                }
                
                Debug.Log("PoolManager remove pool has model null: " + listRemove.Count);
            }
        }

        internal static void AddActionDelay(object key, ActionDelayData actionDelayData, bool ignoreTimeScale)
        {
            if(ignoreTimeScale)
                dictActionDelayRealtime.Add(key, actionDelayData);
            else
                dictActionDelayTimeScale.Add(key, actionDelayData);
        }

        internal static void RemoveActionDelay(object key)
        {
            dictActionDelayTimeScale.Remove(key);
            dictActionDelayRealtime.Remove(key);
        }
        
        public static void DisposeAllPool()
        {
            foreach (var poolObj in dictPool.Values)
            {
                IPoolMaintainable poolMaintainable = (IPoolMaintainable) poolObj;
                poolMaintainable.Dispose();
            }
            
            dictActionDelayTimeScale.Clear();
            dictActionDelayTimeScaleNeedInvoked.Clear();
            
            dictActionDelayRealtime.Clear();
            dictActionDelayRealtimeNeedInvoked.Clear();
            
            dictPool.Clear();
        }




        
        #region GameObject Pool

        public static Pool GetPool(object id)
        {
            if(id == null)
                throw new Exception("Pool id cannot null");
            
            if (id is string s)
                id = s.ToLower();
            
            object poolStorage;
            if (dictPool.TryGetValue(id, out poolStorage))
            {
                if(poolStorage is Pool storage)
                    return storage;
                else
                    throw new Exception("PoolStorage type not correct");
            }
            else
                return null;
        }
        
        
        
        public static Pool CreatePool(object id, GameObject model)
        {
            if(id == null)
                throw new Exception("Pool id cannot null");
            
            if(model == null)
                throw new Exception("Pool model cannot null");
            
            if (id is string s)
                id = s.ToLower();
            
            if (dictPool.ContainsKey(id))
                throw new Exception("Storage already exists!");

            Pool pool = Pool.CreateInstance(id, model);
            dictPool.Add(id, pool);
            return pool;
        }

        public static Pool CreatePool(GameObject idAndModel)
        {
            return CreatePool(idAndModel, idAndModel);
        }
        
        
        
        public static Pool GetOrCreatePool(object id, GameObject model)
        {
            if (id is string s)
                id = s.ToLower();
            
            Pool pool = GetPool(id);
            if (pool == null)
                pool = CreatePool(id, model);

            return pool;
        }

        public static Pool GetOrCreatePool(GameObject idAndModel)
        {
            Pool pool = GetPool(idAndModel);
            if (pool == null)
                pool = CreatePool(idAndModel, idAndModel);

            return pool;
        }

        #endregion


        
        #region Component Pool

        public static Pool<TComp> GetPool<TComp>(object id) where TComp : Component
        {
            if(id == null)
                throw new Exception("Pool id cannot null");
            
            if (id is string s)
                id = s.ToLower();
            
            object poolStorage;
            if (dictPool.TryGetValue(id, out poolStorage))
            {
                if(poolStorage is Pool<TComp> storage)
                    return storage;
                else
                    throw new Exception("PoolStorage type not correct");
            }
            else
                return null;
        }
        
        
        
        public static Pool<TComp> CreatePool<TComp>(object id, TComp model) where TComp : Component
        {
            if(id == null)
                throw new Exception("Pool id cannot null");
            
            if(model == null)
                throw new Exception("Pool model cannot null");
            
            if (id is string s)
                id = s.ToLower();
            
            if (dictPool.ContainsKey(id))
                throw new Exception("Storage already exists!");

            Pool<TComp> pool = Pool<TComp>.CreateInstance(id, model);
            dictPool.Add(id, pool);
            return pool;
        }

        public static Pool<TComp> CreatePool<TComp>(TComp idAndModel) where TComp : Component
        {
            return CreatePool(idAndModel, idAndModel);
        }
        
        
        
        public static Pool<TComp> GetOrCreatePool<TComp>(object id, TComp model) where TComp : Component
        {
            if (id is string s)
                id = s.ToLower();
            
            Pool<TComp> pool = GetPool<TComp>(id);
            if (pool == null)
                pool = CreatePool<TComp>(id, model);

            return pool;
        }

        public static Pool<TComp> GetOrCreatePool<TComp>(TComp idAndModel) where TComp : Component
        {
            Pool<TComp> pool = GetPool<TComp>(idAndModel);
            if (pool == null)
                pool = CreatePool<TComp>(idAndModel, idAndModel);

            return pool;
        }

        #endregion
        
        


        
        #region GameObject Pool - Quick Method

        public static GameObject GetOut(object poolId, GameObject poolModel, out Pool pool)
        {
            pool = GetOrCreatePool(poolId, poolModel);
            return pool.GetOut();
        }

        public static GameObject GetOut(object poolId, GameObject poolModel)
        {
            Pool pool = GetOrCreatePool(poolId, poolModel);
            return pool.GetOut();
        }
        
        public static GameObject GetOut(GameObject poolIdAndModel, out Pool pool)
        {
            pool = GetOrCreatePool(poolIdAndModel, poolIdAndModel);
            return pool.GetOut();
        }
        
        public static GameObject GetOut(GameObject poolIdAndModel)
        {
            Pool pool = GetOrCreatePool(poolIdAndModel, poolIdAndModel);
            return pool.GetOut();
        }
        
        
        

        public static Pool PutIn(object poolId, GameObject obj)
        {
            Pool pool = GetPool(poolId);
            if (pool == null)
            {
                throw new Exception("Cannot find this pool to PutIn: " + poolId);
            }
            
            pool.PutIn(obj);
            return pool;
        }
        
        #endregion


        
        #region Component Pool - Quick Method

        public static TComp GetOut<TComp>(object poolId, TComp poolModel, out Pool<TComp> pool) where TComp : Component
        {
            pool = GetOrCreatePool<TComp>(poolId, poolModel);
            return pool.GetOut();
        }
        
        public static TComp GetOut<TComp>(object poolId, TComp poolModel) where TComp : Component
        {
            Pool<TComp> pool = GetOrCreatePool<TComp>(poolId, poolModel);
            return pool.GetOut();
        }
        
        public static TComp GetOut<TComp>(TComp poolIdAndModel, out Pool<TComp> pool) where TComp : Component
        {
            pool = GetOrCreatePool<TComp>(poolIdAndModel, poolIdAndModel);
            return pool.GetOut();
        }
        
        public static TComp GetOut<TComp>(TComp poolIdAndModel) where TComp : Component
        {
            Pool<TComp> pool = GetOrCreatePool<TComp>(poolIdAndModel, poolIdAndModel);
            return pool.GetOut();
        }
        
        
        
        public static Pool<TComp> PutIn<TComp>(object poolId, TComp obj) where TComp : Component
        {
            Pool<TComp> pool = GetPool<TComp>(poolId);
            if (pool == null)
            {
                throw new Exception("Cannot find this pool to PutIn: " + poolId);
            }
            
            pool.PutIn(obj);
            return pool;
        }

        public static Pool<TComp> PutIn<TComp>(object poolId, TComp obj, float delay, bool ignoreTimeScale = false) where TComp : Component
        {
            Pool<TComp> pool = GetPool<TComp>(poolId);
            if (pool == null)
            {
                throw new Exception("Cannot find this pool to PutIn: " + poolId);
            }
            
            pool.PutIn(obj, delay, ignoreTimeScale);
            return pool;
        }
        
        #endregion
    }
}