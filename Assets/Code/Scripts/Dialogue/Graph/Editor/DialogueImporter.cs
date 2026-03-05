#if UNITY_EDITOR

using Code.Scripts.Dialogue.Events.Runtime;
using Code.Scripts.Dialogue.Graph.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Code.Scripts.Dialogue.Graph.Editor
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
            
            var dialogueMap = IdentifyNodes(editorGraph);
            
            IdentifyStart(editorGraph, runtimeGraph, dialogueMap);
            PopulateRuntimeGraph(dialogueMap, editorGraph, runtimeGraph);
            
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
            var dialogueMap = new Dictionary<INode, int>();
            var nodeIndex = 0;
            
            foreach (var dialogueNode in editorGraph.GetNodes())
            {
                dialogueMap[dialogueNode] = nodeIndex;
                nodeIndex++;
            }
            
            return dialogueMap;
        }

        /// <summary>
        /// Identifies the entry point by locating the first <see cref="DialogueEntry"/> node and assigning its connection as the starting node in the runtime <see cref="Runtime.DialogueGraph"/>.
        /// </summary>
        /// <param name="editorGraph">The source editor <see cref="DialogueGraph"/>.</param>
        /// <param name="runtimeGraph">The runtime <see cref="Runtime.DialogueGraph"/> being constructed.</param>
        /// <param name="dialogueMap">A <see cref="Dictionary{TKey,TValue}"/> of editor <see cref="INode"/> to runtime IDs.</param>
        private static void IdentifyStart(DialogueGraph editorGraph, Runtime.DialogueGraph runtimeGraph, Dictionary<INode, int> dialogueMap)
        {
            var entryNode = editorGraph.GetNodes().OfType<DialogueEntry>().FirstOrDefault();
            if (entryNode is null) return;
            
            var entryPort = entryNode.GetOutputPortByName(DialoguePorts.Output)?.firstConnectedPort;
            if (entryPort is null) return;

            var startingNode = dialogueMap[entryPort.GetNode()];
            runtimeGraph.SetEntry(startingNode);
        }
        
        /// <summary>
        /// Converts each supported editor <see cref="INode"/> into its runtime equivalent.
        /// </summary>
        /// <param name="editorGraph">The source editor <see cref="DialogueGraph"/>.</param>
        /// <param name="runtimeGraph">The runtime <see cref="Runtime.DialogueGraph"/> being constructed.</param>
        /// <param name="dialogueMap">A <see cref="Dictionary{TKey,TValue}"/> of editor <see cref="INode"/> to runtime IDs.</param>
        private static void PopulateRuntimeGraph(Dictionary<INode, int> dialogueMap, DialogueGraph editorGraph, Runtime.DialogueGraph runtimeGraph)
        {
            foreach (var editorNode in editorGraph.GetNodes())
            {
                switch (editorNode)
                {
                    case DialogueEntry:
                        continue;
                    
                    case DialogueExit dialogueExit:
                        ProcessDialogueExit(dialogueMap, dialogueExit, runtimeGraph);
                        break;
                    
                    case DialogueNode dialogueNode:
                        ProcessDialogueNode(dialogueMap, dialogueNode, runtimeGraph);
                        break;

                    case DialogueOption dialogueOption:
                        ProcessDialogueOption(dialogueMap, dialogueOption, runtimeGraph);
                        break;
                    
                    default:
                        throw new NotSupportedException($"{editorNode.GetType().Name} is not supported.");
                }
            }
        }

        /// <summary>
        /// Converts an editor <see cref="DialogueExit"/> into its runtime equivalent.
        /// </summary>
        /// <param name="dialogueMap">A <see cref="Dictionary{TKey,TValue}"/> of editor <see cref="INode"/> to runtime IDs.</param>
        /// <param name="editorNode">The editor <see cref="DialogueExit"/> to convert.</param>
        /// <param name="runtimeGraph">The runtime <see cref="Runtime.DialogueGraph"/> being constructed.</param>
        private static void ProcessDialogueExit(Dictionary<INode, int> dialogueMap, DialogueExit editorNode, Runtime.DialogueGraph runtimeGraph)
        {
            var nodeID = dialogueMap[editorNode];
            
            var runtimeNode = InstantiateDialogueExit(editorNode, nodeID);
            
            runtimeGraph.RegisterNode(runtimeNode);
        }
        
        /// <summary>
        /// Converts an editor <see cref="DialogueNode"/> into its runtime equivalent.
        /// </summary>
        /// <param name="dialogueMap">A <see cref="Dictionary{TKey,TValue}"/> of editor <see cref="INode"/> to runtime IDs.</param>
        /// <param name="editorNode">The editor <see cref="DialogueNode"/> to convert.</param>
        /// <param name="runtimeGraph">The runtime <see cref="Runtime.DialogueGraph"/> being constructed.</param>
        private static void ProcessDialogueNode(Dictionary<INode, int> dialogueMap, DialogueNode editorNode, Runtime.DialogueGraph runtimeGraph)
        {
            var nodeID = dialogueMap[editorNode];
            var outputConnection = editorNode.GetOutputPortByName(DialoguePorts.Output).firstConnectedPort;
            var outputNode = outputConnection.GetNode();
            var outputID = dialogueMap[outputNode];
                    
            var runtimeNode = InstantiateDialogueNode(editorNode, nodeID, outputID);
                    
            runtimeGraph.RegisterNode(runtimeNode);
        }
        
        /// <summary>
        /// Converts an editor <see cref="DialogueOption"/> into its runtime equivalent.
        /// </summary>
        /// <param name="dialogueMap">A <see cref="Dictionary{TKey,TValue}"/> of editor <see cref="INode"/> to runtime IDs.</param>
        /// <param name="editorNode">The editor <see cref="DialogueOption"/> to convert.</param>
        /// <param name="runtimeGraph">The runtime <see cref="Runtime.DialogueGraph"/> being constructed.</param>
        private static void ProcessDialogueOption(Dictionary<INode, int> dialogueMap, DialogueOption editorNode, Runtime.DialogueGraph runtimeGraph)
        {
            var nodeID = dialogueMap[editorNode];
            var runtimeNode = InstantiateDialogueSelection(dialogueMap, editorNode, nodeID);
                        
            runtimeGraph.RegisterNode(runtimeNode);
        }
        
        /// <summary>
        /// Creates a <see cref="Runtime.DialogueExit"/> instance from a supported editor node type.
        /// </summary>
        /// <param name="editorNode">The editor <see cref="INode"/> to convert.</param>
        /// <param name="nodeID">The assigned runtime ID of this node.</param>
        /// <returns>A constructed <see cref="Runtime.DialogueExit"/>.</returns>
        private static Runtime.DialogueExit InstantiateDialogueExit(DialogueExit editorNode, int nodeID)
        {
            var nodeEvent = RetrievePortValue<DialogueEvent>(editorNode.GetInputPortByName(DialoguePorts.Event));
            return new Runtime.DialogueExit(nodeID, nodeEvent);
        }
        
        /// <summary>
        /// Creates a <see cref="Runtime.DialogueNode"/> instance from a supported editor node type.
        /// </summary>=
        /// <param name="editorNode">The editor <see cref="INode"/> to convert.</param>
        /// <param name="nodeID">The assigned runtime ID of this node.</param>
        /// <param name="upcomingID">The assigned runtime ID of the node the output connects to.</param>
        /// <returns>A constructed <see cref="Runtime.DialogueNode"/>.</returns>
        private static Runtime.DialogueNode InstantiateDialogueNode(DialogueNode editorNode, int nodeID, int upcomingID)
        {
            var nodeActor = RetrievePortValue<string>(editorNode.GetInputPortByName(DialoguePorts.Actor));
            var nodeAudio = RetrievePortValue<AudioClip>(editorNode.GetInputPortByName(DialoguePorts.Audio));
            var nodeText = RetrievePortValue<string>(editorNode.GetInputPortByName(DialoguePorts.Text));
            var nodeEvent = RetrievePortValue<DialogueEvent>(editorNode.GetInputPortByName(DialoguePorts.Event));

            return new Runtime.DialogueNode(nodeID, nodeActor, nodeAudio, nodeText, nodeEvent, upcomingID);
        }
        
        /// <summary>
        /// Creates a <see cref="Runtime.DialogueSelection"/> instance from a supported editor node type.
        /// </summary>
        /// <param name="dialogueNodes">A <see cref="Dictionary{TKey,TValue}"/> of editor <see cref="INode"/> to runtime IDs.</param>
        /// <param name="editorNode">The editor <see cref="DialogueOption"/> to convert.</param>
        /// <param name="nodeID">The assigned runtime ID of this node.</param>
        /// <returns>A constructed <see cref="Runtime.DialogueSelection"/>.</returns>
        private static Runtime.DialogueSelection InstantiateDialogueSelection(Dictionary<INode, int> dialogueNodes, DialogueOption editorNode, int nodeID)
        {
            var dialogueSelection = new DialogueSelection(nodeID);
            var outputPorts = editorNode.GetOutputPorts().ToList();

            for (var portIndex = 0; portIndex < outputPorts.Count; portIndex++)
            {
                // Offset index by one to account for the previous node's connection
                var optionData = RetrievePortValue<OptionData>(editorNode.GetInputPort(portIndex + 1));
                        
                var currentPort = outputPorts[portIndex];
                var connectedNode = currentPort.firstConnectedPort.GetNode();
                var upcomingID = dialogueNodes[connectedNode];
                        
                dialogueSelection.DialogueOptions.Add(new Runtime.DialogueOption(nodeID, optionData, upcomingID));
            }
                    
            return dialogueSelection;
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