using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    public class SoundAdjustActions : MonoBehaviour
    {
        public AudioMixerSettingsGroup[] settingsGroups;

        [Serializable]
        public struct AudioMixerSettingsGroup
        {
            public AudioMixerGroup mixerGroup;
            public Slider slider;
        }

        public void Start()
        {
            // Setup sliders
            for(int i = 0; i < settingsGroups.Length; i++)
            {
                AudioMixerSettingsGroup settingsGroup = settingsGroups[i];
                AudioMixerGroup group = settingsGroup.mixerGroup;
                string soundKey = LoadAudioMixerLevels.soundVolumePrefixPlayerPrefKey + settingsGroup.mixerGroup.name;

                settingsGroup.mixerGroup.audioMixer.GetFloat($"{group.name} Volume", out float startingVolume);
                // Set the slider to match the saved value
                settingsGroup.slider.SetValueWithoutNotify(
                    LoadAudioMixerLevels.GetSliderValue(startingVolume));
                // Update saved and current value on player input
                settingsGroup.slider.onValueChanged.AddListener(value => 
                {
                    float currentVolume = LoadAudioMixerLevels.GetVolumeLevel(value);
                    group.audioMixer.SetFloat($"{group.name} Volume", currentVolume);
#if !UNITY_EDITOR
                    PlayerPrefs.SetFloat(soundKey, currentVolume);
#endif
                });
            }
        }
    }
}
