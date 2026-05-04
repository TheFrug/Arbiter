/*
Yarn Spinner is licensed to you under the terms found in the file LICENSE.md.
*/

using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity.Attributes;

#if USE_TMP
using TMPro;
#else
    using TextMeshProUGUI = Yarn.Unity.TMPShim;
#endif

#nullable enable

namespace Yarn.Unity
{
    [System.Serializable]
    internal struct InternalAppearance
    {
        [SerializeField] internal Sprite sprite;
        [SerializeField] internal Color colour;
    }

    public sealed class OptionItem : MonoBehaviour, ISubmitHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [MustNotBeNull, SerializeField] TextMeshProUGUI? text;
        [SerializeField] UnityEngine.UI.Image? selectionImage;

        [Group("Appearance"), SerializeField] InternalAppearance normal;
        [Group("Appearance"), SerializeField] InternalAppearance selected;
        [Group("Appearance"), SerializeField] InternalAppearance disabled;

        [Group("Appearance"), SerializeField] bool disabledStrikeThrough = true;

        public YarnTaskCompletionSource<DialogueOption?>? OnOptionSelected;
        public System.Threading.CancellationToken completionToken;

        private bool hasSubmittedOptionSelection = false;

        private bool _isInteractable = true;
        public bool Interactable
        {
            get => _isInteractable;
            set
            {
                _isInteractable = value;
                ApplyStyle(value ? normal : disabled);
            }
        }

        private DialogueOption? _option;
        public DialogueOption Option
        {
            get
            {
                if (_option == null)
                {
                    throw new System.NullReferenceException("Option has not been set on the option item");
                }
                return _option;
            }

            set
            {
                _option = value;
                hasSubmittedOptionSelection = false;

                string line = value.Line.TextWithoutCharacterName.Text;
                if (disabledStrikeThrough && !value.IsAvailable)
                {
                    line = $"<s>{value.Line.TextWithoutCharacterName.Text}</s>";
                }

                if (text == null)
                {
                    Debug.LogWarning($"The {nameof(text)} is null, is it not connected in the inspector?", this);
                    return;
                }

                text.text = line;
                Interactable = value.IsAvailable;
            }
        }

        private void ApplyStyle(InternalAppearance style)
        {
            Color newColour = style.colour;
            Sprite newSprite = style.sprite;

            if (text == null)
            {
                Debug.LogWarning($"The {nameof(text)} is null, is it not connected in the inspector?", this);
                return;
            }

            text.color = newColour;

            if (selectionImage != null)
            {
                selectionImage.color = newColour;
                if (newSprite != null)
                {
                    selectionImage.sprite = newSprite;
                    selectionImage.gameObject.SetActive(true);
                }
                else
                {
                    selectionImage.gameObject.SetActive(false);
                }
            }
        }

        // --- Added back to resolve external presenter references ---

        public bool IsHighlighted
        {
            get
            {
                return EventSystem.current != null && EventSystem.current.currentSelectedGameObject == this.gameObject;
            }
        }

        public void Select()
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(this.gameObject);
            }
            ApplyStyle(selected);
        }

        // ------------------------------------------------------------

        public void OnSelect(BaseEventData eventData)
        {
            ApplyStyle(selected);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            ApplyStyle(normal);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            InvokeOptionSelected();
        }

        public void InvokeOptionSelected()
        {
            if (!Interactable)
            {
                return;
            }

            if (hasSubmittedOptionSelection == false && !completionToken.IsCancellationRequested)
            {
                hasSubmittedOptionSelection = true;
                OnOptionSelected?.TrySetResult(this.Option);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(this.gameObject);
            }
            InvokeOptionSelected();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(this.gameObject);
            }
            ApplyStyle(selected);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != this.gameObject)
            {
                ApplyStyle(normal);
            }
        }
    }
}
