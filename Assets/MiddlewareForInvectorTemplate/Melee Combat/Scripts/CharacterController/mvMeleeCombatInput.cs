#if INVECTOR_MELEE
using Invector;
using Invector.vCharacterController;
using Invector.vMelee;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // Instead of vMeleeCombatInput class, mvMeleeCombatInput class will be mainly used for MIS and MIS Packages.
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("MELEE INPUT MANAGER", iconName = "misIconRed")]
    public partial class mvMeleeCombatInput : vMeleeCombatInput
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        public bool LockMeleeInput
        {
            get => lockMeleeInput;
        }

        public override void InputHandle()
        {
            if (cc == null || cc.isDead)
            {
                return;
            }

            //base.InputHandle();
            if (!cc.ragdolled && !lockInput)
            {
                MoveInput();
                SprintInput();
                CrouchInput();
                StrafeInput();
                JumpInput();
                RollInput();
            }

            if (MeleeAttackConditions() && !lockMeleeInput)
            {
                MeleeWeakAttackInput();
                MeleeStrongAttackInput();
                BlockingInput();
            }
            else
            {
                ResetAttackTriggers();
                isBlocking = false;
            }
        }

        protected override bool MeleeAttackConditions()
        {
            if (meleeManager == null)
            {
                meleeManager = GetComponent<vMeleeManager>();
            }

#if MIS_SWIMMING
            return 
                meleeManager != null
                && cc.IsSwimOnAction ? true : cc.isGrounded
                && !cc.customAction 
                && !cc.isJumping 
                && !cc.isCrouching 
                && !cc.isRolling 
                && !isEquipping 
                && !cc.animator.IsInTransition(cc.baseLayer);
#else
            return base.MeleeAttackConditions();
#endif
        }
    }
}
#endif