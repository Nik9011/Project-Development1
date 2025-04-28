using Invector;
using Invector.vCharacterController.vActions;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("mvLadderAction", iconName = "misIconRed")]
    public class mvLadderAction : vLadderAction
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Settings", overrideChildOrder: true, order = 0)]
        [SerializeField] protected bool useOnlyOnGrounded = false;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void EnterLadderInput()
        {
            if (targetLadderAction == null || tpInput.cc.customAction)
                return;

            if (useOnlyOnGrounded && (tpInput.cc.isJumping || !tpInput.cc.isGrounded || tpInput.cc.isRolling))
                return;

            if (enterInput.GetButtonDown() && !enterLadderStarted && !isUsingLadder && !targetLadderAction.autoAction)
                TriggerEnterLadder();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void TriggerEnterLadder()
        {
            base.TriggerEnterLadder();

            if (currentLadderAction.GetType() == typeof(mvTriggerLadderAction))
            {
                if ((currentLadderAction as mvTriggerLadderAction).isStaticEnterMatchTarget == false)
                {
                    Vector3 matchTargetPosition = currentLadderAction.matchTarget.position;
                    matchTargetPosition.y = tpInput.cc.transform.position.y + (currentLadderAction as mvTriggerLadderAction).enterMatchTargetYOffset;
                    currentLadderAction.matchTarget.position = matchTargetPosition;
                }
            }
        }
    }
}