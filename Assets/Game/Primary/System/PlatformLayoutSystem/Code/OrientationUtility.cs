using UnityEngine;

namespace Game.Primary.PlatformLayoutSystem
{
    /// <summary>
    /// Mobile (Android/iOS): màn dọc, reference 1080x2400.
    /// Desktop (Windows/macOS): màn ngang 1920x1080, UI dùng reference xoay ngang 2400x1080 để kích thước phần tử giữ như bản dọc.
    /// Quyết định theo tỉ lệ màn hình thực tế để Game View trong Editor cũng hiển thị đúng.
    /// </summary>
    public static class OrientationUtility
    {
        public static readonly Vector2 PortraitReference = new Vector2(1080, 2400);
        public static readonly Vector2 LandscapeReference = new Vector2(2400, 1080);

        public static bool IsPortrait => Screen.height >= Screen.width;

        public static Vector2 CurrentReference => IsPortrait ? PortraitReference : LandscapeReference;

        public static float Aspect => Screen.height > 0 ? (float)Screen.width / Screen.height : 1f;
    }
}
