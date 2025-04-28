using System.Collections.Generic;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class mvMISCapsuleCast : mvMISCast
    {
        [Header("CapsuleCast")]
        public Vector3 origin1 = Vector3.zero;
        public Vector3 origin2 = Vector3.zero;
        [Min(0f)] public float radius = 0.1f;
        [Min(0f)] public float maxDistance = 2f;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public mvMISCapsuleCast() : base()
        {
            this.origin1 = Vector3.zero;
            this.origin2 = Vector3.zero;
            this.radius = 0.1f;
            this.maxDistance = 2f;
        }
        public mvMISCapsuleCast(Vector3 origin1, Vector3 origin2, float radius, float maxDistance) : base()
        {
            this.origin1 = origin1;
            this.origin2 = origin2;
            this.radius = radius;
            this.maxDistance = maxDistance;
        }
        public mvMISCapsuleCast(Vector3 origin1, Vector3 origin2, float radius, float maxDistance, float backOff) : this(origin1, origin2, radius, maxDistance)
        {
            this.backOff = backOff;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Cast(Transform transform, Vector3 direction, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = transform.TransformPoint(origin1) + (backOff * -direction);
            Vector3 p2 = transform.TransformPoint(origin2) + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.CapsuleCastNonAlloc(p1, p2, radius, direction, hits, maxDistance, targetLayerMask, query), filter, hits, out hit) > 0)
            {
                isDetected = true;
                distance = MISMath.Round(hit.distance + radius - backOff, 2);
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawGizmosCapsuleCast(transform, direction);
            }
#endif
        }
        public void Cast(Transform transform, Vector3 direction, List<string> ignoreTags, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = transform.TransformPoint(origin1) + (backOff * -direction);
            Vector3 p2 = transform.TransformPoint(origin2) + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.CapsuleCastNonAlloc(p1, p2, radius, direction, hits, maxDistance, targetLayerMask, query), ignoreTags, filter, hits, out hit) > 0)
            {
                isDetected = true;
                distance = MISMath.Round(hit.distance + radius - backOff, 2);
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawGizmosCapsuleCast(transform, direction);
            }
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Cast(Vector3 direction, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = origin1 + (backOff * -direction);
            Vector3 p2 = origin2 + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.CapsuleCastNonAlloc(p1, p2, radius, direction, hits, maxDistance, targetLayerMask, query), filter, hits, out hit) > 0)
            {
                isDetected = true;
                distance = MISMath.Round(hit.distance + radius - backOff, 2);
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawGizmosCapsuleCast(null, direction);
            }
#endif
        }
        public void Cast(Vector3 direction, List<string> ignoreTags, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = origin1 + (backOff * -direction);
            Vector3 p2 = origin2 + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.CapsuleCastNonAlloc(p1, p2, radius, direction, hits, maxDistance, targetLayerMask, query), ignoreTags, filter, hits, out hit) > 0)
            {
                isDetected = true;
                distance = MISMath.Round(hit.distance + radius - backOff, 2);
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawGizmosCapsuleCast(null, direction);
            }
#endif
        }
    }
}