using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.Extensions
{
    public static class ImageExtension
    {
        public static void SetAlpha(this Image image, float alpha)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        public static Tween DoBlinkColor(this Image image, Color colorA, Color colorB, float duration,
            float vibrato = 10)
        {
            Sequence sequence = DOTween.Sequence(image);
            float timeStep = duration / vibrato;
            for (int i = 0; i < vibrato; i++)
            {
                Tween tween = image.DOColor(i % 2 == 0 ? colorB : colorA, timeStep);
                sequence.Append(tween);
            }

            sequence.OnComplete(() => image.color = colorA);
            sequence.OnKill(() => image.color = colorA);

            return sequence;
        }
    }
}