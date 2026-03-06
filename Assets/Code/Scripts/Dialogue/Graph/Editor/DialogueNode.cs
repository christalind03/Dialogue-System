#if UNITY_EDITOR

using Code.Scripts.Dialogue.Events.Runtime;
using Code.Scripts.Dialogue.Graph.Runtime;
using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;

using static Code.Scripts.Dialogue.Graph.Editor.DialoguePorts;

namespace Code.Scripts.Dialogue.Graph.Editor
{
    /// <summary>
    /// Represents a single line of dialogue within a dialogue graph.
    /// </summary>
    [Serializable]
    internal class DialogueNode : EditorNode
    {
        /// <inheritdoc cref="Instantiate"/>
        public override RuntimeNode Instantiate(Dictionary<INode, int> dialogueMap, EditorNode editorNode)
        {
            var nodeID = dialogueMap[editorNode];
            var nodeActor = RetrievePortValue<string>(editorNode, Port.Input, Actor);
            var nodeAudio = RetrievePortValue<AudioClip>(editorNode, Port.Input, Audio);
            var nodeText = RetrievePortValue<string>(editorNode, Port.Input, Text);
            var nodeEvent = RetrievePortValue<DialogueEvent>(editorNode, Port.Input, DialoguePorts.Event);
            var outputConnection = editorNode.GetOutputPortByName(Output).firstConnectedPort;
            var outputNode = outputConnection.GetNode();
            var outputID = dialogueMap[outputNode];
            
            return new Runtime.DialogueNode(nodeID, nodeActor, nodeAudio, nodeText, nodeEvent, outputID);
        }
        
        /// <summary>
        /// Defines the ports available for this node.
        /// </summary>
        /// <param name="portContext">The context used to define ports for this node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext portContext)
        {
            CreateInputPort(portContext);
            
            // TODO: Use DialogueActor instead of a string when GraphToolkit provides full variable support
            portContext.AddInputPort<string>(Actor).Build();
            portContext.AddInputPort<AudioClip>(Audio).Build();
            portContext.AddInputPort<string>(Text).Build();
            CreateEventPorts(portContext);
            CreateOutputPort(portContext);
        }
    }
}

#endif