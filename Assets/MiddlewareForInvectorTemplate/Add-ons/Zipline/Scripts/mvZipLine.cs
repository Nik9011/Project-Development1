#pragma warning disable 0414

#if MIS_INVECTOR_ZIPLINE
using Invector;
using Invector.vCharacterController.vActions;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("ZipLine", iconName = "misIconRed")]
    public class mvZipLine : vZipLine
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("ChainedAction", order = 98)]
        [mvReadOnly] [SerializeField] string chainedAction = "Chained-Actions are provided as an option";
#if MIS_AIRDASH
        public bool allowFromAirDash = true;
#endif
#if MIS_GROUNDDASH
        public bool allowFromGroundDash = true;
#endif
#if MIS_FREEFLYING
        public bool allowFromFreeFlying = true;
#endif
#if MIS_MOTORCYCLE
        public bool allowFromMotorcycle = true;
#endif
#if MIS_SOFTFLYING
        public bool allowFromSoftFlying = true;
#endif
#if MIS_WATERDASH
        public bool allowFromWaterDash = true;
#endif

#if MIS_INVECTOR_PARACHUTE
        public bool allowFromParachute = true;
#endif
#if MIS_INVECTOR_SHOOTERCOVER
        public bool allowFromShooterCover = true;
#endif


        // ----------------------------------------------------------------------------------------------------
        // if true, it means this action is currently being used
        public bool IsOnAction
        {
            get => isUsingZipline;
        }


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual bool ZiplineStartCondition()
        {
            return
                true
#if MIS_AIRDASH
                && (inputController.cc.IsAirDashOnAction ? (allowFromAirDash ? true : false) : true)
#endif
#if MIS_GROUNDDASH
                && (inputController.cc.IsGroundDashOnAction ? (allowFromGroundDash ? true : false) : true)
#endif
#if MIS_FREEFLYING
                && (inputController.cc.IsFreeFlyingOnAction ? (allowFromFreeFlying ? true : false) : true)
#endif
#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
                && !inputController.cc.IsLedgeClimbOnAction
#endif
#if MIS_MOTORCYCLE
                && (inputController.cc.IsRiderOnAction ? (allowFromMotorcycle ? true : false) : true)
#endif
#if MIS_SOFTFLYING
                && (inputController.cc.IsSoftFlyingOnAction ? (allowFromSoftFlying ? true : false) : true)
#endif
#if MIS_SWIMMING
                && !inputController.cc.IsSwimOnAction
#endif
#if MIS_WATERDASH
                && (inputController.cc.IsWaterDashOnAction ? (allowFromWaterDash ? true : false) : true)
#endif

#if MIS_INVECTOR_PARACHUTE
                && (inputController.cc.IsVParachuteOnAction ? (allowFromParachute ? true : false) : true)
#endif
#if MIS_INVECTOR_PUSH
                && !inputController.cc.IsVPushOnAction
#endif
#if MIS_INVECTOR_SHOOTERCOVER
                && (inputController.cc.IsVShooterCoverOnAction ? (allowFromShooterCover ? true : false) : true)
#endif
                ;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckChainedAction()
        {
#if MIS_AIRDASH
            if (inputController.cc.IsAirDashOnAction && allowFromAirDash)
                inputController.cc.misAirDash.ExitActionState();
#endif
#if MIS_GROUNDDASH
            if (inputController.cc.IsGroundDashOnAction && allowFromGroundDash)
                inputController.cc.misGroundDash.ExitActionState();
#endif
#if MIS_FREEFLYING
            if (inputController.cc.IsFreeFlyingOnAction && allowFromFreeFlying)
                inputController.cc.misFreeFlying.ExitActionState();
#endif
#if MIS_MOTORCYCLE
            if (inputController.cc.IsRiderOnAction && allowFromMotorcycle)
                inputController.cc.misRider.ExitActionState(false);
#endif
#if MIS_SOFTFLYING
            if (inputController.cc.IsSoftFlyingOnAction && allowFromSoftFlying)
                inputController.cc.misSoftFlying.ExitActionState();
#endif
#if MIS_WATERDASH
            if (inputController.cc.IsWaterDashOnAction && allowFromWaterDash)
                inputController.cc.misWaterDash.ExitActionState();
#endif

#if MIS_INVECTOR_PARACHUTE
            if (inputController.cc.IsVParachuteOnAction && allowFromParachute)
                inputController.cc.vmisParachute.ExitActionState();
#endif
#if MIS_INVECTOR_SHOOTERCOVER
            if (inputController.cc.IsVShooterCoverOnAction && allowFromShooterCover)
                inputController.cc.vmisShooterCover.ExitActionState(false);
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void InitiateZipline()
        {
            if (!ZiplineStartCondition())
                return;

            CheckChainedAction();

            base.InitiateZipline();
        }
    }
}
#endif
