using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Yarn.Unity;

[RequireComponent(typeof(OptionItem))]
public class SkillCheckUI : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Skill Highlight Colors")]
    [SerializeField] private Color empathyHighlight = new Color(0.4f, 0.9f, 1f);
    [SerializeField] private Color willpowerHighlight = new Color(1f, 0.4f, 0.4f);
    [SerializeField] private Color insightHighlight = new Color(0.8f, 0.6f, 1f);

    [Header("Dim Strength")]
    [Range(0f, 1f)]
    [SerializeField] private float dimMultiplier = 0.6f;

    private OptionItem optionItem;
    private TextMeshProUGUI text;

    public string StatName { get; private set; }
    public int Difficulty { get; private set; }

    private void Awake()
    {
        optionItem = GetComponent<OptionItem>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Configure(string statName, int difficulty)
    {
        StatName = statName;
        Difficulty = difficulty;

        ApplyBaseStyle();
    }

    private void ApplyBaseStyle()
    {
        if (optionItem == null || text == null || optionItem.Option == null)
            return;

        string baseText = optionItem.Option.Line.TextWithoutCharacterName.Text;

        string difficultyLabel = GetDifficultyLabel(Difficulty);
        string prefix = $"[{StatName} - {difficultyLabel} {Difficulty}] ";

        text.text = prefix + baseText;

        // Apply dimmed color by default
        text.color = GetDimmedColor();
    }

    // Called when highlighted (hovered / keyboard selected)
    public void OnSelect(BaseEventData eventData)
    {
        ApplyHighlightColor();
    }

    // Called when unhighlighted
    public void OnDeselect(BaseEventData eventData)
    {
        ApplyBaseStyle();
    }

    private void ApplyHighlightColor()
    {
        if (text != null)
            text.color = GetHighlightColor();
    }

    private Color GetDimmedColor()
    {
        Color c = GetHighlightColor();
        return new Color(c.r * dimMultiplier, c.g * dimMultiplier, c.b * dimMultiplier, c.a);
    }

    private string GetDifficultyLabel(int difficulty)
    {
        if (difficulty <= 5) return "Easy";
        if (difficulty <= 10) return "Medium";
        if (difficulty <= 15) return "Hard";
        return "Extreme";
    }

    private Color GetHighlightColor()
    {
        switch (StatName.ToLower())
        {
            case "empathy": return empathyHighlight;
            case "willpower": return willpowerHighlight;
            case "insight": return insightHighlight;
            default: return Color.white;
        }
    }
}