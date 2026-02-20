using System;
using UnityEngine;

namespace Code.Scripts.Dialogue.Runtime
{
    /// <summary>
    /// The base class for all runtime <see cref="DialogueGraph"/> nodes.
    /// </summary>
    [Serializable]
    public abstract class RuntimeNode
    {
        /// <summary>
        /// The unique identifier for this node within the <see cref="DialogueGraph"/>.
        /// </summary>
        [SerializeField]
        private int nodeID;
        
        /// <summary>
        /// The ID of the next node in the dialogue sequence.
        /// </summary>
        [SerializeField]
        private int upcomingID;
        
        /// <summary>
        /// Retrieves the unique ID of this node.
        /// </summary>
        public int NodeID => nodeID;
        
        /// <summary>
        /// Retrieves the ID of the upcoming node to process.
        /// </summary>
        public int UpcomingID => upcomingID;

        /// <summary>
        /// Initializes a <see cref="RuntimeNode"/>.
        /// </summary>
        /// <param name="nodeID">The unique identifier for this node within the <see cref="DialogueGraph"/>.</param>
        /// <param name="upcomingID">The ID of the next node in the dialogue sequence.</param>
        protected RuntimeNode(int nodeID, int upcomingID)
        {
            this.nodeID = nodeID;
            this.upcomingID = upcomingID;
        }
    }
}