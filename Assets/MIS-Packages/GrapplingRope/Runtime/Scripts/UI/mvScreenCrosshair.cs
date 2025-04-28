using Invector;
using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.Events;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("Screen Crosshair", iconName = "misIconRed")]
    public class mvScreenCrosshair : vMonoBehaviour
    {
        [SerializeField] RectTransform aimCenter, aimTarget;
        [SerializeField] GameObject validTarget, invalidTarget;
        [SerializeField] bool aimCenterToTarget;
        [SerializeField] bool scaleAimWithMovement = true;
        [SerializeField] float scaleWithMovement = 2f;
        [SerializeField] float smothChangeScale = 2f;
        [Range(0, 1)]
        [SerializeField] float movementSensitity = 0.1f;

        [vEditorToolbar("Event")]
        public UnityEvent onEnableAim;
        public UnityEvent onDisableAim;
        public UnityEvent onCheckvalidAim;
        public UnityEvent onCheckInvalidAim;

        vThirdPersonController cc;
        Camera mainCamera;
        RectTransform canvas;
        bool isAimActive;
        bool isValid;

        Vector2 sizeDeltaTarget;
        Vector2 sizeDeltaCenter;

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void Start()
        {
            canvas = GetComponent<RectTransform>();

            if (aimCenter)
                sizeDeltaCenter = aimCenter.sizeDelta;
            if (aimTarget)
                sizeDeltaTarget = aimTarget.sizeDelta;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void Init(vThirdPersonController cc)
        {
            this.cc = cc;
            mainCamera = Camera.main;

            isValid = true;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void SetAimToCenter(bool validPoint = true)
        {
            if (validPoint != isValid)
            {
                isValid = validPoint;

                if (isValid)
                    onCheckvalidAim.Invoke();
                else
                    onCheckInvalidAim.Invoke();
            }

            if (!aimTarget || !aimCenter)
                return;

            aimTarget.anchoredPosition = aimCenter.anchoredPosition;
            aimTarget.sizeDelta = sizeDeltaTarget;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void SetAimToWordPosition(Vector3 wordPosition, bool validPoint = true)
        {
            if (validPoint != isValid)
            {
                isValid = validPoint;
                if (isValid)
                    onCheckvalidAim.Invoke();
                else
                    onCheckInvalidAim.Invoke();
            }

            if (validPoint == false)
                return;

            if (!aimTarget || !aimCenter)
                return;

            Vector2 viewportPosition = mainCamera.WorldToViewportPoint(wordPosition);
            Vector2 screenPosition = new Vector2(
                ((viewportPosition.x * canvas.sizeDelta.x) - (canvas.sizeDelta.x * 0.5f)),
                ((viewportPosition.y * canvas.sizeDelta.y) - (canvas.sizeDelta.y * 0.5f)));

            if (aimCenterToTarget)
                aimCenter.anchoredPosition = screenPosition;
            aimTarget.anchoredPosition = screenPosition;

            if (scaleAimWithMovement && (cc.input.magnitude > movementSensitity || Input.GetAxis("Mouse X") > movementSensitity || Input.GetAxis("Mouse Y") > movementSensitity))
            {
                aimCenter.sizeDelta = Vector2.Lerp(aimCenter.sizeDelta, sizeDeltaCenter * scaleWithMovement, smothChangeScale * Time.deltaTime);
                aimTarget.sizeDelta = Vector2.Lerp(aimTarget.sizeDelta, sizeDeltaTarget * scaleWithMovement, smothChangeScale * Time.deltaTime);
            }
            else
            {
                aimCenter.sizeDelta = Vector2.Lerp(aimCenter.sizeDelta, sizeDeltaCenter * 1, smothChangeScale * Time.deltaTime);
                aimTarget.sizeDelta = Vector2.Lerp(aimTarget.sizeDelta, sizeDeltaTarget * 1, smothChangeScale * Time.deltaTime);
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void SetActiveAim(bool value)
        {
            if (value != isAimActive)
            {
                isAimActive = value;

                if (value)
                {
                    isValid = true;
                    onEnableAim.Invoke();
                }
                else
                {
                    onDisableAim.Invoke();
                }
            }
        }
    }
}