using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public const string fullscreenModeName = "Fullscreen";
        public const string windowedModeName = "Windowed";
        public const string borderlessWindowModeName = "Borderless Windowed";

        private FullScreenMode currentFullscreen;
        private Resolution currentResolution;
        private Resolution[] resolutions;

        public static readonly List<string> windowedDropdownText = new List<string>(
            new string[] { fullscreenModeName, windowedModeName, borderlessWindowModeName });

        public int GetFullScreenModeDropdownIndex(FullScreenMode mode)
        {
            switch (mode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                    return windowedDropdownText.IndexOf(fullscreenModeName);
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
            this.resolutions = Screen.resolutions.OrderBy(i => new Tuple<int, int>(-i.width, -i.height)).ToArray();
            LoadSettings();

            SetupFullscreenDropdown();
            SetupResolutionDropdown();
        }

        private void SetupFullscreenDropdown()
        {
            windowedDropdown.ClearOptions();
            windowedDropdown.AddOptions(windowedDropdownText);
            windowedDropdown.onValueChanged.AddListener(SetFullscreen);
            windowedDropdown.value = GetFullScreenModeDropdownIndex(currentFullscreen);
            windowedDropdown.RefreshShownValue();
        }

        private void SetupResolutionDropdown()
        {
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
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        private void LoadSettings()
        {
            currentResolution = new Resolution();
            currentResolution.width = PlayerPrefs.GetInt(resolutionWidthPlayerPrefKey, Screen.currentResolution.width);
            currentResolution.height = PlayerPrefs.GetInt(resolutionHeightPlayerPrefKey, Screen.currentResolution.height);
            currentResolution.refreshRate = PlayerPrefs.GetInt(resolutionRefreshRatePlayerPrefKey, Screen.currentResolution.refreshRate);
            currentFullscreen = (FullScreenMode)PlayerPrefs.GetInt(fullScreenPlayerPrefKey, (int)Screen.fullScreenMode);

            UpdateDisplayInfo();
        }

        public void SetResolution(int resolutionIndex)
        {
            currentResolution = resolutions[resolutionIndex];
            UpdateDisplayInfo();
        }

        public void SetFullscreen(int fullscreenIndex)
        {
            currentFullscreen = GetFullScreenMode(windowedDropdown.options[windowedDropdown.value].text.Trim());
            UpdateDisplayInfo();
        }

        public void UpdateDisplayInfo()
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"Setting {currentFullscreen} with resolution {currentResolution.width}x{currentResolution.height} * {currentResolution.refreshRate}");
#else
            Screen.SetResolution(currentResolution.width, currentResolution.height, currentFullscreen, currentResolution.refreshRate);
            PlayerPrefs.SetInt(resolutionWidthPlayerPrefKey, currentResolution.width);
            PlayerPrefs.SetInt(resolutionHeightPlayerPrefKey, currentResolution.height);
            PlayerPrefs.SetInt(resolutionRefreshRatePlayerPrefKey, currentResolution.refreshRate);
            PlayerPrefs.SetInt(fullScreenPlayerPrefKey, (int)currentFullscreen);
#endif
        }
    }
}