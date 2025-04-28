#if INVECTOR_BASIC
using Invector.vCharacterController;
using System;
#endif
#if INVECTOR_AI_TEMPLATE
using Invector.vCharacterController.AI;
#endif
using UnityEditor;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [InitializeOnLoad]
    public class MISRefactoringChecker
    {
        static Vector2 messageAreaSize = new Vector2(400, 100);
        static GUIStyle labelStyle;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        static MISRefactoringChecker()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            try
            {
                if (ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.INVECTOR_FEATURE_FSM))
                {
                    if (!MISSystem.HasFile(MISEditor.INVECTOR_AI_CONTROLAI_ASSETS_PATH) && !MISSystem.HasFile(MISEditor.INVECTOR_FSMAI_CONTROLAI_ASSETS_PATH))
                    {
                        // Remove useless INVECTOR_FSM_TEMPLATE feature
                        ScriptingDefineSymbolManager.RemoveDefineSymbol(MISFeature.INVECTOR_FEATURE_FSM);
                        AssetDatabase.Refresh();
                    }
                    else if (MISMainSetup.HasMISAIRefactoringDone == MIS_AI_REFACTORING.Required)
                    {
                        if (ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_AI_CARRIDER_EVP)
                            || ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_AI_CARRIDER_RCC))
                            Debug.LogWarning("MIS AI Refactoring is required!");
                    }
                }
            }
            catch
            {
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void OnSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();
            {
                if (!MISMainSetup.HasMISRefactoringDone)
                {
                    Rect rect = new Rect(
                        (EditorGUIUtility.currentViewWidth * 0.5f) - (messageAreaSize.x * 0.5f), messageAreaSize.y,
                        messageAreaSize.x, messageAreaSize.y);

                    if (labelStyle == null)
                    {
                        labelStyle = new GUIStyle(EditorStyles.whiteLabel);
                        labelStyle.fontSize = 18;
                        labelStyle.alignment = TextAnchor.MiddleCenter;
                        labelStyle.fontStyle = FontStyle.Bold;
                        labelStyle.wordWrap = true;
                        labelStyle.clipping = TextClipping.Overflow;
                    }

                    GUILayout.BeginArea(rect);
                    {
                        GUILayout.Label("IMPORTANT: MIS Refactoring is required!", labelStyle);

                        if (GUILayout.Button("Open MIS Setup Window", GUILayout.Height(35)))
                        {
                            var window = (MISMainSetup)EditorWindow.GetWindow(typeof(MISMainSetup), false, "MIS v" + MIS.MIS_VERSION);
                            window.Show();
                        }
                    }
                    GUILayout.EndArea();
                }
                /*
#if INVECTOR_AI_TEMPLATE
                else if (MISMainSetup.HasMISRefactoringDone && MISMainSetup.HasMISAIRefactoringDone == MIS_AI_REFACTORING.Required)
                {
                    Rect rect = new Rect(
                        (EditorGUIUtility.currentViewWidth * 0.5f) - (messageAreaSize.x * 0.5f), messageAreaSize.y,
                        messageAreaSize.x, messageAreaSize.y);

                    if (labelStyle == null)
                    {
                        labelStyle = new GUIStyle(EditorStyles.whiteLabel);
                        labelStyle.fontSize = 18;
                        labelStyle.alignment = TextAnchor.MiddleCenter;
                        labelStyle.fontStyle = FontStyle.Bold;
                        labelStyle.wordWrap = true;
                        labelStyle.clipping = TextClipping.Overflow;
                    }

                    GUILayout.BeginArea(rect);
                    {
                        GUILayout.Label("IMPORTANT: MIS AI Refactoring is required!\nPlease do not import any of MIS AI package.", labelStyle);

                        if (GUILayout.Button("Open MIS Setup Window", GUILayout.Height(35)))
                        {
                            var window = (MISMainSetup)EditorWindow.GetWindow(typeof(MISMainSetup), false, "MIS v" + MIS.MIS_VERSION);
                            window.Show();
                        }
                    }
                    GUILayout.EndArea();
                }
#endif
                */
            }
            Handles.EndGUI();
        }
    }
}