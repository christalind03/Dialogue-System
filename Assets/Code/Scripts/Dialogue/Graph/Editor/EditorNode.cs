#if UNITY_EDITOR

using Code.Scripts.Dialogue.Graph.Runtime;
using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace Code.Scripts.Dialogue.Graph.Editor
{
    /// <summary>
    /// The base class for all editor <see cref="DialogueGraph"/> nodes.
    /// </summary>
    [Serializable]
    internal abstract class EditorNode : Node
    {
        /// <summary>
        /// Converts this <see cref="EditorNode"/> into its runtime equivalent.
        /// </summary>
        /// <param name="dialogueMap">A <see cref="Dictionary{TKey,TValue}"/> mapping each <see cref="INode"/> to its assigned runtime ID.</param>
        /// <param name="editorNode">The <see cref="EditorNode"/> to convert.</param>
        /// <returns>The constructed <see cref="RuntimeNode"/>.</returns>
        public abstract RuntimeNode Instantiate(Dictionary<INode, int> dialogueMap, EditorNode editorNode);
    }
}

#endif