using System.Collections.Generic;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class mvMISBoxCast : mvMISCast
    {
        [Header("BoxCast")]
        public Vector3 origin;
        public Vector3 halfExtents;
        [Min(0f)] public float maxDistance;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public mvMISBoxCast() : base()
        {
            this.origin = Vector3.zero;
            this.halfExtents = new Vector3(1f, 0.1f, 0f);
            this.maxDistance = 2f;
    }
        public mvMISBoxCast(Vector3 origin, Vector3 halfExtents, float maxDistance) : base()
        {
            this.halfExtents = halfExtents;
            this.origin = origin;
            this.maxDistance = maxDistance;
        }
        public mvMISBoxCast(Vector3 origin, Vector3 halfExtents, float maxDistance, float backOff) : this(origin, halfExtents, maxDistance)
        {
            this.backOff = backOff;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Cast(Transform tr, Vector3 direction, Quaternion orientation, Vector3 lossyScale, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = tr.TransformPoint(origin) + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.BoxCastNonAlloc(p1, halfExtents, direction, hits, orientation, maxDistance, targetLayerMask, query), filter, hits, out hit) > 0)
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
                this.DrawGizmosBoxCast(tr, direction, orientation, lossyScale);
            }
#endif
        }
        public void Cast(Transform tr, Vector3 direction, Quaternion orientation, Vector3 lossyScale, List<string> ignoreTags, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = tr.TransformPoint(origin) + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.BoxCastNonAlloc(p1, halfExtents, direction, hits, orientation, maxDistance, targetLayerMask, query), ignoreTags, filter, hits, out hit) > 0)
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
                this.DrawGizmosBoxCast(tr, direction, orientation, lossyScale);
            }
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Cast(Vector3 direction, Quaternion orientation, Vector3 lossyScale, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = origin + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.BoxCastNonAlloc(p1, halfExtents, direction, hits, orientation, maxDistance, targetLayerMask, query), filter, hits, out hit) > 0)
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
                this.DrawGizmosBoxCast(null, direction, orientation, lossyScale);
            }
#endif
        }
        public void Cast(Vector3 direction, Quaternion orientation, Vector3 lossyScale, List<string> ignoreTags, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = origin + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.BoxCastNonAlloc(p1, halfExtents, direction, hits, orientation, maxDistance, targetLayerMask, query), ignoreTags, filter, hits, out hit) > 0)
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
                this.DrawGizmosBoxCast(null, direction, orientation, lossyScale);
            }
#endif
        }
    }
}