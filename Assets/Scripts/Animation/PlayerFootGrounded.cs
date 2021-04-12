
using Mirror;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Animation
{
    /// <summary>
    /// Allow for player feet to stick on ground
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerFootGrounded : MonoBehaviour
    {
        /// <summary>
        /// Animator for getting current bone positions
        /// </summary>
        public Animator animator;

        public void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void OnAnimatorIK()
        {            
            Vector3 leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
            Vector3 rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot).position;

            leftFoot = GetHitPosition(leftFoot + Vector3.up, leftFoot - Vector3.up * 0.1f);
            rightFoot = GetHitPosition(rightFoot + Vector3.up, rightFoot - Vector3.up * 0.1f);

            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFoot + Vector3.up * 0.1f);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFoot + Vector3.up * 0.1f);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0.6f);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0.6f);
        }

        public Vector3 GetHitPosition(Vector3 start, Vector3 end) {
            UnityEngine.Debug.DrawLine(start, end);
            if (Physics.Linecast(start, end, out RaycastHit hit))
            {
                return hit.point;
            }
            return end;
        }
    }
}