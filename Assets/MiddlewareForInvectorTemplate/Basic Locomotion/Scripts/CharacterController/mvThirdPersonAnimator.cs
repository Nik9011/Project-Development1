using Invector.vCharacterController;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // Instead of vThirdPersonAnimator class, mvThirdPersonAnimator class will be mainly used for MIS and MIS Packages.
    // ----------------------------------------------------------------------------------------------------
    public partial class mvThirdPersonAnimator : vThirdPersonAnimator
    {
        public override void ResetInputAnimatorParameters()
        {
            // [MIS] To use Animator Parameter hash in order to improve performance
            animator.SetBool(vAnimatorParameters.IsSprinting, false);
            animator.SetBool(vAnimatorParameters.IsSliding, false);
            animator.SetBool(vAnimatorParameters.IsCrouching, false);
            animator.SetBool(vAnimatorParameters.IsGrounded, true);
            animator.SetFloat(vAnimatorParameters.GroundDistance, 0f);
            animator.SetFloat(vAnimatorParameters.InputHorizontal, 0f);
            animator.SetFloat(vAnimatorParameters.InputVertical, 0f);
            animator.SetFloat(vAnimatorParameters.InputMagnitude, 0f);
        }

        protected override void DeadAnimation()
        {
            if (!isDead)
                return;

#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
            if (IsLedgeClimbOnAction)
            {
#if MIS_LEDGECLIMB2
                if (misLedgeClimb2.useDeadRagdoll)
                    return;
#else
                if (misLedgeClimb1.useDeadRagdoll)
                    return;
#endif
            }
#endif

            base.DeadAnimation();
        }
    }

    public static partial class mvAnimatorParameters
    {
        public static int AnimationSpeed = Animator.StringToHash("AnimationSpeed");
        public static int HorizontalInput = Animator.StringToHash("HorizontalInput");
        public static int VerticalInput = Animator.StringToHash("VerticalInput");
        public static int MoveSet_ID = Animator.StringToHash("MoveSet_ID");
        public static int HorizontalVelocity = Animator.StringToHash("HorizontalVelocity");
        public static int IgnoreFalling = Animator.StringToHash("IgnoreFalling");
        public static int IgnoreGravity = Animator.StringToHash("IgnoreGravity");
        public static int FlipAnimation = Animator.StringToHash("FlipAnimation");
    }
}
