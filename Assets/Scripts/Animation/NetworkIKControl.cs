
using System.Collections.Generic;
using Mirror;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Animation
{
    /// <summary>
    /// Manage IK For humanoid avatar over network
    /// </summary>
    public class NetworkIKControl : NetworkBehaviour
    {
        /// <summary>
        /// Target for player looking
        /// </summary>
        public Transform ikLookTarget;
        
        /// <summary>
        /// Are the Inverse Kinematics controls enabled for this character
        /// </summary>
        public bool ikActive = true;

        /// <summary>
        /// Current look state of the player
        /// </summary>
        [SyncVar(hook = nameof(OnLookStateChange))]
        public bool lookState;

        /// <summary>
        /// Current look weight for the player
        /// </summary>
        [SyncVar(hook = nameof(OnLookWeightChange))]
        public float lookWeight;

        /// <summary>
        /// Network service for managing player interactions
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Controller for managing current IK targets for animator (abstract the commands)
        /// </summary>
        public IKControl controller;

        public void OnLookStateChange(bool _, bool newState) => this.controller.lookObj = newState ? ikLookTarget : null;
        public void OnLookWeightChange(float _, float newWeight) => this.controller.lookWeight = newWeight;

        public void Awake()
        {
            networkService = new NetworkService(this);

            this.ikLookTarget = new GameObject().transform;
            this.ikLookTarget.position = transform.position;
            this.ikLookTarget.rotation = transform.rotation;
            this.ikLookTarget.parent = transform;
            NetworkTransformChild childTransform = gameObject.AddComponent<NetworkTransformChild>();
            childTransform.target = this.ikLookTarget;
            childTransform.clientAuthority = true;
        }

        public void SetLookWeight(float newLookWeight)
        {
            if (!networkService.isServer)
            {
                CmdSetLookWeight(newLookWeight);
            }

            if (networkService.isLocalPlayer || networkService.isServer)
            {
                OnLookWeightChange(this.lookWeight, newLookWeight);
            }
        }

        public void SetLookState(bool newLookState)
        {
            if (!networkService.isServer)
            {
                CmdSetLookState(newLookState);
            }

            if (networkService.isLocalPlayer || networkService.isServer)
            {
                OnLookStateChange(this.lookState, newLookState);
            }
        }

        public void Update()
        {
            if ((controller.lookObj != null) != this.lookState)
            {
                OnLookStateChange(this.lookState, this.lookState);
            }
            if (controller.lookWeight != this.lookWeight)
            {
                OnLookWeightChange(this.lookWeight, this.lookWeight);
            }
        }

        [Command]
        public void CmdSetLookWeight(float newLookWeight)
        {
            SetLookWeight(newLookWeight);
        }

        [Command]
        public void CmdSetLookState(bool newLookState)
        {
            SetLookState(newLookState);
        }
    }
}