#if UNITY_EDITOR

using Code.Scripts.Events.Runtime;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Scripts.Events.Editor
{
    /// <summary>
    /// A custom <see cref="Editor"/> for <see cref="EventChannel"/> objects.
    /// </summary>
    [CustomEditor(typeof(EventChannel))]
    internal class EventChannelEditor : UnityEditor.Editor
    {
        /// <summary>
        /// The <see cref="EventChannel"/> to create the editor for.
        /// </summary>
        private EventChannel eventChannel;
        
        /// <summary>
        /// The collection of <see cref="MonoBehaviour"/> listeners that are listening to this <see cref="eventChannel"/>.
        /// </summary>
        private List<MonoBehaviour> eventListeners;

        /// <summary>
        /// Initializes references to the target <see cref="EventChannel"/> and the internal listener list.
        /// </summary>
        private void OnEnable()
        {
            eventChannel ??= target as EventChannel;
            eventListeners ??= new List<MonoBehaviour>();
        }

        /// <summary>
        /// Creates a custom inspector GUI for the <see cref="EventChannel"/>.
        /// </summary>
        /// <returns>A <see cref="VisualElement"/> representing the custom inspector layout.</returns>
        public override VisualElement CreateInspectorGUI()
        {
            var rootElement = new VisualElement();
            
            InspectorElement.FillDefaultInspector(rootElement, serializedObject, this);

            rootElement.Add(CreateSpace());
            rootElement.Add(CreateLabel());
            rootElement.Add(CreateList());
            
            return rootElement;
        }

        /// <summary>
        /// Creates a <see cref="VisualElement"/> representing a vertical spacer element for layout spacing.
        /// </summary>
        /// <returns>A <see cref="VisualElement"/> representing a vertical spacer element for layout spacing.</returns>
        private static VisualElement CreateSpace()
        {
            return new VisualElement
            {
                style =
                {
                    marginBottom = 15
                }
            };
        }

        /// <summary>
        /// Creates a <see cref="Label"/> element titled "Listeners" to display above the listener list.
        /// </summary>
        /// <returns>A <see cref="Label"/> element to display above the listener list.</returns>
        private static VisualElement CreateLabel()
        {
            return new Label
            {
                style =
                {
                    borderBottomWidth = 3,
                    borderBottomColor = Color.gray,
                    marginBottom = 3,
                    unityFontStyleAndWeight = FontStyle.Bold
                },
                text = "Listeners",
                tooltip = "Listeners found during Play mode will appear here."
            };
        }

        /// <summary>
        /// Creates a <see cref="ListView"/> displaying all registered <see cref="MonoBehaviour"/> listeners.
        /// </summary>
        /// <returns>A <see cref="ListView"/> bound to the current listener list.</returns>
        private VisualElement CreateList()
        {
            return new ListView
            {
                bindItem = (itemElement, itemIndex) =>
                {
                    if (itemElement is not Label labelElement) return;
                    
                    var listenerObject = eventListeners[itemIndex];

                    labelElement.text = RetrieveListenerTitle(listenerObject);
                    labelElement.RegisterCallback<MouseDownEvent>(_ => { EditorGUIUtility.PingObject(listenerObject.gameObject); });
                },
                itemsSource = RetrieveListeners(),
                makeItem = () => new Label(),
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
            };
        }

        /// <summary>
        /// Retrieves all unique <see cref="MonoBehaviour"/> listeners currently subscribed to this <see cref="eventChannel"/>.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of listeners.</returns>
        private List<MonoBehaviour> RetrieveListeners()
        {
            eventListeners.Clear();
            if (eventChannel?.OnEvent is null) return eventListeners;

            var delegateSubscribers = eventChannel.OnEvent.GetInvocationList();
            foreach (var delegateSubscriber in delegateSubscribers)
            {
                var componentListener = delegateSubscriber.Target as MonoBehaviour;

                if (eventListeners.Contains(componentListener)) continue;
                eventListeners.Add(componentListener);
            }
            
            return eventListeners;
        }

        /// <summary>
        /// Generates a display-friendly title for a listener object.
        /// </summary>
        /// <param name="listenerObject">The <see cref="MonoBehaviour"/> listener instance.</param>
        /// <returns>The listener name in the format "GameObject(ComponentType)"</returns>
        private static string RetrieveListenerTitle(MonoBehaviour listenerObject)
        {
            if (listenerObject is null) return "<null>";

            var listenerTitle = $"{listenerObject.gameObject.name}({listenerObject.GetType().Name})";
            return listenerTitle;
        }
    }
}

#endif