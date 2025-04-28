using Invector.Utils;
using Invector.vEventSystems;
using static Invector.vEventSystems.vAnimatorEvent;
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
        AnimatorStateMachine base_ParachuteSM;
        AnimatorState base_OpenParachute;
        AnimatorState base_Parachute;
        AnimatorState base_ExitParachute;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void ParachuteSetup(mvSetupOption setupOption, GameObject characterObj, GameObject cameraObj)
        {
#if MIS_INVECTOR_PARACHUTE
            // ----------------------------------------------------------------------------------------------------
            // Setup Options
            // ----------------------------------------------------------------------------------------------------


            // ----------------------------------------------------------------------------------------------------
            // Main Component
            // ----------------------------------------------------------------------------------------------------
            GameObject parachuteObj;
            var parachuteTransform = invectorComponentsParentObj.transform.Find("Parachute");
            if (parachuteTransform == null)
            {
                GameObject parachutePrefab =
                    AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISFeature.MIS_INVECTOR_PARACHUTE_PATH, "Prefabs/Parachute.prefab"));
                parachuteObj = parachutePrefab.Instantiate3D(Vector3.zero, invectorComponentsParentObj.transform);
            }
            else
            {
                parachuteObj = parachuteTransform.gameObject;
            }


            // ----------------------------------------------------------------------------------------------------
            // Parachute
            GameObject parachuteControllerObj = parachuteObj.transform.Find("ParachuteController").gameObject;

            if (parachuteControllerObj.TryGetComponent(out vParachuteController prevPackage))
                DestroyImmediate(prevPackage);

            if (parachuteControllerObj.TryGetComponent(out mvParachuteController package) == false)
            {
                package = parachuteControllerObj.AddComponent<mvParachuteController>();

                package.waterLayerMask = 1 << MISRuntimeTagLayer.LAYER_TRIGGERS;

                // Input/Movement
                package.parachutePivot = parachuteControllerObj.GetComponent<Rigidbody>();
                package.parachuteTilt = parachuteControllerObj.transform.Find("ParachuteTilAndCollision").gameObject;
                package.minHeightToOpenParachute = 10f;
                package.minTimeToReOpen = 0.5f;

                // Vertical Movement
                package.dragPitchBack = 9f;
                package.dragPitchForward = 5f;

                // Horizontal Movement
                package.acceleration = 2f;
                package.speedPitchBack = 5f;
                package.speedPitchForward = 3f;

                // Camera
                package.cameraTarget = parachuteControllerObj.transform;
            }


            // ----------------------------------------------------------------------------------------------------
            // Events
            GameObject parachuteMeshRootObj = parachuteObj.transform.Find("vParachute").gameObject;
            GameObject parachuteMeshObj = parachuteMeshRootObj.transform.Find("Parachute").gameObject;
            GameObject parachuteBeltMeshObj = parachuteMeshRootObj.transform.Find("ParachuteBelt").gameObject;
            Animator parachuteAnimator = parachuteMeshRootObj.GetComponent<Animator>();
            vEventWithDelay eventWithDelay = parachuteControllerObj.GetComponent<vEventWithDelay>();


            // onCanUseParachuteEnable
            if (package.onCanUseParachuteEnable == null)
                package.onCanUseParachuteEnable = new UnityEngine.Events.UnityEvent();

            package.onCanUseParachuteEnable.RemoveMissingPersistents();

            if (package.onCanUseParachuteEnable.HasPersistent(parachuteBeltMeshObj, parachuteBeltMeshObj.GetType(), "SetActive", typeof(bool)) == false)
            {
                UnityAction<bool> packageDelegate = System.Delegate.CreateDelegate(typeof(UnityAction<bool>), parachuteBeltMeshObj, "SetActive") as UnityAction<bool>;
                UnityEventTools.AddBoolPersistentListener(package.onCanUseParachuteEnable, packageDelegate, true);
            }


            // onCanUseParachuteDisable
            if (package.onCanUseParachuteDisable == null)
                package.onCanUseParachuteDisable = new UnityEngine.Events.UnityEvent();

            package.onCanUseParachuteDisable.RemoveMissingPersistents();

            if (package.onCanUseParachuteDisable.HasPersistent(parachuteBeltMeshObj, parachuteBeltMeshObj.GetType(), "SetActive", typeof(bool)) == false)
            {
                UnityAction<bool> packageDelegate = System.Delegate.CreateDelegate(typeof(UnityAction<bool>), parachuteBeltMeshObj, "SetActive") as UnityAction<bool>;
                UnityEventTools.AddBoolPersistentListener(package.onCanUseParachuteDisable, packageDelegate, false);
            }


            // onStartOpen
            if (package.onStartOpen == null)
                package.onStartOpen = new UnityEngine.Events.UnityEvent();

            package.onStartOpen.RemoveMissingPersistents();

            if (package.onStartOpen.HasPersistent(parachuteAnimator, parachuteAnimator.GetType(), "Play", typeof(string)) == false)
            {
                UnityAction<string> packageDelegate = System.Delegate.CreateDelegate(typeof(UnityAction<string>), parachuteAnimator, "Play") as UnityAction<string>;
                UnityEventTools.AddStringPersistentListener(package.onStartOpen, packageDelegate, "Open");
            }

            if (package.onStartOpen.HasPersistent(parachuteMeshObj, parachuteMeshObj.GetType(), "SetActive", typeof(bool)) == false)
            {
                UnityAction<bool> packageDelegate = System.Delegate.CreateDelegate(typeof(UnityAction<bool>), parachuteMeshObj, "SetActive") as UnityAction<bool>;
                UnityEventTools.AddBoolPersistentListener(package.onStartOpen, packageDelegate, true);
            }


            // onClose
            if (package.onClose == null)
                package.onClose = new UnityEngine.Events.UnityEvent();

            package.onClose.RemoveMissingPersistents();

            if (package.onClose.HasPersistent(parachuteAnimator, parachuteAnimator.GetType(), "Play", typeof(string)) == false)
            {
                UnityAction<string> packageDelegate = System.Delegate.CreateDelegate(typeof(UnityAction<string>), parachuteAnimator, "Play") as UnityAction<string>;
                UnityEventTools.AddStringPersistentListener(package.onClose, packageDelegate, "Close");
            }

            if (package.onClose.HasPersistent(eventWithDelay, eventWithDelay.GetType(), "DoEvents", null) == false)
            {
                UnityAction packageDelegate = System.Delegate.CreateDelegate(typeof(UnityAction), eventWithDelay, "DoEvents") as UnityAction;
                UnityEventTools.AddVoidPersistentListener(package.onClose, packageDelegate);
            }


            // mvThirdPersonController
            if (characterObj.TryGetComponent(out mvThirdPersonController cc))
            {
                if (package.groundDetectionDistance <= package.minHeightToOpenParachute)
                {
                    package.groundDetectionDistance = package.minHeightToOpenParachute + 1f;
                    Debug.LogWarning("groundDetectionDistance of mvThirdPersonController must be higher than minHeightToOpenParachute of vParachuteControllers. MIS Parachute Setup has changed its value for you. Please check it later.");
                }
            }


            // ----------------------------------------------------------------------------------------------------
            // Animator
            // ----------------------------------------------------------------------------------------------------
            ParachuteParameters();
            ParachuteBaseLayer();
            ParachuteAnimatorTransitions();
            ParachutePosition();
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ParachuteParameters()
        {
            // Do nothing
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ParachuteBaseLayer()
        {
            // ----------------------------------------------------------------------------------------------------
            // Animation Clips
            // ----------------------------------------------------------------------------------------------------
            var parachutingBackClip =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(MISFeature.MIS_INVECTOR_PARACHUTE_PATH, "Anims/Parachuting_Back.anim"));
            var parachutingForwardClip =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(MISFeature.MIS_INVECTOR_PARACHUTE_PATH, "Anims/Parachuting_Forward.anim"));
            var parachutingIdleClip =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(MISFeature.MIS_INVECTOR_PARACHUTE_PATH, "Anims/Parachuting_Idle.anim"));
            var parachutingLeftClip =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(MISFeature.MIS_INVECTOR_PARACHUTE_PATH, "Anims/Parachuting_Left.anim"));
            var parachutingOpenClip =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(MISFeature.MIS_INVECTOR_PARACHUTE_PATH, "Anims/Parachuting_Open.anim"));

            List<AnimationClip> basicActionsClipList =
                GetClipList(Path.Combine(MISEditor.INVECTOR_ASSETS_PATH, "Basic Locomotion/3D Models/Animations/Basic_Actions.fbx"));
            var rollClip = basicActionsClipList.FindClip("Roll");


            // ----------------------------------------------------------------------------------------------------
            // Base - Action
            // ----------------------------------------------------------------------------------------------------
            SetupBaseLayer();


            // ----------------------------------------------------------------------------------------------------
            // Base - Action - Parachute
            base_ParachuteSM = base_ActionsSM.CreateStateMachineIfNotExist(MISFeature.MIS_PACKAGE_V_PARACHUTE);
            base_ActionsSM.AddExitTransitionIfNotExist(base_ParachuteSM, null);


            // Base - Action - Parachute - OpenParachute
            base_OpenParachute = base_ParachuteSM.CreateStateIfNotExist("OpenParachute", parachutingOpenClip);


            // Base - Action - Parachute - ExitParachute
            base_ExitParachute = base_ParachuteSM.CreateStateIfNotExist("ExitParachute", rollClip);

            // vAnimatorTag
            if (!base_ExitParachute.TryGetStateMachineBehaviour(out vAnimatorTag base_ExitParachuteAnimatorTag))
                base_ExitParachuteAnimatorTag = base_ExitParachute.AddStateMachineBehaviour<vAnimatorTag>();

            base_ExitParachuteAnimatorTag.tags = base_ExitParachuteAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_ExitParachuteAnimatorTag.tags = base_ExitParachuteAnimatorTag.tags.AddStringIfNotExist(TAG_CUSTOM_ACTION);


            // Base - Action - Parachute - Parachute
            base_Parachute = base_ParachuteSM.FindState("Parachute");

            if (base_Parachute == null)
            {
                base_Parachute = base_ParachuteSM.CreateBlendTree("Parachute", out BlendTree blendTree);
                blendTree.blendType = BlendTreeType.SimpleDirectional2D;
                blendTree.blendParameter = PARAM_INPUT_HORIZONTAL;
                blendTree.blendParameterY = PARAM_INPUT_VERTICAL;

                blendTree.useAutomaticThresholds = false;
                blendTree.AddChild(parachutingBackClip, new Vector2(0f, -1f));
                blendTree.AddChild(parachutingIdleClip, new Vector2(0f, 0f));
                blendTree.AddChild(parachutingForwardClip, new Vector2(0f, 1f));
                blendTree.AddChild(parachutingLeftClip, new Vector2(-1f, 0f));
                blendTree.AddChild(parachutingLeftClip, new Vector2(1f, 0f));

                ChildMotion[] blendTreeChildren = blendTree.children;
                blendTreeChildren[4].mirror = true;
                blendTree.children = blendTreeChildren;

                base_Parachute.motion = blendTree;
                base_Parachute.iKOnFeet = false;
            }


            // vAnimatorEvent
            if (!base_Parachute.TryGetStateMachineBehaviour(out vAnimatorEvent base_ParachuteAnimatorEvent))
                base_ParachuteAnimatorEvent = base_Parachute.AddStateMachineBehaviour<vAnimatorEvent>();

            vAnimatorEventTrigger base_ParachuteEnableParachuteAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "EnableParachute",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.EnterState
            };
            vAnimatorEventTrigger base_ParachuteDisableParachuteAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "DisableParachute",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.ExitState
            };

            if (base_ParachuteAnimatorEvent.eventTriggers == null)
                base_ParachuteAnimatorEvent.eventTriggers = new List<vAnimatorEventTrigger>();

            if (base_ParachuteAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(base_ParachuteEnableParachuteAnimatorEventTrigger.eventName)) == null)
                base_ParachuteAnimatorEvent.eventTriggers.Add(base_ParachuteEnableParachuteAnimatorEventTrigger);

            if (base_ParachuteAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(base_ParachuteDisableParachuteAnimatorEventTrigger.eventName)) == null)
                base_ParachuteAnimatorEvent.eventTriggers.Add(base_ParachuteDisableParachuteAnimatorEventTrigger);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ParachuteAnimatorTransitions()
        {
            // ----------------------------------------------------------------------------------------------------
            // Base Layer - Action - Parachute

            // OpenParachute To Parachute
            base_OpenParachute.AddTransitionIfNotExist(base_Parachute, null, true, 0.57f, true, 0.25f, 0f);

            // OpenParachute Tt
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_ACTIONSTATE, AnimatorConditionMode.Equals, 0f));
            base_OpenParachute.AddExitTransitionIfNotExist(conditionList, false, 0.57f, true, 0.25f, 0f);


            // Parachute To Exit
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_ACTIONSTATE, AnimatorConditionMode.Equals, 0f));
            base_Parachute.AddExitTransitionIfNotExist(conditionList, false, 0.93f, true, 0.25f, 0f);


            // ExitParachute To Exit
            base_ExitParachute.AddExitTransitionIfNotExist(null, true, 0.85f, true, 0.25f, 0f);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ParachutePosition()
        {
            // ----------------------------------------------------------------------------------------------------
            // Base Layer
            // ----------------------------------------------------------------------------------------------------


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions
            base_ActionsSM.SetStateMachineRelativePosition(base_ParachuteSM, 10, 2);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - Parachute
            base_ParachuteSM.SetDefaultLayerAllPosition();
            base_ParachuteSM.ArrangeStates(0);
        }
    }
}