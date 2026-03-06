using Code.Scripts.Attributes.Required;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Scripts.Events.Runtime
{
    /// <summary>
    /// Listens to an <see cref="eventChannel"/> and invokes a configured <see cref="UnityEvent"/> reponse when the channel emits.
    /// </summary>
    public class EventListener : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The time (in seconds) to wait after the event before responding.")]
        private float delayDuration;
        
        [Required]
        [SerializeField]
        [Tooltip("The EventChannel asset to listen to.")]
        private EventChannel eventChannel;
        
        [SerializeField]
        [Tooltip("The response invoked when the EventChannel emits.")]
        private UnityEvent eventResponse;

        /// <summary>
        /// Subscribes to the <see cref="eventChannel"/> when this component becomes enabled.
        /// </summary>
        private void OnEnable()
        {
            if (eventChannel is null) return;
            eventChannel.OnEvent += OnEvent;
        }

        /// <summary>
        /// Unsubscribes from the <see cref="eventChannel"/> when this component becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            if (eventChannel is null) return;
            eventChannel.OnEvent -= OnEvent;
        }

        /// <summary>
        /// Called when the <see cref="eventChannel"/> triggers.
        /// </summary>
        private void OnEvent()
        {
            StartCoroutine(InvokeResponse());
        }

        /// <summary>
        /// Waits for the configured delay duration, then invokes the configures <see cref="UnityEvent"/> response.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> that can be used by Unity's coroutine system.</returns>
        private IEnumerator InvokeResponse()
        {
            yield return new WaitForSeconds(delayDuration);
            eventResponse.Invoke();
        }
    }
}