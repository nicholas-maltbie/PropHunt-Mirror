using NUnit.Framework;
using PropHunt.UI;
using UnityEngine;

namespace Tests.EditMode.UI
{
    /// <summary>
    /// Tests for various UI Actions such as connect, disconnect, quit game actions
    /// </summary>
    [TestFixture]
    public class UIActionTests
    {
        private GameObject uiHolder;

        [SetUp]
        public void Setup()
        {
            this.uiHolder = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(this.uiHolder);
        }

        [Test]
        public void QuitGameActionTests()
        {
            this.uiHolder.AddComponent<QuitGameAction>();
            QuitGameAction action = this.uiHolder.GetComponent<QuitGameAction>();
            // Just call the method
            action.QuitGame();
        }
    }
}