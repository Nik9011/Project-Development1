using Invector;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("Billboard", iconName = "misIconRed")]
    public class mvBillboard : vMonoBehaviour
    {
#if MIS
        // ----------------------------------------------------------------------------------------------------
        // 
        [vHelpBox("To make it easier for the player to see the contents of this Canvas, the mvBillboard always rotates towards the main camera.", vHelpBoxAttribute.MessageType.Info)]
        [mvReadOnly] [SerializeField] protected string comment = "";


        // ----------------------------------------------------------------------------------------------------
        // 
        protected Transform tr;


        // ----------------------------------------------------------------------------------------------------
        // 
        Camera mainCamera;
        protected Transform MainCameraTransform
        {
            get
            {
                if (mainCamera == null)
                    mainCamera = Camera.main;

                return mainCamera == null ? null : mainCamera.transform;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void Start()
        {
            tr = transform;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void Update()
        {
            if (MainCameraTransform == null)
                return;

            tr.LookAt(tr.position + MainCameraTransform.rotation * Vector3.forward, MainCameraTransform.rotation * Vector3.up);
        }
#endif
    }
}