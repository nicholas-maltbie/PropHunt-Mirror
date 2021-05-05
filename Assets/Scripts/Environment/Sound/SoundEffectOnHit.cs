using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment.Sound
{
    /// <summary>
    /// Have an object generate sound effects when it hits something
    /// </summary>
    public class SoundEffectOnHit : MonoBehaviour
    {
        /// <summary>
        /// Minimum velocity of a collision to create a sound effect (in units per second)
        /// </summary>
        public float minCollisionVelocity = 1.0f;

        /// <summary>
        /// Material of the given object
        /// </summary>
        public SoundMaterial soundMaterial;

        /// <summary>
        /// Minimum delay (in seconds) between making sound effects
        /// </summary>
        public float minimumDelay = 0.15f;

        /// <summary>
        /// Time that the last sound was played
        /// </summary>
        private float lastSound = Mathf.NegativeInfinity;

        /// <summary>
        /// Unity service for managing time
        /// </summary>
        public IUnityService unityService = new UnityService();

        public void OnCollisionEnter(Collision other)
        {
            if (other.relativeVelocity.magnitude < minCollisionVelocity)
            {
                return;
            }

            if ((unityService.time - lastSound) >= minimumDelay)
            {
                lastSound = unityService.time;
            }
            else
            {
                return;
            }

            SoundEffectManager.CreateSoundEffectAtPoint(transform.position, soundMaterial, SoundType.Hit);
        }
    }
}