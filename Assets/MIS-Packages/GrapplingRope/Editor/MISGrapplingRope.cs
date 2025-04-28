using UnityEditor;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [InitializeOnLoad]
    public class MISGrapplingRope
    {
        // ----------------------------------------------------------------------------------------------------
        // Package
        public static string PACKAGE_VERSION = "1.4.2";
        public static int PACKAGE_VERSION_CODE = 25;


        // ----------------------------------------------------------------------------------------------------
        // MIS
        public static string MIS_MIN_VERSION = "2.6.5";
        public static int MIN_MIS_VERSION_CODE = 46;


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        static MISGrapplingRope()
        {
            if (!SessionState.GetBool(MISFeature.MIS_PACKAGE_GRAPPLINGROPE, false))
            {
                if (!HasValidVersion())
                    Debug.LogError("Currently installed MIS version is not compatible with " + MISFeature.MIS_PACKAGE_GRAPPLINGROPE + ". Please upgrade MIS to make it work properly.");

                SessionState.SetBool(MISFeature.MIS_PACKAGE_GRAPPLINGROPE, true);
            }

            if (!ScriptingDefineSymbolManager.IsSymbolAlreadyDefined(MISFeature.MIS_FEATURE_GRAPPLINGROPE))
                ScriptingDefineSymbolManager.AddDefineSymbol(MISFeature.MIS_FEATURE_GRAPPLINGROPE);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public static bool HasValidVersion()
        {
            return MIN_MIS_VERSION_CODE <= MIS.MIS_VERSION_CODE;
        }
    }
}
