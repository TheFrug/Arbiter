using UnityEngine;
using Yarn.Unity;
using System;

/// <summary>
/// YarnManager
/// Central bridge between Yarn Spinner 3.0 and Unity systems.
/// Handles:
/// - Command routing
/// - Skill checks
/// - Variable access
/// - Dialogue branching
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

    public void RegisterSceneDependencies(
        DialogueRunner runner,
        VariableStorageBehaviour storage,
        PlayerManager player,
        ComplianceForm compliance)
    {
        dialogueRunner = runner;
        variableStorage = storage;
        playerManager = player;
        playerManager = FindObjectOfType<PlayerManager>();
        complianceForm = compliance;
    }

    #endregion

    // =========================================================
    // ================= COMPLIANCE FORM =======================
    // =========================================================

    /*
     * Yarn Usage:
     * <<set_name YarnManager "John Halberd">>
     */
    [YarnCommand("form_set_name")]
    public void SetName(string name)
    {
        complianceForm.RevealNameFromYarn(name);
    }

    /*
     * Yarn Usage:
     * <<set_occupation YarnManager "Mechanic">>
     */
    [YarnCommand("form_set_occupation")]
    public void SetOccupation(string occupation)
    {
        complianceForm.RevealOccupationFromYarn(occupation);
    }

    /*
     * Yarn Usage:
     * <<submit_form YarnManager>>
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
     * <<reset_form YarnManager>>
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
            $"{result.Die1}+{result.Die2} + {statValue} = {result.FinalTotal} " +
            $"vs {difficulty} | Success: {result.Success}"
        );

        dialogueRunner.VariableStorage.SetValue("$last_check_roll", result.DiceTotal);
        dialogueRunner.VariableStorage.SetValue("$last_check_total", result.FinalTotal);
        dialogueRunner.VariableStorage.SetValue("$last_check_stat", result.StatValue);
        dialogueRunner.VariableStorage.SetValue("$last_check_difficulty", result.Difficulty);
        dialogueRunner.VariableStorage.SetValue("$last_check_success", result.Success);
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }

    // ID Card Control
    [YarnCommand("show_id")]
    public void ShowID()
    {
        FindObjectOfType<IDCardUI>()?.ShowCard();
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