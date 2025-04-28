#pragma warning disable CS0219

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public partial class MISMainSetup
    {
        // ----------------------------------------------------------------------------------------------------
        // MIS AI Addons
        bool oldSetupAllAI;

        mvAIAddonSetupOption aiAddonSetupOption;
        SerializedObject aiAddonSetupSO;
        SerializedProperty setupAllAIAddonsSP;

        Vector2 misAIAddonsScroll;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void AIContent()
        {
            GUILayout.BeginVertical(skin.GetStyle("WindowBG"));
            {
                EditorGUILayout.HelpBox("Setup MIS AI packages on your Invector FSM AI character.", MessageType.Info);

                GUILayout.Space(5);

                // ----------------------------------------------------------------------------------------------------
                // Target
                GUILayout.BeginVertical("box");
                {
                    //GUILayout.Label("<b>Target</b>", GUILayout.MaxHeight(25));
                    CheckHumanoidOption();

                    GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true), GUILayout.MaxHeight(35));
                    {
                        GUILayout.BeginVertical();
                        {
                            MISEditor.SetToggleLabelWidth(true, 200f);
                            characterObj = EditorGUILayout.ObjectField("(MIS)INVECTOR AI Character", characterObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                            MISEditor.SetToggleLabelWidth(false);
                            isValidCharacter = IsValidCharacterObject(characterObj, checkHumanoidSP.boolValue);
                            isValidInvectorAICharacter = IsValidInvectorFSMCharacter();
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(10);

                        if (GUILayout.Button(iconView, GUILayout.Width(30), GUILayout.Height(25)))
                        {
                            if (RefreshCharacter() == false)
                            {
                                Debug.LogWarning("Please select a INVECTOR(MIS) AI character in Hierarchy first.");
                            }
                            else
                            {
                                isValidCharacter = IsValidCharacterObject(characterObj, checkHumanoidSP.boolValue);
                                isValidInvectorAICharacter = IsValidInvectorFSMCharacter();
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.Space(10);

                // ----------------------------------------------------------------------------------------------------
                // AI Addons
                GUILayout.BeginVertical("box");
                {
                    CheckMISAIAddons();
                }
                GUILayout.EndVertical();

#if INVECTOR_AI_TEMPLATE
                EditorGUI.BeginDisabledGroup(HasMISAIRefactoringDone == MIS_AI_REFACTORING.Required || !isValidCharacter || !isValidInvectorAICharacter);
#else
                EditorGUI.BeginDisabledGroup(true);
#endif
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

                        SetupAIAddons(characterObj);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndVertical();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void LoadAISetupOptions()
        {
            // ----------------------------------------------------------------------------------------------------
            // AI Addons
            aiAddonSetupOption = AssetDatabase.LoadAssetAtPath<mvAIAddonSetupOption>(
                Path.Combine(MISEditor.MIS_EDITOR_PATH, "MISSetup/MISAIAddon/AIAddonSetupOptionData.asset"));
            if (aiAddonSetupOption != null && aiAddonSetupSO == null)
                aiAddonSetupSO = new SerializedObject(aiAddonSetupOption);


            misAIAddonSetupOptionList = new List<AddonSetupOption>();

#if MIS_FSM_AI
            misAIAddonSetupOptionList.Add(LoadAddonSetupOption(null, 
                MISFeature.MIS_PACKAGE_AI_CRAWLING, MISFeature.MIS_FEATURE_AI_CRAWLING,
                MISFeature.MIS_AI_CRAWLING_PATH, MISFeature.MIS_AI_CRAWLING_OPTION_PATH, MISAIAddonItem,
#if MIS_AI_CRAWLING
                AICrawlingSetup
#else
                null
#endif
                ));


            misAIAddonSetupOptionList.Add(LoadAddonSetupOption(null, 
                MISFeature.MIS_PACKAGE_AI_HELICOPTER, MISFeature.MIS_FEATURE_AI_HELICOPTER,
                MISFeature.MIS_AI_HELICOPTER_PATH, MISFeature.MIS_AI_HELICOPTER_OPTION_PATH, MISAIAddonItem,
#if MIS_AI_HELICOPTER
                AIHelicopterSetup
#else
                null
#endif
                ));


            misAIAddonSetupOptionList.Add(LoadAddonSetupOption(null, 
                MISFeature.MIS_PACKAGE_AI_MOTORCYCLE, MISFeature.MIS_FEATURE_AI_MOTORCYCLE,
                MISFeature.MIS_AI_MOTORCYCLE_PATH, MISFeature.MIS_AI_MOTORCYCLE_OPTION_PATH, MISAIAddonItem,
#if MIS_AI_MOTORCYCLE
                AIMotorcycleSetup
#else
                null
#endif
                ));


            misAIAddonSetupOptionList.Add(LoadAddonSetupOption(null,
                MISFeature.MIS_PACKAGE_AI_FORMATION, MISFeature.MIS_FEATURE_AI_FORMATION,
                MISFeature.MIS_AI_FORMATION_PATH, MISFeature.MIS_AI_FORMATION_OPTION_PATH, MISAIAddonItem,
#if MIS_AI_FORMATION
                AIFormationSetup
#else
                null
#endif
                ));


            misAIAddonSetupOptionList.Add(LoadAddonSetupOption("MIS-CarRider-EVP must be installed first.",
                MISFeature.MIS_PACKAGE_AI_CARRIDER_EVP, MISFeature.MIS_FEATURE_AI_CARRIDER_EVP,
                MISFeature.MIS_AI_CARRIDER_EVP_PATH, MISFeature.MIS_AI_CARRIDER_EVP_OPTION_PATH, MISAIAddonItem,
#if MIS_FSM_AI && MIS_AI_CARRIDER_EVP
                AICarRiderEVPSetup
#else
                null
#endif
                ));


            misAIAddonSetupOptionList.Add(LoadAddonSetupOption("MIS-CarRider-RCC must be installed first.",
                MISFeature.MIS_PACKAGE_AI_CARRIDER_RCC, MISFeature.MIS_FEATURE_AI_CARRIDER_RCC,
                MISFeature.MIS_AI_CARRIDER_RCC_PATH, MISFeature.MIS_AI_CARRIDER_RCC_OPTION_PATH, MISAIAddonItem,
#if MIS_FSM_AI && MIS_AI_CARRIDER_RCC
                AICarRiderRCCSetup
#else
                null
#endif
                ));

#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckMISAIAddons()
        {
            CheckAllMISAIAddonsSetupOption();

            ToggleAllMISAIAddons();

            misAIAddonsScroll = EditorGUILayout.BeginScrollView(misAIAddonsScroll, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                foreach (AddonSetupOption setupOption in misAIAddonSetupOptionList)
                    UserSetupOption(setupOption);
            }
            EditorGUILayout.EndScrollView();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckAllMISAIAddonsSetupOption()
        {
            EditorGUI.BeginChangeCheck();
            setupAllAIAddonsSP = aiAddonSetupSO.FindProperty("setupAllAIAddons");
            setupAllAIAddonsSP.boolValue = EditorGUILayout.ToggleLeft("All MIS AI Addons", setupAllAIAddonsSP.boolValue, skin.label, GUILayout.MaxHeight(25));
            if (EditorGUI.EndChangeCheck())
            {
                aiAddonSetupSO.ApplyModifiedProperties();
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void ToggleAllMISAIAddons()
        {
            if (setupAllAIAddonsSP.boolValue == oldSetupAllAI)
                return;
            oldSetupAllAI = setupAllAIAddonsSP.boolValue;

            foreach (AddonSetupOption setupOption in misAIAddonSetupOptionList)
                ToggleSetupProperty(setupOption, setupAllAIAddonsSP.boolValue);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void SetupAIAddons(GameObject characterObj)
        {
            // ----------------------------------------------------------------------------------------------------
            // Character Conversion
            if (hasMISAIController == false)
                ConvertAICharacter(characterObj);


            try
            {
                // ----------------------------------------------------------------------------------------------------
                // Animator
                PrepareAnimator();
                SetupBaseLayer();

                int totalSetupCount = GetAISetupCount();
                int currentCount = 0;

                EditorUtility.DisplayProgressBar("Please wait...", string.Format("Remains {0}/{1}", currentCount, totalSetupCount), 0f);


                // ----------------------------------------------------------------------------------------------------
                // MIS AI Addons

                for (int i = 0; i < misAIAddonSetupOptionList.Count; i++)
                {
                    if (misAIAddonSetupOptionList[i].addonSetup != null && misAIAddonSetupOptionList[i].isValid && misAIAddonSetupOptionList[i].option.setup)
                    {
                        misAIAddonSetupOptionList[i].addonSetup(misAIAddonSetupOptionList[i].option, characterObj, cameraObj);

                        currentCount++;

                        EditorUtility.DisplayProgressBar(
                            "Please wait...", string.Format("Remains {0}/{1}", currentCount, totalSetupCount), currentCount / totalSetupCount);
                    }
                }


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
        protected virtual int GetAISetupCount()
        {
            int count = 0;

            foreach (AddonSetupOption setupOption in misAIAddonSetupOptionList)
            {
                if (setupOption.isValid && setupOption.option.setup)
                    count++;
            }

            return count;
        }
    }
}