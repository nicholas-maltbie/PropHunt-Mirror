using System.Collections.Generic;
using kcp2k;
using Mirror;
// using Mirror.FizzySteam;
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
        /// Public instance for controlling transport
        /// </summary>
        public static ToggleTransport Instance;

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
        // public FizzySteamworks fizzySteamworksSettings;

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
            // Set main instance
            ToggleTransport.Instance = this;

            // setup a lookup table to link the currently available multiplayer modes
            //  to their enum type in code
            this.transportSettingsLookup = new Dictionary<MultiplayerMode, Transport>();
            // this.transportSettingsLookup[MultiplayerMode.FizzySteamworks] = this.fizzySteamworksSettings;
            this.transportSettingsLookup[MultiplayerMode.KcpTransport] = this.kcpTransportSettings;

            // Un-parent fizzy steamworks because it leads to warning if not
            // this.fizzySteamworksSettings.transform.parent = null;

            // Setup initial mode
            SetMultiplayerMode(this.currentMode, forceUpdate: true);
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
            Transport.activeTransport = currentTransport;
            currentTransport.gameObject.SetActive(true);

            // Attach this game mode to our network manager
            Transport.activeTransport = currentTransport;
        }
    }
}
