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
        if (dialogueRunner == null)
            dialogueRunner = FindObjectOfType<DialogueRunner>();

        if (variableStorage == null && dialogueRunner != null)
            variableStorage = dialogueRunner.VariableStorage;
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
     * <<SkillCheck "Intimidation" 12 "Intimidation_Pass" "Intimidation_Fail">>
     */
    [YarnCommand("SkillCheck")]
    public void SkillCheck(string statName, int difficulty, string passNode, string failNode)
    {
        if (playerManager == null)
        {
            Debug.LogError("YarnManager: PlayerManager reference missing!");
            return;
        }

        int statValue = playerManager.GetStat(statName);
        int roll = UnityEngine.Random.Range(minRoll, maxRoll + 1);
        int total = roll + statValue;

        Debug.Log($"SkillCheck: {statName} | Roll: {roll} + Stat: {statValue} = {total} vs DC {difficulty}");

        bool success = total >= difficulty;

        // Optional: store results in Yarn variables
        SetFloat("$lastRoll", roll);
        SetFloat("$lastTotal", total);
        SetBool("$lastCheckSuccess", success);

        // Branch dialogue
        if (!string.IsNullOrEmpty(passNode) && !string.IsNullOrEmpty(failNode))
        {
            dialogueRunner.StartDialogue(success ? passNode : failNode);
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