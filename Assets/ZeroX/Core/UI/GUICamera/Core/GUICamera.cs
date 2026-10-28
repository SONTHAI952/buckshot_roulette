using UnityEngine;

namespace ZeroX.UI
{
    [RequireComponent(typeof(Camera))]
    public class GUICamera : MonoBehaviour
    {
        private static GUICamera instance;

        public static GUICamera Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = CreateGUICamera();
                }

                return instance;
            }
        }
        
        public Camera Camera { get; private set; }

        
        
        private void Awake()
        {
            if (instance == null || instance == this)
            {
                instance = this;
                
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                
                Init();
            }
            else if (Instance != this)
            {
                Debug.Log("Chỉ có một GUICamera được tồn tại ---> Xóa");
                Destroy(gameObject);
            }
        }
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        
        
        
        private static GUICamera CreateGUICamera()
        {
            var prefab = Resources.Load<GUICamera>("ZeroX/GUICamera/GUI Camera");
            return Instantiate(prefab);
        }
        
        void Init()
        {
            Camera = GetComponent<Camera>();
        }
    }
}