using UnityEngine;

namespace ZeroX.ToggleSystem
{
    public class ZToggleTargetActiveGameObject : ZToggleTarget
    {
        [SerializeField] GameObject onGameObject;


        protected override void Reset()
        {
            base.Reset();
            onGameObject = gameObject;
        }

        public override void UpdateTarget(ZToggle toggle)
        {
            onGameObject.SetActive(toggle.IsOn);
        }
        

        public override void UpdateTargetImmediate(ZToggle toggle)
        {
            onGameObject.SetActive(toggle.IsOn);
        }
    }
}