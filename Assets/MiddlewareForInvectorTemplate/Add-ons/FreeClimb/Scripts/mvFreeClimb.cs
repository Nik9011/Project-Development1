#pragma warning disable 0414

#if MIS_INVECTOR_FREECLIMB
using Invector;
using Invector.vCharacterController.vActions;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("FreeClimb", iconName = "misIconRed")]
    public class mvFreeClimb : vFreeClimb
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("ChainedAction", order = 98)]
        [mvReadOnly] [SerializeField] string chainedAction = "Chained-Actions are provided as an option";
#if MIS_FREEFLYING
        public bool allowFromFreeFlying = true;
#endif
#if MIS_GRAPPLINGHOOK
        public bool allowFromGrapplingHook = true;
#endif
#if MIS_GRAPPLINGROPE
        public bool allowFromGrapplingRope = true;
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


#if MIS_GRAPPLINGROPE
        protected Vector3 ikLeftHandPosition;
#endif


        // ----------------------------------------------------------------------------------------------------
        // 
        public bool IsOnAction
        {
            get => dragInfo.inDrag;
        }


#if MIS_INVECTOR_FREECLIMB
        // ----------------------------------------------------------------------------------------------------
        // 
        protected int isFreeClimbHash = Animator.StringToHash("IsFreeClimb");
        protected bool IsFreeClimb
        {
            get => TP_Input.cc.animator.GetBool(isFreeClimbHash);
            set => TP_Input.cc.animator.SetBool(isFreeClimbHash, value);
        }
#endif


#if MIS_GRAPPLINGROPE
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void Update()
        {
            if (IsFreeClimb && TP_Input.cc.IsGrapplingRopeOnAction)
            {
                input = Vector3.zero;
                TP_Input.cc.animator.SetFloat(Invector.vCharacterController.vAnimatorParameters.InputHorizontal, 0);
                TP_Input.cc.animator.SetFloat(Invector.vCharacterController.vAnimatorParameters.InputVertical, 0);
            }
            else
            {
                base.Update();
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void OnAnimatorIK()
        {
            if (IsFreeClimb && TP_Input.cc.IsGrapplingRopeOnAction)
            {
                TP_Input.cc.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                TP_Input.cc.animator.SetIKPosition(AvatarIKGoal.LeftHand, ikLeftHandPosition);

                return;
            }

            base.OnAnimatorIK();

            ikLeftHandPosition = TP_Input.cc.animator.GetIKPosition(AvatarIKGoal.LeftHand);
        }
#endif

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual bool FreeClimbStartCondition()
        {
            return
                true
#if MIS_CRAWLING
                && !TP_Input.cc.IsCrawlingOnAction
#endif
#if MIS_FREEFLYING
                && (TP_Input.cc.IsFreeFlyingOnAction ? (allowFromFreeFlying ? true : false) : true)
#endif
#if MIS_GRAPPLINGHOOK
                && (TP_Input.cc.IsGrapplingHookOnAction ? (allowFromGrapplingHook ? true : false) : true)
#endif
#if MIS_GRAPPLINGROPE
                && (TP_Input.cc.IsGrapplingRopeOnAction ? (allowFromGrapplingRope ? true : false) : true)
#endif
#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
                && !TP_Input.cc.IsLedgeClimbOnAction
#endif
#if MIS_SOFTFLYING
                && (TP_Input.cc.IsSoftFlyingOnAction ? (allowFromSoftFlying ? true : false) : true)
#endif
#if MIS_SWIMMING
                && (TP_Input.cc.IsSwimOnAction ? (allowFromSwimming ? true : false) : true)
#endif

#if MIS_INVECTOR_PARACHUTE
                && (TP_Input.cc.IsVParachuteOnAction ? (allowFromParachute ? true : false) : true)
#endif
                ;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckChainedAction()
        {
#if MIS_FREEFLYING
            if (TP_Input.cc.IsFreeFlyingOnAction && allowFromFreeFlying)
                TP_Input.cc.misFreeFlying.ExitActionState();
#endif
#if MIS_GRAPPLINGHOOK
            if (TP_Input.cc.IsGrapplingHookOnAction && allowFromGrapplingHook)
                TP_Input.cc.misGrapplingHook.Interrupt();
#endif
#if MIS_GRAPPLINGROPE
            if (TP_Input.cc.IsGrapplingRopeOnAction && allowFromGrapplingRope)
                TP_Input.cc.misGrapplingRope.Interrupt();
#endif
#if MIS_SOFTFLYING
            if (TP_Input.cc.IsSoftFlyingOnAction && allowFromSoftFlying)
                TP_Input.cc.misSoftFlying.ExitActionState();
#endif
#if MIS_SWIMMING
            if (TP_Input.cc.IsSwimOnAction && allowFromSwimming)
                TP_Input.cc.misSwimming.ExitWater();
#endif
#if MIS_WATERDASH
            if (TP_Input.cc.IsWaterDashOnAction)
                TP_Input.cc.misWaterDash.ExitActionState();
#endif

#if MIS_INVECTOR_PARACHUTE
            if (TP_Input.cc.IsVParachuteOnAction && allowFromParachute)
                TP_Input.cc.vmisParachute.ExitActionState();
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void EnterClimb()
        {
            if (!FreeClimbStartCondition())
                return;

            IsFreeClimb = true; // Must be set before CheckChainedAction() for Chained-Action
            CheckChainedAction();

            base.EnterClimb();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void ExitClimb(bool exitOnGround = false)
        {
            base.ExitClimb(exitOnGround);

            IsFreeClimb = false;
            TP_Input.cc.disableCheckGround = false;

#if MIS_GRAPPLINGROPE
            if (TP_Input.cc.IsGrapplingRopeOnAction && allowFromGrapplingRope)
                oldInput -= 1.8f;   // FreeClimb cannot be run within 2 sec. To solve this problem, adjusted it to run again within 0.2 sec max.
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void Interrupt()
        {
            ExitClimb();
        }
    }
}
#endif
