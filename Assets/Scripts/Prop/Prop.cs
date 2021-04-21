using PropHunt.Environment;
using UnityEngine;

namespace PropHunt.Prop
{
    public class Prop : Interactable
    {
        public string propName;

        public GameObject disguiseVisual;

        public Collider disguiseCollider;

        public Vector3 cameraOffset;

        public void Start()
        {
            PropDatabase.AddDisguiseIfNonExists(propName,
                new Disguise
                {
                    disguiseVisual = disguiseVisual,
                    disguiseCollider = disguiseCollider,
                    cameraOffset = cameraOffset
                }
            );
        }

        public override void Interact(GameObject source)
        {
            PropDisguise disguise = source.GetComponent<PropDisguise>();
            if (disguise != null)
            {
                disguise.SetSelectedDisguise(gameObject);
            }
        }
    }
}