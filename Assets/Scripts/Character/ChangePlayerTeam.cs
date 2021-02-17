using Mirror;
using UnityEngine;

namespace PropHunt.Character
{
    public class ChangePlayerTeam : NetworkBehaviour
    {
        public string setTeam;
        public GameObject newPrefab;

        public void OnTriggerEnter(Collider other)
        {
            var team = other.GetComponent<PlayerTeam>();
            if (isServer && team != null && team.playerTeam != setTeam)
            {
                NetworkConnection conn = other.GetComponent<NetworkIdentity>().connectionToClient;
                GameObject oldPlayer = other.gameObject;
                GameObject newPlayer = Instantiate(newPrefab);
                NetworkServer.ReplacePlayerForConnection(conn, newPlayer);
                newPlayer.GetComponent<PlayerTeam>().playerTeam = setTeam;
                NetworkServer.Destroy(oldPlayer);
            }
        }
    }
}
