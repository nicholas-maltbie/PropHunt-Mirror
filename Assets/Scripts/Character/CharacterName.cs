using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mirror;
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
        public static Regex validNamePattern = new Regex("[a-zA-Z0-9]{2-16}");

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
        /// Verifying player name if it is valid, should only cotain letters A-Z,a-z and numbers 0-9
        /// </summary>
        /// <param name="name">Character name</param>
        /// <returns>If a given name is valid</returns>
        public static bool VerifyName(string name)
        {
            return name.Length > 0 && validNamePattern.Match(name).Success;
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
        /// Update a player name from a client with authority via command
        /// </summary>
        /// <param name="newName">New name to assign a player</param>
        [Command]
        public void CmdUpdatePlayerName(string newName)
        {
            characterName = newName;
        }

        public void Start()
        {
            if (isServer)
            {
                playerId = connectionToClient.connectionId;
            }
            // Synchronize state to server if local player
            if (isLocalPlayer)
            {
                CmdUpdatePlayerName(CharacterNameManagement.playerName);
            }
        }
    }
}