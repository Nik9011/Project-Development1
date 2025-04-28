#pragma warning disable 0414

#if MIS_INVECTOR_PUSH
using Invector;
using Invector.vCharacterController;
using System.Collections;
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("mvPushActionController", iconName = "misIconRed")]
    public class mvPushActionController : vPushActionController, IMISColliderFilter
    {
        [vEditorToolbar("Settings")]
        [Space(10)]
        public GenericInput rotateSubInput = new GenericInput("LeftControl", "", "");
        public float rotateSpeed = 10f;


        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("Debug")]
        [SerializeField] protected bool debugMode = false;


        // ----------------------------------------------------------------------------------------------------
        // 
        [vEditorToolbar("RayCast", order = 3)]
        public mvMISSphereCast pushableCaster = new mvMISSphereCast((0.2f * Vector3.up), 0.1f, 0.8f);


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
        protected Transform tr;
        internal Collider[] colliders;

        protected float oldTargetMass;
        protected float oldStrength;

        protected Vector3 oldStartDirection;
        protected float lastBodyEulerY;


        // ----------------------------------------------------------------------------------------------------
        // 
        public bool IsOnAction
        {
            get => isStarted;
        }


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void Awake()
        {
            // IMISColliderFilter
            colliders = GetComponentsInChildren<Collider>();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void Start()
        {
            base.Start();

            tr = transform;
            oldStrength = strength;
        }

        // ----------------------------------------------------------------------------------------------------
        // IMISColliderFilter
        // ----------------------------------------------------------------------------------------------------
        public bool FilterCollider(Collider other)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].Equals(other))
                    return true;
            }

            return false;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void UpdateInput()
        {
            PushAndPullInput();
            MoveInput();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void PushAndPullInput()
        {
            if (startPushPullInput.useInput && startPushPullInput.GetButtonDown())
            {
                if (tpInput.enabled && !isStarted && pushPoint == null && !isPushingPulling)
                {
                    pushableCaster.Cast(tr, tr.forward, pushpullLayer, QueryTriggerInteraction.Collide, this, 0f, true, debugMode);
                    
                    if (pushableCaster.isDetected)
                    {
                        var _object = pushableCaster.hit.collider.gameObject.GetComponent<mvPushObjectPoint>();

                        if (_object && pushPoint != _object && _object.CanUse)
                        {
                            pushPoint = _object as mvPushObjectPoint;
                            onFindObject.Invoke();
                        }
                        else if (_object == null && pushPoint)
                        {
                            pushPoint = null;
                            onLostObject.Invoke();
                        }
                    }
                    else if (pushPoint)
                    {
                        pushPoint = null;
                        onLostObject.Invoke();
                    }

                    if (pushPoint && (pushPoint as mvPushObjectPoint).CanUse)
                        StartCoroutine(StartPushAndPull((pushPoint as mvPushObjectPoint).startAnimation));
                }
                else if (isPushingPulling)
                {
                    StartCoroutine(StopPushAndPull((pushPoint as mvPushObjectPoint).stopAnimation));
                }
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void MoveObject()
        {
            if (Mathf.Abs(inputHorizontal) > 0 && rotateSubInput.useInput && rotateSubInput.GetButton())
            {
                Quaternion delta = Quaternion.Euler(0f, -inputHorizontal * rotateSpeed * vTime.fixedDeltaTime, 0f);
                pushPoint.targetBody.MoveRotation(pushPoint.targetBody.rotation * delta);

                bool _isMoving = pushPoint.targetBody.transform.eulerAngles.y != lastBodyEulerY;

                if (_isMoving != isMoving)
                {
                    isMoving = _isMoving;

                    if (isMoving)
                    {
                        pushPoint.pushableObject.onStartMove.Invoke();
                    }
                    else
                    {
                        pushPoint.pushableObject.onMovimentSpeedChanged.Invoke(0);
                        pushPoint.pushableObject.onStopMove.Invoke();
                    }
                }

                if (isMoving)
                    pushPoint.pushableObject.onMovimentSpeedChanged.Invoke(Mathf.Clamp(pushPoint.targetBody.velocity.magnitude, 0, 1f));

                lastBodyEulerY = pushPoint.targetBody.transform.eulerAngles.y;
            }
            else
            {
                base.MoveObject();
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void MoveCharacter()
        {
            startDirection = (pushPoint.targetBody.position - transform.position).normalized;

            if (Mathf.Abs(inputHorizontal) > 0 && rotateSubInput.useInput && rotateSubInput.GetButton())
            {
                tpInput.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 0f, 0.2f, Time.deltaTime);
                tpInput.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, isMoving ? inputHorizontal * inputWeight : 0, 0.2f, Time.deltaTime);
                tpInput.cc.animator.SetFloat(vAnimatorParameters.InputMagnitude, inputDirection.magnitude > 0.1f ? Mathf.Clamp(inputWeight, 0, 1f) : 1, 0.2f, vTime.fixedDeltaTime);

                Vector3 realPosition = GetCharacterPosition();
                transform.position = Vector3.Lerp(transform.position, realPosition, inputDirection.magnitude);
                transform.rotation = Quaternion.LookRotation(startDirection, Vector3.up);
            }
            else
            {
                base.MoveCharacter();
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual bool PushStartCondition()
        {
            return
                true
#if MIS_AIRDASH
                && !tpInput.cc.IsAirDashOnAction
#endif
#if MIS_CRAWLING
                && (tpInput.cc.IsCrawlingOnAction ? (allowFromCrawling ? true : false) : true)
#endif
#if MIS_FREEFLYING
                && (tpInput.cc.IsFreeFlyingOnAction ? (allowFromFreeFlying ? true : false) : true)
#endif
#if MIS_GRAPPLINGROPE
                && !(tpInput.cc.IsGrapplingRopeOnAction || tpInput.cc.IsGrapplingRopeOnMoveAction)
#endif
#if MIS_GRAPPLINGHOOK
                && !(tpInput.cc.IsGrapplingHookOnAction || tpInput.cc.IsGrapplingHookOnMoveAction)
#endif
#if MIS_LEDGECLIMB1 || MIS_LEDGECLIMB2
                && !tpInput.cc.IsLedgeClimbOnAction
#endif
#if MIS_SOFTFLYING
                && (tpInput.cc.IsSoftFlyingOnAction ? (allowFromSoftFlying ? true : false) : true)
#endif
#if MIS_SWIMMING
                && (tpInput.cc.IsSwimOn ? (allowFromSwimming ? true : false) : true)
#endif
#if MIS_WALLRUN
                && !tpInput.cc.IsWallRunOnAction
#endif
#if MIS_WATERDASH
                && !tpInput.cc.IsWaterDashOnAction
#endif

#if MIS_INVECTOR_FREECLIMB
                && !tpInput.cc.IsVFreeClimbOnAction
#endif
#if MIS_INVECTOR_PARACHUTE
                && (tpInput.cc.IsVParachuteOnAction ? (allowFromParachute ? true : false) : true)
#endif
                ;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void CheckChainedAction()
        {
#if MIS_AIRDASH
            if (tpInput.cc.IsAirDashOnAction)
                tpInput.cc.misAirDash.ExitActionState();
#endif
#if MIS_CRAWLING
            if (tpInput.cc.IsCrawlingOnAction && allowFromCrawling)
                tpInput.cc.misCrawling.Interrupt();
#endif
#if MIS_FREEFLYING
            if (tpInput.cc.IsFreeFlyingOnAction && allowFromFreeFlying)
                tpInput.cc.misFreeFlying.ExitActionState();
#endif
#if MIS_GROUNDDASH
            if (tpInput.cc.IsGroundDashOnAction)
                tpInput.cc.misGroundDash.ExitActionState();
#endif
#if MIS_SOFTFLYING
            if (tpInput.cc.IsSoftFlyingOnAction && allowFromSoftFlying)
                tpInput.cc.misSoftFlying.ExitActionState();
#endif
#if MIS_SWIMMING
            if (tpInput.cc.IsSwimOn && allowFromSwimming)
                tpInput.cc.misSwimming.ExitActionState();
#endif

#if MIS_INVECTOR_PARACHUTE
            if (tpInput.cc.IsVParachuteOnAction && allowFromParachute)
                tpInput.cc.vmisParachute.ExitActionState();
#endif
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override IEnumerator StartPushAndPull()
        {
            if (!PushStartCondition())
            {
                pushPoint = null;
                yield break;
            }

            CheckChainedAction();

            StartCoroutine(base.StartPushAndPull());
        }
        protected virtual IEnumerator StartPushAndPull(string animation)
        {
            if (!PushStartCondition())
            {
                pushPoint = null;
                yield break;
            }

            CheckChainedAction();

            if ((pushPoint as mvPushObjectPoint).useAutoStrength)
            {
                oldTargetMass = pushPoint.targetBody.mass;
                strength = oldTargetMass * (pushPoint as mvPushObjectPoint).autoStrengthMultiplier;
            }

            onStart.Invoke();
            tpInput.cc._rigidbody.velocity = Vector3.zero;
            tpInput.cc.enabled = false;
            tpInput.enabled = false;
            tpInput.cc.isJumping = false;
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, 0);
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 0);
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputMagnitude, 0);
            startDirection = pushPoint.transform.forward;
            startDirection.y = 0;
            startDirection.Normalize();

            oldStartDirection = startDirection;

            isStarted = true;
            var positionA = tr.position;
            Vector3 positionB = GetCharacterTargetPoisiton();
            positionB.y = tr.position.y;
            var rotationA = tr.rotation;
            var rotationB = Quaternion.LookRotation(startDirection, Vector3.up);
            tpInput.animator.PlayInFixedTime(animation);
            float _weight = 0;
            while (_weight < 1)
            {
                _weight += enterPositionSpeed * Time.deltaTime;
                weight = enterCurve.Evaluate(_weight);

                tr.position = Vector3.Lerp(positionA, positionB, weight);
                tr.rotation = Quaternion.Lerp(rotationA, rotationB, weight);
                yield return null;
            }

            lastBodyEulerY = pushPoint.targetBody.transform.eulerAngles.y;

            lastBodyPosition = pushPoint.targetBodyPosition;
            startLocalPosition = pushPoint.transform.InverseTransformPoint(positionB);
            startDistance = Vector3.Distance(tr.position, pushPoint.transform.position);
            weight = 1;
            tr.position = Vector3.Lerp(positionA, positionB, 1);
            tr.rotation = Quaternion.Lerp(rotationA, rotationB, 1);
            isPushingPulling = true;
            pushPoint.pushableObject.StartPushAndPull();
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual IEnumerator StopPushAndPull(string animation, bool playAnimation = true)
        {
            if (isMoving)
            {
                pushPoint.pushableObject.onStopMove.Invoke();
                isMoving = false;
            }

            if ((pushPoint as mvPushObjectPoint).useAutoStrength)
            {
                pushPoint.targetBody.mass = oldTargetMass;
                strength = oldStrength;
            }

            isStoping = true;
            isStarted = false;
            pushPoint.pushableObject.FinishPushAndPull();
            pushPoint.targetBody.velocity = Vector3.zero;
            pushPoint = null;
            onLostObject.Invoke();
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, 0);
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 0);
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputMagnitude, 0);
            isPushingPulling = false;
            if (playAnimation)
            {
                tpInput.animator.PlayInFixedTime(animation);
            }

            yield return new WaitForEndOfFrame();

            transform.rotation = Quaternion.LookRotation(oldStartDirection, Vector3.up);

            weight = 0;
            tpInput.cc.ResetInputAnimatorParameters();
            tpInput.cc.inputSmooth = Vector3.zero;
            tpInput.cc.input = Vector3.zero;
            tpInput.cc.inputMagnitude = 0;
            tpInput.cc.StopCharacter();
            tpInput.cc._rigidbody.velocity = Vector3.zero;
            tpInput.cc.moveDirection = Vector3.zero;
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, 0);
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 0);
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputMagnitude, 0);
            tpInput.cc.enabled = true;
            tpInput.enabled = true;
            onFinish.Invoke();
            isStoping = false;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void CheckBreakActionConditions()
        {
            // radius of the SphereCast
            float radius = tpInput.cc._capsuleCollider.radius * 0.9f;
            var dist = 10f;
            // ray for RayCast
            Ray ray2 = new Ray(transform.position + new Vector3(0, tpInput.cc.colliderHeight / 2, 0), Vector3.down);

            // raycast for check the ground distance
            if (Physics.Raycast(ray2, out tpInput.cc.groundHit, (tpInput.cc.colliderHeight / 2) + dist, tpInput.cc.groundLayer) && !tpInput.cc.groundHit.collider.isTrigger)
                dist = transform.position.y - tpInput.cc.groundHit.point.y;

            // sphere cast around the base of the capsule to check the ground distance
            if (dist >= groundMinDistance)
            {
                Vector3 pos = transform.position + Vector3.up * (tpInput.cc._capsuleCollider.radius);
                Ray ray = new Ray(pos, -Vector3.up);
                if (Physics.SphereCast(ray, radius, out tpInput.cc.groundHit, tpInput.cc._capsuleCollider.radius + groundMaxDistance, tpInput.cc.groundLayer) && !tpInput.cc.groundHit.collider.isTrigger)
                {
                    Physics.Linecast(tpInput.cc.groundHit.point + (Vector3.up * 0.1f), tpInput.cc.groundHit.point + Vector3.down * 0.15f, out tpInput.cc.groundHit, tpInput.cc.groundLayer);
                    float newDist = transform.position.y - tpInput.cc.groundHit.point.y;
                    if (dist > newDist)
                        dist = newDist;
                }
            }

            if (dist > groundMaxDistance || Vector3.Distance(transform.position, pushPoint.transform.TransformPoint(startLocalPosition)) > (breakActionDistance))
            {
                bool falling = dist > groundMaxDistance;

                if (falling)
                {
                    tpInput.cc.isGrounded = false;
                    tpInput.cc.animator.SetBool(vAnimatorParameters.IsGrounded, false);
                    tpInput.cc.animator.PlayInFixedTime("Falling");
                }

                StartCoroutine(StopPushAndPull((pushPoint as mvPushObjectPoint).stopAnimation, !falling));
            }
        }

#if UNITY_EDITOR
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected virtual void OnDrawGizmosSelected()
        {
            if (!debugMode)
                return;

            if (colliders == null)
                colliders = GetComponentsInChildren<Collider>();


            // ----------------------------------------------------------------------------------------------------
            // Pushable Detection
            // ----------------------------------------------------------------------------------------------------
            if (pushableCaster.useCast && !IsOnAction)
            {
                if (Application.isPlaying)
                    pushableCaster.DrawGizmosSphereCast(transform, transform.forward);
                else
                    pushableCaster.Cast(transform, transform.forward, pushpullLayer, QueryTriggerInteraction.Collide, this, 0f, true, true);
            }
        }
#endif
    }
}
#endif