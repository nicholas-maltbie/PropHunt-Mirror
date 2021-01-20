using kcp2k;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    /// <summary>
    /// Tests that involve connecting to the server
    /// </summary>
    public class NetworkIntegrationTest
    {
        protected readonly List<GameObject> _createdObjects = new List<GameObject>();

        [UnitySetUp]
        public void Setup()
        {
            Transport.activeTransport = new GameObject().AddComponent<KcpTransport>();

            // start server/client
            NetworkServer.Listen(1);
            NetworkClient.ConnectHost();
            NetworkServer.SpawnObjects();
            NetworkServer.ActivateHostScene();
            NetworkClient.ConnectLocalServer();

            NetworkServer.localConnection.isAuthenticated = true;
            NetworkClient.connection.isAuthenticated = true;

            ClientScene.Ready(NetworkClient.connection);
        }

        [UnityTearDown]
        public void TearDown()
        {
            // stop server/client
            NetworkClient.DisconnectLocalServer();

            NetworkClient.Disconnect();
            NetworkClient.Shutdown();

            NetworkServer.Shutdown();

            foreach (GameObject item in _createdObjects)
            {
                if (item != null)
                {
                    GameObject.DestroyImmediate(item);
                }
            }
            _createdObjects.Clear();

            NetworkIdentity.spawned.Clear();

            GameObject.DestroyImmediate(Transport.activeTransport.gameObject);
        }
    }
}
