using Invector;
using Invector.vCharacterController.vActions;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("mvTriggerLadderAction", iconName = "misIconRed")]
    public class mvTriggerLadderAction : vTriggerLadderAction
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("MatchTarget")]
        [vHelpBox("If true, the EnterMatchTarget position is automatically adjusted to the character's position. In this case, EnterMatchTargetYOffset value is added.")]
        public bool isStaticEnterMatchTarget = true;
        public float enterMatchTargetYOffset = 0.1f;
    }
}