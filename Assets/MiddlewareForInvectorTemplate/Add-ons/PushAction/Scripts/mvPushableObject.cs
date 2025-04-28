#pragma warning disable 0414

#if MIS_INVECTOR_PUSH
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public class mvPushableObject : vPushableObject
    {
        [Header("Control Rigidbody Constraints")]
        [SerializeField] protected bool freezeRotationX = true;
        [SerializeField] protected bool freezeRotationY = true;
        [SerializeField] protected bool freezeRotationZ = true;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        internal override void StartPushAndPull()
        {
            base.StartPushAndPull();

            if (controlBodyConstriants)
            {
                if (!freezeRotationX)
                    body.constraints &= ~RigidbodyConstraints.FreezeRotationX;

                if (!freezeRotationY)
                    body.constraints &= ~RigidbodyConstraints.FreezeRotationY;

                if (!freezeRotationZ)
                    body.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }
}
#endif