namespace ZeroX.WindowSystem
{
    [System.Serializable]
    public class WindowSortingLayerData
    {
        public WindowSortingLayerId id;
        public int spacingStart = 5;

        public WindowSortingLayerData()
        {}

        public WindowSortingLayerData(WindowSortingLayerId id)
        {
            this.id = id;
        }
    }
}