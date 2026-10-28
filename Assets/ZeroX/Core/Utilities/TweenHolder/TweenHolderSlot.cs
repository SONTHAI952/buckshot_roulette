using DG.Tweening;
using UnityEngine;

namespace ZeroX
{
    public struct TweenHolderSlot
    {
        public Tween tween;


        public void Kill(bool complete)
        {
            if(tween == null)
                return;

            //IsActive phải check trước: tween đã bị kill (autoKill khi complete) mà gọi IsComplete sẽ bị DOTween log warning "killed and is now invalid"
            if (tween.IsActive() == false || tween.IsComplete())
            {
                tween = null;
                return;
            }
            
            tween.Kill(complete);
            tween = null;
        }

        public Tween Register(Tween newTween)
        {
            if (tween != null)
            {
                Debug.LogError("Không thể register tween mới vì tween cũ vẫn tồn tại");
                return null;
            }
            
            tween = newTween;
            return tween;
        }
    }
}