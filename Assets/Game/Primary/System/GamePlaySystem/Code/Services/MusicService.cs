using DG.Tweening;
using UnityEngine;
using ZeroX.AudioSystem;

namespace Game.Primary.GamePlaySystem
{
    /// <summary>
    /// Nhạc nền ambient, route qua group Music của AudioSystem
    /// </summary>
    public class MusicService : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioAsset ambient;
        [SerializeField] private float volume = 0.55f;

        private void Start()
        {
            if (audioSource == null || ambient == null || ambient.AudioClip == null)
                return;

            AudioManager.PlayOnAudioSource(ambient, audioSource);
            audioSource.loop = true;
            audioSource.volume = 0;
            audioSource.DOFade(volume, 3f).SetLink(gameObject);
        }
    }
}
