#if UNITY_EDITOR

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
            foreach (var interfaceNode in editorGraph.GetNodes())
            {
                if (interfaceNode is not EditorNode editorNode) continue;

                var runtimeNode = editorNode.Instantiate(dialogueMap, editorNode);
                if (runtimeNode is not null)
                {
                    runtimeGraph.RegisterNode(runtimeNode);
                }
            }
        }
    }
}

#endif