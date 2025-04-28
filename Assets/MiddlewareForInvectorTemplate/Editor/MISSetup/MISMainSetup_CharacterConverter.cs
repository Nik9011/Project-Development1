//#define USE_MVAI_COMPANION

#pragma warning disable CS0414

#if INVECTOR_BASIC
using Invector;
using Invector.vCamera;
using Invector.vCharacterController;
using Invector.vCharacterController.vActions;
#endif
#if INVECTOR_MELEE
using Invector.vMelee;
using Invector.vCharacterController.AI;
#endif
#if INVECTOR_SHOOTER
using Invector.vShooter;
#endif
#if INVECTOR_MELEE || INVECTOR_SHOOTER
using Invector.vItemManager;
#endif
#if INVECTOR_AI_TEMPLATE && MIS_FSM_AI
using Invector.vCharacterController.AI.FSMBehaviour;
#endif
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.Events;
using UnityEditor.Events;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public partial class MISMainSetup
    {
        // ----------------------------------------------------------------------------------------------------
        // Invector Component
        bool hasItemManager = false;

        bool hasShooterManager = false;
        bool hasMeleeManager = false;

        bool hasLockOnShooter = false;
        bool hasLockOn = false;

        bool hasLadderAction = false;

#if INVECTOR_SHOOTER
        vShooterManager shooterManager;
        vLockOnShooter lockOnShooter;

        vDrawHideShooterWeapons sourceShooterDrawHide;
        mvDrawHideShooterWeapons targetShooterDrawHide;
#endif
#if INVECTOR_MELEE
        vMeleeManager meleeManager;
        vLockOn lockOn;
#endif

#if INVECTOR_BASIC
        vThirdPersonController sourceCC;
        mvThirdPersonController targetCC;
#endif
#if INVECTOR_SHOOTER
        vShooterMeleeInput sourceShooterInput;
        mvShooterMeleeInput targetShooterInput;
#endif
#if INVECTOR_MELEE
        vMeleeCombatInput sourceMeleeInput;
        mvMeleeCombatInput targetMeleeInput;
#endif
#if INVECTOR_BASIC
        vThirdPersonInput sourceCCInput;
        mvThirdPersonInput targetCCInput;
#endif

#if INVECTOR_MELEE || INVECTOR_SHOOTER
        vItemManager itemManager;
#endif

#if INVECTOR_BASIC
        vLadderAction ladderAction;
#endif

#if INVECTOR_BASIC
        vMessageReceiver messageReceiver;
#endif


        // ----------------------------------------------------------------------------------------------------
        // FSM AI
#if INVECTOR_AI_TEMPLATE && MIS_FSM_AI
        vControlAIShooter sourceControlAIShooter;
        vControlAIMelee sourceControlAIMelee;
        vControlAICombat sourceControlAICombat;
        vControlAI sourceControlAI;

        mvControlAIShooter targetControlAIShooter;
        mvControlAIMelee targetControlAIMelee;
        mvControlAICombat targetControlAICombat;
        mvControlAI targetControlAI;

#if INVECTOR_SHOOTER
        vAIShooterManager sourceAIShooterManager;
        mvAIShooterManager targetAIShooterManager;
#endif

#if INVECTOR_MELEE || INVECTOR_SHOOTER
        vSimpleHolder sourceSimpleHolder;
        mvSimpleHolder targetSimpleHolder;
#endif

        vAIHeadtrack sourceAIHeadtrack;
        mvAIHeadtrack targetAIHeadtrack;

#if USE_MVAI_COMPANION
        vAICompanion sourceAICompanion;
        mvAICompanion targetAICompanion;
#endif
#endif


        // ----------------------------------------------------------------------------------------------------
        // Ragdoll Remover
        bool includeInactive = true;
        bool removeColliders = true;
        bool removeCharacterJoints = true;
        bool removeRigidbodies = true;
        bool removeDamageReceiver = true;
        bool removevRagdoll = true;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CharacterConverterContent()
        {
            GUILayout.BeginVertical(skin.GetStyle("WindowBG"), GUILayout.ExpandWidth(true)/*, GUILayout.ExpandHeight(true)*/);
            {
                EditorGUILayout.HelpBox("Since the MIS Refactoring has been done, all Invector characters should be converted to MIS characters.\nIt is automatically converted when you install any MIS package, but if you have a character who does not use MIS package, please use this.", MessageType.Info);

                GUILayout.Space(5);

                // ----------------------------------------------------------------------------------------------------
                // Character Converter
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Label("<b>Target</b>", GUILayout.MaxHeight(25));

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.BeginVertical();
                        {
                            MISEditor.SetToggleLabelWidth(true, 200f);
                            characterObj = EditorGUILayout.ObjectField("(MIS)INVECTOR Character", characterObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                            MISEditor.SetToggleLabelWidth(false);
                            isValidCharacter = IsValidCharacterObject(characterObj, false);
                            isValidInvectorCharacter = IsValidInvectorCharacter(false);
                            isValidInvectorAICharacter = IsValidInvectorFSMCharacter(false);

                            MISEditor.SetToggleLabelWidth(true, 200f);
                            cameraObj = EditorGUILayout.ObjectField("Main Camera (Player character)", cameraObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                            MISEditor.SetToggleLabelWidth(false);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(10);

                        if (GUILayout.Button(iconView, GUILayout.Height(35), GUILayout.Width(35)))
                        {
                            if (RefreshInvectorCharacter() == false)
                                Debug.LogWarning("Please select a INVECTOR(MIS) character in Hierarchy first.");
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    {
                        EditorGUI.BeginDisabledGroup(!isValidCharacter || !isValidInvectorCharacter);
                        {
                            if (GUILayout.Button("Convert Player Character", GUILayout.Width(minWidth * 0.48f)))
                            {
#if UNITY_2018_3_OR_NEWER
                                PrefabAssetType m_AssetType = PrefabUtility.GetPrefabAssetType(characterObj);

                                if (m_AssetType != PrefabAssetType.NotAPrefab)
                                {
                                    GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(characterObj);
                                    PrefabUtility.UnpackPrefabInstance(prefabRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                                }
#endif

                                ConvertCharacter(characterObj);
                                //ApplyMISAnimatorParameters();
                            }
                        }
                        EditorGUI.EndDisabledGroup();

                        GUILayout.FlexibleSpace();

                        EditorGUI.BeginDisabledGroup(!isValidCharacter || !isValidInvectorAICharacter);
                        {
                            if (GUILayout.Button("Convert AI Character", GUILayout.Width(minWidth * 0.48f)))
                            {
#if UNITY_2018_3_OR_NEWER
                                PrefabAssetType m_AssetType = PrefabUtility.GetPrefabAssetType(characterObj);

                                if (m_AssetType != PrefabAssetType.NotAPrefab)
                                {
                                    GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(characterObj);
                                    PrefabUtility.UnpackPrefabInstance(prefabRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                                }
#endif

                                ConvertAICharacter(characterObj);
                                //ApplyAIAnimatorParameters();
                            }
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.Space(10);

                // ----------------------------------------------------------------------------------------------------
                // Ragdoll Remover
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Label("<b>Ragdoll Remover</b>", GUILayout.MaxHeight(25));

                    EditorGUILayout.HelpBox("Remove ragdoll components with the following options.", MessageType.Info);

                    MISEditor.SetToggleLabelWidth(true);
                    includeInactive = EditorGUILayout.Toggle("Include Inactive Objects", includeInactive);
                    removeColliders = EditorGUILayout.Toggle("Remove Colliders", removeColliders);
                    removeCharacterJoints = EditorGUILayout.Toggle("Remove CharacterJoints", removeCharacterJoints);
                    removeRigidbodies = EditorGUILayout.Toggle("Remove Rigidbodies", removeRigidbodies);
                    removeDamageReceiver = EditorGUILayout.Toggle("Remove vDamageReceiver", removeDamageReceiver);
                    removevRagdoll = EditorGUILayout.Toggle("Remove vRagdoll", removevRagdoll);
                    MISEditor.SetToggleLabelWidth(false);

                    GUILayout.Space(10);

                    EditorGUI.BeginDisabledGroup(characterObj == null);
                    {
                        if (GUILayout.Button("Remove"))
                        {
#if UNITY_2018_3_OR_NEWER
                            PrefabAssetType m_AssetType = PrefabUtility.GetPrefabAssetType(characterObj);

                            if (m_AssetType != PrefabAssetType.NotAPrefab)
                            {
                                GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(characterObj);
                                PrefabUtility.UnpackPrefabInstance(prefabRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                            }
#endif

                            RemoveRagdoll();
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool RefreshInvectorCharacter()
        {
            characterObj = Selection.activeGameObject;

            if (characterObj == null)
                return false;

#if INVECTOR_BASIC
            var thirdPersonCamera = characterObj.GetComponentInChildren<vThirdPersonCamera>();

            if (thirdPersonCamera)
                mainCamera = thirdPersonCamera.gameObject.GetComponentInChildren<Camera>();
#endif

            if (mainCamera != null)
                cameraObj = mainCamera.gameObject;

            return true;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void ApplyMISAnimatorParameters()
        {
            AnimatorController animatorController = characterAnimator.runtimeAnimatorController as AnimatorController;

            animatorController.SetMISAddonsParameters();

            EditorUtility.SetDirty(animatorController);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            SceneView.lastActiveSceneView.FrameSelected();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void ApplyAIAnimatorParameters()
        {
            AnimatorController animatorController = characterAnimator.runtimeAnimatorController as AnimatorController;

            animatorController.SetMISAIAddonsParameters();

            EditorUtility.SetDirty(animatorController);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            SceneView.lastActiveSceneView.FrameSelected();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void RemoveRagdoll()
        {
            if (removeColliders)
            {
                Collider[] colliders = characterObj.GetComponentsInChildren<Collider>(includeInactive);
                foreach (Collider component in colliders)
                {
                    if (component.transform != characterObj.transform)
                    {
                        component.hideFlags |= HideFlags.HideInInspector;
                        DestroyImmediate(component);
                    }
                }
            }

            // Muse destroy it before Rigidbody
            if (removeCharacterJoints)
            {
                CharacterJoint[] characterJoints = characterObj.GetComponentsInChildren<CharacterJoint>(includeInactive);
                foreach (CharacterJoint component in characterJoints)
                {
                    if (component.transform != characterObj.transform)
                    {
                        component.hideFlags |= HideFlags.HideInInspector;
                        DestroyImmediate(component);
                    }
                }
            }

            if (removeRigidbodies)
            {
                Rigidbody[] rigidbodys = characterObj.GetComponentsInChildren<Rigidbody>(includeInactive);
                foreach (Rigidbody component in rigidbodys)
                {
                    if (component.transform != characterObj.transform)
                    {
                        component.hideFlags |= HideFlags.HideInInspector;
                        DestroyImmediate(component);
                    }
                }
            }

#if INVECTOR_BASIC
            if (removeDamageReceiver)
            {
                vDamageReceiver[] damageReceivers = characterObj.GetComponentsInChildren<vDamageReceiver>(includeInactive);
                foreach (vDamageReceiver component in damageReceivers)
                {
                    if (component.transform != characterObj.transform)
                    {
                        component.hideFlags |= HideFlags.HideInInspector;
                        DestroyImmediate(component);
                    }
                }
            }

            if (removevRagdoll)
            {
                if (characterObj.TryGetComponent(out vRagdoll ragdoll))
                    DestroyImmediate(ragdoll);
            }
#endif

            //AssetDatabase.Refresh();
            //AssetDatabase.SaveAssets();

            SceneView.lastActiveSceneView.FrameSelected();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool IsValidInvectorCharacter(bool showHelpBox = true)
        {
            if (characterObj == null)
                return false;

            hasMISController = HasMISController();

            ValidateInvectorComponent();

#if INVECTOR_BASIC
            if (!((characterObj.TryGetComponent(out vThirdPersonController cc) && cc.IsTheType<vThirdPersonController>())
                || (characterObj.TryGetComponent(out mvThirdPersonController mcc) && mcc.IsTheType<mvThirdPersonController>())))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has not a ThirdPerson Invector controller.", MessageType.Error);

                templateType = MISEditor.TemplateType.None;
                return false;
            }
#endif

#if INVECTOR_SHOOTER
            if ((characterObj.TryGetComponent(out vShooterMeleeInput shooterInput) && shooterInput.IsTheType<vShooterMeleeInput>())
                || (characterObj.TryGetComponent(out mvShooterMeleeInput mshooterInput) && mshooterInput.IsTheType<mvShooterMeleeInput>()))
            {
                templateType = MISEditor.TemplateType.Shooter;
                return true;
            }
            else
#endif
#if INVECTOR_MELEE
            if ((characterObj.TryGetComponent(out vMeleeCombatInput meleeInput) && meleeInput.IsTheType<vMeleeCombatInput>())
                || (characterObj.TryGetComponent(out mvMeleeCombatInput mmeleeInput) && mmeleeInput.IsTheType<mvMeleeCombatInput>()))
            {
                templateType = MISEditor.TemplateType.Melee;
                return true;
            }
            else
#endif
#if INVECTOR_BASIC
            if ((characterObj.TryGetComponent(out vThirdPersonInput tpInput) && tpInput.IsTheType<vThirdPersonInput>())
                || (characterObj.TryGetComponent(out mvThirdPersonInput mtpInput) && mtpInput.IsTheType<mvThirdPersonInput>()))
            {
                templateType = MISEditor.TemplateType.Basic;
                return true;
            }
            else
#endif
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has not an Invector Input controller.", MessageType.Error);

                templateType = MISEditor.TemplateType.None;
                return false;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool IsValidInvectorFSMCharacter(bool showHelpBox = true)
        {
            if (characterObj == null)
                return false;

            hasMISAIController = HasMISAIController();

#if INVECTOR_AI_TEMPLATE && MIS_FSM_AI
            if (!characterObj.TryGetComponent(out vFSMBehaviourController fsm))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has not an Invector FSM Behaviour Controller.", MessageType.Error);

                fsmAITemplateType = MISEditor.FSMTemplateType.None;
                return false;
            }

            if ((characterObj.TryGetComponent(out vControlAIShooter shooterAI) && shooterAI.IsTheType<vControlAIShooter>())
                || (characterObj.TryGetComponent(out mvControlAIShooter misShooterAI) && misShooterAI.IsTheType<mvControlAIShooter>())
                )
            {
                fsmAITemplateType = MISEditor.FSMTemplateType.Shooter;
                return true;
            }
            else if ((characterObj.TryGetComponent(out vControlAIMelee meleeAI) && meleeAI.IsTheType<vControlAIMelee>())
                || (characterObj.TryGetComponent(out mvControlAIMelee misMeleeAI) && misMeleeAI.IsTheType<mvControlAIMelee>())
                )
            {
                fsmAITemplateType = MISEditor.FSMTemplateType.Melee;
                return true;
            }
            else if ((characterObj.TryGetComponent(out vControlAICombat combatAI) && combatAI.IsTheType<vControlAICombat>())
                || (characterObj.TryGetComponent(out mvControlAICombat misCombatAI) && misCombatAI.IsTheType<mvControlAICombat>())
                )
            {
                fsmAITemplateType = MISEditor.FSMTemplateType.Combat;
                return true;
            }
            else if ((characterObj.TryGetComponent(out vControlAI basicAI) && basicAI.IsTheType<vControlAI>())
                || (characterObj.TryGetComponent(out mvControlAI misBasicAI) && misBasicAI.IsTheType<mvControlAI>())
                )
            {
                fsmAITemplateType = MISEditor.FSMTemplateType.Basic;
                return true;
            }
            else
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has not an Invector FSM AI Controller.", MessageType.Error);

                fsmAITemplateType = MISEditor.FSMTemplateType.None;
                return false;
            }
#else
            fsmAITemplateType = MISEditor.FSMTemplateType.None;
            return false;
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ValidateInvectorComponent()
        {
#if INVECTOR_MELEE || INVECTOR_SHOOTER
            if (characterObj)
                hasItemManager = characterObj.TryGetComponent(out itemManager);
            else
#endif
                hasItemManager = false;

#if INVECTOR_SHOOTER
            if (characterObj)
            {
                hasShooterManager = characterObj.TryGetComponent(out shooterManager) && shooterManager.IsTheType<vShooterManager>();
                hasLockOnShooter = characterObj.TryGetComponent(out lockOnShooter) && lockOnShooter.IsTheType<vLockOnShooter>();
            }
#endif

#if INVECTOR_MELEE
            if (characterObj)
            {
                hasMeleeManager = characterObj.TryGetComponent(out meleeManager) && meleeManager.IsTheType<vMeleeManager>();
                hasLockOn = characterObj.TryGetComponent(out lockOn) && lockOn.IsTheType<vLockOn>();
            }
#endif

#if INVECTOR_BASIC
            if (characterObj)
                hasLadderAction = characterObj.TryGetComponent(out ladderAction);
            else
                hasLadderAction = false;
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool HasInvectorAIController(bool showHelpBox = false)
        {
            if (characterObj == null)
                return false;

#if INVECTOR_MELEE || INVECTOR_SHOOTER
            if (characterObj.TryGetComponent(out vSimpleMeleeAI_Controller simpleai) && simpleai.IsTheType())
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character object has an Invector SimpleMeleeAI Controller.", MessageType.Error);

                return true;
            }
#endif

#if INVECTOR_AI_TEMPLATE
            return HasInvectorFSMAIController(showHelpBox);
#else
            return false;
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool HasInvectorFSMAIController(bool showHelpBox = false)
        {
            if (characterObj == null)
                return false;

#if INVECTOR_AI_TEMPLATE && MIS_FSM_AI
            if (characterObj.TryGetComponent(out vFSMBehaviourController fsm))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has an Invector FSM Behaviour controller.", MessageType.Error);

                return true;
            }
            else if (characterObj.TryGetComponent(out vControlAI ai))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character object has an Invector FSM AI controller.", MessageType.Error);

                return true;
            }
#endif

            return false;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool HasMISController(bool showHelpBox = false)
        {
            if (characterObj == null)
                return false;

#if INVECTOR_BASIC
            if (characterObj.TryGetComponent(out mvThirdPersonController cc) && cc.IsTheType<mvThirdPersonController>())
                return true;
#endif

            if (showHelpBox)
                EditorGUILayout.HelpBox("This character object has not a MIS Controller component.", MessageType.Error);

            return false;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool HasMISAIController(bool showHelpBox = false)
        {
            if (characterObj == null)
                return false;

#if MIS_FSM_AI
            if (characterObj.TryGetComponent(out mvControlAI cc) && cc.IsTheType<mvControlAI>())
                return true;
#endif

            if (showHelpBox)
                EditorGUILayout.HelpBox("This character object has not a MIS AI Controller component.", MessageType.Error);

            return false;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void Setup(bool convertController, bool createParent = true)
        {
            // ----------------------------------------------------------------------------------------------------
            // MIS Conversion
            if (convertController)
            {
                if (hasMISController == false)
                {
                    ConvertCharacter(characterObj);

                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
            }

            // ----------------------------------------------------------------------------------------------------
            // Invector Components
            if (createParent)
            {
                Transform invectorComponentParent = characterObj.transform.Find(INVECTOR_COMPONENTS);
                if (invectorComponentParent == null)
                {
                    invectorComponentParent = characterObj.transform.Find("InvectorComponents");
                    if (invectorComponentParent == null)
                        invectorComponentsParentObj = new GameObject(INVECTOR_COMPONENTS);
                    else
                        invectorComponentsParentObj = invectorComponentParent.gameObject;
                }
                else
                {
                    invectorComponentsParentObj = invectorComponentParent.gameObject;
                }
                invectorComponentsParentObj.transform.SetLocalParent(characterObj.transform);
            }

            // ----------------------------------------------------------------------------------------------------
            // MIS Components
            if (createParent)
            {
                Transform misComponentParent = characterObj.transform.Find(MIS_COMPONENTS);
                if (misComponentParent == null)
                    misComponentsParentObj = new GameObject(MIS_COMPONENTS);
                else
                    misComponentsParentObj = misComponentParent.gameObject;
                misComponentsParentObj.transform.SetLocalParent(characterObj.transform);
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void ConvertCharacter(GameObject characterObj)
        {
            if (isValidInvectorCharacter == false)
                return;


            // ----------------------------------------------------------------------------------------------------
            // DrawHideShooterWeapons
#if INVECTOR_SHOOTER
            if (characterObj.TryGetComponent(out sourceShooterDrawHide) && sourceShooterDrawHide.enabled && sourceShooterDrawHide.IsTheType<vDrawHideShooterWeapons>())
            {
                if (characterObj.GetComponent<mvDrawHideShooterWeapons>() == null)
                    DestroyImmediate(characterObj.GetComponent<mvDrawHideShooterWeapons>());
                targetShooterDrawHide = characterObj.AddComponent<mvDrawHideShooterWeapons>();

                MISEditor.PasteComponentValues(sourceShooterDrawHide, targetShooterDrawHide);

                // ----------------------------------------------------------------------------------------------------
                // ThrowManager
                Invector.Throw.vThrowManager throwManager = characterObj.GetComponentInChildren<Invector.Throw.vThrowManager>();

                if (throwManager != null)
                {
                    if (throwManager.onEnableAim == null)
                        throwManager.onEnableAim = new UnityEvent();

                    SerializedObject serializedObject = new SerializedObject(throwManager);

                    if (serializedObject != null)
                    {
                        SerializedProperty enableAimEventProp = serializedObject.FindProperty("onEnableAim");

                        if (enableAimEventProp != null)
                        {
                            if (sourceShooterDrawHide != null && sourceShooterDrawHide.IsTheType<vDrawHideShooterWeapons>())
                                throwManager.onEnableAim.ReplacePersistent<mvDrawHideShooterWeapons>(enableAimEventProp, typeof(vDrawHideShooterWeapons), typeof(bool), targetShooterDrawHide, false);
                        }
                    }
                }
            }
#endif


            // ----------------------------------------------------------------------------------------------------
            // Controller
            if (characterObj.TryGetComponent(out sourceCC) && sourceCC.enabled && sourceCC.IsTheType<vThirdPersonController>())
            {
                if (characterObj.GetComponent<mvThirdPersonController>() != null)
                    DestroyImmediate(characterObj.GetComponent<mvThirdPersonController>());
                targetCC = characterObj.AddComponent<mvThirdPersonController>();

                MISEditor.PasteComponentValues(sourceCC, targetCC);
            }

            // Input Manager
#if INVECTOR_SHOOTER
            if (characterObj.TryGetComponent(out sourceShooterInput) && sourceShooterInput.enabled && sourceShooterInput.IsTheType<vShooterMeleeInput>())
            {
                if (characterObj.GetComponent<mvShooterMeleeInput>() != null)
                    DestroyImmediate(characterObj.GetComponent<mvShooterMeleeInput>());
                targetShooterInput = characterObj.AddComponent<mvShooterMeleeInput>();

                MISEditor.PasteComponentValues(sourceShooterInput, targetShooterInput);
            }
            else
#endif
#if INVECTOR_MELEE
            if (characterObj.TryGetComponent(out sourceMeleeInput) && sourceMeleeInput.enabled && sourceMeleeInput.IsTheType<vMeleeCombatInput>())
            {
                if (characterObj.GetComponent<mvMeleeCombatInput>() != null)
                    DestroyImmediate(characterObj.GetComponent<mvMeleeCombatInput>());
                targetMeleeInput = characterObj.AddComponent<mvMeleeCombatInput>();

                MISEditor.PasteComponentValues(sourceMeleeInput, targetMeleeInput);
            }
            else
#endif
#if INVECTOR_BASIC
            if (characterObj.TryGetComponent(out sourceCCInput) && sourceCCInput.enabled && sourceCCInput.IsTheType<vThirdPersonInput>())
            {
                if (characterObj.GetComponent<mvThirdPersonInput>() != null)
                    DestroyImmediate(characterObj.GetComponent<mvThirdPersonInput>());
                targetCCInput = characterObj.AddComponent<mvThirdPersonInput>();

                MISEditor.PasteComponentValues(sourceCCInput, targetCCInput);
            }
#endif
            
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();


            // ----------------------------------------------------------------------------------------------------
            // Replace Persistent
#if INVECTOR_MELEE || INVECTOR_SHOOTER
            if (hasItemManager)
            {
                SerializedObject serializedObject = new SerializedObject(itemManager);

                if (serializedObject != null)
                {
                    SerializedProperty applyAttributeEventsProp = serializedObject.FindProperty("applyAttributeEvents");

                    if (itemManager.applyAttributeEvents != null && applyAttributeEventsProp != null)
                    {
                        for (int i = 0; i < itemManager.applyAttributeEvents.Count; i++)
                        {
                            SerializedProperty onApplyAttributeProp = applyAttributeEventsProp.GetArrayElementAtIndex(i).FindPropertyRelative("onApplyAttribute");
                            if (onApplyAttributeProp == null)
                                continue;

                            if (sourceCC && sourceCC.IsTheType<vThirdPersonController>())
                                itemManager.applyAttributeEvents[i].onApplyAttribute.ReplacePersistent<mvThirdPersonController>(onApplyAttributeProp, typeof(vThirdPersonController), typeof(int), targetCC, true);
                        }
                    }

                    SerializedProperty onOpenCloseInventoryProp = serializedObject.FindProperty("onOpenCloseInventory");

                    if (itemManager.onOpenCloseInventory != null && onOpenCloseInventoryProp != null)
                    {
                        itemManager.onOpenCloseInventory.ReplacePersistent<mvThirdPersonController>(onOpenCloseInventoryProp, typeof(vThirdPersonController), null, targetCC, true);

#if INVECTOR_SHOOTER
                        if (sourceShooterInput && sourceShooterInput.IsTheType<vShooterMeleeInput>())
                            itemManager.onOpenCloseInventory.ReplacePersistent<mvShooterMeleeInput>(onOpenCloseInventoryProp, typeof(vShooterMeleeInput), typeof(bool), targetShooterInput, true);
                        else
#endif
#if INVECTOR_MELEE
                            if (sourceMeleeInput && sourceMeleeInput.IsTheType<vMeleeCombatInput>())
                            itemManager.onOpenCloseInventory.ReplacePersistent<mvMeleeCombatInput>(onOpenCloseInventoryProp, typeof(vMeleeCombatInput), typeof(bool), targetMeleeInput, true);
                        else
#endif
#if INVECTOR_BASIC
                            if (sourceCCInput && sourceCCInput.IsTheType<vThirdPersonInput>())
                            itemManager.onOpenCloseInventory.ReplacePersistent<mvThirdPersonInput>(onOpenCloseInventoryProp, typeof(vThirdPersonInput), typeof(bool), targetCCInput, true);
#endif
                    }
                }
            }


#if INVECTOR_SHOOTER
            if (hasLockOnShooter)
            {
                SerializedObject serializedObject = new SerializedObject(lockOnShooter);

                if (lockOnShooter.onLockOnTarget != null && serializedObject != null)
                {
                    SerializedProperty onLockOnTargetProp = serializedObject.FindProperty("onLockOnTarget");

                    if (onLockOnTargetProp != null)
                    {
                        if (lockOnShooter && lockOnShooter.IsTheType<vLockOnShooter>())
                            lockOnShooter.onLockOnTarget.ReplacePersistent<mvShooterMeleeInput>(onLockOnTargetProp, typeof(vShooterMeleeInput), typeof(string), targetShooterInput, false);
                    }
                }
                if (lockOnShooter.onUnLockOnTarget != null && serializedObject != null)
                {
                    SerializedProperty onUnLockOnTargetProp = serializedObject.FindProperty("onUnLockOnTarget");

                    if (onUnLockOnTargetProp != null)
                    {
                        if (lockOnShooter && lockOnShooter.IsTheType<vLockOnShooter>())
                            lockOnShooter.onUnLockOnTarget.ReplacePersistent<mvShooterMeleeInput>(onUnLockOnTargetProp, typeof(vShooterMeleeInput), null, targetShooterInput, true);
                    }
                }
            }
#endif


            if (hasLadderAction && ladderAction.OnEnterLadder != null)
            {
                SerializedObject serializedObject = new SerializedObject(ladderAction);

                if (serializedObject != null)
                {
                    SerializedProperty OnEnterLadderProp = serializedObject.FindProperty("OnEnterLadder");

                    if (OnEnterLadderProp != null)
                    {
#if INVECTOR_SHOOTER
                        if (sourceShooterDrawHide != null && sourceShooterDrawHide.IsTheType<vDrawHideShooterWeapons>())
                            ladderAction.OnEnterLadder.ReplacePersistent<mvDrawHideShooterWeapons>(
                                OnEnterLadderProp, typeof(vDrawHideShooterWeapons), typeof(bool), targetShooterDrawHide, false);
#endif
                    }
                }
            }


#if INVECTOR_BASIC
            // vMessageReceiver
            if (characterObj.TryGetComponent(out messageReceiver))
            {
                SerializedObject serializedObject = new SerializedObject(messageReceiver);

                if (serializedObject != null)
                {
                    SerializedProperty prop = serializedObject.FindProperty("messagesListeners");

                    for (int i = 0; i < messageReceiver.messagesListeners.Count; i++)
                    {
                        if (prop != null)
                        {
                            SerializedProperty eventProp = prop.GetArrayElementAtIndex(i).FindPropertyRelative("onReceiveMessage");

#if INVECTOR_SHOOTER
                            if (templateType == MISEditor.TemplateType.Shooter)
                            {
                                messageReceiver.messagesListeners[i].onReceiveMessage.ReplacePersistent2<mvDrawHideShooterWeapons>(eventProp,
                                    typeof(vDrawHideShooterWeapons), typeof(bool), targetShooterDrawHide, false);
                            }
#endif
                        }
                    }
                }
            }
#endif


#if INVECTOR_SHOOTER
            if (sourceShooterDrawHide && sourceShooterDrawHide.IsTheType<vDrawHideShooterWeapons>())
                DestroyImmediate(sourceShooterDrawHide);
#endif

            if (sourceCC && sourceCC.IsTheType<vThirdPersonController>())
                DestroyImmediate(sourceCC);

#if INVECTOR_SHOOTER
            if (hasShooterManager && sourceShooterInput && sourceShooterInput.IsTheType<vShooterMeleeInput>())
            {
                DestroyImmediate(sourceShooterInput);
            }
            else
#endif
#if INVECTOR_MELEE
                if (hasMeleeManager && sourceMeleeInput && sourceMeleeInput.IsTheType<vMeleeCombatInput>())
            {
                DestroyImmediate(sourceMeleeInput);
            }
            else
#endif
#if INVECTOR_BASIC
                if (sourceCCInput && sourceCCInput.IsTheType<vThirdPersonInput>())
            {
                DestroyImmediate(sourceCCInput);
            }
#endif

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void ConvertAICharacter(GameObject characterObj)
        {
#if MIS_FSM_AI && INVECTOR_AI_TEMPLATE
            if (isValidInvectorAICharacter == false)
                return;


            if (characterObj.TryGetComponent(out sourceControlAIShooter)
                && sourceControlAIShooter.enabled
                && sourceControlAIShooter.IsTheType<vControlAIShooter>())
            {
                if (characterObj.GetComponent<mvControlAIShooter>() != null)
                    DestroyImmediate(characterObj.GetComponent<mvControlAIShooter>());
                targetControlAIShooter = characterObj.AddComponent<mvControlAIShooter>();

                MISEditor.PasteComponentValues(sourceControlAIShooter, targetControlAIShooter);
            }
            else if (characterObj.TryGetComponent(out sourceControlAIMelee)
                && sourceControlAIMelee.enabled
                && sourceControlAIMelee.IsTheType<vControlAIMelee>())
            {
                if (characterObj.GetComponent<mvControlAIMelee>() != null)
                    DestroyImmediate(characterObj.GetComponent<mvControlAIMelee>());
                targetControlAIMelee = characterObj.AddComponent<mvControlAIMelee>();

                MISEditor.PasteComponentValues(sourceControlAIMelee, targetControlAIMelee);
            }
            else if (characterObj.TryGetComponent(out sourceControlAICombat)
                && sourceControlAICombat.enabled
                && sourceControlAICombat.IsTheType<vControlAICombat>())
            {
                if (characterObj.GetComponent<mvControlAICombat>() != null)
                    DestroyImmediate(characterObj.GetComponent<mvControlAICombat>());
                targetControlAICombat = characterObj.AddComponent<mvControlAICombat>();

                MISEditor.PasteComponentValues(sourceControlAICombat, targetControlAICombat);
            }
            else if (characterObj.TryGetComponent(out sourceControlAI)
                && sourceControlAI.enabled
                && sourceControlAI.IsTheType<vControlAI>())
            {
                if (characterObj.GetComponent<mvControlAI>() != null)
                    DestroyImmediate(characterObj.GetComponent<mvControlAI>());
                targetControlAI = characterObj.AddComponent<mvControlAI>();

                MISEditor.PasteComponentValues(sourceControlAI, targetControlAI);
            }


#if INVECTOR_SHOOTER
            if (characterObj.TryGetComponent(out sourceAIShooterManager)
                && sourceAIShooterManager.enabled
                && sourceAIShooterManager.IsTheType<vAIShooterManager>())
            {
                if (characterObj.GetComponent<mvAIShooterManager>() != null)
                    DestroyImmediate(characterObj.GetComponent<mvAIShooterManager>());
                targetAIShooterManager = characterObj.AddComponent<mvAIShooterManager>();

                MISEditor.PasteComponentValues(sourceAIShooterManager, targetAIShooterManager);
            }
#endif

#if INVECTOR_MELEE || INVECTOR_SHOOTER
            if (characterObj.TryGetComponent(out sourceSimpleHolder)
                && sourceSimpleHolder.enabled
                && sourceSimpleHolder.IsTheType<vSimpleHolder>())
            {
                if (characterObj.GetComponent<mvSimpleHolder>() != null)
                    DestroyImmediate(characterObj.GetComponent<mvSimpleHolder>());
                targetSimpleHolder = characterObj.AddComponent<mvSimpleHolder>();

                MISEditor.PasteComponentValues(sourceSimpleHolder, targetSimpleHolder);
            }
#endif

            if (characterObj.TryGetComponent(out sourceAIHeadtrack)
                && sourceAIHeadtrack.enabled
                && sourceAIHeadtrack.IsTheType<vAIHeadtrack>())
            {
                if (characterObj.GetComponent<mvAIHeadtrack>() != null)
                    DestroyImmediate(characterObj.GetComponent<mvAIHeadtrack>());
                targetAIHeadtrack = characterObj.AddComponent<mvAIHeadtrack>();

                MISEditor.PasteComponentValues(sourceAIHeadtrack, targetAIHeadtrack);
            }

#if USE_MVAI_COMPANION
            if (characterObj.TryGetComponent(out sourceAICompanion)
                && sourceAICompanion.enabled
                && sourceAICompanion.IsTheType<vAICompanion>())
            {
                if (characterObj.GetComponent<mvAICompanion>() != null)
                    DestroyImmediate(characterObj.GetComponent<mvAICompanion>());
                targetAICompanion = characterObj.AddComponent<mvAICompanion>();

                MISEditor.PasteComponentValues(sourceAICompanion, targetAICompanion);
            }
#endif


            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();


            if (sourceControlAIShooter && sourceControlAIShooter.IsTheType<vControlAIShooter>())
                DestroyImmediate(sourceControlAIShooter);
            else if (sourceControlAIMelee && sourceControlAIMelee.IsTheType<vControlAIMelee>())
                DestroyImmediate(sourceControlAIMelee);
            else if (sourceControlAICombat && sourceControlAICombat.IsTheType<vControlAICombat>())
                DestroyImmediate(sourceControlAICombat);
            else if (sourceControlAI && sourceControlAI.IsTheType<vControlAI>())
                DestroyImmediate(sourceControlAI);

#if INVECTOR_SHOOTER
            if (sourceAIShooterManager && sourceAIShooterManager.IsTheType<vAIShooterManager>())
                DestroyImmediate(sourceAIShooterManager);
#endif
#if INVECTOR_MELEE || INVECTOR_SHOOTER
            if (sourceSimpleHolder && sourceSimpleHolder.IsTheType<vSimpleHolder>())
                DestroyImmediate(sourceSimpleHolder);
#endif

            if (sourceAIHeadtrack && sourceAIHeadtrack.IsTheType<vAIHeadtrack>())
                DestroyImmediate(sourceAIHeadtrack);

#if USE_MVAI_COMPANION
            if (sourceAICompanion && sourceAICompanion.IsTheType<vAICompanion>())
                DestroyImmediate(sourceAICompanion);
#endif


            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
#endif
        }
    }

    public static class PersistentExtention
    {
        public static void ReplacePersistent2<T>(this UnityEventBase sourceEvent, SerializedProperty sourceUnityEventProp,
            System.Type sourcePersistentTargetType,
            System.Type sourcePersistentArgumentType,
            T targetPersistentTarget,
            bool isDynamic)
        {
            int count = 0;

            if (sourceEvent != null)
                count = sourceEvent.GetPersistentEventCount();

            if (count == 0)
                return;

            for (int i = 0; i < count; i++)
            {
                Object sourcePersistentObject = sourceEvent.GetPersistentTarget(i);

                if (sourcePersistentObject != null && sourcePersistentObject.GetType() == sourcePersistentTargetType)
                {
                    System.Reflection.MethodInfo method = null;
                    string sourcePersistentMethodName = sourceEvent.GetPersistentMethodName(i);

                    if (sourcePersistentArgumentType == null)
                        method = UnityEventBase.GetValidMethodInfo(sourcePersistentObject, sourcePersistentMethodName, new System.Type[0]);
                    else
                        method = UnityEventBase.GetValidMethodInfo(sourcePersistentObject, sourcePersistentMethodName, new System.Type[] { sourcePersistentArgumentType });

                    if (method == null)
                        continue;

                    if (isDynamic)
                    {
                        if (sourcePersistentArgumentType == null)
                        {
                            UnityEventTools.RegisterVoidPersistentListener(sourceEvent, i,
                                System.Delegate.CreateDelegate(typeof(UnityAction), targetPersistentTarget, sourcePersistentMethodName) as UnityAction);
                        }
                        else if (sourcePersistentArgumentType == typeof(int))
                        {
                            UnityEventTools.RegisterPersistentListener<int>(sourceEvent as UnityEvent<int>, i,
                                System.Delegate.CreateDelegate(typeof(UnityAction<int>), targetPersistentTarget, sourcePersistentMethodName) as UnityAction<int>);
                        }
                        else if (sourcePersistentArgumentType == typeof(float))
                        {
                            UnityEventTools.RegisterPersistentListener<float>(sourceEvent as UnityEvent<float>, i,
                                System.Delegate.CreateDelegate(typeof(UnityAction<float>), targetPersistentTarget, sourcePersistentMethodName) as UnityAction<float>);
                        }
                        else if (sourcePersistentArgumentType == typeof(bool))
                        {
                            UnityEventTools.RegisterPersistentListener<bool>(sourceEvent as UnityEvent<bool>, i,
                                System.Delegate.CreateDelegate(typeof(UnityAction<bool>), targetPersistentTarget, sourcePersistentMethodName) as UnityAction<bool>);
                        }
                        else if (sourcePersistentArgumentType == typeof(string))
                        {
                            UnityEventTools.RegisterPersistentListener<string>(sourceEvent as UnityEvent<string>, i,
                                System.Delegate.CreateDelegate(typeof(UnityAction<string>), targetPersistentTarget, sourcePersistentMethodName) as UnityAction<string>);
                        }
                        else if (sourcePersistentArgumentType == typeof(Object))
                        {
                            UnityEventTools.RegisterPersistentListener<Object>(sourceEvent as UnityEvent<Object>, i,
                                System.Delegate.CreateDelegate(typeof(UnityAction<Object>), targetPersistentTarget, sourcePersistentMethodName) as UnityAction<Object>);
                        }
                    }
                    else
                    {
                        SerializedProperty prop = sourceUnityEventProp.GetPersistentArgumentProp2(sourcePersistentArgumentType, i);

                        if (sourcePersistentArgumentType == null)
                        {
                            UnityEventTools.RegisterVoidPersistentListener(sourceEvent, i,
                                System.Delegate.CreateDelegate(typeof(UnityAction), targetPersistentTarget, sourcePersistentMethodName) as UnityAction);
                        }
                        else if (sourcePersistentArgumentType == typeof(int))
                        {
                            UnityEventTools.RegisterIntPersistentListener(sourceEvent, i,
                                System.Delegate.CreateDelegate(typeof(UnityAction<int>), targetPersistentTarget, sourcePersistentMethodName) as UnityAction<int>, prop != null ? prop.intValue : 0);
                        }
                        else if (sourcePersistentArgumentType == typeof(float))
                        {
                            UnityEventTools.RegisterFloatPersistentListener(sourceEvent, i,
                                System.Delegate.CreateDelegate(typeof(UnityAction<float>), targetPersistentTarget, sourcePersistentMethodName) as UnityAction<float>, prop != null ? prop.floatValue : 0f);
                        }
                        else if (sourcePersistentArgumentType == typeof(bool))
                        {
                            UnityEventTools.RegisterBoolPersistentListener(sourceEvent, i,
                                System.Delegate.CreateDelegate(typeof(UnityAction<bool>), targetPersistentTarget, sourcePersistentMethodName) as UnityAction<bool>, prop != null ? prop.boolValue : false);
                        }
                        else if (sourcePersistentArgumentType == typeof(string))
                        {
                            UnityEventTools.RegisterStringPersistentListener(sourceEvent, i,
                                System.Delegate.CreateDelegate(typeof(UnityAction<string>), targetPersistentTarget, sourcePersistentMethodName) as UnityAction<string>, prop != null ? prop.stringValue : null);
                        }
                        else if (sourcePersistentArgumentType == typeof(Object))
                        {
                            UnityEventTools.RegisterObjectPersistentListener(sourceEvent, i,
                                System.Delegate.CreateDelegate(typeof(UnityAction<Object>), targetPersistentTarget, sourcePersistentMethodName) as UnityAction<Object>, prop != null ? prop.objectReferenceValue : null);
                        }
                    }
                }
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static SerializedProperty GetPersistentArgumentProp2(this SerializedProperty unityEventProp, System.Type argumentType, int index)
        {
            var persistentCallsProp = unityEventProp.FindPropertyRelative("m_PersistentCalls.m_Calls");

            if (argumentType == typeof(int))
                return persistentCallsProp.GetArrayElementAtIndex(index).FindPropertyRelative("m_Arguments.m_IntArgument");
            else if (argumentType == typeof(float))
                return persistentCallsProp.GetArrayElementAtIndex(index).FindPropertyRelative("m_Arguments.m_FloatArgument");
            else if (argumentType == typeof(bool))
                return persistentCallsProp.GetArrayElementAtIndex(index).FindPropertyRelative("m_Arguments.m_BoolArgument");
            else if (argumentType == typeof(string))
                return persistentCallsProp.GetArrayElementAtIndex(index).FindPropertyRelative("m_Arguments.m_StringArgument");
            else if (argumentType == typeof(Object))
                return persistentCallsProp.GetArrayElementAtIndex(index).FindPropertyRelative("m_Arguments.m_ObjectArgument");
            else
                return null;
        }
    }
}