using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IDCardUI : MonoBehaviour
{
    [Header("Stat Displays")]
    [SerializeField] private TMP_Text empathyText;
    [SerializeField] private TMP_Text forceText;
    [SerializeField] private TMP_Text insightText;

    [Header("Root")]
    [SerializeField] private GameObject cardRoot;

    [Header("Toggle")]
    [SerializeField] private Button toggleButton;

    private bool FinishedCharacterCreation = false;

    private void Start()
    {
        // Start hidden
        if (cardRoot != null)
            cardRoot.SetActive(false);

        // Hook up button
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleCard);
    }

    // --- Removed Update() TAB input entirely ---

    public void ToggleCard()
    {
        if (cardRoot == null) return;

        bool show = !cardRoot.activeSelf;
        cardRoot.SetActive(show);

        if (show)
            Refresh();

        // Match book behavior: clear UI selection
        EventSystem.current?.SetSelectedGameObject(null);
    }

    public void ShowCard()
    {
        if (cardRoot == null) return;

        cardRoot.SetActive(true);
        Refresh();
    }

    public void HideCard()
    {
        if (cardRoot == null) return;

        cardRoot.SetActive(false);
    }

    public void Refresh()
    {
        if (PlayerManager.Instance == null) return;

        empathyText.text = PlayerManager.Instance.Empathy.ToString();
        forceText.text = PlayerManager.Instance.Force.ToString();
        insightText.text = PlayerManager.Instance.Insight.ToString();
    }

    // Called after character creation completes (if needed later)
    public void FinishCharacterCreation()
    {
        FinishedCharacterCreation = true;
    }
}