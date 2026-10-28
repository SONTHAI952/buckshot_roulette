using UnityEditor;
using UnityEngine;

namespace ZeroX.DashboardSystem
{
    [CreateAssetMenu(menuName = ZDBConst.SOMenuCreate.Core + "Haptic")]
    public class HapticDashboardDrawer : SystemDashboardDrawer
    {
        protected override void DrawBody()
        {
#if !ZEROX_SDK_NICE_VIBRATION
            EditorGUILayout.HelpBox("Khi import Feel, nếu chỉ dùng Nice Vibration thì chỉ tick folder Nice Vibration, bỏ tích các folder khác để tránh nặng project", MessageType.Info);
#endif
        }
    }
}