namespace ZeroX.SafeAreaSystem
{
    [System.Serializable]
    public class SafeAreaConfig
    {
        public bool left = true;
        public bool right = true;
        public bool top = true;
        public bool bottom = true;


        public virtual SafeAreaConfig Clone()
        {
            SafeAreaConfig clone = new SafeAreaConfig();
            clone.left = left;
            clone.right = right;
            clone.top = top;
            clone.bottom = bottom;

            return clone;
        }
    }
}