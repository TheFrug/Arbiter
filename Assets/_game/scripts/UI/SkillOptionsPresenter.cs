/*
Yarn Spinner is licensed to you under the terms found in the file LICENSE.md.
*/

using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Yarn.Unity.Attributes;
using TMPro;
using System.Threading;
#nullable enable

namespace Yarn.Unity
{
    /// <summary>
    /// Receives options from a <see cref="DialogueRunner"/>, and displays and
    /// manages a collection of <see cref="OptionItem"/> views for the user
    /// to choose from.
    /// </summary>
    [HelpURL("https://docs.yarnspinner.dev/using-yarnspinner-with-unity/components/dialogue-view/options-list-view")]
    public class SkillOptionsPresenter : DialoguePresenterBase
    {
        [SerializeField] CanvasGroup? canvasGroup;

        [MustNotBeNull]
        [SerializeField] OptionItem? optionViewPrefab;
        [MustNotBeNull]
        [SerializeField] OptionItem? skillCheckOptionItemPrefab;

        // A cached pool of OptionView objects so that we can reuse them
        List<OptionItem> optionViews = new List<OptionItem>();

        [Space]
        [SerializeField] bool showsLastLine;

        [ShowIf(nameof(showsLastLine))]
        [Indent]
        [MustNotBeNullWhen(nameof(showsLastLine))]
        [SerializeField] TextMeshProUGUI? lastLineText;

        [ShowIf(nameof(showsLastLine))]
        [Indent]
        [SerializeField] GameObject? lastLineContainer;

        [ShowIf(nameof(showsLastLine))]
        [Indent]
        [SerializeField] TextMeshProUGUI? lastLineCharacterNameText;

        [ShowIf(nameof(showsLastLine))]
        [Indent]
        [SerializeField] GameObject? lastLineCharacterNameContainer;

        LocalizedLine? lastSeenLine;

        /// <summary>
        /// Controls whether or not to display options whose <see
        /// cref="OptionSet.Option.IsAvailable"/> value is <see
        /// langword="false"/>.
        /// </summary>
        [Space]
        public bool showUnavailableOptions = false;

        [Group("Fade")]
        [Label("Fade UI")]
        public bool useFadeEffect = true;

        [Group("Fade")]
        [ShowIf(nameof(useFadeEffect))]
        public float fadeUpDuration = 0.25f;

        [Group("Fade")]
        [ShowIf(nameof(useFadeEffect))]
        public float fadeDownDuration = 0.1f;

        private const string TruncateLastLineMarkupName = "lastline";

        /// <summary>
        /// Called by a <see cref="DialogueRunner"/> to dismiss the options view
        /// when dialogue is complete.
        /// </summary>
        /// <returns>A completed task.</returns>
        public override YarnTask OnDialogueCompleteAsync()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            return YarnTask.CompletedTask;
        }

        /// <summary>
        /// Called by Unity to set up the object.
        /// </summary>
        private void Start()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            if (lastLineContainer == null && lastLineText != null)
            {
                lastLineContainer = lastLineText.gameObject;
            }
            if (lastLineCharacterNameContainer == null && lastLineCharacterNameText != null)
            {
                lastLineCharacterNameContainer = lastLineCharacterNameText.gameObject;
            }
        }

        /// <summary>
        /// Called by a <see cref="DialogueRunner"/> to set up the options view
        /// when dialogue begins.
        /// </summary>
        /// <returns>A completed task.</returns>
        public override YarnTask OnDialogueStartedAsync()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            return YarnTask.CompletedTask;
        }

        /// <summary>
        /// Called by a <see cref="DialogueRunner"/> when a line needs to be
        /// presented, and stores the line as the 'last seen line' so that it
        /// can be shown when options appear.
        /// </summary>
        /// <remarks>This view does not display lines directly, but instead
        /// stores lines so that when options are run, the last line that ran
        /// before the options appeared can be shown.</remarks>
        /// <inheritdoc cref="DialoguePresenterBase.RunLineAsync"
        /// path="/param"/>
        /// <returns>A completed task.</returns>
        public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
            if (showsLastLine)
            {
                lastSeenLine = line;
            }
            return YarnTask.CompletedTask;
        }

        /// <summary>
        /// Called by a <see cref="DialogueRunner"/> to display a collection of
        /// options to the user. 
        /// </summary>
        /// <inheritdoc cref="DialoguePresenterBase.RunOptionsAsync"
        /// path="/param"/>
        /// <inheritdoc cref="DialoguePresenterBase.RunOptionsAsync"
        /// path="/returns"/>

        public override async YarnTask<DialogueOption?> RunOptionsAsync(
    DialogueOption[] dialogueOptions,
    CancellationToken cancellationToken)
        {
            // Destroy any previously created option views
            foreach (var view in optionViews)
            {
                if (view != null)
                    Destroy(view.gameObject);
            }
            optionViews.Clear();

            YarnTaskCompletionSource<DialogueOption?> selectedOptionCompletionSource =
                new YarnTaskCompletionSource<DialogueOption?>();

            var completionCancellationSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            async YarnTask CancelSourceWhenDialogueCancelled()
            {
                await YarnTask.WaitUntilCanceled(completionCancellationSource.Token);

                if (cancellationToken.IsCancellationRequested)
                {
                    selectedOptionCompletionSource.TrySetResult(null);
                }
            }

            CancelSourceWhenDialogueCancelled().Forget();

            // ==========================================
            // CREATE OPTION VIEWS PER OPTION (3.0 SAFE)
            // ==========================================

            for (int i = 0; i < dialogueOptions.Length; i++)
            {
                var option = dialogueOptions[i];

                if (option.IsAvailable == false && showUnavailableOptions == false)
                    continue;

                // --- Detect and parse skillcheck tag ---
                bool isSkillCheck = false;
                string skillStat = string.Empty;
                int skillDifficulty = 0;

                if (option.Line.Metadata != null)
                {
                    foreach (var tag in option.Line.Metadata)
                    {
                        // Expected format:
                        // skillcheck:StatName:Difficulty
                        if (tag.StartsWith("skillcheck"))
                        {
                            isSkillCheck = true;

                            var parts = tag.Split(':');

                            // Ensure we actually have parameters
                            if (parts.Length >= 3)
                            {
                                skillStat = parts[1];

                                if (!int.TryParse(parts[2], out skillDifficulty))
                                {
                                    skillDifficulty = 0;
                                }
                            }

                            break;
                        }
                    }
                }


                // --- Choose prefab ---
                OptionItem prefabToUse =
                    isSkillCheck && skillCheckOptionItemPrefab != null
                    ? skillCheckOptionItemPrefab
                    : optionViewPrefab;

                if (prefabToUse == null)
                {
                    throw new System.InvalidOperationException(
                        "OptionsPresenter: Prefab reference missing.");
                }

                // --- Instantiate ---
                var optionView = Instantiate(prefabToUse);

                var targetTransform =
                    canvasGroup != null ? canvasGroup.transform : this.transform;

                optionView.transform.SetParent(targetTransform, false);
                optionView.transform.SetAsLastSibling();
                optionView.gameObject.SetActive(true);

                // --- Bind Yarn data ---
                optionView.Option = option;
                if (isSkillCheck)
                {
                    var skillUI = optionView.GetComponent<SkillCheckUI>();
                    if (skillUI != null)
                    {
                        skillUI.Configure(skillStat, skillDifficulty);
                    }
                }

                optionView.OnOptionSelected = selectedOptionCompletionSource;
                optionView.completionToken = completionCancellationSource.Token;

                optionViews.Add(optionView);
            }

            // ==========================================
            // HIGHLIGHT FIX (UNCHANGED FROM 3.0 SOURCE)
            // ==========================================

            int optionIndexToSelect = -1;

            for (int i = 0; i < optionViews.Count; i++)
            {
                var view = optionViews[i];

                if (!view.isActiveAndEnabled)
                    continue;

                if (view.IsHighlighted)
                {
                    optionIndexToSelect = i;
                    break;
                }

                if (optionIndexToSelect == -1)
                    optionIndexToSelect = i;
            }

            if (optionIndexToSelect > -1)
            {
                optionViews[optionIndexToSelect].Select();
            }

            // ==========================================
            // LAST LINE DISPLAY (UNCHANGED)
            // ==========================================

            if (lastLineContainer != null)
            {
                if (lastSeenLine != null && showsLastLine)
                {
                    var line = lastSeenLine.Text;

                    if (lastLineCharacterNameContainer != null)
                    {
                        if (string.IsNullOrWhiteSpace(lastSeenLine.CharacterName))
                        {
                            lastLineCharacterNameContainer.SetActive(false);
                        }
                        else
                        {
                            line = lastSeenLine.TextWithoutCharacterName;
                            lastLineCharacterNameContainer.SetActive(true);

                            if (lastLineCharacterNameText != null)
                                lastLineCharacterNameText.text =
                                    lastSeenLine.CharacterName;
                        }
                    }
                    else
                    {
                        line = lastSeenLine.TextWithoutCharacterName;
                    }

                    var lineText = line.Text;

                    if (line.TryGetAttributeWithName(
                            TruncateLastLineMarkupName,
                            out var markup))
                    {
                        var end = lineText.Substring(markup.Position);
                        lineText = "..." + end;
                    }

                    if (lastLineText != null)
                        lastLineText.text = lineText;

                    lastLineContainer.SetActive(true);
                }
                else
                {
                    lastLineContainer.SetActive(false);
                }
            }

            if (useFadeEffect && canvasGroup != null)
            {
                await Effects.FadeAlphaAsync(
                    canvasGroup,
                    0,
                    1,
                    fadeUpDuration,
                    cancellationToken);
            }

            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            var completedTask = await selectedOptionCompletionSource.Task;
            completionCancellationSource.Cancel();

            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            if (useFadeEffect && canvasGroup != null)
            {
                await Effects.FadeAlphaAsync(
                    canvasGroup,
                    1,
                    0,
                    fadeDownDuration,
                    cancellationToken);
            }

            foreach (var optionView in optionViews)
            {
                optionView.gameObject.SetActive(false);
            }

            await YarnTask.Yield();

            if (cancellationToken.IsCancellationRequested)
                return await DialogueRunner.NoOptionSelected;

            return completedTask;
        }


        private OptionItem CreateNewOptionView()
        {
            var optionView = Instantiate(optionViewPrefab);

            var targetTransform = canvasGroup != null ? canvasGroup.transform : this.transform;

            if (optionView == null)
            {
                throw new System.InvalidOperationException($"Can't create new option view: {nameof(optionView)} is null");
            }

            optionView.transform.SetParent(targetTransform.transform, false);
            optionView.transform.SetAsLastSibling();
            optionView.gameObject.SetActive(false);

            return optionView;
        }
    }
}
