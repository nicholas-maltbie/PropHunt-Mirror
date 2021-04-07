
using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    public class CameraController : NetworkBehaviour
    {
        /// <summary>
        /// Network service for managing network calls
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Mocked unity service for accessing inputs, delta time, and
        /// various other static unity inputs in a testable manner.
        /// </summary>
        public IUnityService unityService = new UnityService();

        /// <summary>
        /// Maximum pitch for rotating character camera in degrees
        /// </summary>
        public float maxPitch = 90;

        /// <summary>
        /// Minimum pitch for rotating character camera in degrees
        /// </summary>
        public float minPitch = -90;

        /// <summary>
        /// Rotation rate of camera in degrees per second per one unit of axis movement
        /// </summary>
        public float rotationRate = 180;

        /// <summary>
        /// How much the character rotated about the vertical axis this frame
        /// </summary>
        public float frameRotation;

        /// <summary>
        /// Transform holding camera position and rotation data
        /// </summary>
        public Transform cameraTransform;

        /// <summary>
        /// Should body change it's pitch (rotation about tye horizontal axis) with camera movement
        /// </summary>
        public bool pitchBody = false;

        /// <summary>
        /// Camera offset from character center
        /// </summary>        
        private Vector3 baseCameraOffset;

        /// <summary>
        /// Minimum distance (closest zoom) of player camera
        /// </summary>
        public float minCameraDistance = 0.0f;

        /// <summary>
        /// Maximum distance (farthest zoom) of player camera
        /// </summary>
        public float maxCameraDistance = 4.0f;

        /// <summary>
        /// Current distance of the camera from the player position
        /// </summary>
        public float currentDistance;

        /// <summary>
        /// Zoom distance change in units per second
        /// </summary>
        public float zoomSpeed = 1.0f;

        /// <summary>
        /// What can the camera collide with
        /// </summary>
        public LayerMask cameraRaycastMask = ~0;

        public float shadowOnlyDistance = 0.5f;

        public float ditherDistance = 1.0f;

        public void Start()
        {
            this.networkService = new NetworkService(this);
            this.baseCameraOffset = this.cameraTransform.localPosition;
            this.currentDistance = minCameraDistance;
        }

        public void Update()
        {
            if (!networkService.isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }

            float deltaTime = unityService.deltaTime;

            float yaw = transform.rotation.eulerAngles.y;
            float yawChange = 0;
            float zoomChange = 0;
            // bound pitch between -180 and 180
            float pitch = (cameraTransform.rotation.eulerAngles.x % 360 + 180) % 360 - 180;
            // Only allow rotation if player is allowed to move
            if (PlayerInputManager.playerMovementState == PlayerInputState.Allow)
            {
                yawChange = rotationRate * deltaTime * unityService.GetAxis("Mouse X");
                yaw += yawChange;
                pitch += rotationRate * deltaTime * -1 * unityService.GetAxis("Mouse Y");
                zoomChange = zoomSpeed * deltaTime * -1 * unityService.GetAxis("Mouse ScrollWheel");
            }
            // Clamp rotation of camera between minimum and maximum specified pitch
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            frameRotation = yawChange;
            // Change camera zoom by desired level
            // Bound the current distance between minimum and maximum
            this.currentDistance = Mathf.Clamp(this.currentDistance + zoomChange, this.minCameraDistance, this.maxCameraDistance);

            // Set the player's rotation to be that of the camera's yaw
            transform.rotation = Quaternion.Euler(pitchBody ? pitch : 0, yaw, 0);
            // Set pitch to be camera's rotation
            cameraTransform.localRotation = Quaternion.Euler(pitchBody ? 0 : pitch, 0, 0);

            // Set the local position of the camera to be the current rotation projected
            //   backwards by the current distance of the camera from the player
            Vector3 cameraDirection = -cameraTransform.forward * this.currentDistance;
            Vector3 cameraSource = transform.TransformDirection(this.baseCameraOffset) + transform.position;

            // Draw a line from our camera source in the camera direction. If the line hits anything that isn't us
            // Limit the distance by how far away that object is
            // If we hit something
            if (PhysicsUtils.RaycastFirstHitIgnore(gameObject, cameraSource, cameraDirection, cameraDirection.magnitude,
                this.cameraRaycastMask, QueryTriggerInteraction.Ignore, out RaycastHit hit))
            {
                // limit the movement by that hit
                cameraDirection = cameraDirection.normalized * hit.distance;
            }

            cameraTransform.position = cameraSource + cameraDirection;

            float actualDistance = cameraDirection.magnitude;

            if (actualDistance < shadowOnlyDistance)
            {
                MaterialUtils.RecursiveSetShadowCasingMode(gameObject, UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly);
            }
            else
            {
                MaterialUtils.RecursiveSetShadowCasingMode(gameObject, UnityEngine.Rendering.ShadowCastingMode.On);
            }

            if (actualDistance > shadowOnlyDistance && actualDistance < ditherDistance)
            {
                // Set opacity of character based on how close the camera is
                MaterialUtils.RecursiveSetFloatProperty(gameObject, "_Opacity", (actualDistance - minCameraDistance) / (ditherDistance - minCameraDistance));
            }
            else
            {
                // Set opacity of character based on how close the camera is
                MaterialUtils.RecursiveSetFloatProperty(gameObject, "_Opacity", 1);
            }
        }
    }
}