#if INVECTOR_SHOOTER
using Invector;
using Invector.vShooter;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("Draw/Hide Shooter Melee Weapons", "This component works with vItemManager, vWeaponHolderManager and vShooterMeleeInput", useHelpBox = true, iconName = "misIconRed")]
    public partial class mvDrawHideShooterWeapons : vDrawHideShooterWeapons
    {
        protected override bool DrawShooterWeaponImmediateConditions()
        {
            if (!shooter || !shooter.shooterManager || shooter.cc.customAction || !shooter.shooterManager.CurrentWeapon || 
                (
#if MIS_CARRIDER_EVP || MIS_CARRIDER_RCC || MIS_HELICOPTER || MIS_ROWINGBOAT
                !shooter.cc.IsVehicleRiderOnAction &&
#endif
#if MIS_FREEFLYING
                !shooter.cc.IsFreeFlyingOnAction &&
#endif
                shooter.lockInput
                ))
            {
                return false;
            }

            if (shooter.CurrentActiveWeapon == null && ((shooter.aimInput.GetButtonDown() && aim) ||
                (shooter.shooterManager.hipfireShot && shooter.shotInput.GetButtonDown() && hipFire) || (shooter.shotInput.GetButtonDown() && shoot)))
            {
                return true;
            }

            return false;
        }
    }
}
#endif
