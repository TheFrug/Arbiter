using UnityEngine;
using UnityEngine.EventSystems;

namespace Yarn.Unity
{
    [RequireComponent(typeof(OptionItem))]
    public class OptionItemAudio : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioClip? selectSound;
        [SerializeField] private AudioClip? highlightSound;

        private AudioSource? audioSource;
        private OptionItem? optionItem;

        private void Awake()
        {
            optionItem = GetComponent<OptionItem>();

            // Set up or find an AudioSource on the same object
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }

            // If an option is clicked/selected via Yarn
            if (optionItem != null)
            {
                // We cannot modify the base OptionItem, but we can hook into Unity's event system on selection
            }
        }

        // Triggered when a controller or keyboard highlights the button
        public void OnSelect(BaseEventData eventData)
        {
            if (highlightSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(highlightSound);
            }
        }

        // Triggered when the mouse cursor hovers over the button
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (highlightSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(highlightSound);
            }
        }

        // Call this method from your UI or Button click event
        public void PlaySelectSound()
        {
            if (selectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(selectSound);
            }
        }
    }
}