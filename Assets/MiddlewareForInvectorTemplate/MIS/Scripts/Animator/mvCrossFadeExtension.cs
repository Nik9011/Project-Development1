using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public static class mvCrossFadeExtension
    {
        public static void CrossFade(this Animator animator, mvCrossFadeData settings)
        {
            animator.CrossFade(settings.stateName, settings.transitionDuration, settings.layer, settings.timeOffset);
        }

        public static void CrossFadeInFixedTime(this Animator animator, mvCrossFadeData settings)
        {
            animator.CrossFadeInFixedTime(settings.stateName, settings.transitionDuration, settings.layer, settings.timeOffset);
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class mvCrossFadeData
    {
        public string stateName;
        [Min(-1)] public int layer;
        [Min(0f)] public float timeOffset;
        [Min(0f)] public float transitionDuration;
    }
}