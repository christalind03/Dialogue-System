using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Dialogue.Runtime
{
    /// <summary>
    /// A runtime representation of a compiled editor <see cref="Editor.DialogueGraph"/>
    /// </summary>
    public class DialogueGraph : ScriptableObject
    {
        /// <summary>
        /// The ID of the node where dialogue execution begins.
        /// </summary>
        [SerializeField]
        private int entryID = -1;

        /// <summary>
        /// Collection of all <see cref="RuntimeNode"/> that belong to this dialogue graph.
        /// </summary>
        [SerializeReference]
        private List<RuntimeNode> nodeRegistry = new();
        
        /// <summary>
        /// Retrieves the ID of the graph's entry node.
        /// </summary>
        public int EntryID => entryID;
        
        /// <summary>
        /// Retrieves the collection of registered <see cref="RuntimeNode"/>.
        /// </summary>
        public List<RuntimeNode> NodeRegistry => nodeRegistry;

        /// <summary>
        /// Registers a <see cref="RuntimeNode"/> into the graph.
        /// </summary>
        /// <param name="targetNode">The <see cref="RuntimeNode"/> to register.</param>
        public void RegisterNode(RuntimeNode targetNode)
        {
            nodeRegistry.Add(targetNode);
        }
        
        /// <summary>
        /// Sets the entry node ID for the graph.
        /// </summary>
        /// <param name="targetID">The ID of the starting node.</param>
        public void SetEntry(int targetID)
        {
            entryID = targetID;
        }
    }
}