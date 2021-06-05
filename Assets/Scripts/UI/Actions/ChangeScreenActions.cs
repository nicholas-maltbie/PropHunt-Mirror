using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    /// <summary>
    /// Change screen actions to modify resolution and quality settings
    /// </summary>
    public class ChangeScreenActions : MonoBehaviour
    {
        private const string resolutionWidthPlayerPrefKey = "ResolutionWidth";
        private const string resolutionHeightPlayerPrefKey = "ResolutionHeight";
        private const string resolutionRefreshRatePlayerPrefKey = "RefreshRate";
        private const string fullScreenPlayerPrefKey = "FullScreen";
        private const string targetDisplayPlayerPrefKey = "TargetDisplay";
        private const string vsyncPlayerPrefKey = "vsync";

        /// <summary>
        /// Dropdown with windowed/fullScreen/borderless options
        /// </summary>
        public Dropdown windowedDropdown;

        /// <summary>
        /// Options to set screen resolution
        /// </summary>
        public Dropdown resolutionDropdown;

        /// <summary>
        /// Options to set selected monitor/display
        /// </summary>
        public Dropdown displayDropdown;

        /// <summary>
        /// Toggle to control vysnc settings
        /// </summary>
        public Toggle vsyncToggle;

        /// <summary>
        /// Has the display been setup from saved settings
        /// </summary>
        public static bool setupDisplay = false;

        /// <summary>
        /// Name of fullscreen mode
        /// </summary>
        public const string fullScreenModeName = "FullScreen";
        /// <summary>
        /// Name of windowed mode
        /// </summary>
        public const string windowedModeName = "Windowed";
        /// <summary>
        /// Name of borderless window mode
        /// </summary>
        public const string borderlessWindowModeName = "Borderless Windowed";

        /// <summary>
        /// Currently selected fullscreen mode
        /// </summary>
        public FullScreenMode currentFullScreen { get; private set; }
        /// <summary>
        /// Currently selected resolution
        /// </summary>
        public Resolution currentResolution;
        /// <summary>
        /// Set of supported screen resolutions
        /// </summary>
        private Resolution[] resolutions;
        /// <summary>
        /// Currently selected display
        /// </summary>
        public int currentDisplay { get; private set; }

        /// <summary>
        /// Text associated with dropdown window for 
        /// </summary>
        public static readonly List<string> windowedDropdownText = new List<string>(
            new string[] { fullScreenModeName, windowedModeName, borderlessWindowModeName });

        /// <summary>
        /// Get the fullscreen mode integer based on a selected fullscreen mode
        /// </summary>
        /// <param name="mode">Fullscreen mode to load</param>
        /// <returns>Index of fullscreen mode in the windowed dropdown selector</returns>
        public static int GetFullScreenModeDropdownIndex(FullScreenMode mode)
        {
            switch (mode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                    return windowedDropdownText.IndexOf(fullScreenModeName);
                case FullScreenMode.Windowed:
                    return windowedDropdownText.IndexOf(windowedModeName);
                case FullScreenMode.FullScreenWindow:
                    return windowedDropdownText.IndexOf(borderlessWindowModeName);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Get the selected fullscreen mode from a selected fullscreen name
        /// </summary>
        /// <param name="selectedMode">Name of selected mode</param>
        /// <returns>Fullscreen mode associated with this name</returns>
        public static FullScreenMode GetFullScreenMode(string selectedMode)
        {
            switch (selectedMode)
            {
                case fullScreenModeName:
                    return FullScreenMode.ExclusiveFullScreen;
                case windowedModeName:
                    return FullScreenMode.Windowed;
                case borderlessWindowModeName:
                    return FullScreenMode.FullScreenWindow;
                default:
                    return FullScreenMode.ExclusiveFullScreen;
            }
        }

        public void Awake()
        {
            // Load settings if it hasn't already been configured
            if (!setupDisplay)
            {
                LoadSettings();
            }

            // Setup dropdowns
            SetupFullScreenDropdown();
            SetupResolutionDropdown();
            SetupDisplayDropdown();
            SetupVsyncToggle();
        }

        /// <summary>
        /// Setup the dropdown for the fullscreen settings
        /// </summary>
        private void SetupFullScreenDropdown()
        {
            windowedDropdown.ClearOptions();
            windowedDropdown.AddOptions(windowedDropdownText);
            windowedDropdown.onValueChanged.AddListener(SetFullScreen);
            windowedDropdown.value = GetFullScreenModeDropdownIndex(currentFullScreen);
            windowedDropdown.RefreshShownValue();
        }

        /// <summary>
        /// Setup the vsync toggle
        /// </summary>
        private void SetupVsyncToggle()
        {
            vsyncToggle.SetIsOnWithoutNotify(QualitySettings.vSyncCount == 1);
            vsyncToggle.onValueChanged.AddListener(SetVsync);
        }

        /// <summary>
        /// Setup the dropdown for the display settings
        /// </summary>
        private void SetupDisplayDropdown()
        {
            displayDropdown.ClearOptions();
            displayDropdown.AddOptions(Enumerable.Range(1, Display.displays.Length).Select(i => $"Monitor {i}").ToList());
            displayDropdown.onValueChanged.AddListener(SetMonitor);
            displayDropdown.value = currentDisplay < Display.displays.Length ? currentDisplay : 0;
            displayDropdown.RefreshShownValue();
        }

        /// <summary>
        /// Refresh the list of resolutions from the Screen settings
        /// </summary>
        private void RefreshResolutionDropdown()
        {
            this.resolutions = Screen.resolutions.OrderBy(i => new Tuple<int, int>(-i.width, -i.height)).ToArray();
            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);
                if (Mathf.Approximately(resolutions[i].width, currentResolution.width) && Mathf.Approximately(resolutions[i].height, currentResolution.height))
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        /// <summary>
        /// Setup the resolution dropdown on initial startup
        /// </summary>
        private void SetupResolutionDropdown()
        {
            RefreshResolutionDropdown();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        /// <summary>
        /// Load saved settings from player preferences
        /// </summary>
        private void LoadSettings()
        {
            currentResolution = new Resolution();
            currentResolution.width = PlayerPrefs.GetInt(resolutionWidthPlayerPrefKey, Screen.currentResolution.width);
            currentResolution.height = PlayerPrefs.GetInt(resolutionHeightPlayerPrefKey, Screen.currentResolution.height);
            currentResolution.refreshRate = PlayerPrefs.GetInt(resolutionRefreshRatePlayerPrefKey, Screen.currentResolution.refreshRate);
            currentFullScreen = (FullScreenMode)PlayerPrefs.GetInt(fullScreenPlayerPrefKey, (int)Screen.fullScreenMode);
            currentDisplay = PlayerPrefs.GetInt(targetDisplayPlayerPrefKey, 0);
            QualitySettings.vSyncCount = PlayerPrefs.GetInt(vsyncPlayerPrefKey, QualitySettings.vSyncCount);

            UpdateDisplayInfo();
        }

        public void SetVsync(bool isChecked)
        {
            QualitySettings.vSyncCount = isChecked ? 1 : 0;
            PlayerPrefs.SetInt(vsyncPlayerPrefKey, QualitySettings.vSyncCount);
        }

        public void SetResolution(int resolutionIndex)
        {
            currentResolution = resolutions[resolutionIndex];
            UpdateDisplayInfo();
        }

        public void SetFullScreen(int fullScreenIndex)
        {
            currentFullScreen = GetFullScreenMode(windowedDropdown.options[windowedDropdown.value].text.Trim());
            UpdateDisplayInfo();
        }

        public void SetMonitor(int targetMonitor)
        {
            this.currentDisplay = targetMonitor;
            UpdateDisplayInfo();

            // Update the dropdowns for resolution based on new screen
            RefreshResolutionDropdown();
        }

        public void UpdateDisplayInfo()
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"Setting {currentFullScreen} with resolution {currentResolution.width}x{currentResolution.height} * {currentResolution.refreshRate}, monitor {currentDisplay}");
#else
            Screen.SetResolution(currentResolution.width, currentResolution.height, currentFullScreen, currentResolution.refreshRate);
            // Then set target monitor
            // This will be added in 2021.2 so will have that to look forward to in future
            // Example: https://github.com/Unity-Technologies/DesktopSamples/tree/master/MoveWindowSample
            //   Screen.MoveMainWindowTo(display, targetCoordinates);
            // Related forum thread https://forum.unity.com/threads/switch-monitor-at-runtime.501336/
            // Will update and add feature since it should be released soon

            PlayerPrefs.SetInt(resolutionWidthPlayerPrefKey, currentResolution.width);
            PlayerPrefs.SetInt(resolutionHeightPlayerPrefKey, currentResolution.height);
            PlayerPrefs.SetInt(resolutionRefreshRatePlayerPrefKey, currentResolution.refreshRate);
            PlayerPrefs.SetInt(fullScreenPlayerPrefKey, (int)currentFullScreen);
            PlayerPrefs.SetInt(targetDisplayPlayerPrefKey, currentDisplay);
#endif
        }
    }
}
