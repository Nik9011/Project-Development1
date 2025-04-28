using Invector;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [RequireComponent(typeof(LineRenderer))]
    [vClassHeader("SpringRope", iconName = "misIconRed")]
    public partial class mvSpringRope : vMonoBehaviour
    {
        [vEditorToolbar("Rope")]
        public float ropeWidth = 0.05f;

        public int quality = 500;
        public float damper = 3;
        public float strength = 100;
        public float velocity = 60;
        public float waveCount = 3;
        public float waveHeight = 1;
        public float ropeSpeed = 5f;
        public AnimationCurve affectCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.6f, 0.6f), new Keyframe(1f, 0f));

        // ----------------------------------------------------------------------------------------------------
        // 
        mvSpring spring;
        LineRenderer lineRenderer;

        Transform throwTransform;
        [HideInInspector] public Vector3 targetPosition;
        Vector3 currentGrapplePosition;
        [HideInInspector] public bool isOnAction = false;
        bool releaseImmediately = true;
        int releaseQuality;

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void Start()
        {
            if (TryGetComponent(out lineRenderer))
            {
                lineRenderer.useWorldSpace = true;
                lineRenderer.startWidth = ropeWidth;
                lineRenderer.endWidth = ropeWidth;
            }

            spring = new mvSpring();
            spring.SetTarget(0);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void SetThrowTransform(Transform target)
        {
            throwTransform = target;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Throw(Vector3 target)
        {
            if (throwTransform == null)
            {
                Debug.LogError("[MIS-GrapplingRope]throwTransform must be set");
                return;
            }

            isOnAction = true;
            targetPosition = target;

            if (targetPosition == Vector3.zero)
                return;

            Vector3 dir = targetPosition - throwTransform.position;
            float distance = dir.magnitude;

            releaseImmediately = true;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Release(bool immediate = true)
        {
            isOnAction = false;
            targetPosition = Vector3.zero;

            releaseImmediately = immediate;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void LateUpdate()
        {
            if (throwTransform == null)
                return;

            if (isOnAction)
            {
                if (lineRenderer.positionCount == 0)
                {
                    spring.SetVelocity(velocity);
                    lineRenderer.positionCount = quality + 1;
                    releaseQuality = quality;
                }

                spring.SetDamper(damper);
                spring.SetStrength(strength);
                spring.Update(Time.deltaTime);

                var up = Quaternion.LookRotation((targetPosition - throwTransform.position).normalized) * Vector3.up;
                currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, targetPosition, Time.deltaTime * ropeSpeed);

                for (var i = 0; i < quality + 1; i++)
                {
                    var delta = i / (float)quality;
                    var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

                    lineRenderer.SetPosition(i, Vector3.Lerp(throwTransform.position, currentGrapplePosition, delta) + offset);
                }
            }
            else
            {
                if (releaseImmediately)
                {
                    currentGrapplePosition = throwTransform.position;

                    spring.Reset();
                    lineRenderer.positionCount = 0;
                }
                else
                {
                    //spring.SetVelocity(velocity);
                    //spring.SetDamper(damper * 0.5f);
                    //spring.SetStrength(strength);
                    spring.Update(Time.deltaTime);

                    currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, throwTransform.position, Time.deltaTime * ropeSpeed * 2f);
                    var right = Quaternion.LookRotation((currentGrapplePosition - throwTransform.position).normalized) * Vector3.right;

                    if (Mathf.Approximately((currentGrapplePosition - throwTransform.position).sqrMagnitude, 2f))
                    {
                        releaseImmediately = true;
                        return;
                    }

                    for (var i = 0; i < releaseQuality + 1; i++)
                    {
                        var delta = i / (float)releaseQuality;
                        var offset = right * waveHeight * Mathf.Sin(delta * 2/*waveCount*/ * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

                        lineRenderer.SetPosition(i, Vector3.Lerp(throwTransform.position, currentGrapplePosition, delta) + offset);
                    }
                }
            }
        }
    }
}