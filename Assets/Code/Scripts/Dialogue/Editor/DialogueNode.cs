#if UNITY_EDITOR

using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Code.Scripts.Dialogue.Editor
{
    /// <summary>
    /// Represents a single line of dialogue within a dialogue graph.
    /// </summary>
    [Serializable]
    internal class DialogueNode : Node
    {
        /// <summary>
        /// Defines the ports available for this node.
        /// </summary>
        /// <param name="portContext">The context used to define ports for this node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext portContext)
        {
            DialoguePorts.CreateInputPort(portContext);
            
            // TODO: Use DialogueActor instead of a string when GraphToolkit provides full variable support
            portContext.AddInputPort<string>(DialoguePorts.Actor).Build();
            portContext.AddInputPort<AudioClip>(DialoguePorts.Audio).Build();
            portContext.AddInputPort<string>(DialoguePorts.Text).Build();
            DialoguePorts.CreateCallbackPorts(portContext);
            DialoguePorts.CreateOutputPort(portContext);
        }
    }
}

#endif