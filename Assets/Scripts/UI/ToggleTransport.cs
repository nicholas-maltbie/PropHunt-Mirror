
using System.Collections.Generic;
using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.UI
{
    /// <summary>
    /// Mode of multiplayer connection, right now, we will just have a manual toggle,
    /// but in the future we could switch this to many different types of toggles
    /// such as game center, our servers, steam servers, etc...
    /// </summary>
    public enum MultiplayerMode
    {
        None = 0,
        KcpTransport = 1,
        FizzySteamworks = 2
    }

    /// <summary>
    /// Class to toggle various types of transport for a network manager
    /// </summary>
    public class ToggleTransport : MonoBehaviour
    {
        /// <summary>
        /// Current mode we have selected
        /// </summary>
        public MultiplayerMode currentMode = MultiplayerMode.KcpTransport;

        /// <summary>
        /// Lookup from transport type to transport settings
        /// </summary>
        public Dictionary<MultiplayerMode, Transport> transportSettingsLookup = new Dictionary<MultiplayerMode, Transport>();

        /// <summary>
        /// Network service to check if connected to the server
        /// </summary>
        public INetworkService networkService = new NetworkService(null);

        public void Start()
        {
            // Setup transports
            FindTransports();

            // Setup initial mode
            SetMultiplayerMode(this.currentMode, forceUpdate: true);
        }

        public void Update()
        {
            FindTransports();
        }

        /// <summary>
        /// Find the transports for this project if they are not setup
        /// </summary>
        public void FindTransports()
        {
            // setup a lookup table to link the currently available multiplayer modes
            //  to their enum type in code
            if (!this.transportSettingsLookup.ContainsKey(MultiplayerMode.FizzySteamworks))
            {
                this.transportSettingsLookup[MultiplayerMode.FizzySteamworks] = GameObject.FindObjectOfType<FizzySteamworks>();
            }
            if (!this.transportSettingsLookup.ContainsKey(MultiplayerMode.KcpTransport))
            {
                this.transportSettingsLookup[MultiplayerMode.KcpTransport] = GameObject.FindObjectOfType<KcpTransport>();
            }
        }

        /// <summary>
        /// Set multiplayer mode
        /// </summary>
        /// <param name="mode">String name of a multiplayer game mode</param>
        public void SetMultiplayerMode(string mode)
        {
            this.SetMultiplayerMode((MultiplayerMode)System.Enum.Parse(typeof(MultiplayerMode), mode));
        }

        /// <summary>
        /// Set multiplayer mode via enum
        /// </summary>
        /// <param name="mode">Enum of mode to change to</param>
        /// <param name="forceUpdate">Force update even if mode has not changed</param>
        public void SetMultiplayerMode(MultiplayerMode mode, bool forceUpdate = false)
        {
            if (!forceUpdate && mode == this.currentMode)
            {
                // Already in this mode, do nothing
                return;
            }
            // Disable previous mode
            Transport previousTransport = transportSettingsLookup[this.currentMode];
            previousTransport.gameObject.SetActive(false);

            // Enable new mode
            this.currentMode = mode;
            Transport currentTransport = transportSettingsLookup[this.currentMode];
            currentTransport.gameObject.SetActive(true);

            // Attach this game mode to our network manager
            Transport.activeTransport = currentTransport;
        }
    }
}
