using System;
using System.Reflection;
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
        // 
        float misGroupHeight = 250f;
        float misAIGroupHeight = 170f;
        float invectorGroupHeight = 170f;

        Vector2 misScroll;
        Vector2 misAIScroll;
        Vector2 invectorScroll;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void ManagementContent()
        {
            GUILayout.BeginVertical(skin.GetStyle("WindowBG"), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                EditorGUILayout.HelpBox("Do not remove manually, use the Remove Package button instead.\nInvector add-ons that are already imported can be used after activating them.", MessageType.Warning);

                GUILayout.Space(5);

                // ----------------------------------------------------------------------------------------------------
                // MIS
                GUILayout.BeginVertical("box", GUILayout.Height(misGroupHeight));
                {
                    MISAddons();
                }
                GUILayout.EndVertical();

                GUILayout.Space(10);

                // ----------------------------------------------------------------------------------------------------
                // MIS AI
                GUILayout.BeginVertical("box", GUILayout.Height(misAIGroupHeight));
                {
                    MISAIAddons();
                }
                GUILayout.EndVertical();

                GUILayout.Space(10);

                // ----------------------------------------------------------------------------------------------------
                // INVECTOR
                GUILayout.BeginVertical("box", GUILayout.Height(invectorGroupHeight));
                {
                    InvectorAddons();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void MISAddons()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("<b>MIS Packages</b>", GUILayout.MaxHeight(25));
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            misScroll = EditorGUILayout.BeginScrollView(misScroll, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginHorizontal();
                {
                    int leftSideCount = Mathf.RoundToInt((misAddonSetupOptionList.Count / 2f));

                    if (misAddonSetupOptionList.Count == 1)
                        leftSideCount = 1;

                    GUILayout.BeginVertical(GUILayout.Width(minWidth * 0.46f));
                    {
                        for (int i = 0; i < leftSideCount; i++)
                            misAddonSetupOptionList[i].addonItem(misAddonSetupOptionList[i].addonName);
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical(GUILayout.Width(minWidth * 0.46f));
                    {
                        if (misAddonSetupOptionList.Count - leftSideCount > 0)
                        {
                            for (int i = leftSideCount; i < misAddonSetupOptionList.Count; i++)
                                misAddonSetupOptionList[i].addonItem(misAddonSetupOptionList[i].addonName);
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void MISAddonItem(string assetName)
        {
            AddonSetupOption addonSetupOption = misAddonSetupOptionList.Find(x => x.addonName == assetName);

            if (addonSetupOption == null)
                return;

            GUILayout.BeginHorizontal();
            {
                if (addonSetupOption.isFeatureEnabled)
                {
                    if (string.IsNullOrEmpty(addonSetupOption.option.version))
                        GUILayout.Label(assetName, skin.box, GUILayout.Height(30));
                    else
                        GUILayout.Label(assetName + " v" + addonSetupOption.option.version, skin.box, GUILayout.Height(30));

                    EditorGUI.BeginDisabledGroup(!addonSetupOption.isValid);
                    {
                        // ----------------------------------------------------------------------------------------------------
                        // MIS
                        if (addonSetupOption.addonName.Equals(MISFeature.MIS_PACKAGE_CARRIDER_EVP))
                        {
#if MIS_CARRIDER_RCC || MIS_HELICOPTER
                            if (ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_CARRIDER_RCC) ||
                                ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_HELICOPTER))
                            {
                                RemovePackageButton(MISFeature.MIS_PACKAGE_CARRIDER_EVP, MISFeature.MIS_FEATURE_CARRIDER_EVP,
                                    MISFeature.MIS_CARRIDER_EVP_PATH, MISFeature.MIS_AI_CARRIDER_EVP_OPTION_PATH, true);
                            }
                            else
#endif
                                RemovePackageButton(MISFeature.MIS_PACKAGE_CARRIDER_EVP, MISFeature.MIS_FEATURE_CARRIDER_EVP,
                                    MISFeature.MIS_CARRIDER_EVP_PATH, MISFeature.MIS_AI_CARRIDER_EVP_OPTION_PATH, true);
                        }
                        else if (addonSetupOption.addonName.Equals(MISFeature.MIS_PACKAGE_CARRIDER_RCC))
                        {
#if MIS_CARRIDER_EVP || MIS_HELICOPTER
                            if (ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_CARRIDER_RCC) ||
                                ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_HELICOPTER))
                            {
                                RemovePackageButton(MISFeature.MIS_PACKAGE_CARRIDER_RCC, MISFeature.MIS_FEATURE_CARRIDER_RCC,
                                    MISFeature.MIS_CARRIDER_RCC_PATH, MISFeature.MIS_AI_CARRIDER_RCC_OPTION_PATH, true);
                            }
                            else
#endif
                                RemovePackageButton(MISFeature.MIS_PACKAGE_CARRIDER_RCC, MISFeature.MIS_FEATURE_CARRIDER_RCC,
                                    MISFeature.MIS_VEHICLERIDER_PATH, MISFeature.MIS_AI_CARRIDER_RCC_OPTION_PATH, true);
                        }
                        else if (addonSetupOption.addonName.Equals(MISFeature.MIS_PACKAGE_HELICOPTER))
                        {
#if MIS_CARRIDER_EVP || MIS_CARRIDER_RCC
                            if (ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_CARRIDER_EVP) ||
                                ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_CARRIDER_RCC))
                            {
                                RemovePackageButton(MISFeature.MIS_PACKAGE_HELICOPTER, MISFeature.MIS_FEATURE_HELICOPTER,
                                    MISFeature.MIS_HELICOPTER_PATH, MISFeature.MIS_AI_HELICOPTER_OPTION_PATH, true);
                            }
                            else
#endif
                                RemovePackageButton(MISFeature.MIS_PACKAGE_HELICOPTER, MISFeature.MIS_FEATURE_HELICOPTER,
                                    MISFeature.MIS_VEHICLERIDER_PATH, MISFeature.MIS_AI_HELICOPTER_OPTION_PATH, true);
                        }
                        else if (addonSetupOption.addonName.Equals(MISFeature.MIS_PACKAGE_LEDGECLIMB1))
                        {
                            RemoveLedgeClimb1Button();
                        }
                        else if (addonSetupOption.addonName.Equals(MISFeature.MIS_PACKAGE_LEDGECLIMB2))
                        {
                            RemoveLedgeClimb2Button();
                        }
                        // ----------------------------------------------------------------------------------------------------
                        // MIS AI
                        else if (addonSetupOption.addonName.Equals(MISFeature.MIS_PACKAGE_AI_CARRIDER_EVP))
                        {
#if MIS_AI_CARRIDER_RCC
                            if (ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_AI_CARRIDER_RCC))
                            {
                                RemovePackageButton(MISFeature.MIS_PACKAGE_AI_CARRIDER_EVP, MISFeature.MIS_FEATURE_AI_CARRIDER_EVP,
                                    MISFeature.MIS_AI_CARRIDER_EVP_PATH, MISFeature.MIS_AI_CARRIDER_EVP_OPTION_PATH, true);
                            }
                            else
#endif
                                RemovePackageButton(MISFeature.MIS_PACKAGE_AI_CARRIDER_EVP, MISFeature.MIS_FEATURE_AI_CARRIDER_EVP,
                                    MISFeature.MIS_AI_VEHICLERIDER_PATH, MISFeature.MIS_AI_CARRIDER_EVP_OPTION_PATH, true);
                        }
                        else if (addonSetupOption.addonName.Equals(MISFeature.MIS_PACKAGE_AI_CARRIDER_RCC))
                        {
#if MIS_AI_CARRIDER_EVP
                            if (ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_AI_CARRIDER_EVP))
                            {
                                RemovePackageButton(MISFeature.MIS_PACKAGE_AI_CARRIDER_RCC, MISFeature.MIS_FEATURE_AI_CARRIDER_RCC,
                                    MISFeature.MIS_AI_CARRIDER_RCC_PATH, MISFeature.MIS_AI_CARRIDER_RCC_OPTION_PATH, true);
                            }
                            else
#endif
                                RemovePackageButton(MISFeature.MIS_PACKAGE_AI_CARRIDER_RCC, MISFeature.MIS_FEATURE_AI_CARRIDER_RCC,
                                    MISFeature.MIS_AI_VEHICLERIDER_PATH, MISFeature.MIS_AI_CARRIDER_RCC_OPTION_PATH, true);
                        }
                        else
                        {
                            RemovePackageButton(addonSetupOption, true);
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    GUILayout.Label(assetName, skin.box, GUILayout.Height(30));
                }
            }
            GUILayout.EndHorizontal();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void MISAIAddons()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("<b>MIS AI Packages</b>", GUILayout.MaxHeight(25));
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            misAIScroll = EditorGUILayout.BeginScrollView(misAIScroll, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginHorizontal();
                {
                    int leftSideCount = Mathf.RoundToInt((misAIAddonSetupOptionList.Count / 2f));

                    if (misAIAddonSetupOptionList.Count == 1)
                        leftSideCount = 1;

                    GUILayout.BeginVertical(GUILayout.Width(minWidth * 0.46f));
                    {
                        for (int i = 0; i < leftSideCount; i++)
                            misAIAddonSetupOptionList[i].addonItem(misAIAddonSetupOptionList[i].addonName);
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical(GUILayout.Width(minWidth * 0.46f));
                    {
                        if (misAIAddonSetupOptionList.Count - leftSideCount > 0)
                        {
                            for (int i = leftSideCount; i < misAIAddonSetupOptionList.Count; i++)
                                misAIAddonSetupOptionList[i].addonItem(misAIAddonSetupOptionList[i].addonName);
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void MISAIAddonItem(string assetName)
        {
            AddonSetupOption addonSetupOption = misAIAddonSetupOptionList.Find(x => x.addonName == assetName);

            if (addonSetupOption == null)
                return;

            GUILayout.BeginHorizontal();
            {
                if (addonSetupOption.isFeatureEnabled)
                {
                    if (string.IsNullOrEmpty(addonSetupOption.option.version))
                        GUILayout.Label(assetName, skin.box, GUILayout.Height(30));
                    else
                        GUILayout.Label(assetName + " v" + addonSetupOption.option.version, skin.box, GUILayout.Height(30));

                    EditorGUI.BeginDisabledGroup(!addonSetupOption.isValid);
                    {
                        if (addonSetupOption.addonName.Equals(MISFeature.MIS_PACKAGE_AI_CARRIDER_EVP))
                        {
#if MIS_AI_CARRIDER_RCC
                            if (ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_AI_CARRIDER_RCC))
                            {
                                RemovePackageButton(MISFeature.MIS_PACKAGE_AI_CARRIDER_EVP, MISFeature.MIS_FEATURE_AI_CARRIDER_EVP, 
                                    MISFeature.MIS_AI_CARRIDER_EVP_PATH, MISFeature.MIS_AI_CARRIDER_EVP_OPTION_PATH, true);
                            }
                            else
#endif
                            RemovePackageButton(MISFeature.MIS_PACKAGE_AI_CARRIDER_EVP, MISFeature.MIS_FEATURE_AI_CARRIDER_EVP, 
                                MISFeature.MIS_AI_VEHICLERIDER_PATH, MISFeature.MIS_AI_CARRIDER_EVP_OPTION_PATH, true);
                        }
                        else if (addonSetupOption.addonName.Equals(MISFeature.MIS_PACKAGE_AI_CARRIDER_RCC))
                        {
#if MIS_AI_CARRIDER_EVP
                            if (ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_AI_CARRIDER_EVP))
                            {
                                RemovePackageButton(MISFeature.MIS_PACKAGE_AI_CARRIDER_RCC, MISFeature.MIS_FEATURE_AI_CARRIDER_RCC, 
                                    MISFeature.MIS_AI_CARRIDER_RCC_PATH, MISFeature.MIS_AI_CARRIDER_RCC_OPTION_PATH, true);
                            }
                            else
#endif
                            RemovePackageButton(MISFeature.MIS_PACKAGE_AI_CARRIDER_RCC, MISFeature.MIS_FEATURE_AI_CARRIDER_RCC, 
                                MISFeature.MIS_AI_VEHICLERIDER_PATH, MISFeature.MIS_AI_CARRIDER_RCC_OPTION_PATH, true);
                        }
                        else
                        {
                            RemovePackageButton(addonSetupOption, true);
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    GUILayout.Label(assetName, skin.box, GUILayout.Height(30));
                }
            }
            GUILayout.EndHorizontal();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void InvectorAddons()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("<b>INVECTOR Add-ons</b>", GUILayout.MaxHeight(25));
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            invectorScroll = EditorGUILayout.BeginScrollView(invectorScroll, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginHorizontal();
                {
                    int leftSideCount = Mathf.RoundToInt((invectorAddonSetupOptionList.Count / 2f));

                    if (invectorAddonSetupOptionList.Count == 1)
                        leftSideCount = 1;

                    GUILayout.BeginVertical(GUILayout.Width(minWidth * 0.46f));
                    {
                        for (int i = 0; i < leftSideCount; i++)
                            invectorAddonSetupOptionList[i].addonItem(invectorAddonSetupOptionList[i].addonName);
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical(GUILayout.Width(minWidth * 0.46f));
                    {
                        if (invectorAddonSetupOptionList.Count - leftSideCount > 0)
                        {
                            for (int i = leftSideCount; i < invectorAddonSetupOptionList.Count; i++)
                                invectorAddonSetupOptionList[i].addonItem(invectorAddonSetupOptionList[i].addonName);
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public void InvectorAddonItem(string assetName)
        {
            AddonSetupOption addonSetupOption = invectorAddonSetupOptionList.Find(x => x.addonName == assetName);

            if (addonSetupOption == null)
                return;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(assetName, skin.box, GUILayout.Height(30));

                EditorGUI.BeginDisabledGroup(!addonSetupOption.isValid);
                {
                    if (ToggleFeatureButton(addonSetupOption))
                    {
                        AssetDatabase.Refresh();
                        //MISReloadAssemblyGuidePopup.Open();
                    }

                    RemovePackageButton(addonSetupOption, false);
                }
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool ToggleFeatureButton(AddonSetupOption setupOption)
        {
            if (setupOption == null || !setupOption.isValid)
                return false;

            if (setupOption.isFeatureEnabled)
            {
                if (GUILayout.Button(iconPause, GUILayout.Height(30), GUILayout.Width(30)))
                {
                    if (!HasInvectorAddone(setupOption.feature))
                    {
                        Debug.LogWarning($"Cannot find {setupOption.addonName} asset in this project.");
                        return false;
                    }

                    if (EditorUtility.DisplayDialog("Warning", "Would you like to disable " + setupOption.addonName + "?", "Ok", "Cancel"))
                    {
                        ToggleFeature(setupOption, false);
                        return true;
                    }
                }
            }
            else
            {
                if (GUILayout.Button(iconRedo, GUILayout.Height(30), GUILayout.Width(30)))
                {
                    if (!HasInvectorAddone(setupOption.feature))
                    {
                        Debug.LogWarning($"Cannot find {setupOption.addonName} asset in this project.");
                        return false;
                    }

                    if (EditorUtility.DisplayDialog("Warning", "Would you like to enable " + setupOption.addonName + "? " + setupOption.addonName + " must already be setup.", "Ok", "Cancel"))
                    {
                        ToggleFeature(setupOption, true);
                        return true;
                    }
                }
            }

            return false;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void RemovePackageButton(string assetName, string assetFeature, string assetPath, string assetOptionPath, bool clearVersion)
        {
            if (GUILayout.Button(iconRemove, GUILayout.Height(30), GUILayout.Width(30)))
            {
                if (string.IsNullOrEmpty(assetName) || string.IsNullOrEmpty(assetFeature) || string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(assetOptionPath))
                    return;

                if (EditorUtility.DisplayDialog("Warning", "Would you like to remove " + assetName + "? The relevant animator states and components are not removed automatically.", "Ok", "Cancel"))
                {
                    RemovePackage(assetFeature, assetPath, assetOptionPath, clearVersion);

                    AssetDatabase.Refresh();
                    //MISReloadAssemblyGuidePopup.Open();
                }
            }
        }
        protected virtual void RemovePackageButton(AddonSetupOption setupOption, bool clearVersion)
        {
            if (GUILayout.Button(iconRemove, GUILayout.Height(30), GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("Warning", "Would you like to remove " + setupOption.addonName + "? The relevant animator states and components are not removed automatically.", "Ok", "Cancel"))
                {
                    RemovePackage(setupOption, clearVersion);

                    AssetDatabase.Refresh();
                    //MISReloadAssemblyGuidePopup.Open();
                }
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void RemoveLedgeClimb1Button()
        {
            if (GUILayout.Button(iconRemove, GUILayout.Height(30), GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("Warning", "Would you like to remove MIS-LedgeClimb1? The relevant animator states and components are not removed automatically.", "Ok", "Cancel"))
                {
#if MIS_LEDGECLIMB2
                    SetAddonVersion(MISFeature.MIS_LEDGECLIMB1_OPTION_PATH, null);
                    SetFeature(MISFeature.MIS_FEATURE_LEDGECLIMB1, false);
                    RemovePackage(MISFeature.MIS_FEATURE_LEDGECLIMB2, MISFeature.MIS_LEDGECLIMB_PATH, MISFeature.MIS_LEDGECLIMB2_OPTION_PATH, true);
#else
                    RemovePackage(MISFeature.MIS_FEATURE_LEDGECLIMB1, MISFeature.MIS_LEDGECLIMB_PATH, MISFeature.MIS_LEDGECLIMB1_OPTION_PATH, true);
#endif

                    AssetDatabase.Refresh();
                    //MISReloadAssemblyGuidePopup.Open();
                }
            }
        }
        protected virtual void RemoveLedgeClimb2Button()
        {
            if (GUILayout.Button(iconRemove, GUILayout.Height(30), GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("Warning", "Would you like to remove MIS-LedgeClimb2? The relevant animator states and components are not removed automatically.", "Ok", "Cancel"))
                {
                    SetAddonVersion(MISFeature.MIS_LEDGECLIMB1_OPTION_PATH, null);
                    SetFeature(MISFeature.MIS_FEATURE_LEDGECLIMB1, false);
                    RemovePackage(MISFeature.MIS_FEATURE_LEDGECLIMB2, MISFeature.MIS_LEDGECLIMB_PATH, MISFeature.MIS_LEDGECLIMB2_OPTION_PATH, true);

                    AssetDatabase.Refresh();
                    //MISReloadAssemblyGuidePopup.Open();
                }
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void EnableAllFeatures()
        {
            foreach (AddonSetupOption setupOption in misAddonSetupOptionList)
                ToggleFeature(setupOption, true);
        }
        protected virtual void DisableAllFeatures()
        {
            foreach (AddonSetupOption setupOption in misAddonSetupOptionList)
                ToggleFeature(setupOption, false);

            foreach (AddonSetupOption setupOption in invectorAddonSetupOptionList)
                ToggleFeature(setupOption, false);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool HasInvectorAddone(string feature)
        {
            Type package = null;

            if (feature == MISFeature.MIS_V_FEATURE_BUILDER)
                package = Type.GetType("vBuildManager" + ",Assembly-CSharp");
            else if (feature == MISFeature.MIS_V_FEATURE_CRAFTING)
                package = Type.GetType("" + ",Assembly-CSharp");    //?
            else if (feature == MISFeature.MIS_V_FEATURE_FREECLIMB)
                package = Type.GetType("Invector.vCharacterController.vActions.vFreeClimb" + ",Assembly-CSharp");
            else if (feature == MISFeature.MIS_V_FEATURE_PARACHUTE)
                package = Type.GetType("vParachuteController" + ",Assembly-CSharp");
            else if (feature == MISFeature.MIS_V_FEATURE_PUSH)
                package = Type.GetType("vPushActionController" + ",Assembly-CSharp");
            else if (feature == MISFeature.MIS_V_FEATURE_SHOOTERCOVER)
                package = Type.GetType("Invector.vCover.vCoverController" + ",Assembly-CSharp");
            else if (feature == MISFeature.MIS_V_FEATURE_SWIMMING)
                package = Type.GetType("vSwimming" + ",Assembly-CSharp");   //?
            else if (feature == MISFeature.MIS_V_FEATURE_ZIPLINE)
                package = Type.GetType("Invector.vCharacterController.vActions.vZipLine" + ",Assembly-CSharp");

            return package != null;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void RemoveAllTagsAndLayers()
        {
            // Flare
            if (MISEditorTagLayer.HasUnityTag(MISEditorTagLayer.TAG_FLARE))
                MISEditorTagLayer.RemoveUnityTag(MISEditorTagLayer.TAG_FLARE);

            // GrapplingTarget
            if (MISEditorTagLayer.HasUnityTag(MISEditorTagLayer.TAG_GRAPPLING_TARGET))
                MISEditorTagLayer.RemoveUnityTag(MISEditorTagLayer.TAG_GRAPPLING_TARGET);

            // WallRun Tag
            if (MISEditorTagLayer.HasUnityTag(MISEditorTagLayer.TAG_WALLRUN))
                MISEditorTagLayer.RemoveUnityTag(MISEditorTagLayer.TAG_WALLRUN);

            // Door Layer
            if (MISEditorTagLayer.HasUnityLayer(MISEditorTagLayer.LAYER_DOOR))
                MISEditorTagLayer.RemoveUnityLayer(MISEditorTagLayer.LAYER_DOOR);

            // Vehicle Layer
            if (MISEditorTagLayer.HasUnityLayer(MISEditorTagLayer.LAYER_VEHICLE))
                MISEditorTagLayer.RemoveUnityLayer(MISEditorTagLayer.LAYER_VEHICLE);
        }
    }
}