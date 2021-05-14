using Mirror;
using Mirror.Tests;
using NUnit.Framework;
using PropHunt.Game.Flow;
using UnityEditor;
using UnityEngine;

namespace Tests.EditMode.Game.Flow
{
    public class CustomNetworkManagerTest
    {
        CustomNetworkManager networkManager;

        [SetUp]
        public virtual void Setup()
        {
            GameObject go = new GameObject();
            Transport.activeTransport = go.AddComponent<MemoryTransport>();
            networkManager = go.AddComponent<CustomNetworkManager>();
            networkManager.playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tests/EditMode/TestPlayer.prefabs");

            networkManager.StartHost();
        }

        [TearDown]
        public virtual void TearDown()
        {
            networkManager.StopHost();

            GameObject.DestroyImmediate(networkManager.playerPrefab);
            GameObject.DestroyImmediate(networkManager.gameObject);
        }
    }

    [TestFixture]
    public class DebugChatCommunicationTests : CustomNetworkManagerTest
    {
        [Test]
        public void TestDebugLogUpdates()
        {
            // Don't do anything here, test is ensuring setup and tear down don't create errors
        }
    }
}