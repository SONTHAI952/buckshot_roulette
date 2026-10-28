using UnityEngine;

namespace ZeroX.ToggleSystem
{
    public class ZToggleTargetChangeGameObject : ZToggleTarget
    {
        [SerializeField] private GameObject offGameObject;
        [SerializeField] private GameObject onGameObject;
        public override void UpdateTarget(ZToggle toggle)
        {
            offGameObject.SetActive(toggle.IsOn == false);
            onGameObject.SetActive(toggle.IsOn);
        }

        public override void UpdateTargetImmediate(ZToggle toggle)
        {
            offGameObject.SetActive(toggle.IsOn == false);
            onGameObject.SetActive(toggle.IsOn);
        }
    }
}