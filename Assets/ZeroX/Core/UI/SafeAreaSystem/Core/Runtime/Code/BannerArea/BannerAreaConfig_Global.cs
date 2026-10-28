namespace ZeroX.SafeAreaSystem
{
    [System.Serializable]
    public class BannerAreaConfig_Global
    {
        public bool enable = true;
        public BannerAreaConfig config = new BannerAreaConfig();



        public BannerAreaConfig_Global Clone()
        {
            BannerAreaConfig_Global clone = new BannerAreaConfig_Global();
            clone.enable = enable;
            clone.config = config.Clone();

            return clone;
        }
    }
}