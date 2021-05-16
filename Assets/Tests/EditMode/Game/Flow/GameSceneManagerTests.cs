using System.Collections;
using Mirror;
using Moq;
using NUnit.Framework;
using PropHunt.Game.Flow;
using PropHunt.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Game.Flow
{
    [TestFixture]
    public class GameSceneManagerTests : CustomNetworkManagerTestBase
    {
        public GameSceneManager sceneManager;
        public Mock<INetworkService> networkServiceMock;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            sceneManager = new GameObject().AddComponent<GameSceneManager>();
            sceneManager.Start();
            networkServiceMock = new Mock<INetworkService>();
            sceneManager.newtworkService = networkServiceMock.Object;
            sceneManager.OnStartServer();
        }

        [TearDown]
        public override void TearDown()
        {
            LogAssert.ignoreFailingMessages = true;
            base.TearDown();
            GameObject.DestroyImmediate(sceneManager.gameObject);
            sceneManager.OnStopServer();
        }

        [Test]
        public void TestUpdateChangePhase()
        {
            networkServiceMock.Setup(e => e.activeNetworkServer).Returns(false);
            sceneManager.Update();

            networkServiceMock.Setup(e => e.activeNetworkServer).Returns(true);
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            GameManager.ChangePhase(GamePhase.Setup);
            sceneManager.Update();
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            GameManager.ChangePhase(GamePhase.Reset);
            sceneManager.Update();
        }

        [Test]
        public void TestHandleVariousPhaseChanges()
        {
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            GameManager.ChangePhase(GamePhase.Reset);
            GameManager.ChangePhase(GamePhase.Lobby);
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            GameManager.ChangePhase(GamePhase.Setup);
            GameManager.ChangePhase(GamePhase.InGame);
            GameManager.ChangePhase(GamePhase.Score);
            LogAssert.Expect(LogType.Error, "ServerChangeScene empty scene name");
            GameManager.ChangePhase(GamePhase.Reset);
        }
    }
}