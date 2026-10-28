namespace ZeroX.WindowSystem
{
    public static class WindowConst
    {
        public static class MenuCreateAsset
        {
            public const string Root = "ZeroX/Window System";
        }
        
        public static class MenuCreateTemplate
        {
            public const string Root = "GameObject/ZeroX/Window Template";

            public const string CanvasOverlay_Portrait = Root + "/Canvas - Overlay - Portrait";
            public const string CanvasOverlay_Landscape = Root + "/Canvas - Overlay - Landscape";
            
            public const string Overlay = Root + "/Overlay";
            public const string NonOverlay = Root + "/Non Overlay";
        }
        
        public static class MenuItem
        {
            public const string Root = "Tools/ZeroX/Window System";
            public const string WindowSortingTable = Root + "/Window Sorting Table";
            public const string GlobalWindowTransitionTable = Root + "/Global Window Transition Table";
            public const string WindowTemplateTable = Root + "/Window Template Table";
        }
        
        public static class ResourcesPath
        {
            private const string RootFolder = "ZeroX/WindowSystem";
            
            public const string WindowSortingTable = RootFolder + "/Window Sorting";
            public const string GlobalWindowTransitionTable = RootFolder + "/Global Window Transition";
            public const string WindowTemplateTable = RootFolder + "/Window Template";
        }
        
        public static class AbsolutePathImplement
        {
            private const string RootFolder = "Assets/ZeroX/Core/UI/WindowSystem/Implement";
            private const string ResourcesFolder = RootFolder + "/Resources/ZeroX/WindowSystem";
            
            public const string WindowSortingTable = ResourcesFolder + "/Window Sorting.asset";
            public const string GlobalWindowTransitionTable = ResourcesFolder + "/Global Window Transition.asset";
            public const string WindowTemplateTable = ResourcesFolder + "/Window Template.asset";
            public const string DefaultWindowTemplateFolder = RootFolder + "/WindowTemplate";
        }
    }
}