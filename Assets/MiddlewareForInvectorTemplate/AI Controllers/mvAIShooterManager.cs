#if MIS_FSM_AI && INVECTOR_AI_TEMPLATE
using Invector;
using Invector.vCharacterController.AI;
#if INVECTOR_SHOOTER
using Invector.vShooter;
#endif
using UnityEngine;

namespace com.mobilin.games
{
    // ----------------------------------------------------------------------------------------------------
    // 
    // ----------------------------------------------------------------------------------------------------
    [vClassHeader("mvAIShooterManager", iconName = "misIconRed")]
    public class mvAIShooterManager : vAIShooterManager
    {
        // ----------------------------------------------------------------------------------------------------
        // 
        // ----------------------------------------------------------------------------------------------------
        public virtual void Init()
        {
            Animator animator = GetComponent<Animator>();

            if (animator)
            {
                var _rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
                var _lefttHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);

                var weaponR = _rightHand.GetComponentInChildren<vShooterWeapon>();
                var weaponL = _lefttHand.GetComponentInChildren<vShooterWeapon>();

                if (weaponR != null)
                    SetRightWeapon(weaponR.gameObject);
                else
                    rWeapon = null;

                if (weaponL != null)
                    SetLeftWeapon(weaponL.gameObject);
                else
                    lWeapon = null;
            }

            if (!ignoreTags.Contains(gameObject.tag))
                ignoreTags.Add(gameObject.tag);
        }
    }
}
#endif