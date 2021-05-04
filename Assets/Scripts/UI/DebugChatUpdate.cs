using System.Collections;
using PropHunt.Game.Communication;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI
{
    public class DebugChatUpdate : MonoBehaviour
    {
        public Text text;
        public Scrollbar bar;

        public void OnEnable()
        {
            UpdateText();
            DebugChatLog.DebugChatEvents += HandlehatEvent;
        }

        public void OnDisable()
        {
            DebugChatLog.DebugChatEvents -= HandlehatEvent;
        }

        public void HandlehatEvent(object sender, ChatMessageEvent eventArgs)
        {
            UpdateText();
            bar.value = 0;
        }

        public void UpdateText()
        {
            text.text = DebugChatLog.Instance.GetChatLog();
        }
    }
}