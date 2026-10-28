using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.Primary.MistressSystem
{
    /// <summary>
    /// Nhân vật Mistress (vai trò Dealer). Hiện đang dùng model placeholder.
    /// Khi có model thật: đặt model vào modelRoot, gán Animator, tắt placeholder.
    /// Animator nên có các Trigger: Idle, Hit, Down, Revive, UseItem, Shoot, Cuffed (thiếu trigger nào thì bỏ qua).
    /// </summary>
    public class MistressView : MonoBehaviour, IPointerClickHandler
    {
        public const string TriggerIdle = "Idle";
        public const string TriggerHit = "Hit";
        public const string TriggerDown = "Down";
        public const string TriggerRevive = "Revive";
        public const string TriggerUseItem = "UseItem";
        public const string TriggerShoot = "Shoot";
        public const string TriggerCuffed = "Cuffed";

        [SerializeField] private Transform modelRoot;
        [SerializeField] private GameObject placeholder;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject cuffs;
        [SerializeField] private TMP_Text speechText;
        [SerializeField] private Renderer[] flashRenderers;
        [SerializeField] private Color hitFlashColor = new Color(1f, 0.15f, 0.5f);

        [Header("Hooks cho model/animation thật")]
        public UnityEvent onHit;
        public UnityEvent onDown;
        public UnityEvent onRevive;
        public UnityEvent onUseItem;
        public UnityEvent onShoot;
        public UnityEvent onCuffed;

        private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");
        private MaterialPropertyBlock mpb;
        private Vector3 baseLocalPos;
        private Quaternion baseLocalRot;
        private Coroutine speechCoroutine;


        public event Action OnClicked;

        public Transform ModelRoot => modelRoot;



        private void Awake()
        {
            baseLocalPos = transform.localPosition;
            baseLocalRot = transform.localRotation;
            SetCuffed(false);
            if (speechText != null)
                speechText.text = "";
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke();
        }

        public void ResetState()
        {
            transform.DOKill();
            transform.localPosition = baseLocalPos;
            transform.localRotation = baseLocalRot;
            SetCuffed(false);
            SetFlash(Color.black);
            Trigger(TriggerIdle);
        }



        #region Reactions

        public IEnumerator PlayHit(int damage)
        {
            Trigger(TriggerHit);
            onHit?.Invoke();
            SetFlash(hitFlashColor * (damage > 1 ? 4f : 2.5f));
            var seq = DOTween.Sequence();
            seq.Append(transform.DOPunchRotation(new Vector3(-12 * damage, 0, 6), 0.5f, 10));
            seq.Join(transform.DOShakePosition(0.5f, 0.03f * damage, 30));
            seq.SetLink(gameObject);
            yield return seq.WaitForCompletion();
            SetFlash(Color.black);
            transform.localPosition = baseLocalPos;
        }

        public IEnumerator PlayDown()
        {
            Trigger(TriggerDown);
            onDown?.Invoke();
            yield return transform.DOLocalRotateQuaternion(baseLocalRot * Quaternion.Euler(-25, 0, 0), 0.8f)
                .SetEase(Ease.OutBounce).SetLink(gameObject).WaitForCompletion();
        }

        public IEnumerator PlayRevive()
        {
            Trigger(TriggerRevive);
            onRevive?.Invoke();
            yield return transform.DOLocalRotateQuaternion(baseLocalRot, 0.6f).SetLink(gameObject).WaitForCompletion();
        }

        public void PlayUseItem()
        {
            Trigger(TriggerUseItem);
            onUseItem?.Invoke();
        }

        public void PlayShoot()
        {
            Trigger(TriggerShoot);
            onShoot?.Invoke();
        }

        public void SetCuffed(bool value)
        {
            if (cuffs != null)
                cuffs.SetActive(value);

            if (value)
            {
                Trigger(TriggerCuffed);
                onCuffed?.Invoke();
            }
        }

        public void Say(string text, float duration = 2.2f)
        {
            if (speechText == null)
                return;

            if (speechCoroutine != null)
                StopCoroutine(speechCoroutine);
            speechCoroutine = StartCoroutine(SayRoutine(text, duration));
        }

        private IEnumerator SayRoutine(string text, float duration)
        {
            speechText.text = text;
            speechText.alpha = 0;
            yield return speechText.DOFade(1, 0.2f).SetLink(gameObject).WaitForCompletion();
            yield return new WaitForSeconds(duration);
            yield return speechText.DOFade(0, 0.3f).SetLink(gameObject).WaitForCompletion();
            speechText.text = "";
        }

        #endregion



        private void Trigger(string trigger)
        {
            if (animator == null || animator.runtimeAnimatorController == null)
                return;

            foreach (var parameter in animator.parameters)
            {
                if (parameter.name == trigger && parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    animator.SetTrigger(trigger);
                    return;
                }
            }
        }

        private void SetFlash(Color color)
        {
            if (flashRenderers == null)
                return;

            if (mpb == null)
                mpb = new MaterialPropertyBlock();

            foreach (var renderer in flashRenderers)
            {
                if (renderer == null)
                    continue;

                renderer.GetPropertyBlock(mpb);
                mpb.SetColor(EmissionId, color);
                renderer.SetPropertyBlock(mpb);
            }
        }
    }
}
