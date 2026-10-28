using UnityEngine;

namespace ZeroX.SingletonSystem
{
    public abstract class SingletonSO<T> : ScriptableObject where T : SingletonSO<T>, new()
    {
        private static T instance;
        
        protected abstract string InstancePath { get; }

        
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    LoadInstance();
                }

                return instance;
            }
        }


        private static void LoadInstance()
        {
#pragma warning disable
            T temp = new T();
            string path = temp.InstancePath;
            Destroy(temp);
#pragma warning restore

            instance = Resources.Load<T>(path);
            if (instance == null)
                Debug.LogError("SingletonSO Instance not exist at path: " + path);
        }
    }
}