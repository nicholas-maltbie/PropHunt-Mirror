using System;
using System.Collections.Generic;
using UnityEngine;

namespace PropHunt.Environment.Sound
{
    [CreateAssetMenu(fileName = "SFXLibrary", menuName = "ScriptableObjects/SpawnSFXLibraryScriptableObject", order = 1)]
    public class SoundEffectLibrary : ScriptableObject
    {
        [SerializeField]
        public LabeledSFX[] sounds;

        private bool initialized = false;

        private Dictionary<SoundMaterial, List<LabeledSFX>> soundMaterialLookup =
            new Dictionary<SoundMaterial, List<LabeledSFX>>();

        private Dictionary<SoundType, List<LabeledSFX>> soundTypeLookup =
            new Dictionary<SoundType, List<LabeledSFX>>();

        private Dictionary<Tuple<SoundMaterial, SoundType>, List<LabeledSFX>> soundMaterialTypeLookup =
            new Dictionary<Tuple<SoundMaterial, SoundType>, List<LabeledSFX>>();

        private Dictionary<string, LabeledSFX> soundIdLookup =
            new Dictionary<string, LabeledSFX>();

        public void ClearLookups()
        {
            initialized = false;
            soundMaterialLookup.Clear();
            soundTypeLookup.Clear();
            soundMaterialTypeLookup.Clear();
            soundIdLookup.Clear();
        }

        public void VerifyLookups()
        {
            if (!initialized)
            {
                ClearLookups();
                SetupLookups();
            }
        }

        public void SetupLookups()
        {
            foreach (LabeledSFX labeled in sounds)
            {
                Tuple<SoundMaterial, SoundType> tupleKey = new Tuple<SoundMaterial, SoundType>(labeled.soundMaterial, labeled.soundType);
                if (!soundMaterialLookup.ContainsKey(labeled.soundMaterial))
                {
                    soundMaterialLookup[labeled.soundMaterial] = new List<LabeledSFX>();
                }
                if (!soundTypeLookup.ContainsKey(labeled.soundType))
                {
                    soundTypeLookup[labeled.soundType] = new List<LabeledSFX>();
                }
                if (!soundMaterialTypeLookup.ContainsKey(tupleKey))
                {
                    soundMaterialTypeLookup[tupleKey] = new List<LabeledSFX>();
                }
                soundMaterialLookup[labeled.soundMaterial].Add(labeled);
                soundTypeLookup[labeled.soundType].Add(labeled);
                soundMaterialTypeLookup[tupleKey].Add(labeled);
                soundIdLookup[labeled.soundID] = labeled;
            }
            initialized = true;
        }

        public LabeledSFX GetSFXClipBySoundMaterial(SoundMaterial soundMaterial)
        {
            List<LabeledSFX> sounds = soundMaterialLookup[soundMaterial];
            return sounds[(int)UnityEngine.Random.Range(0, sounds.Count)];
        }
        public LabeledSFX GetSFXClipBySoundType(SoundType soundType)
        {
            List<LabeledSFX> sounds = soundTypeLookup[soundType];
            return sounds[(int)UnityEngine.Random.Range(0, sounds.Count)];
        }

        public LabeledSFX GetSFXClipBySoundMaterialAndType(SoundMaterial soundMaterial, SoundType soundType)
        {
            List<LabeledSFX> sounds = soundMaterialTypeLookup[new Tuple<SoundMaterial, SoundType>(soundMaterial, soundType)];
            return sounds[(int)UnityEngine.Random.Range(0, sounds.Count)];
        }

        public LabeledSFX GetSFXClipById(string soundId)
        {
            return soundIdLookup[soundId];
        }
    }

    public enum SoundMaterial
    {
        Glass,
        Wood,
        Metal,
        Concrete,
        Misc
    }

    public enum SoundType
    {
        Hit,
        Break,
        Scrape,
        Roll,
        Footstep,
        Misc
    }

    [System.Serializable]
    public class LabeledSFX
    {
        public SoundMaterial soundMaterial;
        public SoundType soundType;
        public AudioClip audioClip;
        public string soundID;
    }
}
