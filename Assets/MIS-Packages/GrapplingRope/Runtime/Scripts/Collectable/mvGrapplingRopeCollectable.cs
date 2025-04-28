using Invector;
using UnityEngine;
using UnityEngine.Events;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("GrapplingRope Collectable", iconName = "misIconRed")]
    public partial class mvGrapplingRopeCollectable : vMonoBehaviour
    {
#if MIS_GRAPPLINGROPE
        public int amount = 1;

        public UnityEvent onCollected;
        public UnityEvent onReachedMaxAmount;

        mvGrapplingRope grapplingRope;

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && other.GetComponentInChildren<mvGrapplingRope>() != null)
                grapplingRope = other.GetComponentInChildren<mvGrapplingRope>();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void CollectGrapplingRope()
        {
            if (grapplingRope.ropeAmount + amount <= grapplingRope.ropeMaxAmount)
            {
                grapplingRope.SetAmount(amount);
                onCollected?.Invoke();

                Destroy(this.gameObject);
            }
            else
            {
                onReachedMaxAmount?.Invoke();
            }
        }
#endif
    }
}