using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using Game.Primary.GamePlaySystem.Rules;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZeroX.ButtonSystem;

namespace Game.Primary.GamePlaySystem
{
    /// <summary>
    /// UI của màn gameplay (không phải popup): round, stamina, số viên còn lại, lượt, thẻ item, nút chọn mục tiêu...
    /// </summary>
    public class GameHud : MonoBehaviour
    {
        [SerializeField] private CanvasGroup root;

        [Header("Top")]
        [SerializeField] private TMP_Text stageText;
        [SerializeField] private TMP_Text modeText;
        [SerializeField] private TMP_Text cashText;
        [SerializeField] private ZButton buttonPause;

        [Header("Stamina")]
        [SerializeField] private RectTransform playerHeartsRow;
        [SerializeField] private RectTransform dealerHeartsRow;
        [SerializeField] private Image heartPrefab;
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text dealerNameText;
        [SerializeField] private GameObject playerLockedBadge;
        [SerializeField] private GameObject dealerLockedBadge;
        [SerializeField] private GameObject playerCuffedBadge;
        [SerializeField] private GameObject dealerCuffedBadge;
        [SerializeField] private Color heartOnColor = new Color(1f, 0.25f, 0.55f);
        [SerializeField] private Color heartOffColor = new Color(1f, 1f, 1f, 0.15f);
        [SerializeField] private Color heartLockedColor = new Color(0.7f, 0.05f, 0.08f);

        [Header("Shells")]
        [SerializeField] private TMP_Text liveCountText;
        [SerializeField] private TMP_Text blankCountText;
        [SerializeField] private GameObject flippedBadge;
        [SerializeField] private GameObject boostBadge;
        [SerializeField] private TMP_Text knownText;

        [Header("Turn")]
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text hintText;

        [Header("Item card")]
        [SerializeField] private GameObject itemCard;
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text itemDescText;
        [SerializeField] private TMP_Text itemBlockedText;
        [SerializeField] private ZButton buttonUse;

        [Header("Aim")]
        [SerializeField] private GameObject aimPanel;
        [SerializeField] private ZButton buttonZapHer;
        [SerializeField] private ZButton buttonZapSelf;
        [SerializeField] private ZButton buttonCancelAim;

        [Header("Steal")]
        [SerializeField] private GameObject stealPanel;
        [SerializeField] private Image stealTimerFill;

        [Header("Banner")]
        [SerializeField] private CanvasGroup banner;
        [SerializeField] private TMP_Text bannerTitle;
        [SerializeField] private TMP_Text bannerSub;

        [Header("Flash")]
        [SerializeField] private Image flash;

        [Header("Debug")]
        [SerializeField] private GameObject debugPanel;
        [SerializeField] private TMP_Text debugText;
        [SerializeField] private ZButton buttonDebug;

        private readonly List<Image> playerHearts = new List<Image>();
        private readonly List<Image> dealerHearts = new List<Image>();
        private Tween hintTween;


        public event Action OnUseClicked;
        public event Action OnZapHerClicked;
        public event Action OnZapSelfClicked;
        public event Action OnCancelAimClicked;
        public event Action OnPauseClicked;



        private void Awake()
        {
            buttonUse.onClick.AddListener(() => OnUseClicked?.Invoke());
            buttonZapHer.onClick.AddListener(() => OnZapHerClicked?.Invoke());
            buttonZapSelf.onClick.AddListener(() => OnZapSelfClicked?.Invoke());
            buttonCancelAim.onClick.AddListener(() => OnCancelAimClicked?.Invoke());
            buttonPause.onClick.AddListener(() => OnPauseClicked?.Invoke());

            if (buttonDebug != null)
            {
                buttonDebug.gameObject.SetActive(Debug.isDebugBuild);
                buttonDebug.onClick.AddListener(() => debugPanel.SetActive(debugPanel.activeSelf == false));
            }

            if (debugPanel != null)
                debugPanel.SetActive(false);

            ResetPanels();
        }

        public void SetVisible(bool visible)
        {
            root.alpha = visible ? 1 : 0;
            root.interactable = visible;
            root.blocksRaycasts = visible;
            if (visible == false)
                ResetPanels();
        }

        public void ResetPanels()
        {
            itemCard.SetActive(false);
            aimPanel.SetActive(false);
            stealPanel.SetActive(false);
            banner.alpha = 0;
            flash.color = Color.clear;
            flippedBadge.SetActive(false);
            boostBadge.SetActive(false);
            playerLockedBadge.SetActive(false);
            dealerLockedBadge.SetActive(false);
            playerCuffedBadge.SetActive(false);
            dealerCuffedBadge.SetActive(false);
            knownText.text = "";
            hintText.text = "";
            turnText.text = "";
        }



        #region Top

        public void SetStage(string stage, string mode)
        {
            stageText.text = stage;
            modeText.text = mode;
        }

        public void SetCash(string text)
        {
            cashText.text = text;
        }

        #endregion



        #region Stamina

        public void SetupHearts(int maxCharges, string playerName, string dealerName)
        {
            BuildHearts(playerHeartsRow, playerHearts, maxCharges);
            BuildHearts(dealerHeartsRow, dealerHearts, maxCharges);
            playerNameText.text = playerName;
            dealerNameText.text = dealerName;
        }

        private void BuildHearts(RectTransform row, List<Image> hearts, int count)
        {
            foreach (var heart in hearts)
            {
                if (heart != null)
                    Destroy(heart.gameObject);
            }

            hearts.Clear();
            for (int i = 0; i < count; i++)
            {
                var heart = Instantiate(heartPrefab, row);
                heart.gameObject.SetActive(true);
                hearts.Add(heart);
            }
        }

        public void SetHearts(int playerId, int value, bool locked, bool animate)
        {
            var hearts = playerId == PlayerIds.Player ? playerHearts : dealerHearts;
            for (int i = 0; i < hearts.Count; i++)
            {
                bool on = i < value;
                var color = on ? (locked ? heartLockedColor : heartOnColor) : heartOffColor;
                if (animate && hearts[i].color != color)
                {
                    hearts[i].transform.DOKill(true);
                    hearts[i].transform.DOPunchScale(Vector3.one * 0.5f, 0.35f, 8).SetLink(hearts[i].gameObject);
                }

                hearts[i].color = color;
            }

            (playerId == PlayerIds.Player ? playerLockedBadge : dealerLockedBadge).SetActive(locked);
        }

        public void SetCuffed(int playerId, bool cuffed)
        {
            (playerId == PlayerIds.Player ? playerCuffedBadge : dealerCuffedBadge).SetActive(cuffed);
        }

        #endregion



        #region Shells

        /// <summary> Số lượng công khai + những gì người chơi đã biết </summary>
        public void RefreshShellInfo(GameState state, GameplayConfig config)
        {
            var sg = state.shotgun;
            liveCountText.text = $"{config.liveName} x{sg.PublicLiveRemaining}";
            blankCountText.text = $"{config.blankName} x{sg.PublicBlankRemaining}";
            flippedBadge.SetActive(sg.Current != null && sg.Current.inverted);
            boostBadge.SetActive(sg.boosted);

            var sb = new StringBuilder();
            for (int i = 0; i < sg.shells.Count; i++)
            {
                var shell = sg.shells[i];
                if (shell.knownBy.Contains(PlayerIds.Player) == false)
                    continue;

                if (sb.Length > 0)
                    sb.Append("   ");

                string color = ColorUtility.ToHtmlStringRGB(config.ShellColor(shell.type));
                string label = i == 0 ? "NOW" : GameTexts.Ordinal(i + 1);
                sb.Append($"{label}: <color=#{color}>{config.ShellName(shell.type)}</color>");
            }

            knownText.text = sb.ToString();
        }

        public void SetDebug(GameState state, GameplayConfig config)
        {
            if (debugText == null || debugPanel == null || debugPanel.activeSelf == false)
                return;

            var sb = new StringBuilder("SEED ").Append(state.rng.Seed).Append('\n');
            foreach (var shell in state.shotgun.shells)
                sb.Append(shell.type == ShellType.Live ? "H " : "c ");
            sb.Append("\nP:").Append(state.Player.charges).Append(" D:").Append(state.Dealer.charges);
            debugText.text = sb.ToString();
        }

        #endregion



        #region Turn - Hint

        public void SetTurn(bool isPlayer)
        {
            turnText.text = isPlayer ? GameTexts.YourTurn : GameTexts.DealerTurn;
            turnText.color = isPlayer ? new Color(1f, 0.55f, 0.75f) : new Color(0.75f, 0.75f, 0.8f);
            turnText.transform.DOKill(true);
            turnText.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 6).SetLink(turnText.gameObject);
        }

        public void ShowHint(string text)
        {
            hintTween?.Kill();
            hintText.text = text;
            hintText.alpha = 1;
        }

        public void ShowHintTemporary(string text, float duration = 2f)
        {
            ShowHint(text);
            hintTween = DOTween.Sequence().AppendInterval(duration).Append(hintText.DOFade(0, 0.3f)).SetLink(gameObject);
        }

        #endregion



        #region Item card - Aim - Steal

        public void ShowItemCard(ItemDefinition def, string blockedReason, bool canUse)
        {
            itemCard.SetActive(true);
            itemNameText.text = def != null ? def.displayName : "";
            itemNameText.color = def != null ? Color.Lerp(def.color, Color.white, 0.35f) : Color.white;
            itemDescText.text = def != null ? def.description : "";
            itemBlockedText.text = blockedReason ?? "";
            itemBlockedText.gameObject.SetActive(string.IsNullOrEmpty(blockedReason) == false);
            buttonUse.gameObject.SetActive(canUse);
        }

        public void HideItemCard()
        {
            itemCard.SetActive(false);
        }

        public void ShowAim(bool show)
        {
            aimPanel.SetActive(show);
        }

        public void ShowSteal(bool show)
        {
            stealPanel.SetActive(show);
            if (show)
                stealTimerFill.fillAmount = 1;
        }

        public void SetStealTimer(float ratio)
        {
            stealTimerFill.fillAmount = Mathf.Clamp01(ratio);
        }

        #endregion



        #region Banner - Flash

        public IEnumerator ShowBanner(string title, string sub, Color color, float duration)
        {
            bannerTitle.text = title;
            bannerTitle.color = color;
            bannerSub.text = sub ?? "";
            banner.DOKill();
            banner.transform.localScale = Vector3.one * 1.2f;
            var seq = DOTween.Sequence();
            seq.Append(banner.DOFade(1, 0.25f));
            seq.Join(banner.transform.DOScale(1, 0.35f).SetEase(Ease.OutBack));
            seq.AppendInterval(duration);
            seq.Append(banner.DOFade(0, 0.3f));
            seq.SetLink(gameObject);
            yield return seq.WaitForCompletion();
        }

        public void Flash(Color color, float duration = 0.5f)
        {
            flash.DOKill();
            flash.color = color;
            flash.DOFade(0, duration).SetEase(Ease.OutQuad).SetLink(gameObject);
        }

        #endregion
    }
}
