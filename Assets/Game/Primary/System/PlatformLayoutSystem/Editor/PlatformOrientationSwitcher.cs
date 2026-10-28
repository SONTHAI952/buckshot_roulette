using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Game.Primary.PlatformLayoutSystem.Editor
{
    /// <summary>
    /// Khi switch platform: Android/iOS -> màn dọc (1080x2400), Windows/macOS -> màn ngang 1920x1080.
    /// Layout runtime tự đổi theo tỉ lệ màn hình (OrientationUtility), file này chỉ chỉnh PlayerSettings.
    /// </summary>
    public class PlatformOrientationSwitcher : IActiveBuildTargetChanged
    {
        public int callbackOrder => 0;

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            Apply(newTarget);
        }

        [MenuItem("Tools/Tease Roulette/Apply Orientation For Current Platform")]
        public static void ApplyCurrent()
        {
            Apply(EditorUserBuildSettings.activeBuildTarget);
        }

        public static void Apply(BuildTarget target)
        {
            if (IsMobile(target))
            {
                PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
                PlayerSettings.allowedAutorotateToPortrait = true;
                PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                PlayerSettings.allowedAutorotateToLandscapeLeft = false;
                PlayerSettings.allowedAutorotateToLandscapeRight = false;
                Debug.Log($"[Orientation] {target}: Portrait 1080x2400");
            }
            else
            {
                PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
                PlayerSettings.defaultScreenWidth = 1920;
                PlayerSettings.defaultScreenHeight = 1080;
                PlayerSettings.resizableWindow = true;
                Debug.Log($"[Orientation] {target}: Landscape 1920x1080");
            }
        }

        private static bool IsMobile(BuildTarget target)
        {
            return target == BuildTarget.Android || target == BuildTarget.iOS;
        }
    }
}
