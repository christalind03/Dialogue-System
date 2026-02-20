using System;
using UnityEngine;

namespace Code.Scripts.Dialogue.Runtime
{
    /// <summary>
    /// A runtime representation of a <see cref="Editor.DialogueNode"/>.
    /// </summary>
    [Serializable]
    public class DialogueNode : RuntimeNode
    {
        /// <summary>
        /// The name or identifier of the actor speaking.
        /// </summary>
        [SerializeField]
        private string nodeActor;
        
        /// <summary>
        /// The audio clip to play when this dialogue line is processed.
        /// May be null if no audio is associated with this node.
        /// </summary>
        [SerializeField]
        private AudioClip nodeAudio;
        
        /// <summary>
        /// The text content displayed or spoken during this node.
        /// </summary>
        [SerializeField]
        private string nodeText;

        /// <summary>
        /// Retrieves the actor speaking.
        /// </summary>
        public string Actor => nodeActor;
        
        /// <summary>
        /// Retrieves the audio clip associated with this line.
        /// </summary>
        public AudioClip Audio => nodeAudio;
        
        /// <summary>
        /// Retrieves the text content.
        /// </summary>
        public string Text => nodeText;
        
        /// <summary>
        /// Constructs a runtime <see cref="DialogueNode"/>.
        /// </summary>
        /// <param name="nodeID">The unique identifier for this node within the <see cref="DialogueGraph"/>.</param>
        /// <param name="nodeActor">The actor speaking this dialogue line.</param>
        /// <param name="nodeAudio">The audio clip to associate with this dialogue line.</param>
        /// <param name="nodeText">The text content of this dialogue line.</param>
        /// <param name="upcomingID">The ID of the next node in the dialogue sequence.</param>
        public DialogueNode(int nodeID, string nodeActor, AudioClip nodeAudio, string nodeText, int upcomingID) : base(nodeID, upcomingID)
        {
            this.nodeActor = nodeActor;
            this.nodeAudio = nodeAudio;
            this.nodeText = nodeText;
        }
    }
}