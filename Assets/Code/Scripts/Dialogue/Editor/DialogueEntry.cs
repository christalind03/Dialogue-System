#if UNITY_EDITOR

using System;
using Unity.GraphToolkit.Editor;

namespace Code.Scripts.Dialogue.Editor
{
    /// <summary>
    /// Represents the starting point in a dialogue graph.
    /// </summary>
    [Serializable]
    internal class DialogueEntry : Node
    {
        /// <summary>
        /// Defines the ports available for this node.
        /// </summary>
        /// <param name="portContext">The context used to define ports for this node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext portContext)
        {
            PortBuilder.CreateOutputPort(portContext);
        }
    }
}

#endif