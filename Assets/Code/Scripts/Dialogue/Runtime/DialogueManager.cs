using Code.Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Dialogue.Runtime
{
    /// <summary>
    /// Controls the execution of a compiled runtime <see cref="DialogueGraph"/>.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The reference for the input action used to continue dialogue.")]
        private InputActionReference actionReference;
        
        [SerializeField]
        [Tooltip("The AudioSource used to play dialogue voice lines during playback.")]
        private AudioSource dialogueAudio;
        
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
        /// Initializes runtime references.
        /// </summary>
        private void Awake()
        {
            dialogueAudio = GetComponent<AudioSource>();
            inputAction = actionReference.action;
        }

        /// <summary>
        /// Automatically loads and begins dialogue execution when the <see cref="GameObject"/> is initialized.
        /// </summary>
        private void Start()
        {
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
            if (-1 < currentNode?.NodeID)
            {
                ProcessNode();
            }
            else
            {
                StopDialogue();
            }
            
            if (inputAction.enabled == false)
            {
                inputAction.Enable();
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
            dialogueAudio.Stop();
            
            switch (currentNode)
            {
                case DialogueNode activeNode:
                    if (activeNode.Audio is not null && dialogueAudio is not null)
                    {
                        dialogueAudio.clip = activeNode.Audio;
                        dialogueAudio.Play();
                    }
                    
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
        
        #if UNITY_EDITOR
        
        /// <summary>
        /// Validates required references in the Unity Editor.
        /// If validation fails while in Play mode, the editor will immediately exit Play mode to prevent further issues.
        /// </summary>
        private void OnValidate()
        {
            ObjectValidator.AssertConditions(
                this,
                (actionReference is null, $"<b>{nameof(actionReference)}</b> is not assigned."),
                (dialogueAudio is null, $"<b>{nameof(dialogueAudio)}</b> is not assigned."),
                (dialogueGraph is null, $"<b>{nameof(dialogueGraph)}</b> is not assigned.")
            );
        }
        
        #endif
    }
}