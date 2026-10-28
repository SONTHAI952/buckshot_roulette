using UnityEngine;
using UnityEngine.UI;

namespace SpriteSheetRunnerSystem
{
    [RequireComponent(typeof(Image))]
    public class SpriteSheetRunner_Image : SpriteSheetRunner
    {
        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public override void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}