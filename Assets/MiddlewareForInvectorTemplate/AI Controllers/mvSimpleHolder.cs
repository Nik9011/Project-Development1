#if MIS_FSM_AI && INVECTOR_AI_TEMPLATE
using Invector;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("mvSimpleHolder", iconName = "misIconRed")]
    public class mvSimpleHolder : vSimpleHolder
    {
        public bool equipOnStart = true;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void Start()
        {
            if (equipOnStart)
                EquipWeapon();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void EquipWeaponImmediatly()
        {
            //if (isEquiped)
            //    return;

            SetActiveHolder(true);
            SetActiveWeapon(false);
            isEquiped = true;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void UnequipWeaponImmediatly()
        {
            //if (!isEquiped)
            //    return;

            SetActiveHolder(false);
            SetActiveWeapon(true);
            isEquiped = false;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void SetActiveHolder(bool active)
        {
            if (holderObject)
                holderObject.SetActive(active);

            if (active)
                onEnableHolder.Invoke();
            else
                onDisableHolder.Invoke();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void SetActiveWeapon(bool active)
        {
            if (weaponObject)
                weaponObject.SetActive(active);

            if (active)
                onEnableWeapon.Invoke();
            else
                onDisableWeapon.Invoke();
        }
    }
}
#endif