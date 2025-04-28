using Invector;
using Invector.vCharacterController;
#if INVECTOR_MELEE
using Invector.vMelee;
#endif
#if INVECTOR_AI_TEMPLATE && MIS_FSM_AI
using Invector.vCharacterController.AI;
using Invector.vCharacterController.AI.FSMBehaviour;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static com.mobilin.games.MISEditor;
using UnityEngine.Audio;
using System.IO;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public class MISFSMAICharacterSetup : MISSetupBase
    {
#if MIS_FSM_AI && INVECTOR_BASIC && INVECTOR_AI_TEMPLATE
        // ----------------------------------------------------------------------------------------------------
        // 
        protected vFSMBehaviour fsmBehaviour;
        protected FSMCharacterType fsmCharacterType;
        //protected LayerMask detectLayer;
        //protected int tagMask;
        //string[] tags;

        protected bool checkHumanoidAvatar;

        protected bool useHeadtrack;

#if INVECTOR_SHOOTER
        protected bool useThrowObject;
#if MIS_INVECTOR_SHOOTERCOVER
        protected bool useShooterCover;
#endif
#endif

#if INVECTOR_MELEE
        protected bool useSimpleHolder;
#endif

        protected bool useCompanion;

        protected bool useNoiseListener;
        protected bool useDestroyGameObject;


        // ----------------------------------------------------------------------------------------------------
        // 
        Editor characterPreview;
        GameObject newCharacterObj;
        GameObject modelParentObj;
        GameObject eyeObj;

        Animator newAnimator;
        Rigidbody newRigidbody;
        CapsuleCollider newCapsuleCollider;
        NavMeshAgent newNavMeshAgent;
        vFSMBehaviourController newFSMBehaviourController;
        mvAIHeadtrack newAIHeadtrack;

#if INVECTOR_SHOOTER
        vAIShooterManager newAIShooterManager;
        vAIThrowObject newAIThrowObject;
        GameObject throwStartObj;
#if MIS_INVECTOR_SHOOTERCOVER
        vAICover newAICover;
#endif
#endif

#if INVECTOR_MELEE
        vMeleeManager newMeleeManager;
        vSimpleHolder newSimpleHolder;
#endif

        vAICompanion newAICompanion;

        vHitDamageParticle newHitDamageParticle;
        vMessageReceiver newMessageReceiver;
        vAINoiseListener newAINoiseListener;
        vDestroyGameObject newDestroyGameObject;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        [MenuItem("Tools/MIS/FSM AI Character Setup", false, (int)MISEditor.MISMenuItem.SubSetup)]
        public static void ShowWindow()
        {
            GetWindow(typeof(MISFSMAICharacterSetup), false, "MIS FSM AI Character Setup");
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void OnEnable()
        {
            base.OnEnable();

            minWidth = 500;
            minHeight = 650;
            maxSize = new Vector2(minWidth, minHeight);
            minSize = maxSize;

            //tags = InternalEditorUtility.tags;

            useHeadtrack = true;
            useNoiseListener = true;
            useDestroyGameObject = true;

            if (RefreshCharacter())
                characterPreview = Editor.CreateEditor(characterObj);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void OnGUI()
        {
            base.OnGUI();

            GUILayout.BeginVertical(MISFeature.MIS_PACKAGE_AI_FSM_CHARACTER_SETUP, skin.GetStyle("WindowBG"), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginVertical("box");
                {
                    EditorGUILayout.HelpBox("<Recommended Setup Sequence>\n1. MIS Ragdoll Remover\n2. MIS FSM AI Character Setup\n3. Invector Ragdoll Setup and assign an AudioSource", MessageType.Info);

                    GUILayout.Space(5);

                    //GUILayout.Label("<b>Target</b>", GUILayout.MaxHeight(25));
                    CheckHumanoidOption();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.BeginVertical();
                        {
                            MISEditor.SetToggleLabelWidth(true, 200f);
                            {
                                EditorGUI.BeginChangeCheck();
                                characterObj = EditorGUILayout.ObjectField("Humanoid Character", characterObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                                isValidCharacter = IsValidCharacterObject(characterObj, checkHumanoidAvatar);
                                isValidInvectorCharacter = IsInvectorCharacter();
                                isValidInvectorAICharacter = IsFSMCharacter();
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (isValidCharacter && !isValidInvectorCharacter && !isValidInvectorAICharacter)
                                        characterPreview = Editor.CreateEditor(characterObj);
                                }
                            }
                            MISEditor.SetToggleLabelWidth(false);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(10);

                        if (GUILayout.Button(iconView, GUILayout.Width(30), GUILayout.Height(25)))
                        {
                            if (RefreshCharacter() == false)
                            {
                                Debug.LogWarning("Please select a Humanoid character in Hierarchy first.");
                            }
                            else
                            {
                                isValidCharacter = IsValidCharacterObject(characterObj, checkHumanoidAvatar);
                                isValidInvectorCharacter = IsInvectorCharacter();
                                isValidInvectorAICharacter = IsFSMCharacter();

                                if (isValidCharacter && !isValidInvectorCharacter && !isValidInvectorAICharacter)
                                    characterPreview = Editor.CreateEditor(characterObj);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);
                }
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    MISEditor.SetToggleLabelWidth(true, 200f);
                    {
                        fsmAITemplateType =
                            (FSMTemplateType)EditorGUILayout.EnumPopup("Template Type", fsmAITemplateType);

                        fsmBehaviour =
                            EditorGUILayout.ObjectField("FSM Behaviour", fsmBehaviour, typeof(vFSMBehaviour), true, GUILayout.ExpandWidth(true)) as vFSMBehaviour;

                        fsmCharacterType =
                            (FSMCharacterType)EditorGUILayout.EnumPopup("FSM Character Type", fsmCharacterType);

                        //detectLayer = EditorGUILayout.LayerField("Detect Layer", detectLayer);
                        //tagMask = EditorGUILayout.MaskField("Detect Tag", tagMask, tags);

                        GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        {
                            useHeadtrack = EditorGUILayout.Toggle("Use Headtrack", useHeadtrack);

#if INVECTOR_SHOOTER
                            if (fsmAITemplateType == FSMTemplateType.Shooter)
                                useThrowObject = EditorGUILayout.Toggle("Use ThrowObject", useThrowObject);

#if MIS_INVECTOR_SHOOTERCOVER
                            if (fsmAITemplateType == FSMTemplateType.Shooter)
                                useShooterCover = EditorGUILayout.Toggle("Use ShooterCover", useShooterCover);
#endif
#endif

#if INVECTOR_MELEE
                            if (fsmAITemplateType == FSMTemplateType.Combat || fsmAITemplateType == FSMTemplateType.Melee || fsmAITemplateType == FSMTemplateType.Shooter)
                                useSimpleHolder = EditorGUILayout.Toggle("Use SimpleHolder", useSimpleHolder);
#endif

                            if (fsmCharacterType == FSMCharacterType.Companion)
                                useCompanion = EditorGUILayout.Toggle("Use Companion", useCompanion);
                            else
                                useCompanion = false;

                            useNoiseListener = EditorGUILayout.Toggle("Use NoiseListener", useNoiseListener);
                            useDestroyGameObject = EditorGUILayout.Toggle("Use DestroyGameObject", useDestroyGameObject);
                        }
                        GUILayout.EndVertical();
                    }
                    MISEditor.SetToggleLabelWidth(false);

                    GUILayout.FlexibleSpace();

                    if (characterPreview != null && isValidCharacter && !isValidInvectorCharacter && !isValidInvectorAICharacter)
                        characterPreview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(250, 250), "window");

                    GUILayout.Space(10);

                    EditorGUI.BeginDisabledGroup(!isValidCharacter || isValidInvectorCharacter || isValidInvectorAICharacter || fsmAITemplateType == FSMTemplateType.None || fsmBehaviour == null);
                    {
                        if (GUILayout.Button("Setup"))
                        {
#if UNITY_2018_3_OR_NEWER
                            PrefabAssetType m_AssetType = PrefabUtility.GetPrefabAssetType(characterObj);

                            if (m_AssetType != PrefabAssetType.NotAPrefab)
                            {
                                GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(characterObj);
                                PrefabUtility.UnpackPrefabInstance(prefabRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                            }
#endif

                            Setup();
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
        protected virtual void CheckHumanoidOption()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(30));
            {
                GUILayout.Label("<b>Target</b>", GUILayout.MaxHeight(25));

                GUILayout.FlexibleSpace();

                checkHumanoidAvatar = EditorGUILayout.ToggleLeft("Check Humanoid Avatar", checkHumanoidAvatar, skin.label);
                //checkHumanoidAvatar = EditorGUILayout.Toggle("Check Humanoid", checkHumanoidAvatar);
            }
            GUILayout.EndHorizontal();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void Setup()
        {
            SetupStructure();
            SetupComponents();

            AssetDatabase.Refresh();

            MISSetupCompletePopup.Open();

            Selection.activeGameObject = throwStartObj;
            Selection.activeGameObject = eyeObj;

            Debug.Log("Please adjust Eye and ThrowStartPivot position.");
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void SetupStructure()
        {
            newCharacterObj = new GameObject("New FSM " + characterObj.name);
            newCharacterObj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            modelParentObj = new GameObject("3D Model");
            modelParentObj.transform.SetLocalParent(newCharacterObj.transform);

            characterObj.transform.SetLocalParent(modelParentObj.transform);

            invectorComponentsParentObj = new GameObject(INVECTOR_COMPONENTS);
            invectorComponentsParentObj.transform.SetLocalParent(newCharacterObj.transform);

            misComponentsParentObj = new GameObject(MIS_COMPONENTS);
            misComponentsParentObj.transform.SetLocalParent(newCharacterObj.transform);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void SetupComponents()
        {
            // ----------------------------------------------------------------------------------------------------
            // Layer & Tag
            if (fsmCharacterType == FSMCharacterType.Companion)
            {
                newCharacterObj.layer = LayerMask.NameToLayer(MISEditorTagLayer.LAYER_COMPANION_AI);
                newCharacterObj.tag = MISEditorTagLayer.TAG_COMPANION_AI;
            }
            else if (fsmCharacterType == FSMCharacterType.Enemy)
            {
                newCharacterObj.layer = LayerMask.NameToLayer(MISEditorTagLayer.LAYER_ENEMY);
                newCharacterObj.tag = MISEditorTagLayer.TAG_ENEMY;
            }
            else if (fsmCharacterType == FSMCharacterType.Boss)
            {
                newCharacterObj.layer = LayerMask.NameToLayer(MISEditorTagLayer.LAYER_ENEMY);
                newCharacterObj.tag = MISEditorTagLayer.TAG_BOSS;
            }
            else
            {
                newCharacterObj.layer = LayerMask.NameToLayer(MISEditorTagLayer.LAYER_DEFAULT);
                newCharacterObj.tag = default;
            }


            // ----------------------------------------------------------------------------------------------------
            // Animator
            newAnimator = newCharacterObj.AddComponent<Animator>();
            newAnimator.runtimeAnimatorController = characterAnimator.runtimeAnimatorController;
            newAnimator.avatar = characterAnimator.avatar;
            newAnimator.updateMode = AnimatorUpdateMode.Normal;
            newAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;


            // ----------------------------------------------------------------------------------------------------
            // Eye
            eyeObj = new GameObject("Eye");
            eyeObj.transform.SetLocalParent(newAnimator.GetBoneTransform(HumanBodyBones.Head));


            // ----------------------------------------------------------------------------------------------------
            // Clear
            Component[] components = characterObj.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].GetType() != typeof(Transform))
                    DestroyImmediate(components[i]);
            }


            // ----------------------------------------------------------------------------------------------------
            // Rigidbody
            newRigidbody = newCharacterObj.AddComponent<Rigidbody>();
            newRigidbody.mass = 50f;
            newRigidbody.drag = 0f;
            newRigidbody.angularDrag = 0.05f;
            newRigidbody.useGravity = true;
            newRigidbody.isKinematic = false;
            newRigidbody.interpolation = RigidbodyInterpolation.None;
            newRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            newRigidbody.constraints = RigidbodyConstraints.FreezeRotation;


            // ----------------------------------------------------------------------------------------------------
            // CapsuleCollider
            newCapsuleCollider = newCharacterObj.AddComponent<CapsuleCollider>();
            newCapsuleCollider.isTrigger = false;
            newCapsuleCollider.center = new Vector3(0f, 0.9f, 0f);
            newCapsuleCollider.radius = 0.3f;
            newCapsuleCollider.height = 1.8f;
            newCapsuleCollider.direction = 1;   // Y axis


            // ----------------------------------------------------------------------------------------------------
            // NavMeshAgent
            newNavMeshAgent = newCharacterObj.AddComponent<NavMeshAgent>();
            newNavMeshAgent.agentTypeID = 0;
            newNavMeshAgent.baseOffset = 0f;
            newNavMeshAgent.speed = 1f;
            newNavMeshAgent.angularSpeed = 200f;
            newNavMeshAgent.acceleration = 8f;
            newNavMeshAgent.stoppingDistance = 1f;
            newNavMeshAgent.autoBraking = false;
            newNavMeshAgent.radius = 0.3f;
            newNavMeshAgent.height = 1.8f;
            newNavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            newNavMeshAgent.avoidancePriority = 40;
            newNavMeshAgent.autoTraverseOffMeshLink = true;
            newNavMeshAgent.autoRepath = false;
            newNavMeshAgent.areaMask = 1;


            // ----------------------------------------------------------------------------------------------------
            // vFSMBehaviourController
            newFSMBehaviourController = newCharacterObj.AddComponent<vFSMBehaviourController>();
            newFSMBehaviourController.fsmBehaviour = fsmBehaviour;


            // ----------------------------------------------------------------------------------------------------
            // TemplateType
            if (fsmAITemplateType == FSMTemplateType.Shooter)
            {
                mvControlAIShooter shooterControlAI = newCharacterObj.AddComponent<mvControlAIShooter>();

                // Health
                shooterControlAI.removeComponentsAfterDie = true;

                // Agent
                //shooterControlAI.aceleration = 50f;
                shooterControlAI.stopingDistance = 0.5f;

                // Detection
                shooterControlAI.detectionPointReference = eyeObj.transform;
            }
            else if (fsmAITemplateType == FSMTemplateType.Melee)
            {
                mvControlAIMelee meleeControlAI = newCharacterObj.AddComponent<mvControlAIMelee>();

                // Health
                meleeControlAI.removeComponentsAfterDie = true;

                // Agent
                //meleeControlAI.aceleration = 50f;
                meleeControlAI.stopingDistance = 0.5f;

                // Detection
                meleeControlAI.detectionPointReference = eyeObj.transform;
            }
            else if (fsmAITemplateType == FSMTemplateType.Combat)
            {
                mvControlAICombat combatControlAI = newCharacterObj.AddComponent<mvControlAICombat>();

                // Health
                combatControlAI.removeComponentsAfterDie = true;

                // Agent
                //combatControlAI.aceleration = 50f;
                combatControlAI.stopingDistance = 0.5f;

                // Detection
                combatControlAI.detectionPointReference = eyeObj.transform;
            }
            else //if (fsmAITemplateType == FSMTemplateType.Basic)
            {
                mvControlAI controlAI = newCharacterObj.AddComponent<mvControlAI>();

                // Health
                controlAI.removeComponentsAfterDie = true;

                // Agent
                //controlAI.aceleration = 50f;
                controlAI.stopingDistance = 0.5f;

                // Detection
                controlAI.detectionPointReference = eyeObj.transform;
            }


            // ----------------------------------------------------------------------------------------------------
            // vAIHeadtrack
            newAIHeadtrack = newCharacterObj.AddComponent<mvAIHeadtrack>();
            newAIHeadtrack.eyes = eyeObj.transform;


#if INVECTOR_SHOOTER
            // ----------------------------------------------------------------------------------------------------
            // vAIShooterManager
            newAIShooterManager = newCharacterObj.AddComponent<vAIShooterManager>();
            newAIShooterManager.damageLayer = 
                1 << MISRuntimeTagLayer.LAYER_DEFAULT
                | 1 << MISRuntimeTagLayer.LAYER_BODYPART;


            // ----------------------------------------------------------------------------------------------------
            // vAIThrowObject
            if (useThrowObject)
            {
                newAIThrowObject = newCharacterObj.AddComponent<vAIThrowObject>();

                throwStartObj = new GameObject("ThrowStartPivot");
                throwStartObj.transform.SetLocalParent(newAnimator.GetBoneTransform(HumanBodyBones.RightHand));

                newAIThrowObject.defaultThrowStartPoint = throwStartObj.transform;
            }


#if MIS_INVECTOR_SHOOTERCOVER
            // ----------------------------------------------------------------------------------------------------
            // vAICover
            if (useShooterCover)
            {
                newAICover = newCharacterObj.AddComponent<vAICover>();
                newAICover.getCoverRange = 30f;
                newAICover.timeToChangeCover = new Vector2(10f, 20f);
                newAICover.coverLayer = 1 << MISRuntimeTagLayer.LAYER_TRIGGERS;
            }
#endif
#endif


#if INVECTOR_MELEE
            // ----------------------------------------------------------------------------------------------------
            // vMeleeManager
            newMeleeManager = newCharacterObj.AddComponent<vMeleeManager>();


            // ----------------------------------------------------------------------------------------------------
            // vSimpleHolder
            if (useSimpleHolder)
                newSimpleHolder = newCharacterObj.AddComponent<vSimpleHolder>();
#endif


            // ----------------------------------------------------------------------------------------------------
            // vAICompanion
            if (useCompanion)
            {
                newAICompanion = newCharacterObj.AddComponent<vAICompanion>();
                newAICompanion.maxFriendDistance = 5f;
                newAICompanion.minFriendDistance = 1.5f;
            }


            // ----------------------------------------------------------------------------------------------------
            // vHitDamageParticle
            newHitDamageParticle = newCharacterObj.AddComponent<vHitDamageParticle>();


            // ----------------------------------------------------------------------------------------------------
            // vMessageReceiver
            newMessageReceiver = newCharacterObj.AddComponent<vMessageReceiver>();


            // ----------------------------------------------------------------------------------------------------
            // AudioSource
            GameObject audioSourceObj = new GameObject("AudioSource");
            audioSourceObj.transform.SetLocalParent(invectorComponentsParentObj.transform);

            AudioSource audioSource = audioSourceObj.AddComponent<AudioSource>();
            AudioMixer audioMixer =
                AssetDatabase.LoadAssetAtPath<AudioMixer>(Path.Combine(MISEditor.MIS_ASSETS_PATH, "MIS/Scripts/VFX/Audio/MISAudioMixer.mixer"));
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
            audioSource.playOnAwake = false;


            // ----------------------------------------------------------------------------------------------------
            // vAINoiseListener
            if (useNoiseListener)
                newAINoiseListener = newCharacterObj.AddComponent<vAINoiseListener>();


            // ----------------------------------------------------------------------------------------------------
            // vDestroyGameObject
            if (useDestroyGameObject)
            {
                newDestroyGameObject = newCharacterObj.AddComponent<vDestroyGameObject>();
                newDestroyGameObject.enabled = false;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool IsInvectorCharacter(bool showHelpBox = true)
        {
            if (characterObj == null)
                return false;

            if (((characterObj.TryGetComponent(out vThirdPersonController cc) && cc.IsTheType<vThirdPersonController>())
                || (characterObj.TryGetComponent(out mvThirdPersonController mcc) && mcc.IsTheType<mvThirdPersonController>())))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has a ThirdPerson Invector Controller.", MessageType.Error);

                return true;
            }

#if INVECTOR_SHOOTER
            if ((characterObj.TryGetComponent(out vShooterMeleeInput shooterInput) && shooterInput.IsTheType<vShooterMeleeInput>())
                || (characterObj.TryGetComponent(out mvShooterMeleeInput mshooterInput) && mshooterInput.IsTheType<mvShooterMeleeInput>()))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has a ThirdPerson Invector Controller.", MessageType.Error);

                return true;
            }
            else
#endif
#if INVECTOR_MELEE
            if ((characterObj.TryGetComponent(out vMeleeCombatInput meleeInput) && meleeInput.IsTheType<vMeleeCombatInput>())
                || (characterObj.TryGetComponent(out mvMeleeCombatInput mmeleeInput) && mmeleeInput.IsTheType<mvMeleeCombatInput>()))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has a ThirdPerson Invector Controller.", MessageType.Error);

                return true;
            }
            else
#endif
#if INVECTOR_BASIC
            if ((characterObj.TryGetComponent(out vThirdPersonInput tpInput) && tpInput.IsTheType<vThirdPersonInput>())
                || (characterObj.TryGetComponent(out mvThirdPersonInput mtpInput) && mtpInput.IsTheType<mvThirdPersonInput>()))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has a ThirdPerson Invector Controller.", MessageType.Error);

                return true;
            }
#endif

            return false;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool IsFSMCharacter(bool showHelpBox = true)
        {
            if (characterObj == null)
                return false;

            if (characterObj.TryGetComponent(out vFSMBehaviourController fsm))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has an Invector FSM Behaviour Controller.", MessageType.Error);

                return true;
            }

            if ((characterObj.TryGetComponent(out vControlAIShooter shooterAI) && shooterAI.IsTheType<vControlAIShooter>()) ||
                (characterObj.TryGetComponent(out mvControlAIShooter misShooterAI) && misShooterAI.IsTheType<mvControlAIShooter>()))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has an Invector AI Controller.", MessageType.Error);

                return true;
            }
            else if ((characterObj.TryGetComponent(out vControlAIMelee meleeAI) && meleeAI.IsTheType<vControlAIMelee>()) ||
                (characterObj.TryGetComponent(out mvControlAIMelee misMeleeAI) && misMeleeAI.IsTheType<mvControlAIMelee>()))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has an Invector AI Controller.", MessageType.Error);

                return true;
            }
            else if ((characterObj.TryGetComponent(out vControlAICombat combatAI) && combatAI.IsTheType<vControlAICombat>()) ||
                (characterObj.TryGetComponent(out mvControlAICombat misCombatAI) && misCombatAI.IsTheType<mvControlAICombat>()))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has an Invector AI Controller.", MessageType.Error);

                return true;
            }
            else if ((characterObj.TryGetComponent(out vControlAI basicAI) && basicAI.IsTheType<vControlAI>()) ||
                (characterObj.TryGetComponent(out mvControlAI misBasicAI) && misBasicAI.IsTheType<mvControlAI>()))
            {
                if (showHelpBox)
                    EditorGUILayout.HelpBox("This character has an Invector AI Controller.", MessageType.Error);

                return true;
            }

            return false;
        }
#endif
    }
}
