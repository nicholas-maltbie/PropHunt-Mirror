using NUnit.Framework;
using PropHunt.Game.Flow;
using PropHunt.UI;
using UnityEditor;
using UnityEngine;

namespace Tests.EditMode.UI
{
    [TestFixture]
    public class GameManagerButtonTests : CustomNetworkManager
    {
        [Test]
        public void GamePhaseButtonTests()
        {
            GameObject testObj = new GameObject();
            ChangeGamePhaseButton buttonPhase = testObj.AddComponent<ChangeGamePhaseButton>();
            GameManager gameManager = testObj.AddComponent<GameManager>();
            gameManager.playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Tests/EditMode/TestPlayer.prefab");
            Assert.Throws<System.InvalidOperationException>(() => gameManager.Start());
            // Test exiting lobby while in or not in lobby phase
            GameManager.Instance.gamePhase = GamePhase.Lobby;
            buttonPhase.ExitLobby();
            GameManager.Instance.gamePhase = GamePhase.Setup;
            buttonPhase.ExitLobby();
            // Test exiting while in and not in game
            GameManager.Instance.gamePhase = GamePhase.InGame;
            buttonPhase.ExitGame();
            GameManager.Instance.gamePhase = GamePhase.Lobby;
            buttonPhase.ExitGame();
            // Test exiting while in and not in score
            GameManager.Instance.gamePhase = GamePhase.Score;
            buttonPhase.ExitScore();
            GameManager.Instance.gamePhase = GamePhase.Lobby;
            buttonPhase.ExitScore();

            GameObject.DestroyImmediate(testObj);
        }
    }
}