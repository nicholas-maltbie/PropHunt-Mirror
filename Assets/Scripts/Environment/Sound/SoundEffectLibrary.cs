using UnityEngine;

namespace PropHunt.Environment.Sound
{
    public class SoundEffectLibrary : ScriptableObject
    {
        [SerializeField]
        public LabeledSound sounds;
    }

    public enum SoundMaterial
    {
        Glass,
        Wood,
        Metal,
        Concrete
    }

    public enum SoundType
    {
        Hit,
        Break,
        Scrape
    }

    [System.Serializable]
    public class LabeledSound
    {
        public SoundMaterial soundMaterial;
        public SoundType soundType;
        public AudioClip audioClip;
    }
}
