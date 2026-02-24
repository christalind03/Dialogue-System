using Code.Scripts.Events.Runtime;
using System;
using UnityEngine;

namespace Code.Scripts.Dialogue.Events.Runtime
{
    /// <summary>
    /// Represents an event that is triggered after a specified delay.
    /// </summary>
    [Serializable]
    public class DelayEvent
    {
        [SerializeField]
        [Tooltip("The time (in seconds) to wait before emitting the EventChannel.")]
        private float delayDuration;
        
        [SerializeField]
        [Tooltip("The EventChannel to trigger after the delay.")]
        private EventChannel eventChannel;
        
        /// <summary>
        /// The time (in seconds) to wait before emitting the <see cref="eventChannel"/>.
        /// </summary>
        public float DelayDuration => delayDuration;
        
        /// <summary>
        /// The <see cref="Code.Scripts.Events.Runtime.EventChannel"/> to trigger after the delay.
        /// </summary>
        public EventChannel EventChannel => eventChannel;
    }
}