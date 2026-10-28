namespace ZeroX.WindowSystem
{
    [System.Serializable]
    public class FirstAction
    {
        public enum WhenType
        {
            Awake, FirstEnable, Start
        }
            
        public enum ActionType
        {
            Open, OpenImmediate, Close, CloseImmediate
        }

        public WhenType when = WhenType.Start;
        public ActionType actionType = ActionType.CloseImmediate;
        public bool fireEvent = false;
        public float delay = 0f;
    }
}