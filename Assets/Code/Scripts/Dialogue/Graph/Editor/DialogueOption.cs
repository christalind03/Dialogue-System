#if UNITY_EDITOR

using Code.Scripts.Dialogue.Graph.Runtime;
using System;
using Unity.GraphToolkit.Editor;

namespace Code.Scripts.Dialogue.Graph.Editor
{
    /// <summary>
    /// Represents a selection node in the <see cref="DialogueGraph"/>.
    /// </summary>
    [Serializable]
    public class DialogueOption : Node
    {
        private const string OptionPorts = "Options";
        
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
            DialoguePorts.CreateInputPort(portContext);
            
            GetNodeOptionByName(OptionPorts).TryGetValue(out int portCount);
            for (var portIndex = 0; portIndex < portCount; portIndex++)
            {
                var portLabel = $"Option {portIndex}";
                
                portContext.AddInputPort<OptionData>(portLabel).Build();
                portContext.AddOutputPort(portLabel).Build();
            }
        }
    }
}

#endif