using UnityEngine;

public class SkillCheckResolver : MonoBehaviour
{
    public static SkillCheckResolver Instance;

    private void Awake()
    {
        Instance = this;
    }

    // ===============================
    // ===== ACTUAL ROLL LOGIC =======
    // ===============================

    public SkillCheckResult RollSkillCheck(int statValue, int difficulty)
    {
        int die1 = Random.Range(1, 7);
        int die2 = Random.Range(1, 7);
        int diceTotal = die1 + die2;

        bool autoFail = diceTotal == 2;
        bool autoSuccess = diceTotal == 12;

        int total = diceTotal + statValue;

        bool success;

        if (autoFail)
            success = false;
        else if (autoSuccess)
            success = true;
        else
            success = total >= difficulty;

        return new SkillCheckResult
        {
            Die1 = die1,
            Die2 = die2,
            DiceTotal = diceTotal,
            StatValue = statValue,
            FinalTotal = total,
            Difficulty = difficulty,
            Success = success,
            AutoFail = autoFail,
            AutoSuccess = autoSuccess
        };
    }

    // ===================================
    // ===== TOOLTIP PROBABILITY =========
    // ===================================

    public float CalculateChance01(int statValue, int difficulty)
    {
        int successfulOutcomes = 0;
        int totalOutcomes = 36;

        for (int d1 = 1; d1 <= 6; d1++)
        {
            for (int d2 = 1; d2 <= 6; d2++)
            {
                int dice = d1 + d2;

                if (dice == 2)
                    continue; // auto fail

                if (dice == 12)
                {
                    successfulOutcomes++;
                    continue;
                }

                if ((dice + statValue) >= difficulty)
                    successfulOutcomes++;
            }
        }

        return successfulOutcomes / 36f;
    }
}

public struct SkillCheckResult
{
    public int Die1;
    public int Die2;
    public int DiceTotal;
    public int StatValue;
    public int FinalTotal;
    public int Difficulty;
    public bool Success;
    public bool AutoFail;
    public bool AutoSuccess;
}