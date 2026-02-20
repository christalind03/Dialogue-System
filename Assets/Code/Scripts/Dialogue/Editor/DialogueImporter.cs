#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Code.Scripts.Dialogue.Editor
{
    /// <summary>
    /// Responsible for converting the editor <see cref="DialogueGraph"/> into its runtime presentation <see cref="Runtime.DialogueGraph"/>.
    /// </summary>
    /// <remarks>
    /// This runs automatically whenever a <see cref="DialogueGraph"/> asset is imported or reimported.
    /// </remarks>
    [ScriptedImporter(1, DialogueGraph.AssetExtension)]
    public class DialogueImporter : ScriptedImporter
    {
        /// <summary>
        /// Converts the editor <see cref="DialogueGraph"/> into a runtime <see cref="Runtime.DialogueGraph"/>.
        /// </summary>
        /// <param name="importContext">The import context for this asset.</param>
        public override void OnImportAsset(AssetImportContext importContext)
        {
            var editorGraph = GraphDatabase.LoadGraphForImporter<DialogueGraph>(importContext.assetPath);
            var runtimeGraph = ScriptableObject.CreateInstance<Runtime.DialogueGraph>();
            
            var dialogueNodes = IdentifyNodes(editorGraph);
            
            IdentifyStart(editorGraph, runtimeGraph, dialogueNodes);
            PopulateRuntimeGraph(editorGraph, runtimeGraph, dialogueNodes);
            
            importContext.AddObjectToAsset("Dialogue", runtimeGraph);
            importContext.SetMainObject(runtimeGraph);
        }

        /// <summary>
        /// Iterates through all the nodes in the editor graph and assigns each node a unique integer ID.
        /// </summary>
        /// <param name="editorGraph">The source editor <see cref="DialogueGraph"/>.</param>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> mapping each <see cref="INode"/> to its assigned runtime integer ID.</returns>
        private static Dictionary<INode, int> IdentifyNodes(DialogueGraph editorGraph)
        {
            var dialogueNodes = new Dictionary<INode, int>();
            var nodeIndex = 0;
            
            foreach (var dialogueNode in editorGraph.GetNodes())
            {
                dialogueNodes[dialogueNode] = nodeIndex;
                nodeIndex++;
            }
            
            return dialogueNodes;
        }

        /// <summary>
        /// Identifies the entry point by locating the first <see cref="DialogueEntry"/> node and assigning its connection as the starting node in the runtime <see cref="Runtime.DialogueGraph"/>.
        /// </summary>
        /// <param name="editorGraph">The source editor <see cref="DialogueGraph"/>.</param>
        /// <param name="runtimeGraph">The runtime <see cref="Runtime.DialogueGraph"/> being constructed.</param>
        /// <param name="dialogueNodes">A <see cref="Dictionary{TKey,TValue}"/> of editor <see cref="INode"/> to runtime IDs.</param>
        private static void IdentifyStart(DialogueGraph editorGraph, Runtime.DialogueGraph runtimeGraph, Dictionary<INode, int> dialogueNodes)
        {
            var entryNode = editorGraph.GetNodes().OfType<DialogueEntry>().FirstOrDefault();
            if (entryNode is null) return;
            
            var entryPort = entryNode.GetOutputPortByName(DialoguePorts.Output)?.firstConnectedPort;
            if (entryPort is null) return;

            var startingNode = dialogueNodes[entryPort.GetNode()];
            runtimeGraph.SetEntry(startingNode);
        }
        
        /// <summary>
        /// Converts each supported editor <see cref="INode"/> into its runtime equivalent.
        /// </summary>
        /// <param name="editorGraph">The source editor <see cref="DialogueGraph"/>.</param>
        /// <param name="runtimeGraph">The runtime <see cref="Runtime.DialogueGraph"/> being constructed.</param>
        /// <param name="dialogueNodes">A <see cref="Dictionary{TKey,TValue}"/> of editor <see cref="INode"/> to runtime IDs.</param>
        private static void PopulateRuntimeGraph(DialogueGraph editorGraph, Runtime.DialogueGraph runtimeGraph, Dictionary<INode, int> dialogueNodes)
        {
            foreach (var dialogueNode in editorGraph.GetNodes())
            {
                if (dialogueNode is DialogueEntry or DialogueExit) continue;
                
                var nodeID = dialogueNodes[dialogueNode];
                var outputConnection = dialogueNode.GetOutputPortByName(DialoguePorts.Output).firstConnectedPort;
                var outputNode = outputConnection.GetNode();
                var outputID = dialogueNodes[outputNode];
                
                var runtimeNode = InstantiateRuntimeNode(dialogueNode, nodeID, outputID);
                
                runtimeGraph.RegisterNode(runtimeNode);
            }
        }
        
        /// <summary>
        /// Creates a <see cref="Runtime.RuntimeNode"/> instance from a supported editor node type.
        /// </summary>
        /// <param name="targetNode">The editor <see cref="INode"/> to convert.</param>
        /// <param name="nodeID">The assigned runtime ID of this node.</param>
        /// <param name="upcomingID">The assigned runtime ID of the node the output connects to.</param>
        /// <returns>A constructed <see cref="Runtime.RuntimeNode"/>.</returns>
        /// <exception cref="NotSupportedException">
        /// Thrown if the editor <see cref="INode"/> type is not supported for runtime conversion.
        /// </exception>
        private static Runtime.RuntimeNode InstantiateRuntimeNode(INode targetNode, int nodeID, int upcomingID)
        {
            switch (targetNode)
            {
                case DialogueNode:
                    var nodeActor = RetrievePortValue<string>(targetNode.GetInputPortByName(DialoguePorts.Actor));
                    var nodeAudio = RetrievePortValue<AudioClip>(targetNode.GetInputPortByName(DialoguePorts.Audio));
                    var nodeText = RetrievePortValue<string>(targetNode.GetInputPortByName(DialoguePorts.Text));
                    
                    return new Runtime.DialogueNode(nodeID, nodeActor, nodeAudio, nodeText, upcomingID);
                
                default:
                    throw new NotSupportedException($"{targetNode.GetType().Name} is not supported.");
            }
        }
        
        /// <summary>
        /// Retrieves the value from a specific port.
        /// </summary>
        /// <param name="targetPort">The input <see cref="IPort"/> to retrieve the value from.</param>
        /// <typeparam name="T">The expected value type.</typeparam>
        /// <returns>The resolved value, or default(T) if none exists.</returns>
        private static T RetrievePortValue<T>(IPort targetPort)
        {
            if (targetPort is null) return default;

            if (targetPort.isConnected)
            {
                if (targetPort.firstConnectedPort.GetNode() is IVariableNode variableNode)
                {
                    variableNode.variable.TryGetDefaultValue(out T variableValue);
                    return variableValue;
                }
            }

            targetPort.TryGetValue(out T defaultValue);
            return defaultValue;
        }
    }
}

#endif