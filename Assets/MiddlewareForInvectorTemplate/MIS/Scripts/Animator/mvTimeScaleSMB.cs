using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public class mvTimeScaleSMB : mvStateMachineBehaviour
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        [Header("Event")]
        public TriggerType triggerType = TriggerType.EnterState;
        public float normalizedTime = 0.5f;
        public float targetTimeScale = 1f;


        // ----------------------------------------------------------------------------------------------------
        // 
        protected bool hasTriggered;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (triggerType == TriggerType.EnterState)
                SetTimeScale();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.normalizedTime % 1 >= normalizedTime && !hasTriggered)
                SetTimeScale();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (triggerType == TriggerType.ExitState)
                SetTimeScale();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void SetTimeScale()
        {
            Time.timeScale = targetTimeScale;
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