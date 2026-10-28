using CS;
using Game.Primary.GamePlaySystem;
using Game.Primary.RunSystem;
using Game.Primary.UI;
using UnityEngine;

namespace Game.Primary.CampaignModeSystem
{
    /// <summary>
    /// Điều phối scene CampaignMode: menu -> load level json -> GamePlayCore.Init/StartPlay -> popup win/lose.
    /// </summary>
    public class CampaignMode : SingletonBind<CampaignMode>
    {
        [SerializeField] private GamePlayCore gamePlayCore;

        public int CurrentLevel { get; private set; }
        public GamePlayCore GamePlayCore => gamePlayCore;



        protected override void OnAwake()
        {
            gamePlayCore.OnWin += GamePlayCore_OnWin;
            gamePlayCore.OnLose += GamePlayCore_OnLose;
            gamePlayCore.OnDoubleOrNothingOffer += GamePlayCore_OnDoubleOrNothingOffer;
            gamePlayCore.OnPauseRequested += GamePlayCore_OnPauseRequested;
        }

        protected override void OnDestroy()
        {
            if (gamePlayCore != null)
            {
                gamePlayCore.OnWin -= GamePlayCore_OnWin;
                gamePlayCore.OnLose -= GamePlayCore_OnLose;
                gamePlayCore.OnDoubleOrNothingOffer -= GamePlayCore_OnDoubleOrNothingOffer;
                gamePlayCore.OnPauseRequested -= GamePlayCore_OnPauseRequested;
            }

            base.OnDestroy();
        }

        private void Start()
        {
            Application.targetFrameRate = 60;

            if (RunDI.AgeConfirmed)
                OpenHome();
            else
                PopupAgeGate.Instance.Open();
        }



        #region Public API

        public void OpenHome()
        {
            PopupHome.Instance.Open();
        }

        public void PlayLevel(int level)
        {
            var levelData = LevelLoader.Load(level);
            if (levelData == null)
                return;

            CurrentLevel = level;
            if (gamePlayCore.Init(levelData))
                gamePlayCore.StartPlay();
        }

        public void Replay()
        {
            PlayLevel(CurrentLevel);
        }

        public void QuitToHome()
        {
            gamePlayCore.StopPlay();
            OpenHome();
        }

        public void ResumeGame()
        {
            gamePlayCore.SetPaused(false);
        }

        #endregion



        #region GamePlayCore Events

        private void GamePlayCore_OnWin(RunResult result)
        {
            RunDI.RecordRun(result);
            PopupWin.Instance.Open(result);
        }

        private void GamePlayCore_OnLose(RunResult result)
        {
            RunDI.RecordRun(result);
            PopupLose.Instance.Open(result);
        }

        private void GamePlayCore_OnDoubleOrNothingOffer(DoubleOrNothingOffer offer)
        {
            PopupDoubleOrNothing.Instance.Open(offer);
        }

        private void GamePlayCore_OnPauseRequested()
        {
            if (gamePlayCore.IsPlaying == false)
                return;

            gamePlayCore.SetPaused(true);
            PopupPause.Instance.Open();
        }

        #endregion
    }
}
