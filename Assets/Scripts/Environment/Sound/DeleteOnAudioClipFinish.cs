using System.Collections;
using Mirror;
using PropHunt.Utils;
using UnityEngine;

namespace PropHunt.Environment.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class DeleteOnAudioClipFinish : MonoBehaviour
    {
        protected AudioSource source;

        public void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        public IEnumerator DestorySelf()
        {
            yield return null;
            GameObject.Destroy(gameObject);
            yield return null;
        }

        public void Update()
        {
            if(!source.isPlaying)
            {
                StartCoroutine(DestorySelf());
            }
        }
    }
}