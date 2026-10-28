using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using ZeroX.SingletonSystem;

namespace ZeroX.AudioSystem
{
    public class SimpleBGMusic : Singleton_ManualSpawn_Persistent<SimpleBGMusic>
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioAsset song1;


        protected override void WhenAwake()
        {
            base.WhenAwake();
        }

        protected override void WhenStart()
        {
            StartCoroutine(Timeline());
        }
        

        IEnumerator Timeline()
        {
            yield return null;
            yield return null;
            yield return null;
            AudioManager.PlayOnAudioSource(song1, audioSource);
            audioSource.DOFade(0.6f, 3).From(0);
            audioSource.loop = true;
        }
    }
}