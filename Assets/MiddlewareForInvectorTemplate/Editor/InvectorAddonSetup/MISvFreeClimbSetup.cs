using Invector;
using Invector.vCharacterController.vActions;
using Invector.vEventSystems;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
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

        // Base Layer
        AnimatorStateMachine base_FreeClimbSM;
        AnimatorState base_EnterClimbGrounded;
        AnimatorState base_EnterClimbAir;

        AnimatorState base_ClimbJump;
        AnimatorState base_ClimbWall;
        AnimatorState base_ClimbUpWall;

        AnimatorState base_ExitGrounded;
        AnimatorState base_ExitAir;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void FreeClimbSetup(mvSetupOption setupOption, GameObject characterObj, GameObject cameraObj)
        {
#if MIS_INVECTOR_FREECLIMB
            // ----------------------------------------------------------------------------------------------------
            // Setup Options
            // ----------------------------------------------------------------------------------------------------


            // ----------------------------------------------------------------------------------------------------
            // Main Component

            // ----------------------------------------------------------------------------------------------------
            // ClimbHandTarget
            GameObject climbHandTargetObj;
            var climbHandTargetTransform = invectorComponentsParentObj.transform.Find("ClimbHandTarget");
            if (climbHandTargetTransform == null)
            {
                GameObject climbHandTargetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISEditor.MIS_ASSETS_PATH, "Add-ons/FreeClimb/Prefabs/ClimbHandTarget.prefab"));
                climbHandTargetObj = climbHandTargetPrefab.Instantiate3D(new Vector3(0f, 1.515f, 0.4f), invectorComponentsParentObj.transform);
            }
            else
            {
                climbHandTargetObj = climbHandTargetTransform.gameObject;
            }


            // ----------------------------------------------------------------------------------------------------
            // vFreeClimb
            if (characterObj.TryGetComponent(out vFreeClimb prevPackage))
                DestroyImmediate(prevPackage);

            if (characterObj.TryGetComponent(out mvFreeClimb package) == false)
            {
                package = characterObj.AddComponent<mvFreeClimb>();

                package.handTarget = climbHandTargetObj.transform;
                package.climbSurfaceLayers = 1 << MISRuntimeTagLayer.LAYER_DEFAULT;

                package.obstacleLayers = 1 << MISRuntimeTagLayer.LAYER_DEFAULT
                    | 1 << MISRuntimeTagLayer.LAYER_ENEMY
                    | 1 << MISRuntimeTagLayer.LAYER_COMPANION_AI
                    | 1 << MISRuntimeTagLayer.LAYER_STOPMOVE
                    | 1 << MISRuntimeTagLayer.LAYER_PUSHABLE;

                if (MISEditorTagLayer.HasUnityLayer(MISEditorTagLayer.LAYER_VEHICLE))
                    package.obstacleLayers |= 1 << MISRuntimeTagLayer.LAYER_VEHICLE;
                
                package.climbEnterSpeed = 2f;
                package.climbEnterMaxDistance = 1f;
                package.lastPointDistanceH = 0.4f;
                package.lastPointDistanceVUp = 0.1f;
                package.lastPointDistanceVDown = 1.25f;
                package.offsetHandTarget = 0f;
                package.offsetBase = 0.35f;
                package.climbUpMinThickness = 0.3f;
                package.climbUpMinSpace = 0.5f;
                package.offsetHandPositionL = new Vector3(0f, -0.05f, -0.05f);
                package.offsetHandPositionR = new Vector3(0f, -0.05f, -0.05f);
            }


            // Foot Triggers
            vBodySnappingControl bodySnappingControl = invectorComponentsParentObj.transform.GetBodySnappingControl();

            SphereCollider leftFootTriggerCollider;
            SphereCollider rightFootTriggerCollider;

            var leftFootTransform = bodySnappingControl.GetBone("LeftFoot");
            if (leftFootTransform.GetComponentInChildren<vFootStepTrigger>())
            {
                leftFootTriggerCollider = leftFootTransform.GetComponentInChildren<vFootStepTrigger>().gameObject.GetComponentInChildren<SphereCollider>();
            }
            else
            {
                GameObject leftFootTriggerObj = SetFootTrigger(bodySnappingControl, "LeftFoot");
                leftFootTriggerCollider = leftFootTriggerObj.GetComponent<SphereCollider>();
            }

            var rightFootTransform = bodySnappingControl.GetBone("RightFoot");
            if (rightFootTransform.GetComponentInChildren<vFootStepTrigger>())
            {
                rightFootTriggerCollider = rightFootTransform.GetComponentInChildren<vFootStepTrigger>().gameObject.GetComponentInChildren<SphereCollider>();
            }
            else
            {
                GameObject rightFootTriggerObj = SetFootTrigger(bodySnappingControl, "RightFoot");
                rightFootTriggerCollider = rightFootTriggerObj.GetComponent<SphereCollider>();
            }


            // ----------------------------------------------------------------------------------------------------
            // vFreeClimb Event

            // onEnterClimb
            if (package.onEnterClimb == null)
                package.onEnterClimb = new UnityEvent();

            package.onEnterClimb.RemoveMissingPersistents();

            if (package.onEnterClimb.HasPersistent(leftFootTriggerCollider, leftFootTriggerCollider.GetType(), leftFootTriggerCollider.GetType().GetProperty("enabled").GetSetMethod(), typeof(bool)) == false)
            {
                UnityAction<bool> enabledDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), leftFootTriggerCollider, leftFootTriggerCollider.GetType().GetProperty("enabled").GetSetMethod()) as UnityAction<bool>;
                UnityEventTools.AddBoolPersistentListener(package.onEnterClimb, enabledDelegate, false);
            }

            if (package.onEnterClimb.HasPersistent(rightFootTriggerCollider, rightFootTriggerCollider.GetType(), rightFootTriggerCollider.GetType().GetProperty("enabled").GetSetMethod(), typeof(bool)) == false)
            {
                UnityAction<bool> enabledDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), rightFootTriggerCollider, rightFootTriggerCollider.GetType().GetProperty("enabled").GetSetMethod()) as UnityAction<bool>;
                UnityEventTools.AddBoolPersistentListener(package.onEnterClimb, enabledDelegate, false);
            }


            // onExitClimb
            if (package.onExitClimb == null)
                package.onExitClimb = new UnityEvent();

            package.onExitClimb.RemoveMissingPersistents();

            if (package.onExitClimb.HasPersistent(leftFootTriggerCollider, leftFootTriggerCollider.GetType(), leftFootTriggerCollider.GetType().GetProperty("enabled").GetSetMethod(), typeof(bool)) == false)
            {
                UnityAction<bool> enabledDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), leftFootTriggerCollider, leftFootTriggerCollider.GetType().GetProperty("enabled").GetSetMethod()) as UnityAction<bool>;
                UnityEventTools.AddBoolPersistentListener(package.onExitClimb, enabledDelegate, true);
            }

            if (package.onExitClimb.HasPersistent(rightFootTriggerCollider, rightFootTriggerCollider.GetType(), rightFootTriggerCollider.GetType().GetProperty("enabled").GetSetMethod(), typeof(bool)) == false)
            {
                UnityAction<bool> enabledDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), rightFootTriggerCollider, rightFootTriggerCollider.GetType().GetProperty("enabled").GetSetMethod()) as UnityAction<bool>;
                UnityEventTools.AddBoolPersistentListener(package.onExitClimb, enabledDelegate, true);
            }


            // ----------------------------------------------------------------------------------------------------
            // Animator
            // ----------------------------------------------------------------------------------------------------
            FreeClimbParameters();
            FreeClimbBaseLayer();
            FreeClimbAnimatorTransitions();
            FreeClimbPosition();
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void FreeClimbParameters()
        {
            if (!animatorController.parameters.HasParameter(PARAM_IS_FREECLIMB))
                animatorController.AddParameter(PARAM_IS_FREECLIMB, AnimatorControllerParameterType.Bool);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void FreeClimbBaseLayer()
        {
            // ----------------------------------------------------------------------------------------------------
            // Animation Clips
            // ----------------------------------------------------------------------------------------------------

            // ClimbGrounded
            List<AnimationClip> climbUpClipList = 
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_FREECLIMB_PATH, "Animations/VbotClimb/ClimbUp.fbx"));
            var climbUpClip = climbUpClipList.FindClip("Vbot_ClimbUp");

            // ClimbAir
            List<AnimationClip> bracedHangClipList = 
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_FREECLIMB_PATH, "Animations/VbotClimb/V-bot@Highpoly@Braced Hang.fbx"));
            var bracedHangUpClip = bracedHangClipList.FindClip("Highpoly@Braced Hang0");

            // ClimbWall
            List<AnimationClip> hangingIdleClipList = 
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_FREECLIMB_PATH, "Animations/VbotClimb/V-bot@Highpoly@Hanging Idle.fbx"));
            var hangingIdleClip = hangingIdleClipList.FindClip("Highpoly@Hanging Idle");

            List<AnimationClip> hangShimmyClipList = 
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_FREECLIMB_PATH, "Animations/VbotClimb/V-bot@Highpoly@Braced Hang Shimmy(1).fbx"));
            var hangShimmyClip = hangShimmyClipList.FindClip("Highpoly@Braced Hang Shimmy(1)");

            List<AnimationClip> climbingUpWall1ClipList = 
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_FREECLIMB_PATH, "Animations/VbotClimb/V-bot@Highpoly@Climbing Up Wall(1).fbx"));
            var climbingUpWall1Clip = climbingUpWall1ClipList.FindClip("Highpoly@Climbing Up Wall(1)");

            List<AnimationClip> climbingDownWall1ClipList = 
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_FREECLIMB_PATH, "Animations/VbotClimb/V-bot@Highpoly@Climbing Down Wall(1).fbx"));
            var climbingDownWall1Clip = climbingDownWall1ClipList.FindClip("Highpoly@Climbing Down Wall(1)");

            // ClimbJump
            List<AnimationClip> bracedHangDropClipList = 
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_FREECLIMB_PATH, "Animations/VbotClimb/V-bot@Highpoly@Braced Hang Drop(1).fbx"));
            var bracedHangDropClip = bracedHangDropClipList.FindClip("Highpoly@Braced Hang Drop(1)");

            List<AnimationClip> bracedHangHopLeftClipList = 
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_FREECLIMB_PATH, "Animations/VbotClimb/V-bot@Highpoly@Braced Hang Hop Left.fbx"));
            var bracedHangHopLeftClip = bracedHangHopLeftClipList.FindClip("Highpoly@Braced Hang Hop Left");

            List<AnimationClip> bracedHangHopUpClipList = 
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_FREECLIMB_PATH, "Animations/VbotClimb/V-bot@Highpoly@Braced Hang Hop Up.fbx"));
            var bracedHangHopUpClip = bracedHangHopUpClipList.FindClip("Highpoly@Braced Hang Hop Up");

            List<AnimationClip> bracedHangHopRightClipList = 
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_FREECLIMB_PATH, "Animations/VbotClimb/V-bot@Highpoly@Braced Hang Hop Right.fbx"));
            var bracedHangHopRightClip = bracedHangHopRightClipList.FindClip("Highpoly@Braced Hang Hop Right");

            // ClimbUpWall
            List<AnimationClip> hangToCrouchClipList = 
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_FREECLIMB_PATH, "Animations/VbotClimb/V-bot@Highpoly@Braced Hang To Crouch(1).fbx"));
            var hangToCrouchClip = hangToCrouchClipList.FindClip("Highpoly@Braced Hang To Crouch(1)");

            // ExitAir
            List<AnimationClip> basicActionsClipList = 
                GetClipList(Path.Combine(MISEditor.INVECTOR_ASSETS_PATH, "Basic Locomotion/3D Models/Animations/Basic_Actions.fbx"));
            var fallingClip = basicActionsClipList.FindClip("Falling");

            // ExitGrounded
            var ladderEnterBottomClip = basicActionsClipList.FindClip("Ladder_EnterBottom");


            // ----------------------------------------------------------------------------------------------------
            // Base - Action - FreeClimb
            base_FreeClimbSM = base_ActionsSM.CreateStateMachineIfNotExist(MISFeature.MIS_PACKAGE_V_FREECLIMB);
            base_ActionsSM.AddExitTransitionIfNotExist(base_FreeClimbSM, null);


            // Base - Action - FreeClimb - EnterClimbGrounded
            base_EnterClimbGrounded = base_FreeClimbSM.CreateStateIfNotExist("EnterClimbGrounded", climbUpClip);


            // Base - Action - FreeClimb - EnterClimbAir
            base_EnterClimbAir = base_FreeClimbSM.CreateStateIfNotExist("EnterClimbAir", bracedHangUpClip);

            // vAnimatorTag
            if (!base_EnterClimbAir.TryGetStateMachineBehaviour(out vAnimatorTag base_EnterClimbAirAnimatorTag))
                base_EnterClimbAirAnimatorTag = base_EnterClimbAir.AddStateMachineBehaviour<vAnimatorTag>();

            base_EnterClimbAirAnimatorTag.tags = base_EnterClimbAirAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_EnterClimbAirAnimatorTag.tags = base_EnterClimbAirAnimatorTag.tags.AddStringIfNotExist(TAG_CUSTOM_ACTION);


            // Base - Action - FreeClimb - ClimbJump
            base_ClimbJump = base_FreeClimbSM.FindState("ClimbJump");

            if (base_ClimbJump == null)
            {
                base_ClimbJump = base_FreeClimbSM.CreateBlendTree("ClimbJump", out BlendTree blendTree);
                blendTree.blendType = BlendTreeType.FreeformCartesian2D;
                blendTree.blendParameter = PARAM_INPUT_HORIZONTAL;
                blendTree.blendParameterY = PARAM_INPUT_VERTICAL;

                blendTree.useAutomaticThresholds = false;
                blendTree.AddChild(bracedHangDropClip, new Vector2(0f, -1f));
                blendTree.AddChild(bracedHangHopLeftClip, new Vector2(-1f, 0f));
                blendTree.AddChild(bracedHangHopUpClip, new Vector2(0f, 1f));
                blendTree.AddChild(bracedHangHopRightClip, new Vector2(1f, 0f));

                base_ClimbJump.motion = blendTree;
                base_ClimbJump.iKOnFeet = true;
            }

            // vAnimatorTag
            if (!base_ClimbJump.TryGetStateMachineBehaviour(out vAnimatorTag base_ClimbJumpAnimatorTag))
                base_ClimbJumpAnimatorTag = base_ClimbJump.AddStateMachineBehaviour<vAnimatorTag>();

            base_ClimbJumpAnimatorTag.tags = base_ClimbJumpAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_ClimbJumpAnimatorTag.tags = base_ClimbJumpAnimatorTag.tags.AddStringIfNotExist(TAG_CUSTOM_ACTION);


            // Base - Action - FreeClimb - ClimbWall
            base_ClimbWall = base_FreeClimbSM.FindState("ClimbWall");

            if (base_ClimbWall == null)
            {
                base_ClimbWall = base_FreeClimbSM.CreateBlendTree("ClimbWall", out BlendTree blendTree);
                blendTree.blendType = BlendTreeType.FreeformCartesian2D;
                blendTree.blendParameter = PARAM_INPUT_HORIZONTAL;
                blendTree.blendParameterY = PARAM_INPUT_VERTICAL;

                blendTree.useAutomaticThresholds = false;
                blendTree.AddChild(hangingIdleClip, new Vector2(0f, 0f));
                blendTree.AddChild(hangShimmyClip, new Vector2(-1f, 0f));
                blendTree.AddChild(hangShimmyClip, new Vector2(1f, 0f));
                blendTree.AddChild(climbingUpWall1Clip, new Vector2(0f, 1f));
                blendTree.AddChild(climbingDownWall1Clip, new Vector2(0f, -1f));

                ChildMotion[] climbWallBlendTreeChildren = blendTree.children;
                climbWallBlendTreeChildren[1].timeScale = -1.3f;
                climbWallBlendTreeChildren[2].timeScale = 1.3f;
                blendTree.children = climbWallBlendTreeChildren;

                base_ClimbWall.motion = blendTree;
                base_ClimbWall.iKOnFeet = false;
                base_ClimbWall.speed = 1.5f;
            }

            // vAnimatorTag
            if (!base_ClimbWall.TryGetStateMachineBehaviour(out vAnimatorTag base_ClimbWallAnimatorTag))
                base_ClimbWallAnimatorTag = base_ClimbWall.AddStateMachineBehaviour<vAnimatorTag>();

            base_ClimbWallAnimatorTag.tags = base_ClimbWallAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_ClimbWallAnimatorTag.tags = base_ClimbWallAnimatorTag.tags.AddStringIfNotExist(TAG_CUSTOM_ACTION);


            // Base - Action - FreeClimb - ClimbUpWall
            base_ClimbUpWall = base_FreeClimbSM.CreateStateIfNotExist("ClimbUpWall", hangToCrouchClip, true);

            // vAnimatorTag
            if (!base_ClimbUpWall.TryGetStateMachineBehaviour(out vAnimatorTag base_ClimbUpWallAnimatorTag))
                base_ClimbUpWallAnimatorTag = base_ClimbUpWall.AddStateMachineBehaviour<vAnimatorTag>();

            base_ClimbUpWallAnimatorTag.tags = base_ClimbUpWallAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_ClimbUpWallAnimatorTag.tags = base_ClimbUpWallAnimatorTag.tags.AddStringIfNotExist(TAG_CUSTOM_ACTION);


            // Base - Action - FreeClimb - ExitGrounded
            base_ExitGrounded = base_FreeClimbSM.CreateStateIfNotExist("ExitGrounded", ladderEnterBottomClip);


            // Base - Action - FreeClimb - ExitAir
            base_ExitAir = base_FreeClimbSM.CreateStateIfNotExist("ExitAir", fallingClip);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void FreeClimbAnimatorTransitions()
        {
            conditionList.Clear();


            // ----------------------------------------------------------------------------------------------------
            // Base Layer - Action - FreeClimb

            // EnterClimbGrounded To ClimbWall
            base_EnterClimbGrounded.AddTransitionIfNotExist(base_ClimbWall, null, true, 0.21f, true, 0.13f, 0.53f);

            // EnterClimbAir To ClimbWall
            base_EnterClimbAir.AddTransitionIfNotExist(base_ClimbWall, null, true, 0.57f, true, 0.25f, 0f);

            // ClimbJump To ClimbWall
            base_ClimbJump.AddTransitionIfNotExist(base_ClimbWall, null, true, 0.82f, true, 0.25f, 0f);

            // ClimbUpWall To Exit
            base_ClimbUpWall.AddExitTransitionIfNotExist(null, true, 0.77f, true, 0.25f, 0f);

            // ExitGrounded To Exit
            base_ExitGrounded.AddExitTransitionIfNotExist(null, true, 0.1f, true, 0.2f, 0f);

            // ExitAir To Exit
            base_ExitAir.AddExitTransitionIfNotExist(null, true, 0.57f, true, 0.25f, 0f);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void FreeClimbPosition()
        {
            // ----------------------------------------------------------------------------------------------------
            // Base Layer
            // ----------------------------------------------------------------------------------------------------


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions
            base_ActionsSM.SetStateMachineRelativePosition(base_FreeClimbSM, 10, 1);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - FreeClimb
            base_FreeClimbSM.SetDefaultLayerPosition(BASE_LU_POS, BASE_RM_POS);
            base_FreeClimbSM.SetParentStateMachinePosition(ZERO_POS);

            base_FreeClimbSM.SetStateRelativePosition(base_EnterClimbGrounded, 0, -1);
            base_FreeClimbSM.SetStateRelativePosition(base_ClimbWall, 3, -1);
            base_FreeClimbSM.SetStateRelativePosition(base_ClimbJump, 6, -1);
            base_FreeClimbSM.SetStateRelativePosition(base_EnterClimbAir, 3, -2);
            base_FreeClimbSM.SetStateRelativePosition(base_ClimbUpWall, 0, 1);
            base_FreeClimbSM.SetStateRelativePosition(base_ExitGrounded, 3, 1);
            base_FreeClimbSM.SetStateRelativePosition(base_ExitAir, 6, 1);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        GameObject SetFootTrigger(vBodySnappingControl bodySnappingControl, string boneName)
        {
            GameObject footTriggerObj = null;

            vSnapToBody snapToBody = bodySnappingControl.GetSnapToBody(boneName);
            Transform footTriggerTransform = snapToBody.gameObject.transform.Find("FootTriggerHandler/FootTrigger");

            if (footTriggerTransform == null)
            {
                GameObject footTriggerHandlerObj = new GameObject("FootTriggerHandler");
                footTriggerHandlerObj.transform.parent = snapToBody.gameObject.transform;
                footTriggerHandlerObj.transform.localPosition = Vector3.zero;
                footTriggerHandlerObj.transform.localRotation = Quaternion.identity;

                GameObject footTriggerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISEditor.MIS_ASSETS_PATH, "Add-ons/FreeClimb/Prefabs/FootTrigger.prefab"));
                footTriggerObj = footTriggerPrefab.Instantiate3D(new Vector3(0f, 0f, -0.05f), footTriggerHandlerObj.transform);
                footTriggerObj.transform.localRotation = Quaternion.identity;
            }
            else
            {
                footTriggerObj = footTriggerTransform.gameObject;
            }

            return footTriggerObj;
        }
    }
}