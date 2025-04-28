#if INVECTOR_SHOOTER
using Invector;
using Invector.vCamera;
using Invector.vCharacterController;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // Instead of vShooterMeleeInput class, mvShooterMeleeInput class will be mainly used for MIS and MIS Packages.
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("SHOOTER/MELEE INPUT", iconName = "misIconRed")]
    public partial class mvShooterMeleeInput : vShooterMeleeInput
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        public bool LockShooterInput
        {
            get => lockShooterInput;
        }

#if MIS_CRAWLING
        [Tooltip("Make fast to shoot for alignling aiming angle")]
        public float crawlingMaxAimAngle = 45f;
#endif

        public override void InputHandle()
        {
            if (cc == null || cc.isDead)
            {
                return;
            }

            if (!cc.ragdolled && !lockInput
#if MIS_CRAWLING
                // Cannot move while aiming because it doesn't make sense.
                && !(cc.IsCrawlingOnAction && IsAiming)
#endif
                )
            {
                MoveInput();
                SprintInput();
                CrouchInput();
                StrafeInput();
                JumpInput();
                RollInput();
            }

            if (MeleeAttackConditions() && !IsAiming && !isReloading && !lockMeleeInput && !CurrentActiveWeapon)
            {
                if (shooterManager.canUseMeleeWeakAttack_H || shooterManager.CurrentWeapon == null)
                {
                    MeleeWeakAttackInput();
                }

                if (shooterManager.canUseMeleeStrongAttack_H || shooterManager.CurrentWeapon == null)
                {
                    MeleeStrongAttackInput();
                }

                if (shooterManager.canUseMeleeBlock_H || shooterManager.CurrentWeapon == null)
                {
                    BlockingInput();
                }
                else
                {
                    isBlocking = false;
                }
            }

            if (lockShooterInput)
            {
                isAimingByInput = false;
                _aimTiming = 0;
                if (controlAimCanvas != null)
                {
                    if (controlAimCanvas.isAimActive || controlAimCanvas.isScopeCameraActive)
                    {
                        isUsingScopeView = false;
                        controlAimCanvas.DisableAim();
                    }
                }
            }
            else if (shooterManager.CurrentWeapon)
            {
                if (MeleeAttackConditions() && (!IsAiming || shooterManager.canUseMeleeAiming))
                {
                    if (shooterManager.canUseMeleeWeakAttack_E)
                    {
                        MeleeWeakAttackInput();
                    }
                    if (shooterManager.canUseMeleeStrongAttack_E)
                    {
                        MeleeStrongAttackInput();
                    }
                    if (shooterManager.canUseMeleeBlock_E)
                    {
                        BlockingInput();
                    }
                    else
                    {
                        isBlocking = false;
                    }
                }
                else
                {
                    isBlocking = false;
                }

                if (shooterManager == null || CurrentActiveWeapon == null || isEquipping)
                {
                    if (_walkingByDefaultWasChanged)
                    {
                        _walkingByDefaultWasChanged = false;
                        ResetWalkByDefault();
                    }
                    if (IsAiming)
                    {
                        isAimingByInput = false;
                        _aimTiming = 0;
                        if (!cc.lockInStrafe && cc.isStrafing)
                        {
                            cc.Strafe();
                        }

                        if (controlAimCanvas != null)
                        {
                            if (controlAimCanvas.isAimActive || controlAimCanvas.isScopeCameraActive)
                            {
                                isUsingScopeView = false;
                                controlAimCanvas.DisableAim();
                            }
                        }
                        if (shooterManager && shooterManager.CurrentWeapon && shooterManager.CurrentWeapon.chargeWeapon && shooterManager.CurrentWeapon.powerCharge != 0)
                        {
                            CurrentActiveWeapon.powerCharge = 0;
                        }

                        shootCountA = 0;
                    }
                }
                else
                {
                    AimInput();

#if MIS_CRAWLING
                    // Cannot crawl while aiming because it doesn't make sense.
                    if (cc.IsCrawlingOnAction)
                    {
                        if (IsAiming)
                        {
                            cc.input = Vector3.zero;

                            // It should be 0 to move immediately after aiming.
                            // Because of the below single line of codes, SetCustomIKAdjustState() cannot be called in mvCrawlingShooter
                            _aimTiming = 0f;

                            SetCustomIKAdjustState("CrawlingAiming");
                        }
                        else
                        {
                            SetCustomIKAdjustState("Crawling");
                        }
                    }
#endif

#if MIS_FREEFLYING
                    if (cc.IsFreeFlyingOnAction)
                    {
                        if (IsAiming)
                        {
                            // It should be 0 to move immediately after aiming.
                            _aimTiming = 0f;

                            SetCustomIKAdjustState("FlyingAiming");
                        }
                        else
                        {
                            SetCustomIKAdjustState("Flying");
                        }
                    }
#endif

                    ShotInput();
                    ReloadInput();
                    SwitchCameraSideInput();
                    ScopeViewInput();
                }
            }
            else
            {
                isAimingByInput = false;
                _aimTiming = 0;
                if (!cc.lockInStrafe && cc.isStrafing && cc.locomotionType != vThirdPersonMotor.LocomotionType.OnlyStrafe)
                {
                    cc.Strafe();
                }
                if (_walkingByDefaultWasChanged && !IsAiming)
                {
                    _walkingByDefaultWasChanged = false;
                    ResetWalkByDefault();
                }
                if (controlAimCanvas != null)
                {
                    if (controlAimCanvas.isAimActive || controlAimCanvas.isScopeCameraActive)
                    {
                        isUsingScopeView = false;
                        controlAimCanvas.DisableAim();
                    }
                }
            }
        }

        public override void UpdateCameraStates()
        {
            // CAMERA STATE - you can change the CameraState here, the bool means if you want lerp of not, make sure to use the same CameraState String that you named on TPCameraListData
            if (ignoreTpCamera)
            {
                return;
            }

            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                {
                    return;
                }

                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }

            if (changeCameraState)
            {
                tpCamera.ChangeState(customCameraState, customlookAtPoint, true);
            }
            else if (cc.isCrouching && !isAimingByInput)
            {
#if MIS_MAGICSPELL
                if (cc.IsMagicSpellOnAction && !string.IsNullOrEmpty(cc.misMagicSpell.magicCrouchCameraState))
                    tpCamera.ChangeState(cc.misMagicSpell.magicCrouchCameraState, true);
                else
#endif
                tpCamera.ChangeState("Crouch", true);
            }
            else if (cc.isStrafing && !isAimingByInput)
            {
#if MIS_MAGICSPELL
                if (cc.IsMagicSpellOnAction && !string.IsNullOrEmpty(cc.misMagicSpell.magicStrafeCameraState))
                    tpCamera.ChangeState(cc.misMagicSpell.magicStrafeCameraState, true);
                else
#endif
                    tpCamera.ChangeState("Strafing", true);
            }
            else if (isAimingByInput && CurrentActiveWeapon)
            {
                if (isUsingScopeView)
                {
                    if (string.IsNullOrEmpty(CurrentActiveWeapon.customScopeCameraState))
                    {
                        tpCamera.ChangeState(cc.isCrouching ? "CrouchingAiming" : "Aiming", true);
                    }
                    else
                    {
                        tpCamera.ChangeState(CurrentActiveWeapon.customScopeCameraState, true);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(CurrentActiveWeapon.customAimCameraState))
                    {
#if MIS_WATERDASH
                        if (cc.IsWaterDashOnAction)
                            tpCamera.ChangeState(cc.misWaterDash.aimingCameraState, true);
                        else
#endif
                            tpCamera.ChangeState(cc.isCrouching ? "CrouchingAiming" : "Aiming", true);
                    }
                    else
                    {
                        tpCamera.ChangeState(CurrentActiveWeapon.customAimCameraState, true);
                    }
                }
            }
            else
            {
                tpCamera.ChangeState("Default", true);
            }
        }
    }
}
#endif