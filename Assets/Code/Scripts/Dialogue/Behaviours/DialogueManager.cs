using Code.Scripts.Dialogue.Events.Runtime;
using Code.Scripts.Dialogue.Graph.Runtime;
using Code.Scripts.Events.Runtime;
using Code.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Code.Scripts.Dialogue.Behaviours
{
    /// <summary>
    /// Controls the execution of a compiled runtime <see cref="DialogueGraph"/>.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class DialogueManager : MonoBehaviour
    {
        [Header("Input Actions")]
        
        [SerializeField]
        [Tooltip("The reference for the input action used to continue dialogue.")]
        private InputActionReference continueReference;
        
        [SerializeField]
        [Tooltip("The reference for the input action used to select a dialogue option.")]
        private InputActionReference selectReference;
        
        [Header("Runtime References")]
        
        [SerializeField]
        [Tooltip("The AudioSource used to play dialogue voice lines during playback.")]
        private AudioSource dialogueAudio;
        
        [SerializeField]
        [Tooltip("The compiled runtime DialogueGraph to execute.")]
        private DialogueGraph dialogueGraph;
        
        /// <summary>
        /// A runtime lookup table mapping node IDs to their corresponding <see cref="RuntimeNode"/>.
        /// </summary>
        private readonly Dictionary<int, RuntimeNode> dialogueMap = new();
        
        /// <summary>
        /// The <see cref="List{T}"/> of <see cref="DialogueOption"/> available for the current <see cref="DialogueSelection"/> node.
        /// </summary>
        private List<DialogueOption> currentOptions;
        
        /// <summary>
        /// The <see cref="RuntimeNode"/> currently being processed.
        /// </summary>
        private RuntimeNode currentNode;

        /// <summary>
        /// The <see cref="InputAction"/> instance referenced from <see cref="continueReference"/> for continuing dialogue.
        /// </summary>
        private InputAction continueAction;
        
        /// <summary>
        /// The <see cref="InputAction"/> instance referenced from <see cref="selectReference"/> for selecting dialogue options.
        /// </summary>
        private InputAction selectAction;
        
        /// <summary>
        /// Initializes runtime references.
        /// </summary>
        private void Awake()
        {
            dialogueAudio = GetComponent<AudioSource>();
            
            continueAction = continueReference.action;
            selectAction = selectReference.action;
        }

        /// <summary>
        /// Automatically loads and begins dialogue execution when the <see cref="GameObject"/> is initialized.
        /// </summary>
        private void Start()
        {
            if (dialogueGraph is null) return;
            
            LoadDialogue();
            ContinueDialogue();
            
            ToggleContinue(true);
        }

        /// <summary>
        /// Subscribes to the <see cref="continueAction"/>'s <c>performed</c> event to trigger dialogue playback.
        /// </summary>
        private void OnEnable()
        {
            continueAction.performed += ContinueDialogue;
            selectAction.performed += SelectDialogue;
        }

        /// <summary>
        /// Unsubscribes from the <see cref="continueAction"/>'s <c>performed</c> event to clean up listeners.
        /// </summary>
        private void OnDisable()
        {
            continueAction.performed -= ContinueDialogue;
            selectAction.performed -= SelectDialogue;
        }

        /// <summary>
        /// Toggles the <see cref="continueAction"/> to be enabled or disabled.
        /// </summary>
        /// <param name="isEnabled">If <c>true</c>, enables <see cref="continueAction"/>; otherwise, disables it.</param>
        public void ToggleContinue(bool isEnabled)
        {
            if (isEnabled)
            {
                continueAction.Enable();
            }
            else
            {
                continueAction.Disable();
            }
        }

        /// <summary>
        /// Toggles the <see cref="selectAction"/> to be enabled or disabled.
        /// </summary>
        /// <param name="isEnabled">If <c>true</c>, enables <see cref="selectAction"/>; otherwise, disables it.</param>
        public void ToggleSelect(bool isEnabled)
        {
            if (isEnabled)
            {
                selectAction.Enable();
            }
            else
            {
                selectAction.Disable();
            }
        }
        
        /// <summary>
        /// Loads the collection of <see cref="RuntimeNode"/> from a <see cref="DialogueGraph"/> into the internal lookup dictionary and sets the starting node for playback.
        /// </summary>
        /// <param name="targetGraph">The <see cref="DialogueGraph"/> to load.</param>
        public void LoadDialogue(DialogueGraph targetGraph = null)
        {
            dialogueMap.Clear();

            // Replace the current dialogue graph, if provided.
            dialogueGraph = targetGraph ?? dialogueGraph;
            if (dialogueGraph is not null)
            {
                foreach (var dialogueNode in dialogueGraph.NodeRegistry)
                {
                    dialogueMap[dialogueNode.NodeID] = dialogueNode;
                }

                currentNode = dialogueMap[dialogueGraph.EntryID];
            }
            else
            {
                currentNode = null;
            }
        }

        /// <summary>
        /// Callback wrapper for <see cref="ContinueDialogue"/> when <see cref="continueAction"/> is performed.
        /// </summary>
        /// <param name="inputContext">The context information about the <see cref="continueAction"/> trigger.</param>
        private void ContinueDialogue(InputAction.CallbackContext inputContext)
        {                
            ContinueDialogue();
        }
        
        /// <summary>
        /// Handles dialogue option selection via numeric key input.
        /// </summary>
        /// <param name="inputContext">The context information about the <see cref="selectAction"/> trigger.</param>
        private void SelectDialogue(InputAction.CallbackContext inputContext)
        {
            if (inputContext.performed == false) return;
            if (inputContext.control is not KeyControl keyControl) return;
            
            var keyCode = keyControl.name;
            var keySelection = int.Parse(keyCode);
            if (currentOptions.Count <= keySelection) return;
            
            var selectedOption = currentOptions[keySelection];
            
            TriggerEvents(selectedOption.OptionData.Events);
            currentNode = dialogueMap[selectedOption.UpcomingID];
            ContinueDialogue();
            
            ToggleContinue(true);
            ToggleSelect(false);
        }
        
        /// <summary>
        /// Continues dialogue execution.
        /// </summary>
        public void ContinueDialogue()
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
            dialogueAudio.Stop();
            StopAllCoroutines();
            
            switch (currentNode)
            {
                case DialogueExit activeNode:
                    ProcessDialogueExit(activeNode);
                    break;
                
                case DialogueNode activeNode:
                    ProcessDialogueNode(activeNode);
                    break;
                
                case DialogueSelection activeNode:
                    // Selection determines the upcoming node at runtime; return to skip automatic advancement.
                    ProcessDialogueSelection(activeNode);
                    return;
                
                default:
                    throw new NotSupportedException($"{currentNode.GetType().Name} is not supported.");
            }

            currentNode = dialogueMap.GetValueOrDefault(currentNode.UpcomingID);
        }
        
        /// <summary>
        /// Processes a <see cref="DialogueExit"/> by triggering its events and terminating the dialogue flow.
        /// </summary>
        /// <param name="activeNode">The <see cref="DialogueNode"/> to process.</param>
        private void ProcessDialogueExit(DialogueExit activeNode)
        {
            TriggerEvents(activeNode.Events);
            StopDialogue();
        }
        
        /// <summary>
        /// Processes a <see cref="DialogueNode"/> by triggering its events, its associated audio clip (if available), and displaying the actor's lines.
        /// </summary>
        /// <param name="activeNode">The <see cref="DialogueNode"/> to process.</param>
        private void ProcessDialogueNode(DialogueNode activeNode)
        {
            TriggerEvents(activeNode.Events);
            if (activeNode.Audio is not null && dialogueAudio is not null)
            {
                dialogueAudio.clip = activeNode.Audio;
                dialogueAudio.Play();
            }
                    
            Debug.Log($"{activeNode.Actor}: {activeNode.Text}");
        }
        
        /// <summary>
        /// Processes a <see cref="DialogueSelection"/> by displaying the available options and switching the input to selection mode.
        /// </summary>
        /// <param name="activeNode">The <see cref="DialogueSelection"/> to process.</param>
        private void ProcessDialogueSelection(DialogueSelection activeNode)
        {
            var optionDisplay = "<b>EXPAND FOR DIALOGUE OPTIONS</b>\n";
            var optionIndex = 0;

            currentOptions = activeNode.DialogueOptions;
            foreach (var dialogueOption in activeNode.DialogueOptions)
            {
                optionDisplay += "\n";
                optionDisplay += $"<b>Option {optionIndex}</b>: {dialogueOption.OptionData.Text}";
                optionIndex++;
            }
                    
            Debug.Log(optionDisplay);
                    
            ToggleContinue(false);
            ToggleSelect(true);
        }
        
        /// <summary>
        /// Triggers all event channels associated with a given <see cref="DialogueEvent"/>, dispatching each as a delayed <see cref="Coroutine"/>.
        /// </summary>
        /// <param name="targetEvent">The <see cref="DialogueEvent"/> whose channels will be triggered.</param>
        private void TriggerEvents(DialogueEvent targetEvent)
        {
            targetEvent.EventChannels.ForEach(eventChannel => StartCoroutine(DelayEvent(eventChannel)));
        }
        
        /// <summary>
        /// Waits for the specified delay duration in the given <see cref="DelayEvent"/> before emitting the <see cref="DelayEvent.EventChannel"/>.
        /// </summary>
        /// <param name="delayEvent">The <see cref="DelayEvent"/> containing the delay duration and <see cref="EventChannel"/> to emit.</param>
        /// <returns>An <see cref="IEnumerator"/> that can be used by Unity's coroutine system.</returns>
        private static IEnumerator DelayEvent(DelayEvent delayEvent)
        {
            yield return new WaitForSeconds(delayEvent.DelayDuration);
            delayEvent.EventChannel.Emit();
        }
        
        /// <summary>
        /// Stops dialogue execution.
        /// </summary>
        public void StopDialogue()
        {
            dialogueAudio.Stop();
            ToggleContinue(false);

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
                (continueReference is null, $"<b>{nameof(continueReference)}</b> is not assigned."),
                (dialogueAudio is null, $"<b>{nameof(dialogueAudio)}</b> is not assigned."),
                (dialogueGraph is null, $"<b>{nameof(dialogueGraph)}</b> is not assigned.")
            );
        }
        
        #endif
    }
}