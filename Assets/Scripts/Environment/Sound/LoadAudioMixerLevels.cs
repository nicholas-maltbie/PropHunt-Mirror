using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    public class LoadAudioMixerLevels : MonoBehaviour
    {        
        public const string soundVolumePrefixPlayerPrefKey = "SoundVolume_";
        public const float minVolume = -20;
        public const float maxVolume = 10;

        public const float mutedVolume = -80f;

        public AudioMixerGroup[] mixerGroups;

        public static float GetSliderValue(float volumeLevel)
        {
            if (volumeLevel == mutedVolume || volumeLevel < minVolume)
            {
                return 0.0f;
            }
            if (volumeLevel > maxVolume)
            {
                return 1.0f;
            }
            return Mathf.Pow((volumeLevel - minVolume) / (maxVolume - minVolume), 2.0f);
        }

        public static float GetVolumeLevel(float sliderPosition)
        {
            if (sliderPosition == 0.0f)
            {
                return mutedVolume;
            }
            return Mathf.Pow(sliderPosition, 0.5f) * (maxVolume - minVolume) + minVolume;
        }

        public void Start()
        {
            // Setup sliders
            foreach(AudioMixerGroup mixerGroup in mixerGroups)
            {
                string soundKey = soundVolumePrefixPlayerPrefKey + mixerGroup.name;
                mixerGroup.audioMixer.SetFloat($"{mixerGroup.name} Volume", PlayerPrefs.GetFloat(soundKey, 0));
            }
        }
    }
}
