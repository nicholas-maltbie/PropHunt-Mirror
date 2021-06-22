using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    public class RebindInputButton : MonoBehaviour
    {
        public const string inputMappingPlayerPrefPrefix = "Input Mapping";

        public InputActionReference inputAction = null;
        public Text bindingDisplayNameText = null;
        public Button startRebinding = null;
        public GameObject waitingForInputObject = null;
        public MenuController menuController;

        public InputActionRebindingExtensions.RebindingOperation rebindingOperation { get; private set; }

        private string GetKeyReadableName() =>
            InputControlPath.ToHumanReadableString(
                inputAction.action.bindings[0].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

        public string InputMappingKey => $"{inputMappingPlayerPrefPrefix} {inputAction.action.name}";

        public void Awake()
        {
            // Load the default mapping saved to the file
            string inputMapping = PlayerPrefs.GetString(InputMappingKey, string.Empty);
            if (!string.IsNullOrEmpty(inputMapping))
            {
                inputAction.action.ApplyBindingOverride(inputMapping);
            }
        }

        public void Start()
        {
            startRebinding.onClick.AddListener(() => StartRebinding());
            bindingDisplayNameText.text = GetKeyReadableName();
        }

        public void StartRebinding()
        {
            startRebinding.gameObject.SetActive(false);
            waitingForInputObject.SetActive(true);
            menuController.allowInputChanges = false;

            inputAction.action.Disable();
            inputAction.action.actionMap.Disable();
            rebindingOperation = inputAction.action.PerformInteractiveRebinding(0)
                .WithControlsExcluding("<Pointer>/position") // Don't bind to mouse position
                .WithControlsExcluding("<Pointer>/delta")    // To avoid accidental input from mouse motion
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .WithTimeout(5.0f)
                .OnComplete(operation => RebindComplete())
                .Start();
        }

        public void RebindComplete()
        {
            string overridePath = inputAction.action.bindings[0].overridePath;
            foreach (PlayerInput input in GameObject.FindObjectsOfType<PlayerInput>())
            {
                InputAction action = input.actions.FindAction(inputAction.name);
                if (action != null) action.ApplyBindingOverride(0, overridePath);
            }

            bindingDisplayNameText.text = GetKeyReadableName();
            rebindingOperation.Dispose();

            PlayerPrefs.SetString(InputMappingKey, inputAction.action.bindings[0].overridePath);

            startRebinding.gameObject.SetActive(true);
            waitingForInputObject.SetActive(false);
            menuController.allowInputChanges = true;
            inputAction.action.Enable();
            inputAction.action.actionMap.Enable();
        }
    }
}