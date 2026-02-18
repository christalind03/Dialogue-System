#if UNITY_EDITOR

using Unity.GraphToolkit.Editor;
using UnityEngine.Events;

namespace Code.Scripts.Dialogue.Editor
{
    /// <summary>
    /// Provides reusable methods for defining common dialogue-related ports on nodes.
    /// </summary>
    internal static class PortBuilder
    {
        /// <summary>
        /// Registers callback-related input ports on a node.
        /// </summary>
        /// <param name="portContext">The context used to define ports for the node.</param>
        public static void CreateCallbackPorts(Node.IPortDefinitionContext portContext)
        {
            // TODO: Separate "OnDelay", "OnEnter", and "OnExit" callbacks
            portContext.AddInputPort<UnityEvent>("Callbacks").Build();
        }
        
        /// <summary>
        /// Registers the standard input port for a node.
        /// </summary>
        /// <param name="portContext">The context used to define ports for the node.</param>
        public static void CreateInputPort(Node.IPortDefinitionContext portContext)
        {
            portContext.AddInputPort<INode>("Input").Build();
        }

        /// <summary>
        /// Registers the standard output port for a node.
        /// </summary>
        /// <param name="portContext">The context used to define ports for the node.</param>
        public static void CreateOutputPort(Node.IPortDefinitionContext portContext)
        {
            portContext.AddOutputPort<INode>("Output").Build();
        }
    }
}

#endif