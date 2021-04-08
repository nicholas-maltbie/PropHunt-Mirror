using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Highlight an object when a player looks at it
    /// </summary>
    [RequireComponent(typeof(Outline))]
    public class HighlightOnFocus : Focusable
    {
        /// <summary>
        /// Current focused state of the object
        /// </summary>
        private bool focused;

        /// <summary>
        /// Outline object
        /// </summary>
        private Outline outline;

        public void Start()
        {
            this.outline = GetComponent<Outline>();
        }

        public override void Focus(GameObject sender)
        {
            // Set focused to true for this frame
            focused = true;
        }

        public void LateUpdate()
        {
            // Set the current focused state
            outline.enabled = focused;
            // Assume the player looks away unless told otherwise
            focused = false;
        }
    }
}