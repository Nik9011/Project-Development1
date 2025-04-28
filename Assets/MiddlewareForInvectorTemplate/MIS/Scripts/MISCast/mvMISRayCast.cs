using System.Collections.Generic;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class mvMISRayCast : mvMISCast
    {
        [Header("RayCast")]
        public Vector3 origin;
        [Min(0f)] public float maxDistance;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public mvMISRayCast() : base()
        {
            this.origin = Vector3.zero;
            this.maxDistance = 2f;
        }
        public mvMISRayCast(Vector3 origin, float maxDistance) : base()
        {
            this.origin = origin;
            this.maxDistance = maxDistance;
        }
        public mvMISRayCast(Vector3 origin, float maxDistance, float backOff) : this(origin, maxDistance)
        {
            this.backOff = backOff;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Cast(Transform transform, Vector3 direction, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = transform.TransformPoint(origin) + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.RaycastNonAlloc(p1, direction, hits, maxDistance, targetLayerMask, query), filter, hits, out hit) > 0)
            {
                isDetected = true;
                distance = MISMath.Round(hit.distance - backOff, 2);
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawGizmosRayCast(transform, direction);
            }
#endif
        }
        public void Cast(Transform transform, Vector3 direction, List<string> ignoreTags, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = transform.TransformPoint(origin) + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.RaycastNonAlloc(p1, direction, hits, maxDistance, targetLayerMask, query), ignoreTags, filter, hits, out hit) > 0)
            {
                isDetected = true;
                distance = MISMath.Round(hit.distance - backOff, 2);
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawGizmosRayCast(transform, direction);
            }
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Cast(Vector3 direction, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = origin + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.RaycastNonAlloc(p1, direction, hits, maxDistance, targetLayerMask, query), filter, hits, out hit) > 0)
            {
                isDetected = true;
                distance = MISMath.Round(hit.distance - backOff, 2);
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawGizmosRayCast(null, direction);
            }
#endif
        }
        public void Cast(Vector3 direction, List<string> ignoreTags, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = origin + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.RaycastNonAlloc(p1, direction, hits, maxDistance, targetLayerMask, query), ignoreTags, filter, hits, out hit) > 0)
            {
                isDetected = true;
                distance = MISMath.Round(hit.distance - backOff, 2);
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawGizmosRayCast(null, direction);
            }
#endif
        }
    }
}