using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Dialogue.Graph.Runtime
{
    /// <summary>
    /// Represents a selection node in the <see cref="DialogueGraph"/> at runtime.
    /// Holds a collection of <see cref="DialogueOption"/> instances that the player can choose from.
    /// </summary>
    [Serializable]
    public class DialogueSelection : RuntimeNode
    {
        /// <summary>
        /// The <see cref="List{T}"/> of <see cref="DialogueOption"/> available for selection at this node.
        /// </summary>
        [SerializeField]
        private List<DialogueOption> dialogueOptions;
        
        /// <inheritdoc cref="dialogueOptions"/>
        public List<DialogueOption> DialogueOptions => dialogueOptions;
        
        /// <summary>
        /// Constructs a runtime <see cref="DialogueSelection"/>.
        /// </summary>
        /// <param name="nodeID">The unique identifier for this node within the <see cref="DialogueGraph"/>.</param>
        /// <param name="upcomingID">The ID of the next node in the dialogue sequence.</param>
        public DialogueSelection(int nodeID, int upcomingID = -1) : base(nodeID, upcomingID)
        {
            dialogueOptions = new List<DialogueOption>();
        }
    }
}