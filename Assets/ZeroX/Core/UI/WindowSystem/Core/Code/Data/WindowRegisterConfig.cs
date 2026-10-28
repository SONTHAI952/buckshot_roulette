namespace ZeroX.WindowSystem
{
    [System.Serializable]
    public class WindowRegisterConfig
    {
        public bool registerWithWindowManager;
        public bool registerOnAwake = true;
        public string windowName;
    }
}