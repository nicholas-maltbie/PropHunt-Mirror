using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    public class RebindCompositeInput : MonoBehaviour
    {
        public const string inputMappingPlayerPrefPrefix = "Input Mapping";

        public InputActionReference inputAction = null;
        public MenuController menuController;

        [System.Serializable]
        public struct RebindingGroup
        {
            public Text bindingDisplayNameText;
            public Button startRebinding;
            public GameObject waitingForInputObject;
        }

        public RebindingGroup[] rebindingGroups;

        public InputActionRebindingExtensions.RebindingOperation rebindingOperation { get; private set; }

        private string GetKeyReadableName(int index) => InputControlPath.ToHumanReadableString(
            inputAction.action.bindings[index].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        public string InputMappingKey(int index) => $"{inputMappingPlayerPrefPrefix} {index} {inputAction.action.name}";

        public void Awake()
        {
            // Load the default mapping saved to the file
            // Start bindings at 1 as 0 is the composite start and 1 represents the first binding index
            for (int i = 1; i <= rebindingGroups.Length; i++)
            {
                string inputMapping = PlayerPrefs.GetString(InputMappingKey(i), string.Empty);
                if (!string.IsNullOrEmpty(inputMapping))
                {
                    inputAction.action.ApplyBindingOverride(i, inputMapping);
                }
            }
        }

        public void Start()
        {
            for (int i = 0; i < rebindingGroups.Length; i++)
            {
                int temp = i;
                rebindingGroups[i].startRebinding.onClick.AddListener(() => StartRebinding(temp));
                rebindingGroups[i].bindingDisplayNameText.text = GetKeyReadableName(i + 1);
            }
        }

        public void StartRebinding(int index)
        {
            rebindingGroups[index].startRebinding.gameObject.SetActive(false);
            rebindingGroups[index].waitingForInputObject.SetActive(true);
            menuController.allowInputChanges = false;

            inputAction.action.Disable();
            inputAction.action.actionMap.Disable();

            rebindingOperation = inputAction.action.PerformInteractiveRebinding(index + 1)
                .WithControlsExcluding("<Pointer>/position") // Don't bind to mouse position
                .WithControlsExcluding("<Pointer>/delta")    // To avoid accidental input from mouse motion
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .WithTimeout(5.0f)
                .OnComplete(operation => RebindComplete(index))
                .Start();
        }

        public void RebindComplete(int index)
        {
            int bindingIndex = index + 1;
            string overridePath = inputAction.action.bindings[bindingIndex].overridePath;
            foreach (PlayerInput input in GameObject.FindObjectsOfType<PlayerInput>())
            {
                InputAction action = input.actions.FindAction(inputAction.name);
                if (action != null) action.ApplyBindingOverride(bindingIndex, overridePath);
            }

            rebindingGroups[index].bindingDisplayNameText.text = GetKeyReadableName(bindingIndex);
            rebindingOperation.Dispose();

            PlayerPrefs.SetString(InputMappingKey(bindingIndex), inputAction.action.bindings[bindingIndex].overridePath);

            rebindingGroups[index].startRebinding.gameObject.SetActive(true);
            rebindingGroups[index].waitingForInputObject.SetActive(false);
            menuController.allowInputChanges = true;
            inputAction.action.Enable();
            inputAction.action.actionMap.Enable();
        }
    }
}