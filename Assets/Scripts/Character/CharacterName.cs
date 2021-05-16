using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// Character name for the local player
    /// </summary>
    public static class CharacterNameManagement
    {
        /// <summary>
        /// Regex to check if a name is valid
        /// </summary>
        public static Regex validNamePattern = new Regex("^[A-Za-z0-9][A-Za-z0-9\\s]{0,14}[A-Za-z0-9]$");

        /// <summary>
        /// Regex to filter out invalid parts of name
        /// </summary>
        public static Regex filterPattern = new Regex("^\\s+|[^A-Za-z0-9\\s]|\\s+$");

        /// <summary>
        /// Regex pattern to identify duplicate whitespaces
        /// </summary>
        public static Regex duplicateWhitespace = new Regex("\\s{2,}");

        /// <summary>
        /// name of the current local player
        /// </summary>
        public static string playerName = "Player";

        /// <summary>
        /// Get lookup of all names by player connection id
        /// </summary>
        public static SortedDictionary<int, string> GetPlayerNames()
        {
            SortedDictionary<int, string> playerNames = new SortedDictionary<int, string>();
            foreach(CharacterName name in GameObject.FindObjectsOfType<CharacterName>())
            {
                playerNames[name.playerId] = name.characterName;
            }
            return playerNames;
        }

        /// <summary>
        /// Filters a given name using regex to remote trailing and leading whitespace, duplicate whitespace,
        /// as well as any other invalid (non alpha numeric character) from the string.
        /// </summary>
        /// <param name="name">Name to filter</param>
        /// <returns>Filtered name using filterPattern and duplicateWhitespace pattern</returns>
        public static string GetFilteredName(string name)
        {
            return duplicateWhitespace.Replace(filterPattern.Replace(name, ""), name);
        }

        /// <summary>
        /// Verifying player name if it is valid, should only cotain letters A-Z,a-z and numbers 0-9
        /// </summary>
        /// <param name="name">Character name</param>
        /// <returns>If a given name is valid</returns>
        public static bool VerifyName(string name)
        {
            return validNamePattern.Match(name).Success;
        }
    }

    /// <summary>
    /// Component to hold a given character's name
    /// </summary>
    public class CharacterName : NetworkBehaviour
    {
        /// <summary>
        /// Id associated with this player
        /// </summary>
        [SyncVar]
        public int playerId;

        /// <summary>
        /// name associated with this player
        /// </summary>
        [SyncVar]
        public string characterName = "Player";

        /// <summary>
        /// Network service for managing tests
        /// </summary>
        public INetworkService networkService;

        /// <summary>
        /// Update a player name from a client with authority via command
        /// </summary>
        /// <param name="newName">New name to assign a player</param>
        [Command]
        public void CmdUpdatePlayerName(string newName)
        {
            characterName = newName;
        }

        public void Awake()
        {
            networkService = new NetworkService(this);
        }

        public void Start()
        {
            if (networkService.isServer)
            {
                playerId = connectionToClient.connectionId;
            }
            // Synchronize state to server if local player
            if (networkService.isLocalPlayer)
            {
                CmdUpdatePlayerName(CharacterNameManagement.playerName);
            }
        }
    }
}