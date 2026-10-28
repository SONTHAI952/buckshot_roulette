using Game.Primary.GamePlaySystem;
using Game.Primary.GamePlaySystem.Rules;
using ZeroX.RxSystem;
using ZeroX.SaveGameSystem;

namespace Game.Primary.RunSystem
{
    /// <summary>
    /// Dữ liệu lưu của game: xác nhận 18+, thành tích Story / Double or Nothing.
    /// </summary>
    public static class RunDI
    {
        public const string root_key = "teaseRoulette";
            public const string ageConfirmed_key = "ageConfirmed";
            public const string totalRuns_key = "totalRuns";
            public const string storyWins_key = "storyWins";
            public const string storyBestCash_key = "storyBestCash";
            public const string donBestCash_key = "donBestCash";
            public const string donBestSets_key = "donBestSets";
            public const string totalCash_key = "totalCash";


        //SGNode
        private static SGNode RootNode => SaveGame.Default[root_key];


        //Event
        public static readonly Subject<RunResult> OnRunRecorded = new Subject<RunResult>();



        public static bool AgeConfirmed
        {
            get => RootNode[ageConfirmed_key].GetBool();
            set => RootNode[ageConfirmed_key].SetBool(value);
        }

        public static int TotalRuns
        {
            get => RootNode[totalRuns_key].GetInt();
            private set => RootNode[totalRuns_key].SetInt(value);
        }

        public static int StoryWins
        {
            get => RootNode[storyWins_key].GetInt();
            private set => RootNode[storyWins_key].SetInt(value);
        }

        public static int StoryBestCash
        {
            get => RootNode[storyBestCash_key].GetInt();
            private set => RootNode[storyBestCash_key].SetInt(value);
        }

        public static int DoubleOrNothingBestCash
        {
            get => RootNode[donBestCash_key].GetInt();
            private set => RootNode[donBestCash_key].SetInt(value);
        }

        public static int DoubleOrNothingBestSets
        {
            get => RootNode[donBestSets_key].GetInt();
            private set => RootNode[donBestSets_key].SetInt(value);
        }

        public static int TotalCash
        {
            get => RootNode[totalCash_key].GetInt();
            private set => RootNode[totalCash_key].SetInt(value);
        }



        public static void RecordRun(RunResult result)
        {
            TotalRuns++;

            if (result.won)
            {
                TotalCash += result.cash;
                if (result.mode == GameMode.Story)
                {
                    StoryWins++;
                    if (result.cash > StoryBestCash)
                        StoryBestCash = result.cash;
                }
                else
                {
                    if (result.cash > DoubleOrNothingBestCash)
                        DoubleOrNothingBestCash = result.cash;
                }
            }

            if (result.mode == GameMode.DoubleOrNothing && result.setsCleared > DoubleOrNothingBestSets)
                DoubleOrNothingBestSets = result.setsCleared;

            OnRunRecorded.Emit(result);
        }
    }
}
