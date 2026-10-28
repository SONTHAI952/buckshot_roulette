using System.Collections.Generic;
using DG.Tweening;
using Game.Primary.GamePlaySystem.Rules;
using TMPro;
using UnityEngine;

namespace Game.Primary.StaminaSystem
{
    /// <summary>
    /// Hộp Stamina Monitor trên bàn (vai trò hộp defibrillator): mỗi bên một hàng trái tim.
    /// </summary>
    public class StaminaMonitorView : MonoBehaviour
    {
        [SerializeField] private Transform playerRow;
        [SerializeField] private Transform dealerRow;
        [SerializeField] private Renderer heartPrefab;
        [SerializeField] private float heartSpacing = 0.05f;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private Color onColor = new Color(1f, 0.2f, 0.55f);
        [SerializeField] private Color offColor = new Color(0.12f, 0.05f, 0.08f);
        [SerializeField] private Color lockedColor = new Color(0.55f, 0.02f, 0.02f);

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");

        private readonly List<Renderer> playerHearts = new List<Renderer>();
        private readonly List<Renderer> dealerHearts = new List<Renderer>();
        private readonly int[] values = new int[2];
        private readonly bool[] locked = new bool[2];
        private MaterialPropertyBlock mpb;


        public void Setup(int maxCharges)
        {
            Build(playerRow, playerHearts, maxCharges);
            Build(dealerRow, dealerHearts, maxCharges);
            values[0] = values[1] = maxCharges;
            locked[0] = locked[1] = false;
            Refresh(PlayerIds.Player);
            Refresh(PlayerIds.Dealer);
            SetStatus("");
        }

        private void Build(Transform row, List<Renderer> hearts, int count)
        {
            foreach (var heart in hearts)
            {
                if (heart != null)
                    Destroy(heart.gameObject);
            }

            hearts.Clear();
            float start = -(count - 1) * 0.5f * heartSpacing;
            for (int i = 0; i < count; i++)
            {
                var heart = Instantiate(heartPrefab, row);
                heart.gameObject.SetActive(true);
                heart.transform.localPosition = new Vector3(start + i * heartSpacing, 0, 0);
                heart.transform.localRotation = Quaternion.identity;
                hearts.Add(heart);
            }
        }

        public void SetCharges(int playerId, int value, bool healingDisabled)
        {
            int previous = values[playerId];
            values[playerId] = value;
            locked[playerId] = healingDisabled;
            Refresh(playerId);

            var hearts = playerId == PlayerIds.Player ? playerHearts : dealerHearts;
            int changedIndex = value < previous ? value : value - 1;
            if (changedIndex >= 0 && changedIndex < hearts.Count && previous != value)
            {
                var heart = hearts[changedIndex].transform;
                heart.DOKill(true);
                heart.DOPunchScale(Vector3.one * 0.6f, 0.4f, 8).SetLink(heart.gameObject);
            }
        }

        public void SetStatus(string text)
        {
            if (statusText != null)
                statusText.text = text;
        }

        private void Refresh(int playerId)
        {
            var hearts = playerId == PlayerIds.Player ? playerHearts : dealerHearts;
            for (int i = 0; i < hearts.Count; i++)
            {
                bool on = i < values[playerId];
                Color color = on ? (locked[playerId] ? lockedColor : onColor) : offColor;
                SetColor(hearts[i], color, on ? 2f : 0f);
            }
        }

        private void SetColor(Renderer renderer, Color color, float emission)
        {
            if (mpb == null)
                mpb = new MaterialPropertyBlock();

            renderer.GetPropertyBlock(mpb);
            mpb.SetColor(BaseColorId, color);
            mpb.SetColor(EmissionId, color * emission);
            renderer.SetPropertyBlock(mpb);
        }
    }
}
