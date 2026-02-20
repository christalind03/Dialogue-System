using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Dialogue.Runtime
{
    /// <summary>
    /// Controls the execution of a compiled runtime <see cref="DialogueGraph"/>.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        /// <summary>
        /// The compiled runtime <see cref="DialogueGraph"/> to execute.
        /// </summary>
        [SerializeField]
        private DialogueGraph dialogueGraph;

        /// <summary>
        /// A runtime lookup table mapping node IDs to their corresponding <see cref="RuntimeNode"/>.
        /// </summary>
        private readonly Dictionary<int, RuntimeNode> dialogueNodes = new();
        
        /// <summary>
        /// The <see cref="RuntimeNode"/> currently being processed.
        /// </summary>
        private RuntimeNode currentNode;

        /// <summary>
        /// Automatically loads and begins dialogue execution when the <see cref="GameObject"/> is initialized.
        /// This is for debugging purposes only.
        /// </summary>
        private void Awake()
        {
            LoadDialogue();
            PlayDialogue();
        }

        /// <summary>
        /// Loads the collection of <see cref="RuntimeNode"/> from a <see cref="DialogueGraph"/> into the internal lookup dictionary and sets the starting node for playback.
        /// </summary>
        /// <param name="targetGraph">The <see cref="DialogueGraph"/> to load.</param>
        public void LoadDialogue(DialogueGraph targetGraph = null)
        {
            dialogueNodes.Clear();

            // Replace the current dialogue graph, if provided.
            dialogueGraph = targetGraph ?? dialogueGraph;
            if (dialogueGraph is not null)
            {
                foreach (var dialogueNode in dialogueGraph.NodeRegistry)
                {
                    dialogueNodes[dialogueNode.NodeID] = dialogueNode;
                }

                currentNode = dialogueNodes[dialogueGraph.EntryID];
            }
            else
            {
                currentNode = null;
            }
        }

        /// <summary>
        /// Begins or continues dialogue execution.
        /// </summary>
        public void PlayDialogue()
        {
            if (-1 < currentNode?.NodeID)
            {
                ProcessNode();
            }
            else
            {
                StopDialogue();
            }
        }

        /// <summary>
        /// Processes the current node based on its runtime type.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown if the node type is not supported by the runtime processor.
        /// </exception>
        private void ProcessNode()
        {
            switch (currentNode)
            {
                case DialogueNode activeNode:
                    Debug.Log($"{activeNode.Actor}: {activeNode.Text}");
                    break;
                
                default:
                    throw new NotSupportedException($"{currentNode.GetType().Name} is not supported.");
            }

            currentNode = dialogueNodes.GetValueOrDefault(currentNode.UpcomingID);
            
            // TODO: Remove this method call once there's a more reliable way to continue dialogue
            PlayDialogue();
        }
        
        /// <summary>
        /// Stops dialogue execution.
        /// </summary>
        public void StopDialogue()
        {
            Debug.Log("END DIALOGUE");
        }
    }
}