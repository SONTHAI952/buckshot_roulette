using System;

namespace ZeroX.TabSystem
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
            Open, OpenImmediate
        }

        public WhenType when = WhenType.Start;
        public ActionType actionType = ActionType.OpenImmediate;
        public string openTab = "";

        [NonSerialized] internal bool isPerformed = false;
    }
}