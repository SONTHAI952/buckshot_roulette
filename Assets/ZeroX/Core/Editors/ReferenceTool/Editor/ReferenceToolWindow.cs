using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZeroX.Editors.ReferenceTool
{
    public class ReferenceToolWindow : EditorWindow
    {
        //Const
        private const float ObjectFieldHeight = 40f;        //Field kéo thả làm cao lên cho dễ thả trúng

        private static readonly string[] TabNames = new string[] { "Find Reference", "Find Unused Objects" };

        //Các property nội bộ của prefab instance - không tính là reference thật
        private static readonly HashSet<string> IgnoredPropertyNames = new HashSet<string>()
        {
            "m_CorrespondingSourceObject",
            "m_PrefabInstance",
            "m_PrefabAsset",
        };

        //Thư mục đặc biệt của Unity - asset nằm trong đây được load bằng tên/đường dẫn lúc runtime
        //hoặc được Unity dùng ngầm, nên mặc định tính là used
        private static readonly HashSet<string> SpecialFolderNames = new HashSet<string>()
        {
            "Resources",
            "StreamingAssets",
            "Editor",
            "Editor Default Resources",
            "Gizmos",
            "Plugins",
            "Standard Assets",
            "WebGLTemplates",
        };

        //File code - việc dùng bằng code (new, AddComponent, gọi static...) không phát hiện được
        //qua AssetDatabase nên không dám kết luận unused
        private static readonly HashSet<string> CodeFileExtensions = new HashSet<string>()
        {
            ".cs",
            ".asmdef",
            ".asmref",
            ".dll",
        };

        //Bắt guid trong block "m_SourcePrefab: {fileID: ..., guid: xxx, type: 3}" của file YAML -
        //dấu hiệu scene có đặt instance của prefab đó. Chỉ dùng cho scene chưa mở, vì Unity
        //không có API đọc hierarchy của scene đóng; prefab thì duyệt transform bằng API được
        private static readonly Regex SourcePrefabRegex = new Regex(@"m_SourcePrefab:\s*\{fileID:\s*\d+,\s*guid:\s*([0-9a-fA-F]+)", RegexOptions.Compiled);

        private static readonly HashSet<string> EmptyPathSet = new HashSet<string>();

        //Bắt mọi guid trong file YAML của ProjectSettings - asset được settings tham chiếu
        //(splash, icon, always-included shader...) phải tính là used
        private static readonly Regex GuidRegex = new Regex(@"guid:\s*([0-9a-fA-F]{32})", RegexOptions.Compiled);


        //Temp
        private int selectedTabIndex = 0;

        //Temp - Tab Find Reference
        private Object targetObject;
        private List<Object> listFindReferenceResult = new List<Object>();
        private Vector2 findReferenceScrollPos;

        //Temp - Tab Find Unused Objects
        private DefaultAsset targetFolder;
        private bool isQuickDelete = false;
        private List<Object> listUnusedResult = new List<Object>();
        private Vector2 unusedScrollPos;


        #region Unity Method

        [MenuItem("Tools/ZeroX/Editor/Reference Tool")]
        private static void ShowWindow()
        {
            var window = GetWindow<ReferenceToolWindow>();
            window.titleContent = new GUIContent("ZeroX Reference Tool");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Space(5);
            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, TabNames);
            GUILayout.Space(10);

            if (selectedTabIndex == 0)
                DrawTabFindReference();
            else
                DrawTabFindUnusedObjects();
        }

        #endregion


        #region Tab Find Reference

        private void DrawTabFindReference()
        {
            targetObject = DrawBigObjectField("Target Object", targetObject, typeof(Object), allowSceneObjects: true);

            GUILayout.Space(5);

            GUI.enabled = targetObject != null;
            if (GUILayout.Button("Find References In Current Scene"))
                FindReferencesInCurrentScene();

            if (GUILayout.Button("Find References In Project"))
                FindReferencesInProject();
            GUI.enabled = true;

            GUILayout.Space(10);
            DrawResultList("Results", listFindReferenceResult, ref findReferenceScrollPos, pingAction: PingResult, showDeleteButton: false);
        }

        private void FindReferencesInCurrentScene()
        {
            listFindReferenceResult.Clear();
            SaveAllAndRefresh();

            //Instance của prefab đặt trong scene cũng tính là reference tới prefab asset,
            //dù instance đó không bị object/script nào khác tham chiếu
            if (EditorUtility.IsPersistent(targetObject) && targetObject is GameObject)
            {
                foreach (var go in CollectAllGameObjectsInLoadedScenes())
                {
                    if (IsInstanceOfTargetPrefab(go))
                        listFindReferenceResult.Add(go);
                }
            }

            //Object trong scene nằm ở root thì không có parent nào tham chiếu tới,
            //nơi chứa nó chính là scene - tính scene đó là reference để kết quả không bị rỗng
            if (targetObject is GameObject sceneGo && EditorUtility.IsPersistent(targetObject) == false)
            {
                if (sceneGo.transform.parent == null)
                {
                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneGo.scene.path);
                    if (sceneAsset != null)
                        listFindReferenceResult.Add(sceneAsset);
                }
            }

            //Một object được tính là reference tới target khi tham chiếu thẳng target,
            //hoặc tham chiếu component của target khi target là GameObject
            var hashSetMatch = BuildMatchSet();

            try
            {
                var listComponent = CollectAllComponentsInLoadedScenes();
                for (int i = 0; i < listComponent.Count; i++)
                {
                    var comp = listComponent[i];

                    if (EditorUtility.DisplayCancelableProgressBar("Find References In Current Scene", comp.name, (float)i / listComponent.Count))
                        break;

                    if (IsPartOfTarget(comp))
                        continue;

                    if (HasReferenceTo(comp, hashSetMatch))
                        listFindReferenceResult.Add(comp);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            ShowNotification(new GUIContent("Tìm thấy " + listFindReferenceResult.Count + " reference trong scene"));
        }

        private void FindReferencesInProject()
        {
            listFindReferenceResult.Clear();

            var targetPath = AssetDatabase.GetAssetPath(targetObject);
            if (string.IsNullOrEmpty(targetPath))
            {
                ShowNotification(new GUIContent("Object trong scene không thể được asset trong project reference.\nDùng Find References In Current Scene."));
                return;
            }

            SaveAllAndRefresh();

            try
            {
                //Bước 1: dựng map ngược từ dependency trực tiếp: path -> các asset đang reference tới path đó
                var dictReferencers = new Dictionary<string, List<string>>();
                var listPath = GetAllProjectAssetPaths();

                for (int i = 0; i < listPath.Count; i++)
                {
                    var path = listPath[i];

                    if (EditorUtility.DisplayCancelableProgressBar("Find References In Project", path, (float)i / listPath.Count))
                        return;

                    foreach (var dependency in AssetDatabase.GetDependencies(path, false))
                    {
                        if (dependency == path)
                            continue;

                        if (dictReferencers.TryGetValue(dependency, out var listReferencer) == false)
                        {
                            listReferencer = new List<string>();
                            dictReferencers.Add(dependency, listReferencer);
                        }

                        listReferencer.Add(path);
                    }
                }

                //Bước 2: reference trực tiếp - mọi kiểu reference (field script, instance lồng,
                //material trên renderer...) đều tính
                var hashSetResultPath = new HashSet<string>();
                if (dictReferencers.TryGetValue(targetPath, out var listDirectReferencer))
                {
                    foreach (var referencerPath in listDirectReferencer)
                    {
                        if (referencerPath == targetPath)
                            continue;

                        if (hashSetResultPath.Add(referencerPath) == false)
                            continue;

                        var asset = AssetDatabase.LoadMainAssetAtPath(referencerPath);
                        if (asset != null)
                            listFindReferenceResult.Add(asset);
                    }
                }

                //Bước 3: lan gián tiếp CHỈ theo quan hệ chứa instance:
                //- B chứa instance của A -> scene đặt instance B thì A thật sự có trong scene -> tính scene
                //- B chỉ ref A qua field script -> đặt B vào scene thì A chưa hề được sinh ra -> không tính scene
                var dictInstantiateCache = new Dictionary<string, HashSet<string>>();
                var hashSetCarrierPath = new HashSet<string>();
                var queue = new Queue<string>();

                //Seed: những referencer trực tiếp có đặt instance của target bên trong
                foreach (var referencerPath in hashSetResultPath)
                {
                    if (IsInstantiating(referencerPath, targetPath, dictInstantiateCache))
                    {
                        hashSetCarrierPath.Add(referencerPath);
                        queue.Enqueue(referencerPath);
                    }
                }

                while (queue.Count > 0)
                {
                    var carrierPath = queue.Dequeue();

                    //Chỉ prefab mới đặt instance vào scene/prefab khác được - scene là điểm cuối chuỗi
                    if (System.IO.Path.GetExtension(carrierPath).ToLowerInvariant() != ".prefab")
                        continue;

                    if (dictReferencers.TryGetValue(carrierPath, out var listReferencer) == false)
                        continue;

                    foreach (var referencerPath in listReferencer)
                    {
                        if (referencerPath == targetPath)
                            continue;

                        //Đã là carrier rồi thì không xét lại - tránh lặp vô hạn khi có vòng
                        if (hashSetCarrierPath.Contains(referencerPath))
                            continue;

                        if (IsInstantiating(referencerPath, carrierPath, dictInstantiateCache) == false)
                            continue;

                        hashSetCarrierPath.Add(referencerPath);
                        queue.Enqueue(referencerPath);

                        if (hashSetResultPath.Add(referencerPath))
                        {
                            var asset = AssetDatabase.LoadMainAssetAtPath(referencerPath);
                            if (asset != null)
                                listFindReferenceResult.Add(asset);
                        }
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            ShowNotification(new GUIContent("Tìm thấy " + listFindReferenceResult.Count + " reference trong project"));
        }

        #endregion


        #region Tab Find Unused Objects

        private void DrawTabFindUnusedObjects()
        {
            targetFolder = (DefaultAsset)DrawBigObjectField("Target Folder", targetFolder, typeof(DefaultAsset), allowSceneObjects: false);

            var folderPath = targetFolder != null ? AssetDatabase.GetAssetPath(targetFolder) : null;
            var isValidFolder = string.IsNullOrEmpty(folderPath) == false && AssetDatabase.IsValidFolder(folderPath);

            if (targetFolder != null && isValidFolder == false)
                EditorGUILayout.HelpBox("Object đang chọn không phải folder.", MessageType.Warning);

            GUILayout.Space(5);

            GUI.enabled = isValidFolder;
            if (GUILayout.Button("Find Unused Object"))
                FindUnusedObjects(folderPath);
            GUI.enabled = true;

            EditorGUILayout.HelpBox(
                "Mặc định tính là used (không liệt kê): asset trong thư mục đặc biệt (Resources, StreamingAssets, Plugins, Editor...), file code (.cs, .dll...), scene có trong Build Settings, asset được Project Settings tham chiếu (splash, icon, always-included shader...) và asset gắn AssetBundle.\n" +
                "Asset load bằng code ngoài các trường hợp trên (load theo đường dẫn tự ghép, Addressables...) vẫn có thể bị liệt kê nhầm là unused - kiểm tra lại trước khi xóa.",
                MessageType.Info);

            GUILayout.Space(5);

            //Bật thì nút Delete xóa thẳng không hỏi - asset vẫn vào thùng rác hệ điều hành nên khôi phục được
            isQuickDelete = EditorGUILayout.ToggleLeft("Quick Delete (xóa không hiện dialog xác nhận)", isQuickDelete);

            GUILayout.Space(10);
            DrawResultList("Unused Objects", listUnusedResult, ref unusedScrollPos, pingAction: PingDirect, showDeleteButton: true);
        }

        private void FindUnusedObjects(string folderPath)
        {
            listUnusedResult.Clear();
            SaveAllAndRefresh();

            try
            {
                //Bước 1: gom tất cả path đang được asset khác trong project reference tới
                var hashSetReferencedPath = new HashSet<string>();
                var listPath = GetAllProjectAssetPaths();

                for (int i = 0; i < listPath.Count; i++)
                {
                    var path = listPath[i];

                    if (EditorUtility.DisplayCancelableProgressBar("Find Unused Objects", path, (float)i / listPath.Count))
                        return;

                    foreach (var dependency in AssetDatabase.GetDependencies(path, false))
                    {
                        if (dependency != path)
                            hashSetReferencedPath.Add(dependency);
                    }
                }

                //Bước 1.5: asset được ProjectSettings tham chiếu cũng là used - các file settings
                //nằm ngoài AssetDatabase nên GetDependencies không nhìn thấy
                CollectPathsReferencedByProjectSettings(hashSetReferencedPath);

                //Bước 2: asset trong target folder không bị ai reference, không thuộc diện
                //mặc định used (thư mục đặc biệt, file code, scene trong Build Settings, AssetBundle) là unused
                var hashSetSceneInBuild = new HashSet<string>(EditorBuildSettings.scenes.Select(s => s.path));

                var listGuid = AssetDatabase.FindAssets("", new string[] { folderPath }).Distinct();
                foreach (var guid in listGuid)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);

                    if (AssetDatabase.IsValidFolder(path))
                        continue;

                    //Nằm trong Resources, StreamingAssets... - Unity load bằng tên/đường dẫn nên mặc định used
                    if (IsInSpecialFolder(path))
                        continue;

                    //File code: có thể đang được dùng bằng code, không kết luận được
                    if (IsCodeFile(path))
                        continue;

                    //Scene có trong Build Settings sẽ được load lúc runtime
                    if (hashSetSceneInBuild.Contains(path))
                        continue;

                    //Asset gắn vào AssetBundle sẽ được load bằng code lúc runtime
                    var importer = AssetImporter.GetAtPath(path);
                    if (importer != null && string.IsNullOrEmpty(importer.assetBundleName) == false)
                        continue;

                    if (hashSetReferencedPath.Contains(path))
                        continue;

                    var asset = AssetDatabase.LoadMainAssetAtPath(path);
                    if (asset != null)
                        listUnusedResult.Add(asset);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            ShowNotification(new GUIContent("Tìm thấy " + listUnusedResult.Count + " unused object"));
        }

        /// <summary>
        /// Gom path các asset được file ProjectSettings/*.asset tham chiếu (splash, icon,
        /// always-included shader, preloaded asset...) - các file này nằm ngoài AssetDatabase
        /// nên phải đọc YAML lấy guid
        /// </summary>
        private void CollectPathsReferencedByProjectSettings(HashSet<string> hashSetReferencedPath)
        {
            var projectRoot = System.IO.Path.GetDirectoryName(Application.dataPath);
            var settingsFolder = System.IO.Path.Combine(projectRoot, "ProjectSettings");
            if (System.IO.Directory.Exists(settingsFolder) == false)
                return;

            foreach (var filePath in System.IO.Directory.GetFiles(settingsFolder, "*.asset"))
            {
                try
                {
                    var text = System.IO.File.ReadAllText(filePath);
                    if (text.StartsWith("%YAML") == false)
                        continue;

                    foreach (Match match in GuidRegex.Matches(text))
                    {
                        var path = AssetDatabase.GUIDToAssetPath(match.Groups[1].Value);
                        if (string.IsNullOrEmpty(path) == false)
                            hashSetReferencedPath.Add(path);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// Xóa asset khỏi project - đưa vào thùng rác của hệ điều hành để lỡ tay còn khôi phục được
        /// </summary>
        private void DeleteUnusedAsset(Object asset)
        {
            if (asset == null)
                return;

            var path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(path))
                return;

            //Quick Delete bật thì xóa thẳng không hỏi
            if (isQuickDelete == false)
            {
                var isConfirmed = EditorUtility.DisplayDialog(
                    "Delete Asset",
                    "Xóa asset này khỏi project?\n\n" + path + "\n\nAsset sẽ được đưa vào thùng rác của hệ điều hành.",
                    "Delete",
                    "Cancel");
                if (isConfirmed == false)
                    return;
            }

            if (AssetDatabase.MoveAssetToTrash(path))
            {
                listUnusedResult.Remove(asset);
                Repaint();
            }
            else
            {
                Debug.LogError("Không xóa được asset: " + path);
                ShowNotification(new GUIContent("Không xóa được asset:\n" + path));
            }
        }

        #endregion


        #region Ping

        /// <summary>
        /// Ping tới tận nơi target đang được reference: kết quả là scene thì mở scene,
        /// là prefab thì mở prefab mode, rồi định vị đúng object bên trong
        /// </summary>
        private void PingResult(Object result)
        {
            if (result == null || targetObject == null)
                return;

            //Kết quả là scene: mở scene đó rồi định vị chỗ reference bên trong
            if (result is SceneAsset)
            {
                var scenePath = AssetDatabase.GetAssetPath(result);
                if (IsSceneLoaded(scenePath) == false)
                {
                    //Hỏi save scene hiện tại trước khi chuyển - user bấm Cancel thì dừng
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
                        return;

                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                }

                PingReferencersAmong(CollectAllGameObjectsInLoadedScenes(), result);
                return;
            }

            //Kết quả là prefab: mở prefab mode rồi định vị chỗ reference bên trong
            if (result is GameObject && EditorUtility.IsPersistent(result))
            {
                var prefabPath = AssetDatabase.GetAssetPath(result);

                //Model (fbx...) không mở prefab mode được - ping thẳng asset
                if (prefabPath.EndsWith(".prefab") == false)
                {
                    PingDirect(result);
                    return;
                }

                var stage = PrefabStageUtility.OpenPrefab(prefabPath);
                if (stage == null)
                {
                    PingDirect(result);
                    return;
                }

                var listGo = stage.prefabContentsRoot.GetComponentsInChildren<Transform>(true)
                    .Select(t => t.gameObject)
                    .ToList();
                PingReferencersAmong(listGo, result);
                return;
            }

            //Component trong scene: ping GameObject đang giữ nó
            if (result is Component comp)
            {
                Selection.activeGameObject = comp.gameObject;
                EditorGUIUtility.PingObject(comp.gameObject);
                return;
            }

            //Asset thường (material, scriptable object...): bản thân nó là nơi reference
            PingDirect(result);
        }

        /// <summary>
        /// Tìm các GameObject đang reference tới target trong danh sách rồi select + ping cái đầu tiên
        /// </summary>
        private void PingReferencersAmong(List<GameObject> listGo, Object fallback)
        {
            var listReferencer = FindReferencerGameObjects(listGo);

            //Target là object trong scene không bị ai reference (nằm ở root) - ping chính nó để thấy vị trí
            if (listReferencer.Count == 0 && targetObject is GameObject targetGo && listGo.Contains(targetGo))
                listReferencer.Add(targetGo);

            if (listReferencer.Count == 0)
            {
                PingDirect(fallback);
                ShowNotification(new GUIContent("Không định vị được chỗ reference cụ thể"));
                return;
            }

            Selection.objects = listReferencer.ToArray();
            EditorGUIUtility.PingObject(listReferencer[0]);
        }

        /// <summary>
        /// Lọc các GameObject đang reference tới target: có component trỏ tới target,
        /// hoặc bản thân là instance của target prefab
        /// </summary>
        private List<GameObject> FindReferencerGameObjects(List<GameObject> listGo)
        {
            var listReferencer = new List<GameObject>();
            var hashSetMatch = BuildMatchSet();
            var isTargetPrefabAsset = EditorUtility.IsPersistent(targetObject) && targetObject is GameObject;

            foreach (var go in listGo)
            {
                if (isTargetPrefabAsset && IsInstanceOfTargetPrefab(go))
                {
                    listReferencer.Add(go);
                    continue;
                }

                foreach (var comp in go.GetComponents<Component>())
                {
                    if (comp == null)
                        continue;

                    if (IsPartOfTarget(comp))
                        continue;

                    if (HasReferenceTo(comp, hashSetMatch))
                    {
                        listReferencer.Add(go);
                        break;
                    }
                }
            }

            return listReferencer;
        }

        private void PingDirect(Object obj)
        {
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        private static bool IsSceneLoaded(string scenePath)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene.path == scenePath)
                    return true;
            }

            return false;
        }

        #endregion


        #region Utility

        /// <summary>
        /// Tập object được tính là "chính target": target và các component của nó khi target là GameObject
        /// </summary>
        private HashSet<Object> BuildMatchSet()
        {
            var hashSetMatch = new HashSet<Object>() { targetObject };

            if (targetObject is GameObject targetGo)
            {
                foreach (var comp in targetGo.GetComponents<Component>())
                {
                    if (comp != null)
                        hashSetMatch.Add(comp);
                }
            }

            return hashSetMatch;
        }

        /// <summary>
        /// ObjectField cao hơn bình thường cho dễ kéo thả, label căn giữa theo chiều dọc
        /// </summary>
        private Object DrawBigObjectField(string label, Object value, System.Type objType, bool allowSceneObjects)
        {
            GUILayout.BeginHorizontal();

            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(label, labelStyle, GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.Height(ObjectFieldHeight));

            var result = EditorGUILayout.ObjectField(value, objType, allowSceneObjects, GUILayout.Height(ObjectFieldHeight));

            GUILayout.EndHorizontal();

            return result;
        }

        private void DrawResultList(string title, List<Object> listResult, ref Vector2 scrollPos, System.Action<Object> pingAction, bool showDeleteButton)
        {
            GUILayout.Label(title + " (" + listResult.Count + ")", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.helpBox);

            if (listResult.Count == 0)
            {
                GUILayout.Label("Không có kết quả", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                foreach (var item in listResult)
                {
                    GUILayout.BeginHorizontal();

                    //Disable để list ở dạng read only - click vẫn ping được object
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(item, typeof(Object), true);
                    GUI.enabled = true;

                    //Mở scene/prefab hay xóa asset ngay giữa lúc vẽ GUI dễ gây lỗi layout
                    //(và không được sửa list đang duyệt) - dời sang delayCall
                    if (pingAction != null && GUILayout.Button("Ping", GUILayout.Width(45)))
                        EditorApplication.delayCall += () => pingAction(item);

                    if (showDeleteButton)
                    {
                        var cachedBackgroundColor = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);

                        if (GUILayout.Button("Delete", GUILayout.Width(55)))
                            EditorApplication.delayCall += () => DeleteUnusedAsset(item);

                        GUI.backgroundColor = cachedBackgroundColor;
                    }

                    GUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Save scene + asset rồi refresh để data trên disk khớp với trạng thái đang mở,
        /// kết quả tìm mới chính xác
        /// </summary>
        private void SaveAllAndRefresh()
        {
            //EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Gom mọi GameObject (kể cả đang inactive) trong tất cả scene đang mở
        /// </summary>
        private List<GameObject> CollectAllGameObjectsInLoadedScenes()
        {
            var listGo = new List<GameObject>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded == false)
                    continue;

                foreach (var rootGo in scene.GetRootGameObjects())
                {
                    foreach (var transform in rootGo.GetComponentsInChildren<Transform>(true))
                        listGo.Add(transform.gameObject);
                }
            }

            return listGo;
        }

        /// <summary>
        /// Gom mọi component (kể cả trên object đang inactive) trong tất cả scene đang mở
        /// </summary>
        private List<Component> CollectAllComponentsInLoadedScenes()
        {
            var listComponent = new List<Component>();

            foreach (var go in CollectAllGameObjectsInLoadedScenes())
            {
                foreach (var comp in go.GetComponents<Component>())
                {
                    //Missing script trả về null - bỏ qua
                    if (comp == null)
                        continue;

                    listComponent.Add(comp);
                }
            }

            return listComponent;
        }

        /// <summary>
        /// Object trong scene có phải instance của target prefab không - đi ngược chuỗi
        /// corresponding source (instance -> prefab -> prefab lồng bên trong...) để bắt cả nested prefab
        /// </summary>
        private bool IsInstanceOfTargetPrefab(GameObject go)
        {
            var source = PrefabUtility.GetCorrespondingObjectFromSource(go);
            while (source != null)
            {
                if (source == targetObject)
                    return true;

                source = PrefabUtility.GetCorrespondingObjectFromSource(source);
            }

            return false;
        }

        private List<string> GetAllProjectAssetPaths()
        {
            return AssetDatabase.GetAllAssetPaths()
                .Where(path => path.StartsWith("Assets/"))
                .Where(path => AssetDatabase.IsValidFolder(path) == false)
                .ToList();
        }

        /// <summary>
        /// File tại containerPath (scene/prefab) có đặt instance của prefab tại prefabPath không.
        /// Không kết luận được thì coi như có - thà báo thừa còn hơn báo sót
        /// </summary>
        private bool IsInstantiating(string containerPath, string prefabPath, Dictionary<string, HashSet<string>> dictCache)
        {
            var hashSetPath = GetInstantiatedPrefabPaths(containerPath, dictCache);
            if (hashSetPath == null)
                return true;

            return hashSetPath.Contains(prefabPath);
        }

        /// <summary>
        /// Tập path các prefab đang được đặt instance bên trong file tại containerPath.
        /// - Prefab: duyệt cây transform của asset, mỗi GameObject lần ngược chuỗi corresponding
        ///   source để biết nó là instance của prefab nào (không cần đọc file)
        /// - Scene: Unity không có API đọc hierarchy của scene chưa mở, phải đọc block
        ///   m_SourcePrefab trong YAML; trả null khi file không phải YAML text (không kết luận được)
        /// </summary>
        private HashSet<string> GetInstantiatedPrefabPaths(string containerPath, Dictionary<string, HashSet<string>> dictCache)
        {
            if (dictCache.TryGetValue(containerPath, out var cached))
                return cached;

            HashSet<string> result;
            var extension = System.IO.Path.GetExtension(containerPath).ToLowerInvariant();

            if (extension == ".prefab")
            {
                result = new HashSet<string>();

                var rootGo = AssetDatabase.LoadAssetAtPath<GameObject>(containerPath);
                if (rootGo != null)
                {
                    foreach (var transform in rootGo.GetComponentsInChildren<Transform>(true))
                    {
                        var source = PrefabUtility.GetCorrespondingObjectFromSource(transform.gameObject);
                        while (source != null)
                        {
                            result.Add(AssetDatabase.GetAssetPath(source));
                            source = PrefabUtility.GetCorrespondingObjectFromSource(source);
                        }
                    }
                }
            }
            else if (extension == ".unity")
            {
                try
                {
                    var projectRoot = System.IO.Path.GetDirectoryName(Application.dataPath);
                    var text = System.IO.File.ReadAllText(System.IO.Path.Combine(projectRoot, containerPath));

                    if (text.StartsWith("%YAML") == false)
                    {
                        //Project serialize binary - không đọc được
                        result = null;
                    }
                    else
                    {
                        result = new HashSet<string>();
                        foreach (Match match in SourcePrefabRegex.Matches(text))
                        {
                            var prefabPath = AssetDatabase.GUIDToAssetPath(match.Groups[1].Value);
                            if (string.IsNullOrEmpty(prefabPath) == false)
                                result.Add(prefabPath);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                    result = null;
                }
            }
            else
            {
                //Các loại file khác không chứa được instance của prefab
                result = EmptyPathSet;
            }

            dictCache.Add(containerPath, result);
            return result;
        }

        /// <summary>
        /// Asset có nằm trong thư mục đặc biệt (Resources, Plugins...) ở bất kỳ cấp nào không.
        /// Check theo tên folder ở mọi cấp - rộng hơn luật thật của Unity một chút (vài folder
        /// chỉ đặc biệt khi ở root) nhưng thà nhận nhầm used còn hơn báo nhầm unused
        /// </summary>
        private static bool IsInSpecialFolder(string path)
        {
            var segments = path.Split('/');

            //Bỏ segment cuối vì là tên file
            for (int i = 0; i < segments.Length - 1; i++)
            {
                if (SpecialFolderNames.Contains(segments[i]))
                    return true;
            }

            return false;
        }

        private static bool IsCodeFile(string path)
        {
            var extension = System.IO.Path.GetExtension(path).ToLowerInvariant();
            return CodeFileExtensions.Contains(extension);
        }

        /// <summary>
        /// Component thuộc chính target (target là component đó, hoặc nằm trên GameObject target)
        /// thì không tính là "object khác"
        /// </summary>
        private bool IsPartOfTarget(Component comp)
        {
            if (comp == targetObject)
                return true;

            if (targetObject is GameObject targetGo && comp.gameObject == targetGo)
                return true;

            return false;
        }

        private bool HasReferenceTo(Object obj, HashSet<Object> hashSetMatch)
        {
            var so = new SerializedObject(obj);
            var prop = so.GetIterator();

            while (prop.Next(true))
            {
                if (prop.propertyType != SerializedPropertyType.ObjectReference)
                    continue;

                if (prop.objectReferenceValue == null)
                    continue;

                if (IgnoredPropertyNames.Contains(prop.name))
                    continue;

                if (hashSetMatch.Contains(prop.objectReferenceValue))
                    return true;
            }

            return false;
        }

        #endregion
    }
}
