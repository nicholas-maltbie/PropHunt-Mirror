using PropHunt.Utils;
using QuickOutline;
using UnityEngine;

namespace PropHunt.Environment
{
    /// <summary>
    /// Highlight an object when a player looks at it
    /// </summary>
    public class HighlightOnFocus : Focusable
    {
        /// <summary>
        /// Previous state
        /// </summary>
        protected bool previousFocused;

        /// <summary>
        /// Current focused state of the object
        /// </summary>
        protected bool focused;

        /// <summary>
        /// Outline object
        /// </summary>
        protected Outline outline;

        /// <summary>
        /// Mode of outline for this object
        /// </summary>
        public Outline.Mode selectedMode;

        public void Start()
        {
            this.outline = GetComponent<Outline>();
            MaterialUtils.RecursiveSetFloatProperty(gameObject, "_EmissionActive", 0);
        }

        public override void Focus(GameObject sender)
        {
            // Set focused to true for this frame
            focused = true;
        }

        public void Update()
        {
            if (previousFocused != focused)
            {
                // Set the current focused state
                // outline.OutlineMode = focused ? this.selectedMode : Outline.Mode.Disabled;
                MaterialUtils.RecursiveSetFloatProperty(gameObject, "_EmissionActive", focused ? 1 : 0);
            }
            previousFocused = focused;
            // Assume the player looks away unless told otherwise
            focused = false;
        }
    }
}