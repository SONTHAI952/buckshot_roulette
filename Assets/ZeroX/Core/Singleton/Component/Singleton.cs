using UnityEngine;


namespace ZeroX.SingletonSystem
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        /// <summary>
        /// Don't modify this variable
        /// </summary>
        protected static T internal_Instance;
        
        
        protected abstract bool IsPersistent { get; }
        public static bool HasInstance => internal_Instance != null;
        protected bool IsSingletonInstance => internal_Instance == this;
        
        
        //Temp
        //Nếu đã từng là singleton thì sẽ được coi là đã đăng ký event và WhenAwake đã được gọi, vì vậy có nghĩa các hàm event và lifecycle khác cũng nên được gọi cho đầy đủ
        private bool isRegisteredEvent = false;

        
        
        


        #region Unity Method

        protected virtual void Awake()
        {
            CheckAndInitSingleton();

            
            //Nếu là singleton
            if (internal_Instance == this)
            {
                isRegisteredEvent = true;
                
                if (IsPersistent)
                {
                    transform.SetParent(null);
                    DontDestroyOnLoad(gameObject);
                }
                
                WhenAwake();
            }
        }
        
        protected virtual void OnEnable()
        {
            CheckAndInitSingleton();

            if (isRegisteredEvent)
            {
                WhenEnable();
            }
        }
        
        protected virtual void Start()
        {
            if (isRegisteredEvent)
            {
                WhenStart();
            }
        }
        
        protected virtual void OnDisable()
        {
            if (isRegisteredEvent)
            {
                WhenDisable();
            }
        }
        
        protected virtual void OnDestroy()
        {
            //Nếu một object trước đó là singleton, tức nó đã init một số thứ, thì khi Destroy nó nên được OnDestroy để hoàn thiện lifeCycle
            //Vì trong khi nó bị disable thì có thể bị đứa khác lấy mất instance nên phải làm vậy
            if (isRegisteredEvent)
            {
                WhenDestroy();
                isRegisteredEvent = false;
            }

            if (internal_Instance == this)
            {
                internal_Instance = null;
            }
        }

        #endregion
        
        
        
        private void CheckAndInitSingleton()
        {
            //Cho phép instance được đăng ký lại nếu instance cũ bị disable và không nằm trong DontDestroyOnLoad
            if (internal_Instance == null ||
                internal_Instance == this ||
                (internal_Instance.isActiveAndEnabled == false &&
                 internal_Instance.gameObject.scene.name != "DontDestroyOnLoad"))
            {
                
                if (internal_Instance != null && internal_Instance != this)
                {
                    Debug.Log($"Switch Singleton Instance: {internal_Instance.name} -> {this.name}");
                }
                
                
                internal_Instance = (T)this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        


        /// <summary>
        /// Sẽ được gọi nếu nó là instance hoặc đã từng là instance
        /// </summary>
        protected virtual void WhenAwake()
        {
        }
        
        /// <summary>
        /// Sẽ được gọi nếu nó là instance hoặc đã từng là instance
        /// </summary>
        protected virtual void WhenEnable()
        {
        }

        /// <summary>
        /// Sẽ được gọi nếu nó là instance hoặc đã từng là instance
        /// </summary>
        protected virtual void WhenStart()
        {

        }
        
        /// <summary>
        /// Sẽ được gọi nếu nó là instance hoặc đã từng là instance
        /// </summary>
        protected virtual void WhenDisable()
        {
        }

        /// <summary>
        /// Sẽ được gọi nếu nó là instance hoặc đã từng là instance
        /// </summary>
        protected virtual void WhenDestroy()
        {

        }
    }
}