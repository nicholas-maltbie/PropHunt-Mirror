using Mirror;
using PropHunt.Utils;
using System;
using System.Collections.Generic;

namespace PropHunt.Game.Communication
{
    /// <summary>
    /// Events for logging to chat information
    /// </summary>
    public struct ChatMessage
    {
        /// <summary>
        /// Label of source for event as a string
        /// </summary>
        public string source;
        /// <summary>
        /// Time that the event ocurred
        /// </summary>
        public DateTime time;
        /// <summary>
        /// Content int he message
        /// </summary>
        public string content;

        public override string ToString()
        {
            return $"[{time.ToShortTimeString()}] {source}> {content}";
        }
    }

    public class ChatMessageEvent : EventArgs
    {
        public ChatMessage message;
    }


    public class DebugChatLog : NetworkBehaviour
    {
        public static DebugChatLog Instance;

        private SortedList<DateTime, ChatMessage> messages;

        public static event EventHandler<ChatMessageEvent> DebugChatEvents;

        public INetworkService networkService;

        public void Awake()
        {
            messages = new SortedList<DateTime, ChatMessage>();
            if (DebugChatLog.Instance == null)
            {
                DebugChatLog.Instance = this;
            }
            networkService = new NetworkService(this);
        }

        public string GetChatLog()
        {
            string log = "";
            foreach(ChatMessage chatMessage in messages.Values)
            {
                log += chatMessage.ToString() + "\n";
            }
            return log;
        }

        public void ClearChatLog()
        {
            messages.Clear();
        }

        public void SendMessage(ChatMessage chatMessage)
        {
            if (networkService.isServer)
            {
                AdjustMessageLogServer(chatMessage);
            }
            else
            {
                CmdAddMessage(chatMessage);
            }
        }

        private void AdjustMessageLogServer(ChatMessage chatMessage)
        {
            AddLocalMessage(chatMessage);
            RpcAddMessage(chatMessage);
        }

        public void AddLocalMessage(ChatMessage chatMessage)
        {
            messages.Add(chatMessage.time, chatMessage);
            DebugChatEvents?.Invoke(this, new ChatMessageEvent{message = chatMessage});
        }

        [Command]
        private void CmdAddMessage(ChatMessage chatMessage)
        {
            AdjustMessageLogServer(chatMessage);
        }

        [ClientRpc]
        private void RpcAddMessage(ChatMessage chatMessage)
        {
            AddLocalMessage(chatMessage);
        }
    }
}