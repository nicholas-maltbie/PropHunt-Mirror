using PropHunt.Game.Flow;
using UnityEngine;

namespace PropHunt.UI
{
    public class ExitLobbyButton : MonoBehaviour
    {
        public void ExitLobby()
        {
            GameManager.Instance.ExitLobbyPhase();
        }
    }
}