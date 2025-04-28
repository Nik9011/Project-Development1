using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class mvMatchTargetTime
    {
        [Range(0f, 1f)] public float start;
        [Range(0f, 1f)] public float end;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public mvMatchTargetTime()
        {
        }
        public mvMatchTargetTime(float start, float end)
        {
            this.start = start;
            this.end = end;
        }
        public mvMatchTargetTime(mvMatchTargetTime matchTime)
        {
            start = matchTime.start;
            end = matchTime.end;
        }
    }
}