using DG.Tweening;

namespace ZeroX
{
    public class TransformTweenHolder
    {
        private TweenHolderSlot slotTweenPos = new TweenHolderSlot();
        private TweenHolderSlot slotTweenScale = new TweenHolderSlot();
        private TweenHolderSlot slotTweenRotation = new TweenHolderSlot();

        public void KillAll(bool complete = false)
        {
            slotTweenPos.Kill(complete);
            slotTweenScale.Kill(complete);
            slotTweenRotation.Kill(complete);
        }

        public void KillPos(bool complete = false)
        {
            slotTweenPos.Kill(complete);
        }
        
        public Tween RegisterPos(Tween newTween)
        {
            return slotTweenPos.Register(newTween);
        }
        
        public void KillScale(bool complete = false)
        {
            slotTweenScale.Kill(complete);
        }
        
        public Tween RegisterScale(Tween newTween)
        {
            return slotTweenScale.Register(newTween);
        }
        
        public void KillRotation(bool complete = false)
        {
            slotTweenRotation.Kill(complete);
        }
        
        public Tween RegisterRotation(Tween newTween)
        {
            return slotTweenRotation.Register(newTween);
        }
    }
}