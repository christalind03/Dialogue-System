#if UNITY_EDITOR

using Code.Scripts.Dialogue.Events.Runtime;
using Code.Scripts.Dialogue.Graph.Runtime;
using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

using static Code.Scripts.Dialogue.Graph.Editor.DialoguePorts;

namespace Code.Scripts.Dialogue.Graph.Editor
{
    /// <summary>
    /// Represents the exit node in a dialogue graph with the option to trigger callbacks.
    /// </summary>
    [Serializable]
    internal class DialogueExit : EditorNode
    {
        /// <inheritdoc cref="Instantiate"/>
        public override RuntimeNode Instantiate(Dictionary<INode, int> dialogueMap, EditorNode editorNode)
        {
            var nodeID = dialogueMap[editorNode];
            var nodeEvent = RetrievePortValue<DialogueEvent>(editorNode, Port.Input, Event);

            return new Runtime.DialogueExit(nodeID, nodeEvent);
        }
        
        /// <summary>
        /// Defines the ports available for this node.
        /// </summary>
        /// <param name="portContext">The context used to define ports for this node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext portContext)
        {
            CreateInputPort(portContext);
            CreateEventPorts(portContext);
        }
    }
}

#endif