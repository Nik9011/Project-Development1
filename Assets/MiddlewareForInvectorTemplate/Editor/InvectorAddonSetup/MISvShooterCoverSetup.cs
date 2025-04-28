using Invector;
using Invector.vCharacterController;
using Invector.vEventSystems;
using static Invector.vEventSystems.vAnimatorTagAdvanced;
#if INVECTOR_SHOOTER
using Invector.vShooter;
#endif
#if MIS_INVECTOR_SHOOTERCOVER
using Invector.vCover;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
        AnimatorStateMachine base_CoverLocomotionSM;
        AnimatorState base_StartSlideToCover;
        AnimatorState base_Sliding;
        AnimatorState base_FinishSlideToCover;
        AnimatorState base_CoverStanding;
        AnimatorState base_CoverCrouched;
        AnimatorState base_GoToCoverPoint;

        // OnlyArms Layer
        AnimatorStateMachine oa_LocomotionSM;
        AnimatorStateMachine oa_CoverLocomotionSM;
        AnimatorState oa_CoverStanding;
        AnimatorState oa_CoverCrouched;
        AnimatorState oa_GoToCoverPoint;

        // AimingOverBarrier Layer
        AnimatorStateMachine aob_Root;
        AnimatorState aob_Null;
        AnimatorState aob_AimPoseOverBarrier;

        // CoverCorner Layer
        AnimatorStateMachine cvc_Root;
        AnimatorState cvc_Null;
        AnimatorState cvc_AimCornerLeftCrouched;
        AnimatorState cvc_AimCornerRightCrouched;
        AnimatorState cvc_AimCornerLeftStandUp;
        AnimatorState cvc_AimCornerRightStandUp;


        // ----------------------------------------------------------------------------------------------------
        // Position
        // ----------------------------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------------------
        // Cover Locomotion
        Vector3 POS_COVER_LOCOMOTION_START_SLIDE2COVER = new Vector3(300, -(VERTICAL_GAP * 3), 0);
        Vector3 POS_COVER_LOCOMOTION_SLIDING = new Vector3(300, -(VERTICAL_GAP * 2), 0);
        Vector3 POS_COVER_LOCOMOTION_FINISH_SLIDE2COVER = new Vector3(300, -VERTICAL_GAP, 0);
        Vector3 POS_COVER_LOCOMOTION_COVER_CROUCHED = new Vector3(300, VERTICAL_GAP, 0);
        Vector3 POS_COVER_LOCOMOTION_GOTO_COVER_POINT = new Vector3(300, (VERTICAL_GAP * 2), 0);

        // ----------------------------------------------------------------------------------------------------
        // CoverCorner
        Vector3 POS_COVER_COVERCORNER_AIMCORNER_LEFT_CROUCHED = new Vector3(100, -100, 0);
        Vector3 POS_COVER_COVERCORNER_AIMCORNER_RIGHT_CROUCHED = new Vector3(500, -100, 0);
        Vector3 POS_COVER_COVERCORNER_AIMCORNER_LEFT_STANDUP = new Vector3(100, 100, 0);
        Vector3 POS_COVER_COVERCORNER_AIMCORNER_RIGHT_STANDUP = new Vector3(500, 100, 0);


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void ShooterCoverSetup(mvSetupOption setupOption, GameObject characterObj, GameObject cameraObj)
        {
#if MIS_INVECTOR_SHOOTERCOVER
            // ----------------------------------------------------------------------------------------------------
            // Setup Options
            // ----------------------------------------------------------------------------------------------------
            if (templateType != MISEditor.TemplateType.Shooter)
            {
                Debug.LogError("[INVECTOR ShooterCover]This character is not a Shooter.");
                return;
            }


            // ----------------------------------------------------------------------------------------------------
            // Main Component
            // ----------------------------------------------------------------------------------------------------

            // ----------------------------------------------------------------------------------------------------
            // Cover UI Prefab
            GameObject coverUIPrefabObj;
            var coverUIPrefabTransform = invectorComponentsParentObj.transform.Find("Cover UI Prefab");
            if (coverUIPrefabTransform == null)
            {
                GameObject climbHandTargetPrefab =
                    AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISFeature.MIS_INVECTOR_SHOOTERCOVER_PATH, "Prefabs/Cover UI/Cover UI Prefab.prefab"));
                coverUIPrefabObj = climbHandTargetPrefab.Instantiate3D(Vector3.zero, invectorComponentsParentObj.transform);
            }
            else
            {
                coverUIPrefabObj = coverUIPrefabTransform.gameObject;
            }

            // CoverRouteLine LineRenderer
            var coverRouteLine = coverUIPrefabObj.transform.Find("CoverRouteLine").GetComponent<LineRenderer>();

            // CoverUI GameObject
            var coverUIObj = coverUIPrefabObj.transform.Find("CoverUI ").gameObject;

            // CornerUIArrow 
            var cornerUIArrowObj = coverUIPrefabObj.transform.Find("CornerUIArrow").gameObject;

            // LeftArrowPathSlider
            var leftArrowPathSliderObj = coverUIPrefabObj.transform.Find("CornerUIArrow/Canvas/LeftArrowPathSlider").gameObject;
            var leftArrowPathFillImage = leftArrowPathSliderObj.GetComponentInChildren<Image>();

            // RightArrowPathSlider 
            var rightArrowPathSliderObj = coverUIPrefabObj.transform.Find("CornerUIArrow/Canvas/RightArrowPathSlider").gameObject;
            var rightArrowPathFillImage = rightArrowPathSliderObj.GetComponentInChildren<Image>();

            // CornerUIInput
            var cornerUIInputObj = coverUIPrefabObj.transform.Find("CornerUIInput").gameObject;
            var arrowFillImage = cornerUIInputObj.transform.Find("ArrowBG/ArrowFill").GetComponentInChildren<Image>();


            // ----------------------------------------------------------------------------------------------------
            // vDetectionTarget
            if (characterObj.TryGetComponent(out vDetectionTarget detectionTarget) == false)
            {
                detectionTarget = characterObj.AddComponent<vDetectionTarget>();

                vBodySnappingControl bodySnappingControl = invectorComponentsParentObj.transform.GetBodySnappingControl();
                var chestTransform = bodySnappingControl.GetBone("Chest");
                //detectionTarget.center = chestTransform; // read-only

                SerializedObject detectionTargetSO = new SerializedObject(detectionTarget);
                SerializedProperty detectionTargetCenterPointProp = detectionTargetSO.FindProperty("centerPoint");
                detectionTargetCenterPointProp.objectReferenceValue = chestTransform;
                detectionTargetSO.ApplyModifiedProperties();
            }


            // ----------------------------------------------------------------------------------------------------
            // vCoverController
            if (characterObj.TryGetComponent(out vCoverController prevPackage))
                DestroyImmediate(prevPackage);

            if (characterObj.TryGetComponent(out mvCoverController package) == false)
            {
                package = characterObj.AddComponent<mvCoverController>();

                // ----------------------------------------------------------------------------------------------------
                // Cover Settings
                package.enterExitInput = new GenericInput("Space", "A", "A");
                package.autoExitCover = true;
                package.exitInputDirectionAngle = 160f;
                package.exitDirectionTimer = 0f;

                package.nextCoverDistance = new Vector2(4f, 20f);
                package.rayCastAnglePass = 1;
                package.positionOffsetZ = 0.01f;

                package.angleToChangeSide = new Vector2(90f, 135f);

                package.crouchRayRadius = 0.1f;
                package.crouchRayDistance = 1f;
                package.crouchHeight = 1.4f;

                package.useHipFireOverBarrier = true;
                package.hipFireTimer = 0.1f;

                package.checkActionMargin = new Vector3(0.3f, 0.2f, 0.4f);
                package.checkActionOffset = new Vector3(0f, -0.1f, -0.2f);

                // ----------------------------------------------------------------------------------------------------
                // Movement Speed
                package.goToCoverPointRunningMoveSpeed = 4f;
                package.goToCoverPointSprintingMoveSpeed = 6f;

                package.standAnimatorSpeed = 1.2f;
                package.crouchAnimatorSpeed = 1.2f;
                package.goToCornerPointAnimatorSpeed = 1.5f;

                // ----------------------------------------------------------------------------------------------------
                // IK Pose
                package.ikLeftStanding = "CoverStandingLeft";
                package.ikLeftStandingAiming = "CoverStandingAimingLeft";
                package.ikLeftStandingCornerAiming = "CoverStandingCornerAimingLeft";
                package.ikLeftCrouching = "CoverCrouchingLeft";
                package.ikLeftCrouchingAiming = "CoverCrouchingAimingLeft";
                package.ikLeftCrouchingBarrierAiming = "CoverCrouchingBarrierAimingLeft";
                package.ikLeftCrouchingCornerAiming = "CoverCrouchingCornerAimingLeft";
                package.ikRightStanding = "CoverStandingRight";
                package.ikRightStandingAiming = "CoverStandingAimingRight";
                package.ikRightStandingCornerAiming = "CoverStandingCornerAimingRight";
                package.ikRightCrouching = "CoverCrouchingRight";
                package.ikRightCrouchingAiming = "CoverCrouchingAimingRight";
                package.ikRightCrouchingBarrierAiming = "CoverCrouchingBarrierAimingRight";
                package.ikRightCrouchingCornerAiming = "CoverCrouchingCornerAimingRight";

                // ----------------------------------------------------------------------------------------------------
                // Layers and Tags
                package.obstaclesLayer = 1 << MISRuntimeTagLayer.LAYER_DEFAULT
                    | 1 << MISRuntimeTagLayer.LAYER_STOPMOVE;

                package.checkHeightIgnoreNames = new List<string>
                {
                    "glassFrame"
                };
                package.coverLayer = 1 << MISRuntimeTagLayer.LAYER_COVERPOINT;
                package.coverTag = new vTagMask
                {
                    "CoverPoint"
                };

                // ----------------------------------------------------------------------------------------------------
                // Cover UI
                package.routeLineColor = new Color32(135, 135, 135, 28);
                package.drawRouteDelayTime = 0.15f;
                package.routeLine = coverRouteLine;
                package.routeLineSmoothPass = 1;
                package.coverUI = coverUIObj;
                package.cornerUICenter = cornerUIArrowObj;
                package.cornerLeftUI = leftArrowPathSliderObj;
                package.cornerRightUI = rightArrowPathSliderObj;
                package.cornerInputUI = cornerUIInputObj;

                // onUpdateCornerInput Event
                if (package.onUpdateCornerInput == null)
                    package.onUpdateCornerInput = new Slider.SliderEvent();

                package.onUpdateCornerInput.RemoveMissingPersistents();

                if (package.onUpdateCornerInput.HasPersistent(leftArrowPathFillImage, leftArrowPathFillImage.GetType(), leftArrowPathFillImage.GetType().GetProperty("fillAmount").GetSetMethod(), null) == false)
                {
                    UnityAction<float> fillAmountDelegate = Delegate.CreateDelegate(typeof(UnityAction<float>), leftArrowPathFillImage, leftArrowPathFillImage.GetType().GetProperty("fillAmount").GetSetMethod()) as UnityAction<float>;
                    UnityEventTools.AddFloatPersistentListener(package.onUpdateCornerInput, fillAmountDelegate, 0f);
                }

                if (package.onUpdateCornerInput.HasPersistent(rightArrowPathFillImage, rightArrowPathFillImage.GetType(), rightArrowPathFillImage.GetType().GetProperty("fillAmount").GetSetMethod(), null) == false)
                {
                    UnityAction<float> fillAmountDelegate = Delegate.CreateDelegate(typeof(UnityAction<float>), rightArrowPathFillImage, rightArrowPathFillImage.GetType().GetProperty("fillAmount").GetSetMethod()) as UnityAction<float>;
                    UnityEventTools.AddFloatPersistentListener(package.onUpdateCornerInput, fillAmountDelegate, 0f);
                }

                if (package.onUpdateCornerInput.HasPersistent(arrowFillImage, arrowFillImage.GetType(), arrowFillImage.GetType().GetProperty("fillAmount").GetSetMethod(), null) == false)
                {
                    UnityAction<float> fillAmountDelegate = Delegate.CreateDelegate(typeof(UnityAction<float>), arrowFillImage, arrowFillImage.GetType().GetProperty("fillAmount").GetSetMethod()) as UnityAction<float>;
                    UnityEventTools.AddFloatPersistentListener(package.onUpdateCornerInput, fillAmountDelegate, 0f);
                }

                // ----------------------------------------------------------------------------------------------------
                // Animation States
                package.coverStandingLeft = "CoverStanding";
                package.coverStandingRight = "CoverStanding";
                package.coverCrouchedLeft = "CoverCrouched";
                package.coverCrouchedRight = "CoverCrouched";

                package.finishSlideToCoverWaitTime = 0.1f;

                // ----------------------------------------------------------------------------------------------------
                // Camera Settings
                package.aimingOverBarrierCameraState = "AimingOverBarrier";

                // ----------------------------------------------------------------------------------------------------
                // Events

                // onEnterCover
                if (package.onEnterCover == null)
                    package.onEnterCover = new UnityEvent();

                package.onEnterCover.RemoveMissingPersistents();

                if (package.onEnterCover.HasPersistent(detectionTarget, detectionTarget.GetType(), detectionTarget.GetType().GetProperty("DetectionReductionValue").GetSetMethod(), typeof(float)) == false)
                {
                    UnityAction<float> enabledDelegate = Delegate.CreateDelegate(typeof(UnityAction<float>), detectionTarget, detectionTarget.GetType().GetProperty("DetectionReductionValue").GetSetMethod()) as UnityAction<float>;
                    UnityEventTools.AddFloatPersistentListener(package.onEnterCover, enabledDelegate, 6f);
                }

                // onExitCover
                if (package.onExitCover == null)
                    package.onExitCover = new UnityEvent();

                package.onExitCover.RemoveMissingPersistents();

                if (package.onExitCover.HasPersistent(detectionTarget, detectionTarget.GetType(), detectionTarget.GetType().GetProperty("DetectionReductionValue").GetSetMethod(), typeof(float)) == false)
                {
                    UnityAction<float> enabledDelegate = Delegate.CreateDelegate(typeof(UnityAction<float>), detectionTarget, detectionTarget.GetType().GetProperty("DetectionReductionValue").GetSetMethod()) as UnityAction<float>;
                    UnityEventTools.AddFloatPersistentListener(package.onExitCover, enabledDelegate, -6f);
                }

                // onStartGoToCoverPoint
                if (package.onStartGoToCoverPoint == null)
                    package.onStartGoToCoverPoint = new UnityEvent();

                package.onStartGoToCoverPoint.RemoveMissingPersistents();

                if (package.onStartGoToCoverPoint.HasPersistent(detectionTarget, detectionTarget.GetType(), detectionTarget.GetType().GetProperty("DetectionReductionValue").GetSetMethod(), typeof(float)) == false)
                {
                    UnityAction<float> enabledDelegate = Delegate.CreateDelegate(typeof(UnityAction<float>), detectionTarget, detectionTarget.GetType().GetProperty("DetectionReductionValue").GetSetMethod()) as UnityAction<float>;
                    UnityEventTools.AddFloatPersistentListener(package.onStartGoToCoverPoint, enabledDelegate, 3f);
                }

                // onFinishGoToCoverPoint
                if (package.onFinishGoToCoverPoint == null)
                    package.onFinishGoToCoverPoint = new UnityEvent();

                package.onFinishGoToCoverPoint.RemoveMissingPersistents();

                if (package.onFinishGoToCoverPoint.HasPersistent(detectionTarget, detectionTarget.GetType(), detectionTarget.GetType().GetProperty("DetectionReductionValue").GetSetMethod(), typeof(float)) == false)
                {
                    UnityAction<float> enabledDelegate = Delegate.CreateDelegate(typeof(UnityAction<float>), detectionTarget, detectionTarget.GetType().GetProperty("DetectionReductionValue").GetSetMethod()) as UnityAction<float>;
                    UnityEventTools.AddFloatPersistentListener(package.onFinishGoToCoverPoint, enabledDelegate, -3f);
                }
            }


            // ----------------------------------------------------------------------------------------------------
            // vShooterManager
            characterObj.TryGetComponent(out vShooterManager shooterManager);

            // Damage Layers
            shooterManager.blockAimLayer = 1 << MISRuntimeTagLayer.LAYER_DEFAULT
                | 1 << MISRuntimeTagLayer.LAYER_STOPMOVE;

            // Aim
            shooterManager.checkAimRadius = 0.025f;

            // IK Adjust
            shooterManager.smoothArmWeight = 60f;
            shooterManager.alignArmToHitPoint = true;

            // HipFire
            shooterManager.hipfireAimTime = 0.1f;


            // ----------------------------------------------------------------------------------------------------
            // mvShooterMeleeInput
            characterObj.TryGetComponent(out mvShooterMeleeInput shooterMeleeInput);

            // Inputs
            //shooterMeleeInput.jumpInput.useInput = false;


            // ----------------------------------------------------------------------------------------------------
            // vMessageReceiver
            if (characterObj.TryGetComponent(out vMessageReceiver messageReceiver) == false)
                messageReceiver = characterObj.AddComponent<vMessageReceiver>();

            if (messageReceiver.messagesListeners == null)
                messageReceiver.messagesListeners = new List<vMessageReceiver.vMessageListener>();

            // LockPlayer
            var lockPlayerListener = messageReceiver.messagesListeners.Find(x => x.Name.Equals("LockPlayer"));
            if (lockPlayerListener == null)
            {
                lockPlayerListener = new vMessageReceiver.vMessageListener("LockPlayer");
                lockPlayerListener.receiveFromGlobal = true;

                lockPlayerListener.onReceiveMessage = new vMessageReceiver.OnReceiveMessageEvent();
                UnityAction<bool> enableDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), shooterMeleeInput, "SetLockAllInput") as UnityAction<bool>;
                UnityEventTools.AddBoolPersistentListener(lockPlayerListener.onReceiveMessage, enableDelegate, true);

                messageReceiver.messagesListeners.Add(lockPlayerListener);
            }
            else
            {
                lockPlayerListener.onReceiveMessage.RemoveMissingPersistents();

                lockPlayerListener.receiveFromGlobal = true;

                if (lockPlayerListener.onReceiveMessage.HasPersistent(shooterMeleeInput, shooterMeleeInput.GetType(), "SetLockAllInput", typeof(bool)) == false)
                {
                    UnityAction<bool> enableDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), shooterMeleeInput, "SetLockAllInput") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(lockPlayerListener.onReceiveMessage, enableDelegate, true);
                }
            }

            // UnlockPlayer
            var unlockPlayerListener = messageReceiver.messagesListeners.Find(x => x.Name.Equals("UnlockPlayer"));
            if (unlockPlayerListener == null)
            {
                unlockPlayerListener = new vMessageReceiver.vMessageListener("UnlockPlayer");
                unlockPlayerListener.receiveFromGlobal = true;

                unlockPlayerListener.onReceiveMessage = new vMessageReceiver.OnReceiveMessageEvent();
                UnityAction<bool> enableDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), shooterMeleeInput, "SetLockAllInput") as UnityAction<bool>;
                UnityEventTools.AddBoolPersistentListener(unlockPlayerListener.onReceiveMessage, enableDelegate, false);

                messageReceiver.messagesListeners.Add(unlockPlayerListener);
            }
            else
            {
                unlockPlayerListener.onReceiveMessage.RemoveMissingPersistents();

                unlockPlayerListener.receiveFromGlobal = true;

                if (unlockPlayerListener.onReceiveMessage.HasPersistent(shooterMeleeInput, shooterMeleeInput.GetType(), "SetLockAllInput", typeof(bool)) == false)
                {
                    UnityAction<bool> enableDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), shooterMeleeInput, "SetLockAllInput") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(unlockPlayerListener.onReceiveMessage, enableDelegate, false);
                }
            }


            // ----------------------------------------------------------------------------------------------------
            // Animator
            // ----------------------------------------------------------------------------------------------------
            ShooterCoverParameters();
            ShooterCoverBaseLayer();
            ShooterCoverOnlyArmsLayer();
            ShooterCoverAimingOverBarrierLayer();
            ShooterCoverCoverCornerLayer();
            ShooterCoverAnimatorTransitions();
            ShooterCoverPosition();
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ShooterCoverParameters()
        {
            if (!animatorController.parameters.HasParameter(PARAM_COVERSIDE))
                animatorController.AddParameter(PARAM_COVERSIDE, AnimatorControllerParameterType.Float);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ShooterCoverBaseLayer()
        {
            // ----------------------------------------------------------------------------------------------------
            // Animation Clips
            // ----------------------------------------------------------------------------------------------------
            List<AnimationClip> coverAnimationsClipList =
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_SHOOTERCOVER_PATH, "Animations/Cover/Humanoid/CoverAnimations.fbx"));
            var slideForwardDownStartClip = coverAnimationsClipList.FindClip("SlideForwardDownStart");
            var slideForwardDownLoopClip = coverAnimationsClipList.FindClip("SlideForwardDownLoop");
            var slideForwardDownFinishClip = coverAnimationsClipList.FindClip("SlideForwardDownFinish");

            var coverIdleStand_LClip = coverAnimationsClipList.FindClip("Cover Idle Stand_L");
            var coverWalkStand_LClip = coverAnimationsClipList.FindClip("Cover Walk Stand_L");
            var coverIdleStand_RClip = coverAnimationsClipList.FindClip("Cover Idle Stand_R");
            var coverWalkStand_RClip = coverAnimationsClipList.FindClip("Cover Walk Stand_R");

            var coverIdleCrouch_LClip = coverAnimationsClipList.FindClip("Cover Idle Crouch_L");
            var coverCrouchWalk_LClip = coverAnimationsClipList.FindClip("Cover_Crouch_Walk_L");
            var coverIdleCrouch_RClip = coverAnimationsClipList.FindClip("Cover Idle Crouch_R");
            var coverCrouchWalk_RClip = coverAnimationsClipList.FindClip("Cover_Crouch_Walk_R");

            var sprintCoveredClip = coverAnimationsClipList.FindClip("Sprint_Covered");


            List<AnimationClip> basicFreeMovementClipList =
                GetClipList(Path.Combine(MISEditor.INVECTOR_ASSETS_PATH, "Basic Locomotion/3D Models/Animations/Basic_FreeMovement.fbx"));
            var runClip = basicFreeMovementClipList.FindClip("Run");


            List<AnimationClip> basicStrafeMovesetClipList =
                GetClipList(Path.Combine(MISEditor.INVECTOR_ASSETS_PATH, "Basic Locomotion/3D Models/Animations/Basic_StrafeMoveset.fbx"));
            var runRightClip = basicStrafeMovesetClipList.FindClip("RunRight");
            var runBackClip = basicStrafeMovesetClipList.FindClip("RunBack");
            var runForwardRightClip = basicStrafeMovesetClipList.FindClip("RunForwardRight");
            var runBackRightClip = basicStrafeMovesetClipList.FindClip("RunBackRight");


            // ----------------------------------------------------------------------------------------------------
            // Base - Locomotion - Cover Locomotion
            // ----------------------------------------------------------------------------------------------------
            base_CoverLocomotionSM = base_LocomotionSM.CreateStateMachineIfNotExist("Cover Locomotion");

            // vAnimatorTag
            if (!base_CoverLocomotionSM.TryGetStateMachineBehaviour(out vAnimatorTag base_CoverLocomotionSMAnimatorTag))
                base_CoverLocomotionSMAnimatorTag = base_CoverLocomotionSM.AddStateMachineBehaviour<vAnimatorTag>();

            base_CoverLocomotionSMAnimatorTag.tags = base_CoverLocomotionSMAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_CoverLocomotionSMAnimatorTag.tags = base_CoverLocomotionSMAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_HEADTRACK);
            base_CoverLocomotionSMAnimatorTag.tags = base_CoverLocomotionSMAnimatorTag.tags.AddStringIfNotExist(TAG_COVER_LOCOMOTION);


            // ----------------------------------------------------------------------------------------------------
            // Base - Locomotion - Cover Locomotion - CoverStanding
            base_CoverStanding = base_CoverLocomotionSM.FindState("CoverStanding");

            if (base_CoverStanding == null)
            {
                base_CoverStanding = base_CoverLocomotionSM.CreateBlendTree("CoverStanding", out BlendTree blendTree);
                blendTree.blendType = BlendTreeType.Simple1D;
                blendTree.blendParameter = PARAM_COVERSIDE;

                blendTree.useAutomaticThresholds = false;

                // LeftSide BlendTree
                animatorController.CreateBlendTreeOnly("LeftSide", out BlendTree coverStandingLeftSideBT);
                coverStandingLeftSideBT.blendType = BlendTreeType.Simple1D;
                coverStandingLeftSideBT.blendParameter = PARAM_INPUT_MAGNITUDE;

                coverStandingLeftSideBT.useAutomaticThresholds = false;

                coverStandingLeftSideBT.AddChild(coverIdleStand_LClip, 0f);
                coverStandingLeftSideBT.AddChild(coverWalkStand_LClip, 0.5f);

                // RightSide BlendTree
                animatorController.CreateBlendTreeOnly("RightSide", out BlendTree coverStandingRightSideBT);
                coverStandingRightSideBT.blendType = BlendTreeType.Simple1D;
                coverStandingRightSideBT.blendParameter = PARAM_INPUT_MAGNITUDE;

                coverStandingRightSideBT.useAutomaticThresholds = false;

                coverStandingRightSideBT.AddChild(coverIdleStand_RClip, 0f);
                coverStandingRightSideBT.AddChild(coverWalkStand_RClip, 0.5f);

                // 
                blendTree.AddChild(coverStandingLeftSideBT, -1);
                blendTree.AddChild(coverStandingRightSideBT, 1);

                base_CoverStanding.motion = blendTree;
                base_CoverStanding.iKOnFeet = true;
            }


            // ----------------------------------------------------------------------------------------------------
            // Base - Locomotion - Cover Locomotion - StartSlideToCover
            base_StartSlideToCover = base_CoverLocomotionSM.CreateStateIfNotExist("Start Slide To Cover", slideForwardDownStartClip);

            // vAnimatorTagAdvanced
            if (!base_StartSlideToCover.TryGetStateMachineBehaviour(out vAnimatorTagAdvanced base_StartSlideToCoverAnimatorTagAdvanced))
                base_StartSlideToCoverAnimatorTagAdvanced = base_StartSlideToCover.AddStateMachineBehaviour<vAnimatorTagAdvanced>();

            base_StartSlideToCoverAnimatorTagAdvanced.RemoveAnimatorTagAdvancedIfExist(TAG_CUSTOM_ACTION);
            base_StartSlideToCoverAnimatorTagAdvanced.AddAnimatorTagAdvancedIfNotExist(TAG_SLIDE_TO_COVER, vAnimatorEventTriggerType.EnterStateExitState, Vector2.zero);
            base_StartSlideToCoverAnimatorTagAdvanced.AddAnimatorTagAdvancedIfNotExist(TAG_IGNORE_IK, vAnimatorEventTriggerType.EnterStateExitState, Vector2.zero);

            // vTriggerSoundByState
            base_StartSlideToCover.AddvTriggerSoundByState(
                new List<AudioClip>
                {
                    AssetDatabase.LoadAssetAtPath<AudioClip>(Path.Combine(MISFeature.MIS_INVECTOR_SHOOTERCOVER_PATH, "Sounds/slideFx.wav"))
                });


            // ----------------------------------------------------------------------------------------------------
            // Base - Locomotion - Cover Locomotion - Sliding
            base_Sliding = base_CoverLocomotionSM.CreateStateIfNotExist("Sliding", slideForwardDownLoopClip);

            // vAnimatorTagAdvanced
            if (!base_Sliding.TryGetStateMachineBehaviour(out vAnimatorTagAdvanced base_SlidingAnimatorTagAdvanced))
                base_SlidingAnimatorTagAdvanced = base_Sliding.AddStateMachineBehaviour<vAnimatorTagAdvanced>();

            base_SlidingAnimatorTagAdvanced.RemoveAnimatorTagAdvancedIfExist(TAG_CUSTOM_ACTION);
            base_SlidingAnimatorTagAdvanced.AddAnimatorTagAdvancedIfNotExist(TAG_SLIDE_TO_COVER, vAnimatorEventTriggerType.EnterStateExitState, Vector2.zero);
            base_SlidingAnimatorTagAdvanced.AddAnimatorTagAdvancedIfNotExist(TAG_IGNORE_IK, vAnimatorEventTriggerType.EnterStateExitState, Vector2.zero);


            // ----------------------------------------------------------------------------------------------------
            // Base - Locomotion - Cover Locomotion - Finish Slide To Cover
            base_FinishSlideToCover = base_CoverLocomotionSM.CreateStateIfNotExist("Finish Slide To Cover", slideForwardDownFinishClip);

            // vAnimatorTagAdvanced
            if (!base_FinishSlideToCover.TryGetStateMachineBehaviour(out vAnimatorTagAdvanced base_FinishSlideToCoverAnimatorTagAdvanced))
                base_FinishSlideToCoverAnimatorTagAdvanced = base_FinishSlideToCover.AddStateMachineBehaviour<vAnimatorTagAdvanced>();

            base_FinishSlideToCoverAnimatorTagAdvanced.RemoveAnimatorTagAdvancedIfExist(TAG_CUSTOM_ACTION);
            base_FinishSlideToCoverAnimatorTagAdvanced.AddAnimatorTagAdvancedIfNotExist(TAG_SLIDE_TO_COVER, vAnimatorEventTriggerType.EnterStateExitState, Vector2.zero);
            base_FinishSlideToCoverAnimatorTagAdvanced.AddAnimatorTagAdvancedIfNotExist(TAG_IGNORE_IK, vAnimatorEventTriggerType.EnterStateExitState, Vector2.zero);


            // ----------------------------------------------------------------------------------------------------
            // Base - Locomotion - Cover Locomotion - CoverCrouched
            base_CoverCrouched = base_CoverLocomotionSM.FindState("CoverCrouched");

            if (base_CoverCrouched == null)
            {
                base_CoverCrouched = base_CoverLocomotionSM.CreateBlendTree("CoverCrouched", out BlendTree blendTree);
                blendTree.blendType = BlendTreeType.Simple1D;
                blendTree.blendParameter = PARAM_COVERSIDE;

                // LeftSide
                animatorController.CreateBlendTreeOnly("LeftSide", out BlendTree coverCrouchedLeftSideBT);
                coverCrouchedLeftSideBT.blendType = BlendTreeType.Simple1D;
                coverCrouchedLeftSideBT.blendParameter = PARAM_INPUT_MAGNITUDE;

                coverCrouchedLeftSideBT.useAutomaticThresholds = false;

                coverCrouchedLeftSideBT.AddChild(coverIdleCrouch_LClip, 0f);
                coverCrouchedLeftSideBT.AddChild(coverCrouchWalk_LClip, 0.5f);

                // RightSide
                animatorController.CreateBlendTreeOnly("RightSide", out BlendTree coverCrouchedRightSideBT);
                coverCrouchedRightSideBT.blendType = BlendTreeType.Simple1D;
                coverCrouchedRightSideBT.blendParameter = PARAM_INPUT_MAGNITUDE;

                coverCrouchedRightSideBT.useAutomaticThresholds = false;

                coverCrouchedRightSideBT.AddChild(coverIdleCrouch_RClip, 0f);
                coverCrouchedRightSideBT.AddChild(coverCrouchWalk_RClip, 0.5f);

                // 
                blendTree.useAutomaticThresholds = false;
                blendTree.AddChild(coverCrouchedLeftSideBT, -1);
                blendTree.AddChild(coverCrouchedRightSideBT, 1);

                base_CoverCrouched.motion = blendTree;
                base_CoverCrouched.iKOnFeet = true;
            }


            // ----------------------------------------------------------------------------------------------------
            // Base - Locomotion - Cover Locomotion - Go To Cover Point
            base_GoToCoverPoint = base_CoverLocomotionSM.FindState("Go To Cover Point");

            if (base_GoToCoverPoint == null)
            {
                base_GoToCoverPoint = base_CoverLocomotionSM.CreateBlendTree("Go To Cover Point", out BlendTree freeLocomotionBT);
                freeLocomotionBT.blendType = BlendTreeType.FreeformDirectional2D;
                freeLocomotionBT.blendParameter = PARAM_INPUT_HORIZONTAL;
                freeLocomotionBT.blendParameterY = PARAM_INPUT_VERTICAL;

                // Run
                animatorController.CreateBlendTreeOnly("RunForward", out BlendTree runForwardBT);
                runForwardBT.blendType = BlendTreeType.Simple1D;
                runForwardBT.blendParameter = PARAM_INPUT_MAGNITUDE;

                runForwardBT.useAutomaticThresholds = false;

                runForwardBT.AddChild(runClip, 0f);
                runForwardBT.AddChild(sprintCoveredClip, 1.5f);

                freeLocomotionBT.AddChild(runForwardBT, new Vector2(0, 1));
                freeLocomotionBT.AddChild(runRightClip, new Vector2(1, 0));
                freeLocomotionBT.AddChild(runBackClip, new Vector2(0, -1));
                freeLocomotionBT.AddChild(runForwardRightClip, new Vector2(1, 1));
                freeLocomotionBT.AddChild(runBackRightClip, new Vector2(1, -1));
                freeLocomotionBT.AddChild(runBackRightClip, new Vector2(-1, -1));
                freeLocomotionBT.AddChild(runRightClip, new Vector2(-1, 0));
                freeLocomotionBT.AddChild(runForwardRightClip, new Vector2(-1, 1));

                ChildMotion[] freeLocomotionBTChildren = freeLocomotionBT.children;
                freeLocomotionBTChildren[2].timeScale = 1.5f;
                freeLocomotionBTChildren[4].timeScale = 1.5f;
                freeLocomotionBTChildren[5].timeScale = 1.5f;
                freeLocomotionBTChildren[5].mirror = true;
                freeLocomotionBTChildren[6].timeScale = -1f;
                freeLocomotionBTChildren[7].mirror = true;
                freeLocomotionBT.children = freeLocomotionBTChildren;

                base_GoToCoverPoint.motion = freeLocomotionBT;
                base_GoToCoverPoint.iKOnFeet = true;
            }

            // vAnimatorTagAdvanced
            if (!base_GoToCoverPoint.TryGetStateMachineBehaviour(out vAnimatorTagAdvanced base_GoToCoverPointAnimatorTagAdvanced))
                base_GoToCoverPointAnimatorTagAdvanced = base_GoToCoverPoint.AddStateMachineBehaviour<vAnimatorTagAdvanced>();

            base_GoToCoverPointAnimatorTagAdvanced.RemoveAnimatorTagAdvancedIfExist(TAG_CUSTOM_ACTION);
            base_GoToCoverPointAnimatorTagAdvanced.AddAnimatorTagAdvancedIfNotExist(TAG_IGNORE_IK, vAnimatorEventTriggerType.EnterStateExitState, Vector2.zero);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ShooterCoverOnlyArmsLayer()
        {
            //SetupOnlyArmsLayer();


            // ----------------------------------------------------------------------------------------------------
            // Animation Clips
            // ----------------------------------------------------------------------------------------------------
            List<AnimationClip> shooter_UpperBodyPosesClipList = GetClipList(Path.Combine(MISEditor.INVECTOR_ASSETS_PATH, "Shooter/3DModels/Animations/Shooter_UpperBodyPoses.fbx"));
            var idlePistolClip = shooter_UpperBodyPosesClipList.FindClip("Idle@Pistol");
            var idleRifleClip = shooter_UpperBodyPosesClipList.FindClip("Idle@Rifle");
            var idleShotgunClip = shooter_UpperBodyPosesClipList.FindClip("Idle@Shotgun");
            var idleRPGClip = shooter_UpperBodyPosesClipList.FindClip("Idle@RPG");
            List<AnimationClip> bowClipList = GetClipList(Path.Combine(MISEditor.INVECTOR_ASSETS_PATH, "Shooter/Scripts/ArcherySystem/Animations/V-bot@Bow.fbx"));
            var idleBowClip = bowClipList.FindClip("Bow_Idle");

            var crouchIdlePistolClip = shooter_UpperBodyPosesClipList.FindClip("CrouchIdle@Pistol");
            var crouchIdleRifleClip = shooter_UpperBodyPosesClipList.FindClip("CrouchIdle@Rifle");
            var crouchIdleShotgunClip = shooter_UpperBodyPosesClipList.FindClip("CrouchIdle@Shotgun");
            var crouchIdleRPGClip = shooter_UpperBodyPosesClipList.FindClip("CrouchIdle@RPG");
            List<AnimationClip> shooter_ArcheryClipList = GetClipList(Path.Combine(MISEditor.INVECTOR_ASSETS_PATH, "Shooter/3DModels/Animations/Shooter_Archery.fbx"));
            var crouchedIdleBowClip = shooter_ArcheryClipList.FindClip("Bow_Idle");

            var runPistolClip =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(MISFeature.MIS_INVECTOR_SHOOTERCOVER_PATH, "3D Models/Animations/Run_with_Weapon/Run_Pistol.anim"));
            var runHeavyWeaponClip =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(MISFeature.MIS_INVECTOR_SHOOTERCOVER_PATH, "3D Models/Animations/Run_with_Weapon/Run_HeavyWeapon.anim"));


            // ----------------------------------------------------------------------------------------------------
            // OnlyArms - Locomotion - Cover Locomotion - CoverStanding
            var base_CoverStandingMotion = animatorController.GetStateEffectiveMotion(base_CoverStanding, LAYER_ONLY_ARMS);

            if (base_CoverStandingMotion == null || !(base_CoverStandingMotion is BlendTree))
            {
                animatorController.CreateBlendTreeOnly("CoverStanding", out BlendTree blendTree);
                blendTree.blendType = BlendTreeType.Simple1D;
                blendTree.blendParameter = PARAM_UPPERBODY_ID;

                blendTree.useAutomaticThresholds = false;

                blendTree.AddChild(idlePistolClip, 1);
                blendTree.AddChild(idleRifleClip, 2);
                blendTree.AddChild(idleShotgunClip, 3);
                blendTree.AddChild(idleRPGClip, 4);
                blendTree.AddChild(idleBowClip, 5);

                animatorController.SetStateEffectiveMotion(base_CoverStanding, blendTree, LAYER_ONLY_ARMS);
            }

            // ----------------------------------------------------------------------------------------------------
            // OnlyArms - Locomotion - Cover Locomotion - CoverCrouched
            var base_CoverCrouchedMotion = animatorController.GetStateEffectiveMotion(base_CoverCrouched, LAYER_ONLY_ARMS);

            if (base_CoverCrouchedMotion == null || !(base_CoverCrouchedMotion is BlendTree))
            {
                animatorController.CreateBlendTreeOnly("CoverCrouched", out BlendTree blendTree);
                blendTree.blendType = BlendTreeType.Simple1D;
                blendTree.blendParameter = PARAM_UPPERBODY_ID;

                blendTree.useAutomaticThresholds = false;

                blendTree.AddChild(crouchIdlePistolClip, 1);
                blendTree.AddChild(crouchIdleRifleClip, 2);
                blendTree.AddChild(crouchIdleShotgunClip, 3);
                blendTree.AddChild(crouchIdleRPGClip, 4);
                blendTree.AddChild(crouchedIdleBowClip, 5);

                animatorController.SetStateEffectiveMotion(base_CoverCrouched, blendTree, LAYER_ONLY_ARMS);
            }

            // ----------------------------------------------------------------------------------------------------
            // OnlyArms - Locomotion - Cover Locomotion - Go To Cover Point
            var base_GoToCoverPointMotion = animatorController.GetStateEffectiveMotion(base_GoToCoverPoint, LAYER_ONLY_ARMS);

            if (base_GoToCoverPointMotion == null || !(base_GoToCoverPointMotion is BlendTree))
            {
                animatorController.CreateBlendTreeOnly("Go To Cover Point", out BlendTree gotoCoverPointBT);
                gotoCoverPointBT.blendType = BlendTreeType.Simple1D;
                gotoCoverPointBT.blendParameter = PARAM_UPPERBODY_ID;

                // Run
                animatorController.CreateBlendTreeOnly("Run", out BlendTree runBT);
                runBT.blendType = BlendTreeType.Simple1D;
                runBT.blendParameter = PARAM_INPUT_MAGNITUDE;

                runBT.useAutomaticThresholds = false;
                runBT.AddChild(runPistolClip, 1);

                // Run Heavy
                animatorController.CreateBlendTreeOnly("RunHeavy", out BlendTree runHeavyBT);
                runHeavyBT.blendType = BlendTreeType.Simple1D;
                runHeavyBT.blendParameter = PARAM_INPUT_MAGNITUDE;

                runHeavyBT.useAutomaticThresholds = false;
                runHeavyBT.AddChild(runHeavyWeaponClip, 1);

                // 
                gotoCoverPointBT.useAutomaticThresholds = false;
                gotoCoverPointBT.AddChild(runBT, 1);
                gotoCoverPointBT.AddChild(runHeavyBT, 2);

                animatorController.SetStateEffectiveMotion(base_GoToCoverPoint, gotoCoverPointBT, LAYER_ONLY_ARMS);
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ShooterCoverAimingOverBarrierLayer()
        {
            // ----------------------------------------------------------------------------------------------------
            // Animation Clips
            // ----------------------------------------------------------------------------------------------------
            List<AnimationClip> coverAnimationsClipList =
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_SHOOTERCOVER_PATH, "Animations/Cover/Humanoid/CoverAnimations.fbx"));
            var aimedCrouchIdleOverBarrierClip = coverAnimationsClipList.FindClip("AimedCrouchIdle_OverBarrier");
            var aimedCrouchWalkRightOverBarrierClip = coverAnimationsClipList.FindClip("AimedCrouchWalkRight_OverBarrier");


            // ----------------------------------------------------------------------------------------------------
            // AimingOverBarrier Layer Root
            // ----------------------------------------------------------------------------------------------------
            aob_Root = animatorController.FindRootStateMachineInLayers(LAYER_AIMING_OVER_BARRIER);
            if (aob_Root == null)
            {
                aob_Root = animatorController.CreateLayer(
                    LAYER_AIMING_OVER_BARRIER,
                    0f,
                    AssetDatabase.LoadAssetAtPath<AvatarMask>(Path.Combine(MISEditor.INVECTOR_ASSETS_PATH, MASK_UPPER_BODY_PATH)),
                    AnimatorLayerBlendingMode.Override,
                    false);

                AssetDatabase.Refresh();
            }

            if (animatorController.IsTheLastLayer(aob_Root))
                animatorController.MoveUpLayer(animatorController.layers.Length - 1);


            // ----------------------------------------------------------------------------------------------------
            // AimingOverBarrier - Null
            aob_Null = aob_Root.CreateStateIfNotExist(STATE_NULL, null);
            aob_Root.defaultState = aob_Null;


            // ----------------------------------------------------------------------------------------------------
            // AimingOverBarrier - Aim Pose Over Barrier
            // ----------------------------------------------------------------------------------------------------
            aob_AimPoseOverBarrier = aob_Root.FindState("Aim Pose Over Barrier");

            if (aob_AimPoseOverBarrier == null)
            {
                aob_AimPoseOverBarrier = aob_Root.CreateBlendTree("Aim Pose Over Barrier", out BlendTree aimPoseOverBarrierBT);
                aimPoseOverBarrierBT.blendType = BlendTreeType.Simple1D;
                aimPoseOverBarrierBT.blendParameter = PARAM_INPUT_MAGNITUDE;

                aimPoseOverBarrierBT.useAutomaticThresholds = false;

                aimPoseOverBarrierBT.AddChild(aimedCrouchIdleOverBarrierClip, 0f);
                aimPoseOverBarrierBT.AddChild(aimedCrouchWalkRightOverBarrierClip, 1f);

                aob_AimPoseOverBarrier.motion = aimPoseOverBarrierBT;
                aob_AimPoseOverBarrier.iKOnFeet = false;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ShooterCoverCoverCornerLayer()
        {
            // ----------------------------------------------------------------------------------------------------
            // Animation Clips
            // ----------------------------------------------------------------------------------------------------
            List<AnimationClip> coverAnimationsClipList =
                GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_SHOOTERCOVER_PATH, "Animations/Cover/Humanoid/CoverAnimations.fbx"));
            var aimedCrouchIdleCornerLeftClip = coverAnimationsClipList.FindClip("AimedCrouchIdle_Corner_Left");
            var aimedCrouchIdleCornerRightClip = coverAnimationsClipList.FindClip("AimedCrouchIdle_Corner_Right");
            var aimedIdleCornerLeftClip = coverAnimationsClipList.FindClip("AimedIdle_Corner_Left");
            var aimedIdleCornerRightClip = coverAnimationsClipList.FindClip("AimedIdle_Corner_Right");


            // ----------------------------------------------------------------------------------------------------
            // CoverCorner Layer
            // ----------------------------------------------------------------------------------------------------
            cvc_Root = animatorController.FindRootStateMachineInLayers(LAYER_COVER_CORNER);
            if (cvc_Root == null)
            {
                cvc_Root = animatorController.CreateLayer(
                    LAYER_COVER_CORNER,
                    0f,
                    AssetDatabase.LoadAssetAtPath<AvatarMask>(Path.Combine(MISEditor.INVECTOR_ASSETS_PATH, MASK_CORNER)),
                    AnimatorLayerBlendingMode.Override,
                    false);

                AssetDatabase.Refresh();
            }

            if (animatorController.IsTheLastLayer(cvc_Root))
                animatorController.MoveUpLayer(animatorController.layers.Length - 1);


            // AimingOverBarrier - Null
            cvc_Null = cvc_Root.CreateStateIfNotExist(STATE_NULL, null);
            cvc_Root.defaultState = cvc_Null;


            // AimingOverBarrier - Aim Corner Left Crouched
            cvc_AimCornerLeftCrouched = cvc_Root.CreateStateIfNotExist("Aim Corner Left Crouched", aimedCrouchIdleCornerLeftClip, true, 2f);


            // AimingOverBarrier - Aim Corner Right Crouched
            cvc_AimCornerRightCrouched = cvc_Root.CreateStateIfNotExist("Aim Corner Right Crouched", aimedCrouchIdleCornerRightClip, true, 2f);


            // AimingOverBarrier - Aim Corner Left StandUp
            cvc_AimCornerLeftStandUp = cvc_Root.CreateStateIfNotExist("Aim Corner Left StandUp", aimedIdleCornerLeftClip, true, 2f);


            // AimingOverBarrier - Aim Corner Right StandUp
            cvc_AimCornerRightStandUp = cvc_Root.CreateStateIfNotExist("Aim Corner Right StandUp", aimedIdleCornerRightClip, true, 2f);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ShooterCoverAnimatorTransitions()
        {
            // ----------------------------------------------------------------------------------------------------
            // Base Layer
            // ----------------------------------------------------------------------------------------------------

            // ----------------------------------------------------------------------------------------------------
            // Cover Locomotion

            // Start Slide To Cover To Sliding
            base_StartSlideToCover.AddTransitionIfNotExist(base_Sliding, null, true, 0.9f, true, 0.1f);

            // CoverStanding To CoverCrouched
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_IS_CROUCHING, AnimatorConditionMode.If, 0f)); // true
            base_CoverStanding.AddTransitionIfNotExist(base_CoverCrouched, conditionList, false, 0.92f, true, 0.25f, 0.92f);

            // CoverCrouched To CoverStanding
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_IS_CROUCHING, AnimatorConditionMode.IfNot, 0f)); // false
            base_CoverCrouched.AddTransitionIfNotExist(base_CoverStanding, conditionList, false, 0.92f, true, 0.25f, 0.92f);


            // ----------------------------------------------------------------------------------------------------
            // AimingOverBarrier Layer
            // ----------------------------------------------------------------------------------------------------

            // Null To Aim Pose Over Barrier
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_IS_AIMING, AnimatorConditionMode.If, 0f)); // true
            conditionList.Add(Condition(PARAM_CANAIM, AnimatorConditionMode.If, 0f)); // true
            aob_Null.AddTransitionIfNotExist(aob_AimPoseOverBarrier, conditionList, false, 0.75f, true, 0.01f, 0f);

            // Aim Pose Over Barrier To Null 1
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_CANAIM, AnimatorConditionMode.IfNot, 0f)); // false
            aob_AimPoseOverBarrier.AddTransitionIfNotExist(aob_Null, conditionList, false, 0.25f, true, 0.1f);

            // Aim Pose Over Barrier To Null 2
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_IS_AIMING, AnimatorConditionMode.IfNot, 0f)); // false
            aob_AimPoseOverBarrier.AddTransitionIfNotExist(aob_Null, conditionList, false, 0.25f, true, 0.1f);


            // ----------------------------------------------------------------------------------------------------
            // CoverCorner Layer
            // ----------------------------------------------------------------------------------------------------

            // Aim Corner Left Crouched To Null
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_COVERSIDE, AnimatorConditionMode.Greater, -0.1f));
            cvc_AimCornerLeftCrouched.AddTransitionIfNotExist(cvc_Null, conditionList, false, 0.75f, true, 0.25f);

            // Aim Corner Left Crouched To Aim Corner Left StandUp
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_IS_CROUCHING, AnimatorConditionMode.IfNot, 0f));  // false
            cvc_AimCornerLeftCrouched.AddTransitionIfNotExist(cvc_AimCornerLeftStandUp, conditionList, false, 0.75f, true, 0.2f);

            // Null To Aim Corner Left Crouched
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_COVERSIDE, AnimatorConditionMode.Less, -0.1f));
            conditionList.Add(Condition(PARAM_IS_CROUCHING, AnimatorConditionMode.If, 0f));  // true
            cvc_Null.AddTransitionIfNotExist(cvc_AimCornerLeftCrouched, conditionList, false, 0.75f, true, 0.2f);


            // Aim Corner Right Crouched To Null
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_COVERSIDE, AnimatorConditionMode.Less, 0.1f));
            cvc_AimCornerRightCrouched.AddTransitionIfNotExist(cvc_Null, conditionList, false, 0.75f, true, 0.25f);

            // Aim Corner Right Crouched To Aim Corner Right StandUp
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_IS_CROUCHING, AnimatorConditionMode.IfNot, 0f));  // false
            cvc_AimCornerRightCrouched.AddTransitionIfNotExist(cvc_AimCornerRightStandUp, conditionList, false, 0.75f, true, 0.2f);

            // Null To Aim Corner Right Crouched
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_COVERSIDE, AnimatorConditionMode.Greater, 0.1f));
            conditionList.Add(Condition(PARAM_IS_CROUCHING, AnimatorConditionMode.If, 0f));  // true
            cvc_Null.AddTransitionIfNotExist(cvc_AimCornerRightCrouched, conditionList, false, 0.75f, true, 0.2f);


            // Aim Corner Left StandUp To Null
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_COVERSIDE, AnimatorConditionMode.Greater, -0.1f));
            cvc_AimCornerLeftStandUp.AddTransitionIfNotExist(cvc_Null, conditionList, false, 0.75f, true, 0.25f);

            // Aim Corner Left StandUp To Aim Corner Left Crouched
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_IS_CROUCHING, AnimatorConditionMode.If, 0f));  // true
            cvc_AimCornerLeftStandUp.AddTransitionIfNotExist(cvc_AimCornerLeftCrouched, conditionList, false, 0.8f, true, 0.2f);

            // Null To Aim Corner Left StandUp
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_COVERSIDE, AnimatorConditionMode.Less, -0.1f));
            conditionList.Add(Condition(PARAM_IS_CROUCHING, AnimatorConditionMode.IfNot, 0f));  // false
            cvc_Null.AddTransitionIfNotExist(cvc_AimCornerLeftStandUp, conditionList, false, 0.75f, true, 0.2f);


            // Aim Corner Right StandUp To Null
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_COVERSIDE, AnimatorConditionMode.Less, 0.1f));
            cvc_AimCornerRightStandUp.AddTransitionIfNotExist(cvc_Null, conditionList, false, 0.75f, true, 0.25f);

            // Aim Corner Right StandUp To Aim Corner Right Crouched
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_IS_CROUCHING, AnimatorConditionMode.If, 0f));  // true
            cvc_AimCornerRightStandUp.AddTransitionIfNotExist(cvc_AimCornerRightCrouched, conditionList, false, 0.8f, true, 0.2f);

            // Null To Aim Corner Right StandUp
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_COVERSIDE, AnimatorConditionMode.Greater, 0.1f));
            conditionList.Add(Condition(PARAM_IS_CROUCHING, AnimatorConditionMode.IfNot, 0f));  // false
            cvc_Null.AddTransitionIfNotExist(cvc_AimCornerRightStandUp, conditionList, false, 0.75f, true, 0.2f);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ShooterCoverPosition()
        {
            // ----------------------------------------------------------------------------------------------------
            // Base Layer
            // ----------------------------------------------------------------------------------------------------

            Vector3 freeLocomotionPosition = base_LocomotionSM.GetStateMachinePosition(base_FreeLocomotionSM);
            base_LocomotionSM.SetStateMachinePosition(base_CoverLocomotionSM, freeLocomotionPosition + (Vector3.up * -VERTICAL_GAP));

            // ----------------------------------------------------------------------------------------------------
            // 
            base_CoverLocomotionSM.SetDefaultLayerAllPosition();

            base_CoverLocomotionSM.SetStateRelativePosition(base_StartSlideToCover, 0, -2);
            base_CoverLocomotionSM.SetStateRelativePosition(base_Sliding, 3, -1.5f);
            base_CoverLocomotionSM.SetStateRelativePosition(base_FinishSlideToCover, 0, -1);

            base_CoverLocomotionSM.SetStateRelativePosition(base_GoToCoverPoint, 0, 0);

            base_CoverLocomotionSM.SetStateRelativePosition(base_CoverStanding, 0, 1);
            base_CoverLocomotionSM.SetStateRelativePosition(base_CoverCrouched, 3, 1);


            // ----------------------------------------------------------------------------------------------------
            // AimingOverBarrier Layer
            // ----------------------------------------------------------------------------------------------------

            aob_Root.SetDefaultLayerPosition();

            aob_Root.SetStateRelativePosition(aob_Null, 0, -1);
            aob_Root.SetStateRelativePosition(aob_AimPoseOverBarrier, 0, 0);


            // ----------------------------------------------------------------------------------------------------
            // CoverCorner Layer
            // ----------------------------------------------------------------------------------------------------

            cvc_Root.SetDefaultLayerPosition(BASE_LU_POS, BASE_LM_POS);

            cvc_Root.SetStateRelativePosition(cvc_Null, 2, 0);
            cvc_Root.SetStateRelativePosition(cvc_AimCornerLeftCrouched, 0, -1);
            cvc_Root.SetStateRelativePosition(cvc_AimCornerRightCrouched, 4, -1);
            cvc_Root.SetStateRelativePosition(cvc_AimCornerLeftStandUp, 0, 1);
            cvc_Root.SetStateRelativePosition(cvc_AimCornerRightStandUp, 4, 1);
        }
    }
}