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

namespace Tests.EditMode.Prop
{
    /// <summary>
    /// Tests to verify behaviour of PropMovement script
    /// </summary>
    [TestFixture]
    public class PropMovementTests
    {
        PropMovement propMovement;

        Rigidbody rigidbody;

        Collider collider;

        /// <summary>
        /// Mock of network service attached to cameraFollow script
        /// </summary>
        Mock<INetworkService> networkServiceMock;

        /// <summary>
        /// Mock of unity service to mock inputs and delta time
        /// </summary>
        Mock<IUnityService> unityServiceMock;

        [SetUp]
        public void Setup()
        {
            // Setup character movement player
            GameObject characterGo = new GameObject();
            this.collider = characterGo.AddComponent<SphereCollider>();
            this.rigidbody = characterGo.AddComponent<Rigidbody>();
            this.propMovement = characterGo.AddComponent<PropMovement>();
            this.propMovement.Start();
            this.unityServiceMock = new Mock<IUnityService>();
            this.networkServiceMock = new Mock<INetworkService>();
            this.propMovement.unityService = this.unityServiceMock.Object;
            this.propMovement.networkService = this.networkServiceMock.Object;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.propMovement.gameObject);
            PropDatabase.ClearDisguises();
        }

        [Test]
        public void TestMovement()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.propMovement.FixedUpdate();
        }

        [Test]
        public void TestNotLocal()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);
            this.propMovement.FixedUpdate();
        }

        [Test]
        public void TestDenyMovement()
        {
            PlayerInputManager.playerMovementState = PlayerInputState.Deny;
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);
            this.propMovement.FixedUpdate();
        }

        [Test]
        public void TestCollisionInteractions()
        {
            this.propMovement.OnCollisionExit(null);
            Collision testCollision = new Collision();
            this.propMovement.OnCollisionEnter(testCollision);
            IContactPoint[] contactPoints = new IContactPoint[3];
            Mock<IContactPoint> mockedContact1 = new Mock<IContactPoint>();
            Mock<IContactPoint> mockedContact2 = new Mock<IContactPoint>();
            Mock<IContactPoint> mockedContact3 = new Mock<IContactPoint>();

            contactPoints[0] = mockedContact1.Object;
            contactPoints[1] = mockedContact2.Object;
            contactPoints[2] = mockedContact3.Object;

            mockedContact1.Setup(e => e.normal).Returns(Vector3.up);
            mockedContact1.Setup(e => e.normal).Returns(new Vector3(1, 1, 0).normalized);
            mockedContact1.Setup(e => e.normal).Returns(Vector3.forward);

            this.propMovement.UpdateContactPoint(contactPoints);
        }

        [UnityTest]
        public IEnumerator TestGrounded()
        {
            
            // Create a floor below the player
            GameObject floor = new GameObject();
            BoxCollider floorCollider = floor.AddComponent<BoxCollider>();
            floor.transform.transform.position = new Vector3(0, -1.1f, 0);
            this.propMovement.gameObject.transform.position = Vector3.zero;
            yield return null;
            UnityEngine.Debug.Log(rigidbody.transform.position + " " + collider.bounds + " " + floor.transform.position + " " + floorCollider.bounds);
            bool hit = rigidbody.SweepTest(Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, QueryTriggerInteraction.UseGlobal);
            UnityEngine.Debug.Log(hit);
            UnityEngine.Debug.Log(hitInfo.distance);
            yield return null;
            this.propMovement.groundCheckDistance = Mathf.Infinity;
            this.propMovement.groundedDistance = 1.0f;

            // Do a test when the character is turning, moving forward, and jumping
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.propMovement.FixedUpdate();
            GameObject.DestroyImmediate(floor);
            yield return null;
        }

        [Test]
        public void TestJump()
        {
            // Create a floor below the player
            this.propMovement.gameObject.transform.position = Vector3.zero;
            this.propMovement.collidingGround = true;
            this.propMovement.distanceToGround = 0;
            this.propMovement.currentAngle = 0;

            // Do a test when the character is turning, moving forward, and jumping
            this.unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
            this.unityServiceMock.Setup(e => e.GetButton("Jump")).Returns(true);
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.propMovement.FixedUpdate();
        }
    }
}