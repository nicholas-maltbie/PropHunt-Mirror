using UnityEngine;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    public class ChangeScreenActions : MonoBehaviour
    {
        public Dropdown dropdown;
        
        // Dropdown events should be organized as
        //  0 - Fullscreen
        //  1 - Windowed
        //  2 - Borderless Fullscreen

        public void SetFullScreenMode()
        {
            FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;
            if (dropdown.value == 0)
            {
                mode = FullScreenMode.ExclusiveFullScreen;
            }
            else if (dropdown.value == 1)
            {
                mode = FullScreenMode.Windowed;
            }
            else
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