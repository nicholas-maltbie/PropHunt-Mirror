using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    public class ChangeScreenActions : MonoBehaviour
    {
        /// <summary>
        /// Dropdown events should be organized as
        ///  0 - Fullscreen
        ///  1 - Windowed
        ///  2 - Borderless Fullscreen
        /// </summary>
        public Dropdown windowedDropdown;

        public Dropdown resolutionDropdown;

        public const string resolutionParser = @"(\d+)[^\d](\d+)(\s*\*\s*(\d+)\s*)?";

        public void SetScreenResolution()
        {
            string selectedResolution = resolutionDropdown.options[resolutionDropdown.value].text.Trim();

            Match match = Regex.Match(selectedResolution, resolutionParser);

            int resX = int.Parse(match.Groups[1].Value);
            int resY = int.Parse(match.Groups[2].Value);
            if (int.TryParse(match.Groups[4].Value, out int update))
            {
                Screen.SetResolution(resX, resY, Screen.fullScreenMode, update);
                UnityEngine.Debug.Log($"Setting screen resolution to {resX}x{resY} with update {update}");
            }
            else
            {
                Screen.SetResolution(resX, resY, Screen.fullScreenMode);
                UnityEngine.Debug.Log($"Setting screen resolution to {resX}x{resY}");
            }
        }

        public void SetFullScreenMode()
        {
            FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;
            string selectedMode = windowedDropdown.options[windowedDropdown.value].text.Trim();
            if (selectedMode == "Fullscreen")
            {
                mode = FullScreenMode.ExclusiveFullScreen;
            }
            else if (selectedMode == "Windowed")
            {
                mode = FullScreenMode.Windowed;
            }
            else if (selectedMode == "Borderless Fullscreen")
            {
                mode = FullScreenMode.FullScreenWindow;
            }
#if UNITY_EDITOR
            UnityEngine.Debug.Log("Setting screen mode " + mode.ToString());
#else
            Screen.fullScreenMode = mode;
#endif
        }
    }
}