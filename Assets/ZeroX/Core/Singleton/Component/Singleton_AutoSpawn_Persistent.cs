namespace ZeroX.SingletonSystem
{
    public abstract class Singleton_AutoSpawn_Persistent<T> : Singleton<T> where T : Singleton_AutoSpawn_Persistent<T>, new()
    {
        public static T Instance
        {
            get
            {
                if (internal_Instance == null)
                {
                    internal_Instance = Internal_CreateInstance();
                }

                return internal_Instance;
            }
        }
        
        protected sealed override bool IsPersistent => true;


        
        private static T Internal_CreateInstance()
        {
            var temp = new T();
            var newInstance = temp.CreateInstance();
            Destroy(temp);
            return newInstance;
        }
        
        protected abstract T CreateInstance();
    }
}