using System.Collections;
using UnityEngine;

namespace PropHunt.Environment.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class DeleteOnAudioClipFinish : MonoBehaviour
    {
        protected AudioSource source;

        public float clearTime = 1.0f;

        private bool cleared = false;

        public void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        public IEnumerator DestorySelf()
        {
            yield return new WaitForSeconds(clearTime);
            GameObject.Destroy(gameObject);
            yield return null;
        }

        public void Update()
        {
            if(!source.isPlaying && !cleared)
            {
                cleared = true;
                StartCoroutine(DestorySelf());
            }
        }
    }
}