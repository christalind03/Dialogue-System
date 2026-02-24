#if UNITY_EDITOR

using Code.Scripts.Dialogue.Events.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Code.Scripts.Dialogue.Events.Editor
{
    /// <summary>
    /// A custom <see cref="PropertyDrawer"/> for <see cref="DialogueEvent"/> objects.
    /// </summary>
    [CustomPropertyDrawer(typeof(DialogueEvent))]
    internal class DialogueEventPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Creates the custom property inspector user interface for <see cref="DialogueEvent"/>.
        /// </summary>
        /// <param name="serializedProperty">
        /// The <see cref="SerializedProperty"/> representing the <see cref="DialogueEvent"/> instance to be displayed and edited.
        /// </param>
        /// <returns>A <see cref="VisualElement"/> container holding all generated fields for this property.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty serializedProperty)
        {
            var channelsProperty = serializedProperty.FindPropertyRelative("eventChannels");
            var channelsField = new PropertyField(channelsProperty);
            
            return channelsField;
        }
    }
}

#endif