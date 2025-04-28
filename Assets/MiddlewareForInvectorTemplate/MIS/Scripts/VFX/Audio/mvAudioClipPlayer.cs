using Invector;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("AudioClip Player", iconName = "misIconRed")]
    public class mvAudioClipPlayer : mvAudioPlayer
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Settings", order = 0)]
        public AudioClip clip;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void Play()
        {
            if (Source == null || clip == null)
                return;

            Play(clip, Random.Range(1f - pitchRange, 1f + pitchRange), loop, volume);
        }
    }
}