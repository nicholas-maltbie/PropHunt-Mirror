using UnityEngine;
using Mirror;

namespace PropHunt.Character
{
    /// <summary>
    /// Script to move main camera to follow the local player
    /// </summary>
    public class CameraFollow : NetworkBehaviour
    {
        /// <summary>
        /// Position to move camera to
        /// </summary>
        public Transform cameraPosition;

        void Update()
        {
            if (!isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }

            // Do nothing if there is no main camera
            if (Camera.main == null)
            {
                return;
            }

            // Set main camera's parent to be this and set it's relative position and rotation to be zero
            GameObject mainCamera = Camera.main.gameObject;
            mainCamera.transform.rotation = cameraPosition.rotation;
            mainCamera.transform.position = cameraPosition.position;
        }
    }
}