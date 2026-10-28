namespace ZeroX.SafeAreaSystem
{
    [System.Serializable]
    public class MinAreaConfig
    {
        public MinPaddingPositionConfig left = new MinPaddingPositionConfig(false, 0);
        public MinPaddingPositionConfig right = new MinPaddingPositionConfig(false, 0);
        public MinPaddingPositionConfig top = new MinPaddingPositionConfig(false, 75);
        public MinPaddingPositionConfig bottom = new MinPaddingPositionConfig(true, 200);



        public MinAreaConfig Clone()
        {
            MinAreaConfig clone = new MinAreaConfig();
            clone.left = left.Clone();
            clone.right = right.Clone();
            clone.top = top.Clone();
            clone.bottom = bottom.Clone();

            return clone;
        }
    }
}