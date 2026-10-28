namespace ZeroX.RxSystem
{
    public abstract class SubjectBase
    {
        public bool onlyEmitOnMainThread = true;
        
        
        public abstract void UnSubscribe(ObserverBase observerBase);
    }
}