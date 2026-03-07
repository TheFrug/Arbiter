using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private ComplianceForm complianceForm;
    [SerializeField] private PortraitManager portraitController;
    [SerializeField] private InterviewManager interviewManager;

    [Header("Subjects")]
    [SerializeField] private List<SubjectData> subjects;

    private int currentSubjectIndex = 0;

    private void Awake()
    {
        if (complianceForm != null)
            complianceForm.OnFormSubmitted += HandleFormSubmitted;
    }

    private void Start()
    {
        StartCurrentSubject();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
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

        dialogueRunner.VariableStorage.SetValue("$asked_name", false);
        dialogueRunner.VariableStorage.SetValue("$asked_violation", false);

        if (portraitController != null)
            portraitController.ShowPortrait(subject.portrait);

        dialogueRunner.StartDialogue(subject.yarnStartNode);
    }

    private void NextSubject()
    {
        dialogueRunner.Stop();

        if (portraitController != null)
            portraitController.HidePortrait();

        StartCurrentSubject();
    }
}