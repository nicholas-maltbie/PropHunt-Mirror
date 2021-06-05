using NUnit.Framework;
using PropHunt.UI.Actions;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.EditMode.UI.Actions
{
    [TestFixture]
    public class ChangeScreenActionsTests
    {
        ChangeScreenActions changeScreen;
        Dropdown windowedDropdown;
        Dropdown resolutionDropdown;
        Dropdown displayDropdown;

        [SetUp]
        public void SetUp()
        {
            GameObject go = new GameObject();
            ChangeScreenActions.setupDisplay = false;
            changeScreen = go.AddComponent<ChangeScreenActions>();

            // Setup dropdowns
            changeScreen.displayDropdown = new GameObject().AddComponent<Dropdown>();
            changeScreen.windowedDropdown = new GameObject().AddComponent<Dropdown>();
            changeScreen.resolutionDropdown = new GameObject().AddComponent<Dropdown>();
            changeScreen.vsyncToggle = new GameObject().AddComponent<Toggle>();

            // Attach dropdons to objects
            changeScreen.displayDropdown.transform.parent = changeScreen.transform;
            changeScreen.windowedDropdown.transform.parent = changeScreen.transform;
            changeScreen.resolutionDropdown.transform.parent = changeScreen.transform;
            changeScreen.vsyncToggle.transform.parent = changeScreen.transform;

            // Setup change screne object
            changeScreen.Awake();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(changeScreen.gameObject);
        }

        [Test]
        public void ModifyScreenSettings()
        {
            // Simulate selecting an option from each control
            changeScreen.windowedDropdown.onValueChanged?.Invoke(0);
            changeScreen.displayDropdown.onValueChanged?.Invoke(0);
            changeScreen.resolutionDropdown.onValueChanged?.Invoke(0);
            changeScreen.vsyncToggle.onValueChanged?.Invoke(false);
        }

        [Test]
        public void TestFullScreenLookup()
        {
            // Simulate translations of each setting
            ChangeScreenActions.GetFullScreenModeDropdownIndex(FullScreenMode.ExclusiveFullScreen);
            ChangeScreenActions.GetFullScreenModeDropdownIndex(FullScreenMode.FullScreenWindow);
            ChangeScreenActions.GetFullScreenModeDropdownIndex(FullScreenMode.MaximizedWindow);
            ChangeScreenActions.GetFullScreenModeDropdownIndex(FullScreenMode.Windowed);

            Assert.IsTrue(ChangeScreenActions.GetFullScreenMode(ChangeScreenActions.fullScreenModeName)
                == FullScreenMode.ExclusiveFullScreen);
            Assert.IsTrue(ChangeScreenActions.GetFullScreenMode(ChangeScreenActions.windowedModeName)
                == FullScreenMode.Windowed);
            Assert.IsTrue(ChangeScreenActions.GetFullScreenMode(ChangeScreenActions.borderlessWindowModeName)
                == FullScreenMode.FullScreenWindow);
            Assert.IsTrue(ChangeScreenActions.GetFullScreenMode("other")
                == FullScreenMode.ExclusiveFullScreen);
        }
    }
}