using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    public class ChangeScreenActions : MonoBehaviour
    {

        /// <summary>
        /// Dropdown with windowed/fullscreen/borderless options
        /// </summary>
        public Dropdown windowedDropdown;

        /// <summary>
        /// Options to set screen resolution
        /// </summary>
        public Dropdown resolutionDropdown;

        /// <summary>
        /// Display being rendered
        /// </summary>
        public static Display currentDisplay = Display.main;

        public const string resolutionParser = @"(\d+)[^\d](\d+)(\s*\*\s*(\d+)\s*)?";

        public const string fullscreenModeName = "Fullscreen";
        public const string windowedModeName = "Windowed";
        public const string borderlessWindowModeName = "Borderless Windowed";

        public static readonly string[] windowedDropdownText = { fullscreenModeName, windowedModeName, borderlessWindowModeName };
        public static readonly string[] resolutionDropdownText = {
            "1920×1080 * 60",
            "1920×1080 * 30",
            "1600×900",
            "1366×768",
            "1280×720",
            "1024×576"
        };

        public static List<Dropdown.OptionData> MakeDropdownList(string[] optionsText) => new List<string>(optionsText).Select(
            text => new Dropdown.OptionData(text)).ToList<Dropdown.OptionData>();

        public FullScreenMode GetFullScreenMode(string selectedMode)
        {
            switch (selectedMode)
            {
                case fullscreenModeName:
                    return FullScreenMode.ExclusiveFullScreen;
                case windowedModeName:
                    return FullScreenMode.Windowed;
                case borderlessWindowModeName:
                    return FullScreenMode.FullScreenWindow;
                default:
                    return FullScreenMode.ExclusiveFullScreen;
            }
        }

        public void Start()
        {
            windowedDropdown.ClearOptions();
            windowedDropdown.AddOptions(MakeDropdownList(windowedDropdownText));
            windowedDropdown.onValueChanged.AddListener(delegate
            {
                UpdateDisplayInfo();
            });

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(MakeDropdownList(resolutionDropdownText));
            resolutionDropdown.onValueChanged.AddListener(delegate
            {
                UpdateDisplayInfo();
            });
        }

        public void UpdateDisplayInfo()
        {
            string selectedResolution = resolutionDropdown.options[resolutionDropdown.value].text.Trim();
            FullScreenMode mode = GetFullScreenMode(windowedDropdown.options[windowedDropdown.value].text.Trim());

            Match resolutionParse = Regex.Match(selectedResolution, resolutionParser);

            int resX = int.Parse(resolutionParse.Groups[1].Value);
            int resY = int.Parse(resolutionParse.Groups[2].Value);
            if (int.TryParse(resolutionParse.Groups[4].Value, out int update))
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log($"Setting {mode} with resolution {resX}x{resY} and update {update}");
#else
                Screen.SetResolution(resX, resY, mode, update);
#endif
            }
            else
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log($"Setting {mode} with resolution {resX}x{resY}");
#else
                Screen.SetResolution(resX, resY, mode);
#endif
            }
        }
    }
}