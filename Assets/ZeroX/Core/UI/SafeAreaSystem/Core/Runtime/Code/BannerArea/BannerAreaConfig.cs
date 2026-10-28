namespace ZeroX.SafeAreaSystem
{
    [System.Serializable]
    public class BannerAreaConfig
    {
        public BannerPaddingPositionConfig left = new BannerPaddingPositionConfig(false, 0);
        public BannerPaddingPositionConfig right = new BannerPaddingPositionConfig(false, 0);
        public BannerPaddingPositionConfig top = new BannerPaddingPositionConfig(false, 250);
        public BannerPaddingPositionConfig bottom = new BannerPaddingPositionConfig(true, 250);




        public BannerAreaConfig Clone()
        {
            BannerAreaConfig clone = new BannerAreaConfig();
            clone.left = left.Clone();
            clone.right = right.Clone();
            clone.top = top.Clone();
            clone.bottom = bottom.Clone();

            return clone;
        }
    }
}