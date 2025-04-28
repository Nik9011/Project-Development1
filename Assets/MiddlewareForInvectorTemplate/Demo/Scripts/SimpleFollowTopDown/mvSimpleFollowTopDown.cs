using Invector;
using Invector.vCharacterController;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("SimpleFollowTopDown", iconName = "misIconRed")]
    public class mvSimpleFollowTopDown : vMonoBehaviour
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        public GenericInput cameraZoomInput = new GenericInput("Mouse ScrollWheel", "", "");
        [Min(1f)] public float zoomSpeed = 100f;
        public mvFloatMinMax height = new mvFloatMinMax(10f, 300f);

        public Transform target;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void Update()
        {
            if (target == null)
                return;

            CameraZoomInput();

            transform.position = target.position + (height.now * Vector3.up);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CameraZoomInput()
        {
            if (!cameraZoomInput.useInput)
                return;

            height.now -= cameraZoomInput.GetAxis() * zoomSpeed;
            height.now = Mathf.Clamp(height.now, height.min, height.max);
        }
    }
}