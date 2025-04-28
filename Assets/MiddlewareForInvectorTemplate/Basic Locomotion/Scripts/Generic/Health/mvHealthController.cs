using Invector;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // Instead of vHealthController class, mvHealthController class will be mainly used for MIS and MIS Packages.
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("HealthController", iconName = "misIconRed")]
    public partial class mvHealthController : vHealthController
    {
        public ValueChangedEvent onChangeMaxHealth;

        public override void ChangeMaxHealth(int value)
        {
            base.ChangeMaxHealth(value);

            onChangeMaxHealth.Invoke(value);
        }
    }
}
