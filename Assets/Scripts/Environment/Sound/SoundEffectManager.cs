using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment.Sound
{
    public class SoundEffectManager : NetworkBehaviour
    {
        private static SoundEffectManager Instance;

        public INetworkService networkService;

        public SoundEffectLibrary soundEffectLibrary;

        public GameObject soundEffectPrefab;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            this.networkService = new NetworkService(this);
            this.soundEffectLibrary.SetupLookups();
        }

        public static void CreateNetworkedSoundEffectAtPoint(
            Vector3 point, SoundMaterial material, SoundType type)
        {
            if (SoundEffectManager.Instance == null || !NetworkServer.active)
            {
                return;
            }
            
        }

        public static GameObject CreateSoundEffectAtPoint( Vector3 point, SoundMaterial soundMaterial, SoundType soundType)
        {
            return CreateSoundEffectAtPoint(point,
                SoundEffectManager.Instance.soundEffectLibrary.GetSFXClipBySoundMaterialAndType(soundMaterial, soundType).audioClip);
        }

        public static GameObject CreateSoundEffectAtPoint(Vector3 point, AudioClip clip)
        {
            GameObject sfxGo = GameObject.Instantiate(SoundEffectManager.Instance.soundEffectPrefab);
            sfxGo.transform.position = point;
            AudioSource source = sfxGo.GetComponent<AudioSource>();
            source.clip = clip;
            source.Play();
            return sfxGo;
        }
    }
}