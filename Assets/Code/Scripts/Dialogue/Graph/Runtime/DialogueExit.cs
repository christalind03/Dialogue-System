using Code.Scripts.Dialogue.Events.Runtime;
using System;
using UnityEngine;

namespace Code.Scripts.Dialogue.Graph.Runtime
{
    /// <summary>
    /// Represents a runtime representation of a <see cref="Editor.DialogueExit"/>.
    /// </summary>
    [Serializable]
    public class DialogueExit : RuntimeNode
    {
        /// <summary>
        /// The <see cref="DialogueEvent"/> associated with the end of the <see cref="DialogueGraph"/>.
        /// </summary>
        [SerializeField]
        private DialogueEvent nodeEvent;

        /// <inheritdoc cref="nodeEvent"/>
        public DialogueEvent Events => nodeEvent;
        
        /// <summary>
        /// Constructs a runtime <see cref="DialogueExit"/>.
        /// </summary>
        /// <param name="nodeID">The unique identifier for this node within the <see cref="DialogueGraph"/>.</param>
        /// <param name="nodeEvent">The <see cref="DialogueEvent"/> associated with this dialogue line.</param>
        /// <param name="upcomingID">The ID of the next node in the dialogue sequence.</param>
        public DialogueExit(int nodeID, DialogueEvent nodeEvent, int upcomingID = -1) : base(nodeID, upcomingID)
        {
            this.nodeEvent = nodeEvent;
        }
    }
}