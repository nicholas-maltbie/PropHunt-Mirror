using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace PropHunt.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameScreen : MonoBehaviour
    {
        private PlayerInput playerInput;
        private CanvasGroup canvasGroup;

        public void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetupScreen(InputSystemUIInputModule uIInputModule)
        {
            // playerInput.uiInputModule = uIInputModule;
            playerInput.actions = uIInputModule.actionsAsset;
        }

        public void DisplayScreen()
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            if (playerInput != null)
            {
                playerInput.ActivateInput();
            }
            foreach (IScreenComponent screenComponent in gameObject.GetComponents<IScreenComponent>())
            {
                screenComponent.OnScreenLoaded();
            }
            foreach (IScreenComponent screenComponent in gameObject.GetComponentsInChildren<IScreenComponent>())
            {
                screenComponent.OnScreenLoaded();
            }
        }

        public void HideScreen()
        {
            canvasGroup.alpha = 0.0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            if (playerInput != null)
            {
                playerInput.DeactivateInput();
            }
            foreach (IScreenComponent screenComponent in gameObject.GetComponents<IScreenComponent>())
            {
                screenComponent.OnScreenUnloaded();
            }
            foreach (IScreenComponent screenComponent in gameObject.GetComponentsInChildren<IScreenComponent>())
            {
                screenComponent.OnScreenUnloaded();
            }
        }
    }
}