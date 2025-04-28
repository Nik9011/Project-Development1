using Invector;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("ResetParticleLoop", iconName = "misIconRed")]
    public class mvResetParticleLoop : vMonoBehaviour
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        ParticleSystem[] particleSystems;
        int particleCount;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void OnEnable()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>();

            if (particleSystems != null && particleSystems.Length > 0)
                particleCount = particleSystems.Length;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void ResetParticleLoop(bool loop)
        {
            for (int i = 0; i < particleCount; i++)
            {
                ParticleSystem.MainModule main = particleSystems[i].main;
                main.loop = loop;
            }
        }
    }
}