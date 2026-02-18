using UnityEngine;

public class SkillCheckUI : MonoBehaviour
{
    public string StatName { get; private set; }
    public int Difficulty { get; private set; }

    public void Configure(string statName, int difficulty)
    {
        StatName = statName;
        Difficulty = difficulty;

        Debug.Log($"SkillCheck Parsed -> {StatName} vs {Difficulty}");

        // Later:
        // Update tooltip text
        // Calculate success chance
        // Change color
    }
}
