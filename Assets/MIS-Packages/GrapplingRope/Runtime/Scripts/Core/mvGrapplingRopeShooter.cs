using Invector;
using System.Collections;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("GrapplingRope Shooter", iconName = "misIconRed")]
    public partial class mvGrapplingRopeShooter : mvGrapplingRopeMelee
    {
#if MIS_GRAPPLINGROPE && INVECTOR_SHOOTER
        protected mvShooterMeleeInput shooterMeleeInput;
        protected mvDrawHideShooterWeapons drawHideShooterWeapons;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override IEnumerator Start()
        {
            yield return StartCoroutine(base.Start());

            TryGetComponent(out drawHideShooterWeapons);

            if (IsAvailable)
                shooterMeleeInput = tpInput as mvShooterMeleeInput;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void SetAimingMode(bool enable, bool isCancled = false)
        {
            base.SetAimingMode(enable, isCancled);

            shooterMeleeInput.SetLockShooterInput(enable);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void Interrupt()
        {
            base.Interrupt();

#if MIS_INVECTOR_FREECLIMB
            if (allowFromFreeClimb && IsFreeClimb)
                drawHideShooterWeapons.ForceHideWeapons(true);
#endif
        }
#endif
    }
}
