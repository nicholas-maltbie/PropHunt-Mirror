using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    public class ChangeScreenActions : MonoBehaviour
    {
        /// <summary>
        /// Dropdown events should be organized as
        ///  0 - Fullscreen
        ///  1 - Windowed
        ///  2 - Borderless Fullscreen
        /// </summary>
        public Dropdown windowedDropdown;

        public void SetFullScreenMode()
        {
            FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;
            string selectedMode = windowedDropdown.options[windowedDropdown.value].text;
            if (selectedMode == "Fullscreen")
            {
                mode = FullScreenMode.ExclusiveFullScreen;
            }
            else if (selectedMode == "Windowed")
            {
                mode = FullScreenMode.Windowed;
            }
            else if (selectedMode == "Borderless Fullscreen")
            {
                mode = FullScreenMode.FullScreenWindow;
            }
#if UNITY_EDITOR
            UnityEngine.Debug.Log("Setting screen mode " + mode.ToString());
#else
            Screen.fullScreenMode = mode;
#endif
        }
    }
}