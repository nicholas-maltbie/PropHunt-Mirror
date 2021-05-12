using UnityEngine;
using Mirror;
using PropHunt.Game.Communication;
using System;
using PropHunt.Environment.Sound;
using PropHunt.Utils;

namespace PropHunt.Game.Flow
{
    /// <summary>
    /// Phases that the game can be in. Each of these phases acts as a state of 
    /// a state machine in which the initial state is Lobby. When the game starts, 
    /// the game will go into setup and game phases. Once a round ends, the game
    /// will move into Score screen and Reset.
    /// </summary>
    public enum GamePhase
    {
        Lobby,
        Setup,
        InGame,
        Score,
        Reset,
    }

    /// <summary>
    /// Game phase change event that saves the previous and next game states
    /// </summary>
    public class GamePhaseChange : EventArgs
    {
        /// <summary>
        /// Previous game state
        /// </summary>
        public readonly GamePhase previous;
        /// <summary>
        /// Next game state
        /// </summary>
        public readonly GamePhase next;

        public GamePhaseChange(GamePhase previous, GamePhase next)
        {
            this.previous = previous;
            this.next = next;
        }
    }

    /// <summary>
    /// Game manager for managing phases of the game
    /// </summary>
    public class GameManager : NetworkBehaviour
    {
        public static event EventHandler<GamePhaseChange> OnGamePhaseChange;

        [SyncVar(hook = nameof(SetGamePhase))]
        public GamePhase gamePhase;

        /// <summary>
        /// How long have we been in the current phase
        /// </summary>
        public float phaseTime;

        public INetworkService networkService;

        private CustomNetworkManager networkManager;

        public GameObject playerPrefab;

        public IUnityService unityService = new UnityService();

        public static GameManager Instance;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            OnGamePhaseChange += HandleGamePhaseChange;
            this.networkService = new NetworkService(this);
            if (GameManager.Instance == null)
            {
                GameManager.Instance = this;
            }
        }

        public void Start()
        {
            this.networkManager = GameObject.FindObjectOfType<CustomNetworkManager>();

            if (!NetworkClient.prefabs.ContainsValue(playerPrefab))
            {
                NetworkClient.RegisterPrefab(playerPrefab);
            }
        }

        public void ExitLobbyPhase()
        {
            // If host and current phase is lobby, go to next phase
            if (gamePhase == GamePhase.Lobby)
            {
                ChangePhase(GamePhase.Setup);
            }
        }

        [Server]
        private void ChangePhase(GamePhase next)
        {
            gamePhase = next;
        }

        public void SetGamePhase(GamePhase previousPhase, GamePhase newPhase)
        {
            GamePhaseChange changeEvent = new GamePhaseChange(previousPhase, newPhase);
            OnGamePhaseChange?.Invoke(this, changeEvent);
        }

        public void Update()
        {
            if (!networkService.isServer)
            {
                return;
            }

            // Increment current phase time
            phaseTime += unityService.deltaTime;

            switch (gamePhase)
            {
                // Do things differently based on phase
                case GamePhase.Lobby:

                    break;
                case GamePhase.Setup:
                    // Once loading is complete, go to InGame
                    if (NetworkManager.loadingSceneAsync.isDone)
                    {
                        ChangePhase(GamePhase.InGame);
                    }
                    break;
                case GamePhase.InGame:
                    // Check for conditions to end in game phase
                    //   i. Game timeout (time runs out)
                    //  ii. Hunters win (enough props were caught)
                    // iii. Props win (all props finished objectives)
                    break;
                case GamePhase.Score:
                    // Display score screen to players
                    //  End phase either when players have all hit continue or timeout has ocurred
                    break;
                case GamePhase.Reset:
                    // Once laoding is complete, go to lobby
                    if (NetworkManager.loadingSceneAsync.isDone)
                    {
                        ChangePhase(GamePhase.Lobby);
                    }
                    break;
            }
        }

        public void HandleGamePhaseChange(object sender, GamePhaseChange change)
        {
            if (!networkService.isServer)
            {
                return;
            }

            // Reset phase timer
            phaseTime = 0;

            // Handle whenever the game state changes
            switch (change.next)
            {
                // Do things differently based on the new phase
                case GamePhase.Lobby:

                    break;
                case GamePhase.Setup:
                    UnityEngine.Debug.Log("Resetting game state back to lobby");
                    networkManager.LoadGameScene();
                    // Once loading is complete, go to InGame
                    break;
                case GamePhase.InGame:
                    // When in game starts, spawn a player for each connection
                    foreach (NetworkConnection conn in NetworkServer.connections.Values)
                    {
                        GameObject newPlayer = GameObject.Instantiate(playerPrefab);
                        NetworkServer.DestroyPlayerForConnection(conn);
                        NetworkServer.AddPlayerForConnection(conn, newPlayer);
                    }
                    break;
                case GamePhase.Score:

                    break;
                case GamePhase.Reset:
                    // Destory the network clients for each player
                    foreach (NetworkConnection conn in NetworkServer.connections.Values)
                    {
                        NetworkServer.DestroyPlayerForConnection(conn);
                    }
                    // Start loading the lobby scene
                    UnityEngine.Debug.Log("Resetting game state back to lobby");
                    networkManager.LoadLobbyScene();
                    // Once laoding is complete, go to lobby
                    break;
            }
        }
    }
}
