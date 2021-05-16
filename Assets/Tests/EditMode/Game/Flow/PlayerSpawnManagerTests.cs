using System.Collections;
using Mirror;
using NUnit.Framework;
using PropHunt.Game.Flow;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Game.Flow
{
    [TestFixture]
    public class PlayerSpawnManagerTests : CustomNetworkManagerTestBase
    {
        public PlayerSpawnManager spawnManager;
        public GameObject playerPrefab;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            spawnManager = new GameObject().AddComponent<PlayerSpawnManager>();
            playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tests/EditMode/TestPlayer.prefab");
            spawnManager.inGamePlayer = playerPrefab;
            spawnManager.lobbyPlayer = playerPrefab;

            spawnManager.OnStartClient();
            spawnManager.OnStartServer();
        }

        [TearDown]
        public override void TearDown()
        {
            LogAssert.ignoreFailingMessages = true;
            base.TearDown();
            GameObject.DestroyImmediate(spawnManager.gameObject);
            spawnManager.OnStopClient();
            spawnManager.OnStopServer();
        }

        [Test]
        public void TestHandleVariousPhaseChanges()
        {
            GameManager.ChangePhase(GamePhase.Reset);
            GameManager.ChangePhase(GamePhase.Lobby);
            GameManager.ChangePhase(GamePhase.Setup);
            GameManager.ChangePhase(GamePhase.InGame);
            GameManager.ChangePhase(GamePhase.Score);
            GameManager.ChangePhase(GamePhase.Reset);
        }

        [Test]
        public void SpawnCharacterOnJoin()
        {
            GameManager.ChangePhase(GamePhase.InGame);
            spawnManager.HandlePlayerConnect(this, new PlayerConnectEvent(NetworkServer.connections[0]));
            GameManager.ChangePhase(GamePhase.Lobby);
            spawnManager.HandlePlayerConnect(this, new PlayerConnectEvent(NetworkServer.connections[0]));
        }

        [Test]
        public void HandleSpawningCharacter()
        {
            NetworkConnection conn = NetworkServer.connections[0];
            IEnumerator spawnCharacter = spawnManager.SpawnPlayerCharacter(conn, playerPrefab);
            conn.isReady = false;
            int i = 0;
            while (spawnCharacter.MoveNext()) { i++; conn.isReady = i > 10; }
        }
    }
}