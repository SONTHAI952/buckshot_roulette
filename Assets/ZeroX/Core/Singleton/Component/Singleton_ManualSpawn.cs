namespace ZeroX.SingletonSystem
{
    public abstract class Singleton_ManualSpawn<T> : Singleton<T> where T : Singleton_ManualSpawn<T>
    {
        public static T Instance => internal_Instance;
        
        protected sealed override bool IsPersistent => false;
    }
}