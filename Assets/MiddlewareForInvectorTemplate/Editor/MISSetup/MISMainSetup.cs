using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public partial class MISMainSetup : MISSetup
    {
        // ----------------------------------------------------------------------------------------------------
        // Toolbar
        public class ToolBar
        {
            public string title;
            public UnityAction Draw;

            public ToolBar(string title, UnityAction onDraw)
            {
                this.title = title;
                this.Draw = onDraw;
            }
            public static implicit operator string(ToolBar tool)
            {
                return tool.title;
            }
        }
        protected int toolBarIndex = 0;
        protected ToolBar[] toolBars;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        [MenuItem("Tools/MIS/MIS Setup", false, (int)MISEditor.MISMenuItem.MISSetup)]
        public static void ShowWindow()
        {
            GetWindow(typeof(MISMainSetup), false, "MIS v" + MIS.MIS_VERSION);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void OnEnable()
        {
            base.OnEnable();

            maxSize = new Vector2(minWidth, minHeight);
            minSize = maxSize;

            //SetTitleVersion("MIS Setup", MIS.MIS_VERSION);
            misBanner = (Texture2D)Resources.Load("MIS_SetupBanner", typeof(Texture2D));

            // ----------------------------------------------------------------------------------------------------
            // 
            toolBars = new ToolBar[]
            {
                new ToolBar("MIS", MISContent),
                new ToolBar("Management", ManagementContent),
                //new ToolBar("Character Converter", CharacterConverterContent),
                new ToolBar("Addons", AddonsContent),
                new ToolBar("AI Addons", AIContent),
                new ToolBar("ETC", ETCContent)
            };
            toolBarIndex = 0;


            // ----------------------------------------------------------------------------------------------------
            // 
            InitializeRefactoringClass();


            // ----------------------------------------------------------------------------------------------------
            // 
            LoadMISSetupOptions();
            LoadSetupOptions();
            LoadAISetupOptions();


            // ----------------------------------------------------------------------------------------------------
            // 
            OnEnableETC();


            // ----------------------------------------------------------------------------------------------------
            // 
            //RefreshInvectorCharacter();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void OnGUI()
        {
            base.OnGUI();

            DrawBanner();
            DrawToolbar();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void DrawBanner()
        {
            GUILayout.Label(misBanner, /*GUILayout.ExpandWidth(true), */GUILayout.Height(80));
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void DrawToolbar()
        {
            GUILayout.Space(-5);
            
            toolBarIndex = GUILayout.Toolbar(toolBarIndex, ToolbarNames());

            if (EditorApplication.isCompiling)
            {
                GUILayout.BeginVertical(skin.GetStyle("WindowBG"), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.HelpBox("Unity Editor is busy. Please wait...", MessageType.Info);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndVertical();
            }
            else
            {
                toolBars[toolBarIndex].Draw();
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        string[] ToolbarNames()
        {
            string[] names = new string[toolBars.Length];

            for (int i = 0; i < toolBars.Length; i++)
                names[i] = toolBars[i];

            return names;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void LoadMISSetupOptions()
        {
            misSetupOption = AssetDatabase.LoadAssetAtPath<mvAddonSetupOption>(
                Path.Combine(MISEditor.MIS_EDITOR_PATH, "MISSetup/MISAddon/AddonSetupOptionData.asset"));

            if (misSetupOption != null && misSetupSO == null)
                misSetupSO = new SerializedObject(misSetupOption);
        }
    }
}