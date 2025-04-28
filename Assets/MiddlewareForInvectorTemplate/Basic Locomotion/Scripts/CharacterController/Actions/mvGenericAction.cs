using Invector;
using Invector.vCharacterController.vActions;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("GenericAction", iconName = "misIconRed")]
    public partial class mvGenericAction : vGenericAction
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void CheckForTriggerAction()
        {
            if (actions.Count == 0 && !triggerAction || isLockTriggerEvents)
                return;

            vTriggerGenericAction _triggerAction = GetNearAction();

            if (!doingAction && triggerAction != _triggerAction)
            {
                triggerAction = _triggerAction;

                if (triggerAction)
                {
                    triggerAction.OnValidate.Invoke(gameObject);
                    OnEnterTriggerAction.Invoke(triggerAction);
                }
            }

            TriggerActionInput();
        }
    }
}