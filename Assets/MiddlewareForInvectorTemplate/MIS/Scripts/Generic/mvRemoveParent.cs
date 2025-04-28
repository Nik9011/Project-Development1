using Invector;
using UnityEngine.Events;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("RemoveParent", iconName = "misIconRed")]
    public class mvRemoveParent : vMonoBehaviour
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Events", order = 99)]
        public UnityEvent OnRemoved;

        
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void RemoveParent()
        {
            transform.SetParent(null);

            OnRemoved?.Invoke();
        }
    }
}