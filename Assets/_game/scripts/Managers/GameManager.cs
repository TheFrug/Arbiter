using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private ComplianceForm complianceForm;
    [SerializeField] private PortraitManager portraitController;
    [SerializeField] private InterviewManager interviewManager;

    [Header("Subjects")]
    [SerializeField] private List<SubjectData> subjects;

    [Header("Transition Screen Settings")]
    [SerializeField] private GameObject transitionScreenUI;
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private float transitionDuration = 3.0f;

    private CanvasGroup transitionCanvasGroup;

    private void Awake()
    {
        if (complianceForm != null)
            complianceForm.OnFormSubmitted += HandleFormSubmitted;

        // Initialize or add a CanvasGroup for smooth fading
        if (transitionScreenUI != null)
        {
            transitionCanvasGroup = transitionScreenUI.GetComponent<CanvasGroup>();
            if (transitionCanvasGroup == null)
            {
                transitionCanvasGroup = transitionScreenUI.AddComponent<CanvasGroup>();
            }
        }
    }

    private void Start()
    {
        StartCoroutine(RunGapSequence());
    }

    private IEnumerator RunGapSequence()
    {
        // 1. Initialize as fully visible
        if (transitionScreenUI != null)
        {
            transitionScreenUI.SetActive(true);
            transitionCanvasGroup.alpha = 1f;
        }

        // 2. Adjust audio environment
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TransitionAmbience("InterrogationAmbience", 1.5f);
        }

        // 3. Hold for the transition graphic duration
        yield return new WaitForSeconds(transitionDuration);

        // 4. Fade the screen out
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            transitionCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }
        transitionCanvasGroup.alpha = 0f;

        if (transitionScreenUI != null)
        {
            transitionScreenUI.SetActive(false);
        }

        // 5. Start the character / subject segment without triggering sounds in the manager
        StartCurrentSubject();
    }

    private void HandleFormSubmitted(ComplianceForm.ComplianceFormData data)
    {
        NextSubject();
    }

    private void StartCurrentSubject()
    {
        SubjectData subject = interviewManager.GetNextSubject();

        if (subject == null)
        {
            Debug.Log("Shift complete.");
            return;
        }

        complianceForm.ResetForm();
        complianceForm.SetInterviewID(subject.InterviewID);
        complianceForm.SetInterviewDate(subject.Date);

        dialogueRunner.VariableStorage.SetValue("$asked_name", false);
        dialogueRunner.VariableStorage.SetValue("$asked_violation", false);
        dialogueRunner.VariableStorage.SetValue("$asked_occupation", false);
        dialogueRunner.VariableStorage.SetValue("$asked_loyalty", false);

        // Keep the portrait hidden upon initialization. It is only revealed via Yarn command.
        if (portraitController != null)
        {
            portraitController.HidePortrait();
        }

        // Begin the Yarn Node Sequence
        dialogueRunner.StartDialogue(subject.yarnStartNode);
    }

    private void NextSubject()
    {
        StartCoroutine(StartNextSubjectNextFrame());
    }

    private IEnumerator StartNextSubjectNextFrame()
    {
        dialogueRunner.Stop();

        if (portraitController != null)
            portraitController.HidePortrait();

        yield return null;

        StartCurrentSubject();
    }
}