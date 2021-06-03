using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
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

        public static bool setupDisplay = false;

        public const string fullScreenModeName = "FullScreen";
        public const string windowedModeName = "Windowed";
        public const string borderlessWindowModeName = "Borderless Windowed";

        private FullScreenMode currentFullScreen;
        private Resolution currentResolution;
        private Resolution[] resolutions;
        private int currentDisplay;

        public static readonly List<string> windowedDropdownText = new List<string>(
            new string[] { fullScreenModeName, windowedModeName, borderlessWindowModeName });

        public int GetFullScreenModeDropdownIndex(FullScreenMode mode)
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

        public FullScreenMode GetFullScreenMode(string selectedMode)
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

        private void SetupFullScreenDropdown()
        {
            windowedDropdown.ClearOptions();
            windowedDropdown.AddOptions(windowedDropdownText);
            windowedDropdown.onValueChanged.AddListener(SetFullScreen);
            windowedDropdown.value = GetFullScreenModeDropdownIndex(currentFullScreen);
            windowedDropdown.RefreshShownValue();
        }

        private void SetupVsyncToggle()
        {
            vsyncToggle.SetIsOnWithoutNotify(QualitySettings.vSyncCount == 1);
            vsyncToggle.onValueChanged.AddListener(SetVsync);
        }

        private void SetupDisplayDropdown()
        {
            displayDropdown.ClearOptions();
            displayDropdown.AddOptions(Enumerable.Range(1, Display.displays.Length).Select(i => $"Monitor {i}").ToList());
            displayDropdown.onValueChanged.AddListener(SetMonitor);
            displayDropdown.value = currentDisplay < Display.displays.Length ? currentDisplay : 0;
            displayDropdown.RefreshShownValue();
        }

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

        private void SetupResolutionDropdown()
        {
            RefreshResolutionDropdown();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

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

            PlayerPrefs.SetInt(resolutionWidthPlayerPrefKey, currentResolution.width);
            PlayerPrefs.SetInt(resolutionHeightPlayerPrefKey, currentResolution.height);
            PlayerPrefs.SetInt(resolutionRefreshRatePlayerPrefKey, currentResolution.refreshRate);
            PlayerPrefs.SetInt(fullScreenPlayerPrefKey, (int)currentFullScreen);
            PlayerPrefs.SetInt(targetDisplayPlayerPrefKey, currentDisplay);
#endif
        }
    }
}
