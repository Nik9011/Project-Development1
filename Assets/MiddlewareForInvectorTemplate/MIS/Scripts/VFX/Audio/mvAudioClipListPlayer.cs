using Invector;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("AudioClipList Player", iconName = "misIconRed")]
    public class mvAudioClipListPlayer : mvAudioPlayer
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Settings", order = 0)]
        [Space(10)]
        public mvAudioClipListSO audioClipList;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void Play()
        {
            if (Source == null || audioClipList == null || audioClipList.list.Count == 0)
                return;

            Play(audioClipList.list[Random.Range(0, audioClipList.list.Count)], Random.Range(1f - pitchRange, 1f + pitchRange), loop, volume);
        }
        
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void PlayRandom(AudioClip[] clips, bool loop, float pitchRange, float volume)
        {
            Play(clips[Random.Range(0, clips.Length)], Random.Range(1f - pitchRange, 1f + pitchRange), loop, volume);
        }
    }
}