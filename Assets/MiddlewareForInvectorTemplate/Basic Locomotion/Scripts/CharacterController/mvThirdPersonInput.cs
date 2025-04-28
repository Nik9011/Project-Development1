#if INVECTOR_BASIC
using Invector;
using Invector.vCharacterController;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // Instead of vThirdPersonInput class, mvThirdPersonInput class will be mainly used for MIS and MIS Packages.
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("Input Manager", iconName = "misIconRed")]
    public partial class mvThirdPersonInput : vThirdPersonInput
    {
        // ----------------------------------------------------------------------------------------------------
        // Reference cc in Awake so that cameraMain can be accessed in Start() of other components.
        // ----------------------------------------------------------------------------------------------------
        protected virtual void Awake()
        {
            cc = GetComponent<mvThirdPersonController>();

            if (cc != null)
                cc.Init();
        }

#if MIS_FREEFLYING || MIS_MOTORCYCLE
        public override void SetLockBasicInput(bool value)
        {
#if MIS_FREEFLYING
            if (cc.IsFreeFlyingOnAction)
                return;
#endif
#if MIS_MOTORCYCLE
            if (cc.IsRiderOnAction)
                return;
#endif

            base.SetLockBasicInput(value);
        }
#endif

        public override void JumpInput()
        {
#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
            if (cc.IsLedgeClimbOnAction)
                return;
#endif

#if MIS_WALLRUN
            if (jumpInput.GetButtonDown() && cc.IsWallRunAvailable)
            {
                if (cc.misWallRun.JumpInput())
                    return;
            }
#endif

#if MIS_MULTIJUMP
            if (cc.misMultiJump == null)
                base.JumpInput();
#else
            base.JumpInput();
#endif
        }

#if MIS_CRAWLING
        public override void CrouchInput()
        {
            if (cc.IsCrawlingOnAction)
                return;

            base.CrouchInput();
        }
#endif

        public override bool RollConditions()
        {
            return (!cc.isRolling || cc.canRollAgain)
                && cc.isGrounded
                && cc.input != Vector3.zero
                && !cc.customAction
                && cc.currentStamina > cc.rollStamina
                && !cc.isJumping
	            && !cc.isSliding
#if MIS_AIRDASH
                && !cc.IsAirDashOnAction
#endif
#if MIS_GROUNDDASH
                && !cc.IsGroundDashOnAction
#endif
                ;
        }
    }
}
#endif