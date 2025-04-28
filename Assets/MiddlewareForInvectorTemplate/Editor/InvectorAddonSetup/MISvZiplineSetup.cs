using Invector.vCharacterController.vActions;
using Invector.vEventSystems;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static com.mobilin.games.MISAnimator;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public partial class MISMainSetup
    {
        // ----------------------------------------------------------------------------------------------------
        // Animator StateMachine/State
        // ----------------------------------------------------------------------------------------------------

        AnimatorStateMachine base_ZiplineSM;
        AnimatorState base_Zipline_FreeRotation;
        AnimatorState base_Zipline_FreeRotationAiming;
        AnimatorState base_Zipline_Hanging;
        AnimatorState base_Zipline_AnchoredWaist;
        AnimatorState base_Zipline_AnchoredWaistAiming;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void ZiplineSetup(mvSetupOption setupOption, GameObject characterObj, GameObject cameraObj)
        {
#if MIS_INVECTOR_ZIPLINE
            // ----------------------------------------------------------------------------------------------------
            // Setup Options
            // ----------------------------------------------------------------------------------------------------


            // ----------------------------------------------------------------------------------------------------
            // Main Component
            // ----------------------------------------------------------------------------------------------------
            vZipLine vPackage = characterObj.GetComponentInChildren<vZipLine>();
            if (vPackage != null)
                DestroyImmediate(vPackage.gameObject);

            GameObject ziplineComponents =
                AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISEditor.MIS_ASSETS_PATH, "Add-ons/Zipline/Prefabs/ZiplineComponents.prefab"));
            ziplineComponents.Instantiate3D(Vector3.zero, characterObj.transform);


            // ----------------------------------------------------------------------------------------------------
            // Animator
            // ----------------------------------------------------------------------------------------------------
            ZiplineBaseLayer();
            ZiplineAnimatorTransitions();
            ZiplinePosition();
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ZiplineBaseLayer()
        {
            // ----------------------------------------------------------------------------------------------------
            // Animation Clips
            // ----------------------------------------------------------------------------------------------------
            AnimationClip zipline_AchoredWaist_Motion =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(MISFeature.MIS_INVECTOR_ZIPLINE_PATH, "Animations/Animations/Zipline@AchoredWaist.anim"));
            AnimationClip zipline_FreeRotation_Motion =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(MISFeature.MIS_INVECTOR_ZIPLINE_PATH, "Animations/Animations/Zipline@FreeRotation.anim"));
            AnimationClip zipline_FreeRotationAiming_Motion =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(MISFeature.MIS_INVECTOR_ZIPLINE_PATH, "Animations/Animations/Zipline@FreeRotation_Aiming.anim"));
            AnimationClip zipline_Hanging_Motion =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(MISFeature.MIS_INVECTOR_ZIPLINE_PATH, "Animations/Animations/Zipline@Hanging.anim"));


            // ----------------------------------------------------------------------------------------------------
            // Base
            // ----------------------------------------------------------------------------------------------------
            SetupBaseLayer();


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions
            // ----------------------------------------------------------------------------------------------------

            // 
            base_ZiplineSM = base_ActionsSM.CreateStateMachineIfNotExist(MISFeature.MIS_PACKAGE_V_ZIPLINE);


            // ----------------------------------------------------------------------------------------------------
            // Base - Action - Zipline
            // ----------------------------------------------------------------------------------------------------

            // FreeRotation
            base_Zipline_FreeRotation = base_ZiplineSM.CreateStateIfNotExist("Zipline@FreeRotation", zipline_FreeRotation_Motion);

            // vAnimatorTag
            if (!base_Zipline_FreeRotation.TryGetStateMachineBehaviour(out vAnimatorTag freeRotationAnimatorTag))
                freeRotationAnimatorTag = base_Zipline_FreeRotation.AddStateMachineBehaviour<vAnimatorTag>();
            freeRotationAnimatorTag.tags = freeRotationAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            freeRotationAnimatorTag.tags = freeRotationAnimatorTag.tags.AddStringIfNotExist(TAG_LOCK_MOVEMENT);
            freeRotationAnimatorTag.tags = freeRotationAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_IK);


            // FreeRotation_Aiming
            base_Zipline_FreeRotationAiming = base_ZiplineSM.CreateStateIfNotExist("Zipline@FreeRotation_Aiming", zipline_FreeRotationAiming_Motion);

            // vAnimatorTag
            if (!base_Zipline_FreeRotationAiming.TryGetStateMachineBehaviour(out vAnimatorTag freeRotationAimingAnimatorTag))
                freeRotationAimingAnimatorTag = base_Zipline_FreeRotationAiming.AddStateMachineBehaviour<vAnimatorTag>();
            freeRotationAimingAnimatorTag.tags = freeRotationAimingAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            freeRotationAimingAnimatorTag.tags = freeRotationAimingAnimatorTag.tags.AddStringIfNotExist(TAG_LOCK_MOVEMENT);


            // Hanging
            base_Zipline_Hanging = base_ZiplineSM.CreateStateIfNotExist("Zipline@Hanging", zipline_Hanging_Motion);

            // vAnimatorTag
            if (!base_Zipline_Hanging.TryGetStateMachineBehaviour(out vAnimatorTag hangingAnimatorTag))
                hangingAnimatorTag = base_Zipline_Hanging.AddStateMachineBehaviour<vAnimatorTag>();
            hangingAnimatorTag.tags = hangingAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            hangingAnimatorTag.tags = hangingAnimatorTag.tags.AddStringIfNotExist(TAG_LOCK_MOVEMENT);
            hangingAnimatorTag.tags = hangingAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_IK);


            // AnchoredWaist
            base_Zipline_AnchoredWaist = base_ZiplineSM.CreateStateIfNotExist("Zipline@AnchoredWaist", zipline_AchoredWaist_Motion);

            // vAnimatorTag
            if (!base_Zipline_AnchoredWaist.TryGetStateMachineBehaviour(out vAnimatorTag anchoredWaistAnimatorTag))
                anchoredWaistAnimatorTag = base_Zipline_AnchoredWaist.AddStateMachineBehaviour<vAnimatorTag>();
            anchoredWaistAnimatorTag.tags = anchoredWaistAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            anchoredWaistAnimatorTag.tags = anchoredWaistAnimatorTag.tags.AddStringIfNotExist(TAG_LOCK_MOVEMENT);
            anchoredWaistAnimatorTag.tags = anchoredWaistAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_IK);


            // AnchoredWaist_Aiming
            base_Zipline_AnchoredWaistAiming = base_ZiplineSM.CreateStateIfNotExist("Zipline@AnchoredWaist_Aiming", zipline_AchoredWaist_Motion);

            // vAnimatorTag
            if (!base_Zipline_AnchoredWaistAiming.TryGetStateMachineBehaviour(out vAnimatorTag anchoredWaistAimingAnimatorTag))
                anchoredWaistAimingAnimatorTag = base_Zipline_AnchoredWaistAiming.AddStateMachineBehaviour<vAnimatorTag>();
            anchoredWaistAimingAnimatorTag.tags = anchoredWaistAimingAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            anchoredWaistAimingAnimatorTag.tags = anchoredWaistAimingAnimatorTag.tags.AddStringIfNotExist(TAG_LOCK_MOVEMENT);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ZiplineAnimatorTransitions()
        {
            // ----------------------------------------------------------------------------------------------------
            // Base Layer
            // ----------------------------------------------------------------------------------------------------

            // ----------------------------------------------------------------------------------------------------
            // Base - Actions
            // ----------------------------------------------------------------------------------------------------
            base_ActionsSM.AddExitTransitionIfNotExist(base_ZiplineSM, null);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - Zipline
            // ----------------------------------------------------------------------------------------------------

            // FreeRotation to FreeRotation_Aiming
            AnimatorStateTransition freeRotationToFreeRotationAiming = base_Zipline_FreeRotation.AddTransitionIfNotExist(base_Zipline_FreeRotationAiming, null, false, 0f, true, 0.25f);
            freeRotationToFreeRotationAiming.mute = true;


            // FreeRotation_Aiming to FreeRotation
            AnimatorStateTransition freeRotationAimingToFreeRotation = base_Zipline_FreeRotationAiming.AddTransitionIfNotExist(base_Zipline_FreeRotation, null, false, 0f, true, 0.25f);
            freeRotationAimingToFreeRotation.mute = true;


            // AnchoredWaist to AnchoredWaist_Aiming
            AnimatorStateTransition AnchoredWaistToAnchoredWaistAiming = base_Zipline_AnchoredWaist.AddTransitionIfNotExist(base_Zipline_AnchoredWaistAiming, null, false, 0f, true, 0.25f);
            AnchoredWaistToAnchoredWaistAiming.mute = true;


            // AnchoredWaist_Aiming to AnchoredWaist
            AnimatorStateTransition AnchoredWaistAimingToAnchoredWaist = base_Zipline_AnchoredWaistAiming.AddTransitionIfNotExist(base_Zipline_AnchoredWaist, null, false, 0f, true, 0.25f);
            AnchoredWaistAimingToAnchoredWaist.mute = true;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ZiplinePosition()
        {
            // ----------------------------------------------------------------------------------------------------
            // Base Layer
            // ----------------------------------------------------------------------------------------------------


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions
            // ----------------------------------------------------------------------------------------------------
            base_ActionsSM.SetStateMachineRelativePosition(base_ZiplineSM, 10, 4);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - Zipline
            // ----------------------------------------------------------------------------------------------------
            base_ZiplineSM.SetDefaultLayerAllPosition();

            base_ZiplineSM.SetStateRelativePosition(base_Zipline_FreeRotation, 0f, -1f);
            base_ZiplineSM.SetStateRelativePosition(base_Zipline_FreeRotationAiming, 3f, -1f);
            base_ZiplineSM.SetStateRelativePosition(base_Zipline_AnchoredWaist, 0f, 0f);
            base_ZiplineSM.SetStateRelativePosition(base_Zipline_AnchoredWaistAiming, 3f, 0f);
            base_ZiplineSM.SetStateRelativePosition(base_Zipline_Hanging, 0f, 1f);
        }
    }
}