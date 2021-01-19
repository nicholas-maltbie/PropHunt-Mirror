
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
        KcpTransport        = 0,
        FizzySteamworks     = 1
    }

    /// <summary>
    /// Class to toggle various types of transport for a network manager
    /// </summary>
    [RequireComponent(typeof(NetworkManager))]
    public class ToggleTransport : NetworkBehaviour
    {
        /// <summary>
        /// Current mode we have selected
        /// </summary>
        public MultiplayerMode currentMode = MultiplayerMode.KcpTransport;

        /// <summary>
        /// Settings selected for KcpTransport
        /// </summary>
        public KcpTransport kcpTransportSettings;

        /// <summary>
        /// Settings selected for FizzySteamworks
        /// </summary>
        public FizzySteamworks fizzySteamworksSettings;

        /// <summary>
        /// Lookup from transport type to transport settings
        /// </summary>
        public Dictionary<MultiplayerMode, Transport> transportSettingsLookup;

        /// <summary>
        /// Network service to check if connected to the server
        /// </summary>
        public INetworkService networkService = new NetworkService(null);

        public void Start()
        {
            // setup a lookup table to link the currently available multiplayer modes
            //  to their enum type in code
            this.transportSettingsLookup = new Dictionary<MultiplayerMode, Transport>();
            this.transportSettingsLookup[MultiplayerMode.FizzySteamworks] = this.fizzySteamworksSettings;
            this.transportSettingsLookup[MultiplayerMode.KcpTransport] = this.kcpTransportSettings;

            // Setup initial mode
            SetMultiplayerMode(this.currentMode, forceUpdate: true);
        }

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

        public void OnGUI()
        {
            // Disable this GUI when in game, or connecting
            if (this.networkService.activeNetworkClient)
            {
                return;
            }

            // Display buttons to set multiplayer mode
            if (GUI.Button(new Rect(10, 115, 200, 20), "Steam Networking"))
            {
                this.SetMultiplayerMode(MultiplayerMode.FizzySteamworks);
            }
            if (GUI.Button(new Rect(10, 140, 200, 20), "Kcp Networking"))
            {
                this.SetMultiplayerMode(MultiplayerMode.KcpTransport);
            }
        }
    }
}
