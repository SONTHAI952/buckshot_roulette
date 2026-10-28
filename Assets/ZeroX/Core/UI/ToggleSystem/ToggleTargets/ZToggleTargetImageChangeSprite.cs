using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.ToggleSystem
{
    public class ZToggleTargetImageChangeSprite : ZToggleTarget
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite offSprite;
        [SerializeField] private Sprite onSprite;
        
        
        public override void UpdateTarget(ZToggle toggle)
        {
            image.sprite = toggle.IsOn ? onSprite : offSprite;
        }

        public override void UpdateTargetImmediate(ZToggle toggle)
        {
            image.sprite = toggle.IsOn ? onSprite : offSprite;
        }
    }
}