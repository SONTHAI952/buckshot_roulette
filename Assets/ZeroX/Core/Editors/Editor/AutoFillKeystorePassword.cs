using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ZeroX.Editors
{
    /// <summary>
    /// Preprocess build Android trong Unity.
    /// Nếu password keystore / key alias chưa được điền thì tự điền mặc định.
    /// Đặt file này trong thư mục Assets/Editor/
    /// </summary>
    public class AutoFillKeystorePassword : IPreprocessBuildWithReport
    {
        private const string DefaultPassword = "123456789";

        // Chạy sớm nhất trong chuỗi preprocess
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            // Chỉ áp dụng khi build Android
            if (report.summary.platform != BuildTarget.Android)
                return;

            // Không dùng custom keystore thì không điền
            if (!PlayerSettings.Android.useCustomKeystore)
                return;

            // Chưa chỉ định keystore hoặc file keystore không tồn tại thì không điền
            // (keystoreName có thể là đường dẫn tuyệt đối hoặc tương đối so với thư mục project)
            string keystoreName = PlayerSettings.Android.keystoreName;
            if (string.IsNullOrEmpty(keystoreName) || !File.Exists(keystoreName))
                return;

            bool filled = false;

            if (string.IsNullOrEmpty(PlayerSettings.Android.keystorePass))
            {
                PlayerSettings.Android.keystorePass = DefaultPassword;
                filled = true;
            }

            if (string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass))
            {
                PlayerSettings.Android.keyaliasPass = DefaultPassword;
                filled = true;
            }

            if (filled)
            {
                Debug.Log("<color=green>Tôi đã điền password keystore hộ bạn</color>");
            }
            else
            {
                //Debug.Log("[Preprocess] Password keystore đã có sẵn, không cần điền.");
            }
        }
    }
}