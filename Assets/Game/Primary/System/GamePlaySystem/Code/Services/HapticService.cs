using UnityEngine;
using ZeroX.HapticSystem;

namespace Game.Primary.GamePlaySystem
{
    public class HapticService : MonoBehaviour
    {
        public void PlayHit(int damage)
        {
            if (damage > 1)
                HapticManager.PlayHeavyImpact();
            else
                HapticManager.PlayMediumImpact();
        }

        public void PlayLight()
        {
            HapticManager.PlayLightImpact();
        }
    }
}
