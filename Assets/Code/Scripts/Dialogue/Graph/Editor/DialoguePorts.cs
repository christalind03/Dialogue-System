#if UNITY_EDITOR

using Code.Scripts.Dialogue.Events.Runtime;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Code.Scripts.Dialogue.Graph.Editor
{
    /// <summary>
    /// Provides reusable strings and methods for defining common dialogue-related ports on nodes.
    /// </summary>
    internal static class DialoguePorts
    {
        internal enum Port
        {
            Input,
            Output,
        }
        
        /// <summary>
        /// Defines the actor for a dialogue node.
        /// <para>Expected Type: <see cref="string"/></para>
        /// </summary>
        public const string Actor = "Actor";
        
        /// <summary>
        /// Defines the audio clip for a dialogue node.
        /// <para>Expected Type: <see cref="AudioClip"/></para>
        /// </summary>
        public const string Audio = "Audio";
        
        /// <summary>
        /// Defines the actions or callbacks for a dialogue node.
        /// <para>Expected Type: <see cref="DialogueEvent"/></para>
        /// </summary>
        public const string Event = "Events";
        
        /// <summary>
        /// Defines the connection to the previous dialogue node.
        /// <para>Expected Type: <see cref="INode"/></para>
        /// </summary>
        public const string Input = "Input";
        
        /// <summary>
        /// Defines the connection to the next dialogue node.
        /// <para>Expected Type: <see cref="INode"/></para>
        /// </summary>
        public const string Output = "Output";
        
        /// <summary>
        /// Defines the text content for an actor.
        /// <para>Expected Type: <see cref="string"/></para>
        /// </summary>
        public const string Text = "Text";
        
        /// <summary>
        /// Registers callback-related input ports on a node.
        /// </summary>
        /// <param name="portContext">The context used to define ports for the node.</param>
        public static void CreateEventPorts(Node.IPortDefinitionContext portContext)
        {
            portContext.AddInputPort<DialogueEvent>(Event).Build();
        }
        
        /// <summary>
        /// Registers the standard input port for a node.
        /// </summary>
        /// <param name="portContext">The context used to define ports for the node.</param>
        public static void CreateInputPort(Node.IPortDefinitionContext portContext)
        {
            portContext.AddInputPort(Input).Build();
        }

        /// <summary>
        /// Registers the standard output port for a node.
        /// </summary>
        /// <param name="portContext">The context used to define ports for the node.</param>
        public static void CreateOutputPort(Node.IPortDefinitionContext portContext)
        {
            portContext.AddOutputPort(Output).Build();
        }
        
        /// <summary>
        /// Retrieves the value from a specific port.
        /// </summary>
        /// <param name="activeNode">The <see cref="INode"/> to retrieve the port value from.</param>
        /// <param name="portDirection">The <see cref="Port"/> indicating whether to retrieve an input or output port.</param>
        /// <param name="portLabel">The label of the port to retrieve the value from.</param>
        /// <typeparam name="T">The expected value type.</typeparam>
        /// <returns>The resolved value, or default(T) if none exists.</returns>
        public static T RetrievePortValue<T>(INode activeNode, Port portDirection, string portLabel)
        {
            var activePort = portDirection == Port.Input
                ? activeNode.GetInputPortByName(portLabel)
                : activeNode.GetOutputPortByName(portLabel);
            
            return RetrievePortValue<T>(activePort);
        }
        
        /// <summary>
        /// Retrieves the value from a specific port.
        /// </summary>
        /// <param name="activePort">The input <see cref="IPort"/> to retrieve the value from.</param>
        /// <typeparam name="T">The expected value type.</typeparam>
        /// <returns>The resolved value, or default(T) if none exists.</returns>
        public static T RetrievePortValue<T>(IPort activePort)
        {
            if (activePort is null) return default;
            if (activePort.isConnected)
            {
                if (activePort.firstConnectedPort.GetNode() is IVariableNode currentNode)
                {
                    currentNode.variable.TryGetDefaultValue(out T currentValue);
                    return currentValue;
                }
            }

            activePort.TryGetValue(out T defaultValue);
            return defaultValue;
        }
    }
}

#endif