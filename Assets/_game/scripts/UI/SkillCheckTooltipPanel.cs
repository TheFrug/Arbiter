using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    [SerializeField] private Vector2 screenPadding = new Vector2(20f, 20f);

    private CanvasGroup canvasGroup;
    private bool isVisible;

    private void Awake()
    {
        Instance = this;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // CRITICAL: never block raycasts
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        HideImmediate();
    }

    private void Update()
    {
        if (!isVisible)
            return;

        FollowMouse();
    }

    // =========================
    // SHOW / HIDE
    // =========================

    public void Show(string statName, int threshold, float successChance01)
    {
        if (isVisible) return; // prevents redundant re-show flicker

        isVisible = true;
        gameObject.SetActive(true);

        skillLineText.text = $"{statName}: {threshold}";
        chanceText.text = $"{Mathf.RoundToInt(successChance01 * 100f)}%";

        difficultyText.text = GetDifficultyRating(successChance01);
        difficultyText.color = GetDifficultyColor(successChance01);

        FollowMouse();
    }

    public void Hide()
    {
        if (!isVisible) return;

        isVisible = false;
        gameObject.SetActive(false);
    }

    private void HideImmediate()
    {
        isVisible = false;
        gameObject.SetActive(false);
    }

    // =========================
    // POSITIONING (NO JITTER)
    // =========================

    private void FollowMouse()
    {
        Vector2 mouse = Input.mousePosition;

        float width = panelRoot.rect.width;
        float height = panelRoot.rect.height;

        // Anchor tooltip so mouse is at top-right corner
        Vector2 position = new Vector2(
            mouse.x - width,
            mouse.y
        );

        // ----- SCREEN CLAMP -----

        float minX = screenPadding.x;
        float maxX = Screen.width - width - screenPadding.x;

        float minY = height + screenPadding.y; // bottom bound (because pivot top)
        float maxY = Screen.height - screenPadding.y;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        panelRoot.position = position;
    }

    // =========================
    // DIFFICULTY VISUALS
    // =========================

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