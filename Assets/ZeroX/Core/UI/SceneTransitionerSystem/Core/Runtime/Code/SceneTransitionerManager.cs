using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.SceneTransitionerSystem
{
    public class SceneTransitionerManager : MonoBehaviour
    {
        //Static
        private static SceneTransitionerManager instance;

        public static SceneTransitionerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    CreateInstanceIfNot();
                }

                return instance;
            }
        }
        
        
        //Temp
        private bool initialized;
        private Dictionary<string, SceneTransitioner> dictInstance = new Dictionary<string, SceneTransitioner>();









        #region Unity Method

        private void Awake()
        {
            if (instance == null || instance == this)
            {
                instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                
                InitIfNot();
            }
            else
            {
                Debug.LogError("Chỉ có một Scene Transitioner Manager được tồn tại ---> Xóa");
                Destroy(gameObject);
                return;
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        #endregion
        
        
        
        private static void CreateInstanceIfNot()
        {
            if(instance != null)
                return;
            
            instance = new GameObject("Scene Transitioner Manager").AddComponent<SceneTransitionerManager>();
        }
        
        
        private void InitIfNot()
        {
            if(initialized)
                return;
            initialized = true;
        }
        
        
        
        
        
        
        
        
        
        
        #region Register -  UnRegister

        public static bool Register(SceneTransitioner sceneTransitioner)
        {
            if (sceneTransitioner == null)
            {
                Debug.LogError("SceneTransitioner want to register is null");
            }

            string id = sceneTransitioner.Id.ToLower();
            
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError("SceneTransitioner name cannot empty");
                return false;
            }
    
            
            if (Instance.dictInstance.TryGetValue(id, out var sceneTransitionerRegistered))
            {
                if (sceneTransitionerRegistered != null) //Trong dict có key và cũng có instance
                {
                    if (sceneTransitionerRegistered == sceneTransitioner) //Nếu chính là sceneTransitioner đã registered thì cũng coi là success
                        return true;
                    
                    Debug.LogErrorFormat("Cannot register sceneTransitioner with <color=red>{0}</color> because this id has already registered with another scene transitioner", id);
                    return false;
                }
                else //Trong dict có key nhưng instance đã bằng null
                {
                    Instance.dictInstance[id] = sceneTransitioner;
                    return true;
                }
            }
            else //Trong dict chưa có key
            {
                Instance.dictInstance.Add(id, sceneTransitioner);
                return true;
            }
        }

        public static bool UnRegister(SceneTransitioner sceneTransitioner)
        {
            if (instance == null)
                return false;
            
            
            
            string id = sceneTransitioner.Id.ToLower();


            if (Instance.dictInstance.TryGetValue(id, out var registeredSceneTransitioner))
            {
                if (registeredSceneTransitioner == sceneTransitioner)
                {
                    Instance.dictInstance.Remove(id);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public static bool IsRegistered(SceneTransitioner sceneTransitioner)
        {
            string id = sceneTransitioner.Id.ToLower();
            
            if (Instance.dictInstance.TryGetValue(id, out var registeredSceneTransitioner))
            {
                return registeredSceneTransitioner == sceneTransitioner;
            }

            return false;
        }

        #endregion
        
        
        
        
        #region Get

        /// <summary>
        /// Sẽ chỉ tìm kiếm SceneTransitioner trong danh sách instance mà không tự động sinh ra từ resources
        /// </summary>
        public static SceneTransitioner GetRegistered(string id)
        {
            id = id.ToLower();
            
            if (Instance.dictInstance.TryGetValue(id, out SceneTransitioner sceneTransitioner))
            {
                return sceneTransitioner;
            }

            return null;
        }
        
        /// <summary>
        /// Sẽ chỉ tìm kiếm SceneTransitioner trong danh sách instance mà không tự động sinh ra từ resources
        /// </summary>
        public static SceneTransitioner GetRegisteredWithLog(string id)
        {
            SceneTransitioner sceneTransitioner = GetRegistered(id);

            if (sceneTransitioner == null)
            {
                Debug.LogError("SceneTransitioner not found: " + id.ToLower());
            }
            
            return sceneTransitioner;
        }
        
        private static SceneTransitioner GetInSetting(string id)
        {
            id = id.ToLower();
            
            //Load SceneTransitioner prefab
            var prefab = SceneTransitionerSetting.Instance.GetPrefabById(id);
            if (prefab == null)
            {
                Debug.LogError("SceneTransitioner not found with id: " + id);
                return null;
            }
            
            //Instantiate
            var sceneTransitioner = Instantiate(prefab);
            SceneTransitioner.Internal_SetId(sceneTransitioner, id);
            return sceneTransitioner;
        }
        
        public static SceneTransitioner Get(string id)
        {
            //Get trong dict trước
            SceneTransitioner sceneTransitioner = GetRegistered(id);
            if (sceneTransitioner != null)
                return sceneTransitioner;


            //Nếu không tìm thấy instance trong dict thì spawn từ resources
            sceneTransitioner = GetInSetting(id);
            if (sceneTransitioner != null)
            {
                Register(sceneTransitioner);
            }
            return sceneTransitioner;
        }
        
        #endregion
    }
}