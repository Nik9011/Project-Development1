#if MIS_FSM_AI && INVECTOR_AI_TEMPLATE
using Invector;
using Invector.vCharacterController.AI;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("mvControlAIShooter", iconName = "misIconRed")]
    public class mvControlAIShooter : vControlAIShooter
    {
        protected float oldMinTimeShooting;
        protected float oldMaxTimeShooting;
        protected float oldMinShotWaiting;
        protected float oldMaxShotWaiting;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void UpdateShooterAnimator()
        {
            if (shooterManager.CurrentWeapon)
            {
                IsReloading = IsAnimatorTag("IsReloading");

                // find states with the IsEquipping tag
                IsEquipping = IsAnimatorTag("IsEquipping");
                var _isAiming = IsAiming && !IsReloading;
                if (_isAiming && !aimEnable)
                {
                    if (armAlignmentWeight > 0.5f && IsAnimatorTag("Upperbody Pose") && animator.IsInTransition(upperBodyLayer) == false && animatorStateInfos.GetCurrentNormalizedTime(upperBodyLayer) > 0.5f)
                    {
                        shooterManager.CurrentWeapon.onEnableAim.Invoke();
                        aimEnable = true;
                    }
                }
                else if (!_isAiming && aimEnable)
                {
                    shooterManager.CurrentWeapon.onDisableAim.Invoke();
                    aimEnable = false;
                }
                animator.SetBool("CanAim", _isAiming && _canAiming);

                ShotID = shooterManager.GetShotID();
                UpperBodyID = shooterManager.GetUpperBodyID();
                MoveSetID = shooterManager.GetMoveSetID();
                animator.SetBool("IsAiming", _isAiming);
            }
            else
            {
                IsReloading = false;
                animator.SetBool("IsAiming", false);
                animator.SetBool("CanAim", false);
                if (aimEnable)
                {
                    if (shooterManager.CurrentWeapon)
                        shooterManager.CurrentWeapon.onDisableAim.Invoke();
                    aimEnable = false;
                }
            }
            _onlyArmsLayerWeight = Mathf.Lerp(_onlyArmsLayerWeight, isAiming || isRolling ? 0f : shooterManager && shooterManager.CurrentWeapon ? 1f : 0f, 6f * Time.deltaTime);
            animator.SetLayerWeight(onlyArmsLayer, _onlyArmsLayerWeight);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void CheckCanAiming()
        {
            if (ragdolled || (!isStrafing && !lockAimDebug) || customAction || IsReloading)
            {
                _canAiming = false;
                return;
            }

            var p1 = aimTarget;
            p1.y = transform.position.y;
            var angle = Vector3.Angle(transform.forward, p1 - transform.position);

            var outAngle = (angle > aimTurnAngle);
            _canAiming = true;

#if MIS_AI_CARRIDER_EVP || MIS_AI_CARRIDER_RCC
            if (IsOnActionAIVehicleRider)
            {
                if (outAngle)
                    RotateTo(misAIVehicleRider.vehicleAssistant.tr.forward);
            }
            else
#endif
            {
                if (outAngle && IsAiming)
                    RotateTo(aimTarget - transform.position);
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void ChangeShootingFrequency(float minTimeShooting, float maxTimeShooting, float minShotWaiting, float maxShotWaiting)
        {
            oldMinTimeShooting = this.minTimeShooting;
            oldMaxTimeShooting = this.minTimeShooting;
            oldMinShotWaiting = this.minTimeShooting;
            oldMaxShotWaiting = this.minTimeShooting;

            this.minTimeShooting = minTimeShooting;
            this.maxTimeShooting = maxTimeShooting;
            this.minShotWaiting = minShotWaiting;
            this.maxShotWaiting = maxShotWaiting;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void RestoreShootingFrequency()
        {
            this.minTimeShooting = oldMinTimeShooting;
            this.maxTimeShooting = oldMaxTimeShooting;
            this.minShotWaiting = oldMinShotWaiting;
            this.maxShotWaiting = oldMaxShotWaiting;
        }
    }
}
#endif