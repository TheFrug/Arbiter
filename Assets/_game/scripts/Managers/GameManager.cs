using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private InfractionForm infractionForm;
    [SerializeField] private PortraitManager portraitController;

    [Header("Suspects")]
    [SerializeField] private List<SuspectData> suspects;

    private int currentSuspectIndex = 0;

    private void Awake()
    {
        infractionForm.OnFormSubmitted += HandleFormSubmitted;
    }

    private void Start()
    {
        //StartCurrentSuspect();
    }

    private void Update()
    {
        // Quit game with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        // Restart scene with Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    private void HandleFormSubmitted(InfractionForm.InfractionFormData data)
    {
        NextSuspect();
    }

    private void StartCurrentSuspect()
    {
        if (currentSuspectIndex >= suspects.Count)
        {
            Debug.Log("Shift complete.");
            return;
        }

        SuspectData suspect = suspects[currentSuspectIndex];

        // Set form case ID
        infractionForm.SetSuspectData(suspect);

        // Reset Yarn state variables if needed
        dialogueRunner.VariableStorage.SetValue("$asked_name", false);
        dialogueRunner.VariableStorage.SetValue("$asked_violation", false);

        // Show portrait
        portraitController.ShowPortrait(suspect.portrait);

        // Start dialogue
        dialogueRunner.StartDialogue(suspect.yarnStartNode);
    }

    private void NextSuspect()
    {
        dialogueRunner.Stop();
        portraitController.HidePortrait();

        currentSuspectIndex++;

        if (currentSuspectIndex < suspects.Count)
        {
            StartCurrentSuspect();
        }
        else
        {
            Debug.Log("No more suspects.");
        }
    }
}
