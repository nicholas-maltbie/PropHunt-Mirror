using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///*make all triple slashes with summaries*
namespace PropHunt.Character
{
    public class FocusDetection : NetworkBehaviour
    {
        /// <summary>
        /// Network service for managing network calls
        /// </summary>
        public INetworkService networkService;

        // How far can the player look
        public float viewDistance = 5.0f;
        // What direction is the player looking
        public Transform cameraTransform;
        // What the player is looking at
        public GameObject focus;

        // Start is called before the first frame update
        public void Start()
        {
            this.networkService = new NetworkService(this);
        }

        // Update is called once per frame
        public void Update()
        {
            if (!this.networkService.isLocalPlayer)
            {
                // exit from update if this is not the local player
                return;
            }
            // Only upadte if IsLocalPlayer is true
            // Do a sphere cast form where the player is looking this.viewDistance units forward in the
            //   direction they are looking
            // If they hit a game object
            //    if it is the same as current focus, do nothing
            //    if it is different, send a message "look at" to new object, update focus, send a message to old object "look away"
            // If they did not hit anything
            //    if focus was something previous frame, send "look away"
        }
    }
}