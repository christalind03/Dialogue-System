using System;
using UnityEngine;

namespace Code.Scripts.Dialogue.Graph.Runtime
{
    /// <summary>
    /// Represents a single, selectable option within a <see cref="DialogueSelection"/> node at runtime.
    /// Holds the option's data and its reference to the upcoming node.
    /// </summary>
    [Serializable]
    public class DialogueOption : RuntimeNode
    {
        /// <summary>
        /// The data associated with this dialogue option, including its text and events.
        /// </summary>
        [SerializeField]
        private OptionData optionData;
        
        /// <inheritdoc cref="optionData"/>
        public OptionData OptionData => optionData;
        
        /// <summary>
        /// Constructs a runtime <see cref="DialogueOption"/>.
        /// </summary>
        /// <param name="nodeID">The unique identifier for this node within the <see cref="DialogueGraph"/>.</param>
        /// <param name="optionData">The <see cref="OptionData"/> associated with this <see cref="DialogueOption"/>.</param>
        /// <param name="upcomingID">The ID of the next node in the dialogue sequence.</param>
        public DialogueOption(int nodeID, OptionData optionData, int upcomingID) : base(nodeID, upcomingID)
        {
            this.optionData = optionData;
        }
    }
}