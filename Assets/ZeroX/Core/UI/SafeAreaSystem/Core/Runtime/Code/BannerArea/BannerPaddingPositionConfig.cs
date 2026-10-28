namespace ZeroX.SafeAreaSystem
{
    [System.Serializable]
    public class BannerPaddingPositionConfig
    {
        public bool enable;
        public float value;

        public BannerPaddingPositionConfig()
        {
        }

        public BannerPaddingPositionConfig(bool enable, float value)
        {
            this.enable = enable;
            this.value = value;
        }


        public BannerPaddingPositionConfig Clone()
        {
            BannerPaddingPositionConfig clone = new BannerPaddingPositionConfig();
            clone.enable = enable;
            clone.value = value;

            return clone;
        }
    }
}