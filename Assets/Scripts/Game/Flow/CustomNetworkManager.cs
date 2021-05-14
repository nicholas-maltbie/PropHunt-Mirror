using UnityEngine;
using Mirror;
using PropHunt.Game.Communication;
using System;
using PropHunt.Environment.Sound;
using UnityEngine.SceneManagement;
using System.Collections;

namespace PropHunt.Game.Flow
{
    public class PlayerConnectEvent : EventArgs
    {
        public readonly NetworkConnection connection;

        public PlayerConnectEvent(NetworkConnection connection)
        {
            this.connection = connection;
        }
    }

    public class CustomNetworkManager : NetworkManager
    {
        public static event EventHandler<PlayerConnectEvent> OnPlayerConnect;

        [Scene]
        public string lobbyScene;

        [Scene]
        public string gameScene;

        public GameObject gameManager;

        public static CustomNetworkManager Instance;

        public IEnumerator DestorySelf()
        {
            yield return null;
            GameObject.Destroy(this);
        }

        public override void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                // Only let one exist
                StartCoroutine(DestorySelf());
                return;
            }

            NetworkClient.RegisterPrefab(gameManager);

            base.Start();
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            PlayerConnectEvent connectEvent = new PlayerConnectEvent(conn);
            OnPlayerConnect?.Invoke(this, connectEvent);
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            DebugChatLog.SendChatMessage(new ChatMessage("", $"Player {conn.connectionId} connected to server"));
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            DebugChatLog.ClearChatLog();
            NetworkClient.RegisterHandler<ChatMessage>(DebugChatLog.OnMessage);
            NetworkClient.RegisterHandler<SoundEffectEvent>(SoundEffectManager.CreateSoundEffectAtPoint);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            NetworkClient.UnregisterHandler<ChatMessage>();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            DebugChatLog.SendChatMessage(new ChatMessage("", $"Player {conn.connectionId} disconnected from server"));
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            GameObject manager = GameObject.Instantiate(gameManager);
            NetworkServer.Spawn(manager);
            DontDestroyOnLoad(manager);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DisableGameManager();
            }
        }

        public override void OnStopServer()
        {
            GameManager.Instance.DisableGameManager();
            base.OnStopServer();
        }

        /// <summary>
        /// Load the lobby scene for players
        /// </summary>
        public void LoadLobbyScene()
        {
            base.ServerChangeScene(lobbyScene);
        }

        /// <summary>
        /// Load the game scene for players
        /// </summary>
        public void LoadGameScene()
        {
            base.ServerChangeScene(gameScene);
        }
    }
}
