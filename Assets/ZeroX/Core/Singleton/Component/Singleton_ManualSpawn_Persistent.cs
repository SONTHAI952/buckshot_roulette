namespace ZeroX.SingletonSystem
{
    public abstract class Singleton_ManualSpawn_Persistent<T> : Singleton<T> where T : Singleton_ManualSpawn_Persistent<T>
    {
        public static T Instance => internal_Instance;

        protected sealed override bool IsPersistent => true;
    }
}