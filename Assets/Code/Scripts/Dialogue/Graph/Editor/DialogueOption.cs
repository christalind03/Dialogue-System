#if UNITY_EDITOR

using Code.Scripts.Dialogue.Graph.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;

using static Code.Scripts.Dialogue.Graph.Editor.DialoguePorts;

namespace Code.Scripts.Dialogue.Graph.Editor
{
    /// <summary>
    /// Represents a selection node in the <see cref="DialogueGraph"/>.
    /// </summary>
    [Serializable]
    internal class DialogueOption : EditorNode
    {
        private const string OptionLabel = "Option";
        private const string OptionPorts = "Options";
        
        /// <inheritdoc cref="Instantiate"/>
        public override RuntimeNode Instantiate(Dictionary<INode, int> dialogueMap, EditorNode editorNode)
        {
            var nodeID = dialogueMap[editorNode];
            var dialogueSelection = new DialogueSelection(nodeID);
            var outputPorts = editorNode.GetOutputPorts().ToList();

            for (var portIndex = 0; portIndex < outputPorts.Count; portIndex++)
            {
                // Offset the port index by one to account for the previous node's connection.
                var optionData = RetrievePortValue<OptionData>(editorNode, Port.Input, $"{OptionLabel} {portIndex}");
                var outputPort = outputPorts[portIndex];
                var connectedNode = outputPort.firstConnectedPort.GetNode();
                var upcomingID = dialogueMap[connectedNode];
                
                var runtimeNode = new Runtime.DialogueOption(nodeID, optionData, upcomingID);
                
                dialogueSelection.DialogueOptions.Add(runtimeNode);
            }
            
            return dialogueSelection;
        }
        
        /// <summary>
        /// Exposes a configurable port count for the number of options to display and select from.
        /// </summary>
        /// <param name="optionContext">The context used to define options for this node.</param>
        protected override void OnDefineOptions(IOptionDefinitionContext optionContext)
        {
            optionContext.AddOption<int>(OptionPorts).WithDefaultValue(2).Delayed();
        }

        /// <summary>
        /// Creates a single port for incoming dialogue flow with paired input and output ports for each available dialogue option.
        /// </summary>
        /// <param name="portContext">The context used to define ports for this node.</param>
        protected override void OnDefinePorts(IPortDefinitionContext portContext)
        {
            CreateInputPort(portContext);
            
            GetNodeOptionByName(OptionPorts).TryGetValue(out int portCount);
            for (var portIndex = 0; portIndex < portCount; portIndex++)
            {
                var portLabel = $"{OptionLabel} {portIndex}";
                
                portContext.AddInputPort<OptionData>(portLabel).Build();
                portContext.AddOutputPort(portLabel).Build();
            }
        }
    }
}

#endif