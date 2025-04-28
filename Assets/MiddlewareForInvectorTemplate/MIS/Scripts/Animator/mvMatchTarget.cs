using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class mvMatchTarget
    {
        public string animation;
        public Transform matchTarget;
        public mvMatchTargetTime matchTime;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public mvMatchTarget()
        {
            matchTime = new mvMatchTargetTime(0f, 0.9f);
        }
        public mvMatchTarget(string animation) : this()
        {
            this.animation = animation;
        }
    }
}