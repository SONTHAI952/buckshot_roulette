namespace ZeroX.SafeAreaSystem
{
    [System.Serializable]
    public class SafeAreaConfig_Global
    {
        public bool enable = true;
        public SafeAreaConfig config = new SafeAreaConfig();


        public SafeAreaConfig_Global Clone()
        {
            SafeAreaConfig_Global clone = new SafeAreaConfig_Global();
            clone.enable = enable;
            clone.config = config.Clone();

            return clone;
        }
    }
}