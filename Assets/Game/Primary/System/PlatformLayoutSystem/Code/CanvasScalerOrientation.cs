using UnityEngine;
using UnityEngine.UI;

namespace Game.Primary.PlatformLayoutSystem
{
    /// <summary>
    /// Đổi reference resolution của CanvasScaler theo hướng màn hình (dọc 1080x2400 / ngang 2400x1080).
    /// Gắn vào mọi canvas (HUD, popup).
    /// </summary>
    [RequireComponent(typeof(CanvasScaler))]
    [ExecuteAlways]
    public class CanvasScalerOrientation : MonoBehaviour
    {
        private CanvasScaler scaler;
        private bool? lastPortrait;

        private void OnEnable()
        {
            scaler = GetComponent<CanvasScaler>();
            Apply(true);
        }

        private void Update()
        {
            Apply(false);
        }

        private void Apply(bool force)
        {
            if (scaler == null)
                return;

            bool portrait = OrientationUtility.IsPortrait;
            if (force == false && lastPortrait == portrait)
                return;

            lastPortrait = portrait;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            scaler.referenceResolution = OrientationUtility.CurrentReference;
        }
    }
}
