using Invector;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [RequireComponent(typeof(Collider))]
    [vClassHeader("Parent Trigger", iconName = "misIconRed")]
    public class mvParentTrigger : vMonoBehaviour
    {
#if MIS
        protected Collider trigger;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void Start()
        {
            if (TryGetComponent(out trigger))
            {
                trigger.isTrigger = true;
                enabled = true;
            }
            else
            {
                enabled = false;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag(MISRuntimeTagLayer.TAG_PLAYER) && other.transform.parent != transform /*&& other.gameObject.TryGetComponent(out mvThirdPersonController cc)*/)
                other.transform.parent = transform;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.transform.CompareTag(MISRuntimeTagLayer.TAG_PLAYER) && other.transform.parent == transform)
                other.transform.parent = null;
        }
#endif
    }
}