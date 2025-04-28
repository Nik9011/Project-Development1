using Invector;
using Invector.vCamera;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("Action Shooter", iconName = "misIconRed")]
    public class mvThirdPersonCamera : vThirdPersonCamera
    {
        private static mvThirdPersonCamera _instance;

        public static mvThirdPersonCamera Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<mvThirdPersonCamera>();

                    //Tell unity not to destroy this object when loading a new scene!
                    //DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        #region inspector properties
#if MIS_SWIMMING
        // ----------------------------------------------------------------------------------------------------
        // 
        public bool useRotateOnly = false;
#endif
        #endregion

        #region hide properties    
        #endregion


#if MIS_SWIMMING
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public override void FixedUpdate()
        {
            if (useRotateOnly)
            {
                Quaternion newRot = Quaternion.Euler(mouseY + offsetMouse.y, mouseX + offsetMouse.x, 0);
                
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(currentTarget.position - transform.position) * newRot,
                    smoothCameraRotation * Time.fixedDeltaTime);
            }
            else
            {
                base.FixedUpdate();
            }
        }
#endif
    }
}