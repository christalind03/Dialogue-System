using System;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Scripts.Events.Runtime
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> used to broadcast events across systems.
    /// </summary>
    [CreateAssetMenu(fileName = "Event Channel", menuName = "Events/Event Channel")]
    [Serializable]
    public class EventChannel : ScriptableObject
    {
        #if UNITY_EDITOR

        /// <summary>
        /// This field exists purely for documentation purposes inside the Inspector and is not included in builds.
        /// This helps prevent misuse and improves maintainability in larger projects.
        /// </summary>
        [SerializeField]
        [TextArea(3, 5)]
        [Tooltip("Provides details about the asset including its purpose and/or usage. Editor-Only")]
        private string assetDescription;
        
        #endif
        
        /// <summary>
        /// The event invoked when this channel emits.
        /// Subscribers should register/unregister in OnEnable/OnDisable.
        /// </summary>
        public UnityAction OnEvent;

        /// <summary>
        /// Raises the event, notifying all subscribed listeners.
        /// </summary>
        public void Emit()
        {
            OnEvent.Invoke();
        }
    }
}