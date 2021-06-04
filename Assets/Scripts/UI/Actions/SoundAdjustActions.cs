using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    public class SoundAdjustActions : MonoBehaviour
    {
        public static bool setupAudio;
        
        private const string soundVolumePrefixPlayerPrefKey = "SoundVolume_";

        float minVolume = -80;
        float maxVolume = 20;

        public AudioMixerSettingsGroup[] settingsGroups;

        [Serializable]
        public struct AudioMixerSettingsGroup
        {
            public AudioMixerGroup mixerGroup;
            public Slider slider;
            public float currentVolume;
            public bool isMuted;
        }

        public float GetSliderValue(float volumeLevel)
        {
            return (volumeLevel - minVolume) / (maxVolume - minVolume);
        }

        public float GetVolumeLevel(float sliderPosition)
        {
            return sliderPosition * (maxVolume - minVolume) + minVolume;
        }

        public void Start()
        {
            // Setup sliders
            for(int i = 0; i < settingsGroups.Length; i++)
            {
                AudioMixerSettingsGroup settingsGroup = settingsGroups[i];
                AudioMixerGroup group = settingsGroup.mixerGroup;
                string soundKey = soundVolumePrefixPlayerPrefKey + settingsGroup.mixerGroup.name;

                // Load audio settings from disk
                settingsGroup.currentVolume = PlayerPrefs.GetFloat(soundKey, 0);
                group.audioMixer.SetFloat($"{group.name} Volume", settingsGroup.currentVolume);
                // Set the slider to match the saved value
                settingsGroup.slider.SetValueWithoutNotify(GetSliderValue(settingsGroup.currentVolume));
                // Update saved and current value on player input
                settingsGroup.slider.onValueChanged.AddListener(value => 
                {
                    settingsGroup.currentVolume = GetVolumeLevel(value);
                    group.audioMixer.SetFloat($"{group.name} Volume", settingsGroup.currentVolume);
#if !UNITY_EDITOR
                    PlayerPrefs.SetFloat(soundKey, settingsGroup.currentVolume);
#endif
                });
            }
        }
    }
}
