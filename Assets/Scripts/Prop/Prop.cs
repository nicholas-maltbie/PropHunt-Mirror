using Mirror;
using UnityEngine;

namespace PropHunt.Prop
{
    public class Prop : NetworkBehaviour
    {
        public string propName;

        public GameObject disguiseVisual;

        public Collider disguiseCollider;

        public void Start()
        {
            PropDatabase.AddDisguiseIfNonExists(propName,
                new Disguise {
                    disguiseVisual = disguiseVisual,
                    disguiseCollider = disguiseCollider
                }
            );
        }

        public void Interact(GameObject source)
        {
            PropDisguise disguise = source.GetComponent<PropDisguise>();
            if (disguise != null)
            {
                disguise.CmdSetDisguise(gameObject);
            }
        }
    }
}