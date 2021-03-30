using Mirror;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Prop
{
    public class PropMovement : NetworkBehaviour
    {
        /// <summary>
        /// Small offset for computing when player has stopped moving
        /// </summary>
        public const float Epsilon = 0.001f;

        /// <summary>
        /// Maximum angle between two colliding objects
        /// </summary>
        public const float MaxAngleShoveRadians = 90.0f;

        /// <summary>
        /// Movement speed (in units per second)
        /// </summary>
        public float movementSpeed = 2.0f;

        /// <summary>
        /// Distance to ground at which player is considered grounded
        /// </summary>
        public float groundedDistance = 0.01f;

        /// <summary>
        /// Distance to check player distance to ground
        /// </summary>
        public float groundCheckDistance = 10f;

        /// <summary>
        /// Mocked unity service for accessing inputs, delta time, and
        /// various other static unity inputs in a testable manner.
        /// </summary>
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Network service for managing network calls
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Maximum angle at which the player can walk (in degrees)
        /// </summary>
        public float maxWalkAngle = 60f;

        /// <summary>
        /// Maximum number of time player can bounce of walls/floors/objects during an update
        /// </summary>
        public int maxBounces = 5;

        /// <summary>
        /// Direction and strength of gravity
        /// </summary>
        public Vector3 gravity = new Vector3(0, -9.807f, 0);

        /// <summary>
        /// Player input movement from controller
        /// </summary>
        public Vector3 inputMovement;

        /// <summary>
        /// Current player velocity
        /// </summary>
        public Vector3 velocity;

        /// <summary>
        /// Velocity of player jump in units per second
        /// </summary>
        public float jumpVelocity = 5.0f;

        /// <summary>
        /// Is the player attempting to jump
        /// </summary>
        public bool attemptingJump;

        /// <summary>
        /// Maximum distance the player can be pushed out of overlapping objects in units per second
        /// </summary>
        public float maxPushSpeed = 10.0f;

        public void Start()
        {
            this.networkService = new NetworkService(this);
        }

        public void Update()
        {
            // Get palyer input on a frame by frame basis
            inputMovement = new Vector3(unityService.GetAxis("Horizontal"), 0, unityService.GetAxis("Vertical"));
            // Normalize movement vector to be a max of 1 if greater than one
            inputMovement = inputMovement.magnitude > 1 ? inputMovement / inputMovement.magnitude : inputMovement;

            // Get other movemen inputs
            this.attemptingJump = unityService.GetButton("Jump");
        }        

        public void FixedUpdate()
        {
            if (!networkService.isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }

            float deltaTime = unityService.deltaTime;

            // If player is not allowed to move, stop player movement
            if (PlayerInputManager.playerMovementState == PlayerInputState.Deny)
            {
                inputMovement = Vector3.zero;
            }

            // Rotate movement vector by player yaw (rotation about vertical axis)
            Quaternion horizPlaneView = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            Vector3 playerMovementDirection = horizPlaneView * inputMovement;

            Vector3 movement = playerMovementDirection * movementSpeed;

            // Update grounded state and increase velocity if falling
            bool isGrounded = CheckGrounded();
            if (isGrounded && !attemptingJump)
            {
                velocity = Vector3.zero;
            }
            else if (!isGrounded)
            {
                velocity += gravity * deltaTime;
            }

            // Give the player some vertical velocity if they are jumping and grounded
            if (isGrounded && attemptingJump)
            {
                velocity = this.jumpVelocity * -gravity.normalized;
            }

            MovePlayer((movement + velocity) * deltaTime);

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        // public void PushOutOverlapping()
        // {
        //     float deltaTime = unityService.deltaTime;

        //     // Collider cast to move player
            // ColliderCast colliderCast = GetComponent<ColliderCast>();
            // Collider collider = GetComponent<Collider>();

        //     if (collider == null)
        //     {
        //         return;
        //     }

        //     Vector3 center = collider.bounds.center + transform.position;

        //     foreach (ColliderCastHit overlap in colliderCast.GetOverlappingDirectional())
        //     {
        //         Vector3 overlapPoint = Physics.ClosestPoint(overlap.collider.bounds.center, collider, transform.position, transform.rotation);
        //         Vector3 overlapVector = overlapPoint - center;
        //         bool hitSomething = Physics.Raycast(center, overlapVector, out RaycastHit hit, collider.bounds.extents.magnitude, 0, QueryTriggerInteraction.Ignore);
        //         // Push our character by that overlap * max push speed * time
        //         Vector3 pushMovement = hit.normal * maxPushSpeed * deltaTime;
        //         // If the push movement is longer than the overlap, use overlap instead
        //         Vector3 boundedPush = pushMovement.magnitude > hit.distance ? overlapVector : pushMovement;
        //         // Move character by push
        //         transform.position += boundedPush;
        //         UnityEngine.Debug.Log(transform.position + " " + overlapPoint);
        //         UnityEngine.Debug.DrawLine(transform.position, overlapPoint, Color.red);
        //     }
        // }

        public bool CheckGrounded()
        {
            // Collider cast to move player
            ColliderCast colliderCast = GetComponent<ColliderCast>();

            ColliderCastHit hit = colliderCast.CastSelf(Vector3.down, groundCheckDistance);
            return hit.hit && hit.distance <= groundedDistance && Vector3.Angle(hit.normal, -gravity) <= maxWalkAngle;
        }

        public void MovePlayer(Vector3 movement)
        {
            // Save current momentum
            Vector3 momentum = movement;
            
            // Collider cast to move player
            ColliderCast colliderCast = GetComponent<ColliderCast>();
            // current number of bounces
            int bounces = 0;

            // Continue computing while there is momentum and bounces remaining
            while (momentum.magnitude > Epsilon && bounces <= maxBounces)
            {
                // Do a cast of the collider to see if an object is hit during this
                // movement bounce
                ColliderCastHit hit = colliderCast.CastSelf(momentum.normalized, momentum.magnitude);

                if (!hit.hit)
                {
                    // If there is no hit, move to desired position
                    transform.position += momentum;
                    // Exit as we are done bouncing
                    break;
                }

                // Set the fraction of remaining movement (minus some small value)
                transform.position += momentum * (hit.fraction);
                // Push slightly along normal to stop from getting caught in walls
                transform.position += hit.normal * Epsilon;
                // Decrease remaining momentum by fraction of movement remaining
                momentum *= (1 - hit.fraction);

                // Apply some force to the object hit if it is moveable, Apply force on entity hit
                // bool isKinematic = physicsMassGetter.HasComponent(hit.Entity) && IsKinematic(physicsMassGetter[hit.Entity]);
                // if (hit.RigidBodyIndex < collisionWorld.NumDynamicBodies && !isKinematic)
                // {
                //     commandBuffer.AddBuffer<PushForce>(jobIndex, hit.Entity);
                //     commandBuffer.AppendToBuffer(jobIndex, hit.Entity, new PushForce() { force = movement * pushPower, point = hit.Position });
                //     // If pushing something, reduce remaining force significantly
                //     remaining *= pushDecay;
                // }

                // Snap character vertically up if they hit something
                //  close enough to their feet
                float distanceToFeet = hit.pointHit.y - transform.position.y;
                // if (distanceToFeet > 0 && distanceToFeet < verticalSnapUp)
                // {
                //     // Increment vertical (y) value of new position by
                //     //  the distance to the feet of the character
                //     from = from - distanceToFeet * math.normalizesafe(gravityDirection);
                //     // Project rest of movement onto plane perpendicular to gravity
                //     planeNormal = -gravityDirection;
                // }
                // Only apply angular change if hitting something
                // else
                // {
                    // Get angle between surface normal and remaining movement
                    float angleBetween = Vector3.Angle(hit.normal, momentum);
                    // Normalize angle between to be between 0 and 1
                    // 0 means no angle, 1 means 90 degree angle
                    angleBetween = Mathf.Min(MaxAngleShoveRadians, Mathf.Abs(angleBetween));
                    float normalizedAngle = angleBetween / MaxAngleShoveRadians;
                    // Create angle factor using 1 / (1 + normalizedAngle)
                    float angleFactor = 1.0f / (1.0f + normalizedAngle);
                    // Reduce the momentum by the remaining movement that ocurred
                    momentum *= Mathf.Pow(angleFactor, 1.1f);
                // }
                // Rotate the remaining remaining movement to be projected along the plane 
                // of the surface hit (emulate pushing against the object)
                float momentumLeft = momentum.magnitude;
                momentum = Vector3.ProjectOnPlane(momentum, hit.normal);
                momentum = momentum.normalized * momentumLeft;

                // Track number of times the character has bounced
                bounces++;
            }
            // We're done, player was moved as part of loop
        }
    }
}
