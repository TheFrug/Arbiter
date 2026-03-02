using UnityEngine;
using TMPro;

public class SkillCheckTooltipPanel : MonoBehaviour
{
    public static SkillCheckTooltipPanel Instance;

    [Header("UI References")]
    [SerializeField] private RectTransform panelRoot;
    [SerializeField] private TextMeshProUGUI skillLineText;
    [SerializeField] private TextMeshProUGUI chanceText;
    [SerializeField] private TextMeshProUGUI difficultyText;

    [Header("Difficulty Colors")]
    [SerializeField] private Color lowColor = new Color(0.9f, 0.2f, 0.2f);
    [SerializeField] private Color evenColor = new Color(1f, 0.8f, 0.2f);
    [SerializeField] private Color highColor = new Color(0.2f, 0.9f, 0.2f);

    [Header("Follow Settings")]
    [SerializeField] private Vector2 screenOffset = new Vector2(20f, -20f);

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
            return;

        Vector3 mousePos = Input.mousePosition;
        Vector3 offset = new Vector3(screenOffset.x, screenOffset.y, 0f);

        panelRoot.position = mousePos + offset;
    }

    // PURE VISUAL INPUT
    public void Show(
        string statName,
        int threshold,
        float successChance01 // pass in 0.0 - 1.0
    )
    {
        gameObject.SetActive(true);

        skillLineText.text = $"{statName}: {threshold}";
        chanceText.text = $"Success Chance: {Mathf.RoundToInt(successChance01 * 100f)}%";

        string rating = GetDifficultyRating(successChance01);
        difficultyText.text = rating;
        difficultyText.color = GetDifficultyColor(successChance01);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private string GetDifficultyRating(float chance)
    {
        if (chance < 0.4f) return "LOW";
        if (chance < 0.7f) return "EVEN";
        return "HIGH";
    }

    private Color GetDifficultyColor(float chance)
    {
        if (chance < 0.4f) return lowColor;
        if (chance < 0.7f) return evenColor;
        return highColor;
    }
}