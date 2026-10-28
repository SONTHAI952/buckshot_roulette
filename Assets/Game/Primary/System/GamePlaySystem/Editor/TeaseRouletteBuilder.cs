using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Primary.GamePlaySystem.Editor
{
    /// <summary>
    /// Dựng lại toàn bộ asset, prefab, popup và scene CampaignMode từ code (có thể chạy lại bất cứ lúc nào).
    /// Menu: Tools/Tease Roulette/...
    /// </summary>
    public static partial class TeaseRouletteBuilder
    {
        public const string Root = "Assets/Game/Primary";
        public const string ModelDir = Root + "/Media/Models";
        public const string TexDir = Root + "/Media/Textures";
        public const string MatDir = Root + "/Media/Materials";
        public const string ModelMatDir = MatDir + "/Models";
        public const string AudioDir = Root + "/Media/Audio";
        public const string AudioAssetDir = AudioDir + "/Assets";
        public const string PostProcessDir = Root + "/Media/PostProcess";
        public const string TablePrefabDir = Root + "/System/TableSystem/Prefab";
        public const string WandPrefabDir = Root + "/System/WandSystem/Prefab";
        public const string StaminaPrefabDir = Root + "/System/StaminaSystem/Prefab";
        public const string ConfigPath = Root + "/System/GamePlaySystem/Data/Gameplay Config.asset";
        public const string ScenePath = Root + "/Scene/CampaignMode.unity";
        public const string UIDir = Root + "/UI";

        public const int UILayer = 5;


        [MenuItem("Tools/Tease Roulette/Build All (Assets + Popups + Scene)")]
        public static void BuildAll()
        {
            BuildAssets();
            BuildPopups();
            BuildScene();
        }

        [MenuItem("Tools/Tease Roulette/1. Build Assets")]
        public static void BuildAssets()
        {
            EnsureFolders();
            BuildTextures();
            ImportModels();
            BuildMaterials();
            BuildAudioAssets();
            BuildPrefabs();
            BuildConfig();
            BuildPostProcess();
            FixToastPrefabs();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[TeaseRoulette] Assets built");
        }

        [MenuItem("Tools/Tease Roulette/2. Build Popups")]
        public static void BuildPopupsMenu()
        {
            BuildPopups();
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Tools/Tease Roulette/3. Build Scene")]
        public static void BuildSceneMenu()
        {
            BuildScene();
        }



        #region Helper

        private static void EnsureFolders()
        {
            foreach (var dir in new[] { TexDir, MatDir, ModelMatDir, AudioAssetDir, PostProcessDir, TablePrefabDir, WandPrefabDir, StaminaPrefabDir,
                         Path.GetDirectoryName(ConfigPath), Path.GetDirectoryName(ScenePath) })
            {
                EnsureFolder(dir);
            }
        }

        public static void EnsureFolder(string path)
        {
            path = path.Replace('\\', '/');
            if (AssetDatabase.IsValidFolder(path))
                return;

            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        public static void SetRef(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null)
            {
                Debug.LogError($"[TeaseRoulette] Field '{field}' not found on {target.GetType().Name}");
                return;
            }

            prop.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetRefs(Object target, string field, Object[] values)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null)
            {
                Debug.LogError($"[TeaseRoulette] Field '{field}' not found on {target.GetType().Name}");
                return;
            }

            prop.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
                prop.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetValue(Object target, string field, object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null)
            {
                Debug.LogError($"[TeaseRoulette] Field '{field}' not found on {target.GetType().Name}");
                return;
            }

            switch (value)
            {
                case bool b: prop.boolValue = b; break;
                case int i:
                    if (prop.propertyType == SerializedPropertyType.Enum)
                        prop.enumValueIndex = i;
                    else
                        prop.intValue = i;
                    break;
                case long l: prop.longValue = l; break;
                case float f: prop.floatValue = f; break;
                case string s: prop.stringValue = s; break;
                case Color c: prop.colorValue = c; break;
                case Vector3 v: prop.vector3Value = v; break;
                default: Debug.LogError("[TeaseRoulette] Unsupported value type " + value?.GetType()); break;
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static Transform FindDeep(Transform root, string name)
        {
            if (root.name == name)
                return root;

            foreach (Transform child in root)
            {
                var found = FindDeep(child, name);
                if (found != null)
                    return found;
            }

            return null;
        }

        public static void SetLayerRecursive(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
                SetLayerRecursive(child.gameObject, layer);
        }

        public static GameObject InstantiateModel(string modelName, Transform parent)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>($"{ModelDir}/{modelName}.fbx");
            if (asset == null)
            {
                Debug.LogError("[TeaseRoulette] Missing model " + modelName);
                return new GameObject(modelName);
            }

            var go = (GameObject)PrefabUtility.InstantiatePrefab(asset);
            if (parent != null)
                go.transform.SetParent(parent, false);
            return go;
        }

        #endregion
    }
}
