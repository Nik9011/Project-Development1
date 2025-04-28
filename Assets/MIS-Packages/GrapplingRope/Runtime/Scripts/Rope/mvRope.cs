using Invector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [RequireComponent(typeof(LineRenderer))]
    [vClassHeader("Rope", iconName = "misIconRed")]
    public partial class mvRope : vMonoBehaviour
    {
        [Tooltip("The GrapplingRope animation quality. The highter quality produces smoother animations, the more CPU power is required.")]
        [SerializeField] RESOLUTION ropeResolution = RESOLUTION.Normal;
        public enum RESOLUTION
        {
            Low,
            Normal,
            High,
            Ultra
        }
        const int RESOLUTION_PER_UNIT = 8;
        const float RESOLUTION_LOW = 0.5f;
        const float RESOLUTION_HIGH = 1.5f;
        const float RESOLUTION_ULTRA = 2f;
        int currentRopeResolution;

        [Tooltip("The number of rope waves. It is recommended using the default value.")]
        [Range(0.5f, 1.5f)]
        [SerializeField] float waveCountPerUnit = 0.85f;
        float waveCount;

        [Tooltip("The number of rope woobles. It is recommended using the default value.")]
        [Range(2.5f, 4.5f)]
        [SerializeField] float wobbleCount = 3.5f;

        [Tooltip("The speed of rope animation. It is recommended using the default value.")]
        [Range(1f, 3f)]
        [SerializeField] float animationSpeed = 2f;
        [Tooltip("The overall shape of rope waves. It is recommended using the default value.")]
        [SerializeField] AnimationCurve waveCurve = 
            new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(0.8f, 1f), new Keyframe(1f, 0f));
        [Tooltip("The size multiplier of rope waves. It is recommended using the default value.")]
        [Range(0.5f, 1.5f)]
        [SerializeField] float waveSizeMultiplier = 1f;

        LineRenderer lineRenderer;
        Transform throwTransform;
        Vector3 currentRopePosition;
        Coroutine coroutine = null;

        [HideInInspector] public Vector3 targetPosition;

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer)
                lineRenderer.useWorldSpace = true;
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
        public void UpdateRope(bool draw)
        {
            if (draw)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, throwTransform.position);
                lineRenderer.SetPosition(1, targetPosition);
            }
            else
            {
                lineRenderer.positionCount = 0;
                targetPosition = Vector3.zero;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void RemoveRope()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = null;
            lineRenderer.positionCount = 0;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void ThrowRope(Vector3 target, UnityAction callback)
        {
            targetPosition = target;

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                lineRenderer.positionCount = 0;
            }
            coroutine = StartCoroutine(AnimateRope(targetPosition, callback));
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        IEnumerator AnimateRope(Vector3 targetWorldPos, UnityAction callback)
        {
            float distance = Vector3.Distance(throwTransform.position, targetWorldPos);
            currentRopeResolution = (int)(distance * RESOLUTION_PER_UNIT);

            switch (ropeResolution)
            {
            case RESOLUTION.Low:
                currentRopeResolution = (int)(currentRopeResolution * RESOLUTION_LOW);
                break;
            case RESOLUTION.High:
                currentRopeResolution = (int)(currentRopeResolution * RESOLUTION_HIGH);
                break;
            case RESOLUTION.Ultra:
                currentRopeResolution = (int)(currentRopeResolution * RESOLUTION_ULTRA);
                break;
            }
            lineRenderer.positionCount = currentRopeResolution;

            waveCount = distance * waveCountPerUnit;

            float percent = 0f;
            while (percent <= 1f)
            {
                percent += Time.deltaTime * animationSpeed;
                SetPoint(percent);
                yield return null;
            }

            SetPoint(1f);
            
            coroutine = null;
            callback?.Invoke();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void SetPoint(float percent)
        {
            var up = Quaternion.LookRotation((targetPosition - throwTransform.position).normalized) * Vector3.up;
            var right = Quaternion.LookRotation((targetPosition - throwTransform.position).normalized) * Vector3.right;

            currentRopePosition = Vector3.Lerp(currentRopePosition, targetPosition, percent);

            for (int i = 0; i < currentRopeResolution; i++)
            {
                float reversePercent = 1 - percent;
                float amplitude = Mathf.Sin(reversePercent * wobbleCount * Mathf.PI) * ((1f - (float)i / currentRopeResolution) * waveSizeMultiplier);

                var delta = i / (float)currentRopeResolution;
                var offset = up * amplitude * Mathf.Sin(delta * waveCount * Mathf.PI) * waveCurve.Evaluate(delta) +
                    right * amplitude * Mathf.Cos(delta * waveCount * Mathf.PI) * waveCurve.Evaluate(delta);

                lineRenderer.SetPosition(i, Vector3.Lerp(throwTransform.position, currentRopePosition, delta) + offset);
            }
        }
    }
}