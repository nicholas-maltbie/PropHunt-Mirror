using PropHunt.Game.Flow;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI
{
    /// <summary>
    /// Game timer to display the game phase timer
    /// </summary>
    public class GameTimerDisplay : MonoBehaviour, IScreenComponent
    {
        /// <summary>
        /// Game phase timer tag for finding game phase timers (if they exist)
        /// </summary>
        public const string gamePhaseTimerTag = "GamePhaseTimer";

        /// <summary>
        /// Text object to display remaining timer
        /// </summary>
        public Text timerText;

        /// <summary>
        /// Is the timer being currently displayed
        /// </summary>
        private bool isDisplayed = false;

        public void OnScreenLoaded()
        {
            isDisplayed = true;
        }

        public void OnScreenUnloaded()
        {
            isDisplayed = false;
        }

        /// <summary>
        /// Get a string description of the remaining in a time span format of mm:ss
        /// of an object with the game phase timer tag
        /// </summary>
        /// <returns>String description of remaining time in format of mm:ss</returns>
        private static string GetTimerText()
        {
            GameObject go = GameObject.FindGameObjectWithTag(gamePhaseTimerTag);
            if (go == null)
            {
                return "";
            }
            GameTimer gameTimer = go.GetComponent<GameTimer>();
            if (gameTimer == null)
            {
                return "";
            }

            return gameTimer.GetTime();
        }

        public void Update()
        {
            if (isDisplayed)
            {
                timerText.text = GetTimerText();
            }
        }
    }
}