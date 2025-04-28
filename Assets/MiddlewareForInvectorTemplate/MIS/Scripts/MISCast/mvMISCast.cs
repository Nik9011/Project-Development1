using System.Collections.Generic;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [System.Serializable]
    public class mvMISCast
    {
        public bool useCast = true;
        [Min(0f)] public float backOff;

        [Header("Result")]
        [mvReadOnly] public bool isDetected;
        [mvReadOnly] public float distance;
        [mvReadOnly] public RaycastHit hit;

#if UNITY_EDITOR
        [Header("Debug")]
        public Color hitColor = Color.red;
#endif

        protected RaycastHit[] hits;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public mvMISCast()
        {
            this.useCast = true;
            this.backOff = 0f;

            this.isDetected = false;
            this.distance = 0f;
            this.hit = default;
            hits = new RaycastHit[8];
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void ClearResult()
        {
            this.isDetected = false;
            this.distance = 0f;
            this.hit = default;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual int GetClosestHit(int hitCount, IMISColliderFilter filter, RaycastHit[] hits, out RaycastHit hit)
        {
            hit = default;

            if (hitCount == 0)
                return 0;

            int filteredCount = hitCount;
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < hitCount; i++)
            {
                if (hits[i].distance <= 0f || (filter != null && filter.FilterCollider(hits[i].collider)))
                {
                    filteredCount--;
                    continue;
                }

                if (hits[i].distance < closestDistance)
                {
                    hit = hits[i];
                    closestDistance = hit.distance;
                }
            }

            return filteredCount;
        }
        protected virtual int GetClosestHit(int hitCount, List<string> ignoreTags, IMISColliderFilter filter, RaycastHit[] hits, out RaycastHit hit)
        {
            hit = default;

            if (hitCount == 0)
                return 0;

            int filteredCount = hitCount;
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < hitCount; i++)
            {
                if (ignoreTags.Contains(hits[i].collider.gameObject.tag) || hits[i].distance <= 0f || (filter != null && filter.FilterCollider(hits[i].collider)))
                {
                    filteredCount--;
                    continue;
                }

                if (hits[i].distance < closestDistance)
                {
                    hit = hits[i];
                    closestDistance = hit.distance;
                }
            }

            return filteredCount;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual int GetOverlaps(int hitCount, IMISColliderFilter filter, Collider[] results, out List<Collider> resultList)
        {
            resultList = new List<Collider>();

            if (hitCount == 0)
                return 0;

            int filteredCount = hitCount;

            for (int i = 0; i < hitCount; i++)
            {
                if (filter != null && filter.FilterCollider(results[i]))
                {
                    filteredCount--;
                    continue;
                }

                resultList.Add(results[i]);
            }

            return filteredCount;
        }
        protected virtual int GetOverlaps(int hitCount, List<string> ignoreTags, IMISColliderFilter filter, Collider[] results, out List<Collider> resultList)
        {
            resultList = new List<Collider>();

            if (hitCount == 0)
                return 0;

            int filteredCount = hitCount;

            for (int i = 0; i < hitCount; i++)
            {
                if (ignoreTags.Contains(results[i].gameObject.tag) || (filter != null && filter.FilterCollider(results[i])))
                {
                    filteredCount--;
                    continue;
                }

                resultList.Add(results[i]);
            }

            return filteredCount;
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public static class mvMISCastExtension
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void DrawGizmosRayCast(this mvMISRayCast caster, Transform tr, Vector3 direction)
        {
#if UNITY_EDITOR
            Vector3 p1 = Vector3.zero;

            if (tr == null)
                p1 = caster.origin + (caster.backOff * -direction);
            else
                p1 = tr.TransformPoint(caster.origin) + (caster.backOff * -direction);

            Color oldColor = Gizmos.color;

            if (caster.isDetected)
            {
                Gizmos.color = caster.hitColor;
                Gizmos.DrawRay(p1, caster.hit.distance * direction);
            }
            else
            {
                Gizmos.color = Color.white;
                Gizmos.DrawRay(p1, caster.maxDistance * direction);
            }

            Gizmos.color = oldColor;
#endif
        }
        
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void DrawSphereCast(this mvMISSphereCast caster, Transform tr, Vector3 direction, float duration = 0f, bool depthTest = true)
        {
#if UNITY_EDITOR
            Vector3 p1 = Vector3.zero;

            if (tr == null)
                p1 = caster.origin + (caster.backOff * -direction);
            else
                p1 = tr.TransformPoint(caster.origin) + (caster.backOff * -direction);

            if (caster.isDetected)
            {
                MISDebugDraw.WireSphere(p1, caster.radius, Color.cyan, duration, depthTest);

                MISDebugDraw.WireSphere(p1 + (caster.hit.distance * direction), caster.radius, caster.hitColor, duration, depthTest);
                Debug.DrawRay(p1, caster.hit.distance * direction, caster.hitColor, duration);
            }
            else
            {
                MISDebugDraw.WireSphere(p1, caster.radius, Color.cyan, duration, depthTest);

                MISDebugDraw.WireSphere(p1 + (caster.maxDistance * direction), caster.radius, Color.white, duration, depthTest);
                Debug.DrawRay(p1, caster.maxDistance * direction, Color.white, duration);
            }
#endif
        }
        public static void DrawGizmosSphereCast(this mvMISSphereCast caster, Transform tr, Vector3 direction)
        {
#if UNITY_EDITOR
            Vector3 p1 = Vector3.zero;

            if (tr == null)
                p1 = caster.origin + (caster.backOff * -direction);
            else
                p1 = tr.TransformPoint(caster.origin) + (caster.backOff * -direction);

            Color oldColor = Gizmos.color;

            if (caster.isDetected)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(p1, caster.radius);

                Gizmos.color = caster.hitColor;
                Gizmos.DrawWireSphere(p1 + (caster.hit.distance * direction), caster.radius);
                Gizmos.DrawRay(p1, caster.hit.distance * direction);
            }
            else
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(p1, caster.radius);

                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(p1 + (caster.maxDistance * direction), caster.radius);
                Gizmos.DrawRay(p1, caster.maxDistance * direction);
            }

            Gizmos.color = oldColor;
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void DrawGizmosBoxCast(this mvMISBoxCast caster, Transform tr, Vector3 direction, Quaternion orientation, Vector3 lossyScale)
        {
#if UNITY_EDITOR
            Vector3 p1 = Vector3.zero;

            if (tr == null)
                p1 = caster.origin + (caster.backOff * -direction);
            else
                p1 = tr.TransformPoint(caster.origin) + (caster.backOff * -direction);

            Color oldColor = Gizmos.color;
            Matrix4x4 defaultMatrix = Gizmos.matrix;

            if (caster.isDetected)
            {
                Gizmos.color = Color.cyan;
                Gizmos.matrix = Matrix4x4.TRS(p1, orientation, lossyScale);
                Gizmos.DrawWireCube(Vector3.zero, 2f * caster.halfExtents);

                Gizmos.color = caster.hitColor;
                Gizmos.matrix = Matrix4x4.TRS(p1 + (caster.hit.distance * direction), orientation, lossyScale);
                Gizmos.DrawWireCube(Vector3.zero, 2f * caster.halfExtents);
                Gizmos.matrix = defaultMatrix;
                Gizmos.DrawRay(p1, caster.hit.distance * direction);
            }
            else
            {
                Gizmos.color = Color.cyan;
                Gizmos.matrix = Matrix4x4.TRS(p1, orientation, lossyScale);
                Gizmos.DrawWireCube(Vector3.zero, 2f * caster.halfExtents);

                Gizmos.color = Color.white;
                Gizmos.matrix = Matrix4x4.TRS(p1 + (caster.maxDistance * direction), orientation, lossyScale);
                Gizmos.DrawWireCube(Vector3.zero, 2f * caster.halfExtents);
                Gizmos.matrix = defaultMatrix;
                Gizmos.DrawRay(p1, caster.maxDistance * direction);
            }

            Gizmos.color = oldColor;
            Gizmos.matrix = defaultMatrix;
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void DrawGizmosCapsuleCast(this mvMISCapsuleCast caster, Transform tr, Vector3 direction)
        {
#if UNITY_EDITOR
            Vector3 p1 = Vector3.zero;
            Vector3 p2 = Vector3.zero;

            if (tr == null)
            {
                p1 = caster.origin1 + (caster.backOff * -direction);
                p2 = caster.origin2 + (caster.backOff * -direction);
            }
            else
            {
                p1 = tr.TransformPoint(caster.origin1) + (caster.backOff * -direction);
                p2 = tr.TransformPoint(caster.origin2) + (caster.backOff * -direction);
            }

            Color oldColor = Gizmos.color;

            if (caster.isDetected)
            {
                Gizmos.color = Color.cyan;
                DrawGizmosWireCapsule(p1, p2, caster.radius);

                Gizmos.color = caster.hitColor;
                DrawGizmosWireCapsule(
                    p1 + (caster.hit.distance * direction),
                    p2 + (caster.hit.distance * direction),
                    caster.radius);
                Gizmos.DrawRay(p1, caster.hit.distance * direction);
                Gizmos.DrawRay(p2, caster.hit.distance * direction);
            }
            else
            {
                Gizmos.color = Color.cyan;
                DrawGizmosWireCapsule(p1, p2, caster.radius);

                Gizmos.color = Color.white;
                DrawGizmosWireCapsule(
                    p1 + (caster.maxDistance * direction),
                    p2 + (caster.maxDistance * direction),
                    caster.radius);
                Gizmos.DrawRay(p1, caster.maxDistance * direction);
                Gizmos.DrawRay(p2, caster.maxDistance * direction);
            }

            Gizmos.color = oldColor;
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void DrawGizmosWireCapsule(Vector3 origin1, Vector3 origin2, float radius)
        {
#if UNITY_EDITOR
            if (origin1 == origin2)
                return;

            using (new UnityEditor.Handles.DrawingScope(Gizmos.color, Gizmos.matrix))
            {
                Quaternion p1Rotation = Quaternion.LookRotation(origin1 - origin2);
                Quaternion p2Rotation = Quaternion.LookRotation(origin2 - origin1);

                // Check if capsule direction is collinear to Vector.up
                float c = Vector3.Dot((origin1 - origin2).normalized, Vector3.up);

                if (c == 1f || c == -1f)
                    p2Rotation = Quaternion.Euler(p2Rotation.eulerAngles.x, p2Rotation.eulerAngles.y + 180f, p2Rotation.eulerAngles.z);

                // First side
                UnityEditor.Handles.DrawWireArc(origin1, p1Rotation * Vector3.left, p1Rotation * Vector3.down, 180f, radius);
                UnityEditor.Handles.DrawWireArc(origin1, p1Rotation * Vector3.up, p1Rotation * Vector3.left, 180f, radius);
                UnityEditor.Handles.DrawWireDisc(origin1, (origin2 - origin1).normalized, radius);

                // Second side
                UnityEditor.Handles.DrawWireArc(origin2, p2Rotation * Vector3.left, p2Rotation * Vector3.down, 180f, radius);
                UnityEditor.Handles.DrawWireArc(origin2, p2Rotation * Vector3.up, p2Rotation * Vector3.left, 180f, radius);
                UnityEditor.Handles.DrawWireDisc(origin2, (origin1 - origin2).normalized, radius);

                // Lines
                UnityEditor.Handles.DrawLine(origin1 + p1Rotation * Vector3.down * radius, origin2 + p2Rotation * Vector3.down * radius);
                UnityEditor.Handles.DrawLine(origin1 + p1Rotation * Vector3.left * radius, origin2 + p2Rotation * Vector3.right * radius);
                UnityEditor.Handles.DrawLine(origin1 + p1Rotation * Vector3.up * radius, origin2 + p2Rotation * Vector3.up * radius);
                UnityEditor.Handles.DrawLine(origin1 + p1Rotation * Vector3.right * radius, origin2 + p2Rotation * Vector3.left * radius);
            }
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void DrawGizmosOverlapSphere(this mvMISOverlapSphere caster, Transform tr)
        {
#if UNITY_EDITOR
            Vector3 p1 = Vector3.zero;

            if (tr == null)
                p1 = caster.origin;
            else
                p1 = tr.TransformPoint(caster.origin);

            Color oldColor = Gizmos.color;

            if (caster.isDetected)
            {
                Gizmos.color = caster.hitColor;
                Gizmos.DrawWireSphere(p1, caster.radius);
            }
            else
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(p1, caster.radius);
            }

            Gizmos.color = oldColor;
#endif
        }
    }
}