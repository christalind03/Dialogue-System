#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace Code.Scripts.Dialogue.Editor
{
    /// <summary>
    /// Defines the asset type that stores and serializes a dialogue graph.
    /// This graph serves as the root container for all dialogue nodes and connections, represented as a custom Unity asset.
    /// </summary>
    [Graph(AssetExtension)]
    [Serializable]
    internal class DialogueGraph : Graph
    {
        /// <summary>
        /// The file extension used for dialogue graph assets.
        /// </summary>
        internal const string AssetExtension = "dialogue";

        /// <summary>
        /// Allows the user to create a new dialogue graph asset from the project browser.
        /// </summary>
        [MenuItem("Assets/Create/Dialogue", false)]
        private static void CreateAsset()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogueGraph>("Dialogue Graph");
        }
        
        /// <summary>
        /// Invoked when the graph structure changes.
        /// Performs structural validation of the dialogue graph.
        /// </summary>
        /// <param name="graphLogger">The logger used to report validation errors.</param>
        public override void OnGraphChanged(GraphLogger graphLogger)
        {
            if (ContainsInvalidConnections())
            {
                graphLogger.LogError($"[{name}] All nodes must contain a maximum of exactly one input connection and one output connection.");
            }

            if (ContainsInvalidNodes())
            {
                graphLogger.LogError($"[{name}] DialogueGraph must contain exactly one DialogueEntry node and one DialogueExit node.");
            }
        }
        
        /// <summary>
        /// Evaluates whether every node has a maximum of one input connection and one output connection.
        /// </summary>
        /// <returns><c>true</c> if all nodes satisfy the connection restraints; otherwise, <c>false</c>.</returns>
        private bool ContainsInvalidConnections()
        {
            List<IPort> inputPortConnections = new();
            List<IPort> outputPortConnections = new();
            
            foreach (var graphNode in GetNodes())
            {
                var inputPort = RetrieveInputPort(graphNode);
                var outputPort = RetrieveOutputPort(graphNode);
                
                RetrievePortConnections(inputPort, inputPortConnections);
                RetrievePortConnections(outputPort, outputPortConnections);

                if (1 < inputPortConnections.Count || 1 < outputPortConnections.Count) return true;
            }

            return false;
        }
        
        /// <summary>
        /// Evaluates whether there is exactly one <see cref="DialogueEntry"/> and one <see cref="DialogueExit"/> node.
        /// </summary>
        /// <returns><c>true</c> if exactly one <see cref="DialogueEntry"/> and one <see cref="DialogueExit"/> node are present; otherwise, <c>false</c>.</returns>
        private bool ContainsInvalidNodes()
        {
            var entryNodes = GetNodes().OfType<DialogueEntry>().ToList();
            var exitNodes = GetNodes().OfType<DialogueExit>().ToList();

            return 1 != entryNodes.Count || 1 != exitNodes.Count;
        }
        
        /// <summary>
        /// Retrieves the provided collection with all ports connected to the specified port.
        /// </summary>
        /// <param name="targetPort">The port chose connections should be retrieved.</param>
        /// <param name="targetPortConnections">The list that will be populated with connected ports (if any).</param>
        private void RetrievePortConnections(IPort targetPort, List<IPort> targetPortConnections)
        {
            if (targetPort is null)
            {
                targetPortConnections.Clear();
                return;
            }
            
            targetPort.GetConnectedPorts(targetPortConnections);
        }
        
        /// <summary>
        /// Retrieves the input port labelled <c>Input</c> from the specified node.
        /// </summary>
        /// <param name="targetNode">The node to retrieve the input port from.</param>
        /// <returns>The <see cref="IPort"/> labelled <c>Input</c> if present; otherwise, <c>null</c>.</returns>
        private IPort RetrieveInputPort(INode targetNode)
        {
            return targetNode.GetInputPorts().FirstOrDefault(inputPort => inputPort.name == "Input");
        }

        /// <summary>
        /// Retrieves the output port labelled <c>Output</c> from the specified node.
        /// </summary>
        /// <param name="targetNode">The node to retrieve the output port from.</param>
        /// <returns>The <see cref="IPort"/> labelled <c>Output</c> if present; otherwise, <c>null</c>.</returns>
        private IPort RetrieveOutputPort(INode targetNode)
        {
            return targetNode.GetOutputPorts().FirstOrDefault(outputPort => outputPort.name == "Output");
        }
    }
}

#endif