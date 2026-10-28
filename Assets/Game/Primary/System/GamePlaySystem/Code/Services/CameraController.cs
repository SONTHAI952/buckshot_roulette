using DG.Tweening;
using Game.Primary.PlatformLayoutSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZeroX.UI;

namespace Game.Primary.GamePlaySystem
{
    /// <summary>
    /// Chứa MainCamera. Chọn pose theo hướng màn hình và tự tăng FOV để luôn nhìn hết chiều ngang bàn.
    /// Cấu trúc: CameraController -> Rig (đặt theo pose) -> Main Camera (rung)
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform rig;
        [SerializeField] private Transform landscapePose;
        [SerializeField] private Transform portraitPose;
        [SerializeField] private float landscapeFov = 50f;
        [SerializeField] private float portraitFov = 68f;
        [SerializeField] private float maxFov = 95f;

        [Header("Fit")]
        [SerializeField] private Transform fitTarget;
        [SerializeField] private float fitWidth = 1.7f;

        private bool? lastPortrait;
        private float lastAspect;
        private Tween shakeTween;


        public Camera MainCamera => mainCamera;



        private void Start()
        {
            Apply(true);
            StackGuiCamera();
        }

        /// <summary>
        /// URP không hỗ trợ Clear Flags = Depth only cho camera Base, nên GUI Camera phải là Overlay trong stack của MainCamera,
        /// nếu không nó sẽ xoá mất hình của MainCamera.
        /// </summary>
        private void StackGuiCamera()
        {
            if (mainCamera == null || GUICamera.Instance == null || GUICamera.Instance.Camera == null)
                return;

            var guiCamera = GUICamera.Instance.Camera;
            guiCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            var stack = mainCamera.GetUniversalAdditionalCameraData().cameraStack;
            if (stack.Contains(guiCamera) == false)
                stack.Add(guiCamera);
        }

        private void LateUpdate()
        {
            Apply(false);
        }

        private void Apply(bool force)
        {
            bool portrait = OrientationUtility.IsPortrait;
            float aspect = OrientationUtility.Aspect;
            if (force == false && lastPortrait == portrait && Mathf.Approximately(lastAspect, aspect))
                return;

            lastPortrait = portrait;
            lastAspect = aspect;

            var pose = portrait && portraitPose != null ? portraitPose : landscapePose;
            if (pose != null && rig != null)
                rig.SetPositionAndRotation(pose.position, pose.rotation);

            float fov = portrait ? portraitFov : landscapeFov;
            if (fitTarget != null && mainCamera != null)
            {
                //FOV dọc tối thiểu để nhìn đủ fitWidth theo chiều ngang
                float distance = Vector3.Distance(mainCamera.transform.position, fitTarget.position);
                float halfH = Mathf.Atan(fitWidth * 0.5f / Mathf.Max(0.01f, distance));
                float needed = 2f * Mathf.Atan(Mathf.Tan(halfH) / Mathf.Max(0.1f, aspect)) * Mathf.Rad2Deg;
                fov = Mathf.Clamp(Mathf.Max(fov, needed), 20f, maxFov);
            }

            if (mainCamera != null)
                mainCamera.fieldOfView = fov;
        }

        public void Shake(float strength = 0.08f, float duration = 0.45f)
        {
            if (mainCamera == null)
                return;

            shakeTween?.Kill(true);
            mainCamera.transform.localPosition = Vector3.zero;
            shakeTween = mainCamera.transform.DOShakePosition(duration, strength, 30, 90, false, true)
                .OnComplete(() => mainCamera.transform.localPosition = Vector3.zero)
                .SetLink(mainCamera.gameObject);
        }
    }
}
