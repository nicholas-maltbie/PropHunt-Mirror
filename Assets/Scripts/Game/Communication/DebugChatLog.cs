using Mirror;
using PropHunt.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PropHunt.Game.Communication
{
    /// <summary>
    /// Events for logging to chat information
    /// </summary>
    public readonly struct ChatMessage : IEquatable<ChatMessage>, NetworkMessage
    {
        /// <summary>
        /// Label of source for event as a string
        /// </summary>
        public readonly string source;
        /// <summary>
        /// Time that the event ocurred
        /// </summary>
        public readonly long time;
        /// <summary>
        /// Content int he message
        /// </summary>
        public readonly string content;

        public bool Equals(ChatMessage other)
        {
            return other.source == source && other.time == time && other.content == content;
        }

        public ChatMessage(string source, string content)
        {
            this.source = source;
            this.content = content;
            this.time = DateTime.Now.ToBinary();
        }

        public override string ToString()
        {
            if (source != null && source.Length > 0)
            {
                return $"[{DateTime.FromBinary(time).ToShortTimeString()}] {source}> {content}";
            }
            else
            {
                return $"[{DateTime.FromBinary(time).ToShortTimeString()}] {content}";
            }
        }
    }

    public class ChatMessageEvent : EventArgs
    {
        public ChatMessage message;
    }


    public class DebugChatLog : MonoBehaviour
    {
        public static DebugChatLog Instance;

        private List<ChatMessage> messages;

        public static event EventHandler<ChatMessageEvent> DebugChatEvents;

        public void Awake()
        {
            messages = new List<ChatMessage>();
            if (DebugChatLog.Instance == null)
            {
                DebugChatLog.Instance = this;
            }
        }

        public void SetupClient()
        {
            NetworkClient.RegisterHandler<ChatMessage>(OnMessage);
        }

        public string GetChatLog()
        {
            string log = "";
            foreach(ChatMessage chatMessage in messages)
            {
                log += chatMessage.ToString() + "\n";
            }
            return log;
        }

        public void ClearChatLog()
        {
            messages.Clear();
            DebugChatEvents?.Invoke(this, new ChatMessageEvent{message = new ChatMessage()});
        }

        public void SendChatMessage(ChatMessage chatMessage)
        {
            AdjustMessageLogServer(chatMessage);
        }

        private void AdjustMessageLogServer(ChatMessage chatMessage)
        {
            // OnMessage(chatMessage);
            NetworkServer.SendToAll(chatMessage);
            UnityEngine.Debug.Log($"Logging chat events {chatMessage}");
        }

        public void AddInfoMessage(string message)
        {
            AddLocalMessage(new ChatMessage("", message));
        }

        public void AddLocalMessage(ChatMessage chatMessage)
        {
            messages.Add(chatMessage);
            DebugChatEvents?.Invoke(this, new ChatMessageEvent{message = chatMessage});
        }

        public void OnMessage(ChatMessage chatMessage)
        {
            UnityEngine.Debug.Log($"Chat event received from server {chatMessage}");
            AddLocalMessage(chatMessage);
        }
    }
}