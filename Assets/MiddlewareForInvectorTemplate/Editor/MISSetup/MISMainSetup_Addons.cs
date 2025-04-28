#pragma warning disable CS0219

#if INVECTOR_BASIC
using Invector;
using Invector.vCamera;
using System.Collections.Generic;
#endif
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public partial class MISMainSetup
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        bool oldSetupAllMis;
        bool oldSetupAllInvector;

        protected List<AddonSetupOption> misAddonSetupOptionList;
        protected List<AddonSetupOption> misAIAddonSetupOptionList;
        protected List<AddonSetupOption> invectorAddonSetupOptionList;

        float misSetupGroupHeight = 250f;
        float invectorSetupGroupHeight = 180f;

        Vector2 misAddonsScroll;
        Vector2 invectorAddonsScroll;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void AddonsContent()
        {
            GUILayout.BeginVertical(skin.GetStyle("WindowBG"));
            {
                EditorGUILayout.HelpBox("Setup MIS packages on your Invector main character.", MessageType.Info);

                GUILayout.Space(5);

                // ----------------------------------------------------------------------------------------------------
                // Target
                GUILayout.BeginVertical("box");
                {
                    //GUILayout.Label("<b>Target</b>", GUILayout.MaxHeight(25));
                    CheckHumanoidOption();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.MaxHeight(35));
                        {
                            MISEditor.SetToggleLabelWidth(true, 200f);
                            characterObj = EditorGUILayout.ObjectField("(MIS)INVECTOR Character", characterObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                            MISEditor.SetToggleLabelWidth(false);
                            isValidCharacter = IsValidCharacterObject(characterObj, checkHumanoidSP.boolValue);
                            isValidInvectorCharacter = IsValidInvectorCharacter();

                            MISEditor.SetToggleLabelWidth(true, 200f);
                            cameraObj = EditorGUILayout.ObjectField("Main Camera", cameraObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                            MISEditor.SetToggleLabelWidth(false);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(10);

                        if (GUILayout.Button(iconView, GUILayout.Width(30), GUILayout.Height(25)))
                        {
                            if (RefreshInvectorCharacter() == false)
                                Debug.LogWarning("Please select a INVECTOR(MIS) character in Hierarchy first.");
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.Space(15);

                // ----------------------------------------------------------------------------------------------------
                // Addons
                GUILayout.BeginVertical("box");
                {
                    CheckMISCameraStateOption();

                    GUILayout.BeginVertical("box", GUILayout.Height(misSetupGroupHeight));
                    {
                        CheckMISAddons();
                    }
                    GUILayout.EndVertical();

                    GUILayout.Space(10);

                    GUILayout.BeginVertical("box", GUILayout.Height(invectorSetupGroupHeight));
                    {
                        CheckInvectorAddons();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                EditorGUI.BeginDisabledGroup(!HasMISRefactoringDone || !isValidCharacter || !isValidInvectorCharacter);
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

                        SetupAddons(characterObj, cameraObj);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndVertical();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void LoadSetupOptions()
        {
            // ----------------------------------------------------------------------------------------------------
            // MIS
            misAddonSetupOptionList = new List<AddonSetupOption>();


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_AIRDASH, MISFeature.MIS_FEATURE_AIRDASH,
                MISFeature.MIS_AIRDASH_PATH, MISFeature.MIS_AIRDASH_OPTION_PATH, MISAddonItem,
#if MIS_AIRDASH
                AirDashSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_CARRIDER_EVP, MISFeature.MIS_FEATURE_CARRIDER_EVP,
                MISFeature.MIS_CARRIDER_EVP_PATH, MISFeature.MIS_CARRIDER_EVP_OPTION_PATH, MISAddonItem,
#if MIS_CARRIDER_EVP
                CarRiderEVPCharacterSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_CARRIDER_RCC, MISFeature.MIS_FEATURE_CARRIDER_RCC,
                MISFeature.MIS_CARRIDER_RCC_PATH, MISFeature.MIS_CARRIDER_RCC_OPTION_PATH, MISAddonItem,
#if MIS_CARRIDER_RCC
                CarRiderRCCCharacterSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_CRAWLING, MISFeature.MIS_FEATURE_CRAWLING,
                MISFeature.MIS_CRAWLING_PATH, MISFeature.MIS_CRAWLING_OPTION_PATH, MISAddonItem,
#if MIS_CRAWLING
                CrawlingSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption("MIS-FreeFlying cannot be used with MIS-SoftFlying.",
                MISFeature.MIS_PACKAGE_FREEFLYING, MISFeature.MIS_FEATURE_FREEFLYING,
                MISFeature.MIS_FREEFLYING_PATH, MISFeature.MIS_FREEFLYING_OPTION_PATH, MISAddonItem,
#if MIS_FREEFLYING
                FreeFlyingSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_GRAPPLINGHOOK, MISFeature.MIS_FEATURE_GRAPPLINGHOOK,
                MISFeature.MIS_GRAPPLINGHOOK_PATH, MISFeature.MIS_GRAPPLINGHOOK_OPTION_PATH, MISAddonItem,
#if MIS_GRAPPLINGHOOK
                GrapplingHookSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_GRAPPLINGROPE, MISFeature.MIS_FEATURE_GRAPPLINGROPE,
                MISFeature.MIS_GRAPPLINGROPE_PATH, MISFeature.MIS_GRAPPLINGROPE_OPTION_PATH, MISAddonItem,
#if MIS_GRAPPLINGROPE
                GrapplingRopeSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_GROUNDDASH, MISFeature.MIS_FEATURE_GROUNDDASH,
                MISFeature.MIS_GROUNDDASH_PATH, MISFeature.MIS_GROUNDDASH_OPTION_PATH, MISAddonItem,
#if MIS_GROUNDDASH
                GroundDashSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_HELICOPTER, MISFeature.MIS_FEATURE_HELICOPTER,
                MISFeature.MIS_HELICOPTER_PATH, MISFeature.MIS_HELICOPTER_OPTION_PATH, MISAddonItem,
#if MIS_HELICOPTER
                HelicopterCharacterSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_LANDINGROLL, MISFeature.MIS_FEATURE_LANDINGROLL,
                MISFeature.MIS_LANDINGROLL_PATH, MISFeature.MIS_LANDINGROLL_OPTION_PATH, MISAddonItem,
#if MIS_LANDINGROLL
                LandingRollSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_LEDGECLIMB1, MISFeature.MIS_FEATURE_LEDGECLIMB1,
                MISFeature.MIS_LEDGECLIMB_PATH, MISFeature.MIS_LEDGECLIMB1_OPTION_PATH, MISAddonItem,
#if MIS_LEDGECLIMB1
                LedgeClimb1Setup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_LEDGECLIMB2, MISFeature.MIS_FEATURE_LEDGECLIMB2,
                MISFeature.MIS_LEDGECLIMB2_PATH, MISFeature.MIS_LEDGECLIMB2_OPTION_PATH, MISAddonItem,
#if MIS_LEDGECLIMB2
                LedgeClimb2Setup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_LOCKON, MISFeature.MIS_FEATURE_LOCKON,
                MISFeature.MIS_LOCKON_PATH, MISFeature.MIS_LOCKON_OPTION_PATH, MISAddonItem,
#if MIS_LOCKON
                LockOnSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_LOCKON2, MISFeature.MIS_FEATURE_LOCKON2,
                MISFeature.MIS_LOCKON2_PATH, MISFeature.MIS_LOCKON2_OPTION_PATH, MISAddonItem,
#if MIS_LOCKON2
                LockOn2Setup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null, 
                MISFeature.MIS_PACKAGE_MAGICSPELL, MISFeature.MIS_FEATURE_MAGICSPELL,
                MISFeature.MIS_MAGICSPELL_PATH, MISFeature.MIS_MAGICSPELL_OPTION_PATH, MISAddonItem,
#if MIS_MAGICSPELL
                MagicSpellSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_MOTORCYCLE, MISFeature.MIS_FEATURE_MOTORCYCLE,
                MISFeature.MIS_MOTORCYCLE_PATH, MISFeature.MIS_MOTORCYCLE_OPTION_PATH, MISAddonItem,
#if MIS_MOTORCYCLE
                MotorcycleRiderSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_MULTIJUMP, MISFeature.MIS_FEATURE_MULTIJUMP,
                MISFeature.MIS_MULTIJUMP_PATH, MISFeature.MIS_MULTIJUMP_OPTION_PATH, MISAddonItem,
#if MIS_MULTIJUMP
                MultiJumpSetup
#else
                null
#endif
                ));


#if false
            misAddonSetupOptionList.Add(LoadAddonSetupOption(null, 
                MISFeature.MIS_PACKAGE_QUADRUPED, MISFeature.MIS_FEATURE_QUADRUPED,
                MISFeature.MIS_QUADRUPED_PATH, MISFeature.MIS_QUADRUPED_OPTION_PATH, MISAddonItem,
#if MIS_QUADRUPED
                null
#else
                null
#endif
                ));
#endif


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_ROWINGBOAT, MISFeature.MIS_FEATURE_ROWINGBOAT,
                MISFeature.MIS_ROWINGBOAT_PATH, MISFeature.MIS_ROWINGBOAT_OPTION_PATH, MISAddonItem,
#if MIS_ROWINGBOAT
                RowingBoatSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption("MIS-SoftFlying cannot be used with MIS-FreeFlying.",
                MISFeature.MIS_PACKAGE_SOFTFLYING, MISFeature.MIS_FEATURE_SOFTFLYING,
                MISFeature.MIS_SOFTFLYING_PATH, MISFeature.MIS_SOFTFLYING_OPTION_PATH, MISAddonItem,
#if MIS_SOFTFLYING
                SoftFlyingSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption("MIS-Swimming cannot be used with Invector Swimming add-on.",
                MISFeature.MIS_PACKAGE_SWIMMING, MISFeature.MIS_FEATURE_SWIMMING,
                MISFeature.MIS_SWIMMING_PATH, MISFeature.MIS_SWIMMING_OPTION_PATH, MISAddonItem,
#if MIS_SWIMMING
                mvSwimmingSetup
#else
                null
#endif
                ));


#if false
            misAddonSetupOptionList.Add(LoadAddonSetupOption(null, 
                MISFeature.MIS_PACKAGE_UNCHARTEDHOOK, MISFeature.MIS_FEATURE_UNCHARTEDHOOK,
                MISFeature.MIS_UNCHARTEDHOOK_PATH, MISFeature.MIS_UNCHARTEDHOOK_OPTION_PATH, MISAddonItem,
#if MIS_UNCHARTEDHOOK
                null
#else
                null
#endif
                ));
#endif


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null, 
                MISFeature.MIS_PACKAGE_VEHICLEWEAPONS, MISFeature.MIS_FEATURE_VEHICLEWEAPONS,
                MISFeature.MIS_VEHICLEWEAPONS_PATH, MISFeature.MIS_VEHICLEWEAPONS_OPTION_PATH, MISAddonItem,
#if MIS_VEHICLEWEAPONS
                null
#else
                null
#endif
                ));


#if false
            misAddonSetupOptionList.Add(LoadAddonSetupOption(null, 
                MISFeature.MIS_PACKAGE_VEHICLEWEAPONS2, MISFeature.MIS_FEATURE_VEHICLEWEAPONS2,
                MISFeature.MIS_VEHICLEWEAPONS2_PATH, MISFeature.MIS_VEHICLEWEAPONS2_OPTION_PATH, MISAddonItem,
#if MIS_VEHICLEWEAPONS2
                null
#else
                null
#endif
                ));
#endif


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_WALLRUN, MISFeature.MIS_FEATURE_WALLRUN,
                MISFeature.MIS_WALLRUN_PATH, MISFeature.MIS_WALLRUN_OPTION_PATH, MISAddonItem,
#if MIS_WALLRUN
                WallRunSetup
#else
                null
#endif
                ));


            misAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_WATERDASH, MISFeature.MIS_FEATURE_WATERDASH,
                MISFeature.MIS_WATERDASH_PATH, MISFeature.MIS_WATERDASH_OPTION_PATH, MISAddonItem,
#if MIS_WATERDASH
                WaterDashSetup
#else
                null
#endif
                ));


            // ----------------------------------------------------------------------------------------------------
            // INVECTOR
            invectorAddonSetupOptionList = new List<AddonSetupOption>();


            invectorAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_V_BUILDER, MISFeature.MIS_V_FEATURE_BUILDER,
                MISFeature.MIS_INVECTOR_BUILDER_PATH, MISFeature.MIS_INVECTOR_BUILDER_OPTION_PATH, InvectorAddonItem, 
                BuilderSetup));


            invectorAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_V_FREECLIMB, MISFeature.MIS_V_FEATURE_FREECLIMB,
                MISFeature.MIS_INVECTOR_FREECLIMB_PATH, MISFeature.MIS_INVECTOR_FREECLIMB_OPTION_PATH, InvectorAddonItem, 
                FreeClimbSetup));


            invectorAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_V_PARACHUTE, MISFeature.MIS_V_FEATURE_PARACHUTE,
                MISFeature.MIS_INVECTOR_PARACHUTE_PATH, MISFeature.MIS_INVECTOR_PARACHUTE_OPTION_PATH, InvectorAddonItem, 
                ParachuteSetup));


            invectorAddonSetupOptionList.Add(LoadAddonSetupOption("Only MIS-RowingBoat users are required to convert Invector PushAction.\n" +
                "If the Invector PushAction option is unticked, mvPushActionController will be installed.",
                MISFeature.MIS_PACKAGE_V_PUSH, MISFeature.MIS_V_FEATURE_PUSH,
                MISFeature.MIS_INVECTOR_PUSH_PATH, MISFeature.MIS_INVECTOR_PUSH_OPTION_PATH, InvectorAddonItem, 
                PushSetup));


            invectorAddonSetupOptionList.Add(LoadAddonSetupOption("MIS provides the ShooterCover Setup and its chained-action, but does not support any issues.\n" +
                "This is because the ShooterCover is in beta state.",
                MISFeature.MIS_PACKAGE_V_SHOOTERCOVER, MISFeature.MIS_V_FEATURE_SHOOTERCOVER,
                MISFeature.MIS_INVECTOR_SHOOTERCOVER_PATH, MISFeature.MIS_INVECTOR_SHOOTERCOVER_OPTION_PATH, InvectorAddonItem, 
                ShooterCoverSetup));


            invectorAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_V_ZIPLINE, MISFeature.MIS_V_FEATURE_ZIPLINE,
                MISFeature.MIS_INVECTOR_ZIPLINE_PATH, MISFeature.MIS_INVECTOR_ZIPLINE_OPTION_PATH, InvectorAddonItem, 
                ZiplineSetup));
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

                checkHumanoidSP = misSetupSO.FindProperty("checkHumanoid");
                checkHumanoidSP.boolValue = EditorGUILayout.ToggleLeft("Check Humanoid Avatar", checkHumanoidSP.boolValue, skin.label);
            }
            GUILayout.EndHorizontal();

            misSetupSO.ApplyModifiedProperties();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckMISCameraStateOption()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(30));
            {
                GUILayout.Label("<b>Addons</b>", GUILayout.MaxHeight(25));

                GUILayout.FlexibleSpace();

                useMISCameraStateSP = misSetupSO.FindProperty("useMISCameraState");
                //useMISCameraStateSP.boolValue = GUILayout.Toggle(useMISCameraStateSP.boolValue, "Use MIS Camera State");
                useMISCameraStateSP.boolValue = EditorGUILayout.ToggleLeft("Use MIS Camera State", useMISCameraStateSP.boolValue, skin.label);
            }
            GUILayout.EndHorizontal();

            misSetupSO.ApplyModifiedProperties();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckAllMISAddonsSetupOption()
        {
            EditorGUI.BeginChangeCheck();
            setupAllMISAddonsSP = misSetupSO.FindProperty("setupAllMISAddons");
            setupAllMISAddonsSP.boolValue = EditorGUILayout.ToggleLeft("All MIS Addons", setupAllMISAddonsSP.boolValue, skin.label, GUILayout.MaxHeight(25));
            if (EditorGUI.EndChangeCheck())
            {
                misSetupSO.ApplyModifiedProperties();
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckMISAddons()
        {
            CheckAllMISAddonsSetupOption();

            ToggleAllMISAddons();

            misAddonsScroll = EditorGUILayout.BeginScrollView(misAddonsScroll, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                foreach (AddonSetupOption setupOption in misAddonSetupOptionList)
                    UserSetupOption(setupOption);
            }
            EditorGUILayout.EndScrollView();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckAllInvectorAddonsSetupOption()
        {
            EditorGUI.BeginChangeCheck();
            setupAllInvectorSP = misSetupSO.FindProperty("setupAllInvectorAddons");
            setupAllInvectorSP.boolValue =
                EditorGUILayout.ToggleLeft("All INVECTOR Addons", setupAllInvectorSP.boolValue, skin.label, GUILayout.MaxHeight(25));
            if (EditorGUI.EndChangeCheck())
            {
                misSetupSO.ApplyModifiedProperties();
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckInvectorAddons()
        {
            CheckAllInvectorAddonsSetupOption();

            ToggleAllInvectorAddons();

            invectorAddonsScroll = EditorGUILayout.BeginScrollView(invectorAddonsScroll, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                foreach (AddonSetupOption setupOption in invectorAddonSetupOptionList)
                    UserSetupOption(setupOption);
            }
            EditorGUILayout.EndScrollView();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ToggleSetupProperty(AddonSetupOption setupOption, bool enable)
        {
            SerializedProperty setup = setupOption.so.FindProperty("setup");
            setup.boolValue = enable;
            setupOption.so.ApplyModifiedProperties();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ToggleAllMISAddons()
        {
            if (setupAllMISAddonsSP.boolValue == oldSetupAllMis)
                return;
            oldSetupAllMis = setupAllMISAddonsSP.boolValue;

            foreach (AddonSetupOption setupOption in misAddonSetupOptionList)
                ToggleSetupProperty(setupOption, setupAllMISAddonsSP.boolValue);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ToggleAllInvectorAddons()
        {
            if (setupAllInvectorSP.boolValue == oldSetupAllInvector)
                return;
            oldSetupAllInvector = setupAllInvectorSP.boolValue;

            foreach (AddonSetupOption setupOption in invectorAddonSetupOptionList)
                ToggleSetupProperty(setupOption, setupAllInvectorSP.boolValue);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void SetupAddons(GameObject characterObj, GameObject cameraObj)
        {
            // ----------------------------------------------------------------------------------------------------
            // Checking Dependency
            if (CheckSetupDependency(characterObj) == false)
                return;

            // ----------------------------------------------------------------------------------------------------
            // Character Conversion
            if (hasMISController == false)
                ConvertCharacter(characterObj);


            // ----------------------------------------------------------------------------------------------------
            // Invector Components
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

            // Invector Components - UI
            Transform UITransform = invectorComponentsParentObj.transform.Find("UI");
            if (UITransform == null)
            {
                GameObject UIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(MISEditor.MIS_ASSETS_PATH, "MIS/Prefabs/UI.prefab"));
                UIPrefab.Instantiate2D(Vector3.zero, invectorComponentsParentObj.transform);
            }


            // ----------------------------------------------------------------------------------------------------
            // MIS Components
            Transform misComponentParent = characterObj.transform.Find(MIS_COMPONENTS);
            if (misComponentParent == null)
                misComponentsParentObj = new GameObject(MIS_COMPONENTS);
            else
                misComponentsParentObj = misComponentParent.gameObject;
            misComponentsParentObj.transform.SetLocalParent(characterObj.transform);


            try
            {
                // ----------------------------------------------------------------------------------------------------
                // Animator
                PrepareAnimator();
                SetupBaseLayer();

                int totalSetupCount = GetSetupCount();
                int currentCount = 0;

                EditorUtility.DisplayProgressBar(
                    "Please wait...", string.Format("Remains {0}/{1}", currentCount, totalSetupCount), 0f);


                // ----------------------------------------------------------------------------------------------------
                // MIS Addons
                for (int i = 0; i < misAddonSetupOptionList.Count; i++)
                {
                    if (misAddonSetupOptionList[i].addonSetup != null && misAddonSetupOptionList[i].isValid && misAddonSetupOptionList[i].option.setup)
                    {
                        misAddonSetupOptionList[i].addonSetup(misAddonSetupOptionList[i].option, characterObj, cameraObj);

                        currentCount++;

                        EditorUtility.DisplayProgressBar(
                            "Please wait...", string.Format("Remains {0}/{1}", currentCount, totalSetupCount), currentCount / totalSetupCount);
                    }
                }


                // ----------------------------------------------------------------------------------------------------
                // Invector Addons
                for (int i = 0; i < invectorAddonSetupOptionList.Count; i++)
                {
                    if (invectorAddonSetupOptionList[i].addonSetup != null && invectorAddonSetupOptionList[i].isValid && invectorAddonSetupOptionList[i].option.setup)
                    {
                        invectorAddonSetupOptionList[i].addonSetup(invectorAddonSetupOptionList[i].option, characterObj, cameraObj);

                        currentCount++;

                        EditorUtility.DisplayProgressBar(
                            "Please wait...", string.Format("Remains {0}/{1}", currentCount, totalSetupCount), currentCount / totalSetupCount);
                    }
                }


                PositioningBaseJump();
                PositioningBaseFalling();
                PositioningBaseLanding();
                PositioningBaseLocomotion();
                PositioningBaseActions();


#if INVECTOR_BASIC
                // ----------------------------------------------------------------------------------------------------
                // Camera State
                if (useMISCameraStateSP.boolValue && cameraObj != null)
                {
                    vThirdPersonCamera thirdPersonCamera = cameraObj.GetComponentInParent<vThirdPersonCamera>();

                    if (thirdPersonCamera != null)
                    {
                        thirdPersonCamera.CameraStateList =
                            AssetDatabase.LoadAssetAtPath<vThirdPersonCameraListData>(
                                Path.Combine(MISEditor.MIS_ASSETS_PATH, "Basic Locomotion/Scripts/CharacterCreator/CameraStates/MIS@CameraState.asset"));
                    }
                }
#endif


                EditorUtility.SetDirty(animatorController);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                MISSetupCompletePopup.Open();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool CheckSetupDependency(GameObject characterObj)
        {
#if MIS_FREEFLYING && MIS_SOFTFLYING
            AddonSetupOption misFreeFlyingSetupOption = misAddonSetupOptionList.Find(x => x.addonName.Equals(MISFeature.MIS_PACKAGE_FREEFLYING));
            AddonSetupOption misSoftFlyingSetupOption = misAddonSetupOptionList.Find(x => x.addonName.Equals(MISFeature.MIS_PACKAGE_SOFTFLYING));

            if (misFreeFlyingSetupOption != null && misFreeFlyingSetupOption.option.setup && misSoftFlyingSetupOption != null && misSoftFlyingSetupOption.option.setup)
            {
                Debug.LogWarning("MIS-FreeFlying and MIS-SoftFlying cannot be used together.");
                return false;
            }

            if (characterObj.TryGetComponent(out mvFreeFlying misFreeFlying) && misSoftFlyingSetupOption.option.setup)
            {
                Debug.LogWarning("MIS-FreeFlying already has been installed. It cannot be used with MIS-SoftFlying.");
                return false;
            }

            if (characterObj.TryGetComponent(out mvSoftFlying softFlying) && misFreeFlyingSetupOption.option.setup)
            {
                Debug.LogWarning("MIS-SoftFlying already has been installed. It cannot be used with MIS-FreeFlying.");
                return false;
            }
#endif

#if MIS_LEDGECLIMB1 && MIS_LEDGECLIMB2
            AddonSetupOption misLedgeClimb1SetupOption = misAddonSetupOptionList.Find(x => x.addonName.Equals(MISFeature.MIS_PACKAGE_LEDGECLIMB1));
            AddonSetupOption misLedgeClimb2SetupOption = misAddonSetupOptionList.Find(x => x.addonName.Equals(MISFeature.MIS_PACKAGE_LEDGECLIMB2));

            if (misLedgeClimb1SetupOption != null && misLedgeClimb1SetupOption.option.setup && misLedgeClimb2SetupOption != null && misLedgeClimb2SetupOption.option.setup)
            {
                Debug.LogWarning("MIS-LedgeClimb1 and MIS-LedgeClimb2 cannot be used together.");
                return false;
            }
#endif

#if MIS_LOCKON && MIS_LOCKON2
            AddonSetupOption misLockOn1SetupOption = misAddonSetupOptionList.Find(x => x.addonName.Equals(MISFeature.MIS_PACKAGE_LOCKON));
            AddonSetupOption misLockOn2SetupOption = misAddonSetupOptionList.Find(x => x.addonName.Equals(MISFeature.MIS_PACKAGE_LOCKON2));

            if (misLockOn1SetupOption != null && misLockOn1SetupOption.option.setup && misLockOn2SetupOption != null && misLockOn2SetupOption.option.setup)
            {
                Debug.LogWarning("MIS-LockOn and MIS-LockOn2 cannot be used together.");
                return false;
            }
#endif

            /*
#if MIS_SWIMMING
            AddonSetupOption misSwimmingSetupOption = misAddonSetupOptionList.Find(x => x.addonName.Equals(MISFeature.MIS_PACKAGE_SWIMMING));

            if (characterObj.TryGetComponent(out Invector.vCharacterController.vActions.vSwimming invectorSwimming) && misSwimmingSetupOption.option.setup)
            {
                Debug.LogWarning("Invector Swimming already has been installed. It cannot be used with MIS-Swimming.");
                return false;
            }
#endif
            */

            return true;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual int GetSetupCount()
        {
            int count = 0;

            // MIS
            foreach (AddonSetupOption setupOption in misAddonSetupOptionList)
            {
                if (setupOption.isValid && setupOption.option.setup)
                    count++;
            }

            // INVECTOR
            foreach (AddonSetupOption setupOption in invectorAddonSetupOptionList)
            {
                if (setupOption.isValid && setupOption.option.setup)
                    count++;
            }

            return count;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected void PositioningBaseJump()
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected void PositioningBaseFalling()
        {
            AnimatorState targetState = null;
            int horizontal = 0;
            int vertical = 1;

#if MIS_CRAWLING
            if (base_FallingSM.SetStateRelativePosition(base_CrawlFalling, horizontal, vertical))
                vertical++;
#endif

#if MIS_FREEFLYING
            if (base_FallingSM.SetStateRelativePosition(base_FF_HardLanding, horizontal, vertical))
                vertical++;
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected void PositioningBaseLanding()
        {
            AnimatorState targetState = null;
            int horizontal = 0;
            int vertical = 1;

#if MIS_CRAWLING
            if (base_LandingSM.SetStateRelativePosition(base_CrawlLandHigh, horizontal, vertical))
                vertical++;
            if (base_LandingSM.SetStateRelativePosition(base_CrawlLandLow, horizontal, vertical))
                vertical++;
#endif

#if MIS_FREEFLYING
            if (base_LandingSM.SetStateRelativePosition(base_FF_HardLand, horizontal, vertical))
                vertical++;
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected void PositioningBaseLocomotion()
        {
#if MIS_CRAWLING
            // Base - Locomotion
            var base_CrawlSM = base_LocomotionSM.FindStateMachine(STATE_CRAWLING);

            if (base_CrawlSM != null)
            {
                base_LocomotionSM.SetExitPosition(new Vector3(820, 0, 0));
                base_LocomotionSM.SetStateMachineRelativePosition(base_CrawlSM, 5, -1);
            }
#endif

#if MIS_WALLRUN
            // Base - Locomotion
            var base_WallRunSM = base_LocomotionSM.FindStateMachine(STATE_WALLRUN);

            if (base_WallRunSM != null)
            {
                base_LocomotionSM.SetExitPosition(new Vector3(820, 0, 0));
                base_LocomotionSM.SetStateMachineRelativePosition(base_WallRunSM, 5, 1);
            }
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected void PositioningBaseActions()
        {
            AnimatorStateMachine targetStateMachine = null;
            int horizontal = 0;
            int vertical = 2;

#if MIS_FREEFLYING
            if ((targetStateMachine = base_Root.FindStateMachine(STATE_FREEFLYING)) != null)
                if (base_Root.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                    vertical++;
#endif
#if MIS_MOTORCYCLE
            if ((targetStateMachine = base_Root.FindStateMachine(STATE_MOTORCYCLE)) != null)
                if (base_Root.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                    vertical++;
#endif

            if (base_ActionsSM != null)
            {
                horizontal = 5;
                vertical = -1;

#if MIS_AIRDASH
                if ((targetStateMachine = base_ActionsSM.FindStateMachine(STATE_AIRDASH)) != null)
                    if (base_ActionsSM.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                        vertical++;
#endif

#if MIS_GRAPPLINGHOOK
                if ((targetStateMachine = base_ActionsSM.FindStateMachine(STATE_GRAPPLINHOOK)) != null)
                    if (base_ActionsSM.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                        vertical++;
#endif

#if MIS_GRAPPLINGROPE
                if ((targetStateMachine = base_ActionsSM.FindStateMachine(STATE_GRAPPLINGROPE)) != null)
                    if (base_ActionsSM.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                        vertical++;
#endif

#if MIS_GROUNDDASH
                if ((targetStateMachine = base_ActionsSM.FindStateMachine(STATE_GROUNDDASH)) != null)
                    if (base_ActionsSM.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                        vertical++;
#endif

#if MIS_LANDINGROLL
                if ((targetStateMachine = base_ActionsSM.FindStateMachine(STATE_LANDINGROLL)) != null)
                    if (base_ActionsSM.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                        vertical++;
#endif

#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
                if ((targetStateMachine = base_ActionsSM.FindStateMachine(STATE_LEDGECLIMB)) != null)
                    if (base_ActionsSM.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                        vertical++;
#endif

#if MIS_WATERDASH
                if ((targetStateMachine = base_ActionsSM.FindStateMachine(STATE_WATERDASH)) != null)
                    if (base_ActionsSM.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                        vertical++;
#endif

                horizontal = 7;
                vertical = -1;

#if MIS_INVECTOR_BUILDER
                if ((targetStateMachine = base_ActionsSM.FindStateMachine(MISFeature.MIS_PACKAGE_V_BUILDER)) != null)
                    if (base_ActionsSM.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                        vertical++;
#endif
#if MIS_INVECTOR_FREECLIMB
                if ((targetStateMachine = base_ActionsSM.FindStateMachine(MISFeature.MIS_PACKAGE_V_FREECLIMB)) != null)
                    if (base_ActionsSM.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                        vertical++;
#endif
#if MIS_INVECTOR_PARACHUTE
                if ((targetStateMachine = base_ActionsSM.FindStateMachine(MISFeature.MIS_PACKAGE_V_PARACHUTE)) != null)
                    if (base_ActionsSM.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                        vertical++;
#endif
#if MIS_INVECTOR_PUSH
                if ((targetStateMachine = base_ActionsSM.FindStateMachine(MISFeature.MIS_PACKAGE_V_PUSH)) != null)
                    if (base_ActionsSM.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                        vertical++;
#endif
#if MIS_INVECTOR_ZIPLINE
                if ((targetStateMachine = base_ActionsSM.FindStateMachine(MISFeature.MIS_PACKAGE_V_ZIPLINE)) != null)
                    if (base_ActionsSM.SetStateMachineRelativePosition(targetStateMachine, horizontal, vertical))
                        vertical++;
#endif
            }
        }
    }
}