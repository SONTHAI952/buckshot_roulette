using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Primary.GamePlaySystem.Rules;
using UnityEngine;

namespace Game.Primary.WandSystem
{
    /// <summary>
    /// Bày các viên nạp ra giữa bàn ở đầu mỗi load (công khai số HOT/COLD), sau đó nạp vào Wand.
    /// </summary>
    public class ShellDisplayView : MonoBehaviour
    {
        [SerializeField] private ShellView shellPrefab;
        [SerializeField] private Transform rowCenter;
        [SerializeField] private float spacing = 0.07f;

        private readonly List<ShellView> shells = new List<ShellView>();


        public IEnumerator PlayLoad(int live, int blank, Color liveColor, Color blankColor, float showDuration, Transform loadInto)
        {
            Clear();
            int total = live + blank;
            Vector3 right = rowCenter.right;
            float start = -(total - 1) * 0.5f * spacing;

            //Bày theo nhóm để dễ đếm; thứ tự nạp thật đã được xáo trong GameRules
            for (int i = 0; i < total; i++)
            {
                var type = i < live ? ShellType.Live : ShellType.Blank;
                var shell = Instantiate(shellPrefab, rowCenter);
                shell.gameObject.SetActive(true);
                shell.Setup(type, type == ShellType.Live ? liveColor : blankColor);
                Vector3 target = rowCenter.position + right * (start + i * spacing);
                shell.transform.position = target + Vector3.up * 0.25f;
                shell.transform.localScale = Vector3.zero;
                var seq = DOTween.Sequence();
                seq.AppendInterval(i * 0.07f);
                seq.Append(shell.transform.DOMove(target, 0.3f).SetEase(Ease.OutBounce));
                seq.Join(shell.transform.DOScale(Vector3.one, 0.2f));
                seq.SetLink(shell.gameObject);
                shells.Add(shell);
            }

            yield return new WaitForSeconds(total * 0.07f + 0.35f + showDuration);

            //Nạp vào wand
            if (loadInto != null)
            {
                for (int i = 0; i < shells.Count; i++)
                {
                    var shell = shells[i];
                    var seq = DOTween.Sequence();
                    seq.AppendInterval(i * 0.06f);
                    seq.Append(shell.transform.DOMove(loadInto.position, 0.25f).SetEase(Ease.InCubic));
                    seq.Join(shell.transform.DOScale(0.4f, 0.25f));
                    seq.SetLink(shell.gameObject);
                }

                yield return new WaitForSeconds(shells.Count * 0.06f + 0.3f);
            }

            Clear();
        }

        public void Clear()
        {
            foreach (var shell in shells)
            {
                if (shell != null)
                    Destroy(shell.gameObject);
            }

            shells.Clear();
        }
    }
}
