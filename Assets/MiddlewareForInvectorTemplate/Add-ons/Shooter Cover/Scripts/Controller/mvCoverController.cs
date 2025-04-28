#pragma warning disable 0414

#if MIS_INVECTOR_SHOOTERCOVER
using Invector;
using Invector.vCover;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("mvCoverController", iconName = "misIconRed")]
    public class mvCoverController : vCoverController
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("ChainedAction", order = 98)]
        [mvReadOnly] [SerializeField] string chainedAction = "Chained-Actions are provided as an option";
#if MIS_CRAWLING
        public bool allowFromCrawling = true;
#endif
#if MIS_FREEFLYING
        public bool allowFromFreeFlying = true;
#endif
#if MIS_SOFTFLYING
        public bool allowFromSoftFlying = true;
#endif
#if MIS_SWIMMING
        public bool allowFromSwimming = true;
#endif

#if MIS_INVECTOR_PARACHUTE
        public bool allowFromParachute = true;
#endif


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool ShooterCoverStartCondition()
        {
            return
                true
#if MIS_CRAWLING
                && (shooterInput.cc.IsCrawlingOnAction ? (allowFromCrawling ? true : false) : true)
#endif
#if MIS_FREEFLYING
                && (shooterInput.cc.IsFreeFlyingOnAction ? (allowFromFreeFlying ? true : false) : true)
#endif
#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
                && !shooterInput.cc.IsLedgeClimbOnAction
#endif
#if MIS_SOFTFLYING
                && (shooterInput.cc.IsSoftFlyingOnAction ? (allowFromSoftFlying ? true : false) : true)
#endif
#if MIS_SWIMMING
                && (shooterInput.cc.IsSwimOnAction ? (allowFromSwimming ? true : false) : true)
#endif
#if MIS_WALLRUN
                && !shooterInput.cc.IsWallRunOnAction
#endif
#if MIS_WATERDASH
                && !shooterInput.cc.IsWaterDashOnAction
#endif

#if MIS_INVECTOR_PARACHUTE
                && (shooterInput.cc.IsVParachuteOnAction ? (allowFromParachute ? true : false) : true)
#endif
#if MIS_INVECTOR_PUSH
                && !shooterInput.cc.IsVPushOnAction
#endif
                ;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckChainedAction()
        {
            if (!goingToCoverPoint)
                return;

#if MIS_AIRDASH
            if (shooterInput.cc.IsAirDashOnAction)
                shooterInput.cc.misAirDash.ExitActionState();
#endif
#if MIS_GROUNDDASH
            if (shooterInput.cc.IsGroundDashOnAction)
                shooterInput.cc.misGroundDash.ExitActionState();
#endif
#if MIS_CRAWLING
            if (shooterInput.cc.IsCrawlingOnAction && allowFromCrawling)
                shooterInput.cc.misCrawling.Interrupt();
#endif
#if MIS_FREEFLYING
            if (shooterInput.cc.IsFreeFlyingOnAction && allowFromFreeFlying)
                shooterInput.cc.misFreeFlying.ExitActionState();
#endif
#if MIS_SOFTFLYING
            if (shooterInput.cc.IsSoftFlyingOnAction && allowFromSoftFlying)
                shooterInput.cc.misSoftFlying.ExitActionState();
#endif
#if MIS_SWIMMING
            if (shooterInput.cc.IsSwimOnAction && allowFromSwimming)
                shooterInput.cc.misSwimming.ExitWater();
#endif

#if MIS_INVECTOR_PARACHUTE
            if (shooterInput.cc.IsVParachuteOnAction && allowFromParachute)
                shooterInput.cc.vmisParachute.ExitActionState();
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void HandleEnterExitCover()
        {
            if (!ShooterCoverStartCondition())
                return;

            CheckChainedAction();

            base.HandleEnterExitCover();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        //protected override void ExitCover(bool forceExit = false)
        //{
        //    base.ExitCover(forceExit);
        //}

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        //public virtual void EnterActionState()
        //{
        //}

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void ExitActionState(bool forceExit)
        {
            ExitCover(forceExit);
        }
    }
}
#endif
