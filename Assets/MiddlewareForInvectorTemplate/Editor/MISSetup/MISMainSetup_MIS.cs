#pragma warning disable CS0414
//#define USE_REFACTORING_WINDOW

#if INVECTOR_BASIC
using Invector.vCharacterController;
#endif
#if INVECTOR_AI_TEMPLATE
using Invector.vCharacterController.AI;
#endif
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
        mvAddonSetupOption misSetupOption;
        SerializedObject misSetupSO;
        SerializedProperty useMISCameraStateSP;
        SerializedProperty checkHumanoidSP;
        SerializedProperty setupAllMISAddonsSP;
        SerializedProperty setupAllInvectorSP;


        // ----------------------------------------------------------------------------------------------------
        // Refactoring
#if INVECTOR_AI_TEMPLATE
        public bool isRefactoringAI = true;
#endif

        public const string MIS_NAMESPACE = "using com.mobilin.games;";

        List<RefactoringClass> refactoringClassList;
#if INVECTOR_AI_TEMPLATE
        List<RefactoringClass> refactoringAIClassList;
#endif

#if USE_REFACTORING_WINDOW
        System.Text.StringBuilder refactoringContents;
#endif

        public enum REFACTORING_ERROR_CODE
        {
            None = 0,
            FileNotExists = 1,
            PatternNotFound = 2
        };


        // ----------------------------------------------------------------------------------------------------
        // 
        public static bool HasMISRefactoringDone
        {
            get
            {
                return
                    true
#if INVECTOR_BASIC
                    && typeof(vThirdPersonController).BaseType.Equals(typeof(mvThirdPersonAnimator))
#endif
#if INVECTOR_MELEE
                    && typeof(vMeleeCombatInput).BaseType.Equals(typeof(mvThirdPersonInput))
#endif
#if INVECTOR_SHOOTER
                    && typeof(vShooterMeleeInput).BaseType.Equals(typeof(mvMeleeCombatInput))
#endif
                    ;
            }
        }

        public static MIS_AI_REFACTORING HasMISAIRefactoringDone
        {
            get
            {
                if (MISSystem.HasFile(MISEditor.INVECTOR_AI_CONTROLAI_ASSETS_PATH) || MISSystem.HasFile(MISEditor.INVECTOR_FSMAI_CONTROLAI_ASSETS_PATH))
                {
#if MIS_FSM_AI
                    return MIS_AI_REFACTORING.Done;
#else
                    return MIS_AI_REFACTORING.Required;
#endif
                }
                else
                {
                    return MIS_AI_REFACTORING.None;
                }
            }
        }


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void MISContent()
        {
            GUILayout.BeginVertical(skin.GetStyle("WindowBG"), GUILayout.ExpandWidth(true)/*, GUILayout.ExpandHeight(true)*/);
            {
                EditorGUILayout.HelpBox("MIS Refactoring changes the inheritance structure of Invector class so that MIS can work.", MessageType.Warning);

                GUILayout.Space(5);

                // ----------------------------------------------------------------------------------------------------
                // Refactoring
                // ----------------------------------------------------------------------------------------------------
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("<b>Refactoring</b>", GUILayout.MaxHeight(25));

#if INVECTOR_AI_TEMPLATE
                        GUILayout.FlexibleSpace();

                        if (HasMISAIRefactoringDone == MIS_AI_REFACTORING.Required)
                            isRefactoringAI = EditorGUILayout.ToggleLeft("Include FSM AI", isRefactoringAI, skin.label);
#endif
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    GUILayout.BeginHorizontal(GUILayout.Height(30));
                    {
                        if (GUILayout.Button("MIS", GUILayout.Width(BUTTON_WIDTH)))
                        {
                            Refactoring(true);

                            MISReloadAssemblyGuidePopup.Open();
                        }

                        EditorGUILayout.HelpBox("Convert Invector Controller and Input classes inheritance relationships to MIS use.", MessageType.Info);

                        StoreButton(iconAssetStore, "https://assetstore.unity.com/publishers/54920");
                        StoreButton(iconGumroad, "https://mymobilin.gumroad.com/");
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal(GUILayout.Height(30));
                    {
                        if (GUILayout.Button("INVECTOR", GUILayout.Width(BUTTON_WIDTH)))
                        {
                            if (EditorUtility.DisplayDialog("Warning", "Would you like to proceed Reverse Refactoring to pure Invector?\nThe relevant animator states and components are not removed automatically.", "Ok", "Cancel"))
                            {
                                Refactoring(false);

                                DisableAllFeatures();
                                //RemoveAllPackages();
                                RemoveAllTagsAndLayers();

                                //AssetDatabase.DeleteAsset(MISEditor.MIS_PACKAGES_PATH);
                                //AssetDatabase.DeleteAsset(MISEditor.MIS_ASSETS_PATH);

                                AssetDatabase.Refresh();
                                this.Close();
                            }
                        }

                        EditorGUILayout.HelpBox("Return Invector Controller and Input classes inheritance relationships to its original.", MessageType.Warning);

                        StoreButton(iconAssetStore, "https://assetstore.unity.com/publishers/13943");
                        StoreButton(iconInvector, "https://invector.sellfy.store/");
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5f);

                    GUILayout.BeginVertical();
                    {
                        if (HasMISRefactoringDone)
                        {
#if INVECTOR_AI_TEMPLATE
                            if (HasMISAIRefactoringDone == MIS_AI_REFACTORING.Done)
                                EditorGUILayout.HelpBox("MIS Refactoring: DONE / MIS AI Refactoring: DONE", MessageType.Info);
                            else if (HasMISAIRefactoringDone == MIS_AI_REFACTORING.Required)
                                EditorGUILayout.HelpBox("MIS Refactoring: DONE / MIS AI Refactoring: REQUIRED", MessageType.Warning);
                            else
                                EditorGUILayout.HelpBox("MIS Refactoring: DONE", MessageType.Info);
#else
                            EditorGUILayout.HelpBox("MIS Refactoring: DONE", MessageType.Info);
#endif
                        }
                        else
                        {
#if INVECTOR_AI_TEMPLATE
                            EditorGUILayout.HelpBox("MIS / MIS AI Refactoring: REQUIRED", MessageType.Error);
#else                                
                            EditorGUILayout.HelpBox("MIS Refactoring: REQUIRED", MessageType.Error);
#endif
                        }
                        /*
                        // ----------------------------------------------------------------------------------------------------
                        // Verification
                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.Label("<b>Verification</b>", GUILayout.MaxHeight(25));

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("Device ID: " + SystemInfo.deviceUniqueIdentifier.ToString(), skin.label, GUILayout.Height(30));

                                GUILayout.FlexibleSpace();

                                if (GUILayout.Button("Copy To Clipboard", GUILayout.Width(150f), GUILayout.Height(20)))
                                {
                                    string request = "Email to: mymobilin@gmail.com\n\nSubject: Request MIS Verification Key\n\n\nMy Device ID:\n" + SystemInfo.deviceUniqueIdentifier.ToString() + "\n\n\nAsset Store Invoice:\n\n\n\nGumroad License Key:\n";
                                    GUIUtility.systemCopyBuffer = request;

                                    this.ShowNotification(new GUIContent("Copied"), 1f);
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                        */
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();

            CharacterConverterContent();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void Refactoring(bool toMIS = true)
        {
            InitializeRefactoringClass();

#if USE_REFACTORING_WINDOW
            refactoringContents.Clear();
#endif

            if (toMIS)
            {
                foreach (var content in refactoringClassList)
                {
                    if (MISSystem.HasFile(content.classFullPath))
                    {
                        ChangeStringInFile(content, content.invectorClassName, content.misClassName, toMIS);

#if USE_REFACTORING_WINDOW
                        refactoringContents.Append("-  " + content.classFullPath);
                        refactoringContents.AppendLine();
                        refactoringContents.Append(content.invectorClassName + "--> " + content.misClassName);
                        refactoringContents.AppendLine().AppendLine();
#endif

                        SetFeature(MISFeature.MIS_FEATURE, true);
                    }
                }

#if INVECTOR_SHOOTER
                RefactorShooterIKAdjustWindow();
#endif
            }
            else
            {
                foreach (var content in refactoringClassList)
                {
                    if (MISSystem.HasFile(content.classFullPath))
                    {
                        ChangeStringInFile(content, content.misClassName, content.invectorClassName, toMIS);

#if USE_REFACTORING_WINDOW
                        refactoringContents.Append("-  " + content.classFullPath);
                        refactoringContents.AppendLine();
                        refactoringContents.Append(content.misClassName + "--> " + content.invectorClassName);
                        refactoringContents.AppendLine().AppendLine();
#endif

                        SetFeature(MISFeature.MIS_FEATURE, false);
                    }
                }
            }

#if INVECTOR_AI_TEMPLATE
            if (isRefactoringAI)
            {
                RefactoringAI(toMIS);
            }
#endif

            Debug.Log("============================================================");
            Debug.Log("Refactoring has been finished.");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SceneView.lastActiveSceneView.FrameSelected();
        }

#if INVECTOR_AI_TEMPLATE
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void RefactoringAI(bool toMIS = true)
        {
            InitializeRefactoringAIClass();

            if (toMIS)
            {
                foreach (var content in refactoringAIClassList)
                {
                    if (MISSystem.HasFile(content.classFullPath))
                    {
                        ChangeStringInFile(content, content.invectorClassName, content.misClassName, toMIS);

#if USE_REFACTORING_WINDOW
                        refactoringContents.Append("-  " + content.classFullPath);
                        refactoringContents.AppendLine();
                        refactoringContents.Append(content.invectorClassName + "--> " + content.misClassName);
                        refactoringContents.AppendLine().AppendLine();
#endif
                    }
                }

                if (!ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FSM_AI_FEATURE))
                    ScriptingDefineSymbolManager.AddDefineSymbol(MISFeature.MIS_FSM_AI_FEATURE);
            }
            else
            {
                foreach (var content in refactoringAIClassList)
                {
                    if (MISSystem.HasFile(content.classFullPath))
                    {
                        ChangeStringInFile(content, content.misClassName, content.invectorClassName, toMIS);

#if USE_REFACTORING_WINDOW
                        refactoringContents.Append("-  " + content.classFullPath);
                        refactoringContents.AppendLine();
                        refactoringContents.Append(content.misClassName + "--> " + content.invectorClassName);
                        refactoringContents.AppendLine().AppendLine();
#endif
                    }
                }

                if (ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FSM_AI_FEATURE))
                    ScriptingDefineSymbolManager.RemoveDefineSymbol(MISFeature.MIS_FSM_AI_FEATURE);
            }
        }
#endif

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void InitializeRefactoringClass()
        {
            refactoringClassList = new List<RefactoringClass>();

            // Controller: ThirdPerson
            refactoringClassList.Add(new RefactoringClass(
                MISEditor.INVECTOR_ASSETS_PATH + "Basic Locomotion/Scripts/CharacterController/vCharacter.cs",
                "vHealthController", "mvHealthController"));

            refactoringClassList.Add(new RefactoringClass(
                MISEditor.INVECTOR_ASSETS_PATH + "Basic Locomotion/Scripts/CharacterController/vThirdPersonMotor.cs",
                "vCharacter", "mvCharacter"));

            refactoringClassList.Add(new RefactoringClass(
                MISEditor.INVECTOR_ASSETS_PATH + "Basic Locomotion/Scripts/CharacterController/vThirdPersonAnimator.cs",
                "vThirdPersonMotor", "mvThirdPersonMotor"));

            refactoringClassList.Add(new RefactoringClass(
                MISEditor.INVECTOR_ASSETS_PATH + "Basic Locomotion/Scripts/CharacterController/vThirdPersonController.cs",
                "vThirdPersonAnimator", "mvThirdPersonAnimator"));

#if false
            // Controller: TopDown
            refactoringClassList.Add(new RefactoringClass(
                MISEditor.INVECTOR_ASSETS_PATH + "Add-ons/Controller_TopDown/Basic/Scripts/vTopDownController.cs",
                "vThirdPersonController", "mvThirdPersonController"));

            // Controller: Platformer
            refactoringClassList.Add(new RefactoringClass(
                MISEditor.INVECTOR_ASSETS_PATH + "Add-ons/Controller_2.5D Platform/Basic/Scripts/v2_5DController.cs",
                "vThirdPersonController", "mvThirdPersonController"));
#endif

            // Input Manager: ThirdPerson
            refactoringClassList.Add(new RefactoringClass(
                MISEditor.INVECTOR_ASSETS_PATH + "Basic Locomotion/Scripts/CharacterController/vThirdPersonInput.cs",
                "vThirdPersonController", "mvThirdPersonController"));

            if (ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.INVECTOR_FEATURE_MELEE))
            {
                refactoringClassList.Add(new RefactoringClass(
                    MISEditor.INVECTOR_ASSETS_PATH + "Melee Combat/Scripts/CharacterController/vMeleeCombatInput.cs",
                    "vThirdPersonInput", "mvThirdPersonInput"));
            }

            if (ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.INVECTOR_FEATURE_SHOOTER))
            {
                refactoringClassList.Add(new RefactoringClass(
                    MISEditor.INVECTOR_ASSETS_PATH + "Shooter/Scripts/Shooter/vShooterMeleeInput.cs",
                    "vMeleeCombatInput", "mvMeleeCombatInput"));

#if false
                // Input Manager: TopDown
                refactoringClassList.Add(new RefactoringClass(
                    MISEditor.INVECTOR_ASSETS_PATH + "Add-ons/Controller_TopDown/Shooter (Require Shooter Template)/Scripts/vTopDownShooterInput.cs",
                    "vShooterMeleeInput", "mvShooterMeleeInput"));

                // Input Manager: Platformer
                refactoringClassList.Add(new RefactoringClass(
                    MISEditor.INVECTOR_ASSETS_PATH + "Add-ons/Controller_2.5D Platform/Shooter (Require Shooter Template)/Scripts/v2_5DShooterInput.cs",
                    "vShooterMeleeInput", "mvShooterMeleeInput"));
#endif
            }

#if USE_REFACTORING_WINDOW
            // Information
            refactoringContents = new System.Text.StringBuilder();

            refactoringContents.AppendLine("The following Invector classes inheritance relationships will be changed.\nIf you would like to do this job by yourself, please refer to the document.");

            foreach (var content in refactoringClassList)
            {
                refactoringContents.AppendLine("");
                refactoringContents.AppendLine("-  " + content.classFullPath);
                refactoringContents.AppendLine(content.invectorClassName + "  <--->  " + content.misClassName);
            }
#endif
        }

#if INVECTOR_AI_TEMPLATE
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void InitializeRefactoringAIClass()
        {
            refactoringAIClassList = new List<RefactoringClass>();

            if (MISSystem.HasFile(MISEditor.INVECTOR_FSMAI_CONTROLAI_ASSETS_PATH))
            {
                refactoringAIClassList.Add(new RefactoringClass(
                    MISEditor.INVECTOR_FSMAI_ASSETS_PATH + "Scripts/AI/AI Controllers/vControlAI.cs",
                    "vAIMotor", "mvAIMotor"));

                refactoringAIClassList.Add(new RefactoringClass(
                    MISEditor.INVECTOR_FSMAI_ASSETS_PATH + "Scripts/AI/AI Controllers/vControlAIMelee.cs",
                    "vControlAICombat", "mvControlAICombat"));

                refactoringAIClassList.Add(new RefactoringClass(
                    MISEditor.INVECTOR_FSMAI_ASSETS_PATH + "Scripts/AI/AI Controllers/vControlAICombat.cs",
                    "vControlAI", "mvControlAI"));

                refactoringAIClassList.Add(new RefactoringClass(
                    MISEditor.INVECTOR_FSMAI_ASSETS_PATH + "Scripts/AI/AI Controllers/vControlAIShooter.cs",
                    "vControlAICombat", "mvControlAICombat"));
            }
            else if (MISSystem.HasFile(MISEditor.INVECTOR_AI_CONTROLAI_ASSETS_PATH))
            {
                refactoringAIClassList.Add(new RefactoringClass(
                    MISEditor.INVECTOR_AI_ASSETS_PATH + "Scripts/AI/AI Controllers/vControlAI.cs",
                    "vAIMotor", "mvAIMotor"));

                refactoringAIClassList.Add(new RefactoringClass(
                    MISEditor.INVECTOR_AI_ASSETS_PATH + "Scripts/AI/AI Controllers/vControlAIMelee.cs",
                    "vControlAICombat", "mvControlAICombat"));

                refactoringAIClassList.Add(new RefactoringClass(
                    MISEditor.INVECTOR_AI_ASSETS_PATH + "Scripts/AI/AI Controllers/vControlAICombat.cs",
                    "vControlAI", "mvControlAI"));

                refactoringAIClassList.Add(new RefactoringClass(
                    MISEditor.INVECTOR_AI_ASSETS_PATH + "Scripts/AI/AI Controllers/vControlAIShooter.cs",
                    "vControlAICombat", "mvControlAICombat"));
            }

#if USE_REFACTORING_WINDOW
            // Information
            refactoringContents = new System.Text.StringBuilder();

            refactoringContents.AppendLine("The following Invector classes inheritance relationships will be changed.\nIf you would like to do this job by yourself, please refer to the document.");

            foreach (var content in refactoringClassList)
            {
                refactoringContents.AppendLine("");
                refactoringContents.AppendLine("-  " + content.classFullPath);
                refactoringContents.AppendLine(content.invectorClassName + "  <--->  " + content.misClassName);
            }
#endif
        }
#endif

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void ChangeStringInFile(RefactoringClass refactoringClass, string origin, string target, bool toMIS)
        {
            string pattern = "";
            Regex regex;
            MatchCollection matches;

            string fileContents = File.ReadAllText(refactoringClass.classFullPath);

            Debug.Log("============================================================");
            Debug.Log("Refactoring Class: " + refactoringClass.classFullPath);

            // Class name
            pattern = @":[(\t* *)]" + origin;
            regex = new Regex(pattern);
            matches = regex.Matches(fileContents);
            if (matches.Count > 0)
                Debug.Log("Base class: " + origin + " will be exchanged to " + target);
            fileContents = Regex.Replace(fileContents, pattern, ": " + target);

            // Defines
            pattern = @"[(\t*| *)]" + origin + @"[(\t*| *)]";
            regex = new Regex(pattern);
            matches = regex.Matches(fileContents);
            if (matches.Count > 0)
                Debug.Log(origin + " defines will be exchanged to " + target);
            fileContents = Regex.Replace(fileContents, pattern, " " + target + " ");

            // Components
            pattern = @"<(\t* *)" + origin + @"(\t* *)>";
            regex = new Regex(pattern);
            matches = regex.Matches(fileContents);
            if (matches.Count > 0)
                Debug.Log(origin + " references will be exchanged to " + target);
            fileContents = Regex.Replace(fileContents, pattern, "<" + target + ">");

            // Namespace
            if (toMIS)
            {
                pattern = @"(\t* *)" + MIS_NAMESPACE + @"(\t* *)";
                regex = new Regex(pattern);
                matches = regex.Matches(fileContents);
                if (matches.Count == 0)
                {
                    Debug.Log(MIS_NAMESPACE + " will be added");

                    fileContents = MIS_NAMESPACE + "\r\n" + fileContents;
                }
            }
            else
            {
                pattern = @"(\t* *)" + MIS_NAMESPACE + @"(\t* *\r*\n*)";
                regex = new Regex(pattern);
                matches = regex.Matches(fileContents);
                if (matches.Count > 0)
                {
                    Debug.Log(MIS_NAMESPACE + " will be removed");

                    fileContents = Regex.Replace(fileContents, pattern, "");
                }
            }

            File.WriteAllText(refactoringClass.classFullPath, fileContents);
        }

#if INVECTOR_SHOOTER
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void RefactorShooterIKAdjustWindow()
        {
            string fullPath = MISEditor.INVECTOR_ASSETS_PATH + "Shooter/Scripts/Shooter/Editor/vShooterIKAdjustWindow.cs";
            string pattern = "";
            Regex regex;
            MatchCollection matches;

            string fileContents = File.ReadAllText(fullPath);


            // ----------------------------------------------------------------------------------------------------
            // void OnGUI() to protected virtual void OnGUI()
            pattern = @"[^protected virtual]" + @"\t* *" + @"void" + @"\t* *" + @"OnGUI" + @"\t* *\(\t* *\)";
            regex = new Regex(pattern);
            matches = regex.Matches(fileContents);
            if (matches.Count > 0)
            {
                Debug.Log("[vShooterIKAdjustWindow] OnGUI Match count:" + matches.Count);
                fileContents = Regex.Replace(fileContents, pattern, "\t\tprotected virtual void OnGUI()");
            }


            // ----------------------------------------------------------------------------------------------------
            // void Update() to protected virtual void Update()
            pattern = @"[^protected virtual]" + @"\t* *" + @"void" + @"\t* *" + @"Update" + @"\t* *\(\t* *\)";
            regex = new Regex(pattern);
            matches = regex.Matches(fileContents);
            if (matches.Count > 0)
            {
                Debug.Log("[vShooterIKAdjustWindow] Update Match count:" + matches.Count);
                fileContents = Regex.Replace(fileContents, pattern, "\t\tprotected virtual void Update()");
            }


            File.WriteAllText(fullPath, fileContents);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        void StoreButton(Texture2D icon, string path)
        {
            if (GUILayout.Button(icon, GUILayout.Height(35), GUILayout.Width(35)))
            {
                if (!string.IsNullOrEmpty(path))
                    Application.OpenURL(path);
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // 
    public enum MIS_AI_REFACTORING
    {
        None = 0,
        Required,
        Done
    }

    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public class RefactoringClass
    {
        public string classFullPath;
        public string invectorClassName;
        public string misClassName;

        public RefactoringClass(string classFullPath, string invectorClassName, string misClassName)
        {
            this.classFullPath = classFullPath;
            this.invectorClassName = invectorClassName;
            this.misClassName = misClassName;
        }
    }
}