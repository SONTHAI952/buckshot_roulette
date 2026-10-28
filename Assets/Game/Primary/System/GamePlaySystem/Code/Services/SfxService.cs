using Game.Primary.GamePlaySystem.Rules;
using UnityEngine;
using ZeroX.AudioSystem;

namespace Game.Primary.GamePlaySystem
{
    /// <summary>
    /// Phát SFX qua AudioSystem của ZeroX (group SFX)
    /// </summary>
    public class SfxService : MonoBehaviour
    {
        [Header("Wand")]
        [SerializeField] private AudioAsset zapHot;
        [SerializeField] private AudioAsset clickCold;
        [SerializeField] private AudioAsset load;
        [SerializeField] private AudioAsset eject;
        [SerializeField] private AudioAsset tension;

        [Header("Items")]
        [SerializeField] private AudioAsset deal;
        [SerializeField] private AudioAsset peek;
        [SerializeField] private AudioAsset heal;
        [SerializeField] private AudioAsset boost;
        [SerializeField] private AudioAsset cuffs;
        [SerializeField] private AudioAsset leash;
        [SerializeField] private AudioAsset dice;
        [SerializeField] private AudioAsset pill;
        [SerializeField] private AudioAsset invert;

        [Header("Reactions")]
        [SerializeField] private AudioAsset playerHurt;
        [SerializeField] private AudioAsset mistressHurt;
        [SerializeField] private AudioAsset stageStart;
        [SerializeField] private AudioAsset roundWon;
        [SerializeField] private AudioAsset roundLost;


        public void Play(AudioAsset asset)
        {
            if (asset != null && asset.AudioClip != null)
                asset.PlayAvoidResonance();
        }

        public void PlayShot(ShellType type) => Play(type == ShellType.Live ? zapHot : clickCold);
        public void PlayLoad() => Play(load);
        public void PlayEject() => Play(eject);
        public void PlayTension() => Play(tension);
        public void PlayDeal() => Play(deal);
        public void PlayHurt(int playerId) => Play(playerId == PlayerIds.Player ? playerHurt : mistressHurt);
        public void PlayHeal() => Play(heal);
        public void PlayStageStart() => Play(stageStart);
        public void PlayRoundEnd(bool playerWon) => Play(playerWon ? roundWon : roundLost);

        public void PlayItem(ItemId itemId)
        {
            switch (itemId)
            {
                case ItemId.Peek: Play(peek); break;
                case ItemId.Eject: Play(eject); break;
                case ItemId.Heal: Play(heal); break;
                case ItemId.DoubleDamage: Play(boost); break;
                case ItemId.SkipTurn: Play(cuffs); break;
                case ItemId.Steal: Play(leash); break;
                case ItemId.RevealRandom: Play(dice); break;
                case ItemId.GambleHeal: Play(pill); break;
                case ItemId.Invert: Play(invert); break;
            }
        }
    }
}
