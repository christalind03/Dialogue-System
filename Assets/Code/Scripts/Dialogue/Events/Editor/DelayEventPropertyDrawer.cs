using Code.Scripts.Dialogue.Events.Runtime;
using Code.Scripts.Events.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Code.Scripts.Dialogue.Events.Editor
{
    /// <summary>
    /// A custom <see cref="PropertyDrawer"/> for <see cref="DelayEvent"/> objects.
    /// </summary>
    [CustomPropertyDrawer(typeof(DelayEvent))]
    public class DelayEventPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Creates the custom property inspector user interface for <see cref="DelayEvent"/>.
        /// </summary>
        /// <param name="serializedProperty">
        /// The <see cref="SerializedProperty"/> representing the <see cref="DelayEvent"/> instance to be displayed and edited.
        /// </param>
        /// <returns>A <see cref="VisualElement"/> container holding all generated fields for this property.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty serializedProperty)
        {
            var rootElement = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1
                }
            };

            var delayField = CreateDelayField();
            delayField.style.marginRight = 3;
            delayField.style.width = 35;

            var channelField = CreateChannelField();
            
            rootElement.Add(delayField);
            rootElement.Add(channelField);
            
            return rootElement;
        }
        
        /// <summary>
        /// Creates the <see cref="DelayEvent.DelayDuration"/> field.
        /// </summary>
        /// <returns>A <see cref="FloatField"/> representing the <see cref="DelayEvent.DelayDuration"/> field.</returns>
        private static VisualElement CreateDelayField()
        {
            return new FloatField
            {
                bindingPath = "delayDuration"
            };
        }

        /// <summary>
        /// Creates the <see cref="DelayEvent.EventChannel"/> field.
        /// </summary>
        /// <returns>An <see cref="ObjectField"/> representing the <see cref="DelayEvent.EventChannel"/> field.</returns>
        private static VisualElement CreateChannelField()
        {
            return new ObjectField
            {
                allowSceneObjects = false,
                bindingPath = "eventChannel",
                objectType = typeof(EventChannel),
                style =
                {
                    flexGrow = 1
                }
            };
        }
    }
}