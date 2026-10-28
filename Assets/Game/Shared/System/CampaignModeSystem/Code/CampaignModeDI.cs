using ZeroX.RxSystem;
using ZeroX.SaveGameSystem;

namespace Game.Shared.CampaignModeSystem
{
    public static class CampaignModeDI
    {
        public const string root_key = "campaignMode";
            public const string maxLevelCompleted_key = "maxLevelCompleted";
            
            
            
        //SGNode
        private static SGNode RootNode => SaveGame.Default[root_key];
        
        
        //Event
        public static readonly Subject<int> OnLevelCompleted = new Subject<int>();






        public static int MaxLevelCompleted
        {
            get => RootNode[maxLevelCompleted_key].GetInt();
            private set => RootNode[maxLevelCompleted_key].SetInt(value);
        }

        public static int MaxLevelCanPlay => MaxLevelCompleted + 1;

        public static int SelectedLevel
        {
            get
            {
                int selectedLevel = MaxLevelCanPlay;
                if (CampaignLevelTable.DictRow.ContainsKey(selectedLevel) == false)
                    selectedLevel = CampaignLevelTable.MaxLevel;

                return selectedLevel;
            }
        }


        public static void SetLevelCompleted(int level)
        {
            if(level <= MaxLevelCompleted)
                return;
            
            MaxLevelCompleted = level;
            
            //Event
            OnLevelCompleted.Emit(level);
        }

        public static void Cheat_SetMaxLevelCompleted(int level)
        {
            MaxLevelCompleted = level;
        }
    }
}