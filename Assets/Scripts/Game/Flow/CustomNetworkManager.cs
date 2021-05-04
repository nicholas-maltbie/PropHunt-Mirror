using UnityEngine;
using Mirror;
using PropHunt.Game.Communication;
using System;

namespace PropHunt.Game.Flow
{
    public class CustomNetworkManager : NetworkManager
    {
        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            DebugChatLog.Instance.SendChatMessage(new ChatMessage("", $"Player {conn.connectionId} connected to server"));
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            DebugChatLog.Instance.ClearChatLog();
            NetworkClient.RegisterHandler<ChatMessage>(DebugChatLog.Instance.OnMessage);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            NetworkClient.UnregisterHandler<ChatMessage>();
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            DebugChatLog.Instance.SendChatMessage(new ChatMessage("", $"Player {conn.connectionId} disconnected from server"));
        }
    }
}
