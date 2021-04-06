
using UnityEngine;

namespace PropHunt.Utils
{
    public static class PhysicsUtils
    {
        public static bool GetFirstHitIgnore(GameObject ignore, Vector3 source, Vector3 direction, float distance,
            LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, out RaycastHit closest)
        {
            bool hitSomething = false;
            closest = new RaycastHit{distance = Mathf.Infinity};
            foreach(RaycastHit hit in Physics.RaycastAll(source, direction, distance, layerMask, queryTriggerInteraction))
            {
                if (hit.collider.gameObject != ignore && hit.distance < closest.distance)
                {
                    hitSomething = true;
                    closest = hit;
                }
            }
            return hitSomething;
        }
    }
}
