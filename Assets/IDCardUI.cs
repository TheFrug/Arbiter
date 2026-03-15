using UnityEngine;
using TMPro;

public class IDCardUI : MonoBehaviour
{
    [Header("Stat Displays")]
    [SerializeField] private TMP_Text empathyText;
    [SerializeField] private TMP_Text forceText;
    [SerializeField] private TMP_Text insightText;

    [Header("Root")]
    [SerializeField] private GameObject cardRoot;

    private bool FinishedCharacterCreation = false;

    private void Start()
    {
        if (cardRoot != null)
            cardRoot.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCard();
        }
    }

    public void ToggleCard()
    {
        if (FinishedCharacterCreation)
        {
            bool show = !cardRoot.activeSelf;
            cardRoot.SetActive(show);

            if (show)
                Refresh();
        }
    }

    public void ShowCard()
    {
        cardRoot.SetActive(true);
        Refresh();
    }

    public void HideCard()
    {
        cardRoot.SetActive(false);
    }

    public void Refresh()
    {
        if (PlayerManager.Instance == null)
            return;

        empathyText.text = PlayerManager.Instance.Empathy.ToString();
        forceText.text = PlayerManager.Instance.Force.ToString();
        insightText.text = PlayerManager.Instance.Insight.ToString();
    }

    //Set bool
    public void FinishCharacterCreation()
    {
        FinishedCharacterCreation = true;
    }
}