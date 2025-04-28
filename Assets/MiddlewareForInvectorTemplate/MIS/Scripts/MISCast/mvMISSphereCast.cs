using System.Collections.Generic;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class mvMISSphereCast : mvMISCast
    {
        [Header("SphereCast")]
        public Vector3 origin;
        [Min(0f)] public float radius;
        [Min(0f)] public float maxDistance;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public mvMISSphereCast() : base()
        {
            this.origin = Vector3.zero;
            this.radius = 0.1f;
            this.maxDistance = 2f;
        }
        public mvMISSphereCast(Vector3 origin, float radius, float maxDistance) : base()
        {
            this.origin = origin;
            this.radius = radius;
            this.maxDistance = maxDistance;
        }
        public mvMISSphereCast(Vector3 origin, float radius, float maxDistance, float backOff) : this(origin, radius, maxDistance)
        {
            this.backOff = backOff;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Cast(
            Transform transform, 
            Vector3 direction, 
            LayerMask targetLayerMask, 
            QueryTriggerInteraction query, 
            IMISColliderFilter filter, 
            float duration = 0f, 
            bool depthTest = true, 
            bool debug = false)
        {
            Vector3 p1 = transform.TransformPoint(origin) + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.SphereCastNonAlloc(p1, radius, direction, hits, maxDistance, targetLayerMask, query), filter, hits, out hit) > 0)
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
                this.DrawSphereCast(transform, direction, duration, depthTest);
            }
#endif
        }
        public void Cast(
            Transform transform, 
            Vector3 direction, 
            List<string> ignoreTags, 
            LayerMask targetLayerMask, 
            QueryTriggerInteraction query, 
            IMISColliderFilter filter, 
            float duration = 0f, 
            bool depthTest = true, 
            bool debug = false)
        {
            Vector3 p1 = transform.TransformPoint(origin) + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.SphereCastNonAlloc(p1, radius, direction, hits, maxDistance, targetLayerMask, query), ignoreTags, filter, hits, out hit) > 0)
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
                this.DrawSphereCast(transform, direction, duration, depthTest);
            }
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Cast(Vector3 direction, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, float duration = 0f, bool depthTest = true, bool debug = false)
        {
            Vector3 p1 = origin + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.SphereCastNonAlloc(p1, radius, direction, hits, maxDistance, targetLayerMask, query), filter, hits, out hit) > 0)
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
                this.DrawSphereCast(null, direction, duration, depthTest);
            }
#endif
        }
        public void Cast(Vector3 direction, List<string> ignoreTags, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, float duration = 0f, bool depthTest = true, bool debug = false)
        {
            Vector3 p1 = origin + (backOff * -direction);

            if (useCast &&
                GetClosestHit(Physics.SphereCastNonAlloc(p1, radius, direction, hits, maxDistance, targetLayerMask, query), ignoreTags, filter, hits, out hit) > 0)
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
                this.DrawSphereCast(null, direction, duration, depthTest);
            }
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void CastOnly(Transform transform, Vector3 direction, LayerMask targetLayerMask, float duration = 0f, bool depthTest = true, bool debug = false)
        {
            Vector3 p1 = transform.TransformPoint(origin) + (backOff * -direction);

            if (useCast &&
                Physics.SphereCast(p1, radius, direction, out hit, maxDistance))
            {
                if (targetLayerMask.LayerMaskContains(hit.collider.gameObject.layer))
                {
                    isDetected = true;
                    distance = MISMath.Round(hit.distance + radius - backOff, 2);
                }
                else
                {
                    isDetected = false;
                    distance = 0f;
                }
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawSphereCast(transform, direction, duration, depthTest);
            }
#endif
        }
        public void CastOnly(Transform transform, Vector3 direction, List<string> ignoreTags, LayerMask targetLayerMask, float duration = 0f, bool depthTest = true, bool debug = false)
        {
            Vector3 p1 = transform.TransformPoint(origin) + (backOff * -direction);

            if (useCast &&
                Physics.SphereCast(p1, radius, direction, out hit, maxDistance))
            {
                if (!ignoreTags.Contains(hit.collider.gameObject.tag) && targetLayerMask.LayerMaskContains(hit.collider.gameObject.layer))
                {
                    isDetected = true;
                    distance = MISMath.Round(hit.distance + radius - backOff, 2);
                }
                else
                {
                    isDetected = false;
                    distance = 0f;
                }
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawSphereCast(transform, direction, duration, depthTest);
            }
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void CastOnlyExcept(Transform transform, Vector3 direction, LayerMask targetLayerMask, LayerMask exceptLayerMask, float duration = 0f, bool depthTest = true, bool debug = false)
        {
            Vector3 p1 = transform.TransformPoint(origin) + (backOff * -direction);

            exceptLayerMask = ~exceptLayerMask;

            if (useCast &&
                Physics.SphereCast(p1, radius, direction, out hit, maxDistance, exceptLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (targetLayerMask.LayerMaskContains(hit.collider.gameObject.layer))
                {
                    isDetected = true;
                    distance = MISMath.Round(hit.distance + radius - backOff, 2);
                }
                else
                {
                    isDetected = false;
                    distance = 0f;
                }
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawSphereCast(transform, direction, duration, depthTest);
            }
#endif
        }
        public void CastOnlyExcept(Transform transform, Vector3 direction, List<string> ignoreTags, LayerMask targetLayerMask, LayerMask exceptLayerMask, float duration = 0f, bool depthTest = true, bool debug = false)
        {
            Vector3 p1 = transform.TransformPoint(origin) + (backOff * -direction);

            exceptLayerMask = ~exceptLayerMask;

            if (useCast &&
                Physics.SphereCast(p1, radius, direction, out hit, maxDistance, exceptLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (!ignoreTags.Contains(hit.collider.gameObject.tag) && targetLayerMask.LayerMaskContains(hit.collider.gameObject.layer))
                {
                    isDetected = true;
                    distance = MISMath.Round(hit.distance + radius - backOff, 2);
                }
                else
                {
                    isDetected = false;
                    distance = 0f;
                }
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawSphereCast(transform, direction, duration, depthTest);
            }
#endif
        }
    }
}