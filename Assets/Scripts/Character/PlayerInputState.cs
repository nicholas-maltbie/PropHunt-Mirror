using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// Current player input state, is the player allowed to give input or not
    /// </summary>
    public enum PlayerInputState
    {
        Allow,
        Deny
    }

    /// <summary>
    /// Manager for player input information as a static class
    /// </summary>
    public static class PlayerInputManager
    {
        /// <summary>
        /// Current state of the player movement action
        /// </summary>
        public static PlayerInputState playerMovementState = PlayerInputState.Allow;

        /// <summary>
        /// Minimum mouse sensitivity value multiplier
        /// </summary>
        public static readonly float minimumMouseSensitivity = 0.1f;
        /// <summary>
        /// Maximum mouse sensitivity value multiplier
        /// </summary>
        public static readonly float maximumMouseSensitivity = 5.0f;

        /// <summary>
        /// Mouse sensitivity multiplier (should be between 0.1 and 5.0 hopefully)
        /// </summary>
        public static float mouseSensitivity = 1.0f;
    }
}
