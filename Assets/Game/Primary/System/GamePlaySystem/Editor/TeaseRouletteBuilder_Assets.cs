using System.Collections.Generic;
using System.IO;
using Game.Primary.GamePlaySystem.Rules;
using Game.Primary.TableSystem;
using Game.Primary.WandSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ZeroX.AudioSystem;

namespace Game.Primary.GamePlaySystem.Editor
{
    public static partial class TeaseRouletteBuilder
    {
        //Layout bàn (world), dùng chung cho texture và scene
        public const float TableWidth = 1.6f;
        public const float TableDepth = 1.3f;
        public const float TableTopY = 0.75f;
        public static readonly float[] SlotXs = { -0.66f, -0.47f, 0.47f, 0.66f };
        public static readonly float[] SlotZsPlayer = { -0.34f, -0.52f };
        public static readonly Vector2 SlotHalfSize = new Vector2(0.085f, 0.08f);

        public static readonly Color Pink = new Color(1f, 0.25f, 0.58f);
        public static readonly Color Blue = new Color(0.45f, 0.75f, 1f);


        /// <summary> Vị trí slot (x, z). ownerId Player ở phía -Z, Dealer đối xứng phía +Z </summary>
        public static Vector2 SlotPosition(int ownerId, int slot)
        {
            //Mỗi bên 2 cụm 2x2: slot 0-3 cụm trái, 4-7 cụm phải. Hàng 0 là hàng gần giữa bàn
            int group = slot / 4;
            int local = slot % 4;
            int col = group * 2 + local % 2;
            int row = local / 2;
            float z = SlotZsPlayer[row];
            if (ownerId == PlayerIds.Dealer)
                z = -z;
            return new Vector2(SlotXs[col], z);
        }



        #region Textures

        private static void BuildTextures()
        {
            WriteTexture("ui_heart.png", 256, 256, (u, v) =>
            {
                float x = (u - 0.5f) * 2.6f;
                float y = (v - 0.45f) * 2.6f;
                float a = x * x + y * y - 1;
                return a * a * a - x * x * y * y * y <= 0 ? Color.white : Color.clear;
            }, 3);

            WriteTexture("ui_rounded.png", 128, 128, (u, v) =>
            {
                float px = u * 128, py = v * 128;
                return RoundedAlpha(px, py, 128, 128, 36) > 0.5f ? Color.white : Color.clear;
            }, 3);

            WriteTexture("ui_circle.png", 128, 128, (u, v) =>
            {
                float dx = u - 0.5f, dy = v - 0.5f;
                return dx * dx + dy * dy <= 0.25f ? Color.white : Color.clear;
            }, 3);

            WriteTexture("fx_glow_ring.png", 128, 128, (u, v) =>
            {
                float r = Mathf.Sqrt((u - 0.5f) * (u - 0.5f) + (v - 0.5f) * (v - 0.5f)) * 2f;
                if (r > 1)
                    return Color.clear;
                float ring = Mathf.Exp(-Mathf.Pow((r - 0.82f) / 0.07f, 2));
                float fill = 0.28f * (1 - r);
                return new Color(1, 1, 1, Mathf.Clamp01(ring + fill));
            }, 1);

            BuildFeltTexture();

            AssetDatabase.Refresh();
            SetSpriteImport("ui_heart.png", Vector4.zero);
            SetSpriteImport("ui_rounded.png", new Vector4(40, 40, 40, 40));
            SetSpriteImport("ui_circle.png", Vector4.zero);

            var glow = (TextureImporter)AssetImporter.GetAtPath($"{TexDir}/fx_glow_ring.png");
            glow.alphaIsTransparency = true;
            glow.wrapMode = TextureWrapMode.Clamp;
            glow.SaveAndReimport();

            var felt = (TextureImporter)AssetImporter.GetAtPath($"{TexDir}/table_felt.png");
            felt.maxTextureSize = 2048;
            felt.anisoLevel = 4;
            felt.SaveAndReimport();
        }

        private static float RoundedAlpha(float px, float py, float w, float h, float r)
        {
            float qx = Mathf.Abs(px - w / 2) - (w / 2 - r);
            float qy = Mathf.Abs(py - h / 2) - (h / 2 - r);
            float d = new Vector2(Mathf.Max(qx, 0), Mathf.Max(qy, 0)).magnitude + Mathf.Min(Mathf.Max(qx, qy), 0) - r;
            return d <= 0 ? 1 : 0;
        }

        private delegate Color PixelFunc(float u, float v);

        private static void WriteTexture(string fileName, int w, int h, PixelFunc func, int supersample)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var pixels = new Color[w * h];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Color sum = Color.clear;
                    for (int sy = 0; sy < supersample; sy++)
                    {
                        for (int sx = 0; sx < supersample; sx++)
                        {
                            float u = (x + (sx + 0.5f) / supersample) / w;
                            float v = (y + (sy + 0.5f) / supersample) / h;
                            sum += func(u, v);
                        }
                    }

                    pixels[y * w + x] = sum / (supersample * supersample);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            File.WriteAllBytes($"{TexDir}/{fileName}", tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }

        private static void SetSpriteImport(string fileName, Vector4 border)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath($"{TexDir}/{fileName}");
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.spriteBorder = border;
            importer.SaveAndReimport();
        }

        private static float SdBox(Vector2 p, Vector2 b)
        {
            var q = new Vector2(Mathf.Abs(p.x) - b.x, Mathf.Abs(p.y) - b.y);
            return new Vector2(Mathf.Max(q.x, 0), Mathf.Max(q.y, 0)).magnitude + Mathf.Min(Mathf.Max(q.x, q.y), 0);
        }

        /// <summary> Mặt bàn nỉ đỏ đô với các vạch phấn: ô item, vòng tròn giữa, khay bày viên nạp </summary>
        private static void BuildFeltTexture()
        {
            const int w = 2048, h = 1664;
            var felt = new Color(0.2f, 0.05f, 0.085f);
            var chalk = new Color(1f, 0.82f, 0.9f);
            var tex = new Texture2D(w, h, TextureFormat.RGB24, true);
            var pixels = new Color[w * h];
            var rects = new List<(Vector2 c, Vector2 half)>();
            for (int owner = 0; owner < 2; owner++)
            {
                for (int s = 0; s < RuleConst.InventorySlots; s++)
                    rects.Add((SlotPosition(owner, s), SlotHalfSize));
            }

            rects.Add((new Vector2(0, 0.14f), new Vector2(0.28f, 0.045f)));             //Khay bày viên nạp
            rects.Add((Vector2.zero, new Vector2(TableWidth / 2 - 0.03f, TableDepth / 2 - 0.03f)));   //Viền

            var rng = new System.Random(3);
            float lineHalf = 0.0028f;
            for (int py = 0; py < h; py++)
            {
                for (int px = 0; px < w; px++)
                {
                    float u = (px + 0.5f) / w;
                    float v = (py + 0.5f) / h;
                    //uv -> world (xem Table.fbx: u = (0.8 - x)/1.6, v = (0.65 - z)/1.3)
                    var p = new Vector2(TableWidth / 2 - u * TableWidth, TableDepth / 2 - v * TableDepth);

                    float d = 9;
                    foreach (var r in rects)
                        d = Mathf.Min(d, Mathf.Abs(SdBox(p - r.c, r.half)));
                    d = Mathf.Min(d, Mathf.Abs(p.magnitude - 0.3f));
                    d = Mathf.Min(d, Mathf.Abs(p.magnitude - 0.31f));

                    float n = (float)rng.NextDouble();
                    float chalkA = Mathf.Clamp01(1 - (d - lineHalf) / 0.0018f) * (0.55f + 0.35f * n);
                    float edge = Mathf.Max(Mathf.Abs(p.x) / (TableWidth / 2), Mathf.Abs(p.y) / (TableDepth / 2));
                    float shade = (0.92f + 0.08f * n) * Mathf.Lerp(1.05f, 0.75f, edge * edge);
                    var baseColor = felt * shade;
                    baseColor.a = 1;
                    pixels[py * w + px] = Color.Lerp(baseColor, chalk, chalkA);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            File.WriteAllBytes($"{TexDir}/table_felt.png", tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }

        public static Sprite LoadSprite(string fileName) => AssetDatabase.LoadAssetAtPath<Sprite>($"{TexDir}/{fileName}");

        #endregion



        #region Models - Materials

        private static void ImportModels()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Model", new[] { ModelDir }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer == null)
                    continue;

                importer.importAnimation = false;
                importer.importCameras = false;
                importer.importLights = false;
                importer.addCollider = false;
                importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
                importer.SaveAndReimport();

                //Tách material ra ngoài để chỉnh được, material trùng tên dùng chung
                bool remapped = false;
                foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path))
                {
                    if (asset is Material material && AssetDatabase.IsSubAsset(material))
                    {
                        string dst = $"{ModelMatDir}/{material.name}.mat";
                        if (File.Exists(dst))
                        {
                            importer.AddRemap(new AssetImporter.SourceAssetIdentifier(material), AssetDatabase.LoadAssetAtPath<Material>(dst));
                            remapped = true;
                        }
                        else
                        {
                            string error = AssetDatabase.ExtractAsset(material, dst);
                            if (string.IsNullOrEmpty(error) == false)
                                Debug.LogError(error);
                        }
                    }
                }

                if (remapped)
                    importer.SaveAndReimport();
                else
                    AssetDatabase.WriteImportSettingsIfDirty(path);
            }

            AssetDatabase.Refresh();
        }

        private static Material ModelMat(string name) => AssetDatabase.LoadAssetAtPath<Material>($"{ModelMatDir}/{name}.mat");

        private static void SetEmission(Material material, Color color)
        {
            if (material == null)
                return;

            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color);
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            EditorUtility.SetDirty(material);
        }

        private static void SetTransparent(Material material, float alpha)
        {
            if (material == null)
                return;

            material.SetFloat("_Surface", 1);
            material.SetFloat("_Blend", 0);
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)RenderQueue.Transparent;
            var c = material.GetColor("_BaseColor");
            c.a = alpha;
            material.SetColor("_BaseColor", c);
            EditorUtility.SetDirty(material);
        }

        private static Material CreateOrLoadMaterial(string name, string shaderName)
        {
            string path = $"{MatDir}/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find(shaderName));
                AssetDatabase.CreateAsset(material, path);
            }
            else if (material.shader.name != shaderName)
            {
                material.shader = Shader.Find(shaderName);
            }

            return material;
        }

        private static void BuildMaterials()
        {
            SetEmission(ModelMat("Wand_Head"), Pink * 0.4f);
            SetEmission(ModelMat("Shell_Body"), Color.black);
            SetEmission(ModelMat("Heart_LED"), Pink * 2f);
            SetEmission(ModelMat("Neon_Pink"), Pink * 6f);
            SetEmission(ModelMat("Lamp_Bulb"), new Color(1f, 0.75f, 0.55f) * 6f);
            SetEmission(ModelMat("Candle_Flame"), new Color(1f, 0.55f, 0.1f) * 5f);
            SetEmission(ModelMat("Mistress_Eyes"), Pink * 8f);
            SetEmission(ModelMat("Mistress_Latex"), Color.black);
            SetEmission(ModelMat("Mistress_Skin"), Color.black);
            SetEmission(ModelMat("Mistress_Hair"), Color.black);
            SetEmission(ModelMat("Mistress_Glove"), Color.black);
            SetEmission(ModelMat("Monitor_Screen"), new Color(0.05f, 0.0f, 0.02f));
            SetTransparent(ModelMat("Ice"), 0.6f);
            SetTransparent(ModelMat("Pill_Bottle"), 0.8f);

            var felt = ModelMat("Table_Felt");
            if (felt != null)
            {
                felt.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>($"{TexDir}/table_felt.png"));
                felt.SetColor("_BaseColor", Color.white);
                felt.SetFloat("_Smoothness", 0.05f);
                EditorUtility.SetDirty(felt);
            }

            var highlight = CreateOrLoadMaterial("M_Highlight", "Universal Render Pipeline/Unlit");
            highlight.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>($"{TexDir}/fx_glow_ring.png"));
            highlight.SetColor("_BaseColor", new Color(1f, 0.3f, 0.6f, 0.8f));
            highlight.SetFloat("_Surface", 1);
            highlight.SetFloat("_Blend", 2); //Additive
            highlight.SetOverrideTag("RenderType", "Transparent");
            highlight.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            highlight.SetInt("_DstBlend", (int)BlendMode.One);
            highlight.SetInt("_ZWrite", 0);
            highlight.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            highlight.renderQueue = (int)RenderQueue.Transparent;
            EditorUtility.SetDirty(highlight);

            var wall = CreateOrLoadMaterial("M_Room_Wall", "Universal Render Pipeline/Lit");
            wall.SetColor("_BaseColor", new Color(0.07f, 0.025f, 0.04f));
            wall.SetFloat("_Smoothness", 0.15f);

            var floor = CreateOrLoadMaterial("M_Room_Floor", "Universal Render Pipeline/Lit");
            floor.SetColor("_BaseColor", new Color(0.035f, 0.02f, 0.025f));
            floor.SetFloat("_Smoothness", 0.45f);

            var particle = CreateOrLoadMaterial("M_Zap_Particle", "Universal Render Pipeline/Particles/Unlit");
            particle.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>($"{TexDir}/fx_glow_ring.png"));
            particle.SetFloat("_Surface", 1);
            particle.SetFloat("_Blend", 2);
            particle.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            particle.SetInt("_DstBlend", (int)BlendMode.One);
            particle.SetInt("_ZWrite", 0);
            particle.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            particle.renderQueue = (int)RenderQueue.Transparent;

            EditorUtility.SetDirty(wall);
            EditorUtility.SetDirty(floor);
            EditorUtility.SetDirty(particle);
        }

        public static Material LoadMaterial(string name) => AssetDatabase.LoadAssetAtPath<Material>($"{MatDir}/{name}.mat");

        #endregion



        #region Audio

        private static void BuildAudioAssets()
        {
            foreach (var file in Directory.GetFiles(AudioDir, "*.wav"))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(file.Replace('\\', '/'));
                string assetPath = $"{AudioAssetDir}/{name}.asset";
                var audioAsset = AssetDatabase.LoadAssetAtPath<AudioAsset>(assetPath);
                if (audioAsset == null)
                {
                    audioAsset = ScriptableObject.CreateInstance<AudioAsset>();
                    AssetDatabase.CreateAsset(audioAsset, assetPath);
                }

                bool music = name.StartsWith("music_");
                SetValue(audioAsset, "audioGroupId", music ? 0L : 1L);
                SetRef(audioAsset, "audioClip", clip);
                SetValue(audioAsset, "volumeFactor", music ? 0.8f : 1f);

                if (music)
                {
                    var importer = (AudioImporter)AssetImporter.GetAtPath(file.Replace('\\', '/'));
                    var settings = importer.defaultSampleSettings;
                    settings.loadType = AudioClipLoadType.Streaming;
                    importer.defaultSampleSettings = settings;
                    importer.SaveAndReimport();
                }
            }
        }

        public static AudioAsset LoadAudio(string name) => AssetDatabase.LoadAssetAtPath<AudioAsset>($"{AudioAssetDir}/{name}.asset");

        #endregion



        #region Prefabs

        public static readonly (ItemId id, string model, string name, string desc, Color color, float yaw)[] ItemSpecs =
        {
            (ItemId.Peek, "Item_LaceMask", "Lace Mask", "Peek through the lace: see if the charge in the Wand is HOT or COLD.", new Color(1f, 0.45f, 0.7f), 0),
            (ItemId.Eject, "Item_Champagne", "Champagne", "Pop the cork! The current charge slides out without firing.", new Color(1f, 0.85f, 0.4f), 0),
            (ItemId.Heal, "Item_IceCube", "Ice Cube", "Cool down. Restore 1 Stamina.", new Color(0.6f, 0.9f, 1f), 0),
            (ItemId.DoubleDamage, "Item_LeatherWhip", "Leather Whip", "Crack it. The next HOT zap hits twice as hard.", new Color(1f, 0.25f, 0.25f), 35),
            (ItemId.SkipTurn, "Item_FluffyHandcuffs", "Fluffy Handcuffs", "Cuff her. She skips her next turn.", new Color(1f, 0.55f, 0.8f), 0),
            (ItemId.Steal, "Item_CollarLeash", "Collar & Leash", "Leash her: take one of her items and use it right away.", new Color(0.95f, 0.3f, 0.9f), 0),
            (ItemId.RevealRandom, "Item_LoveDice", "Love Dice", "Roll for a hint: reveals one random upcoming charge.", new Color(1f, 0.7f, 0.85f), 0),
            (ItemId.GambleHeal, "Item_MysteryPill", "Mystery Pill", "Feeling lucky? 50%: +2 Stamina. 50%: -1 Stamina.", new Color(0.7f, 0.45f, 1f), 0),
            (ItemId.Invert, "Item_HotWaxCandle", "Hot Wax Candle", "Drip hot wax on the Wand: flips the current charge HOT <-> COLD.", new Color(1f, 0.55f, 0.2f), 0),
        };

        private static void BuildPrefabs()
        {
            BuildShellPrefab();
            BuildHeartPrefab();
            foreach (var spec in ItemSpecs)
                BuildItemPrefab(spec.id, spec.model, spec.yaw);
        }

        private static void BuildShellPrefab()
        {
            var root = new GameObject("Shell");
            var model = InstantiateModel("Shell", root.transform);
            model.name = "Model";
            var view = root.AddComponent<ShellView>();
            SetRef(view, "bodyRenderer", FindDeep(model.transform, "Shell_Body").GetComponent<Renderer>());
            SetRef(view, "markRenderer", FindDeep(model.transform, "Shell_Mark").GetComponent<Renderer>());
            PrefabUtility.SaveAsPrefabAsset(root, $"{WandPrefabDir}/Shell.prefab");
            Object.DestroyImmediate(root);
        }

        private static void BuildHeartPrefab()
        {
            var modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>($"{ModelDir}/Heart.fbx");
            var mesh = modelAsset.GetComponentInChildren<MeshFilter>().sharedMesh;
            var root = new GameObject("Heart LED");
            root.AddComponent<MeshFilter>().sharedMesh = mesh;
            root.AddComponent<MeshRenderer>().sharedMaterial = ModelMat("Heart_LED");
            PrefabUtility.SaveAsPrefabAsset(root, $"{StaminaPrefabDir}/Heart LED.prefab");
            Object.DestroyImmediate(root);
        }

        private static void BuildItemPrefab(ItemId id, string modelName, float yaw)
        {
            var root = new GameObject(modelName);
            var visual = new GameObject("Visual").transform;
            visual.SetParent(root.transform, false);
            var model = InstantiateModel(modelName, visual);
            model.transform.localRotation = Quaternion.Euler(0, yaw, 0);

            //Scale cho vừa ô (0.15m)
            var bounds = CalcBounds(model);
            float footprint = Mathf.Max(bounds.size.x, bounds.size.z);
            float scale = footprint > 0.15f ? 0.15f / footprint : 1f;
            model.transform.localScale = Vector3.one * scale;
            bounds = CalcBounds(model);
            model.transform.localPosition = new Vector3(-bounds.center.x, -bounds.min.y, -bounds.center.z);

            var highlight = GameObject.CreatePrimitive(PrimitiveType.Quad);
            highlight.name = "Highlight";
            Object.DestroyImmediate(highlight.GetComponent<Collider>());
            highlight.transform.SetParent(root.transform, false);
            highlight.transform.localPosition = new Vector3(0, 0.003f, 0);
            highlight.transform.localRotation = Quaternion.Euler(90, 0, 0);
            highlight.transform.localScale = Vector3.one * 0.2f;
            highlight.GetComponent<Renderer>().sharedMaterial = LoadMaterial("M_Highlight");
            highlight.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;

            var collider = root.AddComponent<BoxCollider>();
            collider.size = new Vector3(SlotHalfSize.x * 2, Mathf.Max(0.07f, bounds.size.y), SlotHalfSize.y * 2);
            collider.center = new Vector3(0, collider.size.y / 2, 0);

            var view = root.AddComponent<ItemView>();
            SetValue(view, "itemId", (int)id);
            SetRef(view, "visual", visual);
            SetRef(view, "highlight", highlight.GetComponent<Renderer>());

            PrefabUtility.SaveAsPrefabAsset(root, $"{TablePrefabDir}/{modelName}.prefab");
            Object.DestroyImmediate(root);
        }

        public static Bounds CalcBounds(GameObject go)
        {
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return new Bounds(go.transform.position, Vector3.zero);

            var b = renderers[0].bounds;
            foreach (var r in renderers)
                b.Encapsulate(r.bounds);
            return b;
        }

        #endregion



        #region ZeroX fix

        /// <summary>
        /// Prefab Toast mặc định của ZeroX có component Window chưa gán content/canvas/canvasGroup
        /// -> ToastSingle.Show bị UnassignedReferenceException. Gán lại các ref còn thiếu.
        /// </summary>
        [MenuItem("Tools/Tease Roulette/Fix ZeroX Toast Prefabs")]
        public static void FixToastPrefabs()
        {
            foreach (var name in new[] { "Toast Single", "Toast Multiple", "Toast Fly Up" })
            {
                string path = $"Assets/ZeroX/Core/UI/Toast/Implement/Resources/ZeroX/Toast/Default/{name}.prefab";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
                    continue;

                var root = PrefabUtility.LoadPrefabContents(path);
                bool changed = false;
                foreach (var window in root.GetComponentsInChildren<ZeroX.WindowSystem.Window>(true))
                {
                    var so = new SerializedObject(window);
                    var canvas = so.FindProperty("canvas");
                    var canvasGroup = so.FindProperty("canvasGroup");
                    var content = so.FindProperty("content");

                    if (canvas.objectReferenceValue == null)
                    {
                        canvas.objectReferenceValue = window.GetComponent<Canvas>();
                        changed = true;
                    }

                    if (canvasGroup.objectReferenceValue == null)
                    {
                        var group = window.GetComponent<CanvasGroup>();
                        if (group == null)
                            group = window.gameObject.AddComponent<CanvasGroup>();
                        canvasGroup.objectReferenceValue = group;
                        changed = true;
                    }

                    if (content.objectReferenceValue == null)
                    {
                        var contentTransform = FindDeep(window.transform, "Content");
                        if (contentTransform != null)
                        {
                            content.objectReferenceValue = contentTransform;
                            changed = true;
                        }
                    }

                    so.ApplyModifiedPropertiesWithoutUndo();
                }

                if (changed)
                {
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                    Debug.Log("[TeaseRoulette] Fixed Window refs in " + path);
                }

                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        #endregion



        #region Config - PostProcess

        private static void BuildConfig()
        {
            var config = AssetDatabase.LoadAssetAtPath<GameplayConfig>(ConfigPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<GameplayConfig>();
                AssetDatabase.CreateAsset(config, ConfigPath);
            }

            config.items.Clear();
            foreach (var spec in ItemSpecs)
            {
                config.items.Add(new ItemDefinition
                {
                    id = spec.id,
                    displayName = spec.name,
                    description = spec.desc,
                    color = spec.color,
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{TablePrefabDir}/{spec.model}.prefab")
                });
            }

            EditorUtility.SetDirty(config);
        }

        private static void BuildPostProcess()
        {
            string path = $"{PostProcessDir}/CampaignMode Volume.asset";
            if (AssetDatabase.LoadAssetAtPath<VolumeProfile>(path) != null)
                AssetDatabase.DeleteAsset(path);

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, path);

            var bloom = profile.Add<Bloom>(true);
            bloom.intensity.Override(1.1f);
            bloom.threshold.Override(0.95f);
            bloom.scatter.Override(0.7f);

            var vignette = profile.Add<Vignette>(true);
            vignette.intensity.Override(0.42f);
            vignette.smoothness.Override(0.5f);
            vignette.color.Override(new Color(0.08f, 0f, 0.03f));

            var colorAdjust = profile.Add<ColorAdjustments>(true);
            colorAdjust.contrast.Override(18f);
            colorAdjust.saturation.Override(-8f);
            colorAdjust.postExposure.Override(0.2f);

            var grain = profile.Add<FilmGrain>(true);
            grain.intensity.Override(0.3f);

            var chroma = profile.Add<ChromaticAberration>(true);
            chroma.intensity.Override(0.12f);

            foreach (var component in profile.components)
            {
                component.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(component, profile);
            }

            EditorUtility.SetDirty(profile);
        }

        public static VolumeProfile LoadVolumeProfile() => AssetDatabase.LoadAssetAtPath<VolumeProfile>($"{PostProcessDir}/CampaignMode Volume.asset");

        #endregion
    }
}
