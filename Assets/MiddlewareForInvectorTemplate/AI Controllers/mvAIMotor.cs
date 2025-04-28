#if MIS_FSM_AI && INVECTOR_AI_TEMPLATE
using Invector;
using Invector.vCharacterController.AI;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("mvAIMotor", iconName = "misIconRed")]
    public class mvAIMotor : vAIMotor
    {
        public delegate void OnUpdateEvent();
        //public event OnUpdateEvent onFixedUpdate;
        public event OnUpdateEvent onUpdate;
#if MIS_FSM_AI
        public event OnUpdateEvent onAnimatorMove;
        public event OnUpdateEvent onLateUpdate;
#endif


        // ----------------------------------------------------------------------------------------------------
        // MIS Packages
        // ----------------------------------------------------------------------------------------------------

#if MIS_AI_CARRIDER_EVP || MIS_AI_CARRIDER_RCC
        [HideInInspector] public mvAIVehicleRider misAIVehicleRider;

        public bool IsAvailableAIVehicleRider
        {
            get => misAIVehicleRider != null && misAIVehicleRider.IsAvailable;
        }
        public bool IsOnActionAIVehicleRider
        {
            get => IsAvailableAIVehicleRider && misAIVehicleRider.IsOnAction;
        }
#endif


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void Start()
        {
            base.Start();

            // ----------------------------------------------------------------------------------------------------
            // MIS Packages
            // ----------------------------------------------------------------------------------------------------
#if MIS_AI_CARRIDER_EVP || MIS_AI_CARRIDER_RCC
            TryGetComponent(out misAIVehicleRider);
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void UpdateAI()
        {
            base.UpdateAI();

            onUpdate?.Invoke();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (animator && isGrounded && !ragdolled)
            {
                // use root rotation for custom actions or 
                if (customAction || input.magnitude < 0.1f)
                {
                    if (!_rigidbody.isKinematic)
                        _rigidbody.velocity = Vector3.zero;

#if MIS_FSM_AI
                    transform.position = animator.rootPosition;
                    transform.rotation = animator.rootRotation;
#else
                    _rigidbody.position = animator.rootPosition;
                    transform.rotation = animator.rootRotation;
#endif

                    return;
                }
                if (lockMovement)
                {
                    //_rigidbody.velocity = Vector3.zero;
                    _rigidbody.position = animator.rootPosition;
                    if (lockRotation)
                    {
                        transform.rotation = animator.rootRotation;
                    }

                    return;
                }
                if (lockRotation)
                {
                    _rigidbody.rotation = animator.rootRotation;
                }

                var a_strafeSpeed = Mathf.Abs(strafeMagnitude);

                // strafe extra speed
                if (isStrafing)
                {
                    if (a_strafeSpeed <= 0.5f)
                    {
                        ControlSpeed(strafeSpeed.walkSpeed);
                    }
                    else if (a_strafeSpeed > 0.5f && a_strafeSpeed <= 1f)
                    {
                        ControlSpeed(strafeSpeed.runningSpeed);
                    }
                    else
                    {
                        ControlSpeed(strafeSpeed.sprintSpeed);
                    }

                    if (isCrouching)
                    {
                        ControlSpeed(strafeSpeed.crouchSpeed);
                    }
                }
                else if (!isStrafing)
                {
                    // free extra speed                
                    if (speed <= 0.5f)
                    {
                        ControlSpeed(freeSpeed.walkSpeed);
                    }
                    else if (speed > 0.5 && speed <= 1f)
                    {
                        ControlSpeed(freeSpeed.runningSpeed);
                    }
                    else
                    {
                        ControlSpeed(freeSpeed.sprintSpeed);
                    }

                    if (isCrouching)
                    {
                        ControlSpeed(freeSpeed.crouchSpeed);
                    }
                }
            }

#if MIS_FSM_AI
            if (animator && !ragdolled)
                onAnimatorMove?.Invoke();
#endif
        }

#if MIS_FSM_AI
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void LateUpdate()
        {
            onLateUpdate?.Invoke();
        }
#endif
    }
}
#endif