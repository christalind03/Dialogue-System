using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Dialogue.Events.Runtime
{
    /// <summary>
    /// Represents a collection of <see cref="DelayEvent"/> that can be invoked as part of a dialogue sequence.
    /// </summary>
    [Serializable]
    public class DialogueEvent
    {
        [SerializeField]
        [Tooltip("The sequence of events to trigger.")]
        private List<DelayEvent> eventChannels;
        
        /// <summary>
        /// The sequence of events to trigger.
        /// </summary>
        public List<DelayEvent> EventChannels => eventChannels;
    }
}