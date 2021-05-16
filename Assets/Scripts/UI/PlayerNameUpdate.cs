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
            string selectedName = CharacterNameManagement.GetFilteredName(field.text);
            field.text = selectedName;

            if (selectedName.Length > maxLength)
            {
                selectedName = selectedName.Substring(0, 16);
            }

            CharacterNameManagement.playerName = selectedName;
        }
    }
}