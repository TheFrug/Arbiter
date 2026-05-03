using UnityEngine;
using Yarn.Unity;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// YarnManager
/// Central bridge between Yarn Spinner 3.0 and Unity systems.
/// Handles:
/// - Command routing
/// - Skill checks
/// - Variable access
/// - Dialogue branching
/// - Audio triggering
/// </summary>
public class YarnManager : MonoBehaviour
{
    public static YarnManager Instance { get; private set; }

    [Header("Core References")]
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private VariableStorageBehaviour variableStorage;

    [Header("Game System References")]
    [SerializeField] private ComplianceForm complianceForm;
    [SerializeField] private PlayerManager playerManager;

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the new DialogueRunner in the scene if it isn't assigned or was destroyed
        if (dialogueRunner == null)
        {
            dialogueRunner = FindObjectOfType<DialogueRunner>();
        }

        // Only register if we successfully found a valid runner
        if (dialogueRunner != null)
        {
            RegisterYarnFunctions();
        }
    }

    private void RegisterYarnFunctions()
    {
        // Unregister existing functions to avoid duplicate errors, if supported by the version
        // (Adding a function usually overwrites the existing one in Yarn Spinner)

        dialogueRunner.AddFunction("is_name_filled", () => {
            if (complianceForm == null) return false;
            return !string.IsNullOrEmpty(complianceForm.GetFilledName());
        });

        dialogueRunner.AddFunction("is_occupation_filled", () => {
            if (complianceForm == null) return false;
            return !string.IsNullOrEmpty(complianceForm.GetFilledOccupation());
        });

        dialogueRunner.AddFunction("is_form_complete", () => {
            if (complianceForm == null) return false;
            return complianceForm.IsFormComplete;
        });

        Debug.Log("Yarn functions successfully registered for the active scene.");
    }

    public void RegisterSceneDependencies(
        DialogueRunner runner,
        VariableStorageBehaviour storage,
        PlayerManager player,
        ComplianceForm compliance)
    {
        dialogueRunner = runner;
        variableStorage = storage;
        playerManager = player;

        // Safety check to re-assign if empty
        if (playerManager == null)
        {
            playerManager = FindObjectOfType<PlayerManager>();
        }

        complianceForm = compliance;

        // Register functions when dependencies are set
        if (dialogueRunner != null)
        {
            RegisterYarnFunctions();
        }
    }

    #endregion

    // =========================================================
    // ================= COMPLIANCE FORM =======================
    // =========================================================

    /*
     * Yarn Usage:
     * <<form_set_name "John Halberd">>
     */
    [YarnCommand("form_set_name")]
    public void SetName(string name)
    {
        complianceForm?.RevealNameFromYarn(name);
    }

    /*
     * Yarn Usage:
     * <<form_set_occupation "Mechanic">>
     */
    [YarnCommand("form_set_occupation")]
    public void SetOccupation(string occupation)
    {
        complianceForm?.RevealOccupationFromYarn(occupation);
    }

    /*
     * Yarn Usage:
     * <<form_submit>>
     */
    [YarnCommand("form_submit")]
    public void SubmitComplianceForm()
    {
        if (complianceForm == null)
        {
            Debug.LogWarning("ComplianceForm reference missing.");
            return;
        }

        complianceForm.SubmitForm();
    }

    /*
     * Optional — start a new interview cleanly
     * Yarn Usage:
     * <<form_reset>>
     */
    [YarnCommand("form_reset")]
    public void ResetComplianceForm()
    {
        if (complianceForm == null)
            return;

        complianceForm.ResetForm();
    }



    // =========================================================
    // ==================== SKILL CHECK ========================
    // =========================================================

    /*
     * Usage in Yarn:
     * <<SkillCheck "Force" 12>>
     */
    [YarnCommand("SkillCheck")]
    public void SkillCheck(string statName, int difficulty)
    {
        if (playerManager == null || SkillCheckResolver.Instance == null)
        {
            Debug.LogError("Missing PlayerManager or SkillCheckResolver.");
            return;
        }

        int statValue = playerManager.GetStat(statName);

        var result = SkillCheckResolver.Instance
            .RollSkillCheck(statValue, difficulty);

        Debug.Log(
            $"SkillCheck: {statName} | " +
            $"Result: {result.DiceTotal} + {statValue} = {result.FinalTotal} " +
            $"vs {difficulty} | Success: {result.Success}"
        );

        if (dialogueRunner != null)
        {
            dialogueRunner.VariableStorage.SetValue("$last_check_roll", result.DiceTotal);
            dialogueRunner.VariableStorage.SetValue("$last_check_total", result.FinalTotal);
            dialogueRunner.VariableStorage.SetValue("$last_check_stat", result.StatValue);
            dialogueRunner.VariableStorage.SetValue("$last_check_difficulty", result.Difficulty);
            dialogueRunner.VariableStorage.SetValue("$last_check_success", result.Success);
        }
    }




    // =========================================================
    // ================= VARIABLE HELPERS ======================
    // =========================================================

    public void SetFloat(string variableName, float value)
    {
        if (variableStorage == null) return;
        variableStorage.SetValue(variableName, value);
    }

    public void SetBool(string variableName, bool value)
    {
        if (variableStorage == null) return;
        variableStorage.SetValue(variableName, value);
    }

    public void SetString(string variableName, string value)
    {
        if (variableStorage == null) return;
        variableStorage.SetValue(variableName, value);
    }

    public float GetFloat(string variableName)
    {
        if (variableStorage != null &&
            variableStorage.TryGetValue(variableName, out float value))
        {
            return value;
        }

        return 0f;
    }

    public bool GetBool(string variableName)
    {
        if (variableStorage != null &&
            variableStorage.TryGetValue(variableName, out bool value))
        {
            return value;
        }

        return false;
    }

    public string GetString(string variableName)
    {
        if (variableStorage != null &&
            variableStorage.TryGetValue(variableName, out string value))
        {
            return value;
        }

        return string.Empty;
    }


    // =========================================================
    // ================= CHARACTER CREATION ====================
    // =========================================================

    [YarnCommand("add_stat")]
    public void AddStat(string statName, int amount)
    {
        if (playerManager == null)
        {
            Debug.LogError("PlayerManager missing.");
            return;
        }

        playerManager.ModifyStat(statName, amount);

        Debug.Log($"CharacterCreation: {statName} +{amount}");
    }

    [YarnCommand("set_stat")]
    public void SetStat(string statName, int value)
    {
        if (playerManager == null)
            return;

        playerManager.SetStat(statName, value);
    }

    [YarnCommand("finalize_character")]
    public void FinalizeCharacter(string nextScene)
    {
        Debug.Log("Character creation finalized.");

        FindObjectOfType<IDCardUI>()?.FinishCharacterCreation();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TransitionAmbience("InterrogationAmbience", 1.5f);
        }

        if (ScreenFader.Instance != null)
        {
            ScreenFader.Instance.FadeToBlack(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
                ScreenFader.Instance.FadeToClear();
            });
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }
    }

    // ID Card Control
    [YarnCommand("show_id")]
    public void ShowID()
    {
        FindObjectOfType<IDCardUI>()?.ShowCard();
    }

    // =========================================================
    // ================= AUDIO PLAYBACK ========================
    // =========================================================
    [YarnCommand("play_sound")]
    public void PlayNarratorSound(string speakerKey)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(speakerKey);
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found in the scene.");
        }
    }

    [YarnCommand("stop_music")]
    public void StopMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found in the scene.");
        }
    }


    // =========================================================
    // ================= PERFORMANCE EVALUATION =================
    // =========================================================

    [YarnCommand("get_total_score")]
    public void GetTotalScore()
    {
        if (ComplianceResultsManager.Instance == null || variableStorage == null) return;

        var results = ComplianceResultsManager.Instance.EvaluateAll();

        int total = 0;
        int max = 0;

        foreach (var r in results)
        {
            total += r.score;
            max += r.maxScore;
        }

        variableStorage.SetValue("$totalScore", total);
        variableStorage.SetValue("$maxScore", max);

        Debug.Log($"Total Score (flag-based): {total}/{max}");
    }

    [YarnCommand("get_subject_score")]
    public void GetSubjectScore(string interviewID)
    {
        if (ComplianceResultsManager.Instance == null || variableStorage == null) return;

        var results = ComplianceResultsManager.Instance.EvaluateAll();
        var result = results.Find(r => r.interviewID == interviewID);

        if (result != null)
        {
            variableStorage.SetValue("$subjectScore", result.score);
            variableStorage.SetValue("$subjectMax", result.maxScore);

            Debug.Log($"Subject {interviewID} score: {result.score}/{result.maxScore}");
        }
        else
        {
            variableStorage.SetValue("$subjectScore", 0);
            variableStorage.SetValue("$subjectMax", 0);

            Debug.LogWarning($"No result found for {interviewID}");
        }
    }

    [YarnCommand("get_subject_breakdown")]
    public void GetSubjectBreakdown(string interviewID)
    {
        if (ComplianceResultsManager.Instance == null || variableStorage == null) return;

        string targetID = interviewID.Trim();
        var results = ComplianceResultsManager.Instance.EvaluateAll();
        var result = results.Find(r => r.interviewID.Trim() == targetID);

        if (result != null)
        {
            variableStorage.SetValue("$nameCorrect", result.nameCorrect);
            variableStorage.SetValue("$occupationCorrect", result.occupationCorrect);
            variableStorage.SetValue("$loyaltyCorrect", result.loyaltyCorrect);

            variableStorage.SetValue("$nameEmpty", string.IsNullOrEmpty(result.formData.subjectName));
            variableStorage.SetValue("$occupationEmpty", string.IsNullOrEmpty(result.formData.occupation));
            variableStorage.SetValue("$loyaltyEmpty", result.formData.loyaltyRating == 0);

            variableStorage.SetValue("$subjectScore", (float)result.score);
            variableStorage.SetValue("$subjectMax", (float)result.maxScore);

            Debug.Log($"Breakdown found for {targetID}: Name:{result.nameCorrect}, Score:{result.score}");
        }
        else
        {
            Debug.LogWarning($"Yarn requested breakdown for '{targetID}', but no submitted form matches that ID.");
            variableStorage.SetValue("$nameCorrect", false);
            variableStorage.SetValue("$occupationCorrect", false);
            variableStorage.SetValue("$loyaltyCorrect", false);

            variableStorage.SetValue("$nameEmpty", true);
            variableStorage.SetValue("$occupationEmpty", true);
            variableStorage.SetValue("$loyaltyEmpty", true);

            variableStorage.SetValue("$subjectScore", 0f);
            variableStorage.SetValue("$subjectMax", 0f);
        }
    }

    [YarnCommand("end_day")]
    public void EndDay(string message = "end")
    {
        PauseMenuUI pause = FindObjectOfType<PauseMenuUI>();

        if (pause == null)
        {
            Debug.LogWarning("PauseMenuUI not found.");
            return;
        }

        pause.ShowEndScreen(message);
    }

    // =========================================================
    // ================= DEBUG / UTILITIES =====================
    // =========================================================

    [YarnCommand("print_variable")]
    public void PrintVariable(string variableName)
    {
        if (variableStorage != null &&
            variableStorage.TryGetValue(variableName, out object value))
        {
            Debug.Log($"{variableName} = {value}");
        }
        else
        {
            Debug.LogWarning($"Variable {variableName} not found.");
        }
    }
}