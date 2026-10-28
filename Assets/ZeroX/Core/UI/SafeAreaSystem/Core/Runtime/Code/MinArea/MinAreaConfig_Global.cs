namespace ZeroX.SafeAreaSystem
{
    [System.Serializable]
    public class MinAreaConfig_Global
    {
        public bool enable = true;
        public MinAreaConfig config = new MinAreaConfig();


        public MinAreaConfig_Global Clone()
        {
            MinAreaConfig_Global clone = new MinAreaConfig_Global();
            clone.enable = enable;
            clone.config = config.Clone();

            return clone;
        }
    }
}