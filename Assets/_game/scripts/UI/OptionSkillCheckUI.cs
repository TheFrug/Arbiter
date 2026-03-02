using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Yarn.Unity;

[RequireComponent(typeof(OptionItem))]

public class OptionSkillCheckUI : MonoBehaviour,
    ISelectHandler,
    IDeselectHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Skill Highlight Colors")]
    [SerializeField] private Color empathyHighlight = new Color(0.8f, 0.6f, 1f);
    [SerializeField] private Color forceHighlight = new Color(1f, 0.4f, 0.4f);
    [SerializeField] private Color insightHighlight = new Color(0.4f, 0.9f, 1f);

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

    // ==============================
    // VISUAL STYLE
    // ==============================

    private void ApplyBaseStyle()
    {
        if (optionItem == null || text == null || optionItem.Option == null)
            return;

        string baseText = optionItem.Option.Line.TextWithoutCharacterName.Text;

        string difficultyLabel = GetDifficultyLabel(Difficulty);
        string prefix = $"[{StatName} - {difficultyLabel} {Difficulty}] ";

        text.text = prefix + baseText;
        text.color = GetDimmedColor();
    }

    private void ApplyHighlightColor()
    {
        if (text != null)
            text.color = GetHighlightColor();
    }

    public void OnSelect(BaseEventData eventData)
    {
        ApplyHighlightColor();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ApplyBaseStyle();
    }

    // ==============================
    // TOOLTIP EVENTS
    // ==============================

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Enter Skill Option");

        int statValue = FindObjectOfType<PlayerManager>()
            .GetStat(StatName);

        float chance = SkillCheckResolver.Instance
            .CalculateChance01(statValue, Difficulty);

        SkillCheckTooltipPanel.Instance.Show(
            StatName,
            Difficulty,
            chance
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer Exit Skill Option");

        SkillCheckTooltipPanel.Instance.Hide();
    }

    // ==============================

    private string GetDifficultyLabel(int difficulty)
    {
        if (difficulty <= 5) return "Easy";
        if (difficulty <= 10) return "Medium";
        if (difficulty <= 15) return "Hard";
        return "Extreme";
    }

    private Color GetDimmedColor()
    {
        Color c = GetHighlightColor();
        return new Color(c.r * dimMultiplier, c.g * dimMultiplier, c.b * dimMultiplier, c.a);
    }

    private Color GetHighlightColor()
    {
        switch (StatName.ToLower())
        {
            case "empathy": return empathyHighlight;
            case "force": return forceHighlight;
            case "insight": return insightHighlight;
            default: return Color.white;
        }
    }
}