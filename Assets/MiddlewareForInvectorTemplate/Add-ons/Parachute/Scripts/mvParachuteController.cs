#pragma warning disable 0414

#if MIS_INVECTOR_PARACHUTE
using Invector;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("ParachuteController", iconName = "misIconRed")]
    public class mvParachuteController : vParachuteController
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Input/Movement")]
        [Min(0f)] public float groundDetectionDistance = 10f;

        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("ChainedAction", order = 98)]
        //[mvReadOnly] [SerializeField] string chainedAction = "Chained-Actions are provided as an option";
        public LayerMask waterLayerMask = 1 << MISRuntimeTagLayer.LAYER_TRIGGERS;
#if MIS_AIRDASH
        public bool allowFromAirDash = true;
#endif
#if MIS_CARRIDER_EVP || MIS_CARRIDER_RCC || MIS_HELICOPTER || MIS_ROWINGBOAT
        public bool allowFromVehicleRider = true;
#endif

#if MIS_INVECTOR_ZIPLINE
        public bool allowFromZipLine = true;
#endif


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public bool ParachuteStartCondition()
        {
            tpInput.cc.transform.CheckWaterLevel(tpInput.cc._capsuleCollider, out RaycastHit waterhit, out mvFloatMinMax waterLevel, groundDetectionDistance, waterLayerMask);

            //if (waterLevel.current == 1 && waterLevel.max < minHeightToOpenParachute)
            //Debug.Log("Higher than water surface level");

            return
                canUseParachute
                && !usingParachute
                && !tpInput.cc.ragdolled
                && !tpInput.cc.isRolling
                && !tpInput.cc.customAction
                && (waterLevel.now == 1 ? waterLevel.max >= minHeightToOpenParachute : true)
                && tpInput.cc.groundDistance >= minHeightToOpenParachute    // The original condition is >
                && timerToReOpen < Time.time
#if MIS_AIRDASH
                && (tpInput.cc.IsAirDashOnAction ? (allowFromAirDash ? true : false) : true)
#endif
#if MIS_CARRIDER_EVP || MIS_CARRIDER_RCC || MIS_HELICOPTER || MIS_ROWINGBOAT
                && VehicleRiderCondition()
#endif
#if MIS_FREEFLYING
                && !tpInput.cc.IsFreeFlyingOnAction
#endif
#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
                && !tpInput.cc.IsLedgeClimbOnAction
#endif
#if MIS_SOFTFLYING
                // MIS recommends to use SubInput of SoftFlying in order to distinguish inputs between SoftFlying and Parachute
                && !(tpInput.cc.IsSoftFlyingOnAction && tpInput.cc.misSoftFlying.flyingToggleSubInput.useInput && tpInput.cc.misSoftFlying.flyingToggleSubInput.GetButton())
#endif
#if MIS_SWIMMING
                && !tpInput.cc.IsSwimOn
#endif
#if MIS_WATERDASH
                && !tpInput.cc.IsWaterDashOnAction
#endif

#if MIS_INVECTOR_ZIPLINE
                && (tpInput.cc.IsVZiplineOnAction ? (allowFromZipLine ? true : false) : true)
#endif
                ;
        }

#if MIS_CARRIDER_EVP || MIS_CARRIDER_RCC || MIS_HELICOPTER || MIS_ROWINGBOAT
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        bool VehicleRiderCondition()
        {
            if (!tpInput.cc.IsVehicleRiderOnAction)
                return true;

            if (tpInput.cc.IsVehicleRiderOnAction && !allowFromVehicleRider)
                return false;

            /*
#if MIS_HELICOPTER
            System.Type vehicleAssistantType = tpInput.cc.misVehicleRider.vehicleAssistant.GetType();
            //?
            if (vehicleAssistantType == typeof(mvAH64D) || vehicleAssistantType == typeof(mvHelicopter))
                return tpInput.cc.misVehicleRider.vehicleAssistant.currentEntry.seatState != VehicleEntryState.Occupied;
            else
#endif
            */
            return allowFromVehicleRider;
        }
#endif

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckChainedAction()
        {
#if MIS_AIRDASH
            if (tpInput.cc.IsAirDashOnAction && allowFromAirDash)
            {
                tpInput.cc.misAirDash.ExitActionState();
                tpInput.cc._rigidbody.velocity = Vector3.zero;
                tpInput.cc._rigidbody.angularVelocity = Vector3.zero;
            }
#endif
#if MIS_CARRIDER_EVP || MIS_CARRIDER_RCC || MIS_HELICOPTER || MIS_ROWINGBOAT
            if (tpInput.cc.IsVehicleRiderOnAction && allowFromVehicleRider)
                tpInput.cc.misVehicleRider.Interrupt();
#endif

#if MIS_SOFTFLYING
            if (tpInput.cc.IsSoftFlyingOnAction)
                tpInput.cc.misSoftFlying.ExitActionState();
#endif

#if MIS_INVECTOR_ZIPLINE
            if (tpInput.cc.IsVZiplineOnAction && allowFromZipLine)
                tpInput.cc.vmisZipLine.ExitZipline();
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void UpdateParachute()
        {
            if (openCloseParachute.GetButtonDown())
            {
                if (ParachuteStartCondition())
                {
                    CheckChainedAction();
                    OpenParachute();
                }
                else if (usingParachute)
                {
                    ExitParachuteImmedite();
                }
            }

            if (usingParachute && tpInput.cc.ragdolled)
                ExitParachuteImmedite();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        //protected override void CloseParachute()
        //{
        //    base.CloseParachute();
        //}

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        //public virtual void EnterActionState()
        //{
        //    OpenParachute();
        //}

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void ExitActionState()
        {
            CloseParachute();
        }
    }
}
#endif
