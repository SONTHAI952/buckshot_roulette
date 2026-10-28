using DG.Tweening;
using UnityEngine;

namespace ZeroX.Extensions
{
    public static class SpriteRendererExtension
    {
        public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        public static Tween DoBlinkColor(this SpriteRenderer spriteRenderer, Color colorA, Color colorB, float duration,
            float vibrato = 10)
        {
            Sequence sequence = DOTween.Sequence(spriteRenderer);
            float timeStep = duration / vibrato;
            for (int i = 0; i < vibrato; i++)
            {
                Tween tween = spriteRenderer.DOColor(i % 2 == 0 ? colorB : colorA, timeStep);
                sequence.Append(tween);
            }

            sequence.OnComplete(() => spriteRenderer.color = colorA);
            sequence.OnKill(() => spriteRenderer.color = colorA);

            return sequence;
        }
    }
}