using System.Collections.Generic;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class mvMISOverlapSphere : mvMISCast
    {
        [Header("OverlapSphere")]
        public Vector3 origin;
        [Min(0f)] public float radius;

        public List<Collider> resultList;
        protected Collider[] results;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public mvMISOverlapSphere() : base()
        {
            this.origin = Vector3.zero;
            this.radius = 0.1f;

            resultList = new List<Collider>();
            results = new Collider[8];
        }
        public mvMISOverlapSphere(float radius, int resultCount) : this()
        {
            this.radius = radius;
            results = new Collider[resultCount];
        }
        public mvMISOverlapSphere(Vector3 origin, float radius, int resultCount) : base()
        {
            this.origin = origin;
            this.radius = radius;

            resultList = new List<Collider>();
            results = new Collider[resultCount];
        }
        public mvMISOverlapSphere(Vector3 origin, float radius, int resultCount, float backOff) : this(origin, radius, resultCount)
        {
            this.backOff = backOff;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void ClearResult()
        {
            this.isDetected = false;
            this.distance = 0f;
            this.resultList.Clear();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Overlap(Transform transform, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = transform.TransformPoint(origin);

            if (useCast &&
                GetOverlaps(Physics.OverlapSphereNonAlloc(p1, radius, results, targetLayerMask, query), filter, results, out resultList) > 0)
            {
                isDetected = true;
                distance = 0f;
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawGizmosOverlapSphere(transform);
            }
#endif
        }
        public void Overlap(Transform transform, List<string> ignoreTags, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = transform.TransformPoint(origin);

            if (useCast &&
                GetOverlaps(Physics.OverlapSphereNonAlloc(p1, radius, results, targetLayerMask, query), ignoreTags, filter, results, out resultList) > 0)
            {
                isDetected = true;
                distance = 0f;
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawGizmosOverlapSphere(transform);
            }
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Overlap(LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = origin;

            if (useCast &&
                GetOverlaps(Physics.OverlapSphereNonAlloc(p1, radius, results, targetLayerMask, query), filter, results, out resultList) > 0)
            {
                isDetected = true;
                distance = 0f;
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawGizmosOverlapSphere(null);
            }
#endif
        }
        public void Overlap(List<string> ignoreTags, LayerMask targetLayerMask, QueryTriggerInteraction query, IMISColliderFilter filter, bool debug = false)
        {
            Vector3 p1 = origin;

            if (useCast &&
                GetOverlaps(Physics.OverlapSphereNonAlloc(p1, radius, results, targetLayerMask, query), ignoreTags, filter, results, out resultList) > 0)
            {
                isDetected = true;
                distance = 0f;
            }
            else
            {
                isDetected = false;
                distance = 0f;
            }

#if UNITY_EDITOR
            if (useCast && debug)
            {
                this.DrawGizmosOverlapSphere(null);
            }
#endif
        }
    }
}