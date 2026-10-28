using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ZeroX.Editors
{
    [InitializeOnLoad]
    public class QuickPlayScene
    {
        private const string SAVE_KEY = "ZeroX.QuickPlayScene.openSceneWhenEnterEditMode";
        private const float Button_Height = 21;

        private static readonly Texture2DBase64 iconPlay = new Texture2DBase64(128, 128, "iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAYAAADDPmHLAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAFlGlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNi4wLWMwMDIgNzkuMTY0NDg4LCAyMDIwLzA3LzEwLTIyOjA2OjUzICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjIuMCAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDI1LTA5LTA1VDE1OjUyOjU2KzA3OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAyNS0wOS0wNVQxNTo1NDo0MCswNzowMCIgeG1wOk1ldGFkYXRhRGF0ZT0iMjAyNS0wOS0wNVQxNTo1NDo0MCswNzowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDo0OWU5ZWJhNS0yYWFmLTcx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");
        private static readonly Texture2DBase64 iconPlayQuick = new Texture2DBase64(128, 128, "iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAYAAADDPmHLAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAF8WlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNi4wLWMwMDIgNzkuMTY0NDg4LCAyMDIwLzA3LzEwLTIyOjA2OjUzICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjIuMCAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDI1LTA5LTA1VDE0OjEyOjU2KzA3OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAyNS0wOS0wNVQxNDoxNDoxNyswNzowMCIgeG1wOk1ldGFkYXRhRGF0ZT0iMjAyNS0wOS0wNVQxNDoxNDoxNyswNzowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDoxNTQyMzViYi0yZTI4LTk4NGMtYTc5OS1kZWI2YzhmZGFjYjciIHhtcE1NOkRvY3VtZW50SUQ9ImFkb2JlOmRvY2lkOnBob3Rvc2hvcDo4YmZmOTI2Mi1iMmE3LWE4NGEtOWYwMy02MWY0OGJhMmVhN2EiIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDo4MzFmYzUxYi0yNGZkLWExNGMtYTU2OC01MTRhNmExMzk0NDUiPiA8eG1wTU06SGlzdG9yeT4gPHJkZjpTZXE+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJjcmVhdGVkIiBzdEV2dDppbnN0YW5jZUlEPSJ4bXAuaWlkOjgzMWZjNTFiLTI0ZmQtYTE0Yy1hNTY4LTUxNGE2YTEzOTQ0NSIgc3RFdnQ6d2hlbj0iMjAyNS0wOS0wNVQxNDoxMjo1NiswNzowMCIgc3RFdnQ6c29mdHdhcmVBZ2VudD0iQWRvYmUgUGhvdG9zaG9wIDIyLjAgKFdpbmRvd3MpIi8+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJzYXZlZCIgc3RFdnQ6aW5zdGFuY2VJRD0ieG1wLmlpZDoxNTQyMzViYi0yZTI4LTk4NGMtYTc5OS1kZWI2YzhmZGFjYjciIHN0RXZ0OndoZW49IjIwMjUtMDktMDVUMTQ6MTQ6MTcrMDc6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCAyMi4wIChXaW5kb3dzKSIgc3RFdnQ6Y2hhbmdlZD0iLyIvPiA8L3JkZjpTZXE+IDwveG1wTU06SGlzdG9yeT4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz7tBD5iAAAWt0lEQVR42u2deZRcVZ3HP/e+Wrq6s5EQFhNADEIISQQBiYIIM6CyzOgcVMbd0XGccRznOEcNMDIgjsjACMNRPO4bRiWAA5EskACRxAQCIUnDREkChBBCQpJeQnqp5d3f/FHdSefVW6veq67u1D2nTndXvXr93vv97vd+f+tVIoLb+NCQ31UPqBehawxkCpDtgVQLtGQ5uq+HS7VwkYIZSjhKCS1KKGDYo+A5BY+kSizc08ZL2SJM7gE7DZlW6OyHPguO6YSCBgyM6YFiGkwWWoqg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");
        
        
        static QuickPlayScene()
        {
            ToolbarGUILayout.LeftToolbarGUI_AlignRight.Add(OnGUI);
            
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnGUI()
        {
            if(EditorApplication.isPlaying)
                return;
            
            DrawButtonOpen();
            DrawButtonPlay();
            GUILayout.Space(12);
            DrawButtonQuickPlay();
        }
        

        private static void DrawButtonOpen()
        {
            bool clicked = GUILayout.Button("Open", GUILayout.Height(Button_Height));
            if(clicked == false)
                return;
            
            
            EditorBuildSettingsScene[] listScene = EditorBuildSettings.scenes;
            
            if (listScene.Length == 0)
            {
                Debug.LogError("No scene in build setting");
                return;
            }

            GenericMenu genericMenu = new GenericMenu();
            foreach (var scene in listScene)
            {
                string menuItem = Path.GetFileNameWithoutExtension(scene.path);
                genericMenu.AddItem(new GUIContent(menuItem), false, () => OpenScene(scene.path));
            }
            
            genericMenu.ShowAsContext();
        }
        
        private static void DrawButtonPlay()
        {
            bool clicked = GUILayout.Button(iconPlay, GUILayout.Width(35), GUILayout.Height(Button_Height));
            if(clicked == false)
                return;
            
            
            EditorBuildSettingsScene[] listScene = EditorBuildSettings.scenes;
            
            if (listScene.Length == 0)
            {
                Debug.LogError("No scene in build setting");
                return;
            }

            GenericMenu genericMenu = new GenericMenu();
            foreach (var scene in listScene)
            {
                string menuItem = Path.GetFileNameWithoutExtension(scene.path);
                genericMenu.AddItem(new GUIContent(menuItem), false, () => PlayScene(scene.path));
            }
            
            genericMenu.ShowAsContext();
        }

        private static void DrawButtonQuickPlay()
        {
            bool clicked = GUILayout.Button(iconPlayQuick, GUILayout.Width(35), GUILayout.Height(Button_Height));
            if(clicked == false)
                return;
            
            EditorBuildSettingsScene[] listScene = EditorBuildSettings.scenes;
            
            if (listScene.Length == 0)
            {
                Debug.LogError("No scene in build setting");
                return;
            }
            
            PlayScene(listScene[0].path);
        }

        

        private static void PlayScene(string path)
        {
            var activeScenePath = SceneManager.GetActiveScene().path;
           
            if (activeScenePath == path)
            {
                EditorPrefs.SetString(SAVE_KEY, "");
                EditorApplication.EnterPlaymode();
            }
            else
            {
                //Hỏi có muốn lưu lại scene hiện tại không
                if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
                    return;
                
                EditorSceneManager.OpenScene(path);
                
                //Lưu lại để khi tắt play thì mở lại scene đang làm việc
                EditorPrefs.SetString(SAVE_KEY, activeScenePath);

                EditorApplication.EnterPlaymode();
            }
        }

        private static void OpenScene(string path)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(path);
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if(stateChange != PlayModeStateChange.EnteredEditMode)
                return;
            
            string scenePathNeedOpen = EditorPrefs.GetString(SAVE_KEY);
            if(string.IsNullOrEmpty(scenePathNeedOpen))
                return;
            
            EditorPrefs.SetString(SAVE_KEY, "");
            EditorSceneManager.OpenScene(scenePathNeedOpen);
            
        }
    }
}