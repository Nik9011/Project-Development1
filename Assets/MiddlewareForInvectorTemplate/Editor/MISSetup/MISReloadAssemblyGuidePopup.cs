using UnityEditor;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public class MISReloadAssemblyGuidePopup : MISSetupBase
    {
        static MISReloadAssemblyGuidePopup popupWindow;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void Open()
        {
            if (popupWindow == null)
                popupWindow = CreateInstance<MISReloadAssemblyGuidePopup>();

            GetWindow(typeof(MISReloadAssemblyGuidePopup), false, "Reload Assembly Guide");
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void OnEnable()
        {
            base.OnEnable();

            maxSize = new Vector2(500f, 200f);
            minSize = maxSize;

            SetTitleTooltip("MIS Setup", "");
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void OnGUI()
        {
            base.OnGUI();

            GUILayout.BeginVertical(skin.GetStyle("WindowBG"), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("<b>Please close this popup and wait until Unity Editor reloads assemblies again.</b>", GUILayout.MaxHeight(25));
                GUILayout.FlexibleSpace();

                GUILayout.Space(-15);

                if (GUILayout.Button("OK"))
                {
                    SceneView.lastActiveSceneView.FrameSelected();
                    this.Close();
                }
            }
            GUILayout.EndVertical();
        }
    }
}