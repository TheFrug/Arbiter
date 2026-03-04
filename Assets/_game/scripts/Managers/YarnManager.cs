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
    [SerializeField] private InfractionForm infractionForm;
    [SerializeField] private PlayerManager playerManager;

    [Header("Skill Check Settings")]
    [SerializeField] private int minRoll = 1;
    [SerializeField] private int maxRoll = 20;

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
        InfractionForm form,
        PlayerManager player)
    {
        dialogueRunner = runner;
        variableStorage = storage;
        infractionForm = form;
        playerManager = FindObjectOfType<PlayerManager>();
    }
    #endregion

    // =========================================================
    // =============== BASIC COMMAND EXAMPLE ===================
    // =========================================================

    /// Usage in Yarn:
    /// <<RevealName "Darion Vale">>
    [YarnCommand("RevealName")]
    public void RevealName(string name)
    {
        if (infractionForm == null)
        {
            Debug.LogWarning("YarnManager: InfractionForm reference missing!");
            return;
        }

        infractionForm.RevealNameFromYarn(name);
    }

    // =========================================================
    // ==================== SKILL CHECK ========================
    // =========================================================

    /*
     * Usage in Yarn:
     * <<SkillCheck "YarnManager" "Force" 12 >>
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

        // Store everything Yarn might need
        dialogueRunner.VariableStorage.SetValue("$lastRoll", result.DiceTotal);
        dialogueRunner.VariableStorage.SetValue("$lastTotal", result.FinalTotal);
        dialogueRunner.VariableStorage.SetValue("$lastRollStat", result.StatValue);
        dialogueRunner.VariableStorage.SetValue("$lastRollDifficulty", result.Difficulty);
        dialogueRunner.VariableStorage.SetValue("$lastCheckSuccess", result.Success);
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

    [YarnCommand("AddStat")]
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

    [YarnCommand("SetStat")]
    public void SetStat(string statName, int value)
    {
        if (playerManager == null)
            return;

        playerManager.SetStat(statName, value);
    }

    [YarnCommand("FinalizeCharacter")]
    public void FinalizeCharacter(string nextScene)
    {
        Debug.Log("Character creation finalized.");
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }

    // =========================================================
    // =============== OPTIONAL: INLINE FUNCTION ===============
    // =========================================================

    /*
     * Usage in Yarn:
     * <<if RollStat($intimidation, 12) >= 1>>
     *     You succeed.
     * <<else>>
     *     You fail.
     * <<endif>>
     */
    [YarnFunction("RollStat")]
    public static int RollStat(float statValue, float difficulty)
    {
        int roll = UnityEngine.Random.Range(1, 21);
        return (roll + statValue) >= difficulty ? 1 : 0;
    }

    // =========================================================
    // ================= DEBUG / UTILITIES =====================
    // =========================================================

    [YarnCommand("PrintVariable")]
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