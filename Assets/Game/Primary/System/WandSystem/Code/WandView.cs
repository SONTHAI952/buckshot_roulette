using System;
using System.Collections;
using DG.Tweening;
using Game.Primary.GamePlaySystem.Rules;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Primary.WandSystem
{
    /// <summary>
    /// Love Wand (vai trò shotgun). Chỉ lo hiển thị: cầm lên, chĩa, "bắn", eject, trạng thái boost.
    /// </summary>
    public class WandView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Anchors")]
        [SerializeField] private Transform restAnchor;
        [SerializeField] private Transform playerAimDealerAnchor;
        [SerializeField] private Transform playerAimSelfAnchor;
        [SerializeField] private Transform dealerAimPlayerAnchor;
        [SerializeField] private Transform dealerAimSelfAnchor;

        [Header("Visual")]
        [SerializeField] private Renderer headRenderer;
        [SerializeField] private Light headLight;
        [SerializeField] private Transform ejectPoint;
        [SerializeField] private GameObject boostFx;
        [SerializeField] private Renderer highlight;
        [SerializeField] private ParticleSystem zapParticles;
        [SerializeField] private ShellView shellPrefab;
        [SerializeField] private Color idleGlow = new Color(1f, 0.4f, 0.7f) * 0.4f;
        [SerializeField] private Color boostGlow = new Color(1f, 0.05f, 0.1f) * 1.5f;

        private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");
        private MaterialPropertyBlock mpb;
        private bool interactable;
        private bool boosted;
        private Transform currentAnchor;


        public event Action OnClicked;

        public Transform EjectPoint => ejectPoint;
        public Transform RestAnchor => restAnchor;

        public bool Interactable
        {
            get => interactable;
            set
            {
                interactable = value;
                if (value == false)
                    SetHighlight(false);
            }
        }



        private void Awake()
        {
            SetGlow(idleGlow);
            SetHighlight(false);
            if (boostFx != null)
                boostFx.SetActive(false);
            if (headLight != null)
                headLight.intensity = 0;
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public void ResetToRest()
        {
            transform.DOKill();
            currentAnchor = restAnchor;
            transform.SetPositionAndRotation(restAnchor.position, restAnchor.rotation);
            SetBoosted(false);
            Interactable = false;
        }



        #region Pointer

        public void OnPointerClick(PointerEventData eventData)
        {
            if (interactable)
                OnClicked?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (interactable)
                SetHighlight(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetHighlight(false);
        }

        public void SetHighlight(bool value)
        {
            if (highlight != null)
                highlight.gameObject.SetActive(value);
        }

        #endregion



        #region Motion

        public IEnumerator MoveTo(Transform anchor, float duration = 0.45f)
        {
            if (anchor == null)
                yield break;

            currentAnchor = anchor;
            transform.DOKill();
            var seq = DOTween.Sequence();
            seq.Append(transform.DOMove(anchor.position, duration).SetEase(Ease.InOutCubic));
            seq.Join(transform.DORotateQuaternion(anchor.rotation, duration).SetEase(Ease.InOutCubic));
            seq.SetLink(gameObject);
            yield return seq.WaitForCompletion();
        }

        public Transform GetAimAnchor(int shooterId, int targetId)
        {
            if (shooterId == PlayerIds.Player)
                return targetId == PlayerIds.Player ? playerAimSelfAnchor : playerAimDealerAnchor;

            return targetId == PlayerIds.Dealer ? dealerAimSelfAnchor : dealerAimPlayerAnchor;
        }

        public IEnumerator Aim(int shooterId, int targetId)
        {
            yield return MoveTo(GetAimAnchor(shooterId, targetId), 0.55f);
            //Run nhẹ tạo hồi hộp
            yield return transform.DOShakeRotation(0.6f, 2.5f, 12).SetLink(gameObject).WaitForCompletion();
        }

        public IEnumerator Return()
        {
            yield return MoveTo(restAnchor, 0.4f);
        }

        #endregion



        #region Fire - Eject

        public IEnumerator Fire(ShellType type, Color shellColor, bool wasBoosted)
        {
            if (type == ShellType.Live)
            {
                SetGlow(shellColor * (wasBoosted ? 6f : 3.5f));
                if (headLight != null)
                {
                    headLight.color = shellColor;
                    headLight.intensity = wasBoosted ? 14f : 8f;
                    DOTween.To(() => headLight.intensity, x => headLight.intensity = x, 0f, 0.6f).SetLink(gameObject);
                }

                if (zapParticles != null)
                {
                    var main = zapParticles.main;
                    main.startColor = shellColor;
                    zapParticles.Play();
                }

                yield return transform.DOShakePosition(0.45f, wasBoosted ? 0.05f : 0.03f, 40, 90, false, false).SetLink(gameObject).WaitForCompletion();
            }
            else
            {
                //COLD: chỉ "tạch" một cái
                SetGlow(shellColor * 0.8f);
                yield return transform.DOPunchRotation(new Vector3(6, 0, 0), 0.25f, 8).SetLink(gameObject).WaitForCompletion();
            }

            SetBoosted(false);
            SetGlow(idleGlow);
        }

        public IEnumerator Eject(ShellType type, Color shellColor, Vector3 landPoint)
        {
            yield return transform.DOPunchPosition(Vector3.up * 0.03f, 0.2f, 6).SetLink(gameObject).WaitForCompletion();

            if (shellPrefab == null || ejectPoint == null)
                yield break;

            var shell = Instantiate(shellPrefab, ejectPoint.position, ejectPoint.rotation);
            shell.gameObject.SetActive(true);
            shell.Setup(type, shellColor);
            var seq = DOTween.Sequence();
            seq.Append(shell.transform.DOJump(landPoint, 0.25f, 1, 0.55f).SetEase(Ease.OutQuad));
            seq.Join(shell.transform.DORotate(new Vector3(360, 0, 540), 0.55f, RotateMode.LocalAxisAdd));
            seq.AppendInterval(1.4f);
            seq.Append(shell.transform.DOScale(0, 0.25f));
            seq.OnComplete(() => Destroy(shell.gameObject));
            seq.SetLink(shell.gameObject);
            yield return new WaitForSeconds(0.65f);
        }

        public IEnumerator Invert(Color flashColor)
        {
            SetGlow(flashColor * 2f);
            yield return transform.DOPunchScale(Vector3.one * 0.08f, 0.35f, 6).SetLink(gameObject).WaitForCompletion();
            SetGlow(boosted ? boostGlow : idleGlow);
        }

        public void SetBoosted(bool value)
        {
            boosted = value;
            if (boostFx != null)
                boostFx.SetActive(value);
            SetGlow(value ? boostGlow : idleGlow);
        }

        private void SetGlow(Color color)
        {
            if (headRenderer == null)
                return;

            if (mpb == null)
                mpb = new MaterialPropertyBlock();

            headRenderer.GetPropertyBlock(mpb);
            mpb.SetColor(EmissionId, color);
            headRenderer.SetPropertyBlock(mpb);
        }

        #endregion
    }
}
