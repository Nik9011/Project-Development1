#if MIS_FSM_AI && INVECTOR_AI_TEMPLATE
using Invector;
using Invector.vCharacterController;
using Invector.vCharacterController.AI;
using UnityEngine;
using UnityEngine.AI;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("mvControlAI", iconName = "misIconRed")]
    public class mvControlAI : vControlAI
    {
        [vEditorToolbar("MIS", order = 90)]
        public bool debugMode = false;

#if (MIS_AI_CARRIDER_EVP || MIS_AI_CARRIDER_RCC) && (MIS_LOCKON || MIS_LOCKON2)
        [Header("AI Car")]
        [Range(1, 10)] public int findVehicleRate = 5;
        [Min(0.1f)] public float maxVehicleDistance = 50f;
        [Range(0f, 360f)] public float maxHorizontalVehicleAngle = 150f;
        [mvReadOnly] public mvAIVehicleController targetAIVehicle;
#endif

        protected vRagdoll ragdoll;

#if (MIS_AI_CARRIDER_EVP || MIS_AI_CARRIDER_RCC || MIS_AI_HELICOPTER) && (MIS_LOCKON || MIS_LOCKON2)
        // ----------------------------------------------------------------------------------------------------
        // 
        public virtual mvAIVehicleController TargetAIVehicle
        {
            set => targetAIVehicle = value;
            get => targetAIVehicle;
        }
#endif


        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void Start()
        {
            base.Start();

            TryGetComponent(out ragdoll);
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void SetUseNavMeshAgent(bool enable)
        {
            useNavMeshAgent = enable;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual LayerMask GetDetectionLayerMask()
        {
            return _detectLayer;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual vTagMask GetDetectionTags()
        {
            return _detectTags;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual LayerMask GetObstacleLayerMask()
        {
            return _obstacles;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual bool IsRagdolled()
        {
            if (ragdoll == null)
                return false;

            return ragdoll.isActive;
        }

#if (MIS_AI_CARRIDER_EVP || MIS_AI_CARRIDER_RCC || MIS_AI_HELICOPTER) && (MIS_LOCKON || MIS_LOCKON2)
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void FindTargetAIVehicleAction(LayerMask targetMarkLayerMask)
        {
            if ((Time.frameCount % findVehicleRate) != 0)
                return;

            mvTarget target = mvTargetManager.Instance.FindVehicle(transform, targetMarkLayerMask, maxVehicleDistance, maxHorizontalVehicleAngle);

            if (target != null)
                TargetAIVehicle = target.trRoot.gameObject.GetComponent<mvAIVehicleController>();
            else
                TargetAIVehicle = null;
        }
        public virtual bool FindTargetAIVehicleDecision(LayerMask targetMarkLayerMask)
        {
            mvTarget target = mvTargetManager.Instance.FindVehicle(transform, targetMarkLayerMask, maxVehicleDistance, maxHorizontalVehicleAngle);

            if (target != null)
            {
                TargetAIVehicle = target.trRoot.gameObject.GetComponent<mvAIVehicleController>();
                return true;
            }
            else
            {
                TargetAIVehicle = null;
                return false;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual bool FoundTargetAIVehicle()
        {
            return TargetAIVehicle != null && TargetAIVehicle.gameObject.activeInHierarchy;
        }

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void ClearTargetAIVehicle()
        {
            TargetAIVehicle = null;
        }
#endif

#if UNITY_EDITOR
        protected float gizmoPointRadius = 0.2f;

        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (!debugMode)
                return;

            if (navMeshAgent.path != null)
            {
                if (navMeshAgent.path.status == NavMeshPathStatus.PathComplete)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(navMeshAgent.path.corners[0], gizmoPointRadius);

                    for (int i = 1; i < navMeshAgent.path.corners.Length; i++)
                    {
                        Gizmos.DrawLine(navMeshAgent.path.corners[i - 1], navMeshAgent.path.corners[i]);
                        Gizmos.DrawSphere(navMeshAgent.path.corners[i], gizmoPointRadius);
                    }

                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(navMeshAgent.path.corners[navMeshAgent.path.corners.Length - 1], gizmoPointRadius);
                    Gizmos.color = Color.white;
                }
            }
        }
#endif
    }
}
#endif