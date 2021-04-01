using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Prop;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Utils
{
    /// <summary>
    /// Tests to verify behaviour of Collider cast scripts
    /// </summary>
    [TestFixture]
    public class ColliderCastTests
    {
        private GameObject go;
        private PrimitiveColliderCast primitiveColliderCast;
        private RigidbodyColliderCast rigidbodyColliderCast;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#endif
            // Setup character movement player
            this.go = new GameObject();
            this.go.AddComponent<Rigidbody>();
            this.primitiveColliderCast = this.go.AddComponent<PrimitiveColliderCast>();
            this.rigidbodyColliderCast = this.go.AddComponent<RigidbodyColliderCast>();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            GameObject.DestroyImmediate(this.go);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestPrimitiveColliderCasts()
        {
            // Test with no collider
            this.primitiveColliderCast.GetHits(Vector3.forward, 1.0f);
            // Test with Box Collider
            BoxCollider boxCollider = this.go.AddComponent<BoxCollider>();
            yield return null;
            this.primitiveColliderCast.GetHits(Vector3.forward, 1.0f);
            GameObject.DestroyImmediate(boxCollider);
            yield return null;
            // Test with sphere collider
            SphereCollider sphereCollider = this.go.AddComponent<SphereCollider>();
            yield return null;
            this.primitiveColliderCast.GetHits(Vector3.forward, 1.0f);
            GameObject.DestroyImmediate(sphereCollider);
            yield return null;
            // Test with capsule collider
            CapsuleCollider capsuleCollider = this.go.AddComponent<CapsuleCollider>();
            yield return null;
            this.primitiveColliderCast.GetHits(Vector3.forward, 1.0f);
            GameObject.DestroyImmediate(capsuleCollider);
            yield return null;
        }
    }
}