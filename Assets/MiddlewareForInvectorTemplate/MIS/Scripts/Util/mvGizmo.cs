using Invector;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("Gizmo", iconName = "misIconRed")]
    public class mvGizmo : vMonoBehaviour
    {
#if MIS
        // ----------------------------------------------------------------------------------------------------
        // 
        [mvEditorToolbar("Settings")]
        public GizmoType gizmoType = GizmoType.Sphere;
        public Color color = Color.white;
        public float radius = 0.1f;

#if UNITY_EDITOR
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void OnDrawGizmosSelected()
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.matrix = transform.localToWorldMatrix;

            switch (gizmoType)
            {
            case GizmoType.Sphere:
                Gizmos.DrawWireSphere(Vector3.zero, radius);
                break;

            case GizmoType.Cube:
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one * radius);
                break;
            }

            Gizmos.color = oldColor;
        }
#endif

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public enum GizmoType
        {
            Sphere,
            Cube
        }
#endif
    }
}