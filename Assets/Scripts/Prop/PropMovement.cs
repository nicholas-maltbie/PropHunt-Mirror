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
        public float maxPushSpeed = 1.0f;

        /// <summary>
        /// Decay value of momentum when hitting another object.
        /// Should be between [0, 1]
        /// </summary>
        public float pushDecay = 0.9f;

        /// <summary>
        /// Decrease in momentum factor due to angle change when walking.
        /// Should be a positive float value. It's an exponential applied to 
        /// values between [0, 1] so values smaller than 1 create a positive
        /// curve and grater than 1 for a negative curve.
        /// </summary>
        public float anglePower = 0.5f;

        /// <summary>
        /// Distance that the character can "snap down" vertical steps
        /// </summary>
        public float verticalSnapDown = 0.2f;

        /// <summary>
        /// Was the player grounded this frame
        /// </summary>
        public bool onGround;

        public float stepUpDepth = 0.1f;

        public float verticalSnapUp = 0.3f;

        public float distanceToGround;

        public Vector3 surfaceNormal;

        public float angle;

        public bool StandingOnGround => onGround && distanceToGround <= groundedDistance && distanceToGround > 0;

        public bool Falling => !StandingOnGround || angle > maxWalkAngle;

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

            // push out of overlapping objects
            PushOutOverlapping();

            // Rotate movement vector by player yaw (rotation about vertical axis)
            Quaternion horizPlaneView = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            Vector3 playerMovementDirection = horizPlaneView * inputMovement;

            Vector3 movement = playerMovementDirection * movementSpeed;

            // Update grounded state and increase velocity if falling
            CheckGrounded();
            if (!Falling && !attemptingJump)
            {
                velocity = Vector3.zero;
            }
            else if (Falling)
            {
                velocity += gravity * deltaTime;
            }

            // Give the player some vertical velocity if they are jumping and grounded
            if (!Falling && attemptingJump)
            {
                velocity = this.jumpVelocity * -gravity.normalized;
            }

            // If the player is standing on the ground, project their movement onto the ground plane
            // This allows them to walk up gradual slopes without facing a hit in movement speed
            if (!Falling)
            {
                movement = Vector3.ProjectOnPlane(movement, surfaceNormal).normalized * movement.magnitude;
            }
            MovePlayer(movement * deltaTime);
            MovePlayer(velocity * deltaTime);

            if (StandingOnGround && !attemptingJump)
            {
                SnapPlayerDown();
            }

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        public void SnapPlayerDown()
        {
            // Collider cast to move player
            ColliderCast colliderCast = GetComponent<ColliderCast>();

            // Cast current character collider down
            ColliderCastHit hit = colliderCast.CastSelf(Vector3.down, verticalSnapDown);
            if (hit.hit && hit.distance > Epsilon)
            {
                transform.position += Vector3.down * (hit.distance - Epsilon);
            }
        }

        public void PushOutOverlapping()
        {
            float deltaTime = unityService.deltaTime;

            // Collider cast to move player
            ColliderCast colliderCast = GetComponent<ColliderCast>();
            Collider collider = GetComponent<Collider>();

            if (collider == null)
            {
                return;
            }

            foreach (ColliderCastHit overlap in colliderCast.GetOverlappingDirectional())
            {
                Physics.ComputePenetration(
                    collider, transform.position, transform.rotation,
                    overlap.collider, overlap.collider.transform.position, overlap.collider.transform.rotation,
                    out Vector3 direction, out float distance
                );
                distance += Epsilon;
                float maxPushDistance = maxPushSpeed * unityService.deltaTime;
                if (distance > maxPushDistance)
                {
                    distance = maxPushDistance;
                }
                transform.position += direction.normalized * maxPushDistance;
            }
        }

        public void CheckGrounded()
        {
            // Collider cast to move player
            ColliderCast colliderCast = GetComponent<ColliderCast>();

            ColliderCastHit hit = colliderCast.CastSelf(Vector3.down, groundCheckDistance);
            this.angle = Vector3.Angle(hit.normal, -gravity);
            this.distanceToGround = hit.distance;
            this.onGround = hit.hit;
            this.surfaceNormal = hit.normal;
        }

        public bool AttemptSnapUp(float distanceToSnap, ColliderCastHit hit, ColliderCast colliderCast, Vector3 momentum)
        {
            // If we were to snap the player up and they moved forward, would they hit something?
            Vector3 currentPosition = transform.position;
            Vector3 snapUp = distanceToSnap * Vector3.up;
            transform.position += snapUp;

            Vector3 directionAfterSnap = Vector3.ProjectOnPlane(Vector3.Project(momentum, -hit.normal), Vector3.up).normalized * momentum.magnitude;
            ColliderCastHit snapHit = colliderCast.CastSelf(directionAfterSnap.normalized, Mathf.Max(1, momentum.magnitude));

            // If they can move without instantly hitting something, then snap them up
            if (!Falling && snapHit.distance > Epsilon && (!snapHit.hit || snapHit.distance > stepUpDepth))
            {
                // Project rest of movement onto plane perpendicular to gravity
                transform.position = currentPosition;
                transform.position += distanceToSnap * Vector3.up;
                return true;
            }
            else
            {
                // Otherwise move the player back down
                transform.position = currentPosition;
                return false;
            }
        }

        public void MovePlayer(Vector3 movement)
        {
            // Save current momentum
            Vector3 momentum = movement;

            Collider selfCollider = GetComponent<Collider>();
            // Collider cast to move player
            ColliderCast colliderCast = GetComponent<ColliderCast>();
            // current number of bounces
            int bounces = 0;

            // Character ability to push objects
            CharacterPush push = GetComponent<CharacterPush>();

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

                // Apply some force to the object hit if it is moveable, Apply force on entity hit
                if (push != null && hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.isKinematic)
                {
                    push.PushObject(new KinematicCharacterControllerHit(
                        hit.collider, hit.collider.attachedRigidbody, hit.collider.gameObject,
                        hit.collider.transform, hit.pointHit, hit.normal, momentum, momentum.magnitude
                    ));
                    // If pushing something, reduce remaining force significantly
                    momentum *= pushDecay;
                }

                // Set the fraction of remaining movement (minus some small value)
                transform.position += momentum * (hit.fraction);
                // Push slightly along normal to stop from getting caught in walls
                transform.position += hit.normal * Epsilon;
                // Decrease remaining momentum by fraction of movement remaining
                momentum *= (1 - hit.fraction);

                // Plane to project rest of movement onto
                Vector3 planeNormal = hit.normal;

                // Snap character vertically up if they hit something
                //  close enough to their feet
                float distanceToFeet = hit.pointHit.y - (transform.position - selfCollider.bounds.extents).y;
                if (hit.distance > 0 && !attemptingJump && distanceToFeet < verticalSnapUp && distanceToFeet > 0)
                {
                    if(!AttemptSnapUp(verticalSnapUp, hit, colliderCast, momentum))
                    {
                        AttemptSnapUp(distanceToFeet + Epsilon * 2, hit, colliderCast, momentum);
                    }
                }
                else
                {
                    // Only apply angular change if hitting something
                    // Get angle between surface normal and remaining movement
                    float angleBetween = Vector3.Angle(hit.normal, momentum);
                    // Normalize angle between to be between 0 and 1
                    // 0 means no angle, 1 means 90 degree angle
                    angleBetween = Mathf.Min(MaxAngleShoveRadians, Mathf.Abs(angleBetween));
                    float normalizedAngle = angleBetween / MaxAngleShoveRadians;
                    // Create angle factor using 1 / (1 + normalizedAngle)
                    float angleFactor = 1.0f / (1.0f + normalizedAngle);
                    // Reduce the momentum by the remaining movement that ocurred
                    momentum *= Mathf.Pow(angleFactor, anglePower);
                    // Rotate the remaining remaining movement to be projected along the plane 
                    // of the surface hit (emulate pushing against the object)
                    float momentumLeft = momentum.magnitude;
                    momentum = Vector3.ProjectOnPlane(momentum, planeNormal);
                    momentum = momentum.normalized * momentumLeft;
                }

                // Track number of times the character has bounced
                bounces++;
            }
            // We're done, player was moved as part of loop
        }
    }
}
