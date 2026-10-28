using DG.Tweening;
using Game.Primary.GamePlaySystem.Rules;
using UnityEngine;

namespace Game.Primary.WandSystem
{
    /// <summary>
    /// Một viên nạp (HOT/COLD) dùng để bày ra bàn hoặc khi bị eject
    /// </summary>
    public class ShellView : MonoBehaviour
    {
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private Renderer markRenderer;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");
        private MaterialPropertyBlock mpb;

        public ShellType Type { get; private set; }

        public void Setup(ShellType type, Color color)
        {
            Type = type;
            if (mpb == null)
                mpb = new MaterialPropertyBlock();

            if (bodyRenderer != null)
            {
                bodyRenderer.GetPropertyBlock(mpb);
                mpb.SetColor(BaseColorId, color);
                mpb.SetColor(EmissionId, color * (type == ShellType.Live ? 1.4f : 0.5f));
                bodyRenderer.SetPropertyBlock(mpb);
            }

            //Ký hiệu phụ để không chỉ dựa vào màu: HOT có vạch sáng, COLD thì không
            if (markRenderer != null)
                markRenderer.gameObject.SetActive(type == ShellType.Live);
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}
