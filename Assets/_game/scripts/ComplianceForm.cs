using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ComplianceForm : MonoBehaviour
{
    #region Data Struct

    [Serializable]
    public struct ComplianceFormData
    {
        public string interviewID;
        public string subjectName;
        public string occupation;
        public int loyaltyRating;
        public bool[] behaviorFlags;
    }

    #endregion

    #region Inspector References

    [Header("Header")]
    [SerializeField] private string interviewID;
    [SerializeField] private string interviewDate;

    [Header("Header UI")]
    [SerializeField] private TMP_Text interviewIDText;
    [SerializeField] private TMP_Text interviewDateText;

    [Header("Identity Fields")]
    [SerializeField] private Button nameFillButton;
    [SerializeField] private TMP_Text nameDisplayText;

    [SerializeField] private Button occupationFillButton;
    [SerializeField] private TMP_Text occupationDisplayText;

    [Header("Loyalty Rating (Toggle Group)")]
    [SerializeField] private Toggle loyaltyLow;
    [SerializeField] private Toggle loyaltyMid;
    [SerializeField] private Toggle loyaltyHigh;

    [Header("Behavior Flags")]
    [SerializeField] private Toggle[] behaviorToggles;

    [Header("Form Toggle")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float slideSpeed = 6f;
    [SerializeField] private Vector2 hiddenPosition;
    [SerializeField] private Vector2 visiblePosition;

    [Header("Reveal Flash")]
    [SerializeField] private Color flashColor = new Color(1f, 0.9f, 0.2f);
    [SerializeField] private int flashCount = 3;
    [SerializeField] private float flashDuration = 0.15f;

    #endregion

    #region Internal State

    private string revealedName = null;
    private string revealedOccupation = null;

    private string subjectName = null;
    private string occupation = null;

    private bool isVisible = false;
    private bool isSliding = false;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        nameFillButton.interactable = false;
        occupationFillButton.interactable = false;

        nameFillButton.onClick.AddListener(OnNameClicked);
        occupationFillButton.onClick.AddListener(OnOccupationClicked);

        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleForm);

        // Add listeners to loyalty toggles
        if (loyaltyLow != null) loyaltyLow.onValueChanged.AddListener((bool isOn) => OnCheckboxToggled(isOn));
        if (loyaltyMid != null) loyaltyMid.onValueChanged.AddListener((bool isOn) => OnCheckboxToggled(isOn));
        if (loyaltyHigh != null) loyaltyHigh.onValueChanged.AddListener((bool isOn) => OnCheckboxToggled(isOn));

        // Add listeners to behavior toggles
        for (int i = 0; i < behaviorToggles.Length; i++)
        {
            int index = i;
            behaviorToggles[index].onValueChanged.AddListener((bool isOn) => OnCheckboxToggled(isOn));
        }
    }

    private void Start()
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition = hiddenPosition;

        if (interviewIDText != null)
            interviewIDText.text = interviewID;

        if (interviewDateText != null)
            interviewDateText.text = interviewDate;
    }

    #endregion

    #region Yarn Integration
    public string GetFilledName() => subjectName;
    public string GetFilledOccupation() => occupation;
    public void RevealNameFromYarn(string name)
    {
        revealedName = name;

        if (string.IsNullOrEmpty(subjectName))
        {
            nameFillButton.interactable = true;
            StartCoroutine(FlashButton(toggleButton));
        }
    }

    public void RevealOccupationFromYarn(string occ)
    {
        revealedOccupation = occ;

        if (string.IsNullOrEmpty(occupation))
        {
            occupationFillButton.interactable = true;
            StartCoroutine(FlashButton(toggleButton));
        }
    }

    #endregion

    #region Header Setters

    public void SetInterviewID(string id)
    {
        interviewID = id;

        if (interviewIDText != null)
            interviewIDText.text = id;
    }

    public void SetInterviewDate(string date)
    {
        interviewDate = date;

        if (interviewDateText != null)
            interviewDateText.text = date;
    }

    #endregion

    #region Button Logic

    [Header("Confirmation UI")]
    [SerializeField] private GameObject confirmationPrompt; // Assign in Inspector
    private Action pendingConfirmationAction;

    // The Yarn script can check this property
    public bool IsFormComplete
    {
        get
        {
            // Checks if strings are filled and a loyalty toggle is selected
            bool identityFilled = !string.IsNullOrEmpty(subjectName) && !string.IsNullOrEmpty(occupation);
            bool loyaltySelected = (loyaltyLow != null && loyaltyLow.isOn) ||
                                   (loyaltyMid != null && loyaltyMid.isOn) ||
                                   (loyaltyHigh != null && loyaltyHigh.isOn);

            return identityFilled && loyaltySelected;
        }
    }

    // Updated button listeners to trigger "Are you sure?"
    private void OnNameClicked()
    {
        if (string.IsNullOrEmpty(revealedName)) return;

        ShowConfirmation(() => {
            subjectName = revealedName;
            nameDisplayText.text = subjectName;
            nameFillButton.interactable = false;

            // Audio trigger for writing out the name
            AudioManager.Instance.PlayRandomSoundEffect("WriteSounds");
        });
    }

    private void OnOccupationClicked()
    {
        if (string.IsNullOrEmpty(revealedOccupation)) return;

        ShowConfirmation(() => {
            occupation = revealedOccupation;
            occupationDisplayText.text = occupation;
            occupationFillButton.interactable = false;

            // Audio trigger for writing out the occupation
            AudioManager.Instance.PlayRandomSoundEffect("WriteSounds");
        });
    }

    // Audio trigger modified to only play when checked/toggled on
    private void OnCheckboxToggled(bool isOn)
    {
        if (isOn)
        {
            AudioManager.Instance.PlayRandomSoundEffect("CheckboxSounds");
        }
    }

    private void ShowConfirmation(Action onConfirm)
    {
        pendingConfirmationAction = onConfirm;
        if (confirmationPrompt != null)
        {
            confirmationPrompt.SetActive(true);
            // Force the UI to focus on the popup
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Hook these to the buttons on your "Are you sure?" UI object
    public void ConfirmAction()
    {
        pendingConfirmationAction?.Invoke();
        pendingConfirmationAction = null;
        if (confirmationPrompt != null) confirmationPrompt.SetActive(false);
    }

    public void CancelAction()
    {
        Debug.Log("Cancel button was pressed!");
        pendingConfirmationAction = null;

        if (confirmationPrompt != null)
        {
            confirmationPrompt.SetActive(false);
        }
        else
        {
            // FAILSAFE: If the inspector reference is lost, 
            // try to find the object by name or tag as a backup.
            Debug.LogError("CancelAction called, but confirmationPrompt is missing! Attempting failsafe...");

            // This looks for the object in the children of the object this script is on
            var backup = transform.Find("ConfirmationPopup");
            if (backup != null)
            {
                backup.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Submission

    public event Action<ComplianceFormData> OnFormSubmitted;

    public void SubmitForm()
    {
        int loyaltyValue = 0;

        if (loyaltyLow != null && loyaltyLow.isOn) loyaltyValue = 1;
        else if (loyaltyMid != null && loyaltyMid.isOn) loyaltyValue = 2;
        else if (loyaltyHigh != null && loyaltyHigh.isOn) loyaltyValue = 3;

        bool[] flags = new bool[behaviorToggles.Length];

        for (int i = 0; i < behaviorToggles.Length; i++)
        {
            flags[i] = behaviorToggles[i].isOn;
        }

        ComplianceFormData data = new ComplianceFormData
        {
            interviewID = interviewID,
            subjectName = subjectName,
            occupation = occupation,
            loyaltyRating = loyaltyValue, // Use existing parameter layout
            behaviorFlags = flags
        };

        OnFormSubmitted?.Invoke(data);
        ComplianceResultsManager.Instance?.RegisterForm(data);
        int count = ComplianceResultsManager.Instance.GetAllForms().Count;
        //variableStorage.SetValue("$submittedFormsCount", count);
    }

    #endregion

    #region Slide Logic

    public void ToggleForm()
    {
        if (isSliding) return;

        StartCoroutine(SlideForm(!isVisible));

        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator SlideForm(bool show)
    {
        isSliding = true;

        // Play the paper sound effect whenever the form is toggled
        AudioManager.Instance.PlayRandomSoundEffect("PaperSounds");

        Vector2 start = rectTransform.anchoredPosition;
        Vector2 target = show ? visiblePosition : hiddenPosition;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;
            rectTransform.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        rectTransform.anchoredPosition = target;
        isVisible = show;
        isSliding = false;
    }

    #endregion

    #region Reset

    public void ResetForm()
    {
        revealedName = null;
        revealedOccupation = null;

        subjectName = null;
        occupation = null;

        nameDisplayText.text = "";
        occupationDisplayText.text = "";

        nameFillButton.interactable = false;
        occupationFillButton.interactable = false;

        if (loyaltyLow != null) loyaltyLow.isOn = false;
        if (loyaltyMid != null) loyaltyMid.isOn = false;
        if (loyaltyHigh != null) loyaltyHigh.isOn = false;

        if (behaviorToggles != null)
        {
            foreach (var toggle in behaviorToggles)
                toggle.isOn = false;
        }
    }

    #endregion

    private IEnumerator FlashButton(Button button)
    {
        Image img = button.GetComponent<Image>();
        if (img == null)
            yield break;

        Color original = img.color;

        for (int i = 0; i < flashCount; i++)
        {
            // Fade to flash color
            float t = 0f;
            while (t < flashDuration)
            {
                t += Time.deltaTime;
                float lerp = Mathf.SmoothStep(0f, 1f, t / flashDuration);
                img.color = Color.Lerp(original, flashColor, lerp);
                yield return null;
            }

            // If this is the LAST flash
            if (i == flashCount - 1)
            {
                // Hold bright for 2 seconds
                yield return new WaitForSeconds(2f);

                // Slow fade back to original
                float slowFade = flashDuration * 3f;
                t = 0f;

                while (t < slowFade)
                {
                    t += Time.deltaTime;
                    float lerp = Mathf.SmoothStep(0f, 1f, t / slowFade);
                    img.color = Color.Lerp(flashColor, original, lerp);
                    yield return null;
                }

                break;
            }

            // Normal fade back for early flashes
            t = 0f;
            while (t < flashDuration)
            {
                t += Time.deltaTime;
                float lerp = Mathf.SmoothStep(0f, 1f, t / flashDuration);
                img.color = Color.Lerp(flashColor, original, lerp);
                yield return null;
            }
        }

        img.color = original;
    }
}