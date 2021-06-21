using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PropHunt.UI.Actions
{
    public class RebindInputButton : MonoBehaviour
    {
        public InputActionReference inputAction = null;
        public Text bindingDisplayNameText = null;
        public Button startRebinding = null;
        public GameObject waitingForInputObject = null;
        public MenuController menuController;

        private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

        private string GetKeyReadableName() =>
            InputControlPath.ToHumanReadableString(
                inputAction.action.bindings[0].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

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
                .WithControlsExcluding("Mouse")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete())
                .Start();
        }

        public void RebindComplete()
        {
            foreach(PlayerInput input in GameObject.FindObjectsOfType<PlayerInput>())
            {
                input.actions.FindAction(inputAction.name).ApplyBindingOverride(0, inputAction.action.bindings[0]);
            }

            bindingDisplayNameText.text = GetKeyReadableName();
            rebindingOperation.Dispose();

            startRebinding.gameObject.SetActive(true);
            waitingForInputObject.SetActive(false);
            menuController.allowInputChanges = true;
            inputAction.action.Enable();
            inputAction.action.actionMap.Enable();
        }
    }
}