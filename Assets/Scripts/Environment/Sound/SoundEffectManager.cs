using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment.Sound
{
    public class SoundEffectManager : NetworkBehaviour
    {
        private static SoundEffectManager Instance;

        public INetworkService networkService;

        public GameObject soundEffectPrefab;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            this.networkService = new NetworkService(this);
        }

        public static void CreateNetworkedSoundEffectAtPoint(Vector3 point, AudioClip clip)
        {
            if (SoundEffectManager.Instance == null || !NetworkServer.active)
            {
                return;
            }
            
        }
    }
}