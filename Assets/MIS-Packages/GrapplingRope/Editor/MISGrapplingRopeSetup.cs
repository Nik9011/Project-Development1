#if INVECTOR_BASIC
using Invector.vEventSystems;
using static Invector.vEventSystems.vAnimatorEvent;
#endif
#if INVECTOR_SHOOTER
using Invector.vShooter;
#endif
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using static com.mobilin.games.MISAnimator;
using System.IO;
using static com.mobilin.games.mvGrapplingRope;
using Invector.vCamera;
using Invector;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public partial class MISMainSetup
    {
#if MIS_GRAPPLINGROPE
        // ----------------------------------------------------------------------------------------------------
        // Animator StateMachine/State
        public const string STATE_GRAPPLINGROPE = "GrapplingRope";

        // Base Layer
        AnimatorStateMachine base_GrapplingRopeSM;
        AnimatorState base_GrapplingRopeSM_GrapplingMove;

        // UpperBodyOnly Layer
        AnimatorStateMachine upbo_GrapplingRopeSM;
        AnimatorState upbo_AimingRope;
        AnimatorState upbo_ThrowRopeInPlace;
#if MIS_INVECTOR_FREECLIMB
        AnimatorState upbo_HangingAimingRope;
        AnimatorState upbo_HangingThrowRopeInPlace;
#endif

        // UpperBody_Attacks
        AnimatorStateMachine upba_GrapplingRopeSM;
        AnimatorState upba_AimingRope;
        AnimatorState upba_ThrowRopeInPlace;
        AnimatorState upba_ThrowRopeOnMove;
        AnimatorState upba_ThrowRopeOnAir;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void GrapplingRopeSetup(mvSetupOption setupOption, GameObject characterObj, GameObject cameraObj)
        {
            // ----------------------------------------------------------------------------------------------------
            // Setup Options
            // ----------------------------------------------------------------------------------------------------


            // ----------------------------------------------------------------------------------------------------
            // SpringRope Component
            // ----------------------------------------------------------------------------------------------------
            GameObject springRopeObj;
            Transform springRopeTransform = misComponentsParentObj.transform.Find("SpringRope");
            if (springRopeTransform == null)
            {
                GameObject springRopePrefab = 
                    AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISFeature.MIS_GRAPPLINGROPE_PATH, "Runtime/Prefabs/SpringRope.prefab"));
                springRopeObj = springRopePrefab.Instantiate3D(Vector3.zero, misComponentsParentObj.transform);
            }
            else
            {
                springRopeObj = springRopeTransform.gameObject;
            }
            mvSpringRope springRope = springRopeObj.GetComponent<mvSpringRope>();


            // ----------------------------------------------------------------------------------------------------
            // Main Component
            // ----------------------------------------------------------------------------------------------------
            mvGrapplingRope package = null;
            if (templateType == MISEditor.TemplateType.Shooter)
            {
                package = characterObj.GetComponent<mvGrapplingRopeShooter>();
                if (package == null)
                    package = characterObj.AddComponent<mvGrapplingRopeShooter>();
            }
            else if (templateType == MISEditor.TemplateType.Melee)
            {
                package = characterObj.GetComponent<mvGrapplingRopeMelee>();
                if (package == null)
                    package = characterObj.AddComponent<mvGrapplingRopeMelee>();
            }
            else
            {
                package = characterObj.GetComponent<mvGrapplingRopeBasic>();
                if (package == null)
                    package = characterObj.AddComponent<mvGrapplingRopeBasic>();
            }

            package.springRope = springRope;


            // Audio Clip
            if (package.acThrowRope == null)
                package.acThrowRope = 
                    AssetDatabase.LoadAssetAtPath<AudioClip>(Path.Combine(MISFeature.MIS_GRAPPLINGROPE_PATH, "Runtime/FX/Audio/Throw.wav"));

            if (package.acStartGrappling == null)
                package.acStartGrappling = 
                    AssetDatabase.LoadAssetAtPath<AudioClip>(Path.Combine(MISFeature.MIS_GRAPPLINGROPE_PATH, "Runtime/FX/Audio/StartGrappling.wav"));


            // ----------------------------------------------------------------------------------------------------
            // vAnimatorEventReceiver
            if (characterObj.TryGetComponent(out vAnimatorEventReceiver animatorEventReceiver) == false)
                animatorEventReceiver = characterObj.AddComponent<vAnimatorEventReceiver>();

            if (animatorEventReceiver.animatorEvents == null)
                animatorEventReceiver.animatorEvents = new List<vAnimatorEventReceiver.vAnimatorEvent>();

            // ThrowRope AnimatorEvent
            vAnimatorEventReceiver.vAnimatorEvent throwRopeAnimatorEvent = animatorEventReceiver.animatorEvents.Find(x => x.eventName.Equals("ThrowRope"));
            if (throwRopeAnimatorEvent != null)
                animatorEventReceiver.animatorEvents.Remove(throwRopeAnimatorEvent);

            throwRopeAnimatorEvent = new vAnimatorEventReceiver.vAnimatorEvent
            {
                eventName = "ThrowRope",
                onTriggerEvent = new vAnimatorEventReceiver.vAnimatorEvent.StateEvent()
            };
            animatorEventReceiver.animatorEvents.Add(throwRopeAnimatorEvent);

            UnityAction throwRopeDelegate = System.Delegate.CreateDelegate(typeof(UnityAction), package, "ThrowRope") as UnityAction;
            UnityEventTools.AddVoidPersistentListener(throwRopeAnimatorEvent.onTriggerEvent, throwRopeDelegate);

            // ReadyToGrappleRope AnimatorEvent
            vAnimatorEventReceiver.vAnimatorEvent readyToGrappleRopeAnimatorEvent = animatorEventReceiver.animatorEvents.Find(x => x.eventName.Equals("ReadyToGrappleRope"));
            if (readyToGrappleRopeAnimatorEvent != null)
                animatorEventReceiver.animatorEvents.Remove(readyToGrappleRopeAnimatorEvent);

            readyToGrappleRopeAnimatorEvent = new vAnimatorEventReceiver.vAnimatorEvent
            {
                eventName = "ReadyToGrappleRope",
                onTriggerEvent = new vAnimatorEventReceiver.vAnimatorEvent.StateEvent()
            };
            animatorEventReceiver.animatorEvents.Add(readyToGrappleRopeAnimatorEvent);

            UnityAction readyToGrappleRopeDelegate = System.Delegate.CreateDelegate(typeof(UnityAction), package, "ReadyToGrappleRope") as UnityAction;
            UnityEventTools.AddVoidPersistentListener(readyToGrappleRopeAnimatorEvent.onTriggerEvent, readyToGrappleRopeDelegate);


            // ----------------------------------------------------------------------------------------------------
            // Animator
            // ----------------------------------------------------------------------------------------------------
            GrapplingRopeAnimatorParameters();
            GrapplingRopeBaseLayer();
            GrapplingRopeUpperBodyOnlyLayer();
            GrapplingRopeUpperBodyAttacksLayer();
            GrapplingRopeAnimatorTransitions();
            GrapplingRopePosition();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void GrapplingRopeAnimatorParameters()
        {
            if (!animatorController.parameters.HasParameter(PARAM_GRAPPLINGROPE_STATE))
                animatorController.AddParameter(PARAM_GRAPPLINGROPE_STATE, AnimatorControllerParameterType.Int);

            if (!animatorController.parameters.HasParameter(PARAM_RIDER_STATE))
                animatorController.AddParameter(PARAM_RIDER_STATE, AnimatorControllerParameterType.Int);

#if MIS_INVECTOR_FREECLIMB
            if (!animatorController.parameters.HasParameter(PARAM_IS_FREECLIMB))
                animatorController.AddParameter(PARAM_IS_FREECLIMB, AnimatorControllerParameterType.Bool);
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void GrapplingRopeBaseLayer()
        {
            // ----------------------------------------------------------------------------------------------------
            // Animation Clips
            // ----------------------------------------------------------------------------------------------------
            var moveTwistFlipMotion = 
                AssetDatabase.LoadAssetAtPath<Motion>(Path.Combine(MISFeature.MIS_GRAPPLINGROPE_PATH, "Runtime/Animations/GrapplingRope@MoveTwistFlip.anim"));


            // ----------------------------------------------------------------------------------------------------
            // Base - Airborne
            // ----------------------------------------------------------------------------------------------------

            // Base - Airborne - Falling - Falling
            base_FallingSM_Falling = base_FallingSM.CreateStateIfNotExist(AIRBORNE_FALLING, null);


            // ----------------------------------------------------------------------------------------------------
            // Base - Action - MIS - GrapplingRopeSM
            // ----------------------------------------------------------------------------------------------------
            base_GrapplingRopeSM = base_ActionsSM.CreateStateMachineIfNotExist(STATE_GRAPPLINGROPE);
            base_ActionsSM.AddExitTransitionIfNotExist(base_GrapplingRopeSM, null);


            // Base - Action - MIS - GrapplingRopeSM - GrapplingRopeMove
            base_GrapplingRopeSM_GrapplingMove = base_GrapplingRopeSM.CreateStateIfNotExist("GrapplingRopeMove", moveTwistFlipMotion);

            // vAnimatorTag
            if (!base_GrapplingRopeSM_GrapplingMove.TryGetStateMachineBehaviour(out vAnimatorTag grapplingMoveAnimatorTag))
                grapplingMoveAnimatorTag = base_GrapplingRopeSM_GrapplingMove.AddStateMachineBehaviour<vAnimatorTag>();

            grapplingMoveAnimatorTag.tags = grapplingMoveAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            grapplingMoveAnimatorTag.tags = grapplingMoveAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_IK);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void GrapplingRopeUpperBodyOnlyLayer()
        {
            base.SetupUpperBodyOnlyLayer();


            // ----------------------------------------------------------------------------------------------------
            // Animation Clips
            // ----------------------------------------------------------------------------------------------------
            var aimingMotion =  AssetDatabase.LoadAssetAtPath<Motion>(
                Path.Combine(MISFeature.MIS_GRAPPLINGROPE_PATH, "Runtime/Animations/GrapplingRope@Aiming.anim"));
            var throwInPlaceMotion = AssetDatabase.LoadAssetAtPath<Motion>(
                Path.Combine(MISFeature.MIS_GRAPPLINGROPE_PATH, "Runtime/Animations/GrapplingRope@ThrowInPlace.anim"));

#if MIS_INVECTOR_FREECLIMB
            var hangingAimingMotion = AssetDatabase.LoadAssetAtPath<Motion>(
                Path.Combine(MISFeature.MIS_GRAPPLINGROPE_PATH, "Runtime/Animations/GrapplingRope@HangingAiming_R.anim"));
            var hangingThrowInPlaceMotion = AssetDatabase.LoadAssetAtPath<Motion>(
                Path.Combine(MISFeature.MIS_GRAPPLINGROPE_PATH, "Runtime/Animations/GrapplingRope@HangingThrow_R.anim"));
#endif


            // ----------------------------------------------------------------------------------------------------
            // UpperBodyOnly - MIS
            // ----------------------------------------------------------------------------------------------------
            SetupUpperBodyOnlyMIS();


            // ----------------------------------------------------------------------------------------------------
            // UpperBodyOnly - MIS - GrapplingRopeSM
            upbo_GrapplingRopeSM = upbo_MIS.CreateStateMachineIfNotExist(STATE_GRAPPLINGROPE);
            upbo_MIS.AddExitTransitionIfNotExist(upbo_GrapplingRopeSM, null);


            // UpperBodyOnly - MIS - GrapplingRopeSM - AimingRope
            upbo_AimingRope = upbo_GrapplingRopeSM.CreateStateIfNotExist("AimingRope", aimingMotion);

            // vAnimatorTag
            if (!upbo_AimingRope.TryGetStateMachineBehaviour(out vAnimatorTag upboAimingRopeAnimatorTag))
                upboAimingRopeAnimatorTag = upbo_AimingRope.AddStateMachineBehaviour<vAnimatorTag>();

            upboAimingRopeAnimatorTag.tags = upboAimingRopeAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            upboAimingRopeAnimatorTag.tags = upboAimingRopeAnimatorTag.tags.AddStringIfNotExist(TAG_IS_THROWING);
            upboAimingRopeAnimatorTag.tags = upboAimingRopeAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_IK);


            // UpperBodyOnly - MIS - GrapplingRopeSM - ThrowRopeInPlace
            upbo_ThrowRopeInPlace = upbo_GrapplingRopeSM.CreateStateIfNotExist("ThrowRopeInPlace", throwInPlaceMotion, true, 1.5f);

            // vAnimatorTag
            if (!upbo_ThrowRopeInPlace.TryGetStateMachineBehaviour(out vAnimatorTag throwRopeInPlaceAnimatorTag))
                throwRopeInPlaceAnimatorTag = upbo_ThrowRopeInPlace.AddStateMachineBehaviour<vAnimatorTag>();

            throwRopeInPlaceAnimatorTag.tags = throwRopeInPlaceAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            throwRopeInPlaceAnimatorTag.tags = throwRopeInPlaceAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_IK);

            // vAnimatorEvent
            if (!upbo_ThrowRopeInPlace.TryGetStateMachineBehaviour(out vAnimatorEvent throwRopeInPlaceAnimatorEvent))
                throwRopeInPlaceAnimatorEvent = upbo_ThrowRopeInPlace.AddStateMachineBehaviour<vAnimatorEvent>();

            vAnimatorEventTrigger throwInPlaceStateThrowRopeAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "ThrowRope",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.NormalizedTime,
                normalizedTime = 0.35f
            };
            vAnimatorEventTrigger throwInPlaceStateReadyToGrappleAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "ReadyToGrappleRope",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.NormalizedTime,
                normalizedTime = 0.85f
            };

            if (throwRopeInPlaceAnimatorEvent.eventTriggers == null)
                throwRopeInPlaceAnimatorEvent.eventTriggers = new List<vAnimatorEventTrigger>();

            if (throwRopeInPlaceAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(throwInPlaceStateThrowRopeAnimatorEventTrigger.eventName)) == null)
                throwRopeInPlaceAnimatorEvent.eventTriggers.Add(throwInPlaceStateThrowRopeAnimatorEventTrigger);

            if (throwRopeInPlaceAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(throwInPlaceStateReadyToGrappleAnimatorEventTrigger.eventName)) == null)
                throwRopeInPlaceAnimatorEvent.eventTriggers.Add(throwInPlaceStateReadyToGrappleAnimatorEventTrigger);


#if MIS_INVECTOR_FREECLIMB
            // UpperBodyOnly - MIS - GrapplingRopeSM - HangingAimingRope
            upbo_HangingAimingRope = upbo_GrapplingRopeSM.CreateStateIfNotExist("HangingAimingRope", aimingMotion);

            // vAnimatorTag
            if (!upbo_HangingAimingRope.TryGetStateMachineBehaviour(out vAnimatorTag upboHangingAimingRopeAnimatorTag))
                upboHangingAimingRopeAnimatorTag = upbo_HangingAimingRope.AddStateMachineBehaviour<vAnimatorTag>();

            upboHangingAimingRopeAnimatorTag.tags = upboHangingAimingRopeAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            upboHangingAimingRopeAnimatorTag.tags = upboHangingAimingRopeAnimatorTag.tags.AddStringIfNotExist(TAG_IS_THROWING);
            upboHangingAimingRopeAnimatorTag.tags = upboHangingAimingRopeAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_IK);


            // UpperBodyOnly - MIS - GrapplingRopeSM - HangingThrowRopeInPlace
            upbo_HangingThrowRopeInPlace = upbo_GrapplingRopeSM.CreateStateIfNotExist("HangingThrowRopeInPlace", throwInPlaceMotion, true, 1.5f);

            // vAnimatorTag
            if (!upbo_ThrowRopeInPlace.TryGetStateMachineBehaviour(out vAnimatorTag upboHangingThrowRopeInPlaceAnimatorTag))
                upboHangingThrowRopeInPlaceAnimatorTag = upbo_ThrowRopeInPlace.AddStateMachineBehaviour<vAnimatorTag>();

            upboHangingThrowRopeInPlaceAnimatorTag.tags = upboHangingThrowRopeInPlaceAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            upboHangingThrowRopeInPlaceAnimatorTag.tags = upboHangingThrowRopeInPlaceAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_IK);

            // vAnimatorEvent
            if (!upbo_ThrowRopeInPlace.TryGetStateMachineBehaviour(out vAnimatorEvent upboHangingThrowRopeInPlaceAnimatorEvent))
                upboHangingThrowRopeInPlaceAnimatorEvent = upbo_ThrowRopeInPlace.AddStateMachineBehaviour<vAnimatorEvent>();

            vAnimatorEventTrigger upboHangingThrowInPlaceStateThrowRopeAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "ThrowRope",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.NormalizedTime,
                normalizedTime = 0.35f
            };
            vAnimatorEventTrigger upboHangingThrowInPlaceStateReadyToGrappleAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "ReadyToGrappleRope",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.NormalizedTime,
                normalizedTime = 0.85f
            };

            if (upboHangingThrowRopeInPlaceAnimatorEvent.eventTriggers == null)
                upboHangingThrowRopeInPlaceAnimatorEvent.eventTriggers = new List<vAnimatorEventTrigger>();

            if (upboHangingThrowRopeInPlaceAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(upboHangingThrowInPlaceStateReadyToGrappleAnimatorEventTrigger.eventName)) == null)
                upboHangingThrowRopeInPlaceAnimatorEvent.eventTriggers.Add(upboHangingThrowInPlaceStateReadyToGrappleAnimatorEventTrigger);

            if (upboHangingThrowRopeInPlaceAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(upboHangingThrowInPlaceStateReadyToGrappleAnimatorEventTrigger.eventName)) == null)
                upboHangingThrowRopeInPlaceAnimatorEvent.eventTriggers.Add(upboHangingThrowInPlaceStateReadyToGrappleAnimatorEventTrigger);
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void GrapplingRopeUpperBodyAttacksLayer()
        {
            base.SetupUpperBodyAttacksLayer();


            // ----------------------------------------------------------------------------------------------------
            // Animation Clips
            // ----------------------------------------------------------------------------------------------------
            var aimingMotion = 
                AssetDatabase.LoadAssetAtPath<Motion>(Path.Combine(MISFeature.MIS_GRAPPLINGROPE_PATH, "Runtime/Animations/GrapplingRope@Aiming.anim"));
            var throwInPlaceMotion = 
                AssetDatabase.LoadAssetAtPath<Motion>(Path.Combine(MISFeature.MIS_GRAPPLINGROPE_PATH, "Runtime/Animations/GrapplingRope@ThrowInPlace.anim"));
            var throwOnMoveMotion = 
                AssetDatabase.LoadAssetAtPath<Motion>(Path.Combine(MISFeature.MIS_GRAPPLINGROPE_PATH, "Runtime/Animations/GrapplingRope@ThrowOnMove.anim"));


            // ----------------------------------------------------------------------------------------------------
            // UpperBodyAttacks - MIS
            // ----------------------------------------------------------------------------------------------------
            SetupUpperBodyAttacksMIS();


            // ----------------------------------------------------------------------------------------------------
            // UpperBody_Attacks - MIS - GrapplingRopeSM
            upba_GrapplingRopeSM = upba_MIS.CreateStateMachineIfNotExist(STATE_GRAPPLINGROPE);
            upba_MIS.AddExitTransitionIfNotExist(upba_GrapplingRopeSM, null);


            // AimingRope
            upba_AimingRope = upba_GrapplingRopeSM.CreateStateIfNotExist("AimingRope", aimingMotion);

            // vAnimatorTag
            if (!upba_AimingRope.TryGetStateMachineBehaviour(out vAnimatorTag aimingRopeAnimatorTag))
                aimingRopeAnimatorTag = upba_AimingRope.AddStateMachineBehaviour<vAnimatorTag>();

            aimingRopeAnimatorTag.tags = aimingRopeAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            aimingRopeAnimatorTag.tags = aimingRopeAnimatorTag.tags.AddStringIfNotExist(TAG_IS_THROWING);
            aimingRopeAnimatorTag.tags = aimingRopeAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_IK);


            // ThrowRopeInPlace
            upba_ThrowRopeInPlace = upba_GrapplingRopeSM.CreateStateIfNotExist("ThrowRopeInPlace", throwInPlaceMotion, true, 1.5f);

            // vAnimatorTag
            if (!upba_ThrowRopeInPlace.TryGetStateMachineBehaviour(out vAnimatorTag throwRopeInPlaceAnimatorTag))
                throwRopeInPlaceAnimatorTag = upba_ThrowRopeInPlace.AddStateMachineBehaviour<vAnimatorTag>();

            throwRopeInPlaceAnimatorTag.tags = throwRopeInPlaceAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            throwRopeInPlaceAnimatorTag.tags = throwRopeInPlaceAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_IK);

            // vAnimatorEvent
            if (!upba_ThrowRopeInPlace.TryGetStateMachineBehaviour(out vAnimatorEvent throwRopeInPlaceAnimatorEvent))
                throwRopeInPlaceAnimatorEvent = upba_ThrowRopeInPlace.AddStateMachineBehaviour<vAnimatorEvent>();

            vAnimatorEventTrigger throwInPlaceThrowRopeAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "ThrowRope",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.NormalizedTime,
                normalizedTime = 0.35f
            };
            vAnimatorEventTrigger throwInPlaceReadyToGrappleAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "ReadyToGrappleRope",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.NormalizedTime,
                normalizedTime = 0.85f
            };
            if (throwRopeInPlaceAnimatorEvent.eventTriggers == null)
                throwRopeInPlaceAnimatorEvent.eventTriggers = new List<vAnimatorEventTrigger>();

            if (throwRopeInPlaceAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(throwInPlaceThrowRopeAnimatorEventTrigger.eventName)) == null)
                throwRopeInPlaceAnimatorEvent.eventTriggers.Add(throwInPlaceThrowRopeAnimatorEventTrigger);

            if (throwRopeInPlaceAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(throwInPlaceReadyToGrappleAnimatorEventTrigger.eventName)) == null)
                throwRopeInPlaceAnimatorEvent.eventTriggers.Add(throwInPlaceReadyToGrappleAnimatorEventTrigger);


            // ThrowOnMove
            upba_ThrowRopeOnMove = upba_GrapplingRopeSM.CreateStateIfNotExist("ThrowOnMove", throwOnMoveMotion, true);

            // vAnimatorTag
            if (!upba_ThrowRopeOnMove.TryGetStateMachineBehaviour(out vAnimatorTag throwRopeOnMoveAnimatorTag))
                throwRopeOnMoveAnimatorTag = upba_ThrowRopeOnMove.AddStateMachineBehaviour<vAnimatorTag>();

            throwRopeOnMoveAnimatorTag.tags = throwRopeOnMoveAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            throwRopeOnMoveAnimatorTag.tags = throwRopeOnMoveAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_IK);

            // vAnimatorEvent
            if (!upba_ThrowRopeOnMove.TryGetStateMachineBehaviour(out vAnimatorEvent throwRopeOnMoveAnimatorEvent))
                throwRopeOnMoveAnimatorEvent = upba_ThrowRopeOnMove.AddStateMachineBehaviour<vAnimatorEvent>();

            vAnimatorEventTrigger throwOnMoveThrowRopeAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "ThrowRope",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.NormalizedTime,
                normalizedTime = 0.3f
            };
            vAnimatorEventTrigger throwOnMoveReadyToGrappleAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "ReadyToGrappleRope",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.NormalizedTime,
                normalizedTime = 0.45f
            };
            if (throwRopeOnMoveAnimatorEvent.eventTriggers == null)
                throwRopeOnMoveAnimatorEvent.eventTriggers = new List<vAnimatorEventTrigger>();

            if (throwRopeOnMoveAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(throwOnMoveThrowRopeAnimatorEventTrigger.eventName)) == null)
                throwRopeOnMoveAnimatorEvent.eventTriggers.Add(throwOnMoveThrowRopeAnimatorEventTrigger);

            if (throwRopeOnMoveAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(throwOnMoveReadyToGrappleAnimatorEventTrigger.eventName)) == null)
                throwRopeOnMoveAnimatorEvent.eventTriggers.Add(throwOnMoveReadyToGrappleAnimatorEventTrigger);


            // ThrowRopeOnAir
            upba_ThrowRopeOnAir = upba_GrapplingRopeSM.CreateStateIfNotExist("ThrowRopeOnAir", throwOnMoveMotion, true);

            // vAnimatorTag
            if (!upba_ThrowRopeOnAir.TryGetStateMachineBehaviour(out vAnimatorTag throwRopeOnAirAnimatorTag))
                throwRopeOnAirAnimatorTag = upba_ThrowRopeOnAir.AddStateMachineBehaviour<vAnimatorTag>();

            throwRopeOnAirAnimatorTag.tags = throwRopeOnAirAnimatorTag.tags.RemoveStringIfExist(TAG_CUSTOM_ACTION);
            throwRopeOnAirAnimatorTag.tags = throwRopeOnAirAnimatorTag.tags.AddStringIfNotExist(TAG_IGNORE_IK);

            // vAnimatorEvent
            if (!upba_ThrowRopeOnAir.TryGetStateMachineBehaviour(out vAnimatorEvent throwRopeOnAirAnimatorEvent))
                throwRopeOnAirAnimatorEvent = upba_ThrowRopeOnAir.AddStateMachineBehaviour<vAnimatorEvent>();

            vAnimatorEventTrigger throwOnAirThrowRopeAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "ThrowRope",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.NormalizedTime,
                normalizedTime = 0.3f
            };
            vAnimatorEventTrigger throwOnAirReadyToGrappleAnimatorEventTrigger = new vAnimatorEventTrigger()
            {
                eventName = "ReadyToGrappleRope",
                eventTriggerType = vAnimatorEventTrigger.vAnimatorEventTriggerType.NormalizedTime,
                normalizedTime = 0.45f
            };
            if (throwRopeOnAirAnimatorEvent.eventTriggers == null)
                throwRopeOnAirAnimatorEvent.eventTriggers = new List<vAnimatorEventTrigger>();

            if (throwRopeOnAirAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(throwOnAirThrowRopeAnimatorEventTrigger.eventName)) == null)
                throwRopeOnAirAnimatorEvent.eventTriggers.Add(throwOnAirThrowRopeAnimatorEventTrigger);

            if (throwRopeOnAirAnimatorEvent.eventTriggers.Find(x => x.eventName.Equals(throwOnAirReadyToGrappleAnimatorEventTrigger.eventName)) == null)
                throwRopeOnAirAnimatorEvent.eventTriggers.Add(throwOnAirReadyToGrappleAnimatorEventTrigger);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void GrapplingRopeAnimatorTransitions()
        {
            conditionList.Clear();


            // ----------------------------------------------------------------------------------------------------
            // Base Layer
            // ----------------------------------------------------------------------------------------------------

            // ----------------------------------------------------------------------------------------------------
            // Base - Airborne - FallingSM

            // Falling To Exit
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Greater, (int)GrapplingRopeStateType.None));
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Less, (int)GrapplingRopeStateType.GrapplingMove));
            base_FallingSM_Falling.AddExitTransitionIfNotExist(conditionList);


            // ----------------------------------------------------------------------------------------------------
            // Base - Actions - MIS - GrapplingRope

            // GrapplingRopeMove To Exit
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.None));
            base_GrapplingRopeSM_GrapplingMove.AddExitTransitionIfNotExist(conditionList);



            // ----------------------------------------------------------------------------------------------------
            // UpperBodyOnly Layer: Chained-Action with MIS-MOTORCYCLE
            // ----------------------------------------------------------------------------------------------------

            // Null To AimingRope
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.Aiming));
            conditionList.Add(Condition(PARAM_RIDER_STATE, AnimatorConditionMode.NotEqual, 0f));
            
#if MIS_INVECTOR_FREECLIMB
            AnimatorStateTransition uboNullToAimingRope = upbo_NullSM_Null.FindSameTransition(upbo_AimingRope, conditionList);
            
            if (uboNullToAimingRope != null)
            {
                uboNullToAimingRope.AddCondition(AnimatorConditionMode.IfNot, 0f, PARAM_IS_FREECLIMB);  // false
            }
            else
            {
                conditionList.Add(Condition(PARAM_IS_FREECLIMB, AnimatorConditionMode.IfNot, 0f)); // false
                upbo_NullSM_Null.AddTransitionIfNotExist(upbo_AimingRope, conditionList);
            }
#else
            upbo_NullSM_Null.AddTransitionIfNotExist(upbo_AimingRope, conditionList);
#endif

#if MIS_INVECTOR_FREECLIMB
            // Null To HangingAimingRope
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.Aiming));
            conditionList.Add(Condition(PARAM_RIDER_STATE, AnimatorConditionMode.NotEqual, 0f));
            conditionList.Add(Condition(PARAM_IS_FREECLIMB, AnimatorConditionMode.If, 0f)); // true
            upbo_NullSM_Null.AddTransitionIfNotExist(upbo_HangingAimingRope, conditionList);
#endif


            // ----------------------------------------------------------------------------------------------------
            // UpperBodyOnly - MIS - GrapplingRope

            // AimingRope To Exit
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.None));
            upbo_AimingRope.AddExitTransitionIfNotExist(conditionList, false, 0f);


            // AimingRope To ThrowRopeInPlace
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.Throw));
            upbo_AimingRope.AddTransitionIfNotExist(upbo_ThrowRopeInPlace, conditionList, false, 0f);


            // ThrowRopeInPlace To Exit1
            upbo_ThrowRopeInPlace.AddExitTransitionIfNotExist(null, true, 0.83f);

            // ThrowRopeInPlace To Exit2
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.None));
            upbo_ThrowRopeInPlace.AddExitTransitionIfNotExist(conditionList);


#if MIS_INVECTOR_FREECLIMB
            // HangingAimingRope To Exit
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.None));
            upbo_HangingAimingRope.AddExitTransitionIfNotExist(conditionList, false, 0f);

            // HangingAimingRope To HangingThrowRopeInPlace
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.Throw));
            upbo_HangingAimingRope.AddTransitionIfNotExist(upbo_HangingThrowRopeInPlace, conditionList, false, 0f);

            // HangingThrowRopeInPlace To Exit1
            upbo_HangingThrowRopeInPlace.AddExitTransitionIfNotExist(null, true, 0.85f);

            // HangingThrowRopeInPlace To Exit2
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.None));
            upbo_HangingThrowRopeInPlace.AddExitTransitionIfNotExist(conditionList);
#endif


            // ----------------------------------------------------------------------------------------------------
            // UpperBody_Attacks Layer
            // ----------------------------------------------------------------------------------------------------

            // Null To AimingRope
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.Aiming));
            conditionList.Add(Condition(PARAM_RIDER_STATE, AnimatorConditionMode.Equals, 0f));  // MIS_MOTORCYCLE
            upba_NullSM_Null.AddTransitionIfNotExist(upba_AimingRope, conditionList);


            // ----------------------------------------------------------------------------------------------------
            // UpperBody_Attacks - MIS - GrapplingRope

            // AimingRope To ThrowRopeInPlace
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.Throw));
            conditionList.Add(Condition(PARAM_IS_GROUNDED, AnimatorConditionMode.If, 0f));
            conditionList.Add(Condition(PARAM_INPUT_MAGNITUDE, AnimatorConditionMode.Less, 0.25f));
            upba_AimingRope.AddTransitionIfNotExist(upba_ThrowRopeInPlace, conditionList, false, 0f);


            // ThrowRopeInPlace To Exit1
            upba_ThrowRopeInPlace.AddExitTransitionIfNotExist(null, true, 0.83f);

            // ThrowRopeInPlace To Exit2
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.None));
            upba_ThrowRopeInPlace.AddExitTransitionIfNotExist(conditionList);


            // AimingRope To ThrowRopeOnMove
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, (int)GrapplingRopeStateType.Throw));
            conditionList.Add(Condition(PARAM_IS_GROUNDED, AnimatorConditionMode.If, 0f)); // true
            upba_AimingRope.AddTransitionIfNotExist(upba_ThrowRopeOnMove, conditionList, false, 0f);


            // AimingRope To ThrowRopeOnAir
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, 2f));
            conditionList.Add(Condition(PARAM_IS_GROUNDED, AnimatorConditionMode.IfNot, 0f)); // false
            upba_AimingRope.AddTransitionIfNotExist(upba_ThrowRopeOnAir, conditionList, false, 0f);


            // AimingRope To Exit
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, 0f));
            upba_AimingRope.AddExitTransitionIfNotExist(conditionList, false, 0f);


            // ThrowRopeOnMove to Exit1
            upba_ThrowRopeOnMove.AddExitTransitionIfNotExist(null, true, 0.625f);

            // ThrowRopeOnMove to Exit2
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, 0f));
            upba_ThrowRopeOnMove.AddExitTransitionIfNotExist(conditionList, false, 0.75f);


            // ThrowRopeOnAir to Exit1
            upba_ThrowRopeOnAir.AddExitTransitionIfNotExist(null, true, 0.625f);

            // ThrowRopeOnAir to Exit2
            conditionList.Clear();
            conditionList.Add(Condition(PARAM_GRAPPLINGROPE_STATE, AnimatorConditionMode.Equals, 0f));
            upba_ThrowRopeOnAir.AddExitTransitionIfNotExist(conditionList);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void GrapplingRopePosition()
        {
            // ----------------------------------------------------------------------------------------------------
            // Base Layer
            // ----------------------------------------------------------------------------------------------------
            base_GrapplingRopeSM.SetDefaultLayerPosition(BASE_LD_POS, BASE_RM_POS);
            base_GrapplingRopeSM.SetParentStateMachinePosition(PARENT_LM_POS);

            base_GrapplingRopeSM.SetStatePosition(base_GrapplingRopeSM_GrapplingMove, STATE_POS);


            // ----------------------------------------------------------------------------------------------------
            // UpperBodyOnly Layer
            // ----------------------------------------------------------------------------------------------------
            upbo_MIS.SetStateMachinePosition(upbo_GrapplingRopeSM, UPBO_GRAPPLINGROPE_POS);

            upbo_GrapplingRopeSM.SetDefaultLayerPosition(BASE_LD_POS, BASE_RM_POS);
            upbo_GrapplingRopeSM.SetParentStateMachinePosition(PARENT_LM_POS);

            upbo_GrapplingRopeSM.SetStateRelativePosition(upbo_AimingRope, 0, -1);
            upbo_GrapplingRopeSM.SetStateRelativePosition(upbo_ThrowRopeInPlace, 0, -2);

#if MIS_INVECTOR_FREECLIMB
            upbo_GrapplingRopeSM.SetStateRelativePosition(upbo_HangingAimingRope, 0, 1);
            upbo_GrapplingRopeSM.SetStateRelativePosition(upbo_HangingThrowRopeInPlace, 0, 2);
#endif


            // ----------------------------------------------------------------------------------------------------
            // UpperBody_Attacks Layer
            // ----------------------------------------------------------------------------------------------------
            upba_MIS.SetStateMachinePosition(upba_GrapplingRopeSM, STATE_POS);

            upba_GrapplingRopeSM.SetDefaultLayerPosition(BASE_LU_POS, EXIT_RM_POS + (Vector3.right * HORIZONTAL_GAP * 3));
            upba_GrapplingRopeSM.SetParentStateMachinePosition(STATE_POS + (Vector3.right * HORIZONTAL_GAP * 3) + (Vector3.up * VERTICAL_GAP * 2));

            upba_GrapplingRopeSM.SetStateRelativePosition(upba_AimingRope, 0f, 0);
            upba_GrapplingRopeSM.SetStateRelativePosition(upba_ThrowRopeInPlace, 3, -2);
            upba_GrapplingRopeSM.SetStateRelativePosition(upba_ThrowRopeOnMove, 3, -1);
            upba_GrapplingRopeSM.SetStateRelativePosition(upba_ThrowRopeOnAir, 3, 1);
        }
#endif
    }
}
