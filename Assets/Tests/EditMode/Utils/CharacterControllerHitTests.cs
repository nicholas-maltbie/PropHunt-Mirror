using System;
using NUnit.Framework;
using PropHunt.Utils;
using UnityEngine;

namespace Tests.EditMode.Utils
{
    [TestFixture]
    public class CharacterControllerHitTests
    {
        [Test]
        public void VerifyCopyComponent()
        {
            GameObject gameObject = new GameObject();
            KinematicCharacterControllerHit hit = new KinematicCharacterControllerHit(
                gameObject.AddComponent<BoxCollider>(), gameObject.AddComponent<Rigidbody>(),
                gameObject, gameObject.transform, Vector3.zero, Vector3.zero, Vector3.zero, 2.5f
            );

            Assert.IsTrue(hit.collider == gameObject.GetComponent<BoxCollider>());
            Assert.IsTrue(hit.rigidbody == gameObject.GetComponent<Rigidbody>());
            Assert.IsTrue(hit.gameObject == gameObject);
            Assert.IsTrue(hit.transform == gameObject.transform);
            Assert.IsTrue(hit.point == Vector3.zero);
            Assert.IsTrue(hit.normal == Vector3.zero);
            Assert.IsTrue(hit.moveDirection == Vector3.zero);
            Assert.IsTrue(hit.moveLength == 2.5f);

            GameObject.DestroyImmediate(gameObject);
        }
    }
}