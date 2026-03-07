using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;
using UnityEngine.EventSystems;


public class InfractionForm : MonoBehaviour
{
    #region Enums

    public enum ViolationType
    {
        PublicDisorder,
        EconomicNonCompliance,
        IdeologicalDeviance,
        AdministrativeObstruction
    }

    public enum SeverityLevel
    {
        Minor,
        Standard,
        Aggravated
    }

    #endregion

    #region Data Struct

    [Serializable]
    public struct InfractionFormData
    {
        public string caseID;
        public string suspectName;
        public ViolationType violation;
        public SeverityLevel severity;
        public bool stamped;
    }

    #endregion

    #region Inspector References

    [Header("Header Data")]
    [SerializeField] private string caseID;
    [SerializeField] private string ArrestingOfficer;
    [SerializeField] private string CitizenshipTier;

    [Header("Header Data UI")]
    [SerializeField] private TMP_Text caseIDText;
    [SerializeField] private TMP_Text arrestingOfficerText;
    [SerializeField] private TMP_Text citizenshipTierText;

    [Header("Identity")]
    [SerializeField] private Button nameFillButton;
    [SerializeField] private TMP_Text nameDisplayText;

    [Header("Violation Toggles")]
    [SerializeField] private Toggle publicDisorderToggle;
    [SerializeField] private Toggle economicToggle;
    [SerializeField] private Toggle ideologicalToggle;
    [SerializeField] private Toggle administrativeToggle;

    [Header("Severity Toggles")]
    [SerializeField] private Toggle minorToggle;
    [SerializeField] private Toggle standardToggle;
    [SerializeField] private Toggle aggravatedToggle;

    [Header("Stamp")]
    [SerializeField] private Button stampButton;
    [SerializeField] private GameObject stampVisual;

    [Header("Form Toggle")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float slideSpeed = 6f;
    [SerializeField] private Vector2 hiddenPosition;
    [SerializeField] private Vector2 visiblePosition;

    #endregion

    public void SetSuspectData(SuspectData suspect)
    {
        if (suspect == null)
            return;

        caseID = suspect.caseID;
        ArrestingOfficer = suspect.ArrestingOfficer;
        CitizenshipTier = suspect.CitizenshipTier;

        // Write to UI
        if (caseIDText != null)
            caseIDText.text = caseID;

        if (arrestingOfficerText != null)
            arrestingOfficerText.text = ArrestingOfficer;

        if (citizenshipTierText != null)
            citizenshipTierText.text = CitizenshipTier;
    }


    #region Internal State

    private string revealedName = null;
    private string suspectName = null;
    private ViolationType? selectedViolation = null;
    private SeverityLevel? selectedSeverity = null;
    private bool stampApplied = false;

    private bool isVisible = false;
    private bool isSliding = false;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Disable name button initially
        nameFillButton.interactable = false;
        nameDisplayText.text = "";

        nameFillButton.onClick.AddListener(OnNameButtonClicked);

        // Violation listeners
        publicDisorderToggle.onValueChanged.AddListener((val) =>
            OnViolationSelected(val, ViolationType.PublicDisorder));

        economicToggle.onValueChanged.AddListener((val) =>
            OnViolationSelected(val, ViolationType.EconomicNonCompliance));

        ideologicalToggle.onValueChanged.AddListener((val) =>
            OnViolationSelected(val, ViolationType.IdeologicalDeviance));

        administrativeToggle.onValueChanged.AddListener((val) =>
            OnViolationSelected(val, ViolationType.AdministrativeObstruction));

        // Severity listeners
        minorToggle.onValueChanged.AddListener((val) =>
            OnSeveritySelected(val, SeverityLevel.Minor));

        standardToggle.onValueChanged.AddListener((val) =>
            OnSeveritySelected(val, SeverityLevel.Standard));

        aggravatedToggle.onValueChanged.AddListener((val) =>
            OnSeveritySelected(val, SeverityLevel.Aggravated));

        stampButton.onClick.AddListener(OnStampClicked);

        stampButton.interactable = false;

        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleForm);

        if (stampVisual != null)
            stampVisual.SetActive(false);
    }

    private void Start()
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition = hiddenPosition;
    }

    #endregion

    private void UpdateStampButtonState()
    {
        if (stampButton == null)
            return;

        if (stampApplied)
        {
            stampButton.interactable = false;
            return;
        }

        stampButton.interactable = IsFormComplete();
    }


    #region Yarn Integration

    /// <summary>
    /// Called by YarnManager to set the suspect name and enable the button.
    /// </summary>
    public void RevealNameFromYarn(string name)
    {
        revealedName = name;
        nameFillButton.interactable = true;
    }

    #endregion

    #region CaseID
    public void SetCaseID(string id)
    {
        caseID = id;
    }

    #endregion

    private void LockForm()
    {
        nameFillButton.interactable = false;

        SetFormInteractable(false);

        stampButton.interactable = false;
    }

    #region Name Logic

    private void OnNameButtonClicked()
    {
        if (string.IsNullOrEmpty(revealedName))
            return;

        // Fill the TMP_Text with the revealed name
        suspectName = revealedName;
        nameDisplayText.text = revealedName;

        // Disable the button so it can't be clicked again
        nameFillButton.interactable = false;

        UpdateStampButtonState();
    }

    #endregion

    #region Violation & Severity

    private void OnViolationSelected(bool isOn, ViolationType type)
    {
        if (isOn)
        {
            selectedViolation = type;
        }
        else
        {
            // If this toggle was the selected one, clear it
            if (selectedViolation == type)
                selectedViolation = null;
        }

        UpdateStampButtonState();
    }

    private void OnSeveritySelected(bool isOn, SeverityLevel level)
    {
        if (isOn)
        {
            selectedSeverity = level;
        }
        else
        {
            if (selectedSeverity == level)
                selectedSeverity = null;
        }

        UpdateStampButtonState();
    }

    #endregion

    #region Submission

    public event Action<InfractionFormData> OnFormSubmitted;

    private void OnStampClicked()
    {
        if (!IsFormComplete())
            return;

        stampApplied = true;

        if (stampVisual != null)
            stampVisual.SetActive(true);

        LockForm();

        SubmitForm();
    }

    private bool IsFormComplete()
    {
        return
            !string.IsNullOrEmpty(suspectName) &&
            selectedViolation.HasValue &&
            selectedSeverity.HasValue;
    }

    private void SubmitForm()
    {
        StartCoroutine(SubmitSequence());
    }

    private IEnumerator SubmitSequence()
    {
        InfractionFormData data = new InfractionFormData
        {
            caseID = caseID,
            suspectName = suspectName,
            violation = selectedViolation.Value,
            severity = selectedSeverity.Value,
            stamped = stampApplied
        };

        // Ensure stamp is visible
        if (stampVisual != null)
            stampVisual.SetActive(true);

        yield return new WaitForSeconds(1f);

        ToggleForm();

        //Reset First
        ResetForm();

        OnFormSubmitted?.Invoke(data);

    }


    #endregion

    #region Slide Logic

    public void ToggleForm()
    {
        if (isSliding) return;

        StartCoroutine(SlideForm(!isVisible));

        // Prevent spacebar from re-triggering this button
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator SlideForm(bool show)
    {
        isSliding = true;

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

    private void ResetForm()
    {
        revealedName = null;
        suspectName = null;
        selectedViolation = null;
        selectedSeverity = null;
        stampApplied = false;

        nameDisplayText.text = "";
        nameFillButton.interactable = false;

        if (caseIDText != null)
            caseIDText.text = "";

        if (arrestingOfficerText != null)
            arrestingOfficerText.text = "";

        if (citizenshipTierText != null)
            citizenshipTierText.text = "";

        publicDisorderToggle.isOn = false;
        economicToggle.isOn = false;
        ideologicalToggle.isOn = false;
        administrativeToggle.isOn = false;

        minorToggle.isOn = false;
        standardToggle.isOn = false;
        aggravatedToggle.isOn = false;

        if (stampVisual != null)
            stampVisual.SetActive(false);

        stampButton.interactable = false;

        // Re-enable interaction
        SetFormInteractable(true);

        nameFillButton.interactable = false; // stays disabled until Yarn reveals name
    }

    private void SetFormInteractable(bool state)
    {
        publicDisorderToggle.interactable = state;
        economicToggle.interactable = state;
        ideologicalToggle.interactable = state;
        administrativeToggle.interactable = state;

        minorToggle.interactable = state;
        standardToggle.interactable = state;
        aggravatedToggle.interactable = state;
    }


    #endregion
}
