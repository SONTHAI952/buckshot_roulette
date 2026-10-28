using System.Collections;
using DG.Tweening;
using Game.Primary.GamePlaySystem.Rules;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Primary.TableSystem
{
    /// <summary>
    /// Một vật phẩm nằm trên bàn. Chỉ tự xử lý hiển thị của nó, click/hover được báo lên TableView.
    /// </summary>
    public class ItemView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ItemId itemId;
        [SerializeField] private Transform visual;
        [SerializeField] private Renderer highlight;
        [SerializeField] private Color hoverColor = new Color(1f, 0.3f, 0.6f, 0.35f);
        [SerializeField] private Color selectedColor = new Color(1f, 0.25f, 0.55f, 0.85f);
        [SerializeField] private Color stealableColor = new Color(1f, 0.85f, 0.2f, 0.7f);

        private static readonly int ColorId = Shader.PropertyToID("_BaseColor");
        private MaterialPropertyBlock mpb;
        private bool hovered;
        private bool selected;
        private bool stealable;
        private Tween pulseTween;
        private Vector3 visualBaseLocalPos;


        public ItemId ItemId => itemId;
        public int OwnerId { get; private set; }
        public int Slot { get; private set; }
        public TableView Table { get; private set; }



        private void Awake()
        {
            if (visual == null)
                visual = transform;

            visualBaseLocalPos = visual.localPosition;
            RefreshHighlight();
        }

        private void OnDestroy()
        {
            pulseTween?.Kill();
            transform.DOKill();
            if (visual != null)
                visual.DOKill();
        }

        public void Bind(TableView table, int ownerId, int slot)
        {
            Table = table;
            OwnerId = ownerId;
            Slot = slot;
        }



        #region Pointer

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Table != null)
                Table.NotifyItemClicked(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
            RefreshHighlight();
            if (Table != null)
                Table.NotifyItemHover(this, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
            RefreshHighlight();
            if (Table != null)
                Table.NotifyItemHover(this, false);
        }

        #endregion



        #region Highlight

        public void SetSelected(bool value)
        {
            if (selected == value)
                return;

            selected = value;
            visual.DOKill();
            visual.DOLocalMove(visualBaseLocalPos + (value ? Vector3.up * 0.05f : Vector3.zero), 0.15f);
            RefreshHighlight();
        }

        public void SetStealable(bool value)
        {
            stealable = value;
            RefreshHighlight();
        }

        public void ClearHighlight()
        {
            hovered = false;
            stealable = false;
            SetSelected(false);
            RefreshHighlight();
        }

        private void RefreshHighlight()
        {
            if (highlight == null)
                return;

            pulseTween?.Kill();
            bool on = selected || stealable || hovered;
            highlight.gameObject.SetActive(on);
            if (on == false)
                return;

            Color color = selected ? selectedColor : stealable ? stealableColor : hoverColor;
            SetHighlightColor(color);

            if (stealable && selected == false)
            {
                float t = 0;
                pulseTween = DOTween.To(() => t, x =>
                {
                    t = x;
                    var c = color;
                    c.a = Mathf.Lerp(0.25f, color.a, t);
                    SetHighlightColor(c);
                }, 1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }
        }

        private void SetHighlightColor(Color color)
        {
            if (mpb == null)
                mpb = new MaterialPropertyBlock();

            highlight.GetPropertyBlock(mpb);
            mpb.SetColor(ColorId, color);
            highlight.SetPropertyBlock(mpb);
        }

        #endregion



        #region Animation

        public IEnumerator PlaySpawn(Vector3 from, float delay)
        {
            Vector3 target = transform.position;
            Vector3 baseScale = transform.localScale;
            transform.position = from;
            transform.localScale = baseScale * 0.2f;

            var seq = DOTween.Sequence();
            seq.AppendInterval(delay);
            seq.Append(transform.DOJump(target, 0.25f, 1, 0.45f).SetEase(Ease.OutQuad));
            seq.Join(transform.DOScale(baseScale, 0.35f).SetEase(Ease.OutBack));
            seq.Join(transform.DORotate(new Vector3(0, 360, 0), 0.45f, RotateMode.LocalAxisAdd));
            seq.SetLink(gameObject);
            yield return seq.WaitForCompletion();
        }

        /// <summary> Nhấc lên, làm động tác theo loại item rồi biến mất </summary>
        public IEnumerator PlayUse(Vector3 focusPoint)
        {
            ClearHighlight();
            var seq = DOTween.Sequence();
            Vector3 lifted = Vector3.Lerp(transform.position, focusPoint, 0.55f) + Vector3.up * 0.18f;
            seq.Append(transform.DOMove(lifted, 0.35f).SetEase(Ease.OutCubic));

            switch (itemId)
            {
                case ItemId.Eject:
                    seq.Append(transform.DOShakeRotation(0.35f, new Vector3(0, 0, 35), 18));
                    seq.Append(transform.DOPunchPosition(Vector3.up * 0.08f, 0.2f, 6));
                    break;

                case ItemId.DoubleDamage:
                    seq.Append(transform.DORotate(new Vector3(0, 0, -120), 0.12f, RotateMode.LocalAxisAdd).SetEase(Ease.InQuad));
                    seq.Append(transform.DORotate(new Vector3(0, 0, 120), 0.25f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack));
                    break;

                case ItemId.SkipTurn:
                case ItemId.Steal:
                    seq.Append(transform.DOMove(focusPoint, 0.35f).SetEase(Ease.InOutQuad));
                    seq.Join(transform.DORotate(new Vector3(0, 540, 0), 0.35f, RotateMode.LocalAxisAdd));
                    break;

                case ItemId.RevealRandom:
                    seq.Append(transform.DOJump(lifted + Vector3.down * 0.15f, 0.15f, 3, 0.6f));
                    seq.Join(transform.DORotate(new Vector3(720, 360, 180), 0.6f, RotateMode.LocalAxisAdd));
                    break;

                case ItemId.Heal:
                case ItemId.GambleHeal:
                    seq.Append(transform.DOMove(focusPoint, 0.3f).SetEase(Ease.InQuad));
                    break;

                case ItemId.Invert:
                    seq.Append(transform.DORotate(new Vector3(0, 0, 70), 0.25f, RotateMode.LocalAxisAdd));
                    seq.AppendInterval(0.25f);
                    break;

                default:
                    seq.Append(transform.DOMove(focusPoint, 0.35f).SetEase(Ease.InOutQuad));
                    seq.AppendInterval(0.2f);
                    break;
            }

            seq.Append(transform.DOScale(0f, 0.2f).SetEase(Ease.InBack));
            seq.SetLink(gameObject);
            yield return seq.WaitForCompletion();
            Destroy(gameObject);
        }

        /// <summary> Bị cướp: bay từ phía đối thủ sang phía người cướp rồi được dùng ngay </summary>
        public IEnumerator PlayStolen(Vector3 thiefFocus)
        {
            ClearHighlight();
            yield return transform.DOJump(thiefFocus, 0.3f, 1, 0.45f).SetLink(gameObject).WaitForCompletion();
        }

        #endregion
    }
}
