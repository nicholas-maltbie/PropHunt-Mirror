using Mirror;
using Moq;
using NUnit.Framework;
using PropHunt.Character;
using PropHunt.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode.Character
{
    [TestFixture]
    public class CharacterMovementIntegrationTests : NetworkIntegrationTest
    {
        [UnityTest]
        public IEnumerator TestCharacterMovement()
        {
            // Setup the camera transform
            GameObject cameraObject = new GameObject();
            // Setup the playerGo
            NetworkIdentity identity = new GameObject().AddComponent<NetworkIdentity>();
            CharacterController controller = identity.gameObject.AddComponent<CharacterController>();
            CharacterMovement movementControls = identity.gameObject.AddComponent<CharacterMovement>();
            movementControls.cameraTransform = cameraObject.transform;
            movementControls.movementSpeed = 5.0f;

            // Put a large floor below the player
            GameObject floor = new GameObject();
            floor.transform.position = new Vector3(0, -1, 0);
            // Make the floor large (25 units by 25 units)
            // BoxCollider collider = floor.AddComponent<BoxCollider>();
            // collider.size = new Vector3(25, 1, 25);
    
            // Wait one frame for the game to setup
            yield return null;

            SpawnMessage msg = new SpawnMessage
            {
                netId = NetworkConnection.LocalConnectionId,
                isLocalPlayer = true,
                isOwner = true,
            };

            // Fake the character moving forward for 3 seconds
            Mock<IUnityService> mockInputs = new Mock<IUnityService>();
            movementControls.unityService = mockInputs.Object;
            mockInputs.Setup(e => e.GetAxis("Vertical")).Returns(1.0f);
            // Fake the player being controlled locally
            Mock<INetworkService> mockNetwork = new Mock<INetworkService>();
            mockNetwork.Setup(e => e.isLocalPlayer).Returns(true);
            movementControls.networkService = mockNetwork.Object;

            // Allow for 3 seconds of time to pass
            yield return new WaitForSeconds(1.0f);

            // Assert that the player has moved some distance
            UnityEngine.Debug.Log(identity.gameObject.transform.position);
            Assert.IsTrue(identity.gameObject.transform.position == new Vector3(0, 0, 5));

            // Cleanup objects
            GameObject.DestroyImmediate(identity.gameObject);
            GameObject.DestroyImmediate(floor);
            GameObject.DestroyImmediate(cameraObject);
        }
    }
}
