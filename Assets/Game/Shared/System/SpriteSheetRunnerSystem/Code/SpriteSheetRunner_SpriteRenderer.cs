using System;
using UnityEngine;

namespace SpriteSheetRunnerSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteSheetRunner_SpriteRenderer : SpriteSheetRunner
    {
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}