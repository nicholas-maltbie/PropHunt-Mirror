using System.Collections;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Character
{
    /// <summary>
    /// Tests to verify behaviour of camera controller script behaviour
    /// </summary>
    [TestFixture]
    public class CameraControllerTests
    {
        CameraController cameraController;
        Mock<INetworkService> networkServiceMock;
        GameObject wall;

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {
            // Create a game object and setup camera follow component
            GameObject go = new GameObject();
            this.cameraController = go.AddComponent<CameraController>();
            this.cameraController.cameraTransform = go.transform;
            this.cameraController.Start();
            this.networkServiceMock = new Mock<INetworkService>();
            this.cameraController.networkService = this.networkServiceMock.Object;
            this.cameraController.cameraTransform = go.transform;
            this.cameraController.thirdPersonCharacterBase = go;

            GameObject wall = new GameObject();
            wall.transform.position = Vector3.back;
            wall.AddComponent<BoxCollider>();

            PlayerInputManager.playerMovementState = PlayerInputState.Allow;

            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.cameraController.gameObject);
            GameObject.DestroyImmediate(this.wall);
        }

        [Test]
        public void TestCameraFollowNotLocal()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(false);
            this.cameraController.Update();
        }

        [Test]
        public void TestCameraFollowLocal()
        {
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.cameraController.Update();
        }

        [Test]
        public void TestCameraRaycasthit()
        {
            this.cameraController.maxCameraDistance = 2.0f;
            this.cameraController.currentDistance = 2.0f;
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.cameraController.Update();
        }

        [Test]
        public void TestCameraRaycastFromBase()
        {
            RaycastHit hit;
            Assert.IsFalse(this.cameraController.RaycastFromCameraBase(Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore, out hit));
            Assert.IsFalse(this.cameraController.SpherecastFromCameraBase(Mathf.Infinity, ~0, 0.1f, QueryTriggerInteraction.Ignore, out hit));
        }

        [Test]
        public void TestVariousDitherThresholds()
        {
            this.cameraController.maxCameraDistance = 2.0f;
            this.cameraController.currentDistance = 2.0f;
            this.cameraController.shadowOnlyDistance = 0.0f;
            this.cameraController.ditherDistance = 5.0f;
            this.networkServiceMock.Setup(e => e.isLocalPlayer).Returns(true);
            this.cameraController.Update();
            this.cameraController.ditherDistance = 0.0f;
            this.cameraController.Update();
        }

        [Test]
        public void TestInputProcessing()
        {
            this.cameraController.OnLook(new UnityEngine.InputSystem.InputAction.CallbackContext());
        }
    }
}