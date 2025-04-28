using Invector;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public class mvSceneLinkedSMB<T> : SealedSMB where T : vMonoBehaviour
    {
        [HideInInspector] public T behaviour;

        bool isFirstFrame;
        bool isLastFrame;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void Initialize(Animator animator, T monoBehaviour)
        {
            mvSceneLinkedSMB<T>[] sceneLinkedSMBs = animator.GetBehaviours<mvSceneLinkedSMB<T>>();

            for (int i = 0; i < sceneLinkedSMBs.Length; i++)
                sceneLinkedSMBs[i].InternalInitialize(animator, monoBehaviour);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected void InternalInitialize(Animator animator, T monoBehaviour)
        {
            behaviour = monoBehaviour;
            OnStart(animator);
        }

        // ----------------------------------------------------------------------------------------------------
        // Called by a MonoBehaviour in the scene during its Start function.
        // ----------------------------------------------------------------------------------------------------
        public virtual void OnStart(Animator animator)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
            isFirstFrame = false;

            OnSLStateEnter(animator, stateInfo, layerIndex);
            OnSLStateEnter(animator, stateInfo, layerIndex, controller);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
            if (!animator.gameObject.activeSelf)
                return;

            if (animator.IsInTransition(layerIndex) && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnSLTransitionToStateUpdate(animator, stateInfo, layerIndex);
                OnSLTransitionToStateUpdate(animator, stateInfo, layerIndex, controller);
            }

            if (!animator.IsInTransition(layerIndex) && isFirstFrame)
            {
                OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
                OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex, controller);
            }

            if (animator.IsInTransition(layerIndex) && !isLastFrame && isFirstFrame)
            {
                isLastFrame = true;

                OnSLStatePreExit(animator, stateInfo, layerIndex);
                OnSLStatePreExit(animator, stateInfo, layerIndex, controller);
            }

            if (!animator.IsInTransition(layerIndex) && !isFirstFrame)
            {
                isFirstFrame = true;

                OnSLStatePostEnter(animator, stateInfo, layerIndex);
                OnSLStatePostEnter(animator, stateInfo, layerIndex, controller);
            }

            if (animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnSLTransitionFromStateUpdate(animator, stateInfo, layerIndex);
                OnSLTransitionFromStateUpdate(animator, stateInfo, layerIndex, controller);
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
            isLastFrame = false;

            OnSLStateExit(animator, stateInfo, layerIndex);
            OnSLStateExit(animator, stateInfo, layerIndex, controller);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public sealed override void OnStateMachineEnter(Animator animator, int stateMachinePathHash, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
            OnSLStateMachineEnter(animator, stateMachinePathHash);
            OnSLStateMachineEnter(animator, stateMachinePathHash, controller);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public sealed override void OnStateMachineExit(Animator animator, int stateMachinePathHash, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
            OnSLStateMachineExit(animator, stateMachinePathHash);
            OnSLStateMachineExit(animator, stateMachinePathHash, controller);
        }

        // ----------------------------------------------------------------------------------------------------
        // Called before Updates when execution of the state first starts (on transition to the state).
        // ----------------------------------------------------------------------------------------------------
        public virtual void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
        public virtual void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // Called after OnSLStateEnter every frame during transition to the state.
        // ----------------------------------------------------------------------------------------------------
        public virtual void OnSLTransitionToStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
        public virtual void OnSLTransitionToStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // Called on the first frame after the transition to the state has finished.
        // ----------------------------------------------------------------------------------------------------
        public virtual void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
        public virtual void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // Called every frame after PostEnter when the state is not being transitioned to or from.
        // ----------------------------------------------------------------------------------------------------
        public virtual void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
        public virtual void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // Called on the first frame after the transition from the state has started.
        // Note that if the transition has a duration of less than a frame, this will not be called.
        // ----------------------------------------------------------------------------------------------------
        public virtual void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
        public virtual void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // Called after OnSLStatePreExit every frame during transition to the state.
        // ----------------------------------------------------------------------------------------------------
        public virtual void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
        public virtual void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // Called after Updates when execution of the state first finshes (after transition from the state).
        // ----------------------------------------------------------------------------------------------------
        public virtual void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
        public virtual void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // OnStateMachineEnter is called when entering a statemachine via its Entry Node
        // ----------------------------------------------------------------------------------------------------
        public virtual void OnSLStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
        }
        public virtual void OnSLStateMachineEnter(Animator animator, int stateMachinePathHash, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // OnStateMachineExit is called when exiting a statemachine via its Exit Node
        // ----------------------------------------------------------------------------------------------------
        public virtual void OnSLStateMachineExit(Animator animator, int stateMachinePathHash)
        {

        }
        public virtual void OnSLStateMachineExit(Animator animator, int stateMachinePathHash, UnityEngine.Animations.AnimatorControllerPlayable controller)
        {

        }
    }

    // ----------------------------------------------------------------------------------------------------
    // This class repalce normal StateMachineBehaviour. It add the possibility of having direct reference to the object
    // the state is running on, avoiding the cost of retrienving it through a GetComponent every time.
    // c.f. Documentation for more in depth explainations.
    // ----------------------------------------------------------------------------------------------------
    public abstract class SealedSMB : mvStateMachineBehaviour
    {
        // ----------------------------------------------------------------------------------------------------
        // Called on the first Update frame when a state machine evaluate this state.
        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // Called at each Update frame except for the first and last frame.
        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // Called on the last update frame when a state machine evaluate this state.
        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // Called on the first Update frame when making a transition to a state machine.
        // This is not called when making a transition into a state machine sub-state.
        public sealed override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // Called on the last Update frame when making a transition out of a StateMachine.
        // This is not called when making a transition into a StateMachine sub-state.
        public sealed override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
        }
    }
}