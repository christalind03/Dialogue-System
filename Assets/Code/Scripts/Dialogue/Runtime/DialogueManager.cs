using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Dialogue.Runtime
{
    /// <summary>
    /// Controls the execution of a compiled runtime <see cref="DialogueGraph"/>.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The reference for the input action used to continue dialogue.")]
        private InputActionReference actionReference;
        
        [SerializeField]
        [Tooltip("The compiled runtime DialogueGraph to execute.")]
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
        /// The input action instance created from <see cref="actionReference"/> for continuing dialogue.
        /// </summary>
        private InputAction inputAction;
        
        /// <summary>
        /// Enables the input action when the object becomes enabled and active.
        /// Additionally, loads and begins dialogue execution when the <see cref="GameObject"/> is initialized.
        /// </summary>
        private void Awake()
        {
            inputAction = actionReference.action;

            LoadDialogue();
            PlayDialogue();
        }

        /// <summary>
        /// Subscribes to the <see cref="inputAction"/>'s <c>performed</c> event to trigger dialogue playback.
        /// </summary>
        private void OnEnable()
        {
            inputAction.performed += PlayDialogue;
        }

        /// <summary>
        /// Unsubscribes from the <see cref="inputAction"/>'s <c>performed</c> event to clean up listeners.
        /// </summary>
        private void OnDisable()
        {
            inputAction.performed -= PlayDialogue;
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
        /// Callback invoked when the <see cref="inputAction"/> is performed.
        /// </summary>
        /// <param name="inputContext">The context information about the <see cref="inputAction"/> trigger.</param>
        private void PlayDialogue(InputAction.CallbackContext inputContext)
        {
            PlayDialogue();
        }
        
        /// <summary>
        /// Begins or continues dialogue execution.
        /// </summary>
        public void PlayDialogue()
        {
            if (inputAction.enabled == false)
            {
                inputAction.Enable();
            }
            
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
        }
        
        /// <summary>
        /// Stops dialogue execution.
        /// </summary>
        public void StopDialogue()
        {
            inputAction.Disable();
            Debug.Log("END DIALOGUE");
        }
    }
}