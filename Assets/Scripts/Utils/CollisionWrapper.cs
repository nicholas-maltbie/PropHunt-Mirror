
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropHunt.Utils
{
    /// <summary>
    /// Interfact describing properties of a collision
    /// </summary>
    public interface ICollision
    {
        /// <summary>
        /// The relative linear velocity of the two colliding objects (Read Only).
        /// </summary>
        Vector3 relativeVelocity { get; }
        /// <summary>
        /// The Rigidbody we hit (Read Only). This is null if the object we hit is a collider
        ///    with no rigidbody attached.
        /// </summary>
        Rigidbody rigidbody { get; }
        /// <summary>
        /// The Collider we hit (Read Only).
        /// </summary>
        Collider collider { get; }
        /// <summary>
        /// The Transform of the object we hit (Read Only).
        /// </summary>
        Transform transform { get; }
        /// <summary>
        /// The GameObject whose collider you are colliding with. (Read Only).
        /// </summary>
        GameObject gameObject { get; }
        /// <summary>
        /// Gets the number of contacts for this collision.
        /// </summary>
        int contactCount { get; }
        /// <summary>
        /// The contact points generated by the physics engine. You should avoid using this
        ///    as it produces memory garbage. Use GetContact or GetContacts instead.
        /// </summary>
        IContactPoint[] contacts { get; }
        /// <summary>
        /// The total impulse applied to this contact pair to resolve the collision.
        /// </summary>
        Vector3 impulse { get; }
        /// <summary>
        /// Gets the contact point at the specified index.
        /// </summary>
        /// <param name="index">The index of the contact to retrieve.</param>
        /// <returns>The contact at the specified index.</returns>
        IContactPoint GetContact(int index);
        /// <summary>
        /// Retrieves all contact points for this collision.
        /// </summary>
        /// <param name="contacts">An array of ContactPoint used to receive the results.</param>
        /// <returns>Returns the number of contacts placed in the contacts array.</returns>
        int GetContacts(ContactPoint[] contacts);
        int GetContacts(List<ContactPoint> contacts);
    }

    /// <summary>
    /// Implementation of ICollision using a wrapper fo a collision event
    /// </summary>
    public class CollisionWrapper : ICollision
    {
        private Collision collision;

        public CollisionWrapper(Collision collision)
        {
            this.collision = collision;
        }

        public Vector3 relativeVelocity => collision.relativeVelocity;
        public Rigidbody rigidbody => collision.rigidbody;
        public Collider collider => collision.collider;
        public Transform transform => collision.transform;
        public GameObject gameObject => collision.gameObject;
        public int contactCount => collision.contactCount;
        public IContactPoint[] contacts => Enumerable.Range(0, contactCount).Select(num => GetContact(num)).ToArray();
        public Vector3 impulse => collision.impulse;
        public IContactPoint GetContact(int index) => new ContactPointWrapper(collision.GetContact(index));
        public int GetContacts(ContactPoint[] contacts) => collision.GetContacts(contacts);
        public int GetContacts(List<ContactPoint> contacts) => collision.GetContacts(contacts);
    }
}
