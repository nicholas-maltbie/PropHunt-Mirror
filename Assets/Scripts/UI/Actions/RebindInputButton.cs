using System.Collections.Generic;
using Mirror;
using PropHunt.Character;
using PropHunt.Game.Communication;
using PropHunt.Utils;
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
        private List<PlayerInput> pausedPlayerInput = new List<PlayerInput>();

        private string GetKeyReadableName() =>
            InputControlPath.ToHumanReadableString(
                inputAction.action.bindings[inputAction.action.GetBindingIndexForControl(inputAction.action.controls[0])].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

        public void Start()
        {
            startRebinding.onClick.AddListener(() => StartRebinding());
            bindingDisplayNameText.text = GetKeyReadableName();
        }

        public void StartRebinding()
        {
            pausedPlayerInput.Clear();
            // Get the local player controller in the scene (if one exists) and set it to UI mode

            startRebinding.gameObject.SetActive(false);
            waitingForInputObject.SetActive(true);
            menuController.allowInputChanges = false;

            rebindingOperation = inputAction.action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .OnMatchWaitForAnother(0.1f)
                .WithTimeout(5.0f)
                .OnComplete(operation => RebindComplete())
                .Start();
        }

        public void RebindComplete()
        {
            bindingDisplayNameText.text = GetKeyReadableName();
            rebindingOperation.Dispose();

            startRebinding.gameObject.SetActive(true);
            waitingForInputObject.SetActive(false);
            menuController.allowInputChanges = true;
        }
    }
}