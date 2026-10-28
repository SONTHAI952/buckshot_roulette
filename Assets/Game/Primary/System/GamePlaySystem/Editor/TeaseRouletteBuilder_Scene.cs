using System.Collections.Generic;
using System.Linq;
using Game.Primary.CampaignModeSystem;
using Game.Primary.GamePlaySystem.Rules;
using Game.Primary.MistressSystem;
using Game.Primary.StaminaSystem;
using Game.Primary.TableSystem;
using Game.Primary.WandSystem;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Primary.GamePlaySystem.Editor
{
    public static partial class TeaseRouletteBuilder
    {
        private const string GuiCameraPrefab = "Assets/ZeroX/Core/UI/GUICamera/Implement/Resources/ZeroX/GUICamera/GUI Camera.prefab";
        private const string AudioManagerPrefab = "Assets/ZeroX/Core/AudioSystem/Implement/Audio Manager.prefab";

        public static readonly Vector3 CameraLandscapePos = new Vector3(0, 1.42f, -1.32f);
        public static readonly Vector3 CameraLandscapeLook = new Vector3(0, 0.92f, 0.2f);
        public static readonly Vector3 CameraPortraitPos = new Vector3(0, 2.05f, -1.38f);
        public static readonly Vector3 CameraPortraitLook = new Vector3(0, 0.78f, 0.02f);
        public static readonly Vector3 MistressPos = new Vector3(0, 0, 0.85f);
        public static readonly Vector3 MistressChest = new Vector3(0, 1.15f, 0.82f);
        public static readonly Vector3 MistressFace = new Vector3(0, 1.45f, 0.82f);


        public static void BuildScene()
        {
            EnsureFolders();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            //Thành phần cơ bản (Dev.md mục 13)
            PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(GuiCameraPrefab));
            PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(AudioManagerPrefab));
            BuildDirectionalLight();
            BuildEventSystem();
            BuildVolume();
            SetupRenderSettings();

            var campaign = new GameObject("CampaignMode");
            var campaignMode = campaign.AddComponent<CampaignMode>();

            var coreGo = new GameObject("GamePlayCore");
            coreGo.transform.SetParent(campaign.transform, false);
            var core = coreGo.AddComponent<GamePlayCore>();
            SetRef(campaignMode, "gamePlayCore", core);
            SetRef(core, "config", AssetDatabase.LoadAssetAtPath<GameplayConfig>(ConfigPath));

            var environment = Child(coreGo.transform, "Environment");
            BuildEnvironment(environment);

            var modules = Child(coreGo.transform, "Modules");
            var table = BuildTableModule(modules);
            var wand = BuildWandModule(modules);
            var shellDisplay = BuildShellDisplay(modules);
            var monitor = BuildStaminaMonitor(modules);
            var mistress = BuildMistress(modules);
            SetRef(table, "playerUseFocus", Child(table.transform, "Player Use Focus", new Vector3(0, 1.05f, -0.82f)));
            SetRef(table, "dealerUseFocus", Child(table.transform, "Dealer Use Focus", new Vector3(0, 1.36f, 0.6f)));

            var cameraController = BuildCamera(coreGo.transform);

            var services = Child(coreGo.transform, "Services");
            var turnFlow = Child(services, "TurnFlowService").gameObject.AddComponent<TurnFlowService>();
            var input = Child(services, "PlayerInputService").gameObject.AddComponent<PlayerInputService>();
            var sfx = Child(services, "SfxService").gameObject.AddComponent<SfxService>();
            var haptic = Child(services, "HapticService").gameObject.AddComponent<HapticService>();
            BuildSfx(sfx);
            BuildMusic(Child(services, "MusicService"));

            var uiRoot = Child(coreGo.transform, "UI");
            var hud = BuildHud(uiRoot);

            SetRef(core, "table", table);
            SetRef(core, "wand", wand);
            SetRef(core, "shellDisplay", shellDisplay);
            SetRef(core, "staminaMonitor", monitor);
            SetRef(core, "mistress", mistress);
            SetRef(core, "turnFlow", turnFlow);
            SetRef(core, "playerInput", input);
            SetRef(core, "sfx", sfx);
            SetRef(core, "haptic", haptic);
            SetRef(core, "cameraController", cameraController);
            SetRef(core, "hud", hud);

            Child(campaign.transform, "Tutorials");

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuild();
            Debug.Log("[TeaseRoulette] Scene built: " + ScenePath);
        }



        #region Basic

        private static Transform Child(Transform parent, string name, Vector3? position = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            if (position.HasValue)
                go.transform.position = position.Value;
            return go.transform;
        }

        private static Transform Pose(Transform parent, string name, Vector3 position, Vector3 lookAt)
        {
            var t = Child(parent, name, position);
            t.rotation = Quaternion.LookRotation(lookAt - position, Vector3.up);
            return t;
        }

        private static void BuildDirectionalLight()
        {
            var go = new GameObject("Directional Light");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 0.12f;
            light.color = new Color(0.6f, 0.6f, 0.85f);
            light.shadows = LightShadows.None;
            go.transform.rotation = Quaternion.Euler(50, -30, 0);
        }

        private static void BuildEventSystem()
        {
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            var module = go.AddComponent<InputSystemUIInputModule>();
            module.AssignDefaultActions();
        }

        private static void BuildVolume()
        {
            var go = new GameObject("Global Volume");
            var volume = go.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.sharedProfile = LoadVolumeProfile();
        }

        private static void SetupRenderSettings()
        {
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.06f, 0.035f, 0.05f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.1f;
            RenderSettings.fogColor = new Color(0.02f, 0.005f, 0.012f);
        }

        private static void AddSceneToBuild()
        {
            var scenes = EditorBuildSettings.scenes.Where(s => s.path != ScenePath).ToList();
            scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        #endregion



        #region Environment

        private static void BuildEnvironment(Transform parent)
        {
            var room = Child(parent, "Room");
            var wall = LoadMaterial("M_Room_Wall");
            var floor = LoadMaterial("M_Room_Floor");
            Box(room, "Floor", new Vector3(0, -0.05f, 0), new Vector3(10, 0.1f, 10), floor);
            Box(room, "Ceiling", new Vector3(0, 3.4f, 0), new Vector3(10, 0.1f, 10), wall);
            Box(room, "Wall Back", new Vector3(0, 1.7f, 3.3f), new Vector3(10, 3.6f, 0.1f), wall);
            Box(room, "Wall Front", new Vector3(0, 1.7f, -3.5f), new Vector3(10, 3.6f, 0.1f), wall);
            Box(room, "Wall Left", new Vector3(-3.5f, 1.7f, 0), new Vector3(0.1f, 3.6f, 10), wall);
            Box(room, "Wall Right", new Vector3(3.5f, 1.7f, 0), new Vector3(0.1f, 3.6f, 10), wall);

            InstantiateModel("Table", parent).name = "Table";

            var lamp = InstantiateModel("Lamp", parent);
            lamp.name = "Lamp";
            lamp.transform.position = new Vector3(0, 2.45f, 0.05f);
            var spot = Child(parent, "Lamp Light", new Vector3(0, 2.32f, 0.05f));
            spot.rotation = Quaternion.Euler(90, 0, 0);
            var spotLight = spot.gameObject.AddComponent<Light>();
            spotLight.type = LightType.Spot;
            spotLight.spotAngle = 100;
            spotLight.innerSpotAngle = 40;
            spotLight.range = 5;
            spotLight.intensity = 10;
            spotLight.color = new Color(1f, 0.8f, 0.72f);
            spotLight.shadows = LightShadows.Soft;
            spotLight.shadowStrength = 0.85f;

            var neon = InstantiateModel("NeonHeart", parent);
            neon.name = "Neon Heart";
            neon.transform.position = new Vector3(0, 1.45f, 3.2f);
            PointLight(parent, "Neon Light", new Vector3(0, 1.75f, 2.9f), Pink, 2.5f, 4f);
            PointLight(parent, "Rim Light", new Vector3(0.6f, 1.7f, 1.7f), new Color(1f, 0.3f, 0.55f), 1.5f, 2.5f);
        }

        private static void Box(Transform parent, string name, Vector3 position, Vector3 size, Material material)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.transform.localScale = size;
            go.GetComponent<Renderer>().sharedMaterial = material;
            go.isStatic = true;
        }

        private static Light PointLight(Transform parent, string name, Vector3 position, Color color, float intensity, float range)
        {
            var light = Child(parent, name, position).gameObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
            return light;
        }

        #endregion



        #region Modules

        private static TableView BuildTableModule(Transform modules)
        {
            var go = Child(modules, "Table");
            var table = go.gameObject.AddComponent<TableView>();

            Transform[] MakeSlots(string groupName, int ownerId)
            {
                var group = Child(go, groupName);
                var slots = new Transform[RuleConst.InventorySlots];
                for (int i = 0; i < slots.Length; i++)
                {
                    var p = SlotPosition(ownerId, i);
                    slots[i] = Child(group, "Slot " + i, new Vector3(p.x, TableTopY + 0.001f, p.y));
                    slots[i].rotation = Quaternion.Euler(0, ownerId == PlayerIds.Player ? 0 : 180, 0);
                }

                return slots;
            }

            SetRefs(table, "playerSlots", MakeSlots("Player Slots", PlayerIds.Player));
            SetRefs(table, "dealerSlots", MakeSlots("Dealer Slots", PlayerIds.Dealer));
            SetRef(table, "playerItemSpawn", Child(go, "Player Item Spawn", new Vector3(0, 0.95f, -0.62f)));
            SetRef(table, "dealerItemSpawn", Child(go, "Dealer Item Spawn", new Vector3(0, 0.95f, 0.62f)));
            SetRef(table, "itemRoot", Child(go, "Items"));
            return table;
        }

        private static WandView BuildWandModule(Transform modules)
        {
            var anchors = Child(modules, "Wand Anchors");
            var restPos = new Vector3(-0.04f, TableTopY + 0.034f, -0.1f);
            var rest = Child(anchors, "Rest", restPos);
            rest.rotation = Quaternion.LookRotation(new Vector3(1, 0, 0.35f), Vector3.up);
            var cam = CameraLandscapePos;
            var aimDealer = Pose(anchors, "Player Aim Dealer", new Vector3(0.14f, 1.12f, -0.72f), MistressChest);
            var aimSelf = Pose(anchors, "Player Aim Self", new Vector3(0.06f, 1.2f, -0.6f), cam + Vector3.down * 0.05f);
            var dealerAimPlayer = Pose(anchors, "Dealer Aim Player", new Vector3(-0.1f, 1.12f, 0.42f), cam + Vector3.down * 0.05f);
            var dealerAimSelf = Pose(anchors, "Dealer Aim Self", new Vector3(0.16f, 1.25f, 0.5f), MistressFace);

            var go = Child(modules, "Wand", restPos);
            go.rotation = rest.rotation;
            var view = go.gameObject.AddComponent<WandView>();
            var collider = go.gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.09f, 0.09f, 0.34f);

            var modelRoot = Child(go, "Model");
            modelRoot.localRotation = Quaternion.Euler(0, 180, 0);
            var model = InstantiateModel("Wand", modelRoot);

            var headLight = Child(go, "Head Light").gameObject.AddComponent<Light>();
            headLight.transform.localPosition = new Vector3(0, 0, 0.15f);
            headLight.type = LightType.Point;
            headLight.range = 1.4f;
            headLight.intensity = 0;
            headLight.color = Pink;

            var eject = Child(go, "Eject Point");
            eject.localPosition = new Vector3(0, 0.025f, -0.12f);

            var boost = Child(go, "Boost Fx");
            boost.localPosition = new Vector3(0, 0, 0.12f);
            var boostLight = boost.gameObject.AddComponent<Light>();
            boostLight.type = LightType.Point;
            boostLight.color = new Color(1f, 0.1f, 0.1f);
            boostLight.range = 0.6f;
            boostLight.intensity = 2.5f;
            boost.gameObject.SetActive(false);

            var highlight = GameObject.CreatePrimitive(PrimitiveType.Quad);
            highlight.name = "Highlight";
            Object.DestroyImmediate(highlight.GetComponent<Collider>());
            highlight.transform.SetParent(go, false);
            highlight.transform.localPosition = new Vector3(0, -0.03f, 0);
            highlight.transform.localRotation = Quaternion.Euler(90, 0, 0);
            highlight.transform.localScale = new Vector3(0.16f, 0.42f, 1);
            highlight.GetComponent<Renderer>().sharedMaterial = LoadMaterial("M_Highlight");
            highlight.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;

            var particles = BuildZapParticles(go);

            SetRef(view, "restAnchor", rest);
            SetRef(view, "playerAimDealerAnchor", aimDealer);
            SetRef(view, "playerAimSelfAnchor", aimSelf);
            SetRef(view, "dealerAimPlayerAnchor", dealerAimPlayer);
            SetRef(view, "dealerAimSelfAnchor", dealerAimSelf);
            SetRef(view, "headRenderer", FindDeep(model.transform, "Wand_Head").GetComponent<Renderer>());
            SetRef(view, "headLight", headLight);
            SetRef(view, "ejectPoint", eject);
            SetRef(view, "boostFx", boost.gameObject);
            SetRef(view, "highlight", highlight.GetComponent<Renderer>());
            SetRef(view, "zapParticles", particles);
            SetRef(view, "shellPrefab", AssetDatabase.LoadAssetAtPath<ShellView>($"{WandPrefabDir}/Shell.prefab"));
            return view;
        }

        private static ParticleSystem BuildZapParticles(Transform wand)
        {
            var go = Child(wand, "Zap Particles");
            go.localPosition = new Vector3(0, 0, 0.17f);
            var ps = go.gameObject.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.loop = false;
            main.playOnAwake = false;
            main.duration = 0.5f;
            main.startLifetime = 0.45f;
            main.startSpeed = 1.6f;
            main.startSize = 0.05f;
            main.startColor = Pink;
            main.maxParticles = 80;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 50) });
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.03f;
            var size = ps.sizeOverLifetime;
            size.enabled = true;
            size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 1, 1, 0));
            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = LoadMaterial("M_Zap_Particle");
            return ps;
        }

        private static ShellDisplayView BuildShellDisplay(Transform modules)
        {
            var go = Child(modules, "Shell Display");
            var view = go.gameObject.AddComponent<ShellDisplayView>();
            SetRef(view, "shellPrefab", AssetDatabase.LoadAssetAtPath<ShellView>($"{WandPrefabDir}/Shell.prefab"));
            SetRef(view, "rowCenter", Child(go, "Row Center", new Vector3(0, TableTopY + 0.001f, 0.14f)));
            return view;
        }

        private static StaminaMonitorView BuildStaminaMonitor(Transform modules)
        {
            var position = new Vector3(0.6f, TableTopY + 0.001f, 0.02f);
            var go = Child(modules, "Stamina Monitor", position);
            var toCamera = CameraLandscapePos - position;
            toCamera.y = 0;
            go.rotation = Quaternion.LookRotation(toCamera, Vector3.up);
            var view = go.gameObject.AddComponent<StaminaMonitorView>();
            InstantiateModel("StaminaMonitor", go).name = "Model";

            //Screen: xoay 180 để trục +X là bên phải người xem, chữ đọc được
            var screen = Child(go, "Screen");
            screen.localPosition = new Vector3(0, 0.095f, 0.074f);
            screen.localRotation = Quaternion.Euler(0, 180, 0);

            var playerRow = Child(screen, "Player Row");
            playerRow.localPosition = new Vector3(0.04f, -0.027f, 0);
            var dealerRow = Child(screen, "Dealer Row");
            dealerRow.localPosition = new Vector3(0.04f, 0.027f, 0);
            WorldText(screen, "Label Player", "YOU", new Vector3(-0.1f, -0.027f, 0), new Vector2(0.07f, 0.03f), Color.white);
            WorldText(screen, "Label Dealer", "HER", new Vector3(-0.1f, 0.027f, 0), new Vector2(0.07f, 0.03f), Pink);
            var status = WorldText(screen, "Status", "", new Vector3(0, 0.13f, 0), new Vector2(0.36f, 0.05f), new Color(1f, 0.2f, 0.3f));

            SetRef(view, "playerRow", playerRow);
            SetRef(view, "dealerRow", dealerRow);
            SetRef(view, "statusText", status);
            SetRef(view, "heartPrefab", AssetDatabase.LoadAssetAtPath<GameObject>($"{StaminaPrefabDir}/Heart LED.prefab").GetComponent<Renderer>());
            SetValue(view, "heartSpacing", 0.04f);
            return view;
        }

        private static TextMeshPro WorldText(Transform parent, string name, string text, Vector3 localPos, Vector2 size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshPro>();
            tmp.text = text;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 0.05f;
            tmp.fontSizeMax = 2f;
            tmp.textWrappingMode = TextWrappingModes.NoWrap;
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = size;
            rt.localPosition = localPos;
            return tmp;
        }

        private static MistressView BuildMistress(Transform modules)
        {
            var go = Child(modules, "Mistress", MistressPos);
            go.rotation = Quaternion.Euler(0, 180, 0);
            var view = go.gameObject.AddComponent<MistressView>();

            //Root đã xoay 180 nên +Z local (hướng nhìn của model fbx) quay về phía người chơi
            var modelRoot = Child(go, "Model Root");
            var placeholder = InstantiateModel("Mistress_Placeholder", modelRoot);
            placeholder.name = "Placeholder (replace me)";

            var cuffs = InstantiateModel("Item_FluffyHandcuffs", go);
            cuffs.name = "Cuffs";
            cuffs.transform.localPosition = new Vector3(0, 0.79f, 0.44f);
            cuffs.transform.localScale = Vector3.one * 1.15f;
            cuffs.SetActive(false);

            var speech = WorldText(go, "Speech", "", new Vector3(0, 1.75f, 0), new Vector2(1.1f, 0.16f), new Color(1f, 0.6f, 0.82f));
            speech.transform.localRotation = Quaternion.Euler(0, 180, 0);
            speech.fontStyle = FontStyles.Italic | FontStyles.Bold;
            speech.fontSizeMax = 0.8f;

            var collider = go.gameObject.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0, 1.15f, 0);
            collider.height = 0.9f;
            collider.radius = 0.25f;

            var flash = placeholder.GetComponentsInChildren<Renderer>().Where(r => r.name != "Mistress_Eyes").Cast<Object>().ToArray();
            SetRef(view, "modelRoot", modelRoot);
            SetRef(view, "placeholder", placeholder);
            SetRef(view, "cuffs", cuffs);
            SetRef(view, "speechText", speech);
            SetRefs(view, "flashRenderers", flash);
            return view;
        }

        #endregion



        #region Camera - Audio

        private static CameraController BuildCamera(Transform core)
        {
            var go = Child(core, "Camera Controller");
            var controller = go.gameObject.AddComponent<CameraController>();
            var rig = Child(go, "Rig");
            var landscape = Pose(go, "Landscape Pose", CameraLandscapePos, CameraLandscapeLook);
            var portrait = Pose(go, "Portrait Pose", CameraPortraitPos, CameraPortraitLook);
            var fit = Child(go, "Fit Target", new Vector3(0, TableTopY, 0));
            rig.SetPositionAndRotation(landscape.position, landscape.rotation);

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.SetParent(rig, false);
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.01f, 0.003f, 0.007f);
            cam.nearClipPlane = 0.02f;
            cam.farClipPlane = 30f;
            cam.fieldOfView = 50;
            cam.depth = 0;
            cam.cullingMask = ~(1 << UILayer);
            camGo.AddComponent<AudioListener>();
            var raycaster = camGo.AddComponent<PhysicsRaycaster>();
            raycaster.eventMask = ~(1 << UILayer);
            var data = cam.GetUniversalAdditionalCameraData();
            data.renderPostProcessing = true;
            data.antialiasing = AntialiasingMode.FastApproximateAntialiasing;

            SetRef(controller, "mainCamera", cam);
            SetRef(controller, "rig", rig);
            SetRef(controller, "landscapePose", landscape);
            SetRef(controller, "portraitPose", portrait);
            SetRef(controller, "fitTarget", fit);
            SetValue(controller, "fitWidth", 1.75f);
            return controller;
        }

        private static void BuildSfx(SfxService sfx)
        {
            var map = new Dictionary<string, string>
            {
                { "zapHot", "sfx_zap_hot" }, { "clickCold", "sfx_click_cold" }, { "load", "sfx_load" }, { "eject", "sfx_eject" },
                { "tension", "sfx_tension" }, { "deal", "sfx_deal" }, { "peek", "sfx_peek" }, { "heal", "sfx_heal" },
                { "boost", "sfx_boost" }, { "cuffs", "sfx_cuffs" }, { "leash", "sfx_leash" }, { "dice", "sfx_dice" },
                { "pill", "sfx_pill" }, { "invert", "sfx_invert" }, { "playerHurt", "sfx_player_hurt" },
                { "mistressHurt", "sfx_mistress_hurt" }, { "stageStart", "sfx_stage_start" }, { "roundWon", "sfx_round_won" },
                { "roundLost", "sfx_round_lost" }
            };

            foreach (var pair in map)
                SetRef(sfx, pair.Key, LoadAudio(pair.Value));
        }

        private static void BuildMusic(Transform go)
        {
            var source = go.gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 0;
            var music = go.gameObject.AddComponent<MusicService>();
            SetRef(music, "audioSource", source);
            SetRef(music, "ambient", LoadAudio("music_ambient"));
        }

        #endregion
    }
}
