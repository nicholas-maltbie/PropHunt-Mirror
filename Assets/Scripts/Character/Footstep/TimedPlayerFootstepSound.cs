
using Mirror;
using PropHunt.Environment.Sound;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Character.Footstep
{
    [RequireComponent(typeof(KinematicCharacterController))]
    public class TimedPlayerFootstepSound : NetworkBehaviour
    {

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

        /// <summary>
        /// Maximum time while walking between footstep sounds
        /// </summary>
        public float maxFootstepSoundDelay = 0.75f;

        /// <summary>
        /// Minimum delay between footstep sounds
        /// </summary>
        public float minFootstepSoundDelay = 0.25f;

        /// <summary>
        /// How long has the player been walking but not made a footsep sound
        /// </summary>
        private float elapsedWalkingSilent = 0.0f;

        /// <summary>
        /// Time of the most recent footstep event
        /// </summary>
        protected float lastFootstep = Mathf.NegativeInfinity;

        /// <summary>
        /// Sound type for the footstep
        /// </summary>
        [SerializeField]
        private SoundType soundType = SoundType.Footstep;

        public INetworkService networkService;
        public IUnityService unityService = new UnityService();

        public void Awake()
        {
            this.networkService = new NetworkService(this);
        }

        public virtual void Start()
        {
            this.kcc = GetComponent<KinematicCharacterController>();
        }

        protected void MakeFootstepAtPoint(Vector3 point, GameObject ground)
        {
            this.lastFootstep = unityService.time;
            this.elapsedWalkingSilent = 0.0f;
            SoundEffectEvent sfxEvent = new SoundEffectEvent
            {
                sfxId = SoundEffectManager.Instance.soundEffectLibrary.GetSFXClipBySoundMaterialAndType(
                    GetSoundMaterial(ground),
                    soundType).soundId,
                point = point,
                pitchValue = Random.Range(minPitchRange, maxPitchRange),
                volume = kcc.Sprinting ? sprintVolume : walkVolume,
                mixerGroup = "Footsteps"
            };
            PlayFootstepSound(sfxEvent);
            if (this.networkService.isServer)
            {
                RpcCreateFootstepSound(sfxEvent);
            }
            else
            {
                CmdCreateFootstepSound(sfxEvent);
            }
        }

        public virtual void PlayFootstepSound(SoundEffectEvent sfxEvent)
        {
            SoundEffectManager.CreateSoundEffectAtPoint(sfxEvent);
        }

        public void Update()
        {
            if (!this.networkService.isLocalPlayer)
            {
                return;
            }

            // If the player is on the ground and not moving, update the elapsed walking time
            if (!kcc.Falling && kcc.InputMovement.magnitude > 0)
            {
                elapsedWalkingSilent += unityService.deltaTime;
            }

            if (elapsedWalkingSilent >= maxFootstepSoundDelay)
            {
                MakeFootstepAtPoint(transform.position, kcc.floor);
            }
        }

        protected SoundMaterial GetSoundMaterial(GameObject gameObject)
        {
            return SoundMaterial.Concrete;
        }

        [Command]
        public void CmdCreateFootstepSound(SoundEffectEvent sfxEvent)
        {
            RpcCreateFootstepSound(sfxEvent);
        }

        [ClientRpc]
        public void RpcCreateFootstepSound(SoundEffectEvent sfxEvent)
        {
            if (this.networkService.isLocalPlayer)
            {
                return;
            }
            PlayFootstepSound(sfxEvent);
        }
    }
}
