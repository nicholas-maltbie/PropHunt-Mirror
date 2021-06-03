using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    public class ChangeQualityActions : MonoBehaviour
    {
        private const string qualityLevelPlayerPrefKey = "qualityLevel";

        public Dropdown qualityDropdown;

        public void Awake()
        {
            // Load saved settings
            int currentLevel = PlayerPrefs.GetInt(qualityLevelPlayerPrefKey, QualitySettings.GetQualityLevel());
            QualitySettings.SetQualityLevel(currentLevel);
            UnityEngine.Debug.Log($"Setting quality level to {QualitySettings.names[currentLevel]}");

            // Setup dropdown information
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(QualitySettings.names.ToList());
            qualityDropdown.SetValueWithoutNotify(currentLevel);
            qualityDropdown.onValueChanged.AddListener(OnQualityLevelChange);
        }

        public void OnQualityLevelChange(int level)
        {
            QualitySettings.SetQualityLevel(level);
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"Setting quality level to {QualitySettings.names[level]}");
#else
            PlayerPrefs.SetInt(qualityLevelPlayerPrefKey, level);
#endif
        }
    }
}