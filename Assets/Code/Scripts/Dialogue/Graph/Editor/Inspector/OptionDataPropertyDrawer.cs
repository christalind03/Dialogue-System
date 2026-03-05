using Code.Scripts.Dialogue.Graph.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Code.Scripts.Dialogue.Graph.Editor.Inspector
{
    /// <summary>
    /// A custom <see cref="PropertyDrawer"/> for <see cref="OptionData"/> objects.
    /// </summary>
    [CustomPropertyDrawer(typeof(OptionData))]
    public class OptionDataPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Creates the custom property inspector user interface for <see cref="OptionData"/>.
        /// </summary>
        /// <param name="serializedProperty">
        /// The <see cref="SerializedProperty"/> representing the <see cref="OptionData"/> instance to be displayed and edited.
        /// </param>
        /// <returns>A <see cref="VisualElement"/> container holding all generated fields for this property.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty serializedProperty)
        {
            var rootElement = new VisualElement();

            var textProperty = serializedProperty.FindPropertyRelative("optionText");
            var eventsProperty = serializedProperty.FindPropertyRelative("optionEvents");
            
            var textField = new PropertyField(textProperty);
            var eventsField = new PropertyField(eventsProperty);
            
            rootElement.Add(textField);
            rootElement.Add(eventsField);
            
            return rootElement;
        }
    }
}