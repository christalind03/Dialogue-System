#if UNITY_EDITOR

using System;
using Unity.GraphToolkit.Editor;

namespace Code.Scripts.Dialogue.Editor
{
    /// <summary>
    /// Represents the exit node in a dialogue graph with the option to trigger callbacks.
    /// </summary>
    [Serializable]
    internal class DialogueExit : Node
    {
        /// <summary>
        /// Defines the ports available for this node.
        /// </summary>
        /// <param name="portContext">The context used to define ports for this node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext portContext)
        {
            PortBuilder.CreateInputPort(portContext);
            PortBuilder.CreateCallbackPorts(portContext);
        }
    }
}

#endif