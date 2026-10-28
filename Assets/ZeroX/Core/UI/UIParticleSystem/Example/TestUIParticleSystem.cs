using System;
using UnityEngine;
using ZeroX.Extensions;

namespace ZeroX.UI.Example
{
    public class TestUIParticleSystem : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //Restart có nghĩa là Stop rồi Start.
                //Cần làm như vậy đối vs particleSystemUI thì mới chạy được
                particleSystem.Restart();
            }
                
            
            if(particleSystem.isPlaying)
                Debug.Log("Is Playing");
        }
    }
}