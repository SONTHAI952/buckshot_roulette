using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ZeroX.Extensions
{
    public static class TMP_Text_Extension
    {
        public static Tween DoTextNumber(this TMP_Text text, int from, int to, float duration)
        {
            //int value = from;
            return DOTween.To(() => from, (v) => text.text = v.ToString(), to, duration).SetEase(Ease.Linear);
        }

        public static Tween DoTextNumber(this TMP_Text text, float from, float to, float duration)
        {
            //int value = from;
            return DOTween.To(() => from, (v) => text.text = v.ToString(), to, duration).SetEase(Ease.Linear);
        }

        public static Tween DoTextNumber(this TMP_Text text, string format, int from, int to, float duration)
        {
            float value = from;
            return DOTween.To(() => value,
                (v) =>
                {
                    value = v;
                    text.text = string.Format(format, (int) value);
                }, to, duration).SetEase(Ease.Linear).SetUpdate(true);
        }

        public static Tween DoTextNumber(this TMP_Text text, string format, float from, float to, float duration)
        {
            float value = from;
            return DOTween.To(() => value,
                (v) =>
                {
                    value = v;
                    text.text = string.Format(format, value);
                }, to, duration).SetEase(Ease.Linear).SetUpdate(true);
        }

        public static Tween DoBlinkColor(this TMP_Text text, Color colorA, Color colorB, float duration,
            float vibrato = 10)
        {
            Sequence sequence = DOTween.Sequence(text);
            float timeStep = duration / vibrato;
            for (int i = 0; i < vibrato; i++)
            {
                Tween tween = text.DOColor(i % 2 == 0 ? colorB : colorA, timeStep);
                sequence.Append(tween);
            }

            sequence.OnComplete(() => text.color = colorA);
            sequence.OnKill(() => text.color = colorA);

            return sequence;
        }

        public static void SetAlpha(this TMP_Text text, float alpha)
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }
    }
}