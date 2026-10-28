using ZeroX.RxSystem;


namespace ZeroX.SceneTransitionerSystem
{
    [System.Serializable]
    public class SceneTransitionerEvents
    {
        public readonly Subject<OnLoadStartData> OnLoadStart = new Subject<OnLoadStartData>();
        public readonly Subject<OnLoadEndData> OnLoadEnd = new Subject<OnLoadEndData>();
        
        public readonly Subject<OnTransitionStartData> OnTransitionStart = new Subject<OnTransitionStartData>();
        public readonly Subject<OnTransitionEndData> OnTransitionEnd = new Subject<OnTransitionEndData>();
        
        public readonly Subject<OnProgressChangedData> OnProgressChanged = new Subject<OnProgressChangedData>();
    }
}