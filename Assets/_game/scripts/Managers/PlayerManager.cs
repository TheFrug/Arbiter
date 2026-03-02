using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public enum PlayerStat
    {
        Empathy,
        Force,
        Insight
    }

    [Header("Core Stats")]
    [SerializeField] private int empathy = 5;
    [SerializeField] private int force = 5;
    [SerializeField] private int insight = 5;

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Optional:
        // DontDestroyOnLoad(gameObject);
    }

    #endregion

    // =========================================================
    // ================= STRING LOOKUP (YARN) ==================
    // =========================================================

    /// <summary>
    /// Used by YarnManager for skill checks.
    /// Case-insensitive.
    /// </summary>
    public int GetStat(string statName)
    {
        if (string.IsNullOrEmpty(statName))
        {
            Debug.LogWarning("PlayerManager: GetStat called with null/empty string.");
            return 0;
        }

        switch (statName.ToLower())
        {
            case "empathy":
                return empathy;

            case "force":
                return force;

            case "insight":
                return insight;

            default:
                Debug.LogWarning($"PlayerManager: Unknown stat '{statName}'");
                return 0;
        }
    }

    // =========================================================
    // ================= ENUM VERSION (SAFER) ==================
    // =========================================================

    public int GetStat(PlayerStat stat)
    {
        switch (stat)
        {
            case PlayerStat.Empathy:
                return empathy;

            case PlayerStat.Force:
                return force;

            case PlayerStat.Insight:
                return insight;

            default:
                return 0;
        }
    }

    // =========================================================
    // ================= MODIFICATION METHODS ==================
    // =========================================================

    public void ModifyStat(string statName, int amount)
    {
        switch (statName.ToLower())
        {
            case "empathy":
                empathy += amount;
                break;

            case "force":
                force += amount;
                break;

            case "insight":
                insight += amount;
                break;

            default:
                Debug.LogWarning($"PlayerManager: Cannot modify unknown stat '{statName}'");
                break;
        }
    }

    public void SetStat(string statName, int value)
    {
        switch (statName.ToLower())
        {
            case "empathy":
                empathy = value;
                break;

            case "force":
                force = value;
                break;

            case "insight":
                insight = value;
                break;

            default:
                Debug.LogWarning($"PlayerManager: Cannot set unknown stat '{statName}'");
                break;
        }
    }

    // =========================================================
    // ================= DEBUG ACCESSORS =======================
    // =========================================================

    public int Empathy => empathy;
    public int Force => force;
    public int Insight => insight;
}
