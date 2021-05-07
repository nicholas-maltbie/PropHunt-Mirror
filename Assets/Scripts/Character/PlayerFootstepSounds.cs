using Mirror;
using PropHunt.Animation;
using PropHunt.Environment.Sound;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character
{
    /// <summary>
    /// Create footstep sounds based on player animation
    /// </summary>
    [RequireComponent(typeof(KinematicCharacterController))]
    public class PlayerFootstepSounds : NetworkBehaviour
    {
        /// <summary>
        /// Foot grounded component for detecting footsteps
        /// </summary>
        public PlayerFootGrounded footGrounded;

        /// <summary>
        /// Kinematic character conttroller for player movement
        /// </summary>
        private KinematicCharacterController kcc;

        /// <summary>
        /// Volume of footsteps when sprinting
        /// </summary>
        public float sprintVolume = 0.95f;

        /// <summary>
        /// Volume of footsteps when walking
        /// </summary>
        public float walkVolume = 0.45f;

        /// <summary>
        /// Minimum pitch modulation for footsteps
        /// </summary>
        public float minPitchRange = 0.95f;
        
        /// <summary>
        /// Maximum pitch modulation for footsteps
        /// </summary>
        public float maxPitchRange = 1.05f;

        public INetworkService networkService;
        public UnityService unityService = new UnityService();

        public void Start()
        {
            this.networkService = new NetworkService(this);
            this.kcc = GetComponent<KinematicCharacterController>();
            footGrounded.PlayerFootstep += HandleFootstepEvent;
        }

        private SoundMaterial GetSoundMaterial(GameObject gameObject)
        {
            return SoundMaterial.Concrete;
        }

        public void HandleFootstepEvent(object sender, FootstepEvent footstepEvent)
        {
            if (!networkService.isLocalPlayer || footstepEvent.state != FootstepState.Down)
            {
                return;
            }
            SoundEffectManager.CreateNetworkedSoundEffectAtPoint(
                footstepEvent.footstepPosition, GetSoundMaterial(footstepEvent.floor),
                SoundType.Footstep, Random.Range(minPitchRange, maxPitchRange), 
                kcc.isSprinting ? sprintVolume : walkVolume, "Footsteps"
            );
        }
    }
}