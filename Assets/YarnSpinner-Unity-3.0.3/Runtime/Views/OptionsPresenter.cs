/*
Yarn Spinner is licensed to you under the terms found in the file LICENSE.md.
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity.Attributes;

#if USE_TMP
using TMPro;
#else
using TextMeshProUGUI = Yarn.Unity.TMPShim;
#endif

#nullable enable

using System.Threading;

namespace Yarn.Unity
{
    [HelpURL("https://docs.yarnspinner.dev/using-yarnspinner-with-unity/components/dialogue-view/options-list-view")]
    public sealed class OptionsPresenter : DialoguePresenterBase
    {
        [SerializeField] CanvasGroup? canvasGroup;

        [MustNotBeNull]
        [SerializeField] OptionItem? optionViewPrefab;
        [MustNotBeNull]
        [SerializeField] OptionItem? skillCheckOptionItemPrefab;

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

        private void Update()
        {
            // Defensive recovery: If we have instantiated options and the EventSystem loses selection, 
            // ensure at least one element is selected to prevent soft locking.
            if (optionViews.Count > 0 && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            {
                for (int i = 0; i < optionViews.Count; i++)
                {
                    if (optionViews[i] != null && optionViews[i].gameObject.activeSelf && optionViews[i].interactable)
                    {
                        optionViews[i].Select();
                        break;
                    }
                }
            }
        }

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

        public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
            if (showsLastLine)
            {
                lastSeenLine = line;
            }
            return YarnTask.CompletedTask;
        }

        public override async YarnTask<DialogueOption?> RunOptionsAsync(
    DialogueOption[] dialogueOptions,
    CancellationToken cancellationToken)
        {
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

            for (int i = 0; i < dialogueOptions.Length; i++)
            {
                var option = dialogueOptions[i];

                if (option.IsAvailable == false && showUnavailableOptions == false)
                    continue;

                bool isSkillCheck = false;
                string skillStat = string.Empty;
                int skillDifficulty = 0;

                if (option.Line.Metadata != null)
                {
                    foreach (var tag in option.Line.Metadata)
                    {
                        if (tag.StartsWith("skillcheck"))
                        {
                            isSkillCheck = true;
                            var parts = tag.Split(':');

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

                OptionItem prefabToUse =
                    isSkillCheck && skillCheckOptionItemPrefab != null
                    ? skillCheckOptionItemPrefab
                    : optionViewPrefab;

                if (prefabToUse == null)
                {
                    throw new System.InvalidOperationException(
                        "OptionsPresenter: Prefab reference missing.");
                }

                var optionView = Instantiate(prefabToUse);

                var targetTransform =
                    canvasGroup != null ? canvasGroup.transform : this.transform;

                optionView.transform.SetParent(targetTransform, false);
                optionView.transform.SetAsLastSibling();
                optionView.gameObject.SetActive(true);

                optionView.Option = option;

                optionView.OnOptionSelected = selectedOptionCompletionSource;
                optionView.completionToken = completionCancellationSource.Token;

                optionViews.Add(optionView);
            }

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
