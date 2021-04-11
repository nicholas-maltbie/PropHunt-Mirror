using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Animation
{
    [RequireComponent(typeof(Animator))]
    public class IKControl : NetworkBehaviour
    {
        /// <summary>
        /// Animator associated with this control object
        /// </summary>
        protected Animator animator;

        /// <summary>
        /// Are the Inverse Kinematics controls enabled for this character
        /// </summary>
        public bool ikActive = false;

        /// <summary>
        /// Transform (position and rotation) target for the character's right hand
        /// </summary>
        public Transform rightHandTarget = null;

        /// <summary>
        /// Transform (position and rotation) target for the character's left hands
        /// </summary>
        public Transform leftHandTarget = null;

        /// <summary>
        /// Transform (position and rotation) target for the character's right foot
        /// </summary>
        public Transform rightFootTarget = null;

        /// <summary>
        /// Transform (position and rotation) target for the character's left foot
        /// </summary>
        public Transform leftFootTarget = null;

        /// <summary>
        /// Where is the player currently looking (just positional data)
        /// </summary>
        public Transform lookObj = null;

        /// <summary>
        /// Weight of right hand target (how much do I override current position).
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float rightHandWeight = 1.0f;

        /// <summary>
        /// Weight of the left hand target (how much do I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float leftHandWeight = 1.0f;

        /// <summary>
        /// Weight of the right foot target (how much do I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float rightFootweight = 1.0f;

        /// <summary>
        /// Weight of the left foot target (how much do I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float leftFootWeight = 1.0f;

        /// <summary>
        /// Weight of overriding right elbow hint position (how much should I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float rightElbowWeight = 0.5f;

        /// <summary>
        /// Weight of overriding the left elbow hint position (how much should I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float leftElbowWeight = 0.5f;

        /// <summary>
        /// Weight of overriding the right knee hint position (how much should I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float rightKneeWeight = 0.5f;

        /// <summary>
        /// Weight of overriding the left knee hint position (how much should I override current position)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float leftKneeWeight = 0.5f;

        /// <summary>
        /// Weight of overriding the head look rotation (how much do I change it from current)
        /// Should be between 0 and 1.
        /// </summary>
        [Range(0, 1)]
        public float lookWeight = 1.0f;

        public INetworkService networkService;

        void Start()
        {
            animator = GetComponent<Animator>();
            this.networkService = new NetworkService(this);
        }

        public void OnAnimatorIK()
        {
            // Only configure animator if local player
            if (!networkService.isLocalPlayer)
            {
                return;
            }

            // Skip code if animator is not initialized
            if (!animator)
            {
                return;
            }

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                // Set the look target position, if one has been assigned
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandTarget != null)
                {
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightElbowWeight);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (leftHandTarget != null)
                {
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftElbowWeight);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightFootTarget != null)
                {
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightKneeWeight);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootweight);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootweight);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTarget.rotation);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (leftFootTarget != null)
                {
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftKneeWeight);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTarget.rotation);
                }
            }

            //if the IK is not active, set the position and rotation of the targets back to the original position
            else
            {
                animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

                animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);

                animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);

                animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);

                animator.SetLookAtWeight(0);
            }
        }
    }
}
