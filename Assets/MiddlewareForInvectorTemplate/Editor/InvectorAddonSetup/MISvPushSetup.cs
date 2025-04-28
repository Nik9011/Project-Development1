using Invector.vCharacterController;
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
        AnimatorStateMachine base_PushAndPullSM;

        AnimatorStateMachine base_PushAndPullSM_PushAndPullSM;
        AnimatorState base_StartPushAndPull;
        AnimatorState base_PushAndPull;
        AnimatorState base_StopPushAndPull;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void PushSetup(mvSetupOption setupOption, GameObject characterObj, GameObject cameraObj)
        {
#if MIS_INVECTOR_PUSH
            // ----------------------------------------------------------------------------------------------------
            // Setup Options
            // ----------------------------------------------------------------------------------------------------
            bool invectorPushActionOption = setupOption.HasSetupOption(SetupOption.InvectorPushAction);


            // ----------------------------------------------------------------------------------------------------
            // Main Component
            // ----------------------------------------------------------------------------------------------------
            vPushActionController invectorPackage = null;
            mvPushActionController misPackage = null;

            if (invectorPushActionOption)
            {
                // Setup vPushActionController
                if (!characterObj.TryGetComponent(out invectorPackage))
                {
                    if (characterObj.TryGetComponent(out misPackage) && misPackage.IsTheType<mvPushActionController>())
                        DestroyImmediate(misPackage);

                    invectorPackage = characterObj.AddComponent<vPushActionController>();

                    invectorPackage.startPushPullInput = new GenericInput("Space", "X", "X");
                    invectorPackage.movementSpeed = new Vector2(2f, 1f);
                    invectorPackage.minMovementMagnitude = 0f;
                    invectorPackage.animAcceleration = 10f;
                    invectorPackage.enterCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

                    invectorPackage.pushpullLayer =
                        1 << MISRuntimeTagLayer.LAYER_DEFAULT
                        | 1 << MISRuntimeTagLayer.LAYER_PUSHABLE;

                    invectorPackage.enterPositionSpeed = 2f;
                    invectorPackage.distanceBetweenObject = 0.6f;
                }
            }
            else
            {
                // Setup mvPushActionController
                if (!characterObj.TryGetComponent(out misPackage))
                {
                    if (characterObj.TryGetComponent(out invectorPackage) && invectorPackage.IsTheType<vPushActionController>())
                        DestroyImmediate(invectorPackage);

                    misPackage = characterObj.AddComponent<mvPushActionController>();

                    misPackage.startPushPullInput = new GenericInput("Space", "X", "X");
                    misPackage.movementSpeed = new Vector2(2f, 1f);
                    misPackage.minMovementMagnitude = 0f;
                    misPackage.animAcceleration = 10f;
                    misPackage.enterCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

                    misPackage.pushpullLayer =
                        1 << MISRuntimeTagLayer.LAYER_DEFAULT
                        | 1 << MISRuntimeTagLayer.LAYER_PUSHABLE;

                    misPackage.enterPositionSpeed = 2f;
                    misPackage.distanceBetweenObject = 0.6f;

                    // Raycast
                    misPackage.pushableCaster.useCast = true;
                    misPackage.pushableCaster.backOff = 0f;
                    misPackage.pushableCaster.origin = new Vector3(0f, 0.25f, 0f);
                    misPackage.pushableCaster.radius = 0.2f;
                    misPackage.pushableCaster.maxDistance = 0.5f;
                }
            }


            // ----------------------------------------------------------------------------------------------------
            // PushAndPullUI
            GameObject pushAndPullUIObj;
            var pushAndPullUITransform = invectorComponentsParentObj.transform.Find("UI/PushAndPullUI");
            if (pushAndPullUITransform == null)
            {
                GameObject pushAndPullUIPrefab = 
                    AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISEditor.MIS_ASSETS_PATH, "Add-ons/PushAction/Prefabs/PushAndPullUI.prefab"));
                
                pushAndPullUIObj = pushAndPullUIPrefab.Instantiate2D(Vector3.zero, invectorComponentsParentObj.transform.Find("UI"));
            }
            else
            {
                pushAndPullUIObj = pushAndPullUITransform.gameObject;
            }

            GameObject enterObj = pushAndPullUIObj.transform.Find("Enter").gameObject;
            GameObject exitObj = pushAndPullUIObj.transform.Find("Exit").gameObject;


            // ----------------------------------------------------------------------------------------------------
            // vPushActionController Event
            // ----------------------------------------------------------------------------------------------------

            if (invectorPushActionOption)
            {
                // onStart
                if (invectorPackage.onStart == null)
                    invectorPackage.onStart = new UnityEvent();

                invectorPackage.onStart.RemoveMissingPersistents();

                if (invectorPackage.onStart.HasPersistent(enterObj, enterObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), enterObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(invectorPackage.onStart, setActiveDelegate, false);
                }

                if (invectorPackage.onStart.HasPersistent(exitObj, exitObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), exitObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(invectorPackage.onStart, setActiveDelegate, true);
                }

                // onFinish
                if (invectorPackage.onFinish == null)
                    invectorPackage.onFinish = new UnityEvent();

                invectorPackage.onFinish.RemoveMissingPersistents();

                if (invectorPackage.onFinish.HasPersistent(enterObj, enterObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), enterObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(invectorPackage.onFinish, setActiveDelegate, true);
                }

                if (invectorPackage.onFinish.HasPersistent(exitObj, exitObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), exitObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(invectorPackage.onFinish, setActiveDelegate, false);
                }

                // onFindObject
                if (invectorPackage.onFindObject == null)
                    invectorPackage.onFindObject = new UnityEvent();

                invectorPackage.onFindObject.RemoveMissingPersistents();

                if (invectorPackage.onFindObject.HasPersistent(pushAndPullUIObj, pushAndPullUIObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), pushAndPullUIObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(invectorPackage.onFindObject, setActiveDelegate, true);
                }

                if (invectorPackage.onFindObject.HasPersistent(enterObj, enterObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), enterObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(invectorPackage.onFindObject, setActiveDelegate, true);
                }

                // onLostObject
                if (invectorPackage.onLostObject == null)
                    invectorPackage.onLostObject = new UnityEvent();

                invectorPackage.onLostObject.RemoveMissingPersistents();

                if (invectorPackage.onLostObject.HasPersistent(pushAndPullUIObj, pushAndPullUIObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), pushAndPullUIObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(invectorPackage.onLostObject, setActiveDelegate, false);
                }
            }
            else
            {
                // onStart
                if (misPackage.onStart == null)
                    misPackage.onStart = new UnityEvent();

                misPackage.onStart.RemoveMissingPersistents();

                if (misPackage.onStart.HasPersistent(enterObj, enterObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), enterObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(misPackage.onStart, setActiveDelegate, false);
                }

                if (misPackage.onStart.HasPersistent(exitObj, exitObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), exitObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(misPackage.onStart, setActiveDelegate, true);
                }

                // onFinish
                if (misPackage.onFinish == null)
                    misPackage.onFinish = new UnityEvent();

                misPackage.onFinish.RemoveMissingPersistents();

                if (misPackage.onFinish.HasPersistent(enterObj, enterObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), enterObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(misPackage.onFinish, setActiveDelegate, true);
                }

                if (misPackage.onFinish.HasPersistent(exitObj, exitObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), exitObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(misPackage.onFinish, setActiveDelegate, false);
                }

                // onFindObject
                if (misPackage.onFindObject == null)
                    misPackage.onFindObject = new UnityEvent();

                misPackage.onFindObject.RemoveMissingPersistents();

                if (misPackage.onFindObject.HasPersistent(pushAndPullUIObj, pushAndPullUIObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), pushAndPullUIObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(misPackage.onFindObject, setActiveDelegate, true);
                }

                if (misPackage.onFindObject.HasPersistent(enterObj, enterObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), enterObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(misPackage.onFindObject, setActiveDelegate, true);
                }

                // onLostObject
                if (misPackage.onLostObject == null)
                    misPackage.onLostObject = new UnityEvent();

                misPackage.onLostObject.RemoveMissingPersistents();

                if (misPackage.onLostObject.HasPersistent(pushAndPullUIObj, pushAndPullUIObj.GetType(), "SetActive", null) == false)
                {
                    UnityAction<bool> setActiveDelegate = Delegate.CreateDelegate(typeof(UnityAction<bool>), pushAndPullUIObj, "SetActive") as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(misPackage.onLostObject, setActiveDelegate, false);
                }
            }


            // ----------------------------------------------------------------------------------------------------
            // Animator
            // ----------------------------------------------------------------------------------------------------
            PushActionParameters();
            PushActionBaseLayer();
            PushActionAnimatorTransitions();
            PushActionPosition();
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void PushActionParameters()
        {
            // Do nothing
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void PushActionBaseLayer()
        {
            // ----------------------------------------------------------------------------------------------------
            // Animation Clips
            // ----------------------------------------------------------------------------------------------------
            List<AnimationClip> swimmingIdleClipList = GetClipList(Path.Combine(MISFeature.MIS_INVECTOR_PUSH_PATH, "Resources/vBotForAnim.fbx"));
            AnimationClip pushIdleMotion = swimmingIdleClipList.FindClip("Pushing_Idle");
            AnimationClip pushStartMotion = swimmingIdleClipList.FindClip("Push_Start");
            AnimationClip pushForwardMotion = swimmingIdleClipList.FindClip("Pushing_Forward");
            AnimationClip pushBackMotion = swimmingIdleClipList.FindClip("Pushing_Back");
            AnimationClip pushRightMotion = swimmingIdleClipList.FindClip("Pushing_Right");


            // ----------------------------------------------------------------------------------------------------
            // Base - Action - PushAndPull
            base_PushAndPullSM = base_ActionsSM.CreateStateMachineIfNotExist(MISFeature.MIS_PACKAGE_V_PUSH);
            base_ActionsSM.AddExitTransitionIfNotExist(base_PushAndPullSM, null);


            // ----------------------------------------------------------------------------------------------------
            // Base - Action - PushAndPull - PushAndPull
            base_PushAndPullSM_PushAndPullSM = base_PushAndPullSM.CreateStateMachineIfNotExist(MISFeature.MIS_PACKAGE_V_PUSH);
            base_PushAndPullSM.AddExitTransitionIfNotExist(base_PushAndPullSM_PushAndPullSM, null);


            // Base - Action - PushAndPull - StartPushAndPull
            base_StartPushAndPull = base_PushAndPullSM_PushAndPullSM.CreateStateIfNotExist("StartPushAndPull", pushStartMotion);

            // vAnimatorTag
            if (!base_StartPushAndPull.TryGetStateMachineBehaviour(out vAnimatorTag base_StartPushAndPullAnimatorTag))
                base_StartPushAndPullAnimatorTag = base_StartPushAndPull.AddStateMachineBehaviour<vAnimatorTag>();

            base_StartPushAndPullAnimatorTag.tags = base_StartPushAndPullAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_StartPushAndPullAnimatorTag.tags = base_StartPushAndPullAnimatorTag.tags.AddStringIfNotExist(TAG_CUSTOM_ACTION);


            // Base - Action - PushAndPull - StopPushAndPull
            base_StopPushAndPull = base_PushAndPullSM_PushAndPullSM.CreateStateIfNotExist("StopPushAndPull", pushStartMotion);

            // vAnimatorTag
            if (!base_StopPushAndPull.TryGetStateMachineBehaviour(out vAnimatorTag base_StopPushAndPullAnimatorTag))
                base_StopPushAndPullAnimatorTag = base_StopPushAndPull.AddStateMachineBehaviour<vAnimatorTag>();

            base_StopPushAndPullAnimatorTag.tags = base_StopPushAndPullAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            base_StopPushAndPullAnimatorTag.tags = base_StopPushAndPullAnimatorTag.tags.AddStringIfNotExist(TAG_CUSTOM_ACTION);


            // Base - Action - PushAndPull - PushAndPull
            base_PushAndPull = base_PushAndPullSM_PushAndPullSM.FindState("PushAndPull");

            if (base_PushAndPull == null)
            {
                base_PushAndPull = base_PushAndPullSM_PushAndPullSM.CreateBlendTree("PushAndPull", out BlendTree blendTree);
                blendTree.blendType = BlendTreeType.FreeformDirectional2D;
                blendTree.blendParameter = PARAM_INPUT_HORIZONTAL;
                blendTree.blendParameterY = PARAM_INPUT_VERTICAL;

                blendTree.useAutomaticThresholds = false;
                blendTree.AddChild(pushForwardMotion, new Vector2(0f, 1f));
                blendTree.AddChild(pushIdleMotion, new Vector2(0f, 0f));
                blendTree.AddChild(pushRightMotion, new Vector2(1f, 0f));
                blendTree.AddChild(pushBackMotion, new Vector2(0f, -1f));
                blendTree.AddChild(pushRightMotion, new Vector2(-1f, 0f));

                ChildMotion[] blendTreeChildren = blendTree.children;
                blendTreeChildren[4].mirror = true;
                blendTree.children = blendTreeChildren;

                base_PushAndPull.motion = blendTree;
                base_PushAndPull.iKOnFeet = true;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void PushActionAnimatorTransitions()
        {
            conditionList.Clear();

            // ----------------------------------------------------------------------------------------------------
            // Base - Action - PushAndPull

            // StartPushAndPull To PushAndPull
            base_StartPushAndPull.AddTransitionIfNotExist(base_PushAndPull, null, true, 0.42f, true, 0.29f, 0f);


            // StopPushAndPull To Exit
            base_StopPushAndPull.AddExitTransitionIfNotExist(null, true, 0.4f, true, 0.29f, 0f);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void PushActionPosition()
        {
            // ----------------------------------------------------------------------------------------------------
            // Base Layer
            // ----------------------------------------------------------------------------------------------------


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions
            base_ActionsSM.SetStateMachineRelativePosition(base_PushAndPullSM, 10, 3);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - PushAndPull
            base_PushAndPullSM.SetDefaultLayerAllPosition();
            base_PushAndPullSM.ArrangeStatemachinesStates(0);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - PushAndPull - PushAndPull
            base_PushAndPullSM_PushAndPullSM.SetDefaultLayerAllPosition();

            base_PushAndPullSM_PushAndPullSM.SetStateRelativePosition(base_StartPushAndPull, 0, -1);
            base_PushAndPullSM_PushAndPullSM.SetStateRelativePosition(base_PushAndPull, 0, 0);
            base_PushAndPullSM_PushAndPullSM.SetStateRelativePosition(base_StopPushAndPull, 0, 1);
        }
    }
}