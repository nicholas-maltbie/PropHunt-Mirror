using System.Collections;
using NUnit.Framework;
using PropHunt.UI;
using PropHunt.UI.Actions;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests.EditMode.UI.Actions
{
    [TestFixture]
    public class RebindInputButtonTests
    {
        RebindInputButton rebinding;
        Keyboard keyboard;
        InputActionAsset inputActionAsset;

        [SetUp]
        public void SetUp()
        {
#if UNITY_EDITOR
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
#endif

            rebinding = GameObject.Instantiate(new GameObject()).AddComponent<RebindInputButton>();

            // Create a sample player input to override
            keyboard = InputSystem.AddDevice<Keyboard>();
            PlayerInput input = rebinding.gameObject.AddComponent<PlayerInput>();
            inputActionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
            InputAction testAction = inputActionAsset.AddActionMap("testMap").AddAction("testAction", InputActionType.Button, keyboard.qKey.path, interactions: "Hold");
            input.actions = inputActionAsset;

            // Setup rebinding object
            rebinding.inputAction = InputActionReference.Create(testAction);
            rebinding.menuController = rebinding.gameObject.AddComponent<MenuController>();
            rebinding.bindingDisplayNameText = rebinding.gameObject.AddComponent<Text>();
            rebinding.startRebinding = new GameObject().AddComponent<Button>();
            rebinding.waitingForInputObject = new GameObject();
            rebinding.startRebinding.transform.parent = rebinding.transform;
            rebinding.waitingForInputObject.transform.parent = rebinding.transform;

            // Save a sample rebinding information
            PlayerPrefs.SetString(rebinding.InputMappingKey, keyboard.eKey.path);

            // Test by reading the settings
            rebinding.Awake();
            rebinding.Start();

            rebinding.gameObject.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup
            GameObject.DestroyImmediate(rebinding);

            // Remove rebinding override
            InputSystem.RemoveDevice(keyboard);
            PlayerPrefs.DeleteKey(rebinding.InputMappingKey);

            ScriptableObject.DestroyImmediate(inputActionAsset);
        }

        [Test]
        public void TestRebindInputSettings()
        {
            // Start a test rebinding
            rebinding.startRebinding.onClick?.Invoke();

            // End the test rebinding
            rebinding.rebindingOperation.Complete();
        }
    }
}