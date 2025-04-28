using UnityEditor;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    public class MISSetupCompletePopup : MISSetupBase
    {
        static MISSetupCompletePopup popupWindow;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static void Open()
        {
            if (popupWindow == null)
                popupWindow = CreateInstance<MISSetupCompletePopup>();

            GetWindow(typeof(MISSetupCompletePopup), false, "Setup Complete");
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
                GUILayout.Label("<b>Setup has been complete.</b>", GUILayout.MaxHeight(25));
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