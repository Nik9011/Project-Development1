using Invector;
using Invector.vCharacterController;
using Invector.vCharacterController.vActions;
#if MIS_INVECTOR_SHOOTERCOVER
using Invector.vCover;
#endif
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // Instead of vThirdPersonMotor class, mvThirdPersonMotor class will be mainly used for MIS and MIS Packages.
    // ----------------------------------------------------------------------------------------------------
    public partial class mvThirdPersonMotor : vThirdPersonMotor
    {
        #region Variables               

        #region Stamina       
        #endregion

        #region Crouch
        #endregion

        #region Character Variables       
        #endregion

        #region Actions
        #endregion

        #region Components
        #endregion

        #region Hide Variables
        #endregion

        #endregion


        // ----------------------------------------------------------------------------------------------------
        // MIS Packages
        // ----------------------------------------------------------------------------------------------------
#if MIS_AIRDASH
        [HideInInspector] public mvAirDash misAirDash;
        public bool IsAirDashAvailable => misAirDash != null && misAirDash.IsAvailable;
        public bool IsAirDashOnAction => IsAirDashAvailable && misAirDash.IsOnAction;
#endif

#if MIS_CARRIDER_EVP || MIS_CARRIDER_RCC || MIS_HELICOPTER || MIS_ROWINGBOAT
        [HideInInspector] public mvVehicleRider misVehicleRider;
        public bool IsVehicleRiderAvailable => misVehicleRider != null && misVehicleRider.IsAvailable;
        public bool IsVehicleRiderOnAction => IsVehicleRiderAvailable && misVehicleRider.IsOnAction;
        public mvVehicleAssistant.VehicleType RidingVehicleType => IsVehicleRiderOnAction ? misVehicleRider.vehicleAssistant.vehicleType : mvVehicleAssistant.VehicleType.None;
#endif

#if MIS_CRAWLING
        [HideInInspector] public mvCrawling misCrawling;
        public bool IsCrawlingAvailable => misCrawling != null && misCrawling.IsAvailable;
        public bool IsCrawlingOnAction => IsCrawlingAvailable && misCrawling.IsOnAction;
#endif

#if MIS_FREEFLYING
        [HideInInspector] public mvFreeFlying misFreeFlying;
        public bool IsFreeFlyingAvailable => misFreeFlying != null && misFreeFlying.IsAvailable;
        public bool IsFreeFlyingOnLanding => IsFreeFlyingAvailable && misFreeFlying.FlyingState == (int)FreeFlyingState.HardLanding;
        public bool IsFreeFlyingOnAction => IsFreeFlyingAvailable && misFreeFlying.IsOnAction;
#endif

#if MIS_GRAPPLINGHOOK
        [HideInInspector] public mvGrapplingHook misGrapplingHook;
        public bool IsGrapplingHookAvailable => misGrapplingHook != null && misGrapplingHook.IsAvailable;
        public bool IsGrapplingHookOnAction => IsGrapplingHookAvailable && misGrapplingHook.IsOnAction;
        public bool IsGrapplingHookOnMoveAction => IsGrapplingHookAvailable && misGrapplingHook.isOnMoveAction;
#endif

#if MIS_GRAPPLINGROPE
        [HideInInspector] public mvGrapplingRope misGrapplingRope;
        public bool IsGrapplingRopeAvailable => misGrapplingRope != null && misGrapplingRope.IsAvailable;
        public bool IsGrapplingRopeOnAction => IsGrapplingRopeAvailable && misGrapplingRope.IsOnAction;
        public bool IsGrapplingRopeOnMoveAction => IsGrapplingRopeAvailable && misGrapplingRope.isOnMoveAction;
#endif

#if MIS_GROUNDDASH
        [HideInInspector] public mvGroundDash misGroundDash;
        public bool IsGroundDashAvailable => misGroundDash != null && misGroundDash.IsAvailable;
        public bool IsGroundDashOnAction => IsGroundDashAvailable && misGroundDash.IsOnAction;
#endif

#if MIS_LANDINGROLL
        [HideInInspector] public mvLandingRoll misLandingRoll;
        public bool IsLandingRollAvailable => misLandingRoll != null && misLandingRoll.IsAvailable;
        public bool IsLandingRollOnAction => IsLandingRollAvailable && misLandingRoll.IsOnAction;
#endif

#if MIS_LEDGECLIMB1
        [HideInInspector] public mvLedgeClimb1 misLedgeClimb1;
#endif
#if MIS_LEDGECLIMB2
        [HideInInspector] public mvLedgeClimb2 misLedgeClimb2;
#endif

#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
        public bool IsLedgeClimbAvailable
        {
            get
            {
#if MIS_LEDGECLIMB1
                if (misLedgeClimb1 != null && misLedgeClimb1.IsAvailable)
                    return true;
                else
#endif
#if MIS_LEDGECLIMB2
                if (misLedgeClimb2 != null && misLedgeClimb2.IsAvailable)
                    return true;
                else
#endif
                    return false;
            }
        }

        public bool IsLedgeClimbOnAction
        {
            get
            {
                if (!IsLedgeClimbAvailable)
                    return false;

#if MIS_LEDGECLIMB1
                if (misLedgeClimb1 != null && misLedgeClimb1.IsOnAction)
                    return true;
                else
#endif
#if MIS_LEDGECLIMB2
                if (misLedgeClimb2 != null && misLedgeClimb2.IsOnAction)
                    return true;
                else
#endif
                    return false;
            }
        }
#endif

#if MIS_LOCKON || MIS_LOCKON2
        [HideInInspector] public mvLockOn misLockOn;
        public bool IsLockOnAvailable => misLockOn != null && misLockOn.IsAvailable;
        public bool IsLockOnHasTarget => IsLockOnAvailable && misLockOn.HasTarget();
#endif

#if MIS_MAGICSPELL
        [HideInInspector] public mvMagicSpell misMagicSpell;
        public bool IsMagicSpellAvailable => misMagicSpell != null && misMagicSpell.IsAvailable;
        public bool IsMagicSpellOnAction => IsMagicSpellAvailable && misMagicSpell.IsOnAction;
#endif

#if MIS_MOTORCYCLE
        [HideInInspector] public mvMotorcycleRider misRider;
        public bool IsRiderAvailable => misRider != null && misRider.IsAvailable;
        public bool IsRiderOnAction => IsRiderAvailable && misRider.IsOnAction;
#endif

#if MIS_MULTIJUMP
        [HideInInspector] public mvMultiJump misMultiJump;
        public bool IsMultiJumpAvailable => misMultiJump != null && misMultiJump.IsAvailable;
#endif

#if MIS_SOFTFLYING
        [HideInInspector] public mvSoftFlying misSoftFlying;
        public bool IsSoftFlyingAvailable => misSoftFlying != null && misSoftFlying.IsAvailable;
        public bool IsSoftFlyingOnAction => IsSoftFlyingAvailable && misSoftFlying.IsOnAction;
        public bool IsSoftFlyingOnLandingAction => IsSoftFlyingAvailable && misSoftFlying.FlyingState == mvSoftFlying.FlyingAnimatorState.Landing;
#endif

#if MIS_SWIMMING
        [HideInInspector] public mvSwimming misSwimming;
        public bool IsSwimAvailable => misSwimming != null && misSwimming.IsAvailable;
        public bool IsSwimOnAction => IsSwimAvailable && misSwimming.IsOnAction;
        public bool IsSwimOn => IsSwimAvailable && misSwimming.SwimMode != mvSwimming.SwimModeAnimatorState.None;
#endif

#if MIS_WATERDASH
        [HideInInspector] public mvWaterDash misWaterDash;
        public bool IsWaterDashAvailable => misWaterDash != null && misWaterDash.IsAvailable;
        public bool IsWaterDashOnAction => IsWaterDashAvailable && misWaterDash.IsOnAction;
#endif

#if MIS_WALLRUN
        [HideInInspector] public mvWallRun misWallRun;
        public bool IsWallRunAvailable => misWallRun != null && misWallRun.IsAvailable;
        public bool IsWallRunOnAction => IsWallRunAvailable && misWallRun.IsOnAction;
#endif


        // ----------------------------------------------------------------------------------------------------
        // INVECTOR Addons
        // ----------------------------------------------------------------------------------------------------
#if MIS_INVECTOR_BUILDER
        [HideInInspector] public vBuildManager vmisBuildManager;
        public bool IsVBuildManagerOnAction => vmisBuildManager != null && vmisBuildManager.inBuildMode;
#endif

#if MIS_INVECTOR_FREECLIMB
        [HideInInspector] public mvFreeClimb vmisFreeClimb;
        public bool IsVFreeClimbOnAction => vmisFreeClimb != null && vmisFreeClimb.IsOnAction;
#endif

#if MIS_INVECTOR_PARACHUTE
        [HideInInspector] public mvParachuteController vmisParachute;
        public bool IsVParachuteOnAction => vmisParachute != null && vmisParachute.usingParachute;
#endif

#if MIS_INVECTOR_PUSH
        [HideInInspector] public mvPushActionController vmisPush;
        public bool IsVPushAvailable => vmisPush != null;
        public bool IsVPushOnAction => IsVPushAvailable && vmisPush.IsOnAction;
#endif

#if MIS_INVECTOR_SHOOTERCOVER
        [HideInInspector] public mvCoverController vmisShooterCover;
        public bool IsVShooterCoverOnAction => vmisShooterCover != null && (vmisShooterCover.inCover || vmisShooterCover.goingToCoverPoint);
#endif

#if MIS_INVECTOR_ZIPLINE
        [HideInInspector] public mvZipLine vmisZipLine;
        public bool IsVZiplineOnAction => vmisZipLine != null && vmisZipLine.IsOnAction;
#endif

#if MIS_CRAWLING
        public override bool isCrouching
        {
            get
            {
                return _isCrouching;
            }
            set
            {
                if (value != _isCrouching)
                {
                    if (value)
                    {
                        OnCrouch.Invoke();
                    }
                    else
                    {
                        if (!IsCrawlingOnAction)
                            OnStandUp.Invoke();
                    }
                }
                _isCrouching = value;
            }
        }
#endif


        protected override void Start()
        {
            base.Start();


            // ----------------------------------------------------------------------------------------------------
            // MIS Packages
            // ----------------------------------------------------------------------------------------------------
#if MIS_AIRDASH
            TryGetComponent(out misAirDash);
#endif
#if MIS_CARRIDER_EVP || MIS_CARRIDER_RCC || MIS_HELICOPTER || MIS_ROWINGBOAT
            TryGetComponent(out misVehicleRider);
#endif
#if MIS_CRAWLING
            TryGetComponent(out misCrawling);
#endif
#if MIS_GROUNDDASH
            TryGetComponent(out misGroundDash);
#endif
#if MIS_FREEFLYING
            TryGetComponent(out misFreeFlying);
#endif
#if MIS_GRAPPLINGHOOK
            TryGetComponent(out misGrapplingHook);
#endif
#if MIS_GRAPPLINGROPE
            TryGetComponent(out misGrapplingRope);
#endif
#if MIS_LANDINGROLL
            TryGetComponent(out misLandingRoll);
#endif
#if MIS_LEDGECLIMB1
            TryGetComponent(out misLedgeClimb1);
#endif
#if MIS_LEDGECLIMB2
            TryGetComponent(out misLedgeClimb2);
#endif
#if MIS_LOCKON || MIS_LOCKON2
            TryGetComponent(out misLockOn);
#endif
#if MIS_MAGICSPELL
            TryGetComponent(out misMagicSpell);
#endif
#if MIS_MOTORCYCLE
            TryGetComponent(out misRider);
#endif
#if MIS_MULTIJUMP
            TryGetComponent(out misMultiJump);
#endif
#if MIS_SOFTFLYING
            TryGetComponent(out misSoftFlying);
#endif
#if MIS_SWIMMING
            TryGetComponent(out misSwimming);
#endif
#if MIS_WATERDASH
            TryGetComponent(out misWaterDash);
#endif
#if MIS_WALLRUN
            TryGetComponent(out misWallRun);
#endif


            // ----------------------------------------------------------------------------------------------------
            // INVECTOR Addons
            // ----------------------------------------------------------------------------------------------------
#if MIS_INVECTOR_BUILDER
            vmisBuildManager = GetComponentInChildren<vBuildManager>();
#endif
#if MIS_INVECTOR_FREECLIMB
            TryGetComponent(out vmisFreeClimb);
#endif
#if MIS_INVECTOR_PARACHUTE
            vmisParachute = GetComponentInChildren<mvParachuteController>();
#endif
#if MIS_INVECTOR_PUSH
            TryGetComponent(out vmisPush);
#endif
#if MIS_INVECTOR_SHOOTERCOVER
            TryGetComponent(out vmisShooterCover);
#endif
#if MIS_INVECTOR_ZIPLINE
            TryGetComponent(out vmisZipLine);
#endif


#if UNITY_EDITOR
            //MISVerification();
#endif
        }

        #region Health & Stamina

#if MIS_AIRDASH || MIS_GROUNDDASH || MIS_FREEFLYING || MIS_GRAPPLINGROPE || MIS_GRAPPLINGHOOK || MIS_WATERDASH
        public override void TakeDamage(vDamage damage)
        {
            // don't apply damage if the character is rolling, you can add more conditions here
            if (currentHealth <= 0 || (IgnoreDamageRolling())
#if MIS_LANDINGROLL
                || IgnoreDamageLandingRoll()
#endif
#if MIS_AIRDASH
                || IgnoreDamageAirDash()
#endif
#if MIS_GROUNDDASH
                || IgnoreDamageGroundDash()
#endif
#if MIS_FREEFLYING
                || IgnoreDamageFreeFlying() || IgnoreDamageFreeFlyingEscape() || IgnoreDamageFreeFlyingSprintRoll()
#endif
#if MIS_GRAPPLINGROPE
                || IgnoreDamageGrapplingRope()
#endif
#if MIS_GRAPPLINGHOOK
                || IgnoreDamageGrapplingHook()
#endif
#if MIS_WATERDASH
                || IgnoreDamageWaterDash()
#endif
                )
            {
                if (damage.activeRagdoll && (!IgnoreDamageActiveRagdollRolling()
#if MIS_LANDINGROLL
                    || !IgnoreDamageActiveRagdollLandingRoll()
#endif
#if MIS_AIRDASH
                    || !IgnoreDamageActiveRagdollAirDash()
#endif
#if MIS_GROUNDDASH
                    || !IgnoreDamageActiveRagdollGroundDash()
#endif
#if MIS_FREEFLYING
                    || !IgnoreDamageActiveRagdollFreeFlying() || !IgnoreDamageActiveRagdollFreeFlyingEscape() || !IgnoreDamageActiveRagdollFreeFlyingSprintRoll()
#endif
#if MIS_GRAPPLINGROPE
                    || !IgnoreDamageActiveRagdollGrapplingRope()
#endif
#if MIS_GRAPPLINGHOOK
                    || !IgnoreDamageActiveRagdollGrapplingHook()
#endif
#if MIS_WATERDASH
                    || !IgnoreDamageActiveRagdollWaterDash()
#endif
                    ))
                {
                    onActiveRagdoll.Invoke(damage);
                }

                return;
            }

            if (damage.activeRagdoll && (IgnoreDamageActiveRagdollRolling()
#if MIS_LANDINGROLL
                || IgnoreDamageActiveRagdollLandingRoll()
#endif
#if MIS_AIRDASH
                || IgnoreDamageActiveRagdollAirDash()
#endif
#if MIS_GROUNDDASH
                || IgnoreDamageActiveRagdollGroundDash()
#endif
#if MIS_FREEFLYING
                || IgnoreDamageActiveRagdollFreeFlying() || IgnoreDamageActiveRagdollFreeFlyingEscape() || IgnoreDamageActiveRagdollFreeFlyingSprintRoll()
#endif
#if MIS_GRAPPLINGROPE
                || IgnoreDamageActiveRagdollGrapplingRope()
#endif
#if MIS_GRAPPLINGHOOK
                || IgnoreDamageActiveRagdollGrapplingHook()
#endif
#if MIS_WATERDASH
                || IgnoreDamageActiveRagdollWaterDash()
#endif
                ))
            {
                damage.activeRagdoll = false;
            }

            base.TakeDamage(damage);
        }
#endif

#if MIS_LANDINGROLL
        protected virtual bool IgnoreDamageLandingRoll()
        {
            return IsLandingRollOnAction && misLandingRoll.noDamageWhileAction == true;
        }

        protected virtual bool IgnoreDamageActiveRagdollLandingRoll()
        {
            return IsLandingRollOnAction && misLandingRoll.noActiveRagdollWhileAction == true;
        }
#endif

#if MIS_AIRDASH
        protected virtual bool IgnoreDamageAirDash()
        {
            return misAirDash != null && misAirDash.noDamageWhileAction == true && misAirDash.IsOnAction == true;
        }

        protected virtual bool IgnoreDamageActiveRagdollAirDash()
        {
            return misAirDash != null && misAirDash.noActiveRagdollWhileAction == true && misAirDash.IsOnAction == true;
        }
#endif

#if MIS_GROUNDDASH
        protected virtual bool IgnoreDamageGroundDash()
        {
            return IsGroundDashOnAction && misGroundDash.noDamageWhileAction;
        }

        protected virtual bool IgnoreDamageActiveRagdollGroundDash()
        {
            return IsGroundDashOnAction && misGroundDash.noActiveRagdollWhileAction;
        }
#endif

#if MIS_FREEFLYING
        protected virtual bool IgnoreDamageFreeFlying()
        {
            return IsFreeFlyingOnAction && misFreeFlying.noDamageOnAction == true;
        }
        protected virtual bool IgnoreDamageActiveRagdollFreeFlying()
        {
            return IsFreeFlyingOnAction && misFreeFlying.noRagdollOnAction == true;
        }

        protected virtual bool IgnoreDamageFreeFlyingEscape()
        {
            return IsFreeFlyingOnAction && misFreeFlying.IsFlyingEscape && misFreeFlying.noDamageOnEscape == true;
        }
        protected virtual bool IgnoreDamageActiveRagdollFreeFlyingEscape()
        {
            return IsFreeFlyingOnAction && misFreeFlying.IsFlyingEscape && misFreeFlying.noRagdolOnEscape == true;
        }

        protected virtual bool IgnoreDamageFreeFlyingSprintRoll()
        {
            return IsFreeFlyingOnAction && misFreeFlying.IsFlyingSprintRoll && misFreeFlying.noDamageOnSprintRoll == true;
        }
        protected virtual bool IgnoreDamageActiveRagdollFreeFlyingSprintRoll()
        {
            return IsFreeFlyingOnAction && misFreeFlying.IsFlyingSprintRoll && misFreeFlying.noRagdollOnSprintRoll == true;
        }
#endif

#if MIS_GRAPPLINGROPE
        protected virtual bool IgnoreDamageGrapplingRope()
        {
            return IsGrapplingRopeOnMoveAction && misGrapplingRope.noDamageOnAction == true;
        }

        protected virtual bool IgnoreDamageActiveRagdollGrapplingRope()
        {
            return IsGrapplingRopeOnMoveAction && misGrapplingRope.noRagdollOnAction == true;
        }
#endif

#if MIS_GRAPPLINGHOOK
        protected virtual bool IgnoreDamageGrapplingHook()
        {
            return IsGrapplingHookOnMoveAction && misGrapplingHook.noDamageOnAction == true;
        }

        protected virtual bool IgnoreDamageActiveRagdollGrapplingHook()
        {
            return IsGrapplingHookOnMoveAction && misGrapplingHook.noActiveRagdollWhileAction == true;
        }
#endif

#if MIS_WATERDASH
        protected virtual bool IgnoreDamageWaterDash()
        {
            return IsWaterDashOnAction && misWaterDash.noDamageOnAction == true;
        }

        protected virtual bool IgnoreDamageActiveRagdollWaterDash()
        {
            return IsWaterDashOnAction && misWaterDash.noActiveRagdollWhileAction == true;
        }
#endif

        public virtual void ResetStamina()
        {
            currentStamina = maxStamina;
        }

        #endregion

        #region Locomotion

        public virtual void RotateToDirection(Vector3 direction, bool immediate)
        {
            if (/*lockAnimRotation || */customAction || (!jumpAndRotate && !isGrounded) || ragdolled || isSliding)
                return;

            direction.y = 0f;
            if (direction.normalized.magnitude == 0)
                direction = transform.forward;

            var euler = transform.rotation.eulerAngles.NormalizeAngle();
            euler.y = Quaternion.LookRotation(direction.normalized).eulerAngles.NormalizeAngle().y;
            transform.rotation = Quaternion.Euler(euler);
        }

        #endregion

        #region Jump Methods

        public override void ApplyAirMovement()
        {
#if MIS_AIRDASH
            if (IsAirDashOnAction)
            {
                return;
            }
            else
#endif
#if MIS_SWIMMING
            if (IsSwimOnAction)
            {
                return;
            }
#endif
#if MIS_LANDINGROLL
            if (IsLandingRollAvailable && !isGrounded && verticalVelocity <= fallMinVerticalVelocity && groundDistance > groundMinDistance && groundDistance < misLandingRoll.maxActionGroundDistance)
            {
                misLandingRoll.StartAction();
            }
            else
#endif
                base.ApplyAirMovement();

#if MIS_FREEFLYING
            if (IsFreeFlyingOnLanding)
                heightReached = transform.position.y;
#endif
        }

        public override bool MoveCharacterConditions()
        {
            return
                base.MoveCharacterConditions()
#if MIS_AIRDASH
                && !IsAirDashOnAction
#endif
#if MIS_CRAWLING
                && !IsCrawlingOnAction
#endif
#if MIS_FREEFLYING
                && !IsFreeFlyingOnAction
#endif
#if MIS_GROUNDDASH
                && !IsGroundDashOnAction
#endif
#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
                && !IsLedgeClimbOnAction
#endif
#if MIS_SWIMMING
                && !IsSwimOnAction
#endif
#if MIS_WALLRUN
                && !IsWallRunOnAction
#endif
                ;
        }

        public override bool ApplyMovementConditions()
        {
            return 
                base.ApplyMovementConditions()
#if MIS_AIRDASH
                && !IsAirDashOnAction
#endif
#if MIS_CRAWLING
                && !IsCrawlingOnAction
#endif
#if MIS_FREEFLYING
                && !IsFreeFlyingOnAction
#endif
#if MIS_GROUNDDASH
                && !IsGroundDashOnAction
#endif
#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
                && !IsLedgeClimbOnAction
#endif
#if MIS_SWIMMING
                && !IsSwimOnAction
#endif
#if MIS_WALLRUN
                && !IsWallRunOnAction
#endif
                ;
        }

        public bool JumpFwdCondition
        {
            get
            {
                return jumpFwdCondition;
            }
        }
        #endregion

        #region Crouch Methods

        public virtual bool CanExitCrouch(bool checkByForce)
        {
            if (isCrouching && !checkByForce)
                return true;

            float radius = _capsuleCollider.radius * 0.9f;
            Vector3 pos = transform.position + Vector3.up * ((colliderHeight * 0.5f) - colliderRadius);
            Ray ray2 = new Ray(pos, Vector3.up);

            if (Physics.SphereCast(ray2, radius, out groundHit, crouchHeadDetect - (colliderRadius * 0.1f), autoCrouchLayer))
                return false;
            else
                return true;
        }

        #endregion

        #region Ground Check                
        protected override void CheckGround()
        {
#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
            if (IsLedgeClimbOnAction)
                return;
#endif
#if MIS_SOFTFLYING
            // When isDead is enabled, Invector controller cancels the ground checking.
            // It prevents a character to fall to the ground.
            if (isDead && IsSoftFlyingOnAction)
                return;
#endif
#if MIS_SWIMMING
            // When isDead is enabled, Invector controller cancels the ground checking.
            // It prevents a character to fall to the ground.
            if (IsSwimOnAction)
                return;
#endif

            base.CheckGround();

#if MIS_FREEFLYING
            if (IsFreeFlyingOnLanding)
                verticalVelocity = 0f;
#endif
        }

        public override bool FallDamageConditions()
        {
            return
                base.FallDamageConditions()
#if MIS_SOFTFLYING
                && !IsSoftFlyingOnAction
#endif
                ;
        }

        internal virtual void MISCheckFallDamage(float fallMinHeight, float fallMinVerticalVelocity)
        {
            if (/*isGrounded || */verticalVelocity > fallMinVerticalVelocity || customAction/*!_canApplyFallDamage*/ || fallMinHeight == 0 || fallDamage == 0)
                return;

            float fallHeight = heightReached - transform.position.y;
            fallHeight -= fallMinHeight;

            if (fallHeight > 0)
                TakeDamage(new vDamage((int)(fallDamage * fallHeight), true));
        }

        #endregion

        #region Colliders Check

        public override void ControlCapsuleHeight()
        {
#if MIS_AIRDASH
            if (IsAirDashOnAction)
                return;
#endif
#if MIS_GROUNDDASH
            if (IsGroundDashOnAction)
                return;
#endif
#if MIS_CRAWLING
            if (IsCrawlingOnAction)
                return;
#endif
#if MIS_FREEFLYING
            if (IsFreeFlyingOnAction)
                return;
#endif
#if MIS_MOTORCYCLE
            if (IsRiderOnAction)
                return;
#endif
#if MIS_CARRIDER_EVP || MIS_CARRIDER_RCC || MIS_HELICOPTER || MIS_ROWINGBOAT
            if (IsVehicleRiderOnAction)
                return;
#endif
#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
            if (IsLedgeClimbOnAction)
                return;
#endif
#if MIS_SOFTFLYING
            if (IsSoftFlyingOnAction)
                return;
#endif
#if MIS_SWIMMING
            if (IsSwimOnAction)
                return;
#endif
#if MIS_WATERDASH
            if (IsWaterDashOnAction)
                return;
#endif
#if MIS_WALLRUN
            if (IsWallRunOnAction)
                return;
#endif

            base.ControlCapsuleHeight();
        }

        #endregion
    }

    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public static class mvThirdPersonMotorExtension
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static vThirdPersonMotor.vMovementSpeed DeepCopyMovementSpeed(vThirdPersonMotor.vMovementSpeed source)
        {
            vThirdPersonMotor.vMovementSpeed copy = new vThirdPersonMotor.vMovementSpeed
            {
                movementSmooth = source.movementSmooth,
                animationSmooth = source.animationSmooth,
                rotationSpeed = source.rotationSpeed,
                walkByDefault = source.walkByDefault,
                rotateWithCamera = source.rotateWithCamera,
                walkSpeed = source.walkSpeed,
                runningSpeed = source.runningSpeed,
                sprintSpeed = source.sprintSpeed,
                crouchSpeed = source.crouchSpeed
            };

            return copy;
        }
    }
}
