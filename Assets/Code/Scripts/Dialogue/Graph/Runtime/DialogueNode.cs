using Code.Scripts.Dialogue.Events.Runtime;
using System;
using UnityEngine;

namespace Code.Scripts.Dialogue.Graph.Runtime
{
    /// <summary>
    /// A runtime representation of a <see cref="Editor.DialogueNode"/>.
    /// </summary>
    [Serializable]
    public class DialogueNode : RuntimeNode
    {
        /// <summary>
        /// The actor speaking this dialogue line.
        /// </summary>
        [SerializeField]
        private string nodeActor;
        
        /// <summary>
        /// The <see cref="AudioClip"/> associated with this dialogue line.
        /// </summary>
        [SerializeField]
        private AudioClip nodeAudio;
        
        /// <summary>
        /// The content of this dialogue line.
        /// </summary>
        [SerializeField]
        private string nodeText;

        /// <summary>
        /// The <see cref="DialogueEvent"/> associated with this dialogue line.
        /// </summary>
        [SerializeField]
        private DialogueEvent nodeEvent;
        
        /// <inheritdoc cref="nodeActor"/>
        public string Actor => nodeActor;
        
        /// <inheritdoc cref="nodeAudio"/>
        public AudioClip Audio => nodeAudio;
        
        /// <inheritdoc cref="nodeText"/>
        public string Text => nodeText;
        
        /// <inheritdoc cref="nodeEvent"/>
        public DialogueEvent Events => nodeEvent;
        
        /// <summary>
        /// Constructs a runtime <see cref="DialogueNode"/>.
        /// </summary>
        /// <param name="nodeID">The unique identifier for this node within the <see cref="DialogueGraph"/>.</param>
        /// <param name="nodeActor">The actor speaking this dialogue line.</param>
        /// <param name="nodeAudio">The <see cref="AudioClip"/> associated with this dialogue line.</param>
        /// <param name="nodeText">The content of this dialogue line.</param>
        /// <param name="nodeEvent">The <see cref="DialogueEvent"/> associated with this dialogue line.</param>
        /// <param name="upcomingID">The ID of the next node in the dialogue sequence.</param>
        public DialogueNode(int nodeID, string nodeActor, AudioClip nodeAudio, string nodeText, DialogueEvent nodeEvent, int upcomingID) : base(nodeID, upcomingID)
        {
            this.nodeActor = nodeActor;
            this.nodeAudio = nodeAudio;
            this.nodeText = nodeText;
            this.nodeEvent = nodeEvent;
        }
    }
}