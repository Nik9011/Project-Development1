using Invector;
using System.Collections;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("Ignore Colliders", iconName = "misIconRed")]
    public class mvIgnoreColliders : vMonoBehaviour
    {
#if MIS
        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Settings", order = 1)]
        public Collider mainCollider;


        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Debug", order = 100)]
        [mvReadOnly] public Collider[] parentColliders;
        [mvReadOnly] public Collider[] childColliders;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            if (mainCollider != null)
            {
                if (parentColliders != null)
                    IgnoreColliders(mainCollider, parentColliders, true);

                if (childColliders != null)
                    IgnoreColliders(mainCollider, childColliders, true);
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void IgnoreColliders(Collider collider, Collider[] others, bool ignore)
        {
            if (collider == null || others == null)
                return;

            for (int i = 0; i < others.Length; i++)
            {
                if (others[i] != null)
                    Physics.IgnoreCollision(collider, others[i], ignore);
            }
        }
#endif
    }
}
