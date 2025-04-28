using Invector;
using System.Collections;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("GrapplingRope Melee", iconName = "misIconRed")]
    public partial class mvGrapplingRopeMelee : mvGrapplingRopeBasic
    {
#if MIS_GRAPPLINGROPE && INVECTOR_MELEE
        protected mvMeleeCombatInput meleeCombatInput;
        protected vDrawHideMeleeWeapons drawHideWeapons;


#if MIS_INVECTOR_FREECLIMB
        // ----------------------------------------------------------------------------------------------------
        // 
        protected int isFreeClimbHash = Animator.StringToHash("IsFreeClimb");
        protected bool IsFreeClimb
        {
            get => cc.animator.GetBool(isFreeClimbHash);
        }
#endif


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override IEnumerator Start()
        {
            yield return StartCoroutine(base.Start());

            TryGetComponent(out drawHideWeapons);

            if (IsAvailable)
                meleeCombatInput = tpInput as mvMeleeCombatInput;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void SetAimingMode(bool enable, bool isCancled = false)
        {
            base.SetAimingMode(enable, isCancled);

            meleeCombatInput.SetLockMeleeInput(enable);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void Interrupt()
        {
            base.Interrupt();

#if MIS_INVECTOR_FREECLIMB
            if (allowFromFreeClimb && IsFreeClimb)
                drawHideWeapons.ForceHideWeapons(true);
#endif
        }

#endif
    }
}
