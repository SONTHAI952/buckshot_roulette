using UnityEngine;

namespace ZeroX.Extensions
{
    public static class ParticleSystemExtension
    {
        public static void Restart(this ParticleSystem particleSystem)
        {
            particleSystem.Stop();
            particleSystem.Play();
        }
    }
}