using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment.Sound
{
    public struct SoundEffectEvent : NetworkMessage
    {
        public string sfxId;
        public Vector3 point;
        public float pitchValue;
        public float volume;
    }

    public class SoundEffectManager : MonoBehaviour
    {
        public static SoundEffectManager Instance;

        public INetworkService networkService;

        public SoundEffectLibrary soundEffectLibrary;

        public GameObject soundEffectPrefab;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            this.soundEffectLibrary.SetupLookups();
        }

        [Server]
        public static void CreateNetworkedSoundEffectAtPoint(
            Vector3 point, SoundMaterial material, SoundType type, float pitch=1.0f, float volume=1.0f)
        {
            if (SoundEffectManager.Instance == null || !NetworkServer.active)
            {
                return;
            }
            UnityEngine.Debug.Log(pitch);

            NetworkServer.SendToAll<SoundEffectEvent>(new SoundEffectEvent
            {
                sfxId = SoundEffectManager.Instance.soundEffectLibrary.GetSFXClipBySoundMaterialAndType(material, type).soundID,
                point = point,
                pitchValue = pitch,
                volume = volume
            });
        }

        public static void CreateSoundEffectAtPoint(SoundEffectEvent sfxEvent)
        {
            CreateSoundEffectAtPoint(sfxEvent.point, 
                SoundEffectManager.Instance.soundEffectLibrary.GetSFXClipById(sfxEvent.sfxId).audioClip,
                pitchValue:sfxEvent.pitchValue);
        }

        public static GameObject CreateSoundEffectAtPoint(Vector3 point, SoundMaterial soundMaterial, SoundType soundType)
        {
            return CreateSoundEffectAtPoint(point,
                SoundEffectManager.Instance.soundEffectLibrary.GetSFXClipBySoundMaterialAndType(soundMaterial, soundType).audioClip);
        }

        public static GameObject CreateSoundEffectAtPoint(Vector3 point, AudioClip clip, float pitchValue=1.0f, float volume=1.0f)
        {
            UnityEngine.Debug.Log(volume);
            GameObject sfxGo = GameObject.Instantiate(SoundEffectManager.Instance.soundEffectPrefab);
            sfxGo.transform.position = point;
            AudioSource source = sfxGo.GetComponent<AudioSource>();
            source.pitch = pitchValue;
            source.clip = clip;
            source.volume = volume;
            source.Play();
            return sfxGo;
        }
    }
}