#pragma warning disable 0414

#if MIS_INVECTOR_PUSH
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public class mvPushObjectPoint : vPushObjectPoint
    {
        [Header("Animations")]
        [Tooltip("The Animator State name to be used to start and end this Push action")]
        public string startAnimation = "StartPushAndPull";
        public string stopAnimation = "StopPushAndPull";

        [Header("Auto Strength")]
        [Tooltip("Automatically set to the Strength equal to the target Rigidbody Mass")]
        public bool useAutoStrength = true;
        public float autoStrengthMultiplier = 1f;


        // ----------------------------------------------------------------------------------------------------
        // 
        public virtual bool CanUse
        {
            get => !pushableObject.isStoppig /*&& MISMath.IsCircaZero(targetBody.velocity.magnitude)*/;
        }


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void Start()
        {
        }
    }
}
#endif