using System;
using CS;
using Game.Primary.GamePlaySystem.Rules;
using Game.Primary.MistressSystem;
using Game.Primary.StaminaSystem;
using Game.Primary.TableSystem;
using Game.Primary.WandSystem;
using UnityEngine;

namespace Game.Primary.GamePlaySystem
{
    /// <summary>
    /// Điểm truy cập trung tâm của gameplay: ref tới Module, Service, UI.
    /// Nhận LevelData qua Init(), chạy bằng StartPlay(), báo kết quả qua OnWin/OnLose
    /// để CampaignMode (hoặc một TryPlayMode) hiển thị popup.
    /// </summary>
    public class GamePlayCore : SingletonBind<GamePlayCore>
    {
        [SerializeField] private GameplayConfig config;

        [Header("Modules")]
        [SerializeField] private TableView table;
        [SerializeField] private WandView wand;
        [SerializeField] private ShellDisplayView shellDisplay;
        [SerializeField] private StaminaMonitorView staminaMonitor;
        [SerializeField] private MistressView mistress;

        [Header("Services")]
        [SerializeField] private TurnFlowService turnFlow;
        [SerializeField] private PlayerInputService playerInput;
        [SerializeField] private SfxService sfx;
        [SerializeField] private HapticService haptic;
        [SerializeField] private CameraController cameraController;

        [Header("UI")]
        [SerializeField] private GameHud hud;


        //Property
        public GameplayConfig Config => config;
        public TableView Table => table;
        public WandView Wand => wand;
        public ShellDisplayView ShellDisplay => shellDisplay;
        public StaminaMonitorView StaminaMonitor => staminaMonitor;
        public MistressView Mistress => mistress;
        public TurnFlowService TurnFlow => turnFlow;
        public PlayerInputService PlayerInput => playerInput;
        public SfxService Sfx => sfx;
        public HapticService Haptic => haptic;
        public CameraController CameraController => cameraController;
        public GameHud Hud => hud;

        public LevelData LevelData { get; private set; }
        public GameRules Rules { get; private set; }
        public GameState State => Rules?.State;
        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }


        //Event
        public event Action OnStartPlay;
        public event Action<RunResult> OnWin;
        public event Action<RunResult> OnLose;
        public event Action<DoubleOrNothingOffer> OnDoubleOrNothingOffer;
        public event Action OnPauseRequested;



        protected override void OnAwake()
        {
            turnFlow.Setup(this);
            playerInput.Setup(this);
            hud.OnPauseClicked += () => OnPauseRequested?.Invoke();
            hud.SetVisible(false);
        }

        protected override void OnDestroy()
        {
            OnStartPlay = null;
            OnWin = null;
            OnLose = null;
            OnDoubleOrNothingOffer = null;
            OnPauseRequested = null;
            base.OnDestroy();
        }



        #region Public API

        public bool Init(LevelData levelData)
        {
            if (levelData == null || levelData.stages == null || levelData.stages.Count == 0)
            {
                Debug.LogError("GamePlayCore.Init: LevelData không hợp lệ");
                return false;
            }

            StopPlay();

            LevelData = levelData;
            int seed = config.fixedSeed != 0 ? config.fixedSeed : Environment.TickCount;
            Rules = new GameRules(new GameState(levelData.GetMode(), seed));
            ResetViews();
            return true;
        }

        public void StartPlay()
        {
            if (Rules == null)
            {
                Debug.LogError("GamePlayCore.StartPlay: chưa Init");
                return;
            }

            IsPlaying = true;
            IsPaused = false;
            OnStartPlay?.Invoke();
            StartCoroutine(turnFlow.RunGame());
        }

        public void StopPlay()
        {
            StopAllCoroutines();
            turnFlow.Stop();
            IsPlaying = false;
            IsPaused = false;
            playerInput.EndTurn();
            hud.SetVisible(false);
            ResetViews();
        }

        public void SetPaused(bool paused)
        {
            IsPaused = paused;
        }

        #endregion



        #region Internal

        private void ResetViews()
        {
            table.ClearAll();
            shellDisplay.Clear();
            wand.ResetToRest();
            mistress.ResetState();
        }

        public void FinishRun(RunResult result)
        {
            IsPlaying = false;
            playerInput.EndTurn();
            hud.SetVisible(false);

            if (result.won)
                OnWin?.Invoke(result);
            else
                OnLose?.Invoke(result);
        }

        public void RaiseDoubleOrNothingOffer(DoubleOrNothingOffer offer)
        {
            //Không ai xử lý (ví dụ TryPlayMode) -> mặc định Cash Out
            if (OnDoubleOrNothingOffer == null)
                offer.Resolve(false);
            else
                OnDoubleOrNothingOffer.Invoke(offer);
        }

        #endregion
    }
}
