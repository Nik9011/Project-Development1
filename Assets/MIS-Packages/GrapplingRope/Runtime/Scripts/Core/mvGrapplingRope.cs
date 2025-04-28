using Invector;
using Invector.vCharacterController;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public abstract class mvGrapplingRope : vMonoBehaviour
    {
#if MIS_GRAPPLINGROPE
        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Input")]
        public GenericInput aimInput = new GenericInput("T", "", "");
        public GenericInput throwInput = new GenericInput("Mouse0", "", "");
        public GenericInput cancelInput = new GenericInput("Y", "", "");
        public bool aimHoldingButton = false;

        [Tooltip("Camera State name to use when GrapplingHook action")]
        public string cameraState = "GrapplingRope";
        [Tooltip("If set to 0, no gravity applied on Throwing")]
        public float gravityMultiplier = 0.5f;


        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("GrapplingRope")]
        public mvSpringRope springRope;

        [Tooltip("If it is activated, the character will have unlimited access to the GrapplingRope.")]
        public bool useUnlimitedRope = true;
        public int ropeAmount = 0;
        public int ropeMaxAmount = 10;

        [Tooltip("Ignore all damage while grapplingrope, include Damage that ignore defence")]
        public bool noDamageOnAction = true;
        [Tooltip("Ignore damage that needs to activate ragdoll")]
        public bool noRagdollOnAction = true;


        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Movement")]
        [Tooltip("The Layer that the GrapplingRope can be installed.")]
        [SerializeField] protected LayerMask targetLayerMask = 1 << 0;

        [Tooltip("Multiplies the GrapplingHook movement speed")]
        public AnimationCurve moveSpeedCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.95f, 1.5f), new Keyframe(1f, 1f));

        [Tooltip("Y offset curve while the GrapplingRope movement.")]
        [SerializeField]
        protected AnimationCurve offsetYCurve =
            new AnimationCurve(new Keyframe(0f, 0f, 0f, 0.35f), new Keyframe(0.65f, 0.06f), new Keyframe(1f, 0f));

        [Tooltip("The minimum and maximum distance the GrapplingRope can be installed.")]
        public float distanceLimit =20f;

        [Tooltip("The speed at which the GrapplingRope moves the character.")]
        [SerializeField] protected float moveSpeed = 10f;

        [Header("Stamina")]
        public float grapplingStamina = 20f;
        public float staminaRecoveryDelay = 1.5f;
        protected float timer;

        [Header("Aiming Movement")]
        public bool canMoveOnAiming = true;


        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Audio")]
        [Tooltip("Audio clip to play when throwing the GrapplingRope.")]
        public AudioClip acThrowRope;
        [Tooltip("Audio clip to play when the character starts to move using the GrapplingRope.")]
        public AudioClip acStartGrappling;
        [Tooltip("The Pitch displacement to apply each time the audio clip is played. 0.2 means the Pitch value between 0.8 and 1.")]
        [Range(0f, 0.5f)]
        [SerializeField] protected float pitchRange = 0.2f;


        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Event")]
        [Tooltip("Called when transited the GrapplingRope aiming mode.")]
        public UnityEvent OnStartAction;
        [Tooltip("Called when transited the GrapplingRope aiming mode released.")]
        public UnityEvent OnCancelAction;
        [Tooltip("Called continuously when the character is being moved using the GrapplingRope.")]
        public UnityEvent OnMoving;
        [Tooltip("Called when the character movement has been finished using the GrapplingRope.")]
        public UnityEvent OnFinishAction;
        [Tooltip("Called when the GrapplingRope Collectable has been collected.")]
        public UnityEvent<int> OnRopeCollected;
        [Tooltip("Called when the rope is empty.")]
        public UnityEvent OnRopeEmpty;

        [Tooltip("The camera state name to use in the GrapplingRope aiming mode.")]
        [SerializeField] protected string aimingCameraState = "Aiming";


        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Debug", order = 100)]
        [mvReadOnly] public bool isAvailable;
        [mvReadOnly] public bool isOnAction;
        [mvReadOnly] public bool isOnMoveAction = false;
        [Tooltip("The GrapplingRope is on aiming mode.")]
        [mvReadOnly] public bool isAiming = false;
        [Tooltip("The GrapplingRope has a valid target.")]
        [mvReadOnly] public bool isValidTarget = false;
        [Tooltip("The GrapplingRope is being thrown.")]
        [mvReadOnly] public bool isThrowing = false;


        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("ChainedAction", order = 500)]
        public string chainedAction = "This field is required in order to prevent compiler error.";
#if MIS_AIRDASH
        public bool allowFromAirDash = true;
#endif
#if MIS_CARRIDER_EVP || MIS_CARRIDER_RCC || MIS_HELICOPTER
        public bool allowFromVehicleRider = false;
#endif
#if MIS_FREEFLYING
        public bool allowFromFreeFlying = true;
#endif
#if MIS_GROUNDDASH
        public bool allowFromGroundDash = true;
#endif
#if MIS_MOTORCYCLE
        public bool allowFromMotorcycle = true;
#endif
#if MIS_WALLRUN
        public bool allowFromWallRun = true;
#endif
#if MIS_WATERDASH
        public bool allowFromWaterDash = true;
#endif

#if MIS_INVECTOR_FREECLIMB
        public bool allowFromFreeClimb = true;
        [Range(0f, 120f)] public float maxAimingAngleOnFreeClimb = 100f;
        public bool autoTransitToFreeClimb = true;
#endif
#if MIS_INVECTOR_SHOOTERCOVER
        public bool allowFromShooterCover = true;
#endif
#if MIS_INVECTOR_SWIMMING
        public bool allowFromWaterSurface = true;
#endif


        // ----------------------------------------------------------------------------------------------------
        // Animatort
        protected int grapplingRopeStateHash = Animator.StringToHash("GrapplingRopeState");

        // ----------------------------------------------------------------------------------------------------
        // 
        public enum GrapplingRopeStateType
        {
            None = 0,
            Aiming,
            Throw,
            GrapplingMove
        };
        GrapplingRopeStateType grapplingRopeState;
        protected GrapplingRopeStateType GrapplingRopeState
        {
            get => grapplingRopeState;
            set
            {
                if (grapplingRopeState != value)
                {
                    grapplingRopeState = value;
                    cc.animator.SetInteger(grapplingRopeStateHash, (int)value);
                }
            }
        }


        // ----------------------------------------------------------------------------------------------------
        // 
        protected Vector3 lookAtOffset;
        protected Transform rightHandTransform, leftHandTransform;
        protected Vector3 aimPosition;

        protected float remainDistance, targetDistance;

        protected mvThirdPersonInput tpInput;
        protected mvThirdPersonController cc;
        protected Transform tr;

#if MIS_INVECTOR_FREECLIMB
        protected vHeadTrack headTrack;
#endif

        protected AudioSource audioSource;
        protected float colliderRadius;
        protected Vector3 startPosition, targetPosition;
        protected Vector3 newPosition;


        // ----------------------------------------------------------------------------------------------------
        // 
        mvGrapplingRopeUI grapplingRopeUI;
        protected mvGrapplingRopeUI GrapplingRopeUI
        {
            get
            {
                if (!grapplingRopeUI)
                    grapplingRopeUI = FindObjectOfType<mvGrapplingRopeUI>();

                return grapplingRopeUI;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        mvScreenCrosshair screenCrosshair;
        public virtual mvScreenCrosshair ScreenCrosshair
        {
            get
            {
                if (!screenCrosshair)
                {
                    screenCrosshair = FindObjectOfType<mvScreenCrosshair>();
                    if (screenCrosshair)
                        screenCrosshair.Init(cc);
                }

                return screenCrosshair;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        protected virtual Vector3 AimPoint
        {
            get => tpInput.cameraMain.transform.position + tpInput.cameraMain.transform.forward * distanceLimit;
        }
        protected virtual Vector3 AimDirection
        {
            get => tpInput.cameraMain.transform.forward;
        }

        // ----------------------------------------------------------------------------------------------------
        // if true, it means this action is not blocked and can be used
        public virtual bool IsAvailable
        {
            get => isAvailable;
            set => isAvailable = value;
        }

        // ----------------------------------------------------------------------------------------------------
        // if true, it means this action is currently being used
        public bool IsOnAction
        {
            get => isOnAction;
            set
            {
                if (value != isOnAction)
                    isOnAction = value;
            }
        }


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            tr = transform;
            audioSource = springRope.gameObject.GetComponent<AudioSource>();

            if (springRope && TryGetComponent(out tpInput) && TryGetComponent(out cc)
#if MIS_INVECTOR_FREECLIMB
                && TryGetComponent(out headTrack)
#endif
                    )
            {
                lookAtOffset = Vector3.up * cc.colliderHeight;
                rightHandTransform = cc.animator.GetBoneTransform(HumanBodyBones.RightHand);
                leftHandTransform = cc.animator.GetBoneTransform(HumanBodyBones.LeftHand);
                springRope.SetThrowTransform(rightHandTransform);

                colliderRadius = cc.colliderRadius;

                GrapplingRopeUI.UpdateAmount(ropeAmount, ropeMaxAmount);
                GrapplingRopeUI.SetActive(!useUnlimitedRope);

                IsAvailable = true;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void Update()
        {
            if (!IsAvailable)
                return;

#if MIS_INVECTOR_FREECLIMB
            if (!cc.IsVFreeClimbOnAction && cc.customAction)
                return;
#else
            if (cc.customAction)
                return;
#endif

            if (cc.isDead)
            {
                SetAimingMode(false, true);
                ReleaseHook(true);
                return;
            }

            if (!IsAvailable)
                return;

            GrapplingRopeUI.SetActive(!useUnlimitedRope);

            if (grapplingStamina > 0 && cc.currentStamina <= 0)
                return;

            UpdateThrowInput();
            UpdateCancelInput();

            if (GrapplingRopeState == GrapplingRopeStateType.Throw)
            {
                if (gravityMultiplier != 1f)
                {
                    var vel = cc._rigidbody.velocity;
                    vel.y = gravityMultiplier;
                    cc._rigidbody.velocity = vel;
                }
            }

            MoveAndRotate();

            if (isAiming)
                UpdateAimPosition();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool GrapplingRopeAimingCondition()
        {
            return
                !isAiming
                && !isThrowing
#if MIS_CARRIDER_EVP || MIS_CARRIDER_RCC || MIS_HELICOPTER
                && (cc.IsVehicleRiderOnAction ? (allowFromVehicleRider ? true : false) : true)
#endif
#if MIS_FREEFLYING
                && (cc.IsFreeFlyingOnAction ? (allowFromFreeFlying ? true : false) : true)
#endif
#if MIS_MOTORCYCLE
                && (cc.IsRiderOnAction ? (allowFromMotorcycle ? true : false) : true)
#endif
#if MIS_WALLRUN
                && (cc.IsWallRunOnAction ? (allowFromWallRun ? true : false) : true)
#endif
#if MIS_WATERDASH
                && (cc.IsWaterDashOnAction ? (allowFromWaterDash ? true : false) : true)
#endif

#if MIS_INVECTOR_FREECLIMB
                && (cc.IsVFreeClimbOnAction ? (allowFromFreeClimb ? true : false) : true)
#endif
#if MIS_INVECTOR_PARACHUTE
                && !cc.IsVParachuteOnAction
#endif
#if MIS_INVECTOR_PUSH
                && !cc.IsVPushOnAction
#endif
#if MIS_INVECTOR_SWIMMING
                && (cc.IsVSwimmingOnAction ? (!cc.IsVSwimmingUnderWater ? (allowFromWaterSurface ? true : false) : false) : true)
#endif
                ;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool GrapplingRopeThrowCondition()
        {
            return
                isAiming
                && isValidTarget
                && !isThrowing
#if MIS_AIRDASH
                && (cc.IsAirDashOnAction ? (allowFromAirDash ? true : false) : true)
#endif
#if MIS_FREEFLYING
                && (cc.IsFreeFlyingOnAction ? (allowFromFreeFlying ? true : false) : true)
#endif
#if MIS_GROUNDDASH
                && (cc.IsGroundDashOnAction ? (allowFromGroundDash ? true : false) : true)
#endif

#if MIS_INVECTOR_PARACHUTE
                && !cc.IsVParachuteOnAction
#endif
#if MIS_INVECTOR_PUSH
                && !cc.IsVPushOnAction
#endif
#if MIS_INVECTOR_SWIMMING
                && (cc.IsVSwimmingOnAction ? (!cc.IsVSwimmingUnderWater ? (allowFromWaterSurface ? true : false) : false) : true)
#endif
#if MIS_INVECTOR_SHOOTERCOVER
                && (cc.IsVShooterCoverOnAction ? (allowFromShooterCover ? true : false) : true)
#endif
                ;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckChainedAction()
        {
#if MIS_AIRDASH
            if (cc.IsAirDashOnAction && allowFromAirDash)
                cc.misAirDash.ExitActionState();
#endif
#if MIS_CARRIDER_EVP || MIS_CARRIDER_RCC || MIS_HELICOPTER
            if (cc.IsVehicleRiderOnAction && allowFromVehicleRider)
                cc.misVehicleRider.Interrupt();
#endif
#if MIS_FREEFLYING
            if (cc.IsFreeFlyingOnAction && allowFromFreeFlying)
                cc.misFreeFlying.ExitActionState();
#endif
#if MIS_GROUNDDASH
            if (cc.IsGroundDashOnAction && allowFromGroundDash)
                cc.misGroundDash.ExitActionState();
#endif
#if MIS_MOTORCYCLE
            if (cc.IsRiderOnAction && allowFromMotorcycle)
                cc.misRider.ExitByForce(false);
#endif
#if MIS_WALLRUN
            if (cc.IsWallRunOnAction && allowFromWallRun)
                cc.misWallRun.ExitActionState();
#endif
#if MIS_WATERDASH
            if (cc.IsWaterDashOnAction && allowFromWaterDash)
                cc.misWaterDash.ExitActionState();
#endif

#if MIS_INVECTOR_FREECLIMB
            if (cc.IsVFreeClimbOnAction && allowFromFreeClimb)
                cc.vmisFreeClimb.Interrupt();
#endif
#if MIS_INVECTOR_SHOOTERCOVER
            if (cc.IsVShooterCoverOnAction && allowFromShooterCover)
                cc.vmisShooterCover.ExitActionState(false);
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void UpdateThrowInput()
        {
            if (GrapplingRopeState == GrapplingRopeStateType.GrapplingMove)
                return;

            if (aimInput.useInput && aimInput.GetButtonDown() && GrapplingRopeAimingCondition())
            {
                if (!useUnlimitedRope && ropeAmount <= 0)
                {
                    OnRopeEmpty.Invoke();
                    return;
                }

                if (!string.IsNullOrEmpty(aimingCameraState))
                    tpInput.ChangeCameraState(aimingCameraState, false);

                SetAimingMode(true);
                return;
            }
            if (aimInput.useInput && aimInput.GetButtonUp() && aimHoldingButton && isAiming)
            { // Perform throw on aim release
                if (!useUnlimitedRope && ropeAmount <= 0)
                {
                    OnRopeEmpty.Invoke();
                    SetAimingMode(false, true);
                    tpInput.ResetCameraState();
                    return;
                }
                CheckChainedAction(); // Should be called after throwing a rope

                isAiming = false;
                isThrowing = true;

                SetAimingMode(false);
                tpInput.ResetCameraState();

                GrapplingRopeState = GrapplingRopeStateType.Throw;
                ThrowRope(); // actually throws the rope
                return;
            }

            if (throwInput.useInput && throwInput.GetButtonDown() && GrapplingRopeThrowCondition())
            {
                //CheckChainedAction(); // Should be called after throwing a rope

                isAiming = false;
                isThrowing = true;

                SetAimingMode(false);
                tpInput.ResetCameraState();

                GrapplingRopeState = GrapplingRopeStateType.Throw;
            }
            else if (aimInput.useInput && aimInput.GetButtonDown() && !aimHoldingButton && isAiming)
            {
                tpInput.ResetCameraState();

                SetAimingMode(false, true);
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void UpdateCancelInput()
        {
            if (cancelInput.useInput && cancelInput.GetButtonDown() && IsOnAction)
                Interrupt();
            //FinishGrapplingMove(true);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void SetAimingMode(bool enable, bool isCancled = false)
        {
            isAiming = enable;

            tpInput.SetStrafeLocomotion(enable);

            ScreenCrosshair.SetActiveAim(enable);

            if (enable)
            {
                if (!canMoveOnAiming)
                {
                    tpInput.SetLockBasicInput(true);
                    cc.lockMovement = true;
                }

                IsOnAction = true;
                GrapplingRopeState = GrapplingRopeStateType.Aiming;
                OnStartAction.Invoke();
            }
            else
            {
#if MIS_MOTORCYCLE
                if (!cc.IsRiderOnAction)
#endif
                {
                    tpInput.SetLockBasicInput(false);
                    cc.lockMovement = false;
                }

                if (isCancled)
                {
                    IsOnAction = false;
                    GrapplingRopeState = GrapplingRopeStateType.None;
                    OnCancelAction.Invoke();
                }
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void MoveAndRotate()
        {
            if (
                (isAiming || isThrowing)
#if MIS_MOTORCYCLE
                && !cc.IsRiderOnAction
#endif
                )
            {
                if (canMoveOnAiming)
                {
#if MIS_INVECTOR_FREECLIMB
                    if (!cc.IsVFreeClimbOnAction)
#endif
                        tpInput.MoveInput();
                }

#if MIS_INVECTOR_FREECLIMB
                if (!cc.IsVFreeClimbOnAction)
#endif
                    cc.RotateToDirection(tpInput.cameraMain.transform.forward, true);
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void UpdateAimPosition()
        {
            aimPosition = AimPoint;

            Ray ray = new Ray(tpInput.cameraMain.transform.position, AimDirection);

            if (Physics.Raycast(ray, out RaycastHit hit, distanceLimit, targetLayerMask))
            {
                if (hit.collider.transform.IsChildOf(tr))
                {
                    var collider = hit.collider;
                    var hits = Physics.RaycastAll(ray, distanceLimit, targetLayerMask);
                    var dist = distanceLimit;

                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].distance < dist && hits[i].collider.gameObject != collider.gameObject && !hits[i].collider.transform.IsChildOf(tr))
                        {
                            dist = hits[i].distance;
                            hit = hits[i];
                        }
                    }

                    if (hit.collider)
                        aimPosition = hit.point;
                }
                else
                {
                    aimPosition = hit.point;
                }

#if MIS_INVECTOR_FREECLIMB
                if (allowFromFreeClimb && cc.IsVFreeClimbOnAction)
                    isValidTarget = Vector3.Angle(tr.forward, ray.direction) <= maxAimingAngleOnFreeClimb;
                else
#endif
                    isValidTarget = true;
            }
            else
            {
                isValidTarget = false;
            }

            ScreenCrosshair.SetAimToCenter(isValidTarget);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void ThrowRope()
        {
            if (!useUnlimitedRope)
            {
                ropeAmount--;
                if (ropeAmount < 0)
                    ropeAmount = 0;
                GrapplingRopeUI.UpdateAmount(ropeAmount, ropeMaxAmount);
            }

            //isOnAction = true;
            isThrowing = true;

            if (acThrowRope)
            {
                audioSource.pitch = Random.Range(1f - pitchRange, 1f + pitchRange);
                audioSource.PlayOneShot(acThrowRope);
            }

            springRope.Throw(aimPosition);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void ReadyToGrappleRope()
        {
            CheckChainedAction();

            tpInput.SetLockAllInput(true);

#if MIS_INVECTOR_FREECLIMB
            if (autoTransitToFreeClimb)
                cc.input.z = 1f;
#endif

            cc.isGrounded = true;
            cc.disableCheckGround = true;
            cc.disableAnimations = true;
            cc.ResetInputAnimatorParameters();

            cc._rigidbody.useGravity = false;

            if (acStartGrappling)
            {
                audioSource.pitch = Random.Range(1f - pitchRange, 1f + pitchRange);
                audioSource.PlayOneShot(acStartGrappling);
            }

            GrapplingRopeState = GrapplingRopeStateType.GrapplingMove;
            cc.animator.CrossFadeInFixedTime("GrapplingRopeMove", 0.1f);

            startPosition = tr.position;
            timer = 0f;

            targetPosition = springRope.targetPosition + (Vector3.up * cc.colliderHeight);
            targetDistance = (targetPosition - startPosition).magnitude;

            isThrowing = false;
            isOnMoveAction = true;

            // To change the collider height during the Grappling move, 
            cc.isCrouching = true;
            cc.animator.SetBool(vAnimatorParameters.IsCrouching, true);

            tpInput.ChangeCameraState(cameraState, true);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void FixedUpdate()
        {
            if (!isOnMoveAction)
                return;

            if (cc.isDead)
            {
                FinishGrapplingMove();
                return;
            }

            if (tr.position.y > cc.heightReached)
                cc.heightReached = tr.position.y;

            remainDistance = Vector3.Distance(tr.position, targetPosition);
            timer += Time.fixedDeltaTime;

            if (remainDistance > colliderRadius)
            {
                if (grapplingStamina > 0)
                {
                    if (cc.currentStamina <= 0)
                    {
                        FinishGrapplingMove();
                        return;
                    }
                    else
                    {
                        cc.ReduceStamina(grapplingStamina, true);
                        cc.currentStaminaRecoveryDelay = staminaRecoveryDelay;
                    }
                }

                float remappedTime = MISMath.Remap(timer, 0f, targetDistance / moveSpeed, 0f, 1f);
                newPosition = Vector3.Lerp(startPosition, targetPosition, remappedTime);
                newPosition = Vector3.MoveTowards(newPosition, targetPosition, Time.fixedDeltaTime * moveSpeed * moveSpeedCurve.Evaluate(remappedTime));
                newPosition += Vector3.up * offsetYCurve.Evaluate(remappedTime);
                tr.position = newPosition;
                //m_Transform.SetPositionAndRotation(newPosition, Quaternion.LookRotation(springRope.targetPosition - m_Transform.position));

                if (remappedTime >= 1)
                    ReleaseHook(false);

                var euler = tr.eulerAngles;
                euler.x = 0f;
                tr.eulerAngles = euler;

                OnMoving.Invoke();
            }
            else
            {
                FinishGrapplingMove();
            }
        }

#if MIS_INVECTOR_FREECLIMB
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void LateUpdate()
        {
            if (!IsAvailable || !IsOnAction || !allowFromFreeClimb)
                return;

            if (cc.IsVFreeClimbOnAction)
                headTrack.UpdateHeadTrack();
        }
#endif

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void ReleaseHook(bool immediate = true)
        {
            springRope.Release(immediate);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void FinishGrapplingMove(bool byForce = false)
        {
            if (byForce)
                remainDistance = colliderRadius;

            if (!isOnMoveAction)
            {
                Debug.LogError("FinishGrapplingMove");
                return;
            }
            isOnMoveAction = false;

            ReleaseHook(true);
            isThrowing = false;

            audioSource.Stop();

            IsOnAction = false;
            GrapplingRopeState = GrapplingRopeStateType.None;

            cc.isCrouching = false;
            cc.isJumping = false;
            cc.isSprinting = false;
            cc.isStrafing = false;

            tpInput.SetLockAllInput(false);

            tpInput.lockMoveInput = false;
            cc.lockMovement = false;
            cc.lockRotation = false;

            cc.isGrounded = true;
            cc.disableCheckGround = false;
            cc.disableAnimations = false;
            cc.ResetInputAnimatorParameters();

            cc._rigidbody.useGravity = true;
            //?cc.animator.CrossFadeInFixedTime("Falling", 0.2f);

            tpInput.ResetCameraState();

            OnFinishAction.Invoke();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void Interrupt()
        {
            if (GrapplingRopeState == GrapplingRopeStateType.None)
                return;
            GrapplingRopeState = GrapplingRopeStateType.None;

            IsOnAction = false;
            audioSource.Stop();

            remainDistance = colliderRadius;

            ReleaseHook(true);
            isThrowing = false;
            isOnMoveAction = false;

            SetAimingMode(false, true);

            // Reset the collider height after the Grappling move, 
            cc.isCrouching = false;
            cc.isJumping = false;
            cc.isSprinting = false;
            cc.isStrafing = false;

            tpInput.SetLockAllInput(false);

            tpInput.lockMoveInput = false;
            cc.lockMovement = false;
            cc.lockRotation = false;

            cc.disableCheckGround = false;
            cc.disableAnimations = false;
            cc.ResetInputAnimatorParameters();

            cc._rigidbody.useGravity = true;

            tpInput.ResetCameraState();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void SetAmount(int value)
        {
            ropeAmount += value;

            if (GrapplingRopeUI)
                GrapplingRopeUI.UpdateAmount(ropeAmount, ropeMaxAmount);

            OnRopeCollected.Invoke(value);
        }
#endif
    }
}