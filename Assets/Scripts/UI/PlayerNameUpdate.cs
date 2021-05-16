using PropHunt.Character;
using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI
{
    public class PlayerNameUpdate : MonoBehaviour
    {
        public int maxLength = 16;

        public InputField field;

        public void UpdatePlayerName()
        {
            string selectedName = field.text.Trim();

            if (selectedName.Length > maxLength)
            {
                selectedName = selectedName.Substring(0, 16);
            }

            UnityEngine.Debug.Log(selectedName);

            CharacterNameManagement.playerName = selectedName;
        }
    }
}