/*
Yarn Spinner is licensed to you under the terms found in the file LICENSE.md.
*/

using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Markup;
using Yarn.Unity.Attributes;

#if USE_TMP
using TMPro;
#else
using TMP_Text = Yarn.Unity.TMPShim;
#endif

#nullable enable

namespace Yarn.Unity
{
    public class LinePresenterButtonHandler : ActionMarkupHandler
    {
        [MustNotBeNull, SerializeField] Button? continueButton;

        [MustNotBeNullWhen(nameof(continueButton), "A " + nameof(DialogueRunner) + " must be provided for the continue button to work.")]
        [SerializeField] DialogueRunner? dialogueRunner;

        [MustNotBeNull("The " + nameof(LinePresenterButtonHandler) + " needs a reference to the LineAdvancer to determine the correct skip/advance logic.")]
        [SerializeField] private LineAdvancer? lineAdvancer;

        // In Yarn 3.x, if you are using a custom text reveal component on the same GameObject, 
        // you can reference it here (e.g. Typewriter or similar script).
        [SerializeField] private MonoBehaviour? textRevealer;

        void Start()
        {
            if (continueButton == null)
            {
                Debug.LogWarning($"The {nameof(continueButton)} is null, is it not connected in the inspector?", this);
                return;
            }
            continueButton.interactable = false;
            continueButton.enabled = false;
        }

        public override void OnPrepareForLine(MarkupParseResult line, TMP_Text text)
        {
            if (continueButton == null)
            {
                Debug.LogWarning($"The {nameof(continueButton)} is null, is it not connected in the inspector?", this);
                return;
            }

            // Enable the button
            continueButton.interactable = true;
            continueButton.enabled = true;

            continueButton.onClick.RemoveAllListeners();

            continueButton.onClick.AddListener(() =>
            {
                if (dialogueRunner == null)
                {
                    Debug.LogWarning($"Continue button was clicked, but {nameof(dialogueRunner)} is null!", this);
                    return;
                }

                // Delegate the logic directly to the LineAdvancer.
                // It will evaluate the state (Began vs. Waiting) and automatically 
                // choose between completing the line or advancing.
                if (lineAdvancer != null)
                {
                    lineAdvancer.RequestLineHurryUp();
                }
                else
                {
                    Debug.LogWarning($"Line Advancer reference is not set in {gameObject.name}", this);
                }
            });
        }

        public override void OnLineDisplayBegin(MarkupParseResult line, TMP_Text text)
        {
            return;
        }

        public override YarnTask OnCharacterWillAppear(int currentCharacterIndex, MarkupParseResult line, CancellationToken cancellationToken)
        {
            return YarnTask.CompletedTask;
        }

        public override void OnLineDisplayComplete()
        {
            return;
        }

        public override void OnLineWillDismiss()
        {
            if (continueButton == null)
            {
                return;
            }
            // Disable interaction and clean listeners
            continueButton.onClick.RemoveAllListeners();
            continueButton.interactable = false;
            continueButton.enabled = false;
        }
    }
}
