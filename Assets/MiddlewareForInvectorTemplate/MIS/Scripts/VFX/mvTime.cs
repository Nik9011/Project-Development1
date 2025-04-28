using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public static class mvTime
    {
        public static bool UseUnscaledTime { get; set; }

        // ----------------------------------------------------------------------------------------------------
        // 
        static bool IsUnscaledTime
        {
            get
            {
                return Time.timeScale != 1f && UseUnscaledTime;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        public static float timeScale
        {
            get
            {
                return Time.timeScale;
            }
            set
            {
                Time.timeScale = value;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        public static float deltaTime
        {
            get
            {
                return IsUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        public static float smoothDeltaTime
        {
            get
            {
                return IsUnscaledTime ? Time.unscaledDeltaTime : Time.smoothDeltaTime;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        public static float fixedDeltaTime
        {
            get
            {
                return IsUnscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
            }
            set
            {
                Time.fixedDeltaTime = value;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        public static float time
        {
            get
            {
                return IsUnscaledTime ? Time.unscaledTime : Time.time;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void SetMotionTimeScale(float newTimeScale)
        {
            timeScale = newTimeScale;
            fixedDeltaTime *= newTimeScale;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void ResetMotionTimeScale()
        {
            fixedDeltaTime /= timeScale;
            timeScale = 1f;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static float GetNormalizedTime(this Animator animator, int layer, int round = 2)
        {
            return (float)System.Math.Round(((animator.IsInTransition(layer) ? animator.GetNextAnimatorStateInfo(layer).normalizedTime : animator.GetCurrentAnimatorStateInfo(layer).normalizedTime) % 1), round);
        }
    }
}