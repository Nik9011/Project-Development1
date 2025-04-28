using System.Collections.Generic;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public class mvTriggerSoundSMB : mvStateMachineBehaviour
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        [Header("Event")]
        public TriggerType triggerType = TriggerType.EnterState;
        public float normalizedTime = 0.5f;
        
        [Header("Audio")]
        public GameObject audioSourcePrefab;
        public List<AudioClip> clipList;
        [Range(0f, 1f)] [SerializeField] protected float randomPitchRange = 0.2f;


        // ----------------------------------------------------------------------------------------------------
        // 
        protected GameObject audioSourceObject;
        protected bool hasTriggered;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (triggerType == TriggerType.EnterState)
                PlayClip(animator);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.normalizedTime % 1 >= normalizedTime && !hasTriggered)
                PlayClip(animator);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (triggerType == TriggerType.ExitState)
                PlayClip(animator);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void PlayClip(Animator animator)
        {
            if (audioSourcePrefab == null || clipList == null || clipList.Count == 0)
                return;

            if (audioSourceObject == null)
                audioSourceObject = Instantiate(audioSourcePrefab, animator.transform.position, Quaternion.identity) as GameObject;

            if (audioSourceObject.TryGetComponent(out AudioSource audioSource))
            {
                int random = Random.Range(0, clipList.Count);
                audioSource.pitch = Random.Range(1f - randomPitchRange, 1f + randomPitchRange);
                audioSource.PlayOneShot(clipList[random]);
            }

            hasTriggered = false;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        public enum TriggerType
        {
            EnterState = 0,
            NormalizedTime, 
            ExitState
        }
    }
}