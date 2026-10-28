using Game.Primary.PlatformLayoutSystem;
using Game.Primary.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZeroX.ButtonSystem;
using ZeroX.UI;
using ZeroX.WindowSystem;

namespace Game.Primary.GamePlaySystem.Editor
{
    public static partial class TeaseRouletteBuilder
    {
        private static readonly Color PanelColor = new Color(0.09f, 0.025f, 0.06f, 0.96f);
        private static readonly Color DimColor = new Color(0.02f, 0f, 0.01f, 0.82f);
        private static readonly Color GreyButton = new Color(0.28f, 0.22f, 0.26f);
        private static readonly Color Gold = new Color(1f, 0.82f, 0.4f);


        #region UI Helper

        public static RectTransform UIObject(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = UILayer;
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        public static void Stretch(RectTransform rt, float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
        }

        public static void Place(RectTransform rt, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
        {
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = pivot;
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
        }

        public static Image AddImage(RectTransform rt, Color color, Sprite sprite = null, bool raycast = true)
        {
            var image = rt.gameObject.AddComponent<Image>();
            image.color = color;
            image.sprite = sprite;
            image.raycastTarget = raycast;
            if (sprite != null && sprite.border != Vector4.zero)
                image.type = Image.Type.Sliced;
            return image;
        }

        public static Image AddPanel(RectTransform rt, Color color, bool outline = true)
        {
            var image = AddImage(rt, color, LoadSprite("ui_rounded.png"));
            if (outline)
            {
                var o = rt.gameObject.AddComponent<Outline>();
                o.effectColor = new Color(Pink.r, Pink.g, Pink.b, 0.7f);
                o.effectDistance = new Vector2(3, -3);
            }

            return image;
        }

        public static TextMeshProUGUI AddText(RectTransform parent, string name, string text, float size, Color color,
            TextAlignmentOptions align = TextAlignmentOptions.Center, FontStyles style = FontStyles.Normal)
        {
            var rt = UIObject(name, parent);
            Stretch(rt);
            var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.alignment = align;
            tmp.fontStyle = style;
            tmp.raycastTarget = false;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            tmp.richText = true;
            return tmp;
        }

        public static ZButton AddButton(RectTransform parent, string name, string label, Color color, float fontSize = 48)
        {
            var rt = UIObject(name, parent);
            var image = AddImage(rt, color, LoadSprite("ui_rounded.png"));
            var button = rt.gameObject.AddComponent<ZButton>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            var text = AddText(rt, "Text", label, fontSize, Color.white, TextAlignmentOptions.Center, FontStyles.Bold);
            text.margin = new Vector4(10, 0, 10, 0);
            return button;
        }

        private static GameObject CreateCanvasRoot(string name, Transform parent, bool forWindow)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = UILayer;
            if (parent != null)
                go.transform.SetParent(parent, false);

            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.planeDistance = 10;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = OrientationUtility.PortraitReference;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            go.AddComponent<GraphicRaycaster>();
            go.AddComponent<CanvasGroup>();
            go.AddComponent<SetGUICameraForCanvas>();
            go.AddComponent<CanvasScalerOrientation>();
            if (forWindow)
                go.AddComponent<Window>();
            return go;
        }

        #endregion



        #region Popups

        public static void BuildPopups()
        {
            BuildPopupAgeGate();
            BuildPopupHome();
            BuildPopupWin();
            BuildPopupLose();
            BuildPopupDoubleOrNothing();
            BuildPopupPause();
            Debug.Log("[TeaseRoulette] Popups built");
        }

        /// <summary> Root -> Background + Content, gắn sẵn Window + WindowController </summary>
        private static (GameObject root, RectTransform content, T controller) CreatePopup<T>(string name, Vector2 contentSize) where T : Component
        {
            var root = CreateCanvasRoot(name, null, true);

            var background = UIObject("Background", root.transform);
            Stretch(background);
            AddImage(background, DimColor);

            var content = UIObject("Content", root.transform);
            Place(content, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, contentSize);
            AddPanel(content, PanelColor);

            var window = root.GetComponent<Window>();
            SetRef(window, "content", content);
            SetRef(window, "canvasGroup", root.GetComponent<CanvasGroup>());
            SetRef(window, "canvas", root.GetComponent<Canvas>());
            SetValue(window, "useGuiCamera", true);

            var controller = root.AddComponent<T>();
            SetRef(controller, "window", window);
            return (root, content, controller);
        }

        private static void SavePopup(GameObject root, string name)
        {
            string dir = $"{UIDir}/{name}/Resources/Window";
            EnsureFolder(dir);
            SetLayerRecursive(root, UILayer);
            PrefabUtility.SaveAsPrefabAsset(root, $"{dir}/{name}.prefab");
            Object.DestroyImmediate(root);
        }

        private static TextMeshProUGUI PopupText(RectTransform content, string name, string text, float size, Color color, float top, float height, FontStyles style = FontStyles.Normal)
        {
            var t = AddText(content, name, text, size, color, TextAlignmentOptions.Center, style);
            var rt = t.rectTransform;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -top);
            rt.sizeDelta = new Vector2(-80, height);
            return t;
        }

        private static ZButton PopupButton(RectTransform content, string name, string label, Color color, float bottom, float width = 640, float height = 140)
        {
            var button = AddButton(content, name, label, color, 50);
            Place((RectTransform)button.transform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, bottom), new Vector2(width, height));
            return button;
        }

        private static void BuildPopupAgeGate()
        {
            var (root, content, popup) = CreatePopup<PopupAgeGate>("PopupAgeGate", new Vector2(920, 1000));
            SetRef(popup, "textTitle", PopupText(content, "Text Title", "18+ ONLY", 110, Pink, 60, 140, FontStyles.Bold));
            SetRef(popup, "textBody", PopupText(content, "Text Body",
                "This game contains adult themes:\nBDSM gear, adult toys and suggestive dialogue.\nAll characters are adults.\n\nAre you 18 or older?",
                40, new Color(1, 0.9f, 0.95f), 220, 420));
            SetRef(popup, "buttonConfirm", PopupButton(content, "Button Confirm", "I'M 18+  ENTER", Pink, 210));
            SetRef(popup, "buttonLeave", PopupButton(content, "Button Leave", "LEAVE", GreyButton, 50, 640, 120));
            SavePopup(root, "PopupAgeGate");
        }

        private static void BuildPopupHome()
        {
            var (root, content, popup) = CreatePopup<PopupHome>("PopupHome", new Vector2(920, 1040));
            SetRef(popup, "textTitle", PopupText(content, "Text Title", "TEASE\nROULETTE", 110, Pink, 40, 260, FontStyles.Bold));
            PopupText(content, "Text Subtitle", "a game of HOT & COLD", 40, new Color(1, 0.85f, 0.92f), 300, 60, FontStyles.Italic);
            SetRef(popup, "buttonStory", PopupButton(content, "Button Story", "STORY", Pink, 470));
            SetRef(popup, "buttonDoubleOrNothing", PopupButton(content, "Button Double Or Nothing", "DOUBLE OR NOTHING", new Color(0.6f, 0.15f, 0.45f), 315));
            SetRef(popup, "buttonSetting", PopupButton(content, "Button Setting", "SETTINGS", GreyButton, 175, 640, 120));
            var stats = PopupText(content, "Text Stats", "", 30, new Color(1, 1, 1, 0.7f), 0, 120);
            var rt = stats.rectTransform;
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.anchoredPosition = new Vector2(0, 50);
            SetRef(popup, "textStats", stats);
            SavePopup(root, "PopupHome");
        }

        private static void BuildPopupWin()
        {
            var (root, content, popup) = CreatePopup<PopupWin>("PopupWin", new Vector2(920, 1040));
            SetRef(popup, "textTitle", PopupText(content, "Text Title", "SHE SURRENDERS", 80, Pink, 50, 110, FontStyles.Bold));
            SetRef(popup, "textCash", PopupText(content, "Text Cash", "$0", 120, Gold, 170, 150, FontStyles.Bold));
            SetRef(popup, "textDetail", PopupText(content, "Text Detail", "", 36, new Color(1, 0.9f, 0.95f), 330, 330));
            SetRef(popup, "buttonPlayAgain", PopupButton(content, "Button Play Again", "PLAY AGAIN", Pink, 200));
            SetRef(popup, "buttonMenu", PopupButton(content, "Button Menu", "MENU", GreyButton, 50, 640, 120));
            SavePopup(root, "PopupWin");
        }

        private static void BuildPopupLose()
        {
            var (root, content, popup) = CreatePopup<PopupLose>("PopupLose", new Vector2(920, 940));
            SetRef(popup, "textTitle", PopupText(content, "Text Title", "YOU BROKE", 100, Blue, 60, 140, FontStyles.Bold));
            SetRef(popup, "textDetail", PopupText(content, "Text Detail", "", 40, new Color(1, 0.9f, 0.95f), 230, 340));
            SetRef(popup, "buttonRetry", PopupButton(content, "Button Retry", "RETRY", Pink, 200));
            SetRef(popup, "buttonMenu", PopupButton(content, "Button Menu", "MENU", GreyButton, 50, 640, 120));
            SavePopup(root, "PopupLose");
        }

        private static void BuildPopupDoubleOrNothing()
        {
            var (root, content, popup) = CreatePopup<PopupDoubleOrNothing>("PopupDoubleOrNothing", new Vector2(920, 960));
            SetRef(popup, "textTitle", PopupText(content, "Text Title", "SET CLEARED", 80, Pink, 50, 110, FontStyles.Bold));
            SetRef(popup, "textCash", PopupText(content, "Text Cash", "$0", 120, Gold, 170, 150, FontStyles.Bold));
            SetRef(popup, "textDetail", PopupText(content, "Text Detail", "", 38, new Color(1, 0.9f, 0.95f), 330, 220));
            SetRef(popup, "buttonDouble", PopupButton(content, "Button Double", "DOUBLE IT", Pink, 200));
            SetRef(popup, "buttonCashOut", PopupButton(content, "Button Cash Out", "CASH OUT", new Color(0.2f, 0.45f, 0.7f), 50, 640, 120));
            SavePopup(root, "PopupDoubleOrNothing");
        }

        private static void BuildPopupPause()
        {
            var (root, content, popup) = CreatePopup<PopupPause>("PopupPause", new Vector2(820, 760));
            PopupText(content, "Text Title", "PAUSED", 90, Pink, 60, 120, FontStyles.Bold);
            SetRef(popup, "buttonResume", PopupButton(content, "Button Resume", "RESUME", Pink, 380, 560));
            SetRef(popup, "buttonSetting", PopupButton(content, "Button Setting", "SETTINGS", GreyButton, 220, 560, 130));
            SetRef(popup, "buttonQuit", PopupButton(content, "Button Quit", "QUIT TO MENU", new Color(0.35f, 0.1f, 0.15f), 60, 560, 130));
            SavePopup(root, "PopupPause");
        }

        #endregion



        #region HUD

        public static GameHud BuildHud(Transform parent)
        {
            var root = CreateCanvasRoot("HUD", parent, false);
            var rootRt = (RectTransform)root.transform;
            var hud = root.AddComponent<GameHud>();
            SetRef(hud, "root", root.GetComponent<CanvasGroup>());

            var heartSprite = LoadSprite("ui_heart.png");
            var white = new Color(1, 1, 1, 0.95f);

            //Flash (dưới cùng, không chặn raycast)
            var flash = UIObject("Flash", rootRt);
            Stretch(flash);
            SetRef(hud, "flash", AddImage(flash, Color.clear, null, false));

            //Top-left: round + mode
            var stage = AddText(rootRt, "Text Stage", "ROUND 1/3", 54, white, TextAlignmentOptions.TopLeft, FontStyles.Bold);
            Place(stage.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(40, -40), new Vector2(620, 70));
            SetRef(hud, "stageText", stage);
            var mode = AddText(rootRt, "Text Mode", "STORY", 34, new Color(1, 0.6f, 0.8f), TextAlignmentOptions.TopLeft);
            Place(mode.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(40, -110), new Vector2(760, 50));
            SetRef(hud, "modeText", mode);

            //Top-right: pause + cash
            var pause = AddButton(rootRt, "Button Pause", "II", new Color(0.15f, 0.05f, 0.1f, 0.9f), 50);
            Place((RectTransform)pause.transform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-40, -40), new Vector2(120, 120));
            SetRef(hud, "buttonPause", pause);
            var cash = AddText(rootRt, "Text Cash", "", 40, Gold, TextAlignmentOptions.TopRight, FontStyles.Bold);
            Place(cash.rectTransform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-40, -340), new Vector2(700, 60));
            SetRef(hud, "cashText", cash);

            //Heart template
            var heartTemplate = UIObject("Heart Template", rootRt);
            heartTemplate.sizeDelta = new Vector2(62, 62);
            var heartImage = AddImage(heartTemplate, Pink, heartSprite, false);
            heartImage.preserveAspect = true;
            heartTemplate.gameObject.SetActive(false);
            SetRef(hud, "heartPrefab", heartImage);

            //Dealer stamina (top-right, dưới nút pause để không che mặt Mistress)
            var dealerBox = UIObject("Dealer Stamina", rootRt);
            Place(dealerBox, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-40, -175), new Vector2(560, 150));
            var dealerName = AddText(dealerBox, "Text Name", "MISTRESS", 36, new Color(1, 0.6f, 0.8f), TextAlignmentOptions.TopRight, FontStyles.Bold);
            SetRef(hud, "dealerNameText", dealerName);
            var dealerRow = BuildHeartRow(dealerBox, "Hearts", new Vector2(0, -50), TextAnchor.UpperRight);
            SetRef(hud, "dealerHeartsRow", dealerRow);
            SetRef(hud, "dealerCuffedBadge", BuildBadge(dealerBox, "Badge Cuffed", "CUFFED", new Color(1, 0.55f, 0.8f), new Vector2(1, 0), new Vector2(-100, -10)).gameObject);
            SetRef(hud, "dealerLockedBadge", BuildBadge(dealerBox, "Badge Locked", "NO SAFE WORDS", new Color(0.8f, 0.1f, 0.15f), new Vector2(1, 0), new Vector2(-330, -10)).gameObject);

            //Turn (top-left, dưới round/mode)
            var turn = AddText(rootRt, "Text Turn", "YOUR TURN", 54, white, TextAlignmentOptions.TopLeft, FontStyles.Bold);
            Place(turn.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(40, -165), new Vector2(700, 75));
            SetRef(hud, "turnText", turn);

            //Shell info (middle-left)
            var shellBox = UIObject("Shell Info", rootRt);
            Place(shellBox, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(30, 140), new Vector2(430, 300));
            AddPanel(shellBox, new Color(0.05f, 0.01f, 0.03f, 0.6f), false);
            var live = AddText(shellBox, "Text Live", "HOT x0", 52, Pink, TextAlignmentOptions.Left, FontStyles.Bold);
            Place(live.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -20), new Vector2(380, 70));
            SetRef(hud, "liveCountText", live);
            var blank = AddText(shellBox, "Text Blank", "COLD x0", 52, Blue, TextAlignmentOptions.Left, FontStyles.Bold);
            Place(blank.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -90), new Vector2(380, 70));
            SetRef(hud, "blankCountText", blank);
            SetRef(hud, "flippedBadge", BuildBadge(shellBox, "Badge Flipped", "FLIPPED", new Color(1f, 0.8f, 0.2f), new Vector2(0, 1), new Vector2(110, -190)).gameObject);
            SetRef(hud, "boostBadge", BuildBadge(shellBox, "Badge Boost", "WHIP x2", new Color(1f, 0.2f, 0.2f), new Vector2(0, 1), new Vector2(320, -190)).gameObject);
            var known = AddText(shellBox, "Text Known", "", 32, white, TextAlignmentOptions.Left);
            Place(known.rectTransform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(30, 15), new Vector2(390, 60));
            SetRef(hud, "knownText", known);

            //Màn dọc: đưa lên góc trên trái (khoảng trống bên trái Mistress) để không che ô item
            var shellLandscape = RectOrientationLayout.Layout.From(shellBox);
            Place(shellBox, new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -270), new Vector2(430, 300));
            var shellPortrait = RectOrientationLayout.Layout.From(shellBox);
            shellBox.gameObject.AddComponent<RectOrientationLayout>().Setup(shellPortrait, shellLandscape);

            //Player stamina (bottom-left)
            var playerBox = UIObject("Player Stamina", rootRt);
            Place(playerBox, new Vector2(0, 0), new Vector2(0, 0), new Vector2(40, 30), new Vector2(620, 150));
            var playerName = AddText(playerBox, "Text Name", "YOU", 36, white, TextAlignmentOptions.TopLeft, FontStyles.Bold);
            SetRef(hud, "playerNameText", playerName);
            var playerRow = BuildHeartRow(playerBox, "Hearts", new Vector2(0, -50), TextAnchor.UpperLeft);
            SetRef(hud, "playerHeartsRow", playerRow);
            SetRef(hud, "playerCuffedBadge", BuildBadge(playerBox, "Badge Cuffed", "CUFFED", new Color(1, 0.55f, 0.8f), new Vector2(0, 0), new Vector2(100, -10)).gameObject);
            SetRef(hud, "playerLockedBadge", BuildBadge(playerBox, "Badge Locked", "NO SAFE WORDS", new Color(0.8f, 0.1f, 0.15f), new Vector2(0, 0), new Vector2(330, -10)).gameObject);

            //Hint
            var hint = AddText(rootRt, "Text Hint", "", 40, white, TextAlignmentOptions.Center, FontStyles.Italic);
            Place(hint.rectTransform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 520), new Vector2(1040, 120));
            var hintPortrait = RectOrientationLayout.Layout.From(hint.rectTransform);
            Place(hint.rectTransform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 50), new Vector2(1040, 120));
            var hintLandscape = RectOrientationLayout.Layout.From(hint.rectTransform);
            hint.gameObject.AddComponent<RectOrientationLayout>().Setup(hintPortrait, hintLandscape);
            SetRef(hud, "hintText", hint);

            //Item card
            var card = UIObject("Item Card", rootRt);
            Place(card, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 200), new Vector2(1020, 300));
            AddPanel(card, PanelColor);
            var itemName = AddText(card, "Text Name", "Item", 52, Pink, TextAlignmentOptions.TopLeft, FontStyles.Bold);
            Place(itemName.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(40, -25), new Vector2(940, 70));
            SetRef(hud, "itemNameText", itemName);
            var itemDesc = AddText(card, "Text Desc", "", 36, white, TextAlignmentOptions.TopLeft);
            Place(itemDesc.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(40, -100), new Vector2(660, 170));
            SetRef(hud, "itemDescText", itemDesc);
            var blocked = AddText(card, "Text Blocked", "", 32, new Color(1f, 0.45f, 0.45f), TextAlignmentOptions.BottomRight, FontStyles.Italic);
            Place(blocked.rectTransform, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-30, 25), new Vector2(300, 150));
            SetRef(hud, "itemBlockedText", blocked);
            var use = AddButton(card, "Button Use", GameTexts.Use, Pink, 54);
            Place((RectTransform)use.transform, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-30, 30), new Vector2(260, 130));
            SetRef(hud, "buttonUse", use);
            SetRef(hud, "itemCard", card.gameObject);

            //Aim panel
            var aim = UIObject("Aim Panel", rootRt);
            Place(aim, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 230), new Vector2(1040, 170));
            var layout = aim.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 20;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            SetRef(hud, "buttonZapHer", AddButton(aim, "Button Zap Her", GameTexts.ZapHer, Pink, 46));
            SetRef(hud, "buttonZapSelf", AddButton(aim, "Button Zap Self", GameTexts.ZapMyself, new Color(0.25f, 0.5f, 0.8f), 46));
            SetRef(hud, "buttonCancelAim", AddButton(aim, "Button Cancel", GameTexts.Cancel, GreyButton, 46));
            SetRef(hud, "aimPanel", aim.gameObject);

            //Steal panel
            var steal = UIObject("Steal Panel", rootRt);
            Place(steal, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 230), new Vector2(900, 170));
            AddPanel(steal, PanelColor);
            var stealText = AddText(steal, "Text Steal", GameTexts.StealHint.ToUpper(), 44, new Color(1f, 0.85f, 0.3f), TextAlignmentOptions.Top, FontStyles.Bold);
            Place(stealText.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -20), new Vector2(860, 60));
            var timerBg = UIObject("Timer", steal);
            Place(timerBg, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 30), new Vector2(780, 36));
            AddImage(timerBg, new Color(0, 0, 0, 0.6f), LoadSprite("ui_rounded.png"), false);
            var fill = UIObject("Fill", timerBg);
            Stretch(fill, 4, 4, 4, 4);
            var fillImage = AddImage(fill, new Color(1f, 0.85f, 0.3f), null, false);
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            SetRef(hud, "stealTimerFill", fillImage);
            SetRef(hud, "stealPanel", steal.gameObject);

            //Banner
            var banner = UIObject("Banner", rootRt);
            banner.anchorMin = new Vector2(0, 0.5f);
            banner.anchorMax = new Vector2(1, 0.5f);
            banner.pivot = new Vector2(0.5f, 0.5f);
            banner.anchoredPosition = new Vector2(0, 80);
            banner.sizeDelta = new Vector2(0, 380);
            AddImage(banner, new Color(0.03f, 0f, 0.02f, 0.65f), null, false);
            var bannerGroup = banner.gameObject.AddComponent<CanvasGroup>();
            bannerGroup.blocksRaycasts = false;
            bannerGroup.interactable = false;
            var bannerTitle = AddText(banner, "Text Title", "ROUND 1", 120, Pink, TextAlignmentOptions.Center, FontStyles.Bold);
            Place(bannerTitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 50), new Vector2(1060, 170));
            bannerTitle.enableAutoSizing = true;
            bannerTitle.fontSizeMin = 60;
            bannerTitle.fontSizeMax = 120;
            var bannerSub = AddText(banner, "Text Sub", "", 42, white, TextAlignmentOptions.Center);
            Place(bannerSub.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -95), new Vector2(1060, 130));
            SetRef(hud, "banner", bannerGroup);
            SetRef(hud, "bannerTitle", bannerTitle);
            SetRef(hud, "bannerSub", bannerSub);

            //Debug
            var debugButton = AddButton(rootRt, "Button Debug", "DBG", new Color(0.2f, 0.2f, 0.2f, 0.6f), 30);
            Place((RectTransform)debugButton.transform, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-30, 30), new Vector2(110, 60));
            SetRef(hud, "buttonDebug", debugButton);
            var debugPanel = UIObject("Debug Panel", rootRt);
            Place(debugPanel, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-30, 100), new Vector2(520, 200));
            AddImage(debugPanel, new Color(0, 0, 0, 0.7f), null, false);
            var debugText = AddText(debugPanel, "Text Debug", "", 30, new Color(0.6f, 1f, 0.6f), TextAlignmentOptions.TopLeft);
            debugText.margin = new Vector4(16, 12, 16, 12);
            SetRef(hud, "debugText", debugText);
            SetRef(hud, "debugPanel", debugPanel.gameObject);

            SetLayerRecursive(root, UILayer);
            return hud;
        }

        private static RectTransform BuildHeartRow(RectTransform parent, string name, Vector2 position, TextAnchor alignment = TextAnchor.UpperCenter)
        {
            var row = UIObject(name, parent);
            row.anchorMin = new Vector2(0, 1);
            row.anchorMax = new Vector2(1, 1);
            row.pivot = new Vector2(0.5f, 1);
            row.anchoredPosition = position;
            row.sizeDelta = new Vector2(0, 66);
            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = alignment;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            return row;
        }

        private static RectTransform BuildBadge(RectTransform parent, string name, string text, Color color, Vector2 anchor, Vector2 position)
        {
            var badge = UIObject(name, parent);
            Place(badge, anchor, new Vector2(0.5f, 0.5f), position, new Vector2(text.Length * 22 + 50, 46));
            AddImage(badge, new Color(color.r * 0.35f, color.g * 0.35f, color.b * 0.35f, 0.9f), LoadSprite("ui_rounded.png"), false);
            AddText(badge, "Text", text, 28, color, TextAlignmentOptions.Center, FontStyles.Bold);
            return badge;
        }

        #endregion
    }
}
