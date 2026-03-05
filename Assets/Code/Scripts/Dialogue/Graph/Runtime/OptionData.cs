using Code.Scripts.Dialogue.Events.Runtime;
using System;
using UnityEngine;

namespace Code.Scripts.Dialogue.Graph.Runtime
{
    /// <summary>
    /// Holds the data associated with a single <see cref="DialogueOption"/>, including its text to display and events to trigger upon selection.
    /// </summary>
    [Serializable]
    public class OptionData
    {
        /// <summary>
        /// The content displayed for this dialogue option.
        /// </summary>
        [SerializeField]
        private string optionText;
        
        /// <summary>
        /// The <see cref="DialogueEvent"/> associated with this dialogue option.
        /// </summary>
        [SerializeField]
        private DialogueEvent optionEvents;
        
        /// <inheritdoc cref="optionText" />
        public string Text => optionText;
        
        /// <inheritdoc cref="optionEvents" />
        public DialogueEvent Events => optionEvents;
    }
}