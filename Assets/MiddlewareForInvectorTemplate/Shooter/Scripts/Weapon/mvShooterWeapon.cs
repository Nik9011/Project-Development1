#if INVECTOR_SHOOTER
using Invector;
using Invector.vShooter;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("mvShooterWeapon", iconName = "misIconRed")]
    public partial class mvShooterWeapon : vShooterWeapon
    {
        [vEditorToolbar("IK Options")]
        [Header("MIS-Crawling")]
        [Tooltip("Left IK on Crawling Idle")]
        public bool useIkOnCrawlingIdle = true;
        [Tooltip("Left IK on Crawling free locomotion")]
        public bool useIkOnCrawlingFree = true;
        [Tooltip("Left IK on Crawling strafe locomotion")]
        public bool useIkOnCrawlingStrafe = true;

        [Header("MIS-Flying")]
        [Tooltip("Left IK on Flying Idle")]
        public bool useIkOnFlyingIdle = true;
        [Tooltip("Left IK on Flying free locomotion")]
        public bool useIkOnFlyingFree = true;
        [Tooltip("Left IK on Flying strafe locomotion")]
        public bool useIkOnFlyingStrafe = true;
    }
}
#endif