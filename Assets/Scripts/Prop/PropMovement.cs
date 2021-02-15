using Mirror;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Prop
{
    public class PropMovement : NetworkBehaviour
    {
        /// <summary>
        /// Movement speed (in units per second)
        /// </summary>
        public float movementSpeed = 2.0f;

        /// <summary>
        /// Mocked unity service for accessing inputs, delta time, and
        /// various other static unity inputs in a testable manner.
        /// </summary>
        public IUnityService unityService = new UnityService();

        public void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }

            float deltaTime = unityService.deltaTime;

            Vector3 movement = new Vector3(unityService.GetAxis("Horizontal"), 0, unityService.GetAxis("Vertical"));
            // If player is not allowed to move, stop player movement
            if (PlayerInputManager.playerMovementState == PlayerInputState.Deny)
            {
                movement = Vector3.zero;
            }
            // Normalize movement vector to be a max of 1 if greater than one
            movement = movement.magnitude > 1 ? movement / movement.magnitude : movement;

            // Rotate movement vector by player yaw (rotation about vertical axis)
            Quaternion horizPlaneView = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            Vector3 movementVelocity = horizPlaneView * movement * movementSpeed;
            movementVelocity.y = GetComponent<Rigidbody>().velocity.y;

            // Set player rigidbody velocity based on movement velocity
            GetComponent<Rigidbody>().velocity = movementVelocity;
        }
    }
}
