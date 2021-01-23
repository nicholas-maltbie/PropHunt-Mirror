using NUnit.Framework;
using PropHunt.UI;
using UnityEngine;

namespace Tests.EditMode.UI
{
    /// <summary>
    /// Tests for Menu Controller Tests
    /// </summary>
    [TestFixture]
    public class MenuControllerTests
    {
        /// <summary>
        /// Object to hold menu controller
        /// </summary>
        private GameObject menuControllerObject;

        /// <summary>
        /// Menu controller object
        /// </summary>
        private MenuController menuController;

        /// <summary>
        /// Current screen
        /// </summary>
        private string currentScreen;

        [SetUp]
        public void Setup()
        {
            this.menuControllerObject = new GameObject();
            this.menuController = this.menuControllerObject.AddComponent<MenuController>();

            // Listen to requested screen change events
            UIManager.RequestScreenChange += (object source, RequestScreenChangeEventArgs args) =>
            {
                this.currentScreen = args.newScreen;
            };
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup game object
            GameObject.DestroyImmediate(this.menuControllerObject);
        }

        [Test]
        public void SetScreenTests()
        {
            GameObject holderObject = new GameObject();
            holderObject.name = "Helloworld";
            this.menuController.SetScreen(holderObject);
            Assert.IsTrue(this.currentScreen == "Helloworld");

            this.menuController.SetScreen("NewScreen");
            Assert.IsTrue(this.currentScreen == "NewScreen");

            GameObject.DestroyImmediate(holderObject);
        }
    }
}